using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Drawing
{
    public partial class Texture
    {
        private void ApplyTextureDescription(GraphicsApi api)
        {
            if (_Description.Target == TextureTarget.Texture2DMultisample) return;      //Texture2DMultisample not support TexParameter

            api.TexParameterWrap(_Description.Target, 0, _Description.WrapS);
            api.TexParameterWrap(_Description.Target, 1, _Description.WrapT);
            api.TexParameterWrap(_Description.Target, 2, _Description.WrapR);
            api.TexParameterMinFilter(_Description.Target, _Description.MinFilter);
            api.TexParameterMagFilter(_Description.Target, _Description.MagFilter);
            api.TexParameterBorderColor(_Description.Target, _Description.BorderColor);
        }

        public Texture(Bitmap bitmap, TextureDescription desc, string cachePrefix)
        {
            switch (desc.Target)
            {
                case TextureTarget.TextureCubeMap:
                    {
                        var bitmaps = CreateCubeBitmaps(bitmap);

                        if (bitmaps == null) throw new InvalidDataException();

                        desc.Width = bitmaps[0].Width;
                        desc.Height = bitmaps[0].Height;
                        desc.Validate();

                        var ms = new MemoryStream();

                        try
                        {
                            using (var writer = new BinaryWriter(ms, Encoding.Default, true))
                            {
                                Build(writer, bitmaps, desc, cachePrefix);
                            }
                            ms.Position = 0;
                            Load(ms);
                        }
                        finally
                        {
                            foreach (var b in bitmaps) b.Dispose();
                        }
                    }
                    break;
                case TextureTarget.Texture2D:
                    {
                        desc.Width = bitmap.Width;
                        desc.Height = bitmap.Height;
                        desc.Validate();

                        var ms = new MemoryStream();
                        using (var writer = new BinaryWriter(ms, Encoding.Default, true))
                        {
                            Build(writer, bitmap, desc, cachePrefix);
                        }
                        ms.Position = 0;
                        Load(ms);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Bitmap[] CreateCubeBitmaps(Bitmap src)
        {
            var width = src.Width / 4;
            var height = src.Height / 3;

            if (width * 4 != src.Width || height * 3 != src.Height)
            {
                Console.WriteLine("invalid bitmap size");
                return null;
            }

            var size = Math.Max(width, height);

            GDIRectangle[] frames =
            {
                new GDIRectangle(width * 2, height, width, height),
                new GDIRectangle(0, height, width, height),
                new GDIRectangle(width * 3, height, width, height),
                new GDIRectangle(width, height, width, height),
                new GDIRectangle(width, 0, width, height),
                new GDIRectangle(width, height * 2, width, height)
            };
            RotateFlipType[] rotations =
            {
                RotateFlipType.Rotate270FlipNone,
                RotateFlipType.Rotate90FlipNone,
                RotateFlipType.Rotate180FlipNone,
                RotateFlipType.RotateNoneFlipNone,
                RotateFlipType.RotateNoneFlipNone,
                RotateFlipType.Rotate180FlipNone
            };

            var dest = new Bitmap[6];

            for (var face = 0; face < 6; face++)
            {
                dest[face] = new Bitmap(size, size);

                using (var g = System.Drawing.Graphics.FromImage(dest[face]))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(src, new GDIRectangle(0, 0, size, size), frames[face], GraphicsUnit.Pixel);
                }

                dest[face].RotateFlip(rotations[face]);
            }
            
            return dest;
        }

        public Texture(in TextureDescription desc, bool allocate = true)
        {
            _Description = desc;

            _Description.Validate();

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Object = api.GenTexture();

                api.BindTexture(_Description.Target, Object);

                ApplyTextureDescription(api);
            });
            command?.AddFence(this, BatchFlag.ReadWrite);

            if (allocate) Allocate();
        }

        public void Resize(int width, int height)
        {
            _Description.Width = width;
            _Description.Height = height;

            if (Allocated)
            {
                Allocated = false;

                Allocate();
            }
        }

        public void Allocate()
        {
            if (!Allocated)
            {
                _Description.MipmapCount = 1;
                
                Allocated = true;

                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    api.BindTexture(_Description.Target, Object);

                    switch (_Description.Target)
                    {
                        case TextureTarget.Texture2D:
                        case TextureTarget.Texture3D:
                        case TextureTarget.Texture2DMultisample:
                            Upload(api, null, _Description.Target, _Description.Width, _Description.Height, _Description.Depth, _Description.Format, 0, _Description.Samples);
                            break;
                        case TextureTarget.TextureCubeMap:
                            for (var face = 0; face < 6; face++)
                            {
                                Upload(api, null, TextureTarget.TextureCubeMapPositiveX + face, _Description.Width, _Description.Height, _Description.Depth, _Description.Format, 0, _Description.Samples);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                });

                command?.AddFence(this, BatchFlag.ReadWrite);
            }
        }

        private void Load(MemoryStream ms)
        {
            var reader = new BinaryReader(ms);

            _Description = new TextureDescription
            {
                Width = reader.ReadUInt16(),
                Height = reader.ReadUInt16(),
                Depth = reader.ReadUInt16(),
                Target = (TextureTarget)reader.ReadUInt16(),
                WrapS = (TextureWrapMode)reader.ReadUInt16(),
                WrapT = (TextureWrapMode)reader.ReadUInt16(),
                WrapR = (TextureWrapMode)reader.ReadUInt16(),
                MinFilter = (TextureMinFilter)reader.ReadUInt16(),
                MagFilter = (TextureMagFilter)reader.ReadUInt16(),
                BorderColor = new Color4(reader.ReadUInt32()),
                MipmapCount = reader.ReadByte(),
                Samples = 1
            };

            Allocated = true;
            
            var encodingCount = reader.ReadByte();

            for (var i = 0; i < encodingCount; i++)
            {
                var format = (RawFormat)reader.ReadUInt16();

                var nextPos = reader.BaseStream.Position + reader.ReadInt32();

                if (!format.IsSupported())
                {
                    reader.BaseStream.Position = nextPos;
                    continue;
                }

                _Description.Format = format;

                var faceCount = reader.ReadByte();

                switch (_Description.Target)
                {
                    case TextureTarget.TextureCubeMap:
                        if (faceCount != 6)
                        {
                            reader.Dispose();
                            ms.Dispose();
                            throw new InvalidDataException();
                        }
                        break;
                    case TextureTarget.Texture2D:
                    case TextureTarget.Texture3D:
                        if (faceCount != 1)
                        {
                            reader.Dispose();
                            ms.Dispose();
                            throw new InvalidDataException();
                        }
                        break;
                    default:
                        reader.Dispose();
                        ms.Dispose();
                        throw new NotImplementedException();
                }
                    
                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    var obj = 0;
                    try
                    {
                        obj = api.GenTexture();

                        api.BindTexture(_Description.Target, obj);

                        ApplyTextureDescription(api);

                        var encoding = _Description.Format.GetEncoding();

                        switch (_Description.Target)
                        {
                            case TextureTarget.TextureCubeMap:
                                for (var face = 0; face < 6; face++)
                                {
                                    var levelCount = reader.ReadByte();

                                    if (encoding.Compressed && _Description.MipmapCount > levelCount)
                                    {
                                        _Description.MipmapCount = levelCount;
                                    }

                                    for (var level = 0; level < levelCount; level++)
                                    {
                                        var subwidth = reader.ReadInt16();
                                        var subheight = reader.ReadInt16();
                                        var subdepth = reader.ReadInt16();

                                        Upload(api, reader, TextureTarget.TextureCubeMapPositiveX + face, subwidth, subheight, subdepth, format, level, 1);
                                    }
                                }
                                break;
                            case TextureTarget.Texture2D:
                            case TextureTarget.Texture3D:
                                {
                                    var levelCount = reader.ReadByte();

                                    if (encoding.Compressed && _Description.MipmapCount > levelCount)
                                    {
                                        _Description.MipmapCount = levelCount;
                                    }

                                    var subwidth = reader.ReadInt16();
                                    var subheight = reader.ReadInt16();
                                    var subdepth = reader.ReadInt16();

                                    for (var level = 0; level < levelCount; level++)
                                    {
                                        Upload(api, reader, _Description.Target, subwidth, subheight, subdepth, format, level, 1);
                                    }
                                }
                                break;
                        }

                        if (!encoding.Compressed && _Description.MipmapCount > 1)
                        {
                            if (!api.GenerateMipmap(_Description.Target, _Description.MipmapCount - 1)) Console.WriteLine("generate mipmap fail");
                        }

                        Object = obj;
                    }
                    catch (Exception e)
                    {
                        if (obj != 0) api.DeleteTexture(obj);
                        throw e;
                    }
                    finally
                    {
                        reader.Dispose();
                        ms.Dispose();
                    }
                });

                command?.AddFence(this, BatchFlag.ReadWrite);

                return;
            }

            reader.Dispose();
            ms.Dispose();
            throw new NotSupportedException();
        }

        private void Upload(GraphicsApi api, BinaryReader reader, TextureTarget target, int width, int height, int depth, RawFormat format, int level, int samples)
        {
            var encoding = format.GetEncoding();

            if (encoding.Compressed)
            {
                if (reader == null) throw new IOException("no compressed content");

                switch (target)
                {
                    case TextureTarget.Texture2D:
                    case TextureTarget.TextureCubeMapPositiveX:
                    case TextureTarget.TextureCubeMapNegativeX:
                    case TextureTarget.TextureCubeMapPositiveY:
                    case TextureTarget.TextureCubeMapNegativeY:
                    case TextureTarget.TextureCubeMapPositiveZ:
                    case TextureTarget.TextureCubeMapNegativeZ:
                        {
                            var blockWidth = (width + encoding.CompressBlock - 1) / encoding.CompressBlock;
                            var blockHeight = (height + encoding.CompressBlock - 1) / encoding.CompressBlock;
                            width = blockWidth * encoding.CompressBlock;
                            height = blockHeight * encoding.CompressBlock;
                            var dataLength = blockWidth * blockHeight * encoding.CompressBlockLength;

                            var raw = reader.ReadBytes(dataLength);

                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.CompressedTexImage2D(target, level, format, width, height, dataLength, (IntPtr)p);
                                }
                            }
                        }
                        break;
                    case TextureTarget.Texture3D:
                        {
                            var blockWidth = (width + encoding.CompressBlock - 1) / encoding.CompressBlock;
                            var blockHeight = (height + encoding.CompressBlock - 1) / encoding.CompressBlock;
                            var blockDepth = (depth + encoding.CompressBlock - 1) / encoding.CompressBlock;
                            width = blockWidth * encoding.CompressBlock;
                            height = blockHeight * encoding.CompressBlock;
                            depth = blockDepth * encoding.CompressBlock;
                            var dataLength = blockWidth * blockHeight * blockDepth * encoding.CompressBlockLength;

                            var raw = reader.ReadBytes(dataLength);

                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.CompressedTexImage3D(target, level, format, width, height, depth, dataLength, (IntPtr)p);
                                }
                            }
                        }
                        break;
                    default:
                        throw new NotSupportedException($"unsupported compressed target:{target}");
                }
            }
            else
            {
                switch (target)
                {
                    case TextureTarget.Texture2D:
                    case TextureTarget.TextureCubeMapPositiveX:
                    case TextureTarget.TextureCubeMapNegativeX:
                    case TextureTarget.TextureCubeMapPositiveY:
                    case TextureTarget.TextureCubeMapNegativeY:
                    case TextureTarget.TextureCubeMapPositiveZ:
                    case TextureTarget.TextureCubeMapNegativeZ:
                        if (reader != null)
                        {
                            var dataLength = width * height * encoding.PixelBpp;
                            var raw = reader.ReadBytes(dataLength);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexImage2D(target, level, format, width, height, (IntPtr)p);
                                }
                            }
                        }
                        else api.TexImage2D(target, level, format, width, height, IntPtr.Zero);
                        break;
                    case TextureTarget.Texture2DMultisample:
                        if (reader != null) throw new NotSupportedException($"unsupported raw target with data:{target}");
                        api.TexImage2DMultisample(target, samples, format, width, height);
                        break;
                    case TextureTarget.Texture3D:
                        if (reader != null)
                        {
                            var dataLength = width * height * depth * encoding.PixelBpp;
                            var raw = reader.ReadBytes(dataLength);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexImage3D(target, level, format, width, height, depth, (IntPtr)p);
                                }
                            }
                        }
                        else api.TexImage3D(target, level, format, width, height, depth, IntPtr.Zero);
                        break;
                    default:
                        throw new NotSupportedException($"unsupported raw target:{target}");
                }
            }
        }

        public void Upload<T>(T[] raw, int level, int width, int height, int depth) where T : unmanaged
        {
            var encoding = _Description.Format.GetEncoding();
            Debug.Assert(!encoding.Compressed);

            switch (Target)
            {
                case TextureTarget.Texture2D:
                    Debug.Assert(raw.Length * Marshal.SizeOf<T>() == width * height * encoding.PixelBpp);
                    
                    if (level == 0)
                    {
                        _Description.Width = width;
                        _Description.Height = height;
                        Allocated = true;
                    }
                    else
                    {
                        Debug.Assert(Allocated && width == (Width >> level) && height == (Height >> level));
                    }
                    {
                        var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            api.BindTexture(TextureTarget.Texture2D, Object);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexImage2D(TextureTarget.Texture2D, level, Format, width, height, (IntPtr)p);
                                }
                            }
                        });
                        command?.AddFence(this, BatchFlag.Write);
                    }
                    break;
                case TextureTarget.Texture3D:
                    Debug.Assert(raw.Length * Marshal.SizeOf<T>() == width * height * depth * encoding.PixelBpp);

                    if (level == 0)
                    {
                        _Description.Width = width;
                        _Description.Height = height;
                        _Description.Depth = depth;
                        Allocated = true;
                    }
                    else
                    {
                        Debug.Assert(Allocated && width == (Width >> level) && height == (Height >> level) && depth == (Depth >> level));
                    }

                    {
                        var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            api.BindTexture(TextureTarget.Texture3D, Object);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexImage3D(TextureTarget.Texture3D, level, Format, width, height, depth, (IntPtr)p);
                                }
                            }
                        });
                        command?.AddFence(this, BatchFlag.Write);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void UploadSub<T>(T[] raw, int level, int x, int y, int z, int width, int height, int depth) where T : unmanaged
        {
            var encoding = _Description.Format.GetEncoding();
            Debug.Assert(Allocated && !encoding.Compressed && x >= 0 && y >= 0 && x + width <= (Width >> level) && y + height <= (Height >> level));

            switch (Target)
            {
                case TextureTarget.Texture2D:
                    Debug.Assert(raw.Length * Marshal.SizeOf<T>() == width * height * encoding.PixelBpp);

                    {
                        var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            api.BindTexture(TextureTarget.Texture2D, Object);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexSubImage2D(TextureTarget.Texture2D, level, Format, new Bounds2(x, y, width, height), (IntPtr)p);
                                }
                            }
                        });
                        command?.AddFence(this, BatchFlag.Write);
                    }
                    break;
                case TextureTarget.Texture3D:
                    Debug.Assert(raw.Length * Marshal.SizeOf<T>() == width * height * depth * encoding.PixelBpp);
                    Debug.Assert(z >= 0 && z + depth <= (Depth >> level));

                    {
                        var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                        {
                            api.BindTexture(TextureTarget.Texture3D, Object);
                            unsafe
                            {
                                fixed (void* p = raw)
                                {
                                    api.TexSubImage3D(TextureTarget.Texture3D, level, Format, new Bounds3(x, y, z, width, height, depth), (IntPtr)p);
                                }
                            }
                        });
                        command?.AddFence(this, BatchFlag.Write);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public bool Reload(TextureDescription desc)
        {
            if (desc.Target == 0) desc.Target = _Description.Target;
            else if (desc.Target != _Description.Target) return false;
            
            if (desc.Width == 0) desc.Width = _Description.Width;
            else if (desc.Width != _Description.Width) return false;
            
            if (desc.Height == 0) desc.Height = _Description.Height;
            else if (desc.Height != _Description.Height) return false;

            if (desc.Depth == 0) desc.Depth = _Description.Depth;
            else if (desc.Depth != _Description.Depth) return false;

            if (desc.Format == 0) desc.Format = _Description.Format;
            else if (desc.Format != _Description.Format) return false;
            
            if (desc.WrapS == 0) desc.WrapS = _Description.WrapS;
            if (desc.WrapT == 0) desc.WrapT = _Description.WrapT;
            if (desc.WrapR == 0) desc.WrapR = _Description.WrapR;
            if (desc.MinFilter == 0) desc.MinFilter = _Description.MinFilter;
            if (desc.MagFilter == 0) desc.MagFilter = _Description.MagFilter;

            var generateMipmap = false;
            if (_Description.MipmapCount > 1 || desc.Format.GetEncoding().Compressed) desc.MipmapCount = _Description.MipmapCount;
            else if (Allocated && desc.MipmapCount > 1)
            {
                desc.MipmapCount = Math.Min(desc.MipmapCount, desc.MaxMipmapCount);
                if (desc.MipmapCount > 1) generateMipmap = true;
            }
            else desc.MipmapCount = 1;

            if (_Description != desc)
            {
                _Description = desc;

                var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    api.BindTexture(_Description.Target, Object);

                    ApplyTextureDescription(api);

                    if (generateMipmap && !api.GenerateMipmap(_Description.Target, _Description.MipmapCount - 1)) Console.WriteLine("generate mipmap fail");
                });
                command?.AddFence(this, BatchFlag.ReadWrite);
            }

            return true;
        }
    }
}
