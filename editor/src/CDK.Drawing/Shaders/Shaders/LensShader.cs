using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.IO;

namespace CDK.Drawing
{
    public class LensShader : IDisposable
    {
        private ProgramBranch _programs;

        public LensShader()
        {
            var commonShaderCode = File.ReadAllText(Path.Combine("Resources", "lens.glsl"));
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "lens_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "lens_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, commonShaderCode, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, commonShaderCode, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();

        [StructLayout(LayoutKind.Sequential)]
        private struct UniformData : IEquatable<UniformData>
        {
            public Matrix4x4 WorldViewProjection;
            public Vector3 Center;
            public float Radius;
            public Vector2 Resolution;
            public float Convex;

            public static bool operator ==(in UniformData a, in UniformData b) => a.Equals(b);
            public static bool operator !=(in UniformData a, in UniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(WorldViewProjection.GetHashCode());
                hash.Combine(Center.GetHashCode());
                hash.Combine(Radius.GetHashCode());
                hash.Combine(Resolution.GetHashCode());
                hash.Combine(Convex.GetHashCode());
                return hash;
            }

            public bool Equals(UniformData other)
            {
                return WorldViewProjection == other.WorldViewProjection &&
                    Center == other.Center &&
                    Radius == other.Radius &&
                    Resolution == other.Resolution &&
                    Convex == other.Convex;
            }

            public override bool Equals(object obj)
            {
                return obj is UniformData other && Equals(other);
            }
        }
        public void Draw(GraphicsApi api, Texture screenTexture, in Matrix4x4 worldViewProjection, in Vector3 center, float radius, float convex)
        {
            var target = api.CurrentTarget;

            if (target.BloomSupported) target.SetDrawBuffers(api, 0, 1);
            else target.SetDrawBuffers(api, 0);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            _programs.AddBranch("UsingBloomSupported", target.BloomSupported, ProgramBranchMask.Fragment);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);

            var program = _programs.EndBranch();

            program.Use(api);

            var data = new UniformData
            {
                WorldViewProjection = worldViewProjection,
                Center = center,
                Radius = radius,
                Resolution = new Vector2(target.Viewport.Width, target.Viewport.Height),
                Convex = convex
            };
            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
            api.BindTextureBase(TextureTarget.Texture2D, 0, screenTexture.Object);

            var vertices = VertexArrays.Get3D(
                new Vector3(center.X - radius, center.Y - radius, center.Z),
                new Vector3(center.X + radius, center.Y - radius, center.Z),
                new Vector3(center.X - radius, center.Y + radius, center.Z),
                new Vector3(center.X + radius, center.Y * radius, center.Z));

            vertices.DrawArrays(api, PrimitiveMode.TriangleStrip, 0, 4);
        }
    }
}

