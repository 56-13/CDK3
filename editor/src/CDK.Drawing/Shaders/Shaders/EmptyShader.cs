using System;
using System.IO;

namespace CDK.Drawing
{
    public class EmptyShader : IDisposable
    {
        private Program _program;

        public EmptyShader()
        {
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "empty_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "empty_fs.glsl"));

            var vertexShader = new Shader(ShaderType.VertexShader, vertexShaderCode);
            var fragmentShader = new Shader(ShaderType.FragmentShader, fragmentShaderCode);

            _program = new Program();
            _program.Attach(vertexShader);
            _program.Attach(fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();
        }

        public void Dispose() => _program.Dispose();

        public void Draw(GraphicsApi api, VertexArrayDraw vertices)
        {
            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _program.Use(api);

            vertices.Draw(api);
        }
    }
}

