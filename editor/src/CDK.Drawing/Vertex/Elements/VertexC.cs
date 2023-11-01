using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexC : IEquatable<VertexC>
    {
        public Vector3 Position;
        public Half4 Color;

        public static readonly VertexLayout[] SingleBufferVertexCLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 20, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 20, 12, 0, true)
        };

        public VertexC(in Vector3 position, in Half4 color)
            : this()
        {
            Position = position;
            Color = color;
        }

        public VertexC(in Vector3 position, in Color4 color)
            : this(position, (Half4)color)
        {
        }

        public static bool operator ==(in VertexC a, in VertexC b) => a.Equals(b);
        public static bool operator !=(in VertexC a, in VertexC b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            return hash;
        }

        public bool Equals(VertexC other)
        {
            return Position == other.Position && Color == other.Color;
        }

        public override bool Equals(object obj) => obj is VertexC other && Equals(other);
    }
}
