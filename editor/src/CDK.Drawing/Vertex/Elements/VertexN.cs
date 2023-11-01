using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct VertexN : IEquatable<VertexN>
    {
        public Vector3 Position;
        public Half3 Normal;

        public static readonly VertexLayout[] SingleBufferVertexLayouts =
        {
            new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 18, 0, 0, true),
            new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 18, 12, 0, true)
        };

        public VertexN(in Vector3 position, in Half3 normal)
            : this()
        {
            Position = position;
            Normal = normal;
        }

        public VertexN(in Vector3 position, in Vector3 normal)
            : this(position, (Half3)normal)
        {
        }

        public VertexN(in Vector3 position)
            : this(position, Half3.UnitZ)
        {
            
        }

        public static bool operator ==(in VertexN a, in VertexN b) => a.Equals(b);
        public static bool operator !=(in VertexN a, in VertexN b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Normal.GetHashCode());
            return hash;
        }

        public bool Equals(VertexN other)
        {
            return Position == other.Position && Normal == other.Normal;
        }

        public override bool Equals(object obj) => obj is VertexN other && Equals(other);
    }
}
