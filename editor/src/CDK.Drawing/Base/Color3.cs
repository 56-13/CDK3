using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color3 : IEquatable<Color3>
    {
        public static readonly Color3 Black = new Color3(0.0f, 0.0f, 0.0f);
        public static readonly Color3 White = new Color3(1.0f, 1.0f, 1.0f);
        public static readonly Color3 Gray = new Color3(0.5f, 0.5f, 0.5f);
        public static readonly Color3 DarkGray = new Color3(0.25f, 0.25f, 0.25f);
        public static readonly Color3 LightGray = new Color3(0.75f, 0.75f, 0.75f);
        public static readonly Color3 Red = new Color3(1.0f, 0.0f, 0.0f);
        public static readonly Color3 DarkRed = new Color3(0.5f, 0.0f, 0.0f);
        public static readonly Color3 LightRed = new Color3(1.0f, 0.5f, 0.5f);
        public static readonly Color3 Green = new Color3(0.0f, 1.0f, 0.0f);
        public static readonly Color3 DarkGreen = new Color3(0.0f, 0.5f, 0.0f);
        public static readonly Color3 LightGreen = new Color3(0.5f, 1.0f, 0.5f);
        public static readonly Color3 Blue = new Color3(0.0f, 0.0f, 1.0f);
        public static readonly Color3 DarkBlue = new Color3(0.0f, 0.0f, 0.5f);
        public static readonly Color3 LightBlue = new Color3(0.0f, 1.0f, 1.0f);
        public static readonly Color3 Orange = new Color3(1.0f, 0.5f, 0.0f);
        public static readonly Color3 Yellow = new Color3(1.0f, 1.0f, 0.0f);
        public static readonly Color3 Magenta = new Color3(1.0f, 0.0f, 1.0f);
        public static readonly Color3 Brown = new Color3(0.282f, 0.227f, 0.176f);

        public static readonly Color3 LumCoeff = new Color3(0.2126f, 0.7152f, 0.0722f);

        public float R;
        public float G;
        public float B;

        public Color3(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color3(byte r, byte g, byte b)
        {
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;
        }

        public Color3(in Vector3 value)
        {
            R = value.X;
            G = value.Y;
            B = value.Z;
        }

        public Color3(uint rgba)
        {
            R = ((rgba >> 24) & 255) / 255.0f;
            G = ((rgba >> 16) & 255) / 255.0f;
            B = ((rgba >> 8) & 255) / 255.0f;
        }

        public Color3(uint rgba, float intensity) : this(rgba)
        {
            R *= intensity;
            G *= intensity;
            B *= intensity;
        }

        public Color3(in Color3 color, float intensity)
        {
            R = color.R * intensity;
            G = color.G * intensity;
            B = color.B * intensity;
        }

        public Color3(float[] values)
        {
            R = values[0];
            G = values[1];
            B = values[2];
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
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Color3 run from 0 to 2, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: R = value; break;
                    case 1: G = value; break;
                    case 2: B = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Color3 run from 0 to 2, inclusive.");
                }
            }
        }

        public float Amplification => Math.Max(Math.Max(Math.Max(R, G), B), 1);
        public float Brightness => (R * LumCoeff.R) + (G * LumCoeff.G) + (B * LumCoeff.B);
        public Color3 Normalized
        {
            get
            {
                var amp = Amplification;

                return amp > 1 ? new Color3(R / amp, G / amp, B / amp) : this;
            }
        }

        public byte NormalR => (byte)MathUtil.Clamp(Math.Round(R * 255), 0, 255);
        public byte NormalG => (byte)MathUtil.Clamp(Math.Round(G * 255), 0, 255);
        public byte NormalB => (byte)MathUtil.Clamp(Math.Round(B * 255), 0, 255);
        public uint ToRgba() => ((uint)NormalR << 24) | ((uint)NormalG << 16) | ((uint)NormalB << 8) | 255u;
        public float[] ToArray() => new float[] { R, G, B };

        public static void Add(in Color3 left, in Color3 right, out Color3 result)
        {
            result.R = left.R + right.R;
            result.G = left.G + right.G;
            result.B = left.B + right.B;
        }

        public static Color3 Add(in Color3 left, in Color3 right) => new Color3(left.R + right.R, left.G + right.G, left.B + right.B);

        public static void Subtract(in Color3 left, in Color3 right, out Color3 result)
        {
            result.R = left.R - right.R;
            result.G = left.G - right.G;
            result.B = left.B - right.B;
        }

        public static Color3 Subtract(in Color3 left, in Color3 right) => new Color3(left.R - right.R, left.G - right.G, left.B - right.B);

        public static void Modulate(in Color3 left, in Color3 right, out Color3 result)
        {
            result.R = left.R * right.R;
            result.G = left.G * right.G;
            result.B = left.B * right.B;
        }

        public static Color3 Modulate(in Color3 left, in Color3 right) => new Color3(left.R * right.R, left.G * right.G, left.B * right.B);

        public static void Scale(in Color3 value, float scale, out Color3 result)
        {
            result.R = value.R * scale;
            result.G = value.G * scale;
            result.B = value.B * scale;
        }

        public static Color3 Scale(in Color3 value, float scale) => new Color3(value.R* scale, value.G* scale, value.B* scale);

        public static void Negate(in Color3 value, out Color3 result)
        {
            result.R = 1.0f - value.R;
            result.G = 1.0f - value.G;
            result.B = 1.0f - value.B;
        }

        public static Color3 Negate(in Color3 value) => new Color3(1.0f - value.R, 1.0f - value.G, 1.0f - value.B);

        public static void Clamp(in Color3 value, in Color3 min, in Color3 max, out Color3 result)
        {
            var red = value.R;
            red = (red > max.R) ? max.R : red;
            red = (red < min.R) ? min.R : red;

            var green = value.G;
            green = (green > max.G) ? max.G : green;
            green = (green < min.G) ? min.G : green;

            var blue = value.B;
            blue = (blue > max.B) ? max.B : blue;
            blue = (blue < min.B) ? min.B : blue;

            result = new Color3(red, green, blue);
        }

        public static Color3 Clamp(in Color3 value, in Color3 min, in Color3 max)
        {
            Clamp(value, min, max, out var result);
            return result;
        }

        public static void Lerp(in Color3 start, in Color3 end, float amount, out Color3 result)
        {
            result.R = MathUtil.Lerp(start.R, end.R, amount);
            result.G = MathUtil.Lerp(start.G, end.G, amount);
            result.B = MathUtil.Lerp(start.B, end.B, amount);
        }

        public static Color3 Lerp(in Color3 start, in Color3 end, float amount)
        {
            Lerp(start, end, amount, out var result);
            return result;
        }

        public static void SmoothStep(in Color3 start, in Color3 end, float amount, out Color3 result)
        {
            amount = MathUtil.SmoothStep(amount);
            Lerp(start, end, amount, out result);
        }

        public static Color3 SmoothStep(in Color3 start, in Color3 end, float amount)
        {
            SmoothStep(start, end, amount, out var result);
            return result;
        }

        public static void Max(in Color3 left, in Color3 right, out Color3 result)
        {
            result.R = (left.R > right.R) ? left.R : right.R;
            result.G = (left.G > right.G) ? left.G : right.G;
            result.B = (left.B > right.B) ? left.B : right.B;
        }

        public static Color3 Max(in Color3 left, in Color3 right)
        {
            Max(left, right, out var result);
            return result;
        }

        public static void Min(in Color3 left, in Color3 right, out Color3 result)
        {
            result.R = (left.R < right.R) ? left.R : right.R;
            result.G = (left.G < right.G) ? left.G : right.G;
            result.B = (left.B < right.B) ? left.B : right.B;
        }

        public static Color3 Min(in Color3 left, in Color3 right)
        {
            Min(left, right, out var result);
            return result;
        }

        public static void AdjustContrast(in Color3 value, float contrast, out Color3 result)
        {
            result.R = 0.5f + contrast * (value.R - 0.5f);
            result.G = 0.5f + contrast * (value.G - 0.5f);
            result.B = 0.5f + contrast * (value.B - 0.5f);
        }

        public static Color3 AdjustContrast(in Color3 value, float contrast)
        {
            return new Color3(
                0.5f + contrast * (value.R - 0.5f),
                0.5f + contrast * (value.G - 0.5f),
                0.5f + contrast * (value.B - 0.5f));
        }

        public static void AdjustSaturation(in Color3 value, float saturation, out Color3 result)
        {
            var grey = value.Brightness;

            result.R = grey + saturation * (value.R - grey);
            result.G = grey + saturation * (value.G - grey);
            result.B = grey + saturation * (value.B - grey);
        }

        public static Color3 AdjustSaturation(in Color3 value, float saturation)
        {
            var grey = value.Brightness;

            return new Color3(
                grey + saturation * (value.R - grey),
                grey + saturation * (value.G - grey),
                grey + saturation * (value.B - grey));
        }

        public static void Premultiply(in Color3 value, float alpha, out Color3 result)
        {
            result.R = value.R * alpha;
            result.G = value.G * alpha;
            result.B = value.B * alpha;
        }

        public static Color3 Premultiply(in Color3 value, float alpha)
        {
            Premultiply(value, alpha, out var result);
            return result;
        }

        public static Color3 operator +(in Color3 left, in Color3 right) => new Color3(left.R + right.R, left.G + right.G, left.B + right.B);
        public static Color3 operator -(in Color3 left, in Color3 right) => new Color3(left.R - right.R, left.G - right.G, left.B - right.B);
        public static Color3 operator *(float scale, in Color3 value) => new Color3(value.R * scale, value.G * scale, value.B * scale);
        public static Color3 operator *(in Color3 value, float scale) => new Color3(value.R * scale, value.G * scale, value.B * scale);
        public static Color3 operator *(in Color3 left, in Color3 right) => new Color3(left.R * right.R, left.G * right.G, left.B * right.B);
        public static Color3 operator /(in Color3 value, float scale) => new Color3(value.R / scale, value.G / scale, value.B / scale);
        public static bool operator ==(in Color3 left, in Color3 right) => left.Equals(right);
        public static bool operator !=(in Color3 left, in Color3 right) => !left.Equals(right);

        public static explicit operator Color3(in Color4 value) => new Color3(value.R, value.G, value.B);
        public static explicit operator Color3(in Vector4 value) => new Color3(value.X, value.Y, value.Z);
        public static implicit operator Color3(in Vector3 value) => new Color3(value.X, value.Y, value.Z);
        public static explicit operator uint(in Color3 value) => value.ToRgba();
        public static explicit operator Color3(uint value) => new Color3(value);
        public static implicit operator Color3(in Half3 value) => new Color3(value.X, value.Y, value.Z);
        public static explicit operator Color3(in Half4 value) => new Color3(value.X, value.Y, value.Z);

        public static explicit operator System.Drawing.Color(in Color3 value)
        {
            return System.Drawing.Color.FromArgb(
                255,
                MathUtil.Clamp((int)(value.R * 255), 0, 255),
                MathUtil.Clamp((int)(value.G * 255), 0, 255),
                MathUtil.Clamp((int)(value.B * 255), 0, 255));
        }

        public static implicit operator Color3(in System.Drawing.Color value) => new Color3(value.R, value.G, value.B);

        public override string ToString()
        {
            if (R <= 1 && G <= 1 && B <= 1) return $"{(int)(R * 255):X2}{(int)(G * 255):X2}{(int)(B * 255):X2}";

            return $"R:{R:F3} G:{G:F3} B:{B:F3}";
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(R.GetHashCode());
            hash.Combine(G.GetHashCode());
            hash.Combine(B.GetHashCode());
            return hash;
        }

        public bool Equals(Color3 other) => R == other.R && G == other.G && B == other.B;
        public override bool Equals(object obj) => obj is Color3 other && Equals(other);
    }
}
