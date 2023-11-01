using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public partial class LightSpace
    {
        private List<PointLight> _pointLights;
        private bool _pointLightUpdated;
        private bool _pointLightVisible;
        private Buffer _pointLightBuffer;
        private Texture _pointLightClusterMap;
        private PointShadowMap[] _pointShadowMaps;        
        private List<int> _pointShadowLightIndices;
        private int _pointShadowCursor;

        private void InitializePointLight()
        {
            _pointLights = new List<PointLight>(MaxPointLightCount);
            _pointShadowMaps = new PointShadowMap[MaxPointShadowCount];
            _pointShadowLightIndices = new List<int>(MaxPointShadowCount);
            _pointShadowCursor = -1;
        }

        private void DisposePointLight()
        {
            _pointLightBuffer?.Dispose();
            _pointLightClusterMap?.Dispose();
            foreach (var shadowMap in _pointShadowMaps) shadowMap?.Dispose();
        }

        public int PointLightCount => _pointLights.Count;

        public void SetPointLight(in PointLight light)
        {
            Debug.Assert(_updating);

            lock (this)
            {
                _lightUpdateKeys.Add(light.Key);

                for (var i = 0; i < _pointLights.Count; i++)
                {
                    var prev = _pointLights[i];

                    if (prev.Key.Equals(light.Key))
                    {
                        if (prev != light)
                        {
                            _pointLights[i] = light;
                            _pointLightUpdated = true;
                        }
                        return;
                    }
                }
                if (_pointLights.Count < MaxPointLightCount)
                {
                    _pointLights.Add(light);
                    _pointLightUpdated = true;
                }
            }
        }

        private bool BeginPointShadow(Graphics graphics)
        {
            var camera = graphics.Camera;

            if (_pointShadowCursor == -1)
            {
                _pointShadowLightIndices.Clear();

                if (AllowShadow)
                {
                    for (var i = 0; i < _pointLights.Count; i++)
                    {
                        var light = _pointLights[i];

                        if (light.CastShadow)
                        {
                            if (_pointShadowLightIndices.Count < MaxPointShadowCount)
                            {
                                _pointShadowLightIndices.Add(i);
                            }
                            else
                            {
                                var dist = Vector3.Distance(camera.Position, light.Position) - light.Range;

                                var index = -1;
                                for (var j = 0; j < MaxPointShadowCount; j++)
                                {
                                    var other = _pointLights[_pointShadowLightIndices[j]];

                                    var d = Vector3.Distance(camera.Position, other.Position) - other.Range;

                                    if (dist < d)
                                    {
                                        index = j;
                                        dist = d;
                                    }
                                }
                                if (index != -1) _pointShadowLightIndices[index] = i;
                            }
                        }
                    }
                }
                for (var i = _pointShadowLightIndices.Count; i < MaxPointShadowCount; i++)
                {
                    _pointShadowMaps[i]?.Dispose();
                    _pointShadowMaps[i] = null;
                }

                if (_pointShadowLightIndices.Count == 0) return false;

                _pointShadowCursor = 0;
            }
            else if (_pointShadowCursor >= _pointShadowLightIndices.Count)
            {
                _pointShadowCursor = -1;
                return false;
            }

            {
                var light = _pointLights[_pointShadowLightIndices[_pointShadowCursor]];
                var range = light.Range;
                if (!AllowShadowPixel32) light.Shadow.Pixel32 = false;
                if (light.Shadow.Resolution > MaxShadowResolution) light.Shadow.Resolution = MaxShadowResolution;
                while ((light.Shadow.Resolution >> 1) >= range) light.Shadow.Resolution >>= 1;
                if (_pointShadowMaps[_pointShadowCursor] == null) _pointShadowMaps[_pointShadowCursor] = new PointShadowMap();
                _pointShadowMaps[_pointShadowCursor].Begin(graphics, light, range);
                return true;
            }
        }

        private void EndPointShadow(Graphics graphics)
        {
            _pointShadowMaps[_pointShadowCursor].End(graphics);

            _pointShadowCursor++;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PointLightData
        {
            public Vector3 Position;
            public float Pad0;
            public Color3 Color;
            public float Pad1;
            public Vector4 Attenuation;
            public int ShadowMapIndex;
            public float ShadowBias;
            public float ShadowBleeding;
            public float Pad2;
        }

        private void UpdatePointLights(bool viewUpdated)
        {
            if (_pointLightUpdated || viewUpdated)
            {
                if (_pointLights.Count != 0)
                {
                    if (_pointLightBuffer == null) _pointLightBuffer = new Buffer(BufferTarget.UniformBuffer);
                    if (_pointLightClusterMap == null)
                    {
                        _pointLightClusterMap = new Texture(new TextureDescription()
                        {
                            Target = TextureTarget.Texture3D,
                            Width = Cluster,
                            Height = Cluster,
                            Depth = Cluster,
                            Format = RawFormat.Rgba8ui
                        }, false);
                    }
                    var lightData = new PointLightData[_pointLights.Count];
                    for (var i = 0; i < _pointLights.Count; i++)
                    {
                        var light = _pointLights[i];
                        lightData[i] = new PointLightData
                        {
                            Position = light.Position,
                            Color = light.Color,
                            Attenuation = new Vector4(light.Range, light.Attenuation.Constant, light.Attenuation.Linear, light.Attenuation.Quadratic),
                            ShadowMapIndex = light.CastShadow ? _pointShadowLightIndices.IndexOf(i) : -1,
                            ShadowBias = light.Shadow.Bias,
                            ShadowBleeding = light.Shadow.Bleeding
                        };
                    }
                    _pointLightBuffer.Upload(lightData, BufferUsageHint.DynamicDraw);

                    UpdatePointLightCluster();
                }
                else
                {
                    _pointLightBuffer?.Dispose();
                    _pointLightBuffer = null;
                    _pointLightClusterMap?.Dispose();
                    _pointLightClusterMap = null;
                    _pointLightVisible = false;
                }
                _pointLightUpdated = false;
            }
        }

        private void UpdatePointLightCluster()
        {
            var clusters = new byte[Cluster * Cluster * Cluster * 4];
            var clusterAtts = new float[Cluster * Cluster * Cluster * 3];

            _pointLightVisible = false;

            for (var li = 0; li < _pointLights.Count; li++)
            {
                var light = _pointLights[li];
                var range = light.Range;

                var min = new Vector3(float.MaxValue);
                var max = new Vector3(float.MinValue);
                foreach (var wp in ABoundingBox.GetCorners(light.Position - new Vector3(range), light.Position + new Vector3(range)))
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
                var range2 = range * range;

                for (var x = minx; x <= maxx; x++)
                {
                    for (var y = miny; y <= maxy; y++)
                    {
                        for (var z = minz; z <= maxz; z++)
                        {
                            var wp = ClusterGridToWorld(x, y, z, lcp);
                            var d2 = Vector3.DistanceSquared(wp, light.Position);

                            if (d2 < range2)
                            {
                                var ci = z * (Cluster * Cluster) + y * Cluster + x;
                                var ci3 = ci * 3;
                                var ci4 = ci * 4;

                                var clusterLightCount = clusters[ci4];

                                var att = light.Color.Brightness;
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

                                _pointLightVisible = true;
                            }
                        }
                    }
                }
            }

            if (_pointLightVisible) _pointLightClusterMap.Upload(clusters, 0, Cluster, Cluster, Cluster);
        }
    }
}
