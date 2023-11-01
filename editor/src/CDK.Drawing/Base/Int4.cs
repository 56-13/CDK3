using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int4 : IEquatable<Int4>
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public Int4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static void Clamp(in Int4 vector, in Int4 min, in Int4 max, out Int4 result)
        {
            result.X = MathUtil.Clamp(vector.X, min.X, max.X);
            result.Y = MathUtil.Clamp(vector.Y, min.Y, max.Y);
            result.Z = MathUtil.Clamp(vector.Z, min.Z, max.Z);
            result.W = MathUtil.Clamp(vector.W, min.W, max.W);
        }
        public static Int4 Clamp(in Int4 vector, in Int4 min, in Int4 max)
        {
            Int4 result;
            Clamp(vector, min, max, out result);
            return result;
        }
        public void Clamp(in Int4 min, in Int4 max)
        {
            Clamp(this, min, max, out this);
        }
        public static void Max(in Int4 left, in Int4 right, out Int4 result)
        {
            result.X = Math.Max(left.X, right.X);
            result.Y = Math.Max(left.Y, right.Y);
            result.Z = Math.Max(left.Z, right.Z);
            result.W = Math.Max(left.W, right.W);
        }
        public static Int4 Max(in Int4 left, in Int4 right)
        {
            Int4 result;
            Max(left, right, out result);
            return result;
        }
        public static void Min(in Int4 left, in Int4 right, out Int4 result)
        {
            result.X = Math.Min(left.X, right.X);
            result.Y = Math.Min(left.Y, right.Y);
            result.Z = Math.Min(left.Z, right.Z);
            result.W = Math.Min(left.W, right.W);
        }
        public static Int4 Min(in Int4 left, in Int4 right)
        {
            Int4 result;
            Min(left, right, out result);
            return result;
        }
        public static Int4 operator -(in Int4 a) => new Int4(-a.X, -a.Y, -a.Z, -a.W);
        public static Int4 operator +(in Int4 left, in Int4 right) => new Int4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        public static Int4 operator -(in Int4 left, in Int4 right) => new Int4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        public static Int4 operator *(int scale, in Int4 value) => new Int4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        public static Int4 operator *(in Int4 value, int scale) => new Int4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        public static Int4 operator *(in Int4 left, in Int4 right) => new Int4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        public static Int4 operator /(in Int4 value, int scale) => new Int4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);

        public static bool operator ==(in Int4 left, in Int4 right) => left.Equals(right);
        public static bool operator !=(in Int4 left, in Int4 right) => !left.Equals(right);

        public static explicit operator Int4(in Vector4 value) => new Int4((int)value.X, (int)value.Y, (int)value.Z, (int)value.W);
        public static implicit operator Vector4(in Int4 value) => new Vector4(value.X, value.Y, value.Z, value.W);

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

        public bool Equals(Int4 other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        public override bool Equals(object obj) => obj is Int4 other && Equals(other);
    }
}
