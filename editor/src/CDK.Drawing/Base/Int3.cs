using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int3 : IEquatable<Int3>
    {
        public int X;
        public int Y;
        public int Z;

        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static void Clamp(in Int3 vector, in Int3 min, in Int3 max, out Int3 result)
        {
            result.X = MathUtil.Clamp(vector.X, min.X, max.X);
            result.Y = MathUtil.Clamp(vector.Y, min.Y, max.Y);
            result.Z = MathUtil.Clamp(vector.Z, min.Z, max.Z);
        }
        public static Int3 Clamp(in Int3 vector, in Int3 min, in Int3 max)
        {
            Int3 result;
            Clamp(vector, min, max, out result);
            return result;
        }
        public void Clamp(in Int3 min, in Int3 max)
        {
            Clamp(this, min, max, out this);
        }
        public static void Max(in Int3 left, in Int3 right, out Int3 result)
        {
            result.X = Math.Max(left.X, right.X);
            result.Y = Math.Max(left.Y, right.Y);
            result.Z = Math.Max(left.Z, right.Z);
        }
        public static Int3 Max(in Int3 left, in Int3 right)
        {
            Int3 result;
            Max(left, right, out result);
            return result;
        }
        public static void Min(in Int3 left, in Int3 right, out Int3 result)
        {
            result.X = Math.Min(left.X, right.X);
            result.Y = Math.Min(left.Y, right.Y);
            result.Z = Math.Min(left.Z, right.Z);
        }
        public static Int3 Min(in Int3 left, in Int3 right)
        {
            Int3 result;
            Min(left, right, out result);
            return result;
        }
        public static Int3 operator -(in Int3 a) => new Int3(-a.X, -a.Y, -a.Z);
        public static Int3 operator +(in Int3 left, in Int3 right) => new Int3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Int3 operator -(in Int3 left, in Int3 right) => new Int3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        public static Int3 operator *(int scale, in Int3 value) => new Int3(value.X * scale, value.Y * scale, value.Z * scale);
        public static Int3 operator *(in Int3 value, int scale) => new Int3(value.X * scale, value.Y * scale, value.Z * scale);
        public static Int3 operator *(in Int3 left, in Int3 right) => new Int3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        public static Int3 operator /(in Int3 value, int scale) => new Int3(value.X / scale, value.Y / scale, value.Z / scale);

        public static bool operator ==(in Int3 left, in Int3 right) => left.Equals(right);
        public static bool operator !=(in Int3 left, in Int3 right) => !left.Equals(right);

        public static explicit operator Int3(in Vector3 value) => new Int3((int)value.X, (int)value.Y, (int)value.Z);
        public static implicit operator Vector3(in Int3 value) => new Vector3(value.X, value.Y, value.Z);

        public override string ToString() => $"X:{X} Y:{Y} Z:{Z}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Z.GetHashCode());
            return hash;
        }

        public bool Equals(Int3 other) => X == other.X && Y == other.Y && Z == other.Z;
        public override bool Equals(object obj) => obj is Int3 other && Equals(other);
    }
}
