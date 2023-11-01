using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Half4 : IEquatable<Half4>
    {
        public static readonly Half4 Zero = new Half4();
        public static readonly Half4 UnitX = new Half4(1, 0, 0, 0);
        public static readonly Half4 UnitY = new Half4(0, 1, 0, 0);
        public static readonly Half4 UnitZ = new Half4(0, 0, 1, 0);
        public static readonly Half4 UnitW = new Half4(0, 0, 0, 1);
        public static readonly Half4 One = new Half4(1, 1, 1, 1);

        public Half X;
        public Half Y;
        public Half Z;
        public Half W;

        public Half4(Half x, Half y, Half z, Half w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static bool operator ==(in Half4 left, in Half4 right) => left.Equals(right);
        public static bool operator !=(in Half4 left, in Half4 right) => !left.Equals(right);

        public static explicit operator Half4(in Vector4 value) => new Half4((Half)value.X, (Half)value.Y, (Half)value.Z, (Half)value.W);
        public static explicit operator Half4(in Color4 value) => new Half4((Half)value.R, (Half)value.G, (Half)value.B, (Half)value.A);
        public static implicit operator Vector4(in Half4 value) => new Vector4(value.X, value.Y, value.Z, value.W);

        public override string ToString() => $"X:{X} Y:{Y} Z:{Z} W:{W}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Z.GetHashCode());
            hash.Combine(W.GetHashCode());
            return hash;
        }

        public bool Equals(Half4 other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        public override bool Equals(object obj) => obj is Half4 other && Equals(other);
    }
}
