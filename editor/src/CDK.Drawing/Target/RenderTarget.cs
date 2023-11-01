using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using Bitmap = System.Drawing.Bitmap;

namespace CDK.Drawing
{
    public struct SystemRenderTargetDescription
    {
        public int RedBit;
        public int GreenBit;
        public int BlueBit;
        public int AlphaBit;
        public int DepthBit;
        public int StencilBit;
        public int Samples;
    };

    public class RenderTarget : GraphicsResource
    {
        public int Framebuffer { private set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }
        public bool SystemBuffer { private set; get; }
        public bool BloomSupported { private set; get; }
        internal class Buffer
        {
            public FramebufferAttachment Attachment;
            public RenderBuffer Renderbuffer;
            public Texture Texture;
            public int TextureLayer;
            public bool Own;

            public GraphicsResource Resource => Renderbuffer != null ? (GraphicsResource)Renderbuffer : Texture;
        }
        private List<Buffer> _buffers;
        private int[] _drawBuffers;
        private Bounds2[] _viewport;

        public RenderTarget(int width, int height)
        {
            Width = width;
            Height = height;
            
            _buffers = new List<Buffer>();
            _drawBuffers = new int[] { 0 };

            _viewport = new Bounds2[2];
            _viewport[0] = _viewport[1] = new Bounds2(0, 0, width, height);

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => Framebuffer = api.GenFramebuffer());
            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        public RenderTarget(int width, int height, int framebuffer, in SystemRenderTargetDescription desc)
        {
            Framebuffer = framebuffer;
            Width = width;
            Height = height;
            SystemBuffer = true;

            _buffers = new List<Buffer>(2);
            _drawBuffers = new int[] { 0 };

            _viewport = new Bounds2[2];
            _viewport[0] = _viewport[1] = new Bounds2(0, 0, width, height);

            RawFormat colorFormat;
            if (desc.RedBit == 8 && desc.GreenBit == 8 && desc.BlueBit == 8)
            {
                if (desc.AlphaBit == 8) colorFormat = RawFormat.Rgba8;
                else if (desc.AlphaBit == 0) colorFormat = RawFormat.Rgb8;
                else throw new InvalidOperationException("unknown color format");
            }
            else throw new InvalidOperationException("unknown color format");

            Buffer colorBuffer = new Buffer
            {
                Attachment = FramebufferAttachment.ColorAttachment0,
                Renderbuffer = new RenderBuffer(width, height, colorFormat, desc.Samples, true)
            };
            _buffers.Add(colorBuffer);

            switch (desc.DepthBit)
            {
                case 32:
                    if (desc.StencilBit == 8)
                    {
                        Buffer depthStencilBuffer = new Buffer
                        {
                            Attachment = FramebufferAttachment.DepthStencilAttachment,
                            Renderbuffer = new RenderBuffer(width, height, RawFormat.Depth32fStencil8, desc.Samples, true)
                        };
                        _buffers.Add(depthStencilBuffer);
                    }
                    else if (desc.StencilBit == 0)
                    {
                        Buffer depthBuffer = new Buffer
                        {
                            Attachment = FramebufferAttachment.DepthAttachment,
                            Renderbuffer = new RenderBuffer(width, height, RawFormat.DepthComponent32f, desc.Samples, true)
                        };
                        _buffers.Add(depthBuffer);
                    }
                    else throw new InvalidOperationException("unknown depth stencil format");
                    break;
                case 24:
                    if (desc.StencilBit == 8)
                    {
                        Buffer depthStencilBuffer = new Buffer
                        {
                            Attachment = FramebufferAttachment.DepthStencilAttachment,
                            Renderbuffer = new RenderBuffer(width, height, RawFormat.Depth24Stencil8, desc.Samples, true)
                        };
                        _buffers.Add(depthStencilBuffer);
                    }
                    else if (desc.StencilBit == 0)
                    {
                        Buffer depthBuffer = new Buffer
                        {
                            Attachment = FramebufferAttachment.DepthAttachment,
                            Renderbuffer = new RenderBuffer(width, height, RawFormat.DepthComponent24, desc.Samples, true)
                        };
                        _buffers.Add(depthBuffer);
                    }
                    else throw new InvalidOperationException("unknown depth stencil format");
                    break;
                case 16:
                    if (desc.StencilBit == 0)
                    {
                        Buffer depthBuffer = new Buffer
                        {
                            Attachment = FramebufferAttachment.DepthAttachment,
                            Renderbuffer = new RenderBuffer(width, height, RawFormat.DepthComponent16, desc.Samples, true)
                        };
                        _buffers.Add(depthBuffer);
                    }
                    else throw new InvalidOperationException("unknown depth stencil format");
                    break;
                case 0:
                    break;
                default:
                    throw new InvalidOperationException("unknown depth stencil format");
            }
        }

        public RenderTarget(RenderTargetDescription desc)
        {
            desc.Validate();

            Width = desc.Width;
            Height = desc.Height;

            _buffers = new List<Buffer>();
            _drawBuffers = new int[] { 0 };

            _viewport = new Bounds2[2];
            _viewport[0] = _viewport[1] = new Bounds2(0, 0, desc.Width, desc.Height);

            if (desc.Attachments != null && desc.Attachments.Length > 0)
            {
                foreach (var a in desc.Attachments)
                {
                    if (a.Texture)
                    {
                        var textureDesc = new TextureDescription()
                        {
                            Width = desc.Width,
                            Height = desc.Height,
                            Target = a.TextureTarget,
                            Format = a.Format,
                            WrapS = a.TextureWrapS,
                            WrapT = a.TextureWrapT,
                            WrapR = a.TextureWrapR,
                            MinFilter = a.TextureMinFilter,
                            MagFilter = a.TextureMagFilter,
                            BorderColor = a.TextureBorderColor,
                            Samples = a.Samples
                        };
                        var texture = new Texture(textureDesc);

                        _buffers.Add(new Buffer()
                        {
                            Attachment = a.Attachment,
                            Texture = texture,
                            TextureLayer = a.TextureLayer,
                            Own = true
                        });
                    }
                    else
                    {
                        var renderbuffer = new RenderBuffer(Width, Height, a.Format, a.Samples);

                        _buffers.Add(new Buffer()
                        {
                            Attachment = a.Attachment,
                            Renderbuffer = renderbuffer,
                            Own = true
                        });
                    }
                    if (a.Attachment == FramebufferAttachment.ColorAttachment1) BloomSupported = true;
                }

                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => {
                    Framebuffer = api.GenFramebuffer();
                    api.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
                    foreach (var buffer in _buffers) {
                        if (buffer.Renderbuffer != null) api.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, buffer.Attachment, buffer.Renderbuffer.Object);
                        else api.FramebufferTexture(FramebufferTarget.Framebuffer, buffer.Attachment, buffer.Texture.Object, buffer.TextureLayer);
                    }
                    api.CurrentTarget = null;
                });
                if (command != null)
                {
                    command.AddFence(this, BatchFlag.ReadWrite);
                    foreach (var buffer in _buffers) command.AddFence(buffer.Resource, BatchFlag.Read);
                }
            }
            else
            {
                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => Framebuffer = api.GenFramebuffer());
                command?.AddFence(this, BatchFlag.ReadWrite);
            }
        }

        ~RenderTarget()
        {
            if (!SystemBuffer && GraphicsContext.IsCreated)
            {
                GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => {
                    api.DeleteFramebuffer(Framebuffer);
                });

                GraphicsContext.Instance.ClearTargets(this);
            }
        }

        public override void Dispose()
        {
            if (!SystemBuffer)
            {
                GraphicsContext.Instance.Invoke(false, (GraphicsApi api) =>
                {
                    api.DeleteFramebuffer(Framebuffer);

                    foreach (var buffer in _buffers)
                    {
                        if (buffer.Own)
                        {
                            buffer.Renderbuffer?.Dispose();
                            buffer.Texture?.Dispose();
                        }
                    }
                });
                GraphicsContext.Instance.ClearTargets(this);
            }

            GC.SuppressFinalize(this);
        }

        public override int Cost
        {
            get
            {
                var cost = 0;
                foreach (var buffer in _buffers)
                {
                    if (buffer.Own)
                    {
                        if (buffer.Renderbuffer != null) cost += buffer.Renderbuffer.Cost;
                        else cost += buffer.Texture.Cost;
                    }
                }
                return cost;
            }
        }

        public int Samples
        {
            get
            {
                foreach (var buffer in _buffers)
                {
                    return buffer.Renderbuffer != null ? buffer.Renderbuffer.Samples : buffer.Texture.Samples;
                }
                return 0;
            }
        }

        private void AttachImpl(FramebufferAttachment attachment, RenderBuffer renderbuffer, bool own)
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Focus(api);
                api.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, renderbuffer.Object);
            });
            if (command != null)
            {
                command.AddFence(renderbuffer, BatchFlag.Read);
                command.AddFence(this, BatchFlag.ReadWrite);
            }

            _buffers.Add(new Buffer
            {
                Attachment = attachment,
                Renderbuffer = renderbuffer,
                Own = own
            });

            _Description = null;
        }

        public void Attach(FramebufferAttachment attachment, RenderBuffer renderbuffer, bool own)
        {
            if (SystemBuffer) throw new InvalidOperationException("can not attach to system buffer");
            if (renderbuffer.Width != Width || renderbuffer.Height != Height) throw new InvalidOperationException("can not attach different size buffer");
            if (_buffers.Any(b => b.Attachment == attachment)) throw new InvalidOperationException("already attached");

            AttachImpl(attachment, renderbuffer, own);
        }

        private void AttachImpl(FramebufferAttachment attachment, Texture texture, int layer, bool own)
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Focus(api);
                api.FramebufferTexture(FramebufferTarget.Framebuffer, attachment, texture.Object, layer);
            });
            if (command != null)
            {
                command.AddFence(texture, BatchFlag.Read);
                command.AddFence(this, BatchFlag.ReadWrite);
            }

            _buffers.Add(new Buffer()
            {
                Attachment = attachment,
                Texture = texture,
                TextureLayer = layer,
                Own = own
            });

            _Description = null;
        }

        public void Attach(FramebufferAttachment attachment, Texture texture, int layer, bool own)
        {
            if (SystemBuffer) throw new InvalidOperationException("can not attach to system buffer");
            if (texture.Description.Width != Width || texture.Description.Height != Height) throw new InvalidOperationException("can not attach different size buffer");
            if (_buffers.Any(b => b.Attachment == attachment)) throw new InvalidOperationException("already attached");

            AttachImpl(attachment, texture, layer, own);
        }

        private void DetachImpl(FramebufferAttachment attachment)
        {
            for (var i = 0; i < _buffers.Count; i++)
            {
                var buffer = _buffers[i];

                if (buffer.Attachment == attachment)
                {
                    DelegateCommand command;

                    if (buffer.Renderbuffer != null)
                    {
                        command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            Focus(api);
                            api.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, buffer.Attachment, 0);
                        });
                        if (buffer.Own) buffer.Renderbuffer.Dispose();
                    }
                    else
                    {
                        command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            Focus(api);
                            api.FramebufferTexture(FramebufferTarget.Framebuffer, buffer.Attachment, 0, 0);
                        });
                        if (buffer.Own) buffer.Texture.Dispose();
                    }
                    command?.AddFence(this, BatchFlag.ReadWrite);

                    _buffers.RemoveAt(i);
                    break;
                }
            }
            _Description = null;
        }

        public void Detach(FramebufferAttachment attachment)
        {
            if (SystemBuffer) throw new InvalidOperationException("can not detach from system buffer");

            DetachImpl(attachment);
        }

        public Texture GetTextureAttachment(FramebufferAttachment attachment) => _buffers.FirstOrDefault(b => b.Attachment == attachment)?.Texture;
        public RenderBuffer GetRenderBufferAttachment(FramebufferAttachment attachment) => _buffers.FirstOrDefault(b => b.Attachment == attachment)?.Renderbuffer;
        public RawFormat GetFormat(FramebufferAttachment attachment)
        {
            foreach (var buffer in _buffers)
            {
                if (buffer.Attachment == attachment)
                {
                    return buffer.Renderbuffer != null ? buffer.Renderbuffer.Format : buffer.Texture.Description.Format;
                }
            }
            return 0;
        }

        private RenderTargetDescription? _Description;
        public RenderTargetDescription Description
        {
            get
            {
                if (_Description == null)
                {
                    var desc = new RenderTargetDescription
                    {
                        Width = Width,
                        Height = Height
                    };
                    desc.Attachments = new RenderTargetAttachmentDescription[_buffers.Count];

                    for (var i = 0; i < _buffers.Count; i++)
                    {
                        var buffer = _buffers[i];

                        desc.Attachments[i].Attachment = buffer.Attachment;
                        if (buffer.Renderbuffer != null)
                        {
                            desc.Attachments[i].Format = buffer.Renderbuffer.Format;
                            desc.Attachments[i].Samples = buffer.Renderbuffer.Samples;
                        }
                        else
                        {
                            var textureDesc = buffer.Texture.Description;

                            desc.Attachments[i].Texture = true;
                            desc.Attachments[i].Format = textureDesc.Format;
                            desc.Attachments[i].Samples = textureDesc.Samples;
                            desc.Attachments[i].TextureTarget = textureDesc.Target;
                            desc.Attachments[i].TextureWrapS = textureDesc.WrapS;
                            desc.Attachments[i].TextureWrapT = textureDesc.WrapT;
                            desc.Attachments[i].TextureWrapR = textureDesc.WrapR;
                            desc.Attachments[i].TextureMinFilter = textureDesc.MinFilter;
                            desc.Attachments[i].TextureMagFilter = textureDesc.MagFilter;
                            desc.Attachments[i].TextureBorderColor = textureDesc.BorderColor;
                            desc.Attachments[i].TextureLayer = buffer.TextureLayer;
                        }
                    }
                    _Description = desc;
                }
                return _Description.Value;
            }
        }

        protected internal override bool Batch(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes, BatchFlag flags) 
        {
            var result = base.Batch(reads, writes, flags);
            if ((flags & BatchFlag.Retrieve) != 0) {
                foreach (Buffer buffer in _buffers) result &= buffer.Resource.Batch(reads, writes, flags);
            }
            return result;
        }

        public void ClearColor(int buf, Color4 color)
        {
            var attachmment = FramebufferAttachment.ColorAttachment0 + buf;
            Debug.Assert(GetFormat(attachmment) != RawFormat.None);
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => {
                Focus(api);
                api.ClearBufferColor(buf, color);
            });

            if (command != null) command.AddFence(_buffers.First(b => b.Attachment == attachmment).Resource, BatchFlag.ReadWrite);
        }

        public void ClearDepthStencil()
        {
            Debug.Assert(GetFormat(FramebufferAttachment.DepthStencilAttachment) != RawFormat.None);
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => {
                Focus(api);
                api.ClearBufferDepthStencil();
            });

            if (command != null) command.AddFence(_buffers.First(b => b.Attachment == FramebufferAttachment.DepthStencilAttachment).Resource, BatchFlag.ReadWrite);
        }

        public void ClearDepth()
        {
            Debug.Assert(GetFormat(FramebufferAttachment.DepthAttachment) != RawFormat.None);
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => {
                Focus(api);
                api.ClearBufferDepth();
            });

            if (command != null) command.AddFence(_buffers.First(b => b.Attachment == FramebufferAttachment.DepthAttachment).Resource, BatchFlag.ReadWrite);
        }

        public void SetDrawBuffers(GraphicsApi api, params int[] bufs)
        {
            Debug.Assert(api.CurrentTarget == this);
            if (!bufs.SequenceEqual(_drawBuffers))
            {
                _drawBuffers = bufs;
                api.DrawBuffers(bufs);
            }
        }


        public Bounds2 Viewport {
            set
            {
                if (value.X > Width) value.X = Width;
                if (value.Y > Height) value.Y = Height;
                if (value.X + value.Width > Width) value.Width = Width - value.X;
                if (value.Y + value.Height > Height) value.Height = Height - value.Y;

                if (value != _viewport[0])
                {
                    _viewport[0] = value;

                    var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => {
                        if (api.CurrentTarget == this) api.Viewport(value);
                        _viewport[1] = value;
                    });
                    command?.AddFence(this, BatchFlag.ReadWrite);
                }
            }
            get
            {
                return _viewport[GraphicsContext.Instance.IsRenderThread(out _) ? 1 : 0];
            }
        }

        public void ClearViewport()
        {
            Viewport = new Bounds2(0, 0, Width, Height);
        }

        public bool HasViwport
        {
            get
            {
                var viewport = Viewport;

                return viewport.X != 0 || viewport.Y != 0 || viewport.Width != Width || viewport.Height != Height;
            }
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            foreach (var buffer in _buffers)
            {
                if (buffer.Own)
                {
                    if (buffer.Renderbuffer != null) buffer.Renderbuffer.Resize(width, height);
                    else buffer.Texture.Resize(width, height);
                }
            }

            _Description = null;
        }

        public void Focus(GraphicsApi api)
        {
            if (api.CurrentTarget != this)
            {
                api.CurrentTarget = this;
                api.Viewport(_viewport[1]);
                api.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
            }
        }
        
        public Texture CaptureColor(int buf, bool temporary) => CaptureColor(buf, Viewport, temporary);
        
        public Texture CaptureColor(int buf, Bounds2 bounds, bool temporary)
        {
            bounds.Intersect(Viewport);

            if (bounds.Width <= 0 || bounds.Height <= 0) return null;

            var attachment = FramebufferAttachment.ColorAttachment0 + buf;

            var format = GetFormat(attachment);

            if (format == 0) return null;

            var textureDesc = new TextureDescription()
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Format = format
            };
            
            var texture = temporary ? Textures.NewTemporary(textureDesc, false) : new Texture(textureDesc, false);

            DelegateCommand command;

            if (Samples > 1)
            {
                command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    texture.Allocate();

                    var sampleDesc = new RenderTargetDescription()
                    {
                        Width = bounds.Width,
                        Height = bounds.Height
                    };

                    var sampleTarget = RenderTargets.NewTemporary(sampleDesc);
                    sampleTarget.Attach(attachment, texture, 0, false);
                    Blit(sampleTarget, bounds, new Bounds2(0, 0, bounds.Width, bounds.Height), ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest, buf, buf);
                    sampleTarget.Detach(attachment);
                    ResourcePool.Instance.Remove(sampleTarget);
                });
            }
            else
            {
                command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    api.ReadBuffer(buf);
                    api.BindTexture(TextureTarget.Texture2D, texture.Object);
                    api.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
                    if (!api.CopyTexImage2D(TextureTarget.Texture2D, 0, format, bounds)) Console.WriteLine("copy tex image 2d fail");
                    if (api.CurrentTarget != this) api.CurrentTarget = null;
                });
            }

            if (command != null)
            {
                command.AddFence(_buffers.First(b => b.Attachment == attachment).Resource, BatchFlag.Read);
                command.AddFence(texture, BatchFlag.ReadWrite);
            }

            return texture;
        }

        public Texture CaptureDepth(bool temporary) => CaptureDepth(Viewport, temporary);

        public Texture CaptureDepth(Bounds2 bounds, bool temporary)
        {
            bounds.Intersect(Viewport);

            if (bounds.Width <= 0 || bounds.Height <= 0) return null;

            var attachment = FramebufferAttachment.DepthStencilAttachment;

            var format = GetFormat(attachment);

            if (format == 0)
            {
                attachment = FramebufferAttachment.DepthAttachment;

                format = GetFormat(attachment);

                if (format == 0) return null;
            }

            var textureDesc = new TextureDescription()
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Format = format
            };
            var texture = temporary ? Textures.NewTemporary(textureDesc) : new Texture(textureDesc);

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                texture.Allocate();

                var sampleDesc = new RenderTargetDescription()
                {
                    Width = bounds.Width,
                    Height = bounds.Height
                };
                var sampleTarget = RenderTargets.NewTemporary(sampleDesc);
                sampleTarget.Attach(attachment, texture, 0, false);
                Blit(sampleTarget, bounds, new Bounds2(0, 0, bounds.Width, bounds.Height), ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                sampleTarget.Detach(attachment);
                ResourcePool.Instance.Remove(sampleTarget);
            });

            if (command != null)
            {
                command.AddFence(_buffers.First(b => b.Attachment == attachment).Resource, BatchFlag.Read);
                command.AddFence(texture, BatchFlag.ReadWrite);
            }

            return texture;
        }

        private byte ConvertBitmapComponent(byte[] pixels, ref int index, PixelType pixelType)
        {
            switch(pixelType)
            {
                case PixelType.UnsignedByte:
                    return pixels[index++];
                case PixelType.Byte:
                    return (byte)((sbyte)pixels[index++] + 128);
                case PixelType.HalfFloat:
                    {
                        var r = (byte)(MathUtil.Clamp((float)Half.ToHalf(pixels, index), 0, 1) * 255);
                        index += 2;
                        return r;
                    }
                case PixelType.Float:
                    {
                        var r = (byte)(MathUtil.Clamp(BitConverter.ToSingle(pixels, index), 0, 1) * 255);
                        index += 4;
                        return r;
                    }
                case PixelType.UnsignedShort:
                    {
                        var r = (byte)(BitConverter.ToUInt16(pixels, index) * 255 / 65536);
                        index += 2;
                        return r;
                    }
                case PixelType.Short:
                    {
                        var r = (byte)((BitConverter.ToInt16(pixels, index) + 32768) * 255 / 65536);
                        index += 2;
                        return r;
                    }
                case PixelType.UnsignedInt:
                    {
                        var r = (byte)((long)BitConverter.ToUInt32(pixels, index) * 255 / uint.MaxValue);
                        index += 4;
                        return r;
                    }
                case PixelType.Int:
                    {
                        var r = (byte)(((long)BitConverter.ToInt32(pixels, index) - int.MinValue) * 255 / uint.MaxValue);
                        index += 4;
                        return r;
                    }
            }
            throw new NotImplementedException();
        }

        private unsafe void WriteBitmapRaw(byte* dest, byte[] pixels, ref int index, PixelFormat pixelFormat, PixelType pixelType)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Rgba:
                    dest[2] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[1] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[0] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[3] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    return;
                case PixelFormat.Rgb:
                    dest[2] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[1] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[0] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[3] = 255;
                    return;
                case PixelFormat.Rg:
                    dest[2] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[1] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[0] = 0;
                    dest[3] = 255;
                    return;
                case PixelFormat.Red:
                    dest[2] = ConvertBitmapComponent(pixels, ref index, pixelType);
                    dest[1] = 0;
                    dest[0] = 0;
                    dest[3] = 255;
                    return;
            }
            throw new NotImplementedException();
        }

        private Bitmap _captureBitmap;

        private void CaptureBitmapImpl(GraphicsApi api, int buf)
        {
            var viewport = _viewport[1];

            if (viewport.Width <= 0 || viewport.Height <= 0) return;

            var format = GetFormat(FramebufferAttachment.ColorAttachment0 + buf);
            var encoding = format.GetEncoding();

            Focus(api);

            api.ReadBuffer(buf);

            var pixels = new byte[viewport.Width * viewport.Height * encoding.PixelBpp];

            unsafe
            {
                fixed (void* p = pixels)
                {
                    api.ReadPixels(viewport, encoding.PixelFormat, encoding.PixelType, (IntPtr)p);
                }
            }

            _captureBitmap = new Bitmap(viewport.Width, viewport.Height);
            var bitmapData = _captureBitmap.LockBits(new System.Drawing.Rectangle(0, 0, viewport.Width, viewport.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapLength = viewport.Width * viewport.Height * 4;

            var pixelIndex = 0;

            unsafe
            {
                var p = (byte*)bitmapData.Scan0;
                for (var i = 0; i < bitmapLength; i += 4)
                {
                    WriteBitmapRaw(p + i, pixels, ref pixelIndex, encoding.PixelFormat, encoding.PixelType);
                }
            }
            _captureBitmap.UnlockBits(bitmapData);
            _captureBitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
        }

        public Bitmap CaptureBitmap(int buf)
        {
            if (GraphicsContext.Instance.IsRenderThread(out var api))
            {
                CaptureBitmapImpl(api, buf);
            }
            else
            {
                lock (this)
                {
                    GraphicsContext.Instance.Invoke(false, (GraphicsApi papi) =>
                    {
                        CaptureBitmapImpl(papi, buf);

                        lock (this)
                        {
                            Monitor.Pulse(this);
                        }
                    });

                    Monitor.Wait(this);
                }
            }
            var result = _captureBitmap;
            _captureBitmap = null;
            return result;
        }

        public void Blit(RenderTarget target, ClearBufferMask mask, BlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0)
        {
            Blit(target, Viewport, target.Viewport, mask, filter, srcbuf, destbuf);
        }

        public void Blit(RenderTarget target, in Bounds2 bounds, ClearBufferMask mask, BlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0)
        {
            Blit(target, Viewport, bounds, mask, filter, srcbuf, destbuf);
        }

        public void Blit(RenderTarget target, Bounds2 srcbounds, Bounds2 destbounds, ClearBufferMask mask, BlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0)
        {
            srcbounds.Intersect(Viewport);
            destbounds.Intersect(target.Viewport);

            if (srcbounds.Width <= 0 || srcbounds.Height <= 0 || destbounds.Width <= 0 || destbounds.Height <= 0) return;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                api.ApplyScissor(Bounds2.Zero);

                api.BindFramebuffer(FramebufferTarget.ReadFramebuffer, Framebuffer);
                api.BindFramebuffer(FramebufferTarget.DrawFramebuffer, target.Framebuffer);
                api.ReadBuffer(srcbuf);
                api.DrawBuffer(destbuf);

                api.BlitFramebuffer(srcbounds, destbounds, mask, filter);

                api.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

                api.DrawBuffers(_drawBuffers);

                if (api.CurrentTarget != this) api.CurrentTarget = null;
            });

            if (command != null)
            {
                command.AddFence(this, BatchFlag.Read | BatchFlag.Retrieve);
                command.AddFence(target, BatchFlag.ReadWrite);
            }
        }
    }
}

