using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Half3 : IEquatable<Half3>
    {
        public static readonly Half3 Zero = new Half3();
        public static readonly Half3 UnitX = new Half3(1, 0, 0);
        public static readonly Half3 UnitY = new Half3(0, 1, 0);
        public static readonly Half3 UnitZ = new Half3(0, 0, 1);
        public static readonly Half3 One = new Half3(1, 1, 1);

        public Half X;
        public Half Y;
        public Half Z;

        public Half3(Half x, Half y, Half z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(in Half3 left, in Half3 right) => left.Equals(right);
        public static bool operator !=(in Half3 left, in Half3 right) => !left.Equals(right);

        public static explicit operator Half3(in Vector3 value) => new Half3((Half)value.X, (Half)value.Y, (Half)value.Z);
        public static explicit operator Half3(in Color3 value) => new Half3((Half)value.R, (Half)value.G, (Half)value.B);
        public static implicit operator Vector3(in Half3 value) => new Vector3(value.X, value.Y, value.Z);
        
        public override string ToString() => $"X:{X} Y:{Y} Z:{Z}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Z.GetHashCode());
            return hash;
        }

        public bool Equals(Half3 other) => X == other.X && Y == other.Y && Z == other.Z;
        public override bool Equals(object obj) => obj is Half3 other && Equals(other);
    }
}
