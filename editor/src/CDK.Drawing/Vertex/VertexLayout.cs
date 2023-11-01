using System;

namespace CDK.Drawing
{
    public struct VertexLayout : IEquatable<VertexLayout>
    {
        public Buffer Buffer;
        public int BufferIndex;
        public int Attrib;
        public int Size;
        public VertexAttribType Type;
        public bool Normalized;
        public int Stride;
        public int Offset;
        public int Divisor;
        public bool EnabledByDefault;

        private VertexLayout(Buffer buffer, int bufferIndex, int attrib, int size, VertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault)
        {
            Buffer = buffer;
            BufferIndex = bufferIndex;
            Attrib = attrib;
            Size = size;
            Type = type;
            Normalized = normalized;
            Stride = stride;
            Offset = offset;
            Divisor = divisor;
            EnabledByDefault = enabledByDefault;
        }

        public VertexLayout(Buffer buffer, int attrib, int size, VertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault) :
            this(buffer, -1, attrib, size, type, normalized, stride, offset, divisor, enabledByDefault)
        {
            Debug.Assert(buffer.Target == BufferTarget.ArrayBuffer);
        }

        public VertexLayout(int bufferIndex, int attrib, int size, VertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault) :
            this(null, bufferIndex, attrib, size, type, normalized, stride, offset, divisor, enabledByDefault)
        {
        }

        public static bool operator ==(in VertexLayout a, in VertexLayout b) => a.Equals(b);
        public static bool operator !=(in VertexLayout a, in VertexLayout b) => !a.Equals(b);

        public bool Equals(VertexLayout other)
        {
            return Buffer == other.Buffer &&
                BufferIndex == other.BufferIndex &&
                Attrib == other.Attrib &&
                Size == other.Size &&
                Type == other.Type &&
                Normalized == other.Normalized &&
                Stride == other.Stride &&
                Offset == other.Offset &&
                Divisor == other.Divisor &&
                EnabledByDefault == other.EnabledByDefault;
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Buffer?.GetHashCode() ?? BufferIndex);
            hash.Combine(Attrib);
            hash.Combine(Size);
            hash.Combine(Type.GetHashCode());
            hash.Combine(Normalized.GetHashCode());
            hash.Combine(Stride);
            hash.Combine(Offset);
            hash.Combine(Divisor);
            hash.Combine(EnabledByDefault.GetHashCode());
            return hash;
        }

        public override bool Equals(object obj) => obj is VertexLayout other && Equals(other);
    }
}
