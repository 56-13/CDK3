using System;

namespace CDK.Drawing
{
    internal class BufferSliceSet : IDisposable
    {
        public Buffer Buffer { private set; get; }

        private BufferSlice _first;

        public BufferSliceSet(BufferTarget target, int size, int capacity, BufferUsageHint hint)
        {
            Buffer = new Buffer(target);
            Buffer.Allocate(size, capacity, hint);
        }

        public BufferSlice Obtain(int count)
        {
            if (_first == null)
            {
                _first = new BufferSlice(this, 0, count);
                return _first;
            }

            var offset = 0;
            var current = _first;
            for (; ; )
            {
                var remaining = current.Offset - offset;
                if (remaining >= count)
                {
                    var slice = new BufferSlice(this, offset, count)
                    {
                        Next = current
                    };
                    if (current.Prev != null)
                    {
                        slice.Prev = current.Prev;
                        current.Prev.Next = slice;
                    }
                    else _first = slice;
                    current.Prev = slice;

                    return slice;
                }
                offset = current.Offset + current.Count;
                if (current.Next == null) break;
                current = current.Next;
            }
            if (offset + count <= Buffer.Count)
            {
                var slice = new BufferSlice(this, offset, count)
                {
                    Prev = current
                };
                current.Next = slice;
                return slice;
            }
            return null;
        }

        public bool Release(BufferSlice slice)
        {
            if (slice.Prev != null) slice.Prev.Next = slice.Next;
            else _first = slice.Next;
            if (slice.Next != null) slice.Next.Prev = slice.Prev;
            return _first == null;
        }

        public void Dispose() => Buffer.Dispose();
    }

    public class BufferSlice : IResource
    {
        internal BufferSliceSet Parent { private set; get; }
        public Buffer Buffer => Parent.Buffer;
        public int Offset { private set; get; }
        public int Count { private set; get; }
        public int Cost => Count * Buffer.Size;
        internal BufferSlice Prev { set; get; }
        internal BufferSlice Next { set; get; }

        internal BufferSlice(BufferSliceSet parent, int offset, int count)
        {
            Parent = parent;
            Offset = offset;
            Count = count;
        }

        public void Dispose() => Buffers.ReleaseSlice(Parent, this);
    }
}
