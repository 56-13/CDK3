using System;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Bounds2 : IEquatable<Bounds2>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public static readonly Bounds2 Zero = new Bounds2();

        public Bounds2(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Bounds2(in Rectangle rect)
        {
            X = (int)Math.Floor(rect.X);
            Y = (int)Math.Floor(rect.Y); 
            Width = (int)Math.Ceiling(rect.Right) - X;
            Height = (int)Math.Ceiling(rect.Bottom) - Y;
        }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public float Center => (Left + Right) * 0.5f;
        public float Middle => (Top + Bottom) * 0.5f;
        public Int2 LeftTop => new Int2(Left, Top);
        public Int2 RightTop => new Int2(Right, Top);
        public Int2 LeftBottom => new Int2(Left, Bottom);
        public Int2 RightBottom => new Int2(Right, Bottom);

        public Int2 Origin
        {
            set
            {
                X = value.X;
                Y = value.Y;
            }
            get => new Int2(X, Y);
        }

        public Int2 Size
        {
            set
            {
                Width = value.X;
                Height = value.Y;
            }
            get => new Int2(Width, Height);
        }

        public bool Contains(int x, int y) => x >= Left && x <= Right && y >= Top && y <= Bottom;
        public bool Contains(in Int2 p) => Contains(p.X, p.Y);
        public bool Contains(in Bounds2 other) => Left <= other.Left && other.Right <= Right && Top <= other.Top && other.Bottom <= Bottom;
        public bool Intersects(in Bounds2 other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;

        public void Offset(in Int2 v) => Offset(v.X, v.Y);
        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public Bounds2 OffsetBounds(in Int2 v) => OffsetBounds(v.X, v.Y);
        public Bounds2 OffsetBounds(int x, int y)
        {
            var result = this;
            result.Offset(x, y);
            return result;
        }

        public void Inflate(in Int2 v) => Inflate(v.X, v.Y);
        public void Inflate(int w, int h)
        {
            if (Width + w * 2 <= 0)
            {
                X += w / 2;
                Width = 0;
            }
            else
            {
                X -= w;
                Width += w * 2;
            }
            if (Height + h * 2 <= 0)
            {
                Y += Height / 2;
                Height = 0;
            }
            else
            {
                Y -= h;
                Height += h * 2;
            }
        }
        public Bounds2 InflateBounds(in Int2 v) => InflateBounds(v.X, v.Y);
        public Bounds2 InflateBounds(int w, int h)
        {
            var result = this;
            result.Inflate(w, h);
            return result;
        }

        public static Bounds2 Intersect(in Bounds2 value1, in Bounds2 value2)
        {
            var result = value1;
            result.Intersect(value2);
            return result;
        }

        public static void Intersect(in Bounds2 value1, in Bounds2 value2, out Bounds2 result)
        {
            result = value1;
            result.Intersect(value2);
        }

        public Bounds2 IntersectBounds(in Bounds2 other)
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

        public static implicit operator Bounds2(in Rectangle value) => new Bounds2(value);

        public static bool operator ==(in Bounds2 left, in Bounds2 right) => left.Equals(right);
        public static bool operator !=(in Bounds2 left, in Bounds2 right) => !left.Equals(right);

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

        public bool Equals(Bounds2 other)
        {
            return other.X == X &&
                other.Y == Y &&
                other.Width == Width &&
                other.Height == Height;
        }

        public override bool Equals(object obj) => obj is Bounds2 other && Equals(other);
    }
}
