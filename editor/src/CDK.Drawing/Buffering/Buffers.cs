using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public static class Buffers
    {
        private static List<BufferSliceSet> _sliceSets;

        internal static void CreateShared()
        {
            _sliceSets = new List<BufferSliceSet>();
        }

        internal static void DisposeShared()
        {
            foreach (var sliceSet in _sliceSets) sliceSet.Dispose();
            _sliceSets = null;
        }

        public static Buffer New(object key, int life, bool recycle, BufferTarget target)
        {
            if (ResourcePool.Instance.Recycle(((IResource candidate) => candidate is Buffer buf && buf.Target == target), key, life, out var resource))
            {
                return (Buffer)resource;
            }
            var newBuf = new Buffer(target);
            ResourcePool.Instance.Add(key, newBuf, life, recycle);
            return newBuf;
        }

        public static Buffer New(object key, int life, bool recycle, BufferTarget target, int preferredSize, int preferredCount)
        {
            if (ResourcePool.Instance.Recycle(((IResource candidate) => candidate is Buffer buf && buf.Target == target && buf.Size == preferredSize && buf.Count == preferredCount), key, life, out var resource))
            {
                return (Buffer)resource;
            }
            return New(key, life, recycle, target);
        }

        public static Buffer NewTemporary(BufferTarget target) => New(null, 1, true, target);

        public static Buffer FromData<T>(in T data, BufferTarget target) where T : unmanaged
        {
            var key = (target, data);
            var buffer = (Buffer)ResourcePool.Instance.Get(key);
            if (buffer == null)
            {
                buffer = New(key, 1, true, target, Marshal.SizeOf<T>(), 1);
                buffer.Upload(data, BufferUsageHint.DynamicDraw);
            }
            return buffer;
        }

        public static Buffer FromData<T>(BufferData<T> data, BufferTarget target) where T : unmanaged
        {
            var key = (target, data);
            var buffer = (Buffer)ResourcePool.Instance.Get(key);
            if (buffer == null)
            {
                buffer = New(key, 1, true, target, BufferData<T>.Size, data.Count);
                buffer.Upload(data, BufferUsageHint.DynamicDraw);
            }
            return buffer;
        }

        public static BufferSlice NewSlice(BufferTarget target, int size, int count, int capacity, BufferUsageHint hint)
        {
            lock (_sliceSets)
            {
                foreach (var sliceSet in _sliceSets)
                {
                    if (sliceSet.Buffer.Target == target && sliceSet.Buffer.Size == size)
                    {
                        var slice = sliceSet.Obtain(count);

                        if (slice != null) return slice;
                    }
                }

                Debug.Assert(count <= capacity);

#if (DEBUG && RESOURCE_POOL_LOG)
                Console.WriteLine($"buffer slice add:{size}:{capacity}:{_sliceSets.Count + 1}");
#endif
                {
                    var sliceSet = new BufferSliceSet(target, size, capacity, hint);
                    _sliceSets.Add(sliceSet);
                    var slice = sliceSet.Obtain(count);
                    return slice;
                }
            }
        }

        internal static void ReleaseSlice(BufferSliceSet sliceSet, BufferSlice slice)
        {
            lock (_sliceSets)
            {
                if (sliceSet.Release(slice))
                {
                    _sliceSets.Remove(sliceSet);
                    sliceSet.Dispose();
                }
            }
        }
    }
}
