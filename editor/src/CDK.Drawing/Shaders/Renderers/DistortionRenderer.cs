using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace CDK.Drawing
{
    public class DistortionRenderer : IRenderer, IDisposable
    {
        private ProgramBranch _programs;

        public DistortionRenderer()
        {
            _programs = new ProgramBranch();

            var vertexShaderPredefines = "#define UsingNormal\n";
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "distortion_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "distortion_fs.glsl"));

            _programs.Attach(ShaderType.VertexShader, vertexShaderPredefines, ShaderCode.VSBase, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, ShaderCode.Base, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();
        public bool Visible(in RenderState state) => state.Material.Shader == MaterialShader.Distortion && state.Material.DistortionScale > 0;
        public void Validate(ref RenderState state)
        {
            state.DepthMode = DepthMode.ReadWrite;
            state.Material.BlendMode = BlendMode.None;
            if (!state.Material.AlphaTest) state.Material.ColorMap = null;
            state.Material.MaterialMap = null;
            state.Material.MaterialMapComponents = 0;
            state.Material.Metallic = 0;
            state.Material.Roughness = 0;
            state.Material.AmbientOcclusion = 0;
            state.Material.Rim = 0;
            state.Material.Reflection = false;
            state.Material.Bloom = false;
            state.Material.ReceiveShadow = false;
            state.Material.ReceiveShadow2D = false;
            if (!state.Material.AlphaTest) state.Material.AlphaTestBias = 0;
            state.Material.EmissiveMap = null;
            state.Material.Emission = Color3.Black;
            state.FogColor = Color3.Black;
            state.FogNear = 0;
            state.FogFar = 0;
            state.BloomThreshold = 1;
            state.Brightness = 0;
            state.Contrast = 1;
            state.Saturation = 1;
            state.StrokeWidth = 0;
            state.StrokeColor = Color4.Black;
            state.StrokeMode = StrokeMode.Normal;
            state.LightSpaceState = null;
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
            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            _programs.AddBranch("UsingMap", state.UsingMap, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingColorMap", state.Material.ColorMap != null, ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingNormalMap", state.Material.NormalMap != null, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingAlphaTest", state.Material.AlphaTest, ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingDepthBias", state.Material.DepthBias != 0, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingBone", boneBuffer != null, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingInstance", usingInstance, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingPerspective", state.Camera.Fov != 0, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingUVOffset", state.Material.UVOffset != Vector2.Zero, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBloomSupported", state.Target.BloomSupported, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api, boneBuffer);

            var screenTexture = (Texture)state.RendererParam;

            api.BindTextureBase(TextureTarget.Texture2D, RenderState.TextureBindingScreenMap, screenTexture.Object);
        }

        public void Begin(Graphics graphics)
        {
            var screenTexture = graphics.Target.CaptureColor(0, true);
            screenTexture.Flush();
            graphics.RendererParam = screenTexture;
        }

        public void End(Graphics graphics)
        {
            var screenTexture = (Texture)graphics.RendererParam;
            var command = graphics.Command((GraphicsApi api) => ResourcePool.Instance.Remove(screenTexture));
            command.AddFence(graphics.Target, BatchFlag.ReadWrite);           //타겟에 쓰는 명령을 flush
            graphics.RendererParam = null;
        }
    }
}
