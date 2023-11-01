using System;
using System.Numerics;
using System.Runtime.InteropServices;

using GDIRectangle = System.Drawing.Rectangle;
using GDIRectangleF = System.Drawing.RectangleF;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static readonly Rectangle Zero = new Rectangle();
        public static readonly Rectangle ZeroToOne = new Rectangle(0, 0, 1, 1);
        public static readonly Rectangle ScreenNone = new Rectangle(2, 2, -4, -4);
        public static readonly Rectangle ScreenFull = new Rectangle(-1, -1, 2, 2);

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(in Vector2 pos, float width, float height) : this(pos.X, pos.Y, width, height) { }
        public Rectangle(float x, float y, in Vector2 size) : this(x, y, size.X, size.Y) { }
        public Rectangle(in Vector2 pos, in Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;
        public float Center => (Left + Right) * 0.5f;
        public float Middle => (Top + Bottom) * 0.5f;
        public Vector2 LeftTop => new Vector2(Left, Top);
        public Vector2 RightTop => new Vector2(Right, Top);
        public Vector2 LeftBottom => new Vector2(Left, Bottom);
        public Vector2 RightBottom => new Vector2(Right, Bottom);
        public Vector2 CenterMiddle => new Vector2(Center, Middle);
        public float HalfWidth => Width * 0.5f;
        public float HalfHeight => Width * 0.5f;
        public Vector2 HalfSize => new Vector2(HalfWidth, HalfHeight);

        public Vector2 Origin
        {
            set
            {
                X = value.X;
                Y = value.Y;
            }
            get => new Vector2(X, Y);
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

        public Vector2[] GetQuad()
        {
            return new Vector2[]
            {
                LeftTop,
                RightTop,
                LeftBottom,
                RightBottom
            };
        }

        public bool Contains(float x, float y) => x >= Left && x <= Right && y >= Top && y <= Bottom;
        public bool Contains(in Vector2 p) => Contains(p.X, p.Y);
        public bool Contains(in Rectangle other) => Left <= other.Left && other.Right <= Right && Top <= other.Top && other.Bottom <= Bottom;
        public bool Intersects(in Rectangle other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;

        public void Offset(in Vector2 v) => Offset(v.X, v.Y);
        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }
        public Rectangle OffsetRect(in Vector2 v) => OffsetRect(v.X, v.Y);
        public Rectangle OffsetRect(float x, float y)
        {
            var result = this;
            result.Offset(x, y);
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
        public Rectangle InflateBounds(in Vector2 v) => InflateBounds(v.X, v.Y);
        public Rectangle InflateBounds(float w, float h)
        {
            var result = this;
            result.Inflate(w, h);
            return result;
        }
        
        public static Rectangle Intersect(in Rectangle value1, in Rectangle value2)
        {
            var result = value1;
            result.Intersect(value2);
            return result;
        }

        public static void Intersect(in Rectangle value1, in Rectangle value2, out Rectangle result)
        {
            result = value1;
            result.Intersect(value2);
        }

        public Rectangle IntersectBounds(in Rectangle other)
        {
            var result = this;
            result.Intersect(other);
            return result;
        }

        public void Intersect(in Bounds2 other)
        {
            X = Math.Max(Left, other.Left);
            Y = Math.Max(Top, other.Top);
            Width = Math.Max(Math.Min(Right, other.Right) - X, 0);
            Height = Math.Max(Math.Min(Bottom, other.Bottom) - Y, 0);
        }

        public bool OnScreen => Left <= 1 && Right >= -1 && Top <= 1 && Bottom >= -1;
        public bool FullScreen => Left <= -1 && Right >= 1 && Top <= -1 && Bottom >= 1;
        public void ClipScreen()
        {
            if (Left < -1) X = -1;
            if (Right > 1) Width = 1 - X;
            if (Top < -1) Y = -1;
            if (Bottom > 1) Height = 1 - Y;
        }

        public void Append(float x, float y)
        {
            if (Left > x) X = x;
            if (Right < x) Width = x - X;
            if (Top > y) Y = y;
            if (Bottom < y) Height = y - Y;
        }

        public void Append(in Vector2 p)
        {
            Append(p.X, p.Y);
        }

        public void Append(in Rectangle value)
        {
            Append(this, value, out this);
        }

        public static Rectangle Append(in Rectangle value1, in Rectangle value2)
        {
            Append(value1, value2, out var result);
            return result;
        }

        public static void Append(in Rectangle value1, in Rectangle value2, out Rectangle result)
        {
            var left = Math.Min(value1.Left, value2.Left);
            var right = Math.Max(value1.Right, value2.Right);
            var top = Math.Min(value1.Top, value2.Top);
            var bottom = Math.Max(value1.Bottom, value2.Bottom);
            result = new Rectangle(left, top, right - left, bottom - top);
        }

        public static void Lerp(in Rectangle start, in Rectangle end, float amount, out Rectangle result)
        {
            result.X = MathUtil.Lerp(start.Left, end.Left, amount);
            result.Y = MathUtil.Lerp(start.Top, end.Top, amount);
            result.Width = MathUtil.Lerp(start.Right, end.Right, amount) - result.X;
            result.Height = MathUtil.Lerp(start.Bottom, end.Bottom, amount) - result.Y;
        }

        public static Rectangle Lerp(in Rectangle start, in Rectangle end, float amount)
        {
            Lerp(start, end, amount, out var result);
            return result;
        }

        public static bool operator ==(in Rectangle left, in Rectangle right) => left.Equals(right);
        public static bool operator !=(in Rectangle left, in Rectangle right) => !left.Equals(right);

        public static explicit operator Rectangle(in ZRectangle value) => new Rectangle(value.X, value.Y, value.Width, value.Height);
        public static explicit operator GDIRectangle(in Rectangle value) => new GDIRectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        public static implicit operator GDIRectangleF(in Rectangle value) => new GDIRectangleF(value.X, value.Y, value.Width, value.Height);
        public static implicit operator Rectangle(in GDIRectangle value) => new Rectangle(value.X, value.Y, value.Width, value.Height);
        public static implicit operator Rectangle(in GDIRectangleF value) => new Rectangle(value.X, value.Y, value.Width, value.Height);

        public override string ToString() => $"X:{X} Y:{Y} Width:{Width} Height:{Height}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            return hash;
        }

        public bool Equals(Rectangle other)
        {
            return other.X == X &&
                other.Y == Y &&
                other.Width == Width &&
                other.Height == Height;
        }

        public override bool Equals(object obj) => obj is Rectangle other && Equals(other);
    }
}
