using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexT : IEquatable<VertexT>
    {
        public Vector3 Position;
        public Vector2 TexCoord;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 20, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 20, 12, 0, true)
        };

        public VertexT(in Vector3 position, in Vector2 texCoord)
            : this()
        {
            Position = position;
            TexCoord = texCoord;
        }

        public static bool operator ==(in VertexT a, in VertexT b) => a.Equals(b);
        public static bool operator !=(in VertexT a, in VertexT b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            return hash;
        }

        public bool Equals(VertexT other)
        {
            return Position == other.Position && TexCoord == other.TexCoord;
        }

        public override bool Equals(object obj) => obj is VertexT other && Equals(other);
    }
}
