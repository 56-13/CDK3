using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{

    [StructLayout(LayoutKind.Sequential)]
    public struct FVertex : IEquatable<FVertex>
    {
        public Vector3 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;

        public FVertex(in Vector3 position, in Color4 color, in Vector2 texCoord, in Vector3 normal, in Vector3 tangent)
            : this()
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
            Normal = normal;
            Tangent = tangent;
        }

        public FVertex(in Vector3 position)
            : this(position, Color4.White, Vector2.Zero, Vector3.UnitZ, Vector3.UnitX)
        {
        }

        public FVertex(in Vector3 position, in Color4 color)
            : this(position, color, Vector2.Zero, Vector3.UnitZ, Vector3.UnitX)
        {
        }

        public FVertex(in Vector3 position, in Vector2 texCoord)
            : this(position, Color4.White, texCoord, Vector3.UnitZ, Vector3.UnitX)
        {
        }

        public FVertex(in Vector3 position, in Color4 color, in Vector2 texCoord)
            : this(position, color, texCoord, Vector3.UnitZ, Vector3.UnitX)
        {
        }

        public FVertex(in Vector3 position, in Vector3 normal, in Vector3 tangent)
            : this(position, Color4.White, Vector2.Zero, normal, tangent)
        {
        }

        public FVertex(in Vector3 position, in Color4 color, in Vector3 normal, in Vector3 tangent)
            : this(position, color, Vector2.Zero, normal, tangent)
        {
        }

        public FVertex(in Vector3 position, in Vector2 texCoord, in Vector3 normal, in Vector3 tangent)
            : this(position, Color4.White, texCoord, normal, tangent)
        {
        }

        public static bool operator ==(in FVertex a, in FVertex b) => a.Equals(b);
        public static bool operator !=(in FVertex a, in FVertex b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(TexCoord.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            hash.Combine(Tangent.GetHashCode());
            return hash;
        }

        public bool Equals(FVertex other)
        {
            return Position == other.Position &&
                Color == other.Color &&
                TexCoord == other.TexCoord &&
                Normal == other.Normal &&
                Tangent == other.Tangent;
        }

        public override bool Equals(object obj) => obj is FVertex other && Equals(other);
    }
}
