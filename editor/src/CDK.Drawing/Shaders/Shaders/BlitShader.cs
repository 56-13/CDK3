using System.IO;

namespace CDK.Drawing
{
    public class BlitShader
    {
        private ProgramBranch _programs;

        public BlitShader()
        {
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "blit_vs.glsl"));
            var geometryShaderCode = File.ReadAllText(Path.Combine("Resources", "blit_gs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "blit_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, vertexShaderCode);
            _programs.Attach(ShaderType.GeometryShader, geometryShaderCode);
            _programs.Attach(ShaderType.FragmentShader, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();

        private static Rectangle BoundsToQuad(RenderTarget target, in Bounds2 bounds) {
            var viewport = target.Viewport;

            return new Rectangle(
                (float)(bounds.X - viewport.X) / viewport.Width,
                (float)(bounds.Y - viewport.Y) / viewport.Height,
                (float)bounds.Width / viewport.Width,
                (float)bounds.Height / viewport.Height);
        }

        public void Draw(GraphicsApi api, Texture texture, bool cube)
        {
            Draw(api, texture, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), cube);
        }

        public void Draw(GraphicsApi api, Texture texture, in Bounds2 bounds, bool cube)
        {
            Draw(api, texture, VertexArrayDraw.Array(VertexArrays.Get2D(BoundsToQuad(api.CurrentTarget, bounds)), PrimitiveMode.TriangleStrip, 0, 4), cube);
        }

        public void Draw(GraphicsApi api, Texture texture, VertexArrayDraw vertices, bool cube)
        {
            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            if (cube) _programs.AddLink(ProgramBranchMask.Geometry);
            _programs.AddBranch("UsingCube", cube, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            api.BindTextureBase(TextureTarget.Texture2D, 0, texture.Object);

            vertices.Draw(api);
        }
    }
}
