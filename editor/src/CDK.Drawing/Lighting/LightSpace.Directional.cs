using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public partial class LightSpace
    {
        private List<DirectionalLight> _directionalLights;
        private bool _directionalLightUpdated;
        private Buffer _directionalLightBuffer;
        private DirectionalShadowMap[] _directionalShadowMaps;
        private int _directionalShadowCursor;

        private void InitializeDirectionalLight()
        {
            _directionalLights = new List<DirectionalLight>(MaxDirectionalLightCount);
            _directionalShadowMaps = new DirectionalShadowMap[MaxDirectionalLightCount];
        }

        private void DisposeDirectionalLight()
        {
            _directionalLightBuffer?.Dispose();
            foreach (var shadowMap in _directionalShadowMaps) shadowMap?.Dispose();
        }

        public int DirectionalLightCount => _directionalLights.Count;

        public void SetDirectionalLight(in DirectionalLight light)
        {
            Debug.Assert(_updating);

            lock (this)
            {
                _lightUpdateKeys.Add(light.Key);

                for (var i = 0; i < _directionalLights.Count; i++)
                {
                    var prev = _directionalLights[i];

                    if (prev.Key.Equals(light.Key))
                    {
                        if (prev != light)
                        {
                            _directionalLights[i] = light;
                            _directionalLightUpdated = true;
                        }
                        return;
                    }
                }
                if (_directionalLights.Count < MaxDirectionalLightCount)
                {
                    _directionalLights.Add(light);
                    _directionalLightUpdated = true;
                }
            }
        }

        private bool BeginDirectionalShadow(Graphics graphics, out bool shadow2D)
        {
            if (!AllowShadow)
            {
                for (var i = 0; i < MaxDirectionalLightCount; i++)
                {
                    _directionalShadowMaps[i]?.Dispose();
                    _directionalShadowMaps[i] = null;
                }
                _directionalShadowCursor = 0;
                shadow2D = false;
                return false;
            }
            while (_directionalShadowCursor < _directionalLights.Count * 2)
            {
                var lightIndex = _directionalShadowCursor >> 1;
                shadow2D = (_directionalShadowCursor & 1) == 1;

                var light = _directionalLights[lightIndex];

                if (shadow2D ? light.CastShadow2D : light.CastShadow)
                {
                    if (!AllowShadowPixel32) light.Shadow.Pixel32 = false;
                    if (light.Shadow.Resolution > MaxShadowResolution) light.Shadow.Resolution = MaxShadowResolution;
                    if (_directionalShadowMaps[lightIndex] == null) _directionalShadowMaps[lightIndex] = new DirectionalShadowMap();
                    if (_directionalShadowMaps[lightIndex].Begin(graphics, light, Space, shadow2D)) return true;
                }
                else if (!shadow2D && !light.CastShadow2D)      //both false
                {
                    _directionalShadowMaps[lightIndex]?.Dispose();
                    _directionalShadowMaps[lightIndex] = null;
                }
                else _directionalShadowMaps[lightIndex]?.Dispose(shadow2D);

                _directionalShadowCursor++;
            }
            for (var i = _directionalLights.Count; i < MaxDirectionalLightCount; i++)
            {
                _directionalShadowMaps[i]?.Dispose();
                _directionalShadowMaps[i] = null;
            }
            _directionalShadowCursor = 0;
            shadow2D = false;
            return false;
        }

        private void EndDirectionalShadow(Graphics graphics)
        {
            var lightIndex = _directionalShadowCursor >> 1;
            var shadow2D = (_directionalShadowCursor & 1) == 1;
            _directionalShadowMaps[lightIndex].End(graphics, shadow2D);

            _directionalShadowCursor++;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DirectionalLightData
        {
            public Vector3 Direction;
            public float Pad0;
            public Color3 Color;
            public float Pad1;
            public Matrix4x4 ShadowViewProjection;
            public Matrix4x4 Shadow2DViewProjection;
            public int ShadowMapIndex;
            public int Shadow2DMapIndex;
            public float ShadowBias;
            public float ShadowBleeding;
        }

        private void UpdateDirectionalLights(bool viewUpdated)
        {
            if (_directionalLightUpdated || viewUpdated)
            {
                if (_directionalLights.Count != 0)
                {
                    if (_directionalLightBuffer == null) _directionalLightBuffer = new Buffer(BufferTarget.UniformBuffer);

                    var lightData = new DirectionalLightData[_directionalLights.Count];
                    for (var i = 0; i < _directionalLights.Count; i++)
                    {
                        var light = _directionalLights[i];

                        lightData[i] = new DirectionalLightData
                        {
                            Direction = light.Direction,
                            Color = light.Color,
                            ShadowMapIndex = -1,
                            Shadow2DMapIndex = -1,
                            ShadowBias = light.Shadow.Bias,
                            ShadowBleeding = light.Shadow.Bleeding
                        };
                        if (_directionalShadowMaps[i] != null && _directionalShadowMaps[i].Visible) 
                        {
                            if (light.CastShadow)
                            {
                                lightData[i].ShadowMapIndex = i;
                                lightData[i].ShadowViewProjection = _directionalShadowMaps[i].ViewProjection;
                            }
                            if (light.CastShadow2D)
                            {
                                lightData[i].ShadowMapIndex = i;
                                lightData[i].Shadow2DViewProjection = _directionalShadowMaps[i].ViewProjection2D;
                            }
                        }
                    }
                    _directionalLightBuffer.Upload(lightData, BufferUsageHint.DynamicDraw);
                }
                else
                {
                    _directionalLightBuffer?.Dispose();
                    _directionalLightBuffer = null;
                }
                _directionalLightUpdated = false;
            }
        }
    }
}
