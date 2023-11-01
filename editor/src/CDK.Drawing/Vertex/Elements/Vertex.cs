using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex : IEquatable<Vertex>
    {
        public Vector3 Position;
        public Half4 Color;
        public Vector2 TexCoord;
        public Half3 Normal;
        public Half3 Tangent;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 40, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 40, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 40, 20, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 40, 28, 0, true),
            new VertexLayout(0, RenderState.VertexAttribTangent, 3, VertexAttribType.HalfFloat, false, 40, 34, 0, true)
        };

        public Vertex(in Vector3 position, in Half4 color, in Vector2 texCoord, in Half3 normal, in Half3 tangent)
            : this()
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
            Normal = normal;
            Tangent = tangent;
        }

        public Vertex(in Vector3 position, in Color4 color, in Vector2 texCoord, in Vector3 normal, in Vector3 tangent)
            : this(position, (Half4)color, texCoord, (Half3)normal, (Half3)tangent)
        {
        }

        public Vertex(in Vector3 position)
            : this(position, Half4.One, Vector2.Zero, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Half4 color)
            : this(position, color, Vector2.Zero, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Color4 color)
            : this(position, (Half4)color, Vector2.Zero, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Vector2 texCoord)
            : this(position, Half4.One, texCoord, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Half4 color, in Vector2 texCoord)
            : this(position, color, texCoord, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Color4 color, in Vector2 texCoord)
            : this(position, (Half4)color, texCoord, Half3.UnitZ, Half3.UnitX)
        {
        }

        public Vertex(in Vector3 position, in Half3 normal, in Half3 tangent)
            : this(position, Half4.One, Vector2.Zero, normal, tangent)
        {
        }

        public Vertex(in Vector3 position, in Vector3 normal, in Vector3 tangent)
            : this(position, Half4.One, Vector2.Zero, (Half3)normal, (Half3)tangent)
        {
        }

        public Vertex(in Vector3 position, in Half4 color, in Half3 normal, in Half3 tangent)
            : this(position, color, Vector2.Zero, normal, tangent)
        {
        }

        public Vertex(in Vector3 position, in Color4 color, in Vector3 normal, in Vector3 tangent)
            : this(position, (Half4)color, Vector2.Zero, (Half3)normal, (Half3)tangent)
        {
        }

        public Vertex(in Vector3 position, in Vector2 texCoord, in Half3 normal, in Half3 tangent)
            : this(position, Half4.One, texCoord, normal, tangent)
        {
        }

        public Vertex(in Vector3 position, in Vector2 texCoord, in Vector3 normal, in Vector3 tangent)
            : this(position, Half4.One, texCoord, (Half3)normal, (Half3)tangent)
        {
        }

        public static explicit operator Vertex(in FVertex value)
        {
            return new Vertex(value.Position, (Half4)value.Color, value.TexCoord, (Half3)value.Normal, (Half3)value.Tangent);
        }

        public static bool operator ==(in Vertex a, in Vertex b) => a.Equals(b);
        public static bool operator !=(in Vertex a, in Vertex b) => !a.Equals(b);

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

        public bool Equals(Vertex other)
        {
            return Position == other.Position &&
                Color == other.Color &&
                TexCoord == other.TexCoord &&
                Normal == other.Normal &&
                Tangent == other.Tangent;
        }

        public override bool Equals(object obj) => obj is Vertex other && Equals(other);
    }
}
