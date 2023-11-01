using System;

namespace CDK.Drawing
{
    public struct RenderBufferDescription : IEquatable<RenderBufferDescription>
    {
        public int Width;
        public int Height;
        public RawFormat Format;
        public int Samples;

        public void Validate()
        {
            Debug.Assert(Width > 0 && Height > 0 && Format != RawFormat.None && !Format.GetEncoding().Compressed && Samples >= 0);

            if (Samples == 0) Samples = 1;
        }

        public static bool operator ==(in RenderBufferDescription left, in RenderBufferDescription right) => left.Equals(right);
        public static bool operator !=(in RenderBufferDescription left, in RenderBufferDescription right) => !left.Equals(right);
        public override string ToString() => $"Width:{Width} Height:{Height} Format:{Format} Sample:{Samples}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            hash.Combine(Format.GetHashCode());
            hash.Combine(Samples.GetHashCode());
            return hash;
        }

        public bool Equals(RenderBufferDescription other)
        {
            return Width == other.Width &&
                Height == other.Height &&
                Format == other.Format &&
                Samples == other.Samples;
        }

        public override bool Equals(object obj) => obj is RenderBufferDescription other && Equals(other);
    }
}
