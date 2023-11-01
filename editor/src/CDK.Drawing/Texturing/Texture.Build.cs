using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CDK.Drawing
{
    public partial class Texture
    {
        public static void GetEncodedSize(ref Size size, RawFormat[] formats)
        {
            var resizing = BitmapTextureResizing.None;

            foreach (var format in formats)
            {
                var encoding = format.GetEncoding();

                if (encoding.Compressed && resizing < encoding.BitmapResizing) resizing = encoding.BitmapResizing;
            }

            size = BitmapTexture.ConvertSize(size, resizing);
        }

        private static string GetCachePath(string prefix, string pvrFormat, int mipmapCount)
        {
            var suffix = mipmapCount != 1 ? $"_m{mipmapCount}.pvr" : ".pvr";
            return $"{prefix}.{pvrFormat.Replace(',', '_').ToLower()}{suffix}";
        }

        public static string GetCachePath(string prefix, RawFormat format, int mipmapCount)
        {
            var encoding = format.GetEncoding();

            return encoding.Compressed ? GetCachePath(prefix, encoding.CompressPvrFormat, mipmapCount) : null;
        }

        public static string GetCachePath(string prefix, in TextureDescription desc)
        {
            return GetCachePath(prefix, desc.Format, desc.MipmapCount);
        }

        public static void GetCachePaths(string prefix, RawFormat[] formats, int mipmapCount, List<string> paths)
        {
            foreach (var format in formats)
            {
                var encoding = format.GetEncoding();

                if (encoding.Compressed) paths.Add(GetCachePath(prefix, encoding.CompressPvrFormat, mipmapCount));
            }
        }

        private static void BuildImpl(BinaryWriter writer, Bitmap bitmap, in TextureDescription desc, string cachePrefix)
        {
            var encoding = desc.Format.GetEncoding();

            if (encoding.Compressed)
            {
                var size = BitmapTexture.ConvertSize(bitmap.Size, encoding.BitmapResizing);

                var cachePath = GetCachePath(cachePrefix, desc);

                var texture = PVRTexture.Encode(bitmap, encoding.CompressPvrFormat, desc.MipmapCount, encoding.BitmapColor, encoding.BitmapResizing, cachePath);
                if (texture == null || texture.Depth != 1 || texture.NumSurfaces != 1 || texture.NumFaces != 1)
                {
                    throw new IOException($"unabled to encode compressed texture:{desc.Format}");
                }

                writer.Write((byte)texture.MipmapCount);

                var offset = 0;
                for (var i = 0; i < texture.MipmapCount; i++)
                {
                    var blockWidth = (size.Width + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    var blockHeight = (size.Height + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    var blockLength = blockWidth * blockHeight * encoding.CompressBlockLength;

                    writer.Write((ushort)blockWidth * encoding.CompressBlock);
                    writer.Write((ushort)blockHeight * encoding.CompressBlock);
                    writer.Write((ushort)0);        //depth
                    writer.Write(texture.Data, offset, blockLength);

                    offset += blockLength;

                    Console.WriteLine($"build compressed texture:{desc.Format} level:{i} length:{blockLength} = {offset} / {texture.Data.Length}");

                    size.Width = Math.Max(1, size.Width >> 1);
                    size.Height = Math.Max(1, size.Height >> 1);
                }
                if (offset != texture.Data.Length)
                {
                    throw new IOException($"invalid build offset compressed texture:{desc.Format} offset:{offset} / {texture.Data.Length}");
                }
            }
            else
            {
                if (encoding.BitmapColor == BitmapTextureColor.None) throw new IOException();

                writer.Write((byte)1);      //level count, use generateMipmap later
                writer.Write((ushort)bitmap.Width);
                writer.Write((ushort)bitmap.Height);
                writer.Write((ushort)0);    //depth

                BitmapTexture.Build(writer, bitmap, encoding.BitmapColor);
            }
        }

        public static void Build(BinaryWriter writer, Bitmap[] bitmaps, RawFormat[] formats, TextureDescription desc, string cachePrefix)
        {
            desc.Width = bitmaps[0].Width;
            desc.Height = bitmaps[0].Height;
            desc.Validate();

            writer.Write((ushort)desc.Width);
            writer.Write((ushort)desc.Height);
            writer.Write((ushort)desc.Depth);
            writer.Write((ushort)desc.Target);
            writer.Write((ushort)desc.WrapS);
            writer.Write((ushort)desc.WrapT);
            writer.Write((ushort)desc.WrapR);
            writer.Write((ushort)desc.MinFilter);
            writer.Write((ushort)desc.MagFilter);
            writer.Write(desc.BorderColor.ToRgba());
            writer.Write((byte)desc.MipmapCount);

            writer.Write((byte)formats.Length);
            foreach (var format in formats)
            {
                writer.Write((ushort)format);

                var prevPos = writer.BaseStream.Position;
                writer.Write(0);

                writer.Write((byte)bitmaps.Length);

                for (var face = 0; face < bitmaps.Length; face++)
                {
                    desc.Format = format;

                    BuildImpl(writer, bitmaps[face], desc, $"{cachePrefix}.{face}");
                }

                var nextPos = writer.BaseStream.Position;
                writer.BaseStream.Position = prevPos;
                writer.Write((int)(nextPos - prevPos));
                writer.BaseStream.Position = nextPos;
            }
        }

        public static void Build(BinaryWriter writer, Bitmap[] bitmaps, in TextureDescription desc, string cachePrefix)
        {
            Build(writer, bitmaps, new RawFormat[] { desc.Format }, desc, cachePrefix);
        }

        public static void Build(BinaryWriter writer, Bitmap bitmap, RawFormat[] formats, in TextureDescription desc, string cachePrefix)
        {
            switch (desc.Target)
            {
                case TextureTarget.TextureCubeMap:
                    {
                        var bitmaps = CreateCubeBitmaps(bitmap);

                        if (bitmaps == null) throw new IOException();

                        Build(writer, bitmaps, formats, desc, cachePrefix);
                    }
                    break;
                case TextureTarget.Texture2D:
                    Build(writer, new Bitmap[] { bitmap }, formats, desc, cachePrefix);
                    break;
                default:    //only support 2d now
                    throw new NotImplementedException();
            }
        }

        public static void Build(BinaryWriter writer, Bitmap bitmap, in TextureDescription desc, string cachePrefix)
        {
            Build(writer, bitmap, new RawFormat[] { desc.Format }, desc, cachePrefix);
        }

        public static Bitmap Decode(Bitmap image, in TextureDescription desc, string cachePrefix)
        {
            var encoding = desc.Format.GetEncoding();

            if (encoding.Compressed)
            {
                var cachePath = GetCachePath(cachePrefix, desc);

                return PVRTexture.Decode(image, encoding.CompressPvrFormat, desc.MipmapCount, encoding.BitmapColor, encoding.BitmapResizing, cachePath);
            }
            else return BitmapTexture.Convert(image, encoding.BitmapColor, encoding.BitmapResizing);
        }
    }
}
