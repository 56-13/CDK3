using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public class Buffer : GraphicsResource
    {
        public BufferTarget Target { private set; get; }
        public int Object { private set; get; }
        public int Size { private set; get; }
        public int Count { private set; get; }
        public bool Allocated { private set; get; }

        public Buffer(BufferTarget target)
        {
            Target = target;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => Object = api.GenBuffer());
            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        ~Buffer()
        {
            if (GraphicsContext.IsCreated) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteBuffer(Object));
        }

        public override void Dispose()
        {
            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteBuffer(Object));
            
            GC.SuppressFinalize(this);
        }

        public override int Cost => Size * Count;

        public void Deallocate()
        {
            if (Allocated)
            {
                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    Debug.Assert(api.GetVertexArrayBinding() == 0);

                    api.BindBuffer(Target, Object);
                    api.BufferData(Target, 0, IntPtr.Zero, BufferUsageHint.StreamDraw);      //TODO:check zero size safe?
                    api.BindBuffer(Target, 0);
                });
                command?.AddFence(this, BatchFlag.ReadWrite);
                Size = 0;
                Count = 0;
                Allocated = false;
            }
        }

        public void Allocate(int size, int count, BufferUsageHint usage)
        {
            Size = size;
            Count = count;
            Allocated = true;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Debug.Assert(api.GetVertexArrayBinding() == 0);

                api.BindBuffer(Target, Object);
                api.BufferData(Target, size * count, IntPtr.Zero, usage);
                api.BindBuffer(Target, 0);
            });
            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        private void UploadFail()
        {
            Console.WriteLine($"upload fail:target:{Target} size:{Size} count:{Count}");
            Allocated = false;
        }

        public void Upload(GraphicsApi api, IntPtr p, int size, int count, BufferUsageHint usage)
        {
            Debug.Assert(api.GetVertexArrayBinding() == 0);

            Size = size;
            Count = count;
            Allocated = true;

            api.BindBuffer(Target, Object);
            if (!api.BufferData(Target, size * count, p, usage)) UploadFail();
            api.BindBuffer(Target, 0);
        }

        public void UploadSub(GraphicsApi api, IntPtr p, int size, int offset, int count)
        {
            Debug.Assert(Size == size && offset + count <= Count && api.GetVertexArrayBinding() == 0);

            api.BindBuffer(Target, Object);
            api.BufferSubData(Target, size * offset, size * count, p);
            api.BindBuffer(Target, 0);
        }

        public void Upload<T>(T value, BufferUsageHint usage) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            Size = size;
            Count = 1;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                T value_ = value;

                unsafe
                {
                    Upload(api, (IntPtr)(&value_), size, 1, usage);
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void Upload<T>(T[] values, BufferUsageHint usage) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            Size = size;
            Count = values.Length;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                unsafe
                {
                    fixed (void* p = values)
                    {
                        Upload(api, (IntPtr)p, size, values.Length, usage);
                    }
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void Upload<T>(T[] values, int srcOffset, int srcLength, BufferUsageHint usage) where T : unmanaged
        {
            Debug.Assert(srcOffset >= 0 && srcOffset + srcLength <= values.Length);

            var size = Marshal.SizeOf<T>();
            Size = size;
            Count = srcLength;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                unsafe
                {
                    fixed (void* p = &values[srcOffset])
                    {
                        Upload(api, (IntPtr)p, size, srcLength, usage);
                    }
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void Upload<T>(BufferData<T> data, BufferUsageHint usage) where T : unmanaged
        {
            Debug.Assert(Target != BufferTarget.ElementArrayBuffer);
            Upload(data.ToArray(), usage);
        }

        public void Upload(VertexIndexData data, BufferUsageHint usage)
        {
            Debug.Assert(Target == BufferTarget.ElementArrayBuffer);
            if (data.VertexCapacity <= 256) Upload(data.Select(i => (byte)i).ToArray(), usage);
            else if (data.VertexCapacity <= 65536) Upload(data.Select(i => (ushort)i).ToArray(), usage);
            else Upload(data.ToArray(), usage);
        }


        public void UploadSub<T>(T value, int offset) where T : unmanaged
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                T value_ = value;

                unsafe
                {
                    UploadSub(api, (IntPtr)(&value_), Marshal.SizeOf<T>(), offset, 1);
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void UploadSub<T>(T[] values, int offset) where T : unmanaged
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                unsafe
                {
                    fixed (void* p = values)
                    {
                        UploadSub(api, (IntPtr)p, Marshal.SizeOf<T>(), offset, values.Length);
                    }
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void UploadSub<T>(T[] values, int offset, int srcOffset, int srcLength) where T : unmanaged
        {
            Debug.Assert(srcOffset >= 0 && srcOffset + srcLength <= values.Length);

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                unsafe
                {
                    fixed (void* p = &values[srcOffset])
                    {
                        UploadSub(api, (IntPtr)p, Marshal.SizeOf<T>(), offset, srcLength);
                    }
                }
            });
            command?.AddFence(this, BatchFlag.Write);
        }

        public void UploadSub<T>(BufferData<T> data, int offset) where T : unmanaged
        {
            Debug.Assert(Target != BufferTarget.ElementArrayBuffer);
            UploadSub(data.ToArray(), offset);
        }

        public void UploadSub(VertexIndexData data, int offset)
        {
            Debug.Assert(Target == BufferTarget.ElementArrayBuffer);
            if (data.VertexCapacity <= 256) UploadSub(data.Select(i => (byte)i).ToArray(), offset);
            else if (data.VertexCapacity <= 65536) UploadSub(data.Select(i => (ushort)i).ToArray(), offset);
            else UploadSub(data.ToArray(), offset);
        }
    }
}
