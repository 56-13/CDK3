using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public partial class LightSpace : IDisposable
    {
        public LightMode Mode { set; get; }
        public Color3 AmbientLight { set; get; }
        public Texture EnvMap { set; get; }
        public Color3 EnvColor { set; get; }
        public Texture BRDFMap { set; get; }
        public ABoundingBox Space { set; get; }
        public bool AllowShadow { set; get; }
        public bool AllowShadowPixel32 { set; get; }
        public int MaxShadowResolution { set; get; }

        private Camera _camera;
        private Matrix4x4 _viewProjectionInv;
        private float _clusterMaxDepth;
        private HashSet<object> _lightUpdateKeys;
        private int _cursor;
        private bool _updating;
        private bool _first;

        public const int MaxDirectionalLightCount = 3;
        public const int MaxPointLightCount = 256;
        public const int MaxSpotLightCount = 256;
        public const int MaxPointShadowCount = 3;
        public const int MaxSpotShadowCount = 3;

        public LightSpace()
        {
            Mode = LightMode.CookGGX;
            AllowShadow = true;
            AllowShadowPixel32 = true;
            MaxShadowResolution = 2048;

            _lightUpdateKeys = new HashSet<object>();
            _first = true;

            InitializeDirectionalLight();
            InitializePointLight();
            InitializeSpotLight();
        }

        public void Dispose()
        {
            DisposeDirectionalLight();
            DisposePointLight();
            DisposeSpotLight();
        }

        public void Clear()
        {
            Debug.Assert(!_updating);

            _directionalLights.Clear();
            _directionalLightUpdated = true;
            _pointLights.Clear();
            _pointLightUpdated = true;
            _spotLights.Clear();
            _spotLightUpdated = true;
        }

        public void BeginUpdate()
        {
            _updating = true;
        }

        public void EndUpdate()
        {
            Debug.Assert(_updating);

            _updating = false;

            var i = 0;
            while (i < _directionalLights.Count)
            {
                if (_lightUpdateKeys.Contains(_directionalLights[i].Key)) i++;
                else
                {
                    _directionalLights.RemoveAt(i);
                    _directionalLightUpdated = true;
                }
            }
            i = 0;
            while (i < _pointLights.Count)
            {
                if (_lightUpdateKeys.Contains(_pointLights[i].Key)) i++;
                else
                {
                    _pointLights.RemoveAt(i);
                    _pointLightUpdated = true;
                }
            }
            i = 0;
            while (i < _spotLights.Count)
            {
                if (_lightUpdateKeys.Contains(_spotLights[i].Key)) i++;
                else
                {
                    _spotLights.RemoveAt(i);
                    _spotLightUpdated = true;
                }
            }
            _lightUpdateKeys.Clear();
        }

        public bool BeginDraw(Graphics graphics, out InstanceLayer layer)
        {
            switch (_cursor)
            {
                case 0:
                    if (BeginDirectionalShadow(graphics, out var shadow2D))
                    {
                        layer = shadow2D ? InstanceLayer.Shadow2D : InstanceLayer.Shadow;
                        return true;
                    }
                    _cursor = 1;
                    goto case 1;
                case 1:
                    if (BeginPointShadow(graphics))
                    {
                        layer = InstanceLayer.Shadow;
                        return true;
                    }
                    _cursor = 2;
                    goto case 2;
                case 2:
                    if (BeginSpotShadow(graphics))
                    {
                        layer = InstanceLayer.Shadow;
                        return true;
                    }
                    _cursor = 3;
                    goto case 3;
                case 3:
                    UploadState(graphics);
                    _cursor = 0;
                    break;
            }
            layer = InstanceLayer.None;
            return false;
        }

        public void EndDraw(Graphics graphics)
        {
            switch (_cursor)
            {
                case 0:
                    EndDirectionalShadow(graphics);
                    break;
                case 1:
                    EndPointShadow(graphics);
                    break;
                case 2:
                    EndSpotShadow(graphics);
                    break;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UniformData : IEquatable<UniformData>
        {
            public Color3 AmbientLight;
            public float ClusterMaxDepth;
            public Color3 EnvColor;
            public int EnvMapMaxLod;
            public int DirectionalLightCount;

            public static bool operator ==(in UniformData a, in UniformData b) => a.Equals(b);
            public static bool operator !=(in UniformData a, in UniformData b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(AmbientLight.GetHashCode());
                hash.Combine(ClusterMaxDepth.GetHashCode());
                hash.Combine(EnvColor.GetHashCode());
                hash.Combine(EnvMapMaxLod.GetHashCode());
                hash.Combine(DirectionalLightCount.GetHashCode());
                return hash;
            }

            public bool Equals(UniformData other)
            {
                return AmbientLight == other.AmbientLight &&
                    ClusterMaxDepth == other.ClusterMaxDepth &&
                    EnvColor == other.EnvColor &&
                    EnvMapMaxLod == other.EnvMapMaxLod &&
                    DirectionalLightCount == other.DirectionalLightCount;
            }

            public override bool Equals(object obj)
            {
                return obj is UniformData other && Equals(other);
            }
        }

        private void UploadState(Graphics graphics)
        {
            Debug.Assert(Mode != LightMode.None);

            var viewUpdated = _first || _camera != graphics.Camera;

            if (viewUpdated)
            {
                _camera = graphics.Camera;
                var viewProjection = _camera.ViewProjection;
                if (!Matrix4x4.Invert(viewProjection, out _viewProjectionInv)) throw new InvalidOperationException();
                _clusterMaxDepth = 0;
                foreach (var wp in Space.GetCorners())
                {
                    var vp = Vector4.Transform(wp, viewProjection);
                    var d = Math.Min(vp.W, _camera.Far);
                    if (d > _clusterMaxDepth) _clusterMaxDepth = vp.W;
                }
                _first = false;
            }
            UpdateDirectionalLights(viewUpdated);
            UpdatePointLights(viewUpdated);
            UpdateSpotLights(viewUpdated);

            var state = new LightSpaceState
            {
                Mode = Mode,
                UsingShadow = AllowShadow,
                EnvMap = EnvMap,
                BRDFMap = BRDFMap
            };

            var data = new UniformData
            {
                AmbientLight = AmbientLight,
                ClusterMaxDepth = _clusterMaxDepth,
                EnvColor = EnvColor,
                DirectionalLightCount = _directionalLights.Count
            };
            if (EnvMap != null) data.EnvMapMaxLod = EnvMap.Description.MipmapCount - 1;
            state.LightBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);

            if (_directionalLights.Count != 0)
            {
                state.UsingDirectionalLight = true;
                state.DirectionalLightBuffer = _directionalLightBuffer;
                for (var i = 0; i < MaxDirectionalLightCount; i++)
                {
                    if (_directionalShadowMaps[i] != null && _directionalShadowMaps[i].Visible)
                    {
                        state.SetDirectionalShadowMap(i, _directionalShadowMaps[i].Texture);
                        state.SetDirectionalShadow2DMap(i, _directionalShadowMaps[i].Texture2D);
                    }
                }
            }
            if (_pointLightVisible)
            {
                state.UsingPointLight = true;
                state.PointLightBuffer = _pointLightBuffer;
                state.PointLightClusterMap = _pointLightClusterMap;
                for (var i = 0; i < MaxPointShadowCount; i++)
                {
                    if (_pointShadowMaps[i] != null) state.SetPointShadowMap(i, _pointShadowMaps[i].Texture);
                }
            }
            if (_spotLightVisible)
            {
                state.UsingSpotLight = true;
                state.SpotLightBuffer = _spotLightBuffer;
                state.SpotLightClusterMap = _spotLightClusterMap;
                state.SpotShadowBuffer = _spotShadowBuffer;
                for (var i = 0; i < MaxSpotShadowCount; i++)
                {
                    if (_spotShadowMaps[i] != null) state.SetSpotShadowMap(i, _spotShadowMaps[i].Texture);
                }
            }
            
            graphics.LightSpaceState = state;

            state.Flush(graphics);
        }
    }
}
