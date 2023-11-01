using System;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Bounds3 : IEquatable<Bounds3>
    {
        public int X;
        public int Y;
        public int Z;
        public int Width;
        public int Height;
        public int Depth;

        public static readonly Bounds3 Zero = new Bounds3();

        public Bounds3(int x, int y, int z, int width, int height, int depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public float Center => (Left + Right) * 0.5f;
        public float Middle => (Top + Bottom) * 0.5f;
        public int Near => Z;
        public int Far => Z + Depth;
        public Int3 Min => new Int3(X, Y, Z);
        public Int3 Max => new Int3(X + Width, Y + Height, Z + Depth);
        public Int3 Origin
        {
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
            get => new Int3(X, Y, Z);
        }

        public Int3 Size
        {
            set
            {
                Width = value.X;
                Height = value.Y;
                Depth = value.Z;
            }
            get => new Int3(Width, Height, Depth);
        }

        public bool Contains(int x, int y, int z) => x >= Left && x <= Right && y >= Top && y <= Bottom && z >= Near && z <= Far;
        public bool Contains(in Int3 p) => Contains(p.X, p.Y, p.Z);
        public bool Contains(in Bounds3 other) => Left <= other.Left && other.Right <= Right && Top <= other.Top && other.Bottom <= Bottom && Near <= other.Near && other.Far <= Far;
        public bool Intersects(in Bounds3 other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom && other.Near < Far && Near < other.Far;

        public void Offset(in Int3 v) => Offset(v.X, v.Y, v.Z);
        public void Offset(int x, int y, int z)
        {
            X += x;
            Y += y;
            Z += z;
        }
        public Bounds3 OffsetBounds(in Int3 v) => OffsetBounds(v.X, v.Y, v.Z);
        public Bounds3 OffsetBounds(int x, int y, int z)
        {
            var result = this;
            result.Offset(x, y, z);
            return result;
        }

        public void Inflate(in Int3 v) => Inflate(v.X, v.Y, v.Z);
        public void Inflate(int w, int h, int d)
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
            if (Depth + d * 2 <= 0)
            {
                Z += Depth / 2;
                Depth = 0;
            }
            else
            {
                Z -= d;
                Depth += d * 2;
            }
        }
        public Bounds3 InflateBounds(in Int3 v) => InflateBounds(v.X, v.Y, v.Z);
        public Bounds3 InflateBounds(int w, int h, int d)
        {
            var result = this;
            result.Inflate(w, h, d);
            return result;
        }

        public static Bounds3 Intersect(in Bounds3 value1, in Bounds3 value2)
        {
            var result = value1;
            result.Intersect(value2);
            return result;
        }
        public Bounds3 IntersectBounds(in Bounds3 other)
        {
            var result = this;
            result.Intersect(other);
            return result;
        }

        public static void Intersect(in Bounds3 value1, in Bounds3 value2, out Bounds3 result)
        {
            result = value1;
            result.Intersect(value2);
        }

        public void Intersect(in Bounds3 other)
        {
            X = Math.Max(Left, other.Left);
            Y = Math.Max(Top, other.Top);
            Z = Math.Max(Near, other.Near);
            Width = Math.Max(Math.Min(Right, other.Right) - X, 0);
            Height = Math.Max(Math.Min(Bottom, other.Bottom) - Y, 0);
            Depth = Math.Max(Math.Min(Far, other.Far) - Z, 0);
        }

        public static bool operator ==(in Bounds3 left, in Bounds3 right) => left.Equals(right);
        public static bool operator !=(in Bounds3 left, in Bounds3 right) => !left.Equals(right);

        public override string ToString() => $"X:{X} Y:{Y} Z:{Z} Width:{Width} Height:{Height} Depth:{Depth}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(X.GetHashCode());
            hash.Combine(Y.GetHashCode());
            hash.Combine(Z.GetHashCode());
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            hash.Combine(Depth.GetHashCode());
            return hash;
        }

        public bool Equals(Bounds3 other)
        {
            return other.X == X &&
                other.Y == Y &&
                other.Z == Z &&
                other.Width == Width &&
                other.Height == Height &&
                other.Depth == Depth;
        }

        public override bool Equals(object obj) => obj is Bounds3 other && Equals(other);
    }
}
