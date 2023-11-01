using System;
using System.Numerics;
using System.IO;

namespace CDK.Drawing
{
    public class SkyboxShader : IDisposable
    {
        private ProgramBranch _programs;
        private VertexArray _vertices;

        public SkyboxShader()
        {
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "skybox_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "skybox_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, fragmentShaderCode);

            _vertices = new VertexArray(1, false, new VertexLayout[]
            {
                new VertexLayout(0, 0, 3, VertexAttribType.Float, false, 12, 0, 0, true)
            });
            Vector3[] vertices = {
                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),

                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),

                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(1.0f, -1.0f,  1.0f)
            };
            _vertices.GetVertexBuffer(0).Upload(vertices, BufferUsageHint.StaticDraw);
        }

        public void Dispose()
        {
            _programs.Dispose();
            _vertices.Dispose();
        }

        public void Draw(GraphicsApi api, in Camera camera, Texture texture)
        {
            api.CurrentTarget.SetDrawBuffers(api, 0);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.Read);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var dataBuffer = Buffers.FromData(Matrix4x4.CreateTranslation(camera.Position) * camera.ViewProjection, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
            api.BindTextureBase(TextureTarget.TextureCubeMap, 0, texture.Object);

            _vertices.DrawArrays(api, PrimitiveMode.Triangles, 0, 36);
        }
    }
}
