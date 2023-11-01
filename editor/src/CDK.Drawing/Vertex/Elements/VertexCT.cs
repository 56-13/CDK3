using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexCT : IEquatable<VertexCT>
    {
        public Vector3 Position;
        public Half4 Color;
        public Vector2 TexCoord;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 28, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 28, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 28, 20, 0, true)
        };

        public VertexCT(in Vector3 position, in Half4 color, in Vector2 texCoord)
            : this()
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }

        public VertexCT(in Vector3 position, in Color4 color, in Vector2 texCoord)
            : this(position, (Half4)color, texCoord)
        {
        }

        public static bool operator ==(in VertexCT a, in VertexCT b) => a.Equals(b);
        public static bool operator !=(in VertexCT a, in VertexCT b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            return hash;
        }

        public bool Equals(VertexCT other)
        {
            return Position == other.Position &&
                Color == other.Color &&
                TexCoord == other.TexCoord;
        }

        public override bool Equals(object obj) => obj is VertexCT other && Equals(other);
    }
}
