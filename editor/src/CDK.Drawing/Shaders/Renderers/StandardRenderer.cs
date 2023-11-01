using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace CDK.Drawing
{
    public partial class StandardRenderer : IRenderer, IDisposable
    {
        private ProgramBranch _programs;

        public StandardRenderer()
        {
            _programs = new ProgramBranch();

            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "standard_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "standard_fs.glsl"));

            _programs.Attach(ShaderType.VertexShader, ShaderCode.VSBase, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, ShaderCode.FSBase, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();

        public bool Visible(in RenderState state) => state.Material.Shader != MaterialShader.Distortion;

        public void Validate(ref RenderState state)
        {
            var usingLight = state.Material.ReceiveLight && state.LightSpaceState != null;
            var usingStroke = state.UsingStroke;
            var usingFog = state.UsingFog;

            if (state.StencilMode != StencilMode.None)
            {
                state.Material.Bloom = false;
                if (state.Material.BlendMode > BlendMode.Alpha) state.Material.BlendMode = BlendMode.Alpha;
                state.Material.Emission = Color3.Black;
                usingLight = false;
                usingStroke = false;
                usingFog = false;
                state.Brightness = 0;
                state.Contrast = 1;
                state.Saturation = 1;
            }

            if (!usingLight)
            {
                if (state.Material.Shader == MaterialShader.Light) state.Material.Shader = MaterialShader.NoLight;
                if (state.Material.DisplacementScale == 0) state.Material.NormalMap = null;
                state.Material.MaterialMap = null;
                state.Material.MaterialMapComponents = 0;
                state.Material.Reflection = false;
                state.Material.Metallic = 0;
                state.Material.Roughness = 0;
                state.Material.AmbientOcclusion = 0;
                state.Material.Rim = 0;
                state.Material.ReceiveShadow = false;
                state.Material.ReceiveShadow2D = false;
                state.LightSpaceState = null;
            }
            else
            {
                if (state.Material.MaterialMap == null) state.Material.MaterialMapComponents = 0;
                else if (state.Material.MaterialMapComponents == 0) state.Material.MaterialMap = null;

                if (state.LightSpaceState.EnvMap == null) state.Material.Reflection = false;

                switch(state.LightSpaceState.Mode)
                {
                    case LightMode.Blinn:
                    case LightMode.Phong:
                        state.Material.Rim = 0;
                        break;
                }
            }

            state.Material.DistortionScale = 0;

            if (state.Material.NormalMap == null) state.Material.DisplacementScale = 0;

            if (!state.UsingMap) state.Material.UVOffset = Vector2.Zero;

            if (!state.Target.BloomSupported) state.Material.Bloom = false;
            if (!state.Material.Bloom) state.BloomThreshold = 1;

            if (state.DepthMode == DepthMode.None || !state.Material.DepthTest)
            {
                state.Material.DepthTest = false;
                state.Material.DepthBias = 0;
            }

            if (!state.Material.AlphaTest) state.Material.AlphaTestBias = 0;

            if (!usingFog)
            {
                state.FogColor = Color3.Black;
                state.FogNear = 0;
                state.FogFar = 0;
            }

            if (!usingStroke)
            {
                state.StrokeWidth = 0;
                state.StrokeColor = Color4.Black;
                state.StrokeMode = StrokeMode.Normal;
            }
        }

        public bool Clip(in RenderState state, IEnumerable<Vector3> wps, out Rectangle bounds)
        {
            bounds = Rectangle.ScreenNone;

            var box = ABoundingBox.None;
            var vp = state.Camera.ViewProjection;
            foreach (var wp in wps) box.Append(wp, vp);
            if (box.Intersects(ABoundingBox.ViewSpace, CollisionFlags.None, out _) != CollisionResult.Front)
            {
                bounds.Append(box.Minimum.X, box.Minimum.Y);
                bounds.Append(box.Maximum.X, box.Minimum.Y);
                bounds.Append(box.Minimum.X, box.Maximum.Y);
                bounds.Append(box.Maximum.X, box.Maximum.Y);
                return true;
            }
            return false;
        }

        public void Setup(GraphicsApi api, in RenderState state, PrimitiveMode mode, Buffer boneBuffer, bool usingInstance, bool usingVertexColor)
        {
            state.ApplyBranch(_programs, boneBuffer != null, usingInstance, usingVertexColor);

            var program = _programs.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api, boneBuffer);
        }
    }
}
