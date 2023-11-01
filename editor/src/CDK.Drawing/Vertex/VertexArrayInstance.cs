using System.Numerics;

namespace CDK.Drawing
{
    public struct VertexArrayInstance
    {
        public Matrix4x4 Model;
        public Color4 Color;

        public static readonly VertexArrayInstance Origin = new VertexArrayInstance(Matrix4x4.Identity, Color4.White);

        public VertexArrayInstance(in Matrix4x4 model, in Color4 color) : this()
        {
            Model = model;
            Color = color;
        }
    }
}
