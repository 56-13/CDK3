using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public class BufferData<T> : List<T>, IEquatable<BufferData<T>> where T : unmanaged
    {
        public static readonly int Size = Marshal.SizeOf<T>();

        public BufferData() { }
        public BufferData(int capacity) : base(capacity) { }
        public static bool operator ==(BufferData<T> a, BufferData<T> b) => a is null ? b is null : (!(b is null) && a.Equals(b));
        public static bool operator !=(BufferData<T> a, BufferData<T> b) => !(a == b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            foreach (var v in this) hash.Combine(v.GetHashCode());
            return hash;
        }

        public bool Equals(BufferData<T> other) => this.SequenceEqual(other);
        public override bool Equals(object obj) => obj is BufferData<T> other && Equals(other);
    }

    public class VertexIndexData : BufferData<int>
    {
        public int VertexCapacity { set; get; }

        public VertexIndexData(int vertexCapacity)
        {
            VertexCapacity = vertexCapacity;
        }

        public VertexIndexData(int vertexCapacity, int indexCapacity) : base(indexCapacity)
        {
            VertexCapacity = vertexCapacity;
        }
    }
}
