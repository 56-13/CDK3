using System;

namespace CDK.Drawing
{
    public struct TextureDescription : IEquatable<TextureDescription>
    {
        public int Width;
        public int Height;
        public int Depth;
        public TextureTarget Target;
        public RawFormat Format;
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;
        public TextureWrapMode WrapR;
        public TextureMinFilter MinFilter;
        public TextureMagFilter MagFilter;
        public Color4 BorderColor;
        public int Samples;
        public int MipmapCount;

        public void Validate()
        {
            if (Target == TextureTarget.Texture2D && Samples > 1) Target = TextureTarget.Texture2DMultisample;
#if DEBUG
            if (Target == TextureTarget.Texture2DMultisample)   //Texture2DMultisample not support TexParameter
            {
                Debug.Assert(WrapS == 0 || WrapS == TextureWrapMode.Repeat);
                Debug.Assert(WrapR == 0 || WrapR == TextureWrapMode.Repeat);
                Debug.Assert(WrapT == 0 || WrapT == TextureWrapMode.Repeat);
                Debug.Assert(MinFilter == 0 || MinFilter == TextureMinFilter.Nearest);
                Debug.Assert(MagFilter == 0 || MagFilter == TextureMagFilter.Nearest);
                Debug.Assert(MipmapCount == 0 || MipmapCount == 1);
            }
#endif
            if (WrapS == 0) WrapS = TextureWrapMode.Repeat;
            if (WrapT == 0) WrapT = TextureWrapMode.Repeat;
            if (WrapR == 0) WrapR = TextureWrapMode.Repeat;
            if (MinFilter == 0) MinFilter = TextureMinFilter.Nearest;
            if (MagFilter == 0) MagFilter = TextureMagFilter.Nearest;
            if (Samples < 1) Samples = 1;
            if (MipmapCount == 0) MipmapCount = 1;
            else if (MipmapCount > 1) MipmapCount = Math.Min(MipmapCount, MaxMipmapCount);
#if DEBUG
            switch (Target)
            {
                case TextureTarget.Texture2D:
                case TextureTarget.TextureCubeMap:
                    if (Width <= 0 || Height <= 0) throw new InvalidOperationException("invalid size");
                    if (Depth != 0) throw new InvalidOperationException("depth should be zero");
                    if (Samples > 1) throw new InvalidOperationException("multisample should be 1 if target is not Texture2DMultisample");
                    break;
                case TextureTarget.Texture3D:
                    if (Width <= 0 || Height <= 0 || Depth <= 0) throw new InvalidOperationException("invalid size");
                    if (Samples > 1) throw new InvalidOperationException("multisample should be 1 if target is not Texture2DMultisample");
                    break;
                case TextureTarget.Texture2DMultisample:
                    if (Width <= 0 || Height <= 0) throw new InvalidOperationException("invalid size");
                    if (Depth != 0) throw new InvalidOperationException("depth should be zero");
                    if (Samples <= 1) throw new InvalidOperationException("samples should be greater than 1");
                    break;
                default:
                    throw new InvalidOperationException("invalid format");
            }
#endif
        }

        public int MaxMipmapCount
        {
            get
            {
                var d = Math.Max(Width, Height);
                if (Target == TextureTarget.Texture3D) d = Math.Max(d, Depth);
                var c = 1;
                while (d > 0)
                {
                    d >>= 1;
                    c++;
                }
                return c;
            }
        }

        public static bool operator ==(in TextureDescription left, in TextureDescription right) => left.Equals(right);
        public static bool operator !=(in TextureDescription left, in TextureDescription right) => !left.Equals(right);
        public override string ToString() => $"Size:{Width}, {Height}, {Depth} Target:{Target} Format:{Format} Wrap:{WrapS}, {WrapT}, {WrapR} Filter:{MinFilter}, {MagFilter} BorderColor:{BorderColor} Mipmap:{MipmapCount} Multisample:{Samples}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            hash.Combine(Depth.GetHashCode());
            hash.Combine(Target.GetHashCode());
            hash.Combine(Format.GetHashCode());
            hash.Combine(WrapS.GetHashCode());
            hash.Combine(WrapT.GetHashCode());
            hash.Combine(WrapR.GetHashCode());
            hash.Combine(MinFilter.GetHashCode());
            hash.Combine(MagFilter.GetHashCode());
            hash.Combine(BorderColor.GetHashCode());
            hash.Combine(Samples.GetHashCode());
            hash.Combine(MipmapCount.GetHashCode());
            return hash;
        }

        public bool Equals(TextureDescription other)
        {
            return Width == other.Width &&
                Height == other.Height &&
                Depth == other.Depth &&
                Target == other.Target &&
                Format == other.Format &&
                WrapS == other.WrapS &&
                WrapT == other.WrapT &&
                WrapR == other.WrapR &&
                MinFilter == other.MinFilter &&
                MagFilter == other.MagFilter &&
                BorderColor == other.BorderColor &&
                Samples == other.Samples &&
                MipmapCount == other.MipmapCount;
        }

        public override bool Equals(object obj) => obj is TextureDescription other && Equals(other);
    }
}
