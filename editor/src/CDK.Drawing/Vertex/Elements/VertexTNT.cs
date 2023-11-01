using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexTNT : IEquatable<VertexTNT>
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Half3 Normal;
        public Half3 Tangent;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 32, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 32, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 32, 20, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTangent, 3, VertexAttribType.HalfFloat, false, 32, 26, 0, true)
        };

        public VertexTNT(in Vector3 position, in Vector2 texCoord, in Half3 normal, in Half3 tangent)
            : this()
        {
            Position = position;
            TexCoord = texCoord;
            Normal = normal;
            Tangent = tangent;
        }

        public VertexTNT(in Vector3 position, in Vector2 texCoord, in Vector3 normal, in Vector3 tangent)
            : this(position, texCoord, (Half3)normal, (Half3)tangent)
        {
        }

        public VertexTNT(in Vector3 position, in Vector2 texCoord)
            : this(position, texCoord, Half3.UnitZ, Half3.UnitX)
        {
        }

        public static bool operator ==(in VertexTNT a, in VertexTNT b) => a.Equals(b);
        public static bool operator !=(in VertexTNT a, in VertexTNT b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            hash.Combine(Tangent.GetHashCode());
            return hash;
        }

        public bool Equals(VertexTNT other)
        {
            return Position == other.Position &&
                TexCoord == other.TexCoord &&
                Normal == other.Normal &&
                Tangent == other.Tangent;
        }

        public override bool Equals(object obj) => obj is VertexTNT other && Equals(other);
    }
}
