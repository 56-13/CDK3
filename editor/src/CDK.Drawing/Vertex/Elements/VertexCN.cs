using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct VertexCN : IEquatable<VertexCN>
    {
        public Vector3 Position;
        public Half4 Color;
        public Half3 Normal;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 26, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 26, 12, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 26, 20, 0, true)
        };

        public VertexCN(in Vector3 position, in Half4 color, in Half3 normal)
            : this()
        {
            Position = position;
            Color = color;
            Normal = normal;
        }

        public VertexCN(in Vector3 position, in Color4 color, in Vector3 normal)
            : this(position, (Half4)color, (Half3)normal)
        {
        }

        public VertexCN(in Vector3 position, in Half4 color)
            : this(position, color, Half3.UnitZ)
        {
        }

        public VertexCN(in Vector3 position, in Color4 color)
            : this(position, (Half4)color, Half3.UnitZ)
        {
        }

        public static bool operator ==(in VertexCN a, in VertexCN b) => a.Equals(b);
        public static bool operator !=(in VertexCN a, in VertexCN b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            return hash;
        }

        public bool Equals(VertexCN other)
        {
            return Position == other.Position &&
                Color == other.Color &&
                Normal == other.Normal;
        }

        public override bool Equals(object obj) => obj is VertexCN other && Equals(other);
    }
}
