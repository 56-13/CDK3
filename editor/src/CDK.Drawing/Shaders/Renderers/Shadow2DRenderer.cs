using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

namespace CDK.Drawing
{
    public class Shadow2DRendererParam
    {
        public Matrix4x4 ViewProjection { private set; get; }
        public Vector3 LightDirection { private set; get; }
        public Buffer UniformBuffer { private set; get; }

        public Shadow2DRendererParam(in Matrix4x4 viewProjection, in Vector3 lightDir)
        {
            ViewProjection = viewProjection;
            LightDirection = lightDir;
            UniformBuffer = Buffers.FromData(viewProjection, BufferTarget.UniformBuffer);
        }
    }

    public class Shadow2DRenderer : IRenderer, IDisposable
    {
        private ProgramBranch _programs;

        public Shadow2DRenderer()
        {
            _programs = new ProgramBranch();

            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow2D_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow2D_fs.glsl"));

            _programs.Attach(ShaderType.VertexShader, ShaderCode.VSBase, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, ShaderCode.Base, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();
        public bool Visible(in RenderState state) => state.Material.Shader != MaterialShader.Distortion;
        public void Validate(ref RenderState state)
        {
            state.DepthMode = DepthMode.None;
            state.Material.Shader = MaterialShader.Light;
            state.Material.BlendMode = BlendMode.Multiply;
            state.Material.CullMode = CullMode.None;
            if (state.Material.NormalMap == null) state.Material.DisplacementScale = 0;
            else if (state.Material.DisplacementScale == 0) state.Material.NormalMap = null;
            state.Material.DistortionScale = 0;
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
            state.Material.DepthTest = false;
            state.Material.DepthBias = 0;
            state.Material.AlphaTest = false;
            state.Material.AlphaTestBias = 0;
            state.Material.EmissiveMap = null;
            state.Material.Emission = Color3.Black;
            if (!state.UsingMap) state.Material.UVOffset = Vector2.Zero;
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
            state.Scissor = Rectangle.Zero;
            state.LightSpaceState = null;
        }

        public bool Clip(in RenderState state, IEnumerable<Vector3> wps, out Rectangle bounds)
        {
            bounds = Rectangle.ScreenNone;

            var param = (Shadow2DRendererParam)state.RendererParam;

            var box = ABoundingBox.None;
            foreach (var wp in wps) box.Append(wp, param.ViewProjection);
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
            _programs.AddBranch("UsingDisplacementMap", state.Material.DisplacementScale != 0, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingBone", boneBuffer != null, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingInstance", usingInstance, ProgramBranchMask.Vertex);

            var program = _programs.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api, boneBuffer);

            var param = (Shadow2DRendererParam)state.RendererParam;

            api.BindBufferBase(BufferTarget.UniformBuffer, RenderState.UniformBlockBindingShadow, param.UniformBuffer.Object);
        }

        public void Begin(Graphics graphics, in Matrix4x4 viewProjection, in Vector3 lightDir)
        {
            graphics.RendererParam = new Shadow2DRendererParam(viewProjection, lightDir);
        }

        public void End(Graphics graphics)
        {
            graphics.RendererParam = null;
        }
    }
}
