using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using GDIRectangle = System.Drawing.Rectangle;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CDK.Drawing
{
    public enum BitmapTextureColor
    {
        None,

        R8i,
        R8ui,
        R16f,
        R16i,
        R16ui,
        R32f,
        R32i,
        R32ui,
        Rg8i,
        Rg8ui,
        Rg16f,
        Rg16i,
        Rg16ui,
        Rg32f,
        Rg32i,
        Rg32ui,
        Rgb8i,
        Rgb8ui,
        Rgb16f,
        Rgb16i,
        Rgb16ui,
        Rgb32f,
        Rgb32i,
        Rgb32ui,
        Rgba8i,
        Rgba8ui,
        Rgba16f,
        Rgba16i,
        Rgba16ui,
        Rgba32f,
        Rgba32i,
        Rgba32ui,
        Rgb5,
        Rgb5A1,
        Rgba4,
        L8,
        A8,
        La8,
        R24,
        R24G8,
        R32fG8
    }

    public enum BitmapTextureResizing
    {
        None,
        X4,
        Pot,
        PotSquared
    }

    public class BitmapTexture
    {
        public static Size ConvertSize(Size size, BitmapTextureResizing resizing)
        {
            switch (resizing)
            {
                case BitmapTextureResizing.X4:
                    size.Width = (size.Width + 3) / 4 * 4;
                    size.Height = (size.Height + 3) / 4 * 4;
                    break;
                case BitmapTextureResizing.Pot:
                    {
                        Size convSize = new Size();
                        convSize.Width = 4;
                        convSize.Height = 4;

                        while (convSize.Width < size.Width)
                        {
                            convSize.Width <<= 1;
                        }
                        while (convSize.Height < size.Height)
                        {
                            convSize.Height <<= 1;
                        }
                        size = convSize;
                    }
                    break;
                case BitmapTextureResizing.PotSquared:
                    {
                        Size convSize = new Size();
                        convSize.Width = 4;
                        convSize.Height = 4;

                        while (convSize.Width < size.Width)
                        {
                            convSize.Width <<= 1;
                        }
                        while (convSize.Height < size.Height)
                        {
                            convSize.Height <<= 1;
                        }
                        if (convSize.Width < convSize.Height) convSize.Width = convSize.Height;
                        else if (convSize.Width > convSize.Height) convSize.Height = convSize.Width;
                        size = convSize;
                    }
                    break;
            }
            return size;
        }

        private delegate void ConvertColorFunc(ref byte r, ref byte g, ref byte b, ref byte a);

        private static void ConvertColorR(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            g = 0;
            b = 0;
            a = 255;
        }

        private static void ConvertColorRg(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            b = 0;
            a = 255;
        }

        private static void ConvertColorRgb(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            a = 255;
        }

        private static void ConvertColorRgb5(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = (byte)(255 * (r >> 3) / 31);
            g = (byte)(255 * (g >> 2) / 63);
            b = (byte)(255 * (b >> 3) / 31);
            a = 255;
        }

        private static void ConvertColorRgb5A1(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = (byte)(255 * (r >> 3) / 31);
            g = (byte)(255 * (g >> 3) / 31);
            b = (byte)(255 * (b >> 3) / 31);
            a = (byte)(255 * (a >> 7));
        }

        private static void ConvertColorRgba4(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = (byte)(255 * (r >> 4) / 15);
            g = (byte)(255 * (g >> 4) / 15);
            b = (byte)(255 * (b >> 4) / 15);
            a = (byte)(255 * (a >> 4) / 15);
        }

        private static void ConvertColorL(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = g = b = (byte)((r + g + b) / 3);
            a = 255;
        }

        private static void ConvertColorA(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = g = b = 255;
        }

        private static void ConvertColorLa(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            r = g = b = (byte)((r + g + b) / 3);
        }

        private static void ConvertColor(Bitmap image, ConvertColorFunc func)
        {
            if (image.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new NotSupportedException();

            var w = image.Width;
            var h = image.Height;
            var imageData = image.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadWrite, GDIPixelFormat.Format32bppArgb);

            unsafe
            {
                var dptr = (byte*)imageData.Scan0;

                var len = 4 * w * h;

                for (var i = 0; i < len; i += 4)
                {
                    func(ref dptr[2], ref dptr[1], ref dptr[0], ref dptr[3]);

                    dptr += 4;
                }
            }
            image.UnlockBits(imageData);
        }

        public static Bitmap Convert(Bitmap image, BitmapTextureColor color, BitmapTextureResizing resizing)
        {
            var size = new Size(image.Width, image.Height);

            var convSize = ConvertSize(size, resizing);
            
            Bitmap conv;

            if (convSize != size)
            {
                conv = new Bitmap(convSize.Width, convSize.Height);

                using (var g = System.Drawing.Graphics.FromImage(conv))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(image, new GDIRectangle(0, 0, conv.Width, conv.Height));
                }
            }
            else
            {
                conv = new Bitmap(image);
            }
            switch (color)
            {

                case BitmapTextureColor.R8ui:
                case BitmapTextureColor.R8i:
                case BitmapTextureColor.R16f:
                case BitmapTextureColor.R16ui:
                case BitmapTextureColor.R16i:
                case BitmapTextureColor.R32f:
                case BitmapTextureColor.R32ui:
                case BitmapTextureColor.R32i:
                    ConvertColor(conv, ConvertColorR);
                    break;
                case BitmapTextureColor.Rg8ui:
                case BitmapTextureColor.Rg8i:
                case BitmapTextureColor.Rg16f:
                case BitmapTextureColor.Rg16ui:
                case BitmapTextureColor.Rg16i:
                case BitmapTextureColor.Rg32f:
                case BitmapTextureColor.Rg32ui:
                case BitmapTextureColor.Rg32i:
                    ConvertColor(conv, ConvertColorRg);
                    break;
                case BitmapTextureColor.Rgb8ui:
                case BitmapTextureColor.Rgb8i:
                case BitmapTextureColor.Rgb16f:
                case BitmapTextureColor.Rgb16ui:
                case BitmapTextureColor.Rgb16i:
                case BitmapTextureColor.Rgb32f:
                case BitmapTextureColor.Rgb32ui:
                case BitmapTextureColor.Rgb32i:
                    ConvertColor(conv, ConvertColorRgb);
                    break;
                case BitmapTextureColor.Rgba8ui:
                case BitmapTextureColor.Rgba8i:
                case BitmapTextureColor.Rgba16f:
                case BitmapTextureColor.Rgba16ui:
                case BitmapTextureColor.Rgba16i:
                case BitmapTextureColor.Rgba32f:
                case BitmapTextureColor.Rgba32ui:
                case BitmapTextureColor.Rgba32i:
                    break;
                case BitmapTextureColor.Rgb5:
                    ConvertColor(conv, ConvertColorRgb5);
                    break;
                case BitmapTextureColor.Rgb5A1:
                    ConvertColor(conv, ConvertColorRgb5A1);
                    break;
                case BitmapTextureColor.Rgba4:
                    ConvertColor(conv, ConvertColorRgba4);
                    break;
                case BitmapTextureColor.L8:
                    ConvertColor(conv, ConvertColorL);
                    break;
                case BitmapTextureColor.A8:
                    ConvertColor(conv, ConvertColorA);
                    break;
                case BitmapTextureColor.La8:
                    ConvertColor(conv, ConvertColorLa);
                    break;
                case BitmapTextureColor.R24G8:
                case BitmapTextureColor.R32fG8:
                    ConvertColor(conv, ConvertColorRg);
                    break;
            }
            return conv;
        }

        private delegate void BuildFunc(BinaryWriter writer, byte r, byte g, byte b, byte a);

        private static void BuildR8ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r);
        }

        private static void BuildR8i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((sbyte)(r - 128));
        }

        private static void BuildR16f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(Half.GetBits((Half)(r / 255.0f)));
        }

        private static void BuildR16ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(r * 65535 / 255));
        }

        private static void BuildR16i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((short)((r * 65535 / 255) - 32768));
        }

        private static void BuildR32f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r / 255.0f);
        }

        private static void BuildR32ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)((long)r * uint.MaxValue / 255));
        }

        private static void BuildR32i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((int)((long)r * uint.MaxValue / 255 + int.MinValue));
        }

        private static void BuildRg8ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r);
            writer.Write(g);
        }

        private static void BuildRg8i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((sbyte)(r - 128));
            writer.Write((sbyte)(g - 128));
        }

        private static void BuildRg16f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(Half.GetBits((Half)(r / 255.0f)));
            writer.Write(Half.GetBits((Half)(g / 255.0f)));
        }

        private static void BuildRg16ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(r * 65535 / 255));
            writer.Write((ushort)(g * 65535 / 255));
        }

        private static void BuildRg16i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((short)((r * 65535 / 255) - 32768));
            writer.Write((short)((g * 65535 / 255) - 32768));
        }

        private static void BuildRg32f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r / 255.0f);
            writer.Write(g / 255.0f);
        }

        private static void BuildRg32ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)((long)r * uint.MaxValue / 255));
            writer.Write((uint)((long)g* uint.MaxValue / 255));
        }

        private static void BuildRg32i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((int)((long)r * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)g * uint.MaxValue / 255 + int.MinValue));
        }
        private static void BuildRgb8ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
        }

        private static void BuildRgb8i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((sbyte)(r - 128));
            writer.Write((sbyte)(g - 128));
            writer.Write((sbyte)(b - 128));
        }

        private static void BuildRgb16f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(Half.GetBits((Half)(r / 255.0f)));
            writer.Write(Half.GetBits((Half)(g / 255.0f)));
            writer.Write(Half.GetBits((Half)(b / 255.0f)));
        }

        private static void BuildRgb16ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(r * 65535 / 255));
            writer.Write((ushort)(g * 65535 / 255));
            writer.Write((ushort)(b * 65535 / 255));
        }

        private static void BuildRgb16i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((short)((r * 65535 / 255) - 32768));
            writer.Write((short)((g * 65535 / 255) - 32768));
            writer.Write((short)((b * 65535 / 255) - 32768));
        }

        private static void BuildRgb32f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r / 255.0f);
            writer.Write(g / 255.0f);
            writer.Write(b / 255.0f);
        }

        private static void BuildRgb32ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)((long)r * uint.MaxValue / 255));
            writer.Write((uint)((long)g * uint.MaxValue / 255));
            writer.Write((uint)((long)b * uint.MaxValue / 255));
        }

        private static void BuildRgb32i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((int)((long)r * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)g * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)b * uint.MaxValue / 255 + int.MinValue));
        }

        private static void BuildRgba8ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
            writer.Write(a);
        }

        private static void BuildRgba8i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((sbyte)(r - 128));
            writer.Write((sbyte)(g - 128));
            writer.Write((sbyte)(b - 128));
            writer.Write((sbyte)(a - 128));
        }

        private static void BuildRgba16f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(Half.GetBits((Half)(r / 255.0f)));
            writer.Write(Half.GetBits((Half)(g / 255.0f)));
            writer.Write(Half.GetBits((Half)(b / 255.0f)));
            writer.Write(Half.GetBits((Half)(a / 255.0f)));
        }

        private static void BuildRgba16ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(r * 65535 / 255));
            writer.Write((ushort)(g * 65535 / 255));
            writer.Write((ushort)(b * 65535 / 255));
            writer.Write((ushort)(a * 65535 / 255));
        }

        private static void BuildRgba16i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((short)((r * 65535 / 255) - 32768));
            writer.Write((short)((g * 65535 / 255) - 32768));
            writer.Write((short)((b * 65535 / 255) - 32768));
            writer.Write((short)((a * 65535 / 255) - 32768));
        }

        private static void BuildRgba32f(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r / 255.0f);
            writer.Write(g / 255.0f);
            writer.Write(b / 255.0f);
            writer.Write(a / 255.0f);
        }

        private static void BuildRgba32ui(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)((long)r * uint.MaxValue / 255));
            writer.Write((uint)((long)g * uint.MaxValue / 255));
            writer.Write((uint)((long)b * uint.MaxValue / 255));
            writer.Write((uint)((long)a * uint.MaxValue / 255));
        }

        private static void BuildRgba32i(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((int)((long)r * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)g * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)b * uint.MaxValue / 255 + int.MinValue));
            writer.Write((int)((long)a * uint.MaxValue / 255 + int.MinValue));
        }

        private static void BuildRgb5(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(
                (r >> 3 << 11) |
                (g >> 2 << 5) |
                (b >> 3)));
        }

        private static void BuildRgb5A1(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(
                (r >> 3 << 11) |
                (g >> 3 << 6) |
                (b >> 3 << 1) |
                (a >> 7)));
        }

        private static void BuildRgba4(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((ushort)(
                (r >> 4 << 12) |
                (g >> 4 << 8) |
                (b >> 4 << 4) |
                (a >> 4)));
        }

        private static void BuildR24(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)(r * 0xFFFFFF / 255));
        }

        private static void BuildR24G8(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((uint)((r * 0xFFFFFF / 255) << 8 | g));
        }

        private static void BuildR32fG8(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(r / 255.0f);
            writer.Write(g);
        }

        private static void BuildL8(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((byte)((r + g + b) / 3));
        }

        private static void BuildA8(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write(a);
        }

        private static void BuildLa8(BinaryWriter writer, byte r, byte g, byte b, byte a)
        {
            writer.Write((byte)((r + g + b) / 3));
            writer.Write(a);
        }

        private static void Build(BinaryWriter writer, Bitmap image, BuildFunc func)
        {
            if (image.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new NotSupportedException();

            var w = image.Width;
            var h = image.Height;

            var imageData = image.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            unsafe
            {
                var imageptr = (byte*)imageData.Scan0;

                int len = w * h;

                for (int i = 0; i < len; i++)
                {
                    byte r = imageptr[2];
                    byte g = imageptr[1];
                    byte b = imageptr[0];
                    byte a = imageptr[3];

                    func(writer, r, g, b, a);

                    imageptr += 4;
                }
            }
            image.UnlockBits(imageData);
        }

        public static void Build(BinaryWriter writer, Bitmap image, BitmapTextureColor color)
        {
            switch (color)
            {
                case BitmapTextureColor.R8ui:
                    Build(writer, image, BuildR8ui);
                    break;
                case BitmapTextureColor.R8i:
                    Build(writer, image, BuildR8i);
                    break;
                case BitmapTextureColor.R16f:
                    Build(writer, image, BuildR16f);
                    break;
                case BitmapTextureColor.R16ui:
                    Build(writer, image, BuildR16ui);
                    break;
                case BitmapTextureColor.R16i:
                    Build(writer, image, BuildR16i);
                    break;
                case BitmapTextureColor.R32f:
                    Build(writer, image, BuildR32f);
                    break;
                case BitmapTextureColor.R32ui:
                    Build(writer, image, BuildR32ui);
                    break;
                case BitmapTextureColor.R32i:
                    Build(writer, image, BuildR32i);
                    break;
                case BitmapTextureColor.Rg8ui:
                    Build(writer, image, BuildRg8ui);
                    break;
                case BitmapTextureColor.Rg8i:
                    Build(writer, image, BuildRg8i);
                    break;
                case BitmapTextureColor.Rg16f:
                    Build(writer, image, BuildRg16f);
                    break;
                case BitmapTextureColor.Rg16ui:
                    Build(writer, image, BuildRg16ui);
                    break;
                case BitmapTextureColor.Rg16i:
                    Build(writer, image, BuildRg16i);
                    break;
                case BitmapTextureColor.Rg32f:
                    Build(writer, image, BuildRg32f);
                    break;
                case BitmapTextureColor.Rg32ui:
                    Build(writer, image, BuildRg32ui);
                    break;
                case BitmapTextureColor.Rg32i:
                    Build(writer, image, BuildRg32i);
                    break;
                case BitmapTextureColor.Rgb8ui:
                    Build(writer, image, BuildRgb8ui);
                    break;
                case BitmapTextureColor.Rgb8i:
                    Build(writer, image, BuildRgb8i);
                    break;
                case BitmapTextureColor.Rgb16f:
                    Build(writer, image, BuildRgb16f);
                    break;
                case BitmapTextureColor.Rgb16ui:
                    Build(writer, image, BuildRgb16ui);
                    break;
                case BitmapTextureColor.Rgb16i:
                    Build(writer, image, BuildRgb16i);
                    break;
                case BitmapTextureColor.Rgb32f:
                    Build(writer, image, BuildRgb32f);
                    break;
                case BitmapTextureColor.Rgb32ui:
                    Build(writer, image, BuildRgb32ui);
                    break;
                case BitmapTextureColor.Rgb32i:
                    Build(writer, image, BuildRgb32i);
                    break;
                case BitmapTextureColor.Rgba8ui:
                    Build(writer, image, BuildRgba8ui);
                    break;
                case BitmapTextureColor.Rgba8i:
                    Build(writer, image, BuildRgba8i);
                    break;
                case BitmapTextureColor.Rgba16f:
                    Build(writer, image, BuildRgba16f);
                    break;
                case BitmapTextureColor.Rgba16ui:
                    Build(writer, image, BuildRgba16ui);
                    break;
                case BitmapTextureColor.Rgba16i:
                    Build(writer, image, BuildRgba16i);
                    break;
                case BitmapTextureColor.Rgba32f:
                    Build(writer, image, BuildRgba32f);
                    break;
                case BitmapTextureColor.Rgba32ui:
                    Build(writer, image, BuildRgba32ui);
                    break;
                case BitmapTextureColor.Rgba32i:
                    Build(writer, image, BuildRgba32i);
                    break;
                case BitmapTextureColor.Rgb5:
                    Build(writer, image, BuildRgb5);
                    break;
                case BitmapTextureColor.Rgb5A1:
                    Build(writer, image, BuildRgb5A1);
                    break;
                case BitmapTextureColor.Rgba4:
                    Build(writer, image, BuildRgba4);
                    break;
                case BitmapTextureColor.L8:
                    Build(writer, image, BuildL8);
                    break;
                case BitmapTextureColor.A8:
                    Build(writer, image, BuildA8);
                    break;
                case BitmapTextureColor.La8:
                    Build(writer, image, BuildLa8);
                    break;
                case BitmapTextureColor.R24:
                    Build(writer, image, BuildR24);
                    break;
                case BitmapTextureColor.R24G8:
                    Build(writer, image, BuildR24G8);
                    break;
                case BitmapTextureColor.R32fG8:
                    Build(writer, image, BuildR32fG8);
                    break;
                default:
                    throw new IOException();
            }
        }

        public static Bitmap Load(string path)
        {
            var image = new Bitmap(new MemoryStream(File.ReadAllBytes(path)));

            if (image.PixelFormat != GDIPixelFormat.Format32bppArgb)
            {
                var newImage = new Bitmap(image.Width, image.Height, GDIPixelFormat.Format32bppArgb);
                using (var g = System.Drawing.Graphics.FromImage(newImage))
                {
                    g.DrawImage(image, new GDIRectangle(0, 0, image.Width, image.Height));
                }
                image.Dispose();
                image = newImage;
            }
            return image;
        }

        public static Bitmap Compose31(Bitmap rgb, Bitmap a)
        {
            if (rgb.PixelFormat != GDIPixelFormat.Format32bppArgb || a.PixelFormat != GDIPixelFormat.Format32bppArgb || rgb.Width != a.Width || rgb.Height != a.Height)
            {
                throw new NotSupportedException();
            }

            var w = rgb.Width;
            var h = rgb.Height;

            var rgbData = rgb.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);
            var aData = a.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            var dest = new Bitmap(w, h, GDIPixelFormat.Format32bppArgb);
            var destData = dest.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.WriteOnly, GDIPixelFormat.Format32bppArgb);

            unsafe
            {
                var rgbptr = (byte*)rgbData.Scan0;
                var aptr = (byte*)aData.Scan0;
                var dptr = (byte*)destData.Scan0;

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        dptr[0] = rgbptr[0];
                        dptr[1] = rgbptr[1];
                        dptr[2] = rgbptr[2];
                        dptr[3] = aptr[2];
                        rgbptr += 4;
                        aptr += 4;
                        dptr += 4;
                    }
                }
            }
            rgb.UnlockBits(rgbData);
            a.UnlockBits(aData);
            dest.UnlockBits(destData);

            return dest;
        }

        public static Bitmap Compose1111(params Bitmap[] src)
        {
            if (src.Length <= 1 || src.Length > 4) throw new InvalidOperationException();
            if (src[0].PixelFormat != GDIPixelFormat.Format32bppArgb || src[1].PixelFormat != GDIPixelFormat.Format32bppArgb || src[0].Width != src[1].Width  || src[0].Height != src[1].Height) throw new NotSupportedException();
            if (src.Length >= 3 && (src[2].PixelFormat != GDIPixelFormat.Format32bppArgb || src[0].Width != src[2].Width || src[0].Height != src[2].Height)) throw new NotSupportedException();
            if (src.Length >= 4 && (src[3].PixelFormat != GDIPixelFormat.Format32bppArgb || src[0].Width != src[3].Width || src[0].Height != src[3].Height)) throw new NotSupportedException();

            var w = src[0].Width;
            var h = src[0].Height;

            var rData = src[0].LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);
            var gData = src[1].LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);
            var bData = src.Length >= 3 ? src[2].LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb) : null;
            var aData = src.Length == 4 ? src[3].LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb) : null;

            var dest = new Bitmap(w, h, GDIPixelFormat.Format32bppArgb);
            var destData = dest.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.WriteOnly, GDIPixelFormat.Format32bppArgb);

            unsafe
            {
                var rptr = (byte*)rData.Scan0;
                var gptr = (byte*)gData.Scan0;
                var bptr = bData != null ? (byte*)bData.Scan0 : null;
                var aptr = aData != null ? (byte*)aData.Scan0 : null;
                var dptr = (byte*)destData.Scan0;

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        dptr[2] = rptr[2];
                        dptr[1] = gptr[2];
                        dptr[0] = bptr != null ? bptr[2] : (byte)255;
                        dptr[3] = aptr != null ? aptr[2] : (byte)255;

                        rptr += 4;
                        gptr += 4;
                        if (bptr != null) bptr += 4;
                        if (aptr != null) aptr += 4;
                        dptr += 4;
                    }
                }
            }
            src[0].UnlockBits(rData);
            src[1].UnlockBits(gData);
            if (src.Length >= 3) src[2].UnlockBits(bData);
            if (src.Length >= 4) src[3].UnlockBits(aData);
            dest.UnlockBits(destData);

            return dest;
        }

        public static void Seperate31(Bitmap bitmap, out Bitmap rgb, out Bitmap a)
        {
            if (bitmap.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new InvalidOperationException();

            var w = bitmap.Width;
            var h = bitmap.Height;

            var srcData = bitmap.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            rgb = new Bitmap(w, h);
            a = new Bitmap(w, h);

            var rgbData = rgb.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.WriteOnly, GDIPixelFormat.Format32bppArgb);
            var aData = a.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.WriteOnly, GDIPixelFormat.Format32bppArgb);

            unsafe
            {
                var sptr = (byte*)srcData.Scan0;
                var rgbptr = (byte*)rgbData.Scan0;
                var aptr = (byte*)aData.Scan0;

                int rgbbpp = rgbData.Stride / w;
                int abpp = aData.Stride / w;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        rgbptr[0] = sptr[0];
                        rgbptr[1] = sptr[1];
                        rgbptr[2] = sptr[2];
                        rgbptr[3] = 255;
                        aptr[0] = aptr[1] = aptr[2] = sptr[3];
                        aptr[3] = 255;

                        rgbptr += 4;
                        aptr += 4;
                        sptr += 4;
                    }
                }
            }
            rgb.UnlockBits(rgbData);
            a.UnlockBits(aData);
            bitmap.UnlockBits(srcData);
        }

        public static void Seperate1111(Bitmap bitmap, Bitmap[] dest)
        {
            if (bitmap.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new InvalidOperationException();

            var w = bitmap.Width;
            var h = bitmap.Height;

            var srcData = bitmap.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            var destData = new BitmapData[4];
            for (var i = 0; i < dest.Length; i++)
            {
                dest[i] = new Bitmap(w, h);
                destData[i] = dest[i].LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.WriteOnly, GDIPixelFormat.Format32bppArgb);
            }

            unsafe
            {
                var sptr = (byte*)srcData.Scan0;
                var dptr0 = destData[0] != null ? (byte*)destData[0].Scan0 : null;
                var dptr1 = destData[1] != null ? (byte*)destData[1].Scan0 : null;
                var dptr2 = destData[2] != null ? (byte*)destData[2].Scan0 : null;
                var dptr3 = destData[3] != null ? (byte*)destData[3].Scan0 : null;

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        if (dptr0 != null)
                        {
                            dptr0[0] = dptr0[1] = dptr0[2] = sptr[2];
                            dptr0[3] = 255;
                            dptr0 += 4;
                        }
                        if (dptr1 != null)
                        {
                            dptr1[0] = dptr1[1] = dptr1[2] = sptr[1];
                            dptr1[3] = 255;
                            dptr1 += 4;
                        }
                        if (dptr2 != null)
                        {
                            dptr2[0] = dptr2[1] = dptr2[2] = sptr[0];
                            dptr2[3] = 255;
                            dptr2 += 4;
                        }
                        if (dptr3 != null)
                        {
                            dptr3[0] = dptr3[1] = dptr3[2] = sptr[3];
                            dptr3[3] = 255;
                            dptr3 += 4;
                        }
                        sptr += 4;
                    }
                }
            }
            for (var i = 0; i < dest.Length; i++) dest[i].UnlockBits(destData[i]);
            bitmap.UnlockBits(srcData);
        }

        public static bool HasOpacity(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new InvalidOperationException();

            var w = bitmap.Width;
            var h = bitmap.Height;

            var bitmapData = bitmap.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            var len = w * h * 4;

            var opacity = false;

            unsafe
            {
                var ptr = (byte*)bitmapData.Scan0;

                for (var i = 0; i < len; i += 4)
                {
                    if (ptr[i + 3] != 255)
                    {
                        opacity = true;
                        break;
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);

            return opacity;
        }

        public static Color4 Average(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != GDIPixelFormat.Format32bppArgb) throw new InvalidOperationException();

            var w = bitmap.Width;
            var h = bitmap.Height;

            var bitmapData = bitmap.LockBits(new GDIRectangle(0, 0, w, h), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

            var len = w * h * 4;

            long r = 0;
            long g = 0;
            long b = 0;
            long a = 0;

            unsafe
            {
                var ptr = (byte*)bitmapData.Scan0;

                for (var i = 0; i < len; i += 4)
                {
                    b += ptr[i + 0];
                    g += ptr[i + 1];
                    r += ptr[i + 2];
                    a += ptr[i + 3];
                }
            }
            bitmap.UnlockBits(bitmapData);

            len = w * h;

            r /= len;
            g /= len;
            b /= len;
            a /= len;

            return new Color4((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
