using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct VertexTN : IEquatable<VertexTN>
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Half3 Normal;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 26, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 26, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 26, 20, 0, true)
        };

        public VertexTN(in Vector3 position, in Vector2 texCoord, in Half3 normal)
            : this()
        {
            Position = position;
            TexCoord = texCoord;
            Normal = normal;
        }

        public VertexTN(in Vector3 position, in Vector2 texCoord, in Vector3 normal)
            : this(position, texCoord, (Half3)normal)
        {
        }

        public VertexTN(in Vector3 position, in Vector2 texCoord)
            : this(position, texCoord, Half3.UnitZ)
        {
        }

        public static bool operator ==(in VertexTN a, in VertexTN b) => a.Equals(b);
        public static bool operator !=(in VertexTN a, in VertexTN b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            return hash;
        }

        public bool Equals(VertexTN other)
        {
            return Position == other.Position &&
                TexCoord == other.TexCoord &&
                Normal == other.Normal;
        }

        public override bool Equals(object obj) => obj is VertexTN other && Equals(other);
    }
}
