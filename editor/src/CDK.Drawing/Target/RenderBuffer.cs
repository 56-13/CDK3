using System;

namespace CDK.Drawing
{
    public class RenderBuffer : GraphicsResource
    {
        public int Width { private set; get; }
        public int Height { private set; get; }
        public int Object { private set; get; }
        public RawFormat Format { private set; get; }
        public int Samples { private set; get; }
        public bool SystemBuffer { private set; get; }

        public RenderBuffer(int width, int height, RawFormat format, int samples = 1, bool systemBuffer = false)
        {
            Debug.Assert(!format.GetEncoding().Compressed);

            Format = format;
            Width = width;
            Height = height;
            Samples = samples;
            SystemBuffer = systemBuffer;

            if (!systemBuffer) Create();
        }

        public RenderBuffer(in RenderBufferDescription desc)
        {
            desc.Validate();

            Width = desc.Width;
            Height = desc.Height;
            Format = desc.Format;
            Samples = desc.Samples;

            Create();
        }

        ~RenderBuffer()
        {
            if (Object != 0 && !SystemBuffer && GraphicsContext.IsCreated) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteRenderbuffer(Object));
        }

        private void Create()
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Object = api.GenRenderbuffer();
                api.BindRenderbuffer(Object);
                if (!api.RenderbufferStorage(Format, Samples, Width, Height)) Console.WriteLine("renderbuffer storage fail");
            });
            
            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        public override void Dispose()
        {
            if (Object != 0 && !SystemBuffer) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteRenderbuffer(Object));

            GC.SuppressFinalize(this);
        }

        public override int Cost => Width * Height * Format.GetEncoding().PixelBpp;

        public RenderBufferDescription Description
        {
            get
            {
                RenderBufferDescription desc = new RenderBufferDescription
                {
                    Width = Width,
                    Height = Height,
                    Format = Format,
                    Samples = Samples
                };
                return desc;
            }
        }


        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            if (Object != 0 && !SystemBuffer)
            {
                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    api.BindRenderbuffer(Object);
                    if (!api.RenderbufferStorage(Format, Samples, Width, Height)) Console.WriteLine("renderbuffer storage fail");
                });

                command?.AddFence(this, BatchFlag.ReadWrite);
            }
        }
    }
}
