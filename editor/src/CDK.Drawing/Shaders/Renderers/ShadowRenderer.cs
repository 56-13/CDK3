using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace CDK.Drawing
{
    public enum ShadowMode
    {
        Direction = 1,
        Point,
        Spot
    }

    public class ShadowRendererParam
    {
        public ShadowMode Mode { private set; get; }
        public Matrix4x4[] ViewProjections { private set; get; }
        public Buffer UniformBuffer { private set; get; }

        public ShadowRendererParam(ShadowMode mode, Matrix4x4[] viewProjections, Buffer uniformBuffer)
        {
            Mode = mode;
            ViewProjections = viewProjections;
            UniformBuffer = uniformBuffer;
        }
    }

    public class ShadowRenderer : IRenderer, IDisposable
    {
        private ProgramBranch _programs;

        public ShadowRenderer()
        {
            _programs = new ProgramBranch();

            var commonShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow.glsl"));
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow_vs.glsl"));
            var geometryShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow_gs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow_fs.glsl"));

            _programs.Attach(ShaderType.VertexShader, ShaderCode.VSBase, commonShaderCode, vertexShaderCode);
            _programs.Attach(ShaderType.GeometryShader, commonShaderCode, geometryShaderCode);
            _programs.Attach(ShaderType.FragmentShader, ShaderCode.Base, commonShaderCode, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();
        public bool Visible(in RenderState state) => state.Material.ReceiveLight;
        public void Validate(ref RenderState state)
        {
            var param = (ShadowRendererParam)state.RendererParam;

            state.DepthMode = DepthMode.ReadWrite;
            state.Material.Shader = MaterialShader.Light;
            state.Material.BlendMode = BlendMode.None;
            state.Material.CullMode = param.Mode == ShadowMode.Point ? CullMode.Front : CullMode.Back;       //LH / RH Problem, Consider vulkan or metal later
            if (!state.Material.AlphaTest) state.Material.ColorMap = null;
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
            state.Material.DepthTest = true;
            state.Material.DepthBias = 0;
            if (!state.Material.AlphaTest) state.Material.AlphaTestBias = 0;
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

            var param = (ShadowRendererParam)state.RendererParam;

            if (param.ViewProjections.Length == 1)
            {
                var box = ABoundingBox.None;
                foreach (var wp in wps) box.Append(wp, param.ViewProjections[0]);
                if (box.Intersects(ABoundingBox.ViewSpace, CollisionFlags.None, out _) != CollisionResult.Front)
                {
                    bounds.Append(box.Minimum.X, box.Minimum.Y);
                    bounds.Append(box.Maximum.X, box.Minimum.Y);
                    bounds.Append(box.Minimum.X, box.Maximum.Y);
                    bounds.Append(box.Maximum.X, box.Maximum.Y);
                    return true;
                }
            }
            else
            {
                bounds = Rectangle.ScreenFull;

                foreach (var m in param.ViewProjections)
                {
                    var box = ABoundingBox.None;
                    foreach (var wp in wps) box.Append(wp, m);
                    if (box.Intersects(ABoundingBox.ViewSpace, CollisionFlags.None, out _) != CollisionResult.Front)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Setup(GraphicsApi api, in RenderState state, PrimitiveMode mode, Buffer boneBuffer, bool usingInstance, bool usingVertexColor)
        {
            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            var param = (ShadowRendererParam)state.RendererParam;

            if (param.Mode == ShadowMode.Point)
            {
                _programs.AddLink(ProgramBranchMask.Geometry);
                switch (mode)
                {
                    case PrimitiveMode.Points:
                        _programs.AddBranch("UsingPrimitive", 1, 4, ProgramBranchMask.Geometry);
                        break;
                    case PrimitiveMode.Lines:
                    case PrimitiveMode.LineLoop:
                    case PrimitiveMode.LineStrip:
                        _programs.AddBranch("UsingPrimitive", 2, 4, ProgramBranchMask.Geometry);
                        break;
                    case PrimitiveMode.Triangles:
                    case PrimitiveMode.TriangleStrip:
                    case PrimitiveMode.TriangleFan:
                        _programs.AddBranch("UsingPrimitive", 3, 4, ProgramBranchMask.Geometry);
                        break;
                }
            }

            _programs.AddBranch("UsingShadowMode", (int)param.Mode, 4, ProgramBranchMask.Vertex | ProgramBranchMask.Geometry | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingMap", state.UsingMap, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingColorMap", state.Material.ColorMap != null, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingDisplacementMap", state.Material.DisplacementScale != 0, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingAlphaTest", state.Material.AlphaTest, ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBone", boneBuffer != null, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingInstance", usingInstance, ProgramBranchMask.Vertex);

            var program = _programs.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api, boneBuffer);

            api.BindBufferBase(BufferTarget.UniformBuffer, RenderState.UniformBlockBindingShadow, param.UniformBuffer.Object);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointUniformData : IEquatable<PointUniformData>
        {
            public Matrix4x4 ViewProjections0;
            public Matrix4x4 ViewProjections1;
            public Matrix4x4 ViewProjections2;
            public Matrix4x4 ViewProjections3;
            public Matrix4x4 ViewProjections4;
            public Matrix4x4 ViewProjections5;
            public Vector3 Position;
            public float Range;

            public static bool operator ==(in PointUniformData a, in PointUniformData b) => a.Equals(b);
            public static bool operator !=(in PointUniformData a, in PointUniformData b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Position.GetHashCode());
                hash.Combine(Range.GetHashCode());
                return hash;
            }

            public bool Equals(PointUniformData other)
            {
                return Position == other.Position && Range == other.Range;
            }

            public override bool Equals(object obj)
            {
                return obj is PointUniformData other && Equals(other);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SpotUniformData : IEquatable<SpotUniformData>
        {
            public Matrix4x4 ViewProjection;
            public Vector3 Position;
            public float Range;

            public static bool operator ==(in SpotUniformData a, in SpotUniformData b) => a.Equals(b);
            public static bool operator !=(in SpotUniformData a, in SpotUniformData b) => !a.Equals(b);
            public override int GetHashCode() => ViewProjection.GetHashCode();
            public bool Equals(SpotUniformData other) => ViewProjection == other.ViewProjection;
            public override bool Equals(object obj)
            {
                return obj is SpotUniformData other && Equals(other);
            }
        }

        public void BeginDirectional(Graphics graphics, in Matrix4x4 viewProjection)
        {
            graphics.RendererParam = new ShadowRendererParam(
                ShadowMode.Direction,
                new Matrix4x4[] { viewProjection },
                Buffers.FromData(viewProjection, BufferTarget.UniformBuffer));
        }

        public void BeginPoint(Graphics graphics, Matrix4x4[] viewProjections, in Vector3 pos, float range)
        {
            var data = new PointUniformData
            {
                ViewProjections0 = viewProjections[0],
                ViewProjections1 = viewProjections[1],
                ViewProjections2 = viewProjections[2],
                ViewProjections3 = viewProjections[3],
                ViewProjections4 = viewProjections[4],
                ViewProjections5 = viewProjections[5],
                Position = pos,
                Range = range
            };
            graphics.RendererParam = new ShadowRendererParam(
                ShadowMode.Point,
                viewProjections,
                Buffers.FromData(data, BufferTarget.UniformBuffer));
        }

        public void BeginSpot(Graphics graphics, in Matrix4x4 viewProjection, in Vector3 pos, float range)
        {
            var data = new SpotUniformData()
            {
                ViewProjection = viewProjection,
                Position = pos,
                Range = range
            };
            graphics.RendererParam = new ShadowRendererParam(
                ShadowMode.Spot,
                new Matrix4x4[] { viewProjection },
                Buffers.FromData(data, BufferTarget.UniformBuffer));
        }

        public void End(Graphics graphics)
        {
            graphics.RendererParam = null;
        }
    }
}
