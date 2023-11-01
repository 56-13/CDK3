namespace CDK.Drawing
{
    public enum RawFormat
    {
        None,
        Alpha8,
        Luminance8,
        Luminance8Alpha8,
        R8,
        R8i,
        R8ui,
        R8Snorm,
        R16,
        R16f,
        R16i,
        R16ui,
        R16Snorm,
        R32f,
        R32i,
        R32ui,
        Rg8,
        Rg8i,
        Rg8ui,
        Rg8Snorm,
        Rg16,
        Rg16f,
        Rg16i,
        Rg16ui,
        Rg16Snorm,
        Rg32f,
        Rg32i,
        Rg32ui,
        Rgb5,
        Rgb8,
        Rgb8i,
        Rgb8ui,
        Rgb8Snorm,
        Srgb8,
        Rgb16,
        Rgb16f,
        Rgb16i,
        Rgb16ui,
        Rgb16Snorm,
        Rgb32i,
        Rgb32ui,
        Rgba4,
        Rgb5A1,
        Rgba8,
        Rgba8i,
        Rgba8ui,
        Rgba8Snorm,
        Srgb8Alpha8,
        Rgba16,
        Rgba16f,
        Rgba16i,
        Rgba16ui,
        Rgba32f,
        Rgba32i,
        Rgba32ui,
        DepthComponent16,
        DepthComponent24,
        Depth24Stencil8,
        DepthComponent32f,
        Depth32fStencil8,
        CompressedRgbaS3tcDxt1Ext,
        CompressedRgbaS3tcDxt3Ext,
        CompressedRgbaS3tcDxt5Ext,
        CompressedSrgbAlphaS3tcDxt1Ext,
        CompressedSrgbAlphaS3tcDxt3Ext,
        CompressedSrgbAlphaS3tcDxt5Ext,
        CompressedRgb8Etc2,
        CompressedSrgb8Etc2,
        CompressedRgb8PunchthroughAlpha1Etc2,
        CompressedSrgb8PunchthroughAlpha1Etc2,
        CompressedRgba8Etc2Eac,
        CompressedSrgb8Alpha8Etc2Eac,
        CompressedRgbaAstc4x4,
        CompressedRgbaAstc5x5,
        CompressedRgbaAstc6x6,
        CompressedRgbaAstc8x8,
        CompressedRgbaAstc10x10,
        CompressedRgbaAstc12x12,
        CompressedSrgbaAstc4x4,
        CompressedSrgbaAstc5x5,
        CompressedSrgbaAstc6x6,
        CompressedSrgbaAstc8x8,
        CompressedSrgbaAstc10x10,
        CompressedSrgbaAstc12x12
    }

    public class RawFormatEncoding
    {
        public PixelFormat PixelFormat { private set; get; }
        public PixelType PixelType { private set; get; }
        public int PixelBpp { private set; get; }
        public int CompressBlock { private set; get; }
        public int CompressBlockLength { private set; get; }
        public string CompressPvrFormat { private set; get; }
        public bool Compressed { private set; get; }
        public BitmapTextureResizing BitmapResizing { private set; get; }
        public BitmapTextureColor BitmapColor { private set; get; }

        internal RawFormatEncoding(PixelFormat pixelFormat, PixelType pixelType, int pixelBpp, BitmapTextureColor bitmapColor = BitmapTextureColor.None)
        {
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            PixelBpp = pixelBpp;
            BitmapColor = bitmapColor;
        }

        internal RawFormatEncoding(int compressBlock, int compressBlockLength, string compressPvrFormat, BitmapTextureResizing bitmapResizing = BitmapTextureResizing.None, BitmapTextureColor bitmapColor = BitmapTextureColor.None)
        {
            CompressBlock = compressBlock;
            CompressBlockLength = compressBlockLength;
            CompressPvrFormat = compressPvrFormat;
            Compressed = true;

            BitmapResizing = bitmapResizing;
            BitmapColor = bitmapColor;
        }
    }

    public static class RawFormats
    {
        private static RawFormatEncoding[] _encodings =
        {
            null,   //None,
            new RawFormatEncoding(PixelFormat.Alpha, PixelType.UnsignedByte, 1, BitmapTextureColor.A8),	                //Alpha8,
	        new RawFormatEncoding(PixelFormat.Luminance, PixelType.UnsignedByte, 1, BitmapTextureColor.L8),	            //Luminance8,
            new RawFormatEncoding(PixelFormat.LuminanceAlpha, PixelType.UnsignedByte, 2, BitmapTextureColor.La8),	    //Luminance8Alpha8,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.UnsignedByte, 1, BitmapTextureColor.R8ui),	                //R8,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.Byte, 1, BitmapTextureColor.R8i),	                //R8i,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.UnsignedByte, 1, BitmapTextureColor.R8ui),	        //R8ui,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.Byte, 1, BitmapTextureColor.R8i),	                        //R8Snorm,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.UnsignedShort, 2, BitmapTextureColor.R16ui),	            //R16,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.HalfFloat, 2, BitmapTextureColor.R16f),	                //R16f,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.Short, 2, BitmapTextureColor.R16i),	                //R16i,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.UnsignedShort, 2, BitmapTextureColor.R16ui),	    //R16ui,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.Short, 2, BitmapTextureColor.R16i),	                    //R16Snorm,
	        new RawFormatEncoding(PixelFormat.Red, PixelType.Float, 4, BitmapTextureColor.R32f),	                    //R32f,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.Int, 4, BitmapTextureColor.R32i),	                //R32i,
	        new RawFormatEncoding(PixelFormat.RedInteger, PixelType.UnsignedInt, 4, BitmapTextureColor.R32ui),	        //R32ui,
            new RawFormatEncoding(PixelFormat.Rg, PixelType.UnsignedByte, 2, BitmapTextureColor.Rg8ui),	                //Rg8,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.Byte, 2, BitmapTextureColor.Rg8i),	                //Rg8i,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.UnsignedByte, 2, BitmapTextureColor.Rg8ui),	        //Rg8ui,
	        new RawFormatEncoding(PixelFormat.Rg, PixelType.Byte, 2, BitmapTextureColor.Rg8i),	                        //Rg8Snorm,
	        new RawFormatEncoding(PixelFormat.Rg, PixelType.UnsignedShort, 4, BitmapTextureColor.Rg16ui),	            //Rg16,
	        new RawFormatEncoding(PixelFormat.Rg, PixelType.HalfFloat, 4, BitmapTextureColor.Rg16f),	                //Rg16f,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.Short, 4, BitmapTextureColor.Rg16i),	                //Rg16i,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.UnsignedShort, 4, BitmapTextureColor.Rg16ui),	    //Rg16ui,
	        new RawFormatEncoding(PixelFormat.Rg, PixelType.Short, 4, BitmapTextureColor.Rg16i),	                    //Rg16Snorm,
	        new RawFormatEncoding(PixelFormat.Rg, PixelType.Float, 8, BitmapTextureColor.Rg32f),	                    //Rg32f,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.Int, 8, BitmapTextureColor.Rg32i),	                //Rg32i,
	        new RawFormatEncoding(PixelFormat.RgInteger, PixelType.UnsignedInt, 8, BitmapTextureColor.Rg32ui),	        //Rg32ui,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.UnsignedShort565, 2, BitmapTextureColor.Rgb5),             //Rgb5,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.UnsignedByte, 3, BitmapTextureColor.Rgb8ui),               //Rgb8,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.Byte, 3, BitmapTextureColor.Rgb8i),                 //Rgb8i,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.UnsignedByte, 3, BitmapTextureColor.Rgb8ui),        //Rgb8ui,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.Byte, 3, BitmapTextureColor.Rgb8i),                        //Rgb8Snorm,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.UnsignedByte, 3, BitmapTextureColor.Rgb8ui),               //Srgb8,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.UnsignedShort, 6, BitmapTextureColor.Rgb8i),               //Rgb16,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.HalfFloat, 6, BitmapTextureColor.Rgb16f),                  //Rgb16f,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.Short, 6, BitmapTextureColor.Rgb16i),               //Rgb16i,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.UnsignedShort, 6, BitmapTextureColor.Rgb16ui),      //Rgb16ui,
	        new RawFormatEncoding(PixelFormat.Rgb, PixelType.Short, 6, BitmapTextureColor.Rgb16i),                      //Rgb16Snorm,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.Int, 12, BitmapTextureColor.Rgb32i),                //Rgb32i,
	        new RawFormatEncoding(PixelFormat.RgbInteger, PixelType.UnsignedInt, 12, BitmapTextureColor.Rgb32ui),       //Rgb32ui,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.UnsignedShort4444, 2, BitmapTextureColor.Rgba4),          //Rgba4,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.UnsignedShort5551, 2, BitmapTextureColor.Rgb5A1),         //Rgb5A1,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.UnsignedByte, 4, BitmapTextureColor.Rgba8ui),             //Rgba8,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.Byte, 4, BitmapTextureColor.Rgba8i),               //Rgba8i,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.UnsignedByte, 4, BitmapTextureColor.Rgba8ui),      //Rgba8ui,
            new RawFormatEncoding(PixelFormat.Rgba, PixelType.Byte, 4, BitmapTextureColor.Rgba8i),                      //Rgba8Snorm,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.UnsignedByte, 4, BitmapTextureColor.Rgba8ui),             //Srgb8Alpha8,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.UnsignedShort, 8, BitmapTextureColor.Rgba16ui),           //Rgba16,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.HalfFloat, 8, BitmapTextureColor.Rgba16f),                //Rgba16f,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.Short, 8, BitmapTextureColor.Rgba16i),             //Rgba16i,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.UnsignedShort, 8, BitmapTextureColor.Rgba16ui),    //Rgba16ui,
	        new RawFormatEncoding(PixelFormat.Rgba, PixelType.Float, 16, BitmapTextureColor.Rgba32f),                   //Rgba32f,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.Int, 16, BitmapTextureColor.Rgba32i),              //Rgba32i,
	        new RawFormatEncoding(PixelFormat.RgbaInteger, PixelType.UnsignedInt, 16, BitmapTextureColor.Rgba32ui),     //Rgba32ui,
	        new RawFormatEncoding(PixelFormat.DepthComponent, PixelType.UnsignedShort, 2, BitmapTextureColor.R16ui),    //DepthComponent16,
	        new RawFormatEncoding(PixelFormat.DepthComponent, PixelType.UnsignedInt248, 3, BitmapTextureColor.R24G8),   //DepthComponent24,
            new RawFormatEncoding(PixelFormat.DepthStencil, PixelType.UnsignedInt248, 4, BitmapTextureColor.R24G8),     //Depth24Stencil8,
	        new RawFormatEncoding(PixelFormat.DepthComponent, PixelType.Float, 4, BitmapTextureColor.R32f),             //DepthComponent32f,
	        new RawFormatEncoding(PixelFormat.DepthStencil, PixelType.Float32UnsignedInt248Rev, 4, BitmapTextureColor.R32f),   //Depth32fStencil8,
	        new RawFormatEncoding(4, 8, "BC1", BitmapTextureResizing.X4),           //CompressedRgbaS3tcDxt1Ext,
	        new RawFormatEncoding(4, 16, "BC2", BitmapTextureResizing.X4),          //CompressedRgbaS3tcDxt3Ext,
	        new RawFormatEncoding(4, 16, "BC3", BitmapTextureResizing.X4),          //CompressedRgbaS3tcDxt5Ext,
	        new RawFormatEncoding(4, 8, "BC1,UBN,sRGB", BitmapTextureResizing.X4),  //CompressedSrgbAlphaS3tcDxt1Ext,
	        new RawFormatEncoding(4, 16, "BC2,UBN,sRGB", BitmapTextureResizing.X4), //CompressedSrgbAlphaS3tcDxt3Ext,
	        new RawFormatEncoding(4, 16, "BC3,UBN,sRGB", BitmapTextureResizing.X4), //CompressedSrgbAlphaS3tcDxt5Ext,
            new RawFormatEncoding(4, 8, "ETC2_RGB"),                                //CompressedRgb8Etc2,
            new RawFormatEncoding(4, 8, "ETC2_RGB,UBN,sRGB"),                       //CompressedSrgb8Etc2,
            new RawFormatEncoding(4, 8, "ETC2_RGB_A1"),                             //CompressedRgb8PunchthroughAlpha1Etc2,    
            new RawFormatEncoding(4, 8, "ETC2_RGB_A1,UBN,sRGB"),                    //CompressedSrgb8PunchthroughAlpha1Etc2,
            new RawFormatEncoding(4, 16, "ETC2_RGBA"),                              //CompressedRgba8Etc2Eac,
            new RawFormatEncoding(4, 16, "ETC2_RGBA,UBN,sRGB"),                     //CompressedSrgb8Alpha8Etc2Eac,
            new RawFormatEncoding(4, 16, "ASTC_4x4"),                               //CompressedRgbaAstc4x4,
            new RawFormatEncoding(5, 16, "ASTC_5x5"),                               //CompressedRgbaAstc5x5,
            new RawFormatEncoding(6, 16, "ASTC_6x6"),                               //CompressedRgbaAstc6x6,
            new RawFormatEncoding(8, 16, "ASTC_8x8"),                               //CompressedRgbaAstc8x8,
            new RawFormatEncoding(10, 16, "ASTC_10x10"),                            //CompressedRgbaAstc10x10,
            new RawFormatEncoding(12, 16, "ASTC_12x12"),                            //CompressedRgbaAstc12x12,
            new RawFormatEncoding(4, 16, "ASTC_4x4,UBN,sRGB"),                      //CompressedSrgbaAstc4x4,
            new RawFormatEncoding(5, 16, "ASTC_5x5,UBN,sRGB"),                      //CompressedSrgbaAstc5x5,
            new RawFormatEncoding(6, 16, "ASTC_6x6,UBN,sRGB"),                      //CompressedSrgbaAstc6x6,
            new RawFormatEncoding(8, 16, "ASTC_8x8,UBN,sRGB"),                      //CompressedSrgbaAstc8x8,
            new RawFormatEncoding(10, 16, "ASTC_10x10,UBN,sRGB"),                   //CompressedSrgbaAstc10x10,
            new RawFormatEncoding(12, 16, "ASTC_12x12,UBN,sRGB")                    //CompressedSrgbaAstc12x12
        };

        public static RawFormatEncoding GetEncoding(this RawFormat format) => _encodings[(int)format];
        public static bool IsSupported(this RawFormat format) => GraphicsContext.Instance.IsSupportRawFormat(format);
    }
}
