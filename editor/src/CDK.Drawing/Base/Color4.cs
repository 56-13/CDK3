using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IEquatable<Color4>
    {
        public static readonly Color4 Black = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Color4 White = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Color4 Transparent = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Color4 Gray = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
        public static readonly Color4 DarkGray = new Color4(0.25f, 0.25f, 0.25f, 1.0f);
        public static readonly Color4 LightGray = new Color4(0.75f, 0.75f, 0.75f, 1.0f);
        public static readonly Color4 Red = new Color4(1.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Color4 DarkRed = new Color4(0.5f, 0.0f, 0.0f, 1.0f);
        public static readonly Color4 LightRed = new Color4(1.0f, 0.5f, 0.5f, 1.0f);
        public static readonly Color4 Green = new Color4(0.0f, 1.0f, 0.0f, 1.0f);
        public static readonly Color4 DarkGreen = new Color4(0.0f, 0.5f, 0.0f, 1.0f);
        public static readonly Color4 LightGreen = new Color4(0.5f, 1.0f, 0.5f, 1.0f);
        public static readonly Color4 Blue = new Color4(0.0f, 0.0f, 1.0f, 1.0f);
        public static readonly Color4 DarkBlue = new Color4(0.0f, 0.0f, 0.5f, 1.0f);
        public static readonly Color4 LightBlue = new Color4(0.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Color4 Orange = new Color4(1.0f, 0.5f, 0.0f, 1.0f);
        public static readonly Color4 Yellow = new Color4(1.0f, 1.0f, 0.0f, 1.0f);
        public static readonly Color4 Magenta = new Color4(1.0f, 0.0f, 1.0f, 1.0f);
        public static readonly Color4 Brown = new Color4(0.282f, 0.227f, 0.176f, 1.0f);
        public static readonly Color4 TranslucentBlack = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        public static readonly Color4 TranslucentWhite = new Color4(1.0f, 1.0f, 1.0f, 0.5f);
        public static readonly Color4 FaintBlack = new Color4(0.0f, 0.0f, 0.0f, 0.25f);
        public static readonly Color4 FaintWhite = new Color4(1.0f, 1.0f, 1.0f, 0.25f);

        public float R;
        public float G;
        public float B;
        public float A;

        public Color4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color4(byte r, byte g, byte b, byte a)
        {
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;
            A = a / 255.0f;
        }

        public Color4(in Vector4 value)
        {
            R = value.X;
            G = value.Y;
            B = value.Z;
            A = value.W;
        }

        public Color4(in Vector3 value, float alpha)
        {
            R = value.X;
            G = value.Y;
            B = value.Z;
            A = alpha;
        }

        public Color4(uint rgba)
        {
            R = ((rgba >> 24) & 255) / 255.0f;
            G = ((rgba >> 16) & 255) / 255.0f;
            B = ((rgba >> 8) & 255) / 255.0f;
            A = (rgba & 255) / 255.0f;
        }

        public Color4(uint rgba, float intensity) : this(rgba)
        {
            R *= intensity;
            G *= intensity;
            B *= intensity;
        }

        public Color4(in Color4 color, float intensity)
        {
            R = color.R * intensity;
            G = color.G * intensity;
            B = color.B * intensity;
            A = color.A;
        }

        public Color4(float[] values)
        {
            R = values[0];
            G = values[1];
            B = values[2];
            A = values[3];
        }

        public Color4(in Color3 color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = 1.0f;
        }

        public Color4(in Color3 color, float alpha)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = alpha;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R;
                    case 1: return G;
                    case 2: return B;
                    case 3: return A;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Color4 run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: R = value; break;
                    case 1: G = value; break;
                    case 2: B = value; break;
                    case 3: A = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Color4 run from 0 to 3, inclusive.");
                }
            }
        }

        public float Amplification => Math.Max(Math.Max(Math.Max(R, G), B), 1);
        public float Brightness => (R * Color3.LumCoeff.R) + (G * Color3.LumCoeff.G) + (B * Color3.LumCoeff.B);
        public Color4 Normalized
        {
            get
            {
                var amp = Amplification;

                return amp > 1 ? new Color4(R / amp, G / amp, B / amp, A) : this;
            }
        }

        public byte NormalR => (byte)MathUtil.Clamp(Math.Round(R * 255), 0, 255);
        public byte NormalG => (byte)MathUtil.Clamp(Math.Round(G * 255), 0, 255);
        public byte NormalB => (byte)MathUtil.Clamp(Math.Round(B * 255), 0, 255);
        public byte NormalA => (byte)MathUtil.Clamp(Math.Round(A * 255), 0, 255);
        public uint ToRgba() => ((uint)NormalR << 24) | ((uint)NormalG << 16) | ((uint)NormalB << 8) | NormalA;
        public float[] ToArray() => new float[] { R, G, B, A };

        public static void Add(in Color4 left, in Color4 right, out Color4 result)
        {
            result.A = left.A + right.A;
            result.R = left.R + right.R;
            result.G = left.G + right.G;
            result.B = left.B + right.B;
        }

        public static Color4 Add(in Color4 left, in Color4 right) => new Color4(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);

        public static void Subtract(in Color4 left, in Color4 right, out Color4 result)
        {
            result.A = left.A - right.A;
            result.R = left.R - right.R;
            result.G = left.G - right.G;
            result.B = left.B - right.B;
        }

        public static Color4 Subtract(in Color4 left, in Color4 right) => new Color4(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);

        public static void Modulate(in Color4 left, in Color4 right, out Color4 result)
        {
            result.A = left.A * right.A;
            result.R = left.R * right.R;
            result.G = left.G * right.G;
            result.B = left.B * right.B;
        }

        public static Color4 Modulate(in Color4 left, in Color4 right) => new Color4(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);

        public static void Scale(in Color4 value, float scale, out Color4 result)
        {
            result.A = value.A * scale;
            result.R = value.R * scale;
            result.G = value.G * scale;
            result.B = value.B * scale;
        }

        public static Color4 Scale(in Color4 value, float scale) => new Color4(value.R * scale, value.G * scale, value.B * scale, value.A * scale);

        public static void Negate(in Color4 value, out Color4 result)
        {
            result.A = 1.0f - value.A;
            result.R = 1.0f - value.R;
            result.G = 1.0f - value.G;
            result.B = 1.0f - value.B;
        }

        public static Color4 Negate(in Color4 value) => new Color4(1.0f - value.R, 1.0f - value.G, 1.0f - value.B, 1.0f - value.A);

        public static void Clamp(in Color4 value, in Color4 min, in Color4 max, out Color4 result)
        {
            var alpha = value.A;
            alpha = (alpha > max.A) ? max.A : alpha;
            alpha = (alpha < min.A) ? min.A : alpha;

            var red = value.R;
            red = (red > max.R) ? max.R : red;
            red = (red < min.R) ? min.R : red;

            var green = value.G;
            green = (green > max.G) ? max.G : green;
            green = (green < min.G) ? min.G : green;

            var blue = value.B;
            blue = (blue > max.B) ? max.B : blue;
            blue = (blue < min.B) ? min.B : blue;

            result = new Color4(red, green, blue, alpha);
        }

        public static Color4 Clamp(in Color4 value, in Color4 min, in Color4 max)
        {
            Clamp(value, min, max, out var result);
            return result;
        }

        public static void Lerp(in Color4 start, in Color4 end, float amount, out Color4 result)
        {
            result.R = MathUtil.Lerp(start.R, end.R, amount);
            result.G = MathUtil.Lerp(start.G, end.G, amount);
            result.B = MathUtil.Lerp(start.B, end.B, amount);
            result.A = MathUtil.Lerp(start.A, end.A, amount);
        }

        public static Color4 Lerp(in Color4 start, in Color4 end, float amount)
        {
            Lerp(start, end, amount, out var result);
            return result;
        }

        public static void SmoothStep(in Color4 start, in Color4 end, float amount, out Color4 result)
        {
            amount = MathUtil.SmoothStep(amount);
            Lerp(start, end, amount, out result);
        }

        public static Color4 SmoothStep(in Color4 start, in Color4 end, float amount)
        {
            SmoothStep(start, end, amount, out var result);
            return result;
        }

        public static void Max(in Color4 left, in Color4 right, out Color4 result)
        {
            result.A = (left.A > right.A) ? left.A : right.A;
            result.R = (left.R > right.R) ? left.R : right.R;
            result.G = (left.G > right.G) ? left.G : right.G;
            result.B = (left.B > right.B) ? left.B : right.B;
        }

        public static Color4 Max(in Color4 left, in Color4 right)
        {
            Max(left, right, out var result);
            return result;
        }

        public static void Min(in Color4 left, in Color4 right, out Color4 result)
        {
            result.A = (left.A < right.A) ? left.A : right.A;
            result.R = (left.R < right.R) ? left.R : right.R;
            result.G = (left.G < right.G) ? left.G : right.G;
            result.B = (left.B < right.B) ? left.B : right.B;
        }

        public static Color4 Min(in Color4 left, in Color4 right)
        {
            Min(left, right, out var result);
            return result;
        }

        public static void AdjustContrast(in Color4 value, float contrast, out Color4 result)
        {
            result.A = value.A;
            result.R = 0.5f + contrast * (value.R - 0.5f);
            result.G = 0.5f + contrast * (value.G - 0.5f);
            result.B = 0.5f + contrast * (value.B - 0.5f);
        }

        public static Color4 AdjustContrast(in Color4 value, float contrast)
        {
            return new Color4(
                0.5f + contrast * (value.R - 0.5f),
                0.5f + contrast * (value.G - 0.5f),
                0.5f + contrast * (value.B - 0.5f),
                value.A);
        }

        public static void AdjustSaturation(in Color4 value, float saturation, out Color4 result)
        {
            var grey = value.R * 0.2125f + value.G * 0.7154f + value.B * 0.0721f;

            result.A = value.A;
            result.R = grey + saturation * (value.R - grey);
            result.G = grey + saturation * (value.G - grey);
            result.B = grey + saturation * (value.B - grey);
        }

        public static Color4 AdjustSaturation(in Color4 value, float saturation)
        {
            var grey = value.R * 0.2125f + value.G * 0.7154f + value.B * 0.0721f;

            return new Color4(
                grey + saturation * (value.R - grey),
                grey + saturation * (value.G - grey),
                grey + saturation * (value.B - grey),
                value.A);
        }

        public static void Premultiply(in Color4 value, out Color4 result)
        {
            result.A = value.A;
            result.R = value.R * value.A;
            result.G = value.G * value.A;
            result.B = value.B * value.A;
        }

        public static Color4 Premultiply(in Color4 value)
        {
            Premultiply(value, out var result);
            return result;
        }

        public static Color4 operator +(in Color4 left, in Color4 right) => new Color4(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);
        public static Color4 operator -(in Color4 left, in Color4 right) => new Color4(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);
        public static Color4 operator *(float scale, in Color4 value) => new Color4(value.R * scale, value.G * scale, value.B * scale, value.A * scale);
        public static Color4 operator *(in Color4 value, float scale) => new Color4(value.R * scale, value.G * scale, value.B * scale, value.A * scale);
        public static Color4 operator *(in Color4 left, in Color4 right) => new Color4(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);
        public static Color4 operator /(in Color4 value, float scale) => new Color4(value.R / scale, value.G / scale, value.B / scale, value.A / scale);
        public static bool operator ==(in Color4 left, in Color4 right) => left.Equals(right);
        public static bool operator !=(in Color4 left, in Color4 right) => !left.Equals(right);

        public static implicit operator Color4(in Color3 value) => new Color4(value.R, value.G, value.B, 1.0f);
        public static implicit operator Color4(in Vector3 value) => new Color4(value.X, value.Y, value.Z, 1.0f);
        public static implicit operator Color4(in Vector4 value) => new Color4(value.X, value.Y, value.Z, value.W);
        public static explicit operator uint(in Color4 value) => value.ToRgba();
        public static implicit operator Color4(uint value) => new Color4(value);
        public static implicit operator Color4(in Half3 value) => new Color4(value.X, value.Y, value.Z, 1.0f);
        public static implicit operator Color4(in Half4 value) => new Color4(value.X, value.Y, value.Z, value.W);
        public static explicit operator System.Drawing.Color(in Color4 value)
        {
            return System.Drawing.Color.FromArgb(
                MathUtil.Clamp((int)(value.A * 255), 0, 255),
                MathUtil.Clamp((int)(value.R * 255), 0, 255),
                MathUtil.Clamp((int)(value.G * 255), 0, 255),
                MathUtil.Clamp((int)(value.B * 255), 0, 255));
        }

        public static implicit operator Color4(in System.Drawing.Color value) => new Color4(value.R, value.G, value.B, value.A);

        public override string ToString()
        {
            if (R <= 1 && G <= 1 && B <= 1 && A <= 1) return $"{(int)(R * 255):X2}{(int)(G * 255):X2}{(int)(B * 255):X2}{(int)(A * 255):X2}";

            return $"R:{R:F3} G:{G:F3} B:{B:F3} A:{A:F3}";
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(R.GetHashCode());
            hash.Combine(G.GetHashCode());
            hash.Combine(B.GetHashCode());
            hash.Combine(A.GetHashCode());
            return hash;
        }

        public bool Equals(Color4 other) => R == other.R && G == other.G && B == other.B && A == other.A;
        public override bool Equals(object obj) => obj is Color4 other && Equals(other);
    }
}
