using System;

namespace CDK.Drawing
{
    public struct Shadow : IEquatable<Shadow>
    {
        public bool Pixel32;
        public int Resolution;
        public float Blur;
        public float Bias;
        public float Bleeding;

        public const int DefaultResolution = 1024;
        public const float DefaultBlur = 4.0f;
        public const float DefaultBias = 0.001f;
        public const float DefaultBleeding = 0.9f;

        public static Shadow Default = new Shadow(true, DefaultResolution, DefaultBlur, DefaultBias, DefaultBleeding);

        public Shadow(bool pixel32, int resolution, float blur, float bias, float bleeding)
        {
            Pixel32 = pixel32;
            Resolution = resolution;
            Blur = blur;
            Bias = bias;
            Bleeding = bleeding;
        }

        public static bool operator ==(in Shadow left, in Shadow right) => left.Equals(right);
        public static bool operator !=(in Shadow left, in Shadow right) => !left.Equals(right);

        public override string ToString() => $"Pixel32:{Pixel32} Resolution:{Resolution} Blur:{Blur} Bias:{Bias} Bleeding:{Bleeding}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Pixel32.GetHashCode());
            hash.Combine(Resolution.GetHashCode());
            hash.Combine(Blur.GetHashCode());
            hash.Combine(Bias.GetHashCode());
            hash.Combine(Bleeding.GetHashCode());
            return hash;
        }

        public bool Equals(Shadow other)
        {
            return Pixel32 == other.Pixel32 &&
                Resolution == other.Resolution &&
                Blur == other.Blur &&
                Bias == other.Bias &&
                Bleeding == other.Bleeding;
        }

        public override bool Equals(object obj) => obj is Shadow other && Equals(other);
    }
}
