#define CDK_IMPL

#include "CSGraphicsContext.h"

#include "CSMacro.h"

static const CSRawFormatEncoding __encodings[] = {
	CSRawFormatEncoding(CSPixelFormatAlpha, CSPixelTypeUnsignedByte, 1),				//Alpha8,
	CSRawFormatEncoding(CSPixelFormatLuminance, CSPixelTypeUnsignedByte, 1),			//Luminance8,
	CSRawFormatEncoding(CSPixelFormatLuminanceAlpha, CSPixelTypeUnsignedByte, 2),		//Luminance8Alpha8,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeUnsignedByte, 1),					//R8,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeByte, 1),					//R8i,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeUnsignedByte, 1),			//R8ui,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeByte, 1),							//R8Snorm,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeUnsignedShort, 2),					//R16,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeHalfFloat, 2),						//R16f,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeShort, 2),					//R16i,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeUnsignedShort, 2),			//R16ui,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeShort, 2),							//R16Snorm,
	CSRawFormatEncoding(CSPixelFormatRed, CSPixelTypeFloat, 4),							//R32f,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeInt, 4),					//R32i,
	CSRawFormatEncoding(CSPixelFormatRedInteger, CSPixelTypeUnsignedInt, 4),			//R32ui,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeUnsignedByte, 2),					//Rg8,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeByte, 2),					//Rg8i,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeUnsignedByte, 2),			//Rg8ui,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeByte, 2),							//Rg8Snorm,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeUnsignedShort, 4),					//Rg16,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeHalfFloat, 4),						//Rg16f,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeShort, 4),					//Rg16i,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeUnsignedShort, 4),			//Rg16ui,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeShort, 4),							//Rg16Snorm,
	CSRawFormatEncoding(CSPixelFormatRg, CSPixelTypeFloat, 8),							//Rg32f,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeInt, 8),						//Rg32i,
	CSRawFormatEncoding(CSPixelFormatRgInteger, CSPixelTypeUnsignedInt, 8),				//Rg32ui,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeUnsignedShort565, 2),				//Rgb5,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeUnsignedByte, 3),					//Rgb8,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeByte, 3),					//Rgb8i,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeUnsignedByte, 3),			//Rgb8ui,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeByte, 3),							//Rgb8Snorm,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeUnsignedByte, 3),					//Srgb8,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeUnsignedShort, 6),					//Rgb16,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeHalfFloat, 6),						//Rgb16f,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeShort, 6),					//Rgb16i,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeUnsignedShort, 6),			//Rgb16ui,
	CSRawFormatEncoding(CSPixelFormatRgb, CSPixelTypeShort, 6),							//Rgb16Snorm,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeInt, 12),					//Rgb32i,
	CSRawFormatEncoding(CSPixelFormatRgbInteger, CSPixelTypeUnsignedInt, 12),			//Rgb32ui,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeUnsignedShort4444, 2),			//Rgba4,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeUnsignedShort5551, 2),			//Rgb5A1,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeUnsignedByte, 4),					//Rgba8,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeByte, 4),					//Rgba8i,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeUnsignedByte, 4),			//Rgba8ui,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeByte, 4),							//Rgba8Snorm,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeUnsignedByte, 4),					//Srgb8Alpha8,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeUnsignedShort, 8),				//Rgba16,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeHalfFloat, 8),					//Rgba16f,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeShort, 8),					//Rgba16i,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeUnsignedShort, 8),			//Rgba16ui,
	CSRawFormatEncoding(CSPixelFormatRgba, CSPixelTypeFloat, 16),						//Rgba32f,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeInt, 16),					//Rgba32i,
	CSRawFormatEncoding(CSPixelFormatRgbaInteger, CSPixelTypeUnsignedInt, 16),			//Rgba32ui,
	CSRawFormatEncoding(CSPixelFormatDepthComponent, CSPixelTypeUnsignedShort, 2),		//DepthComponent16,
	CSRawFormatEncoding(CSPixelFormatDepthComponent, CSPixelTypeUnsignedInt248, 3),		//DepthComponent24,
	CSRawFormatEncoding(CSPixelFormatDepthStencil, CSPixelTypeUnsignedInt248, 4),		//Depth24Stencil8,
	CSRawFormatEncoding(CSPixelFormatDepthComponent, CSPixelTypeFloat, 4),				//DepthComponent32f,
	CSRawFormatEncoding(CSPixelFormatDepthStencil, CSPixelTypeFloat32UnsignedInt248Rev, 4),	//Depth32fStencil8,
	CSRawFormatEncoding(4, 8),				//CompressedRgbaS3tcDxt1Ext,
	CSRawFormatEncoding(4, 16),				//CompressedRgbaS3tcDxt3Ext,
	CSRawFormatEncoding(4, 16),				//CompressedRgbaS3tcDxt5Ext,
	CSRawFormatEncoding(4, 8),				//CompressedSrgbAlphaS3tcDxt1Ext,
	CSRawFormatEncoding(4, 16),				//CompressedSrgbAlphaS3tcDxt3Ext,
	CSRawFormatEncoding(4, 16),				//CompressedSrgbAlphaS3tcDxt5Ext,
	CSRawFormatEncoding(4, 8),				//CompressedRgb8Etc2,
	CSRawFormatEncoding(4, 8),				//CompressedSrgb8Etc2,
	CSRawFormatEncoding(4, 8),				//CompressedRgb8PunchthroughAlpha1Etc2,    
	CSRawFormatEncoding(4, 8),				//CompressedSrgb8PunchthroughAlpha1Etc2,
	CSRawFormatEncoding(4, 16),				//CompressedRgba8Etc2Eac,
	CSRawFormatEncoding(4, 16),				//CompressedSrgb8Alpha8Etc2Eac,
	CSRawFormatEncoding(4, 16),				//CompressedRgbaAstc4x4,
	CSRawFormatEncoding(5, 16),				//CompressedRgbaAstc5x5,
	CSRawFormatEncoding(6, 16),				//CompressedRgbaAstc6x6,
	CSRawFormatEncoding(8, 16),				//CompressedRgbaAstc8x8,
	CSRawFormatEncoding(10, 16),			//CompressedRgbaAstc10x10,
	CSRawFormatEncoding(12, 16),			//CompressedRgbaAstc12x12,
	CSRawFormatEncoding(4, 16),				//CompressedSrgbaAstc4x4,
	CSRawFormatEncoding(5, 16),				//CompressedSrgbaAstc5x5,
	CSRawFormatEncoding(6, 16),				//CompressedSrgbaAstc6x6,
	CSRawFormatEncoding(8, 16),				//CompressedSrgbaAstc8x8,
	CSRawFormatEncoding(10, 16),			//CompressedSrgbaAstc10x10,
	CSRawFormatEncoding(12, 16)				//CompressedSrgbaAstc12x12
};


CSRawFormatEncoding::CSRawFormatEncoding(CSPixelFormat pixelFormat, CSPixelType pixelType, int pixelBpp) : 
	pixelFormat(pixelFormat), 
	pixelType(pixelType), 
	pixelBpp(pixelBpp), 
	compressBlock(0), 
	compressBlockLength(0),  
	compressed(false) 
{

}

CSRawFormatEncoding::CSRawFormatEncoding(int compressBlock, int compressBlockLength) :
	pixelFormat(CSPixelFormatNone),
	pixelType(CSPixelTypeNone),
	pixelBpp(0),
	compressBlock(compressBlock),
	compressBlockLength(compressBlockLength),
	compressed(true)
{

}

const CSRawFormatEncoding& CSRawFormat::encoding() const {
	CSAssert(_value > 0 && _value <= countof(__encodings));
	return __encodings[_value - 1];
}

bool CSRawFormat::isSupported() const {
	return CSGraphicsContext::sharedContext()->isSupportRawFormat(*this);
}
