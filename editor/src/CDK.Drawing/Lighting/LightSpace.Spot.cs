using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public partial class LightSpace
    {
        private List<SpotLight> _spotLights;
        private bool _spotLightUpdated;
        private bool _spotLightVisible;
        private Buffer _spotLightBuffer;
        private Texture _spotLightClusterMap;
        private Buffer _spotShadowBuffer;
        private SpotShadowMap[] _spotShadowMaps;
        private List<int> _spotShadowLightIndices;
        private int _spotShadowCursor;

        private void InitializeSpotLight()
        {
            _spotLights = new List<SpotLight>(MaxSpotLightCount);

            _spotShadowMaps = new SpotShadowMap[MaxSpotShadowCount];
            _spotShadowLightIndices = new List<int>(MaxSpotShadowCount);
            _spotShadowCursor = -1;
        }

        private void DisposeSpotLight()
        {
            _spotLightBuffer?.Dispose();
            _spotLightClusterMap?.Dispose();
            _spotShadowBuffer?.Dispose();
            foreach (var shadowMap in _spotShadowMaps) shadowMap?.Dispose();
        }

        public int SpotLightCount => _spotLights.Count;

        public void SetSpotLight(in SpotLight light)
        {
            Debug.Assert(_updating);

            lock (this)
            {
                _lightUpdateKeys.Add(light.Key);

                for (var i = 0; i < _spotLights.Count; i++)
                {
                    var prev = _spotLights[i];

                    if (prev.Key.Equals(light.Key))
                    {
                        if (prev != light)
                        {
                            _spotLights[i] = light;
                            _spotLightUpdated = true;
                        }
                        return;
                    }
                }
                if (_spotLights.Count < MaxSpotLightCount)
                {
                    _spotLights.Add(light);
                    _spotLightUpdated = true;
                }
            }
        }

        private bool BeginSpotShadow(Graphics graphics)
        {
            var camera = graphics.Camera;

            if (_spotShadowCursor == -1)
            {
                _spotShadowLightIndices.Clear();

                if (AllowShadow)
                {
                    for (var i = 0; i < _spotLights.Count; i++)
                    {
                        var light = _spotLights[i];

                        if (light.CastShadow)
                        {
                            if (_spotShadowLightIndices.Count < MaxSpotShadowCount)
                            {
                                _spotShadowLightIndices.Add(i);
                            }
                            else
                            {
                                var dist = Vector3.Distance(camera.Position, light.Position) - light.Range;

                                var index = -1;
                                for (var j = 0; j < MaxSpotShadowCount; j++)
                                {
                                    var other = _spotLights[_spotShadowLightIndices[j]];

                                    var d = Vector3.Distance(camera.Position, other.Position) - other.Range;

                                    if (dist < d)
                                    {
                                        index = j;
                                        dist = d;
                                    }
                                }
                                if (index != -1) _spotShadowLightIndices[index] = i;
                            }
                        }
                    }
                }
                for (var i = _spotShadowLightIndices.Count; i < MaxSpotShadowCount; i++)
                {
                    _spotShadowMaps[i]?.Dispose();
                    _spotShadowMaps[i] = null;
                }
                if (_spotShadowLightIndices.Count == 0) return false;

                _spotShadowCursor = 0;
            }
            else if (_spotShadowCursor >= _spotShadowLightIndices.Count)
            {
                _spotShadowCursor = -1;
                return false;
            }

            {
                var light = _spotLights[_spotShadowLightIndices[_spotShadowCursor]];
                var range = light.Range;
                if (!AllowShadowPixel32) light.Shadow.Pixel32 = false;
                if (light.Shadow.Resolution > MaxShadowResolution) light.Shadow.Resolution = MaxShadowResolution;
                var maxResolution = range * Math.Tan(light.Angle * 0.5f + light.Dispersion) * 2;
                while ((light.Shadow.Resolution >> 1) >= maxResolution) light.Shadow.Resolution >>= 1;
                if (_spotShadowMaps[_spotShadowCursor] == null) _spotShadowMaps[_spotShadowCursor] = new SpotShadowMap();
                _spotShadowMaps[_spotShadowCursor].Begin(graphics, light, range);
                return true;
            }
        }

        private void EndSpotShadow(Graphics graphics)
        {
            _spotShadowMaps[_spotShadowCursor].End(graphics);

            _spotShadowCursor++;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SpotLightData
        {
            public Vector3 Position;
            public float Cutoff;
            public Vector3 Direction;
            public float Epsilon;
            public Color3 Color;
            public float Pad0;
            public Vector4 Attenuation;
            public int ShadowMapIndex;
            public float ShadowBias;
            public float ShadowBleeding;
            public float Pad1;
        }

        private void UpdateSpotLights(bool viewUpdated)
        {
            if (_spotLightUpdated || viewUpdated)
            {
                if (_spotLights.Count != 0)
                {
                    if (_spotLightBuffer == null) _spotLightBuffer = new Buffer(BufferTarget.UniformBuffer);
                    if (_spotLightClusterMap == null)
                    {
                        _spotLightClusterMap = new Texture(new TextureDescription()
                        {
                            Target = TextureTarget.Texture3D,
                            Width = Cluster,
                            Height = Cluster,
                            Depth = Cluster,
                            Format = RawFormat.Rgba8ui
                        }, false);
                    }
                    var lightData = new SpotLightData[_spotLights.Count];
                    for (var i = 0; i < _spotLights.Count; i++)
                    {
                        var light = _spotLights[i];

                        var halfAngle = light.Angle * 0.5f;
                        var cutoff = (float)Math.Cos(halfAngle + light.Dispersion);
                        var epsilon = (float)Math.Cos(halfAngle) - cutoff;

                        lightData[i] = new SpotLightData
                        {
                            Position = light.Position,
                            Direction = light.Direction,
                            Cutoff = cutoff,
                            Epsilon = epsilon,
                            Color = light.Color,
                            Attenuation = new Vector4(light.Range, light.Attenuation.Constant, light.Attenuation.Linear, light.Attenuation.Quadratic),
                            ShadowMapIndex = light.CastShadow ? _spotShadowLightIndices.IndexOf(i) : -1,
                            ShadowBias = light.Shadow.Bias,
                            ShadowBleeding = light.Shadow.Bleeding
                        };
                    }
                    _spotLightBuffer.Upload(lightData, BufferUsageHint.DynamicDraw);

                    if (_spotShadowLightIndices.Count != 0)
                    {
                        if (_spotShadowBuffer == null) _spotShadowBuffer = new Buffer(BufferTarget.UniformBuffer);
                        _spotShadowBuffer.Upload(_spotShadowMaps.Select(m => m?.ViewProjection ?? Matrix4x4.Identity).ToArray(), BufferUsageHint.DynamicDraw);
                    }
                    else
                    {
                        _spotShadowBuffer?.Dispose();
                        _spotShadowBuffer = null;
                    }

                    UpdateSpotLightCluster();
                }
                else
                {
                    _spotLightBuffer?.Dispose();
                    _spotLightBuffer = null;
                    _spotLightClusterMap?.Dispose();
                    _spotLightClusterMap = null;
                    _spotShadowBuffer?.Dispose();
                    _spotShadowBuffer = null;
                    _spotLightVisible = false;
                }
                _spotLightUpdated = false;
            }
        }

        private bool IntersectSpotLightCluster(in Ray ray, float range, float tanq, int x, int y, int z)
        {
            var wp0 = ClusterToWorld(new Vector3(x, y, z));
            var wp1 = ClusterToWorld(new Vector3(x + 1, y, z));
            var wp2 = ClusterToWorld(new Vector3(x, y + 1, z));
            var wp3 = ClusterToWorld(new Vector3(x + 1, y + 1, z));
            var wp4 = ClusterToWorld(new Vector3(x, y, z + 1));
            var wp5 = ClusterToWorld(new Vector3(x + 1, y, z + 1));
            var wp6 = ClusterToWorld(new Vector3(x, y + 1, z + 1));
            var wp7 = ClusterToWorld(new Vector3(x + 1, y + 1, z + 1));

            float rd;

            if (ray.Intersects(new Triangle(wp0, wp1, wp2), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp1, wp3, wp2), out rd) && rd < range) return true;

            if (ray.Intersects(new Triangle(wp4, wp5, wp6), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp5, wp7, wp6), out rd) && rd < range) return true;

            if (ray.Intersects(new Triangle(wp4, wp0, wp6), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp0, wp2, wp6), out rd) && rd < range) return true;

            if (ray.Intersects(new Triangle(wp4, wp5, wp0), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp5, wp1, wp0), out rd) && rd < range) return true;

            if (ray.Intersects(new Triangle(wp5, wp1, wp7), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp1, wp3, wp7), out rd) && rd < range) return true;

            if (ray.Intersects(new Triangle(wp6, wp7, wp2), out rd) && rd < range) return true;
            if (ray.Intersects(new Triangle(wp7, wp3, wp2), out rd) && rd < range) return true;

            Vector3 n;

            ray.Intersects(new Segment(wp0, wp1), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp0, wp2), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp1, wp3), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp2, wp3), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp4, wp5), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp4, wp6), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp5, wp7), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp6, wp7), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp0, wp4), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp1, wp5), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp2, wp6), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;
            ray.Intersects(new Segment(wp3, wp7), out rd, out n);
            if (rd >= 0 && rd < range && Vector3.Distance(ray.Position + ray.Direction * rd, n) < rd * tanq) return true;

            return false;
        }

        private static Vector3[] SpotAngleAxis = new Vector3[]
        {
            Vector3.UnitX,
            -Vector3.UnitX,
            Vector3.UnitY,
            -Vector3.UnitY,
            Vector3.UnitZ,
            -Vector3.UnitZ
        };

        private void UpdateSpotLightCluster()
        {
            var clusters = new byte[Cluster * Cluster * Cluster * 4];
            var clusterAtts = new float[Cluster * Cluster * Cluster * 3];

            _spotLightVisible = false;

            var planes = new List<Plane>(3);

            for (var li = 0; li < _spotLights.Count; li++)
            {
                var light = _spotLights[li];
                var range = light.Range;
                var angle = light.Angle * 0.5f + light.Dispersion;

                var box = new ABoundingBox(light.Position);

                foreach (var axis in SpotAngleAxis)
                {
                    var rayrot = new Ray(light.Position, Vector3.Transform(light.Direction, Quaternion.CreateFromAxisAngle(axis, angle)));

                    planes.Clear();
                    if (rayrot.Direction.X < 0) planes.Add(new Plane(Vector3.UnitX, -Space.Minimum.X));
                    else if (rayrot.Direction.X > 0) planes.Add(new Plane(-Vector3.UnitX, -Space.Maximum.X));
                    if (rayrot.Direction.Y < 0) planes.Add(new Plane(Vector3.UnitY, -Space.Minimum.Y));
                    else if (rayrot.Direction.Y > 0) planes.Add(new Plane(-Vector3.UnitY, -Space.Maximum.Y));
                    if (rayrot.Direction.Z < 0) planes.Add(new Plane(Vector3.UnitZ, -Space.Minimum.Z));
                    else if (rayrot.Direction.Z > 0) planes.Add(new Plane(-Vector3.UnitZ, -Space.Maximum.Z));

                    var distance = range;
                    foreach (var plane in planes)
                    {
                        if (rayrot.Intersects(plane, out var d) && d < distance) distance = d;
                    }
                    box.Append(rayrot.Position + rayrot.Direction * distance);
                }

                var min = new Vector3(float.MaxValue);
                var max = new Vector3(float.MinValue);

                foreach (var wp in box.GetCorners())
                {
                    var cp = WorldToCluster(wp);
                    min = Vector3.Min(min, cp);
                    max = Vector3.Max(max, cp);
                }
                var minx = Math.Max((int)min.X, 0);
                var maxx = Math.Min((int)max.X, Cluster - 1);
                var miny = Math.Max((int)min.Y, 0);
                var maxy = Math.Min((int)max.Y, Cluster - 1);
                var minz = Math.Max((int)min.Z, 0);
                var maxz = Math.Min((int)max.Z, Cluster - 1);

                var lcp = WorldToCluster(light.Position);

                var tanq = (float)Math.Tan(angle);
                
                var ray = new Ray(light.Position, light.Direction);

                for (var x = minx; x <= maxx; x++)
                {
                    for (var y = miny; y <= maxy; y++)
                    {
                        for (var z = minz; z <= maxz; z++)
                        {
                            if (!IntersectSpotLightCluster(ray, range, tanq, x, y, z)) continue;

                            var wp = ClusterGridToWorld(x, y, z, lcp);

                            var d2 = Vector3.DistanceSquared(wp, light.Position);

                            var ci = z * (Cluster * Cluster) + y * Cluster + x;
                            var ci3 = ci * 3;
                            var ci4 = ci * 4;

                            var clusterLightCount = clusters[ci4];

                            var att = light.Color.Brightness * MathUtil.TwoPi / light.Angle;
                            att /= (light.Attenuation.Constant + light.Attenuation.Linear * (float)Math.Sqrt(d2) + light.Attenuation.Quadratic * d2);

                            if (clusterLightCount < 3)
                            {
                                clusterAtts[ci3 + clusterLightCount] = att;

                                clusters[ci4] = ++clusterLightCount;
                                clusters[ci4 + clusterLightCount] = (byte)li;
                            }
                            else
                            {
                                for (var i = 0; i < 3; i++)
                                {
                                    if (att > clusterAtts[ci3 + i])
                                    {
                                        clusterAtts[ci3 + i] = att;
                                        clusters[ci4 + 1 + i] = (byte)li;
                                        break;
                                    }
                                }
                            }

                            _spotLightVisible = true;
                        }
                    }
                }
            }

            if (_spotLightVisible) _spotLightClusterMap.Upload(clusters, 0, Cluster, Cluster, Cluster);
        }
    }
}
