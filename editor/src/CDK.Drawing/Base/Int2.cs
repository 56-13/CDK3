using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int2 : IEquatable<Int2>
    {
        public int X;
        public int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static void Clamp(in Int2 vector, in Int2 min, in Int2 max, out Int2 result)
        {
            result.X = MathUtil.Clamp(vector.X, min.X, max.X);
            result.Y = MathUtil.Clamp(vector.Y, min.Y, max.Y);
        }
        public static Int2 Clamp(in Int2 vector, in Int2 min, in Int2 max)
        {
            Int2 result;
            Clamp(vector, min, max, out result);
            return result;
        }
        public void Clamp(in Int2 min, in Int2 max)
        {
            Clamp(this, min, max, out this);
        }
        public static void Max(in Int2 left, in Int2 right, out Int2 result)
        {
            result.X = Math.Max(left.X, right.X);
            result.Y = Math.Max(left.Y, right.Y);
        }
        public static Int2 Max(in Int2 left, in Int2 right)
        {
            Int2 result;
            Max(left, right, out result);
            return result;
        }
        public static void Min(in Int2 left, in Int2 right, out Int2 result)
        {
            result.X = Math.Min(left.X, right.X);
            result.Y = Math.Min(left.Y, right.Y);
        }
        public static Int2 Min(in Int2 left, in Int2 right)
        {
            Int2 result;
            Min(left, right, out result);
            return result;
        }
        public static Int2 operator -(in Int2 a) => new Int2(-a.X, -a.Y);
        public static Int2 operator +(in Int2 left, in Int2 right) => new Int2(left.X + right.X, left.Y + right.Y);
        public static Int2 operator -(in Int2 left, in Int2 right) => new Int2(left.X - right.X, left.Y - right.Y);
        public static Int2 operator *(int scale, in Int2 value) => new Int2(value.X * scale, value.Y * scale);
        public static Int2 operator *(in Int2 value, int scale) => new Int2(value.X * scale, value.Y * scale);
        public static Int2 operator *(in Int2 left, in Int2 right) => new Int2(left.X * right.X, left.Y * right.Y);
        public static Int2 operator /(in Int2 value, int scale) => new Int2(value.X / scale, value.Y / scale);

        public static bool operator ==(in Int2 left, in Int2 right) => left.Equals(right);
        public static bool operator !=(in Int2 left, in Int2 right) => !left.Equals(right);

        public static explicit operator Int2(in Vector2 value) => new Int2((int)value.X, (int)value.Y);
        public static implicit operator Vector2(in Int2 value) => new Vector2(value.X, value.Y);

        public override string ToString() => $"X:{X} Y:{Y}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            return hash;
        }

        public bool Equals(Int2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Int2 other && Equals(other);
    }
}
