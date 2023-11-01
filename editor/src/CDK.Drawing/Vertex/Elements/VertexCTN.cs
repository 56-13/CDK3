using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct VertexCTN : IEquatable<VertexCTN>
    {
        public Vector3 Position;
        public Half4 Color;
        public Vector2 TexCoord;
        public Half3 Normal;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 34, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 34, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 34, 20, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 34, 28, 0, true)
        };

        public VertexCTN(in Vector3 position, in Half4 color, in Vector2 texCoord, in Half3 normal)
            : this()
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
            Normal = normal;
        }

        public VertexCTN(in Vector3 position, in Color4 color, in Vector2 texCoord, in Vector3 normal)
            : this(position, (Half4)color, texCoord, (Half3)normal)
        {
        }

        public VertexCTN(in Vector3 position, in Half4 color, in Vector2 texCoord)
            : this(position, color, texCoord, Half3.UnitZ)
        {
        }

        public VertexCTN(in Vector3 position, in Color4 color, in Vector2 texCoord)
            : this(position, (Half4)color, texCoord, Half3.UnitZ)
        {
        }

        public static bool operator ==(in VertexCTN a, in VertexCTN b) => a.Equals(b);
        public static bool operator !=(in VertexCTN a, in VertexCTN b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            return hash;
        }

        public bool Equals(VertexCTN other)
        {
            return Position == other.Position &&
                Color == other.Color &&
                TexCoord == other.TexCoord &&
                Normal == other.Normal;
        }

        public override bool Equals(object obj) => obj is VertexCTN other && Equals(other);
    }
}
