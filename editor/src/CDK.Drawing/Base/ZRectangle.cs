using System;
using System.Numerics;
using System.Runtime.InteropServices;

using GDIRectangle = System.Drawing.Rectangle;
using GDIRectangleF = System.Drawing.RectangleF;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ZRectangle : IEquatable<ZRectangle>
    {
        public float X;
        public float Y;
        public float Z;
        public float Width;
        public float Height;

        public ZRectangle(float x, float y, float z, float width, float height)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
        }

        public ZRectangle(in Vector3 pos, float width, float height) : this(pos.X, pos.Y, pos.Z, width, height) { }
        public ZRectangle(float x, float y, float z, in Vector2 size) : this(x, y, z, size.X, size.Y) { }
        public ZRectangle(in Vector3 pos, in Vector2 size) : this(pos.X, pos.Y, pos.Z, size.X, size.Y) { }

        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;
        public float Center => (Left + Right) * 0.5f;
        public float Middle => (Top + Bottom) * 0.5f;
        public Vector3 LeftTop => new Vector3(Left, Top, Z);
        public Vector3 RightTop => new Vector3(Right, Top, Z);
        public Vector3 LeftBottom => new Vector3(Left, Bottom, Z);
        public Vector3 RightBottom => new Vector3(Right, Bottom, Z);
        public Vector3 CenterMiddle => new Vector3(Center, Middle, Z);
        public float HalfWidth => Width * 0.5f;
        public float HalfHeight => Width * 0.5f;
        public Vector2 HalfSize => new Vector2(HalfWidth, HalfHeight);

        public Vector3 Origin
        {
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
            get => new Vector3(X, Y, Z);
        }

        public Vector2 Size
        {
            set
            {
                Width = value.X;
                Height = value.Y;
            }
            get => new Vector2(Width, Height);
        }

        public Vector3[] GetQuad()
        {
            return new Vector3[]
            {
                LeftTop,
                RightTop,
                LeftBottom,
                RightBottom
            };
        }

        public bool Contains(float x, float y, float z) => x >= Left && x <= Right && y >= Top && y <= Bottom && MathUtil.NearEqual(Z, z);
        public bool Contains(in Vector3 p) => Contains(p.X, p.Y, p.Z);
        public bool Contains(in ZRectangle other) => Left <= other.Left && other.Right <= Right && Top <= other.Top && other.Bottom <= Bottom && MathUtil.NearEqual(Z, other.Z);
        public bool Intersects(in ZRectangle other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom && MathUtil.NearEqual(Z, other.Z);

        public void Offset(in Vector3 v) => Offset(v.X, v.Y, v.Z);
        public void Offset(float x, float y, float z)
        {
            X += x;
            Y += y;
            Z += z;
        }
        public ZRectangle OffsetBounds(in Vector3 v) => OffsetBounds(v.X, v.Y, v.Z);
        public ZRectangle OffsetBounds(float x, float y, float z)
        {
            var result = this;
            result.Offset(x, y, z);
            return result;
        }

        public void Inflate(float w, float h)
        {
            if (Width + w * 2 <= 0)
            {
                X += w * 0.5f;
                Width = 0;
            }
            else
            {
                X -= w;
                Width += w * 2;
            }
            if (Height + h * 2 <= 0)
            {
                Y += Height * 0.5f;
                Height = 0;
            }
            else
            {
                Y -= h;
                Height += h * 2;
            }
        }
        public ZRectangle InflateBounds(in Vector2 v) => InflateBounds(v.X, v.Y);
        public ZRectangle InflateBounds(float w, float h)
        {
            var result = this;
            result.Inflate(w, h);
            return result;
        }

        public static void Lerp(in ZRectangle start, in ZRectangle end, float amount, out ZRectangle result)
        {
            result.X = MathUtil.Lerp(start.Left, end.Left, amount);
            result.Y = MathUtil.Lerp(start.Top, end.Top, amount);
            result.Z = MathUtil.Lerp(start.Z, end.Z, amount);
            result.Width = MathUtil.Lerp(start.Right, end.Right, amount) - result.X;
            result.Height = MathUtil.Lerp(start.Bottom, end.Bottom, amount) - result.Y;
        }

        public static ZRectangle Lerp(in ZRectangle start, in ZRectangle end, float amount)
        {
            Lerp(start, end, amount, out var result);
            return result;
        }

        public static bool operator ==(in ZRectangle left, in ZRectangle right) => left.Equals(right);
        public static bool operator !=(in ZRectangle left, in ZRectangle right) => !left.Equals(right);

        public static implicit operator ZRectangle(in Rectangle value) => new ZRectangle(value.X, value.Y, 0, value.Width, value.Height);
        public static explicit operator GDIRectangle(in ZRectangle value) => new GDIRectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        public static explicit operator GDIRectangleF(in ZRectangle value) => new GDIRectangleF(value.X, value.Y, value.Width, value.Height);
        public static implicit operator ZRectangle(in GDIRectangle value) => new ZRectangle(value.X, value.Y, 0, value.Width, value.Height);
        public static implicit operator ZRectangle(in GDIRectangleF value) => new ZRectangle(value.X, value.Y, 0, value.Width, value.Height);

        public override string ToString() => $"X:{X} Y:{Y} Z:{Z} Width:{Width} Height:{Height}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Z.GetHashCode());
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            return hash;
        }

        public bool Equals(ZRectangle other)
        {
            return other.X == X &&
                other.Y == Y &&
                other.Z == Z &&
                other.Width == Width &&
                other.Height == Height;
        }

        public override bool Equals(object obj) => obj is ZRectangle other && Equals(other);
    }
}
