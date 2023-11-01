using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Half2 : IEquatable<Half2>
    {
        public static readonly Half2 Zero = new Half2();
        public static readonly Half2 UnitX = new Half2(1, 0);
        public static readonly Half2 UnitY = new Half2(0, 1);
        public static readonly Half2 One = new Half2(1, 1);

        public Half X;
        public Half Y;

        public Half2(Half x, Half y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(in Half2 left, in Half2 right) => left.Equals(right);
        public static bool operator !=(in Half2 left, in Half2 right) => !left.Equals(right);

        public static explicit operator Half2(in Vector2 value) => new Half2((Half)value.X, (Half)value.Y);
        public static implicit operator Vector2(in Half2 value) => new Vector2(value.X, value.Y);
        
        public override string ToString() => $"X:{X} Y:{Y}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            return hash;
        }

        public bool Equals(Half2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Half2 other && Equals(other);
    }
}
