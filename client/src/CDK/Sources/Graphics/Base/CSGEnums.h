#ifndef __CDK__CSGEnums__
#define __CDK__CSGEnums__

#include "CSTypes.h"

enum CSPolygonMode : byte {
    CSPolygonFill = 0,
    CSPolygonLineFront = 1 << 2,
    CSPolygonPointFront = 2 << 2,
    CSPolygonLineBack = 1,
    CSPolygonPointBack = 2,
    CSPolygonLine = CSPolygonLineFront | CSPolygonLineBack,
    CSPolygonPoint = CSPolygonPointFront | CSPolygonPointBack
};

enum CSStrokeMode : byte {
    CSStrokeNormal,
    CSStrokeLighten
};

enum CSCullMode : byte {
    CSCullNone,
    CSCullBack,
    CSCullFront
};

enum CSDepthMode : byte {
    CSDepthNone,
    CSDepthRead,
    CSDepthWrite,
    CSDepthReadWrite
};

enum CSStencilMode : byte {
    CSStencilNone,
    CSStencilInclusive,
    CSStencilExclusive
};

enum CSBlendMode : byte {
    CSBlendNone,
    CSBlendAlpha,
    CSBlendAdd,
    CSBlendSubstract,
    CSBlendMultiply,
    CSBlendScreen
};

enum CSLightMode : byte {
    CSLightNone,
    CSLightPhong,
    CSLightBlinn,
    CSLightCookBlinn,
    CSLightCookBeckmann,
    CSLightCookGGX
};

enum CSCorner : byte {
    CSCornerLeftTop = 1,
    CSCornerRightTop = 2,
    CSCornerLeftBottom = 4,
    CSCornerRightBottom = 8,
    CSCornerAll = 15
};

enum CSAlign : byte {
    CSAlignNone = 0,
    CSAlignCenter = 1,
    CSAlignRight = 2,
    CSAlignMiddle = 4,
    CSAlignBottom = 8,
    CSAlignCenterMiddle = CSAlignCenter | CSAlignMiddle,
    CSAlignCenterBottom = CSAlignCenter | CSAlignBottom,
    CSAlignRightMiddle = CSAlignRight | CSAlignMiddle,
    CSAlignRightBottom = CSAlignRight | CSAlignBottom
};

enum CSInstanceLayer : byte {
    CSInstanceLayerNone,
    CSInstanceLayerShadow,
    CSInstanceLayerShadow2D,
    CSInstanceLayerBase,
    CSInstanceLayerBlendBottom,
    CSInstanceLayerBlendMiddle,
    CSInstanceLayerBlendTop,
    CSInstanceLayerDistortion
};

enum CSInstanceBlendLayer : byte {
    CSInstanceBlendLayerBottom = CSInstanceLayerBlendBottom,
    CSInstanceBlendLayerMiddle = CSInstanceLayerBlendMiddle,
    CSInstanceBlendLayerTop = CSInstanceLayerBlendTop
};

enum CSPrimitiveMode : byte {
    CSPrimitivePoints,
    CSPrimitiveLines,
    CSPrimitiveLineLoop,
    CSPrimitiveLineStrip,
    CSPrimitiveTriangles,
    CSPrimitiveTriangleStrip,
    CSPrimitiveTriangleFan
};

enum CSVertexAttribType : byte {
    CSVertexAttribTypeByte,
    CSVertexAttribTypeUnsignedByte,
    CSVertexAttribTypeShort,
    CSVertexAttribTypeUnsignedShort,
    CSVertexAttribTypeInt,
    CSVertexAttribTypeUnsignedInt,
    CSVertexAttribTypeFloat,
    CSVertexAttribTypeDouble,
    CSVertexAttribTypeHalfFloat
};

enum CSGBufferTarget : byte {
    CSGBufferTargetArray,
    CSGBufferTargetElementArray,
    CSGBufferTargetPixelPack,
    CSGBufferTargetPixelUnpack,
    CSGBufferTargetUniform,
    CSGBufferTargetTexture,
    CSGBufferTargetTransformFeedback,
    CSGBufferTargetCopyRead,
    CSGBufferTargetCopyWrite,
    CSGBufferTargetDrawIndirect,
    CSGBufferTargetShaderStorage,
    CSGBufferTargetDispatchIndirect,
    CSGBufferTargetQuery,
    CSGBufferTargetAtomicCounter
};

enum CSGBufferUsageHint : byte {
    CSGBufferUsageHintStreamDraw,
    CSGBufferUsageHintStreamRead,
    CSGBufferUsageHintStreamCopy,
    CSGBufferUsageHintStaticDraw,
    CSGBufferUsageHintStaticRead,
    CSGBufferUsageHintStaticCopy,
    CSGBufferUsageHintDynamicDraw,
    CSGBufferUsageHintDynamicRead,
    CSGBufferUsageHintDynamicCopy
};

enum CSDrawElementsType : byte {
    CSDrawElementsTypeUnsignedByte,
    CSDrawElementsTypeUnsignedShort,
    CSDrawElementsTypeUnsignedInt
};

enum CSShaderType : byte {
    CSShaderTypeFragment,
    CSShaderTypeVertex,
    CSShaderTypeGeometry,
    CSShaderTypeTessEvaluation,
    CSShaderTypeTessControl,
    CSShaderTypeCompute
};

enum CSFramebufferAttachment : byte {
    CSFramebufferAttachmentColor0,
    CSFramebufferAttachmentColor1,
    CSFramebufferAttachmentColor2,
    CSFramebufferAttachmentColor3,
    CSFramebufferAttachmentColor4,
    CSFramebufferAttachmentColor5,
    CSFramebufferAttachmentColor6,
    CSFramebufferAttachmentColor7,
    CSFramebufferAttachmentDepthStencil,
    CSFramebufferAttachmentDepth,
    CSFramebufferAttachmentStencil
};

enum CSFramebufferTarget : byte {
    CSFramebufferTargetReadFramebuffer,
    CSFramebufferTargetDrawFramebuffer,
    CSFramebufferTargetFramebuffer
};

enum CSClearBuffer {
    CSClearBufferDepth = 0x0100,
    CSClearBufferAccum = 0x0200,
    CSClearBufferStencil = 0x0400,
    CSClearBufferColor = 0x4000
};
typedef int CSClearBufferMask;

enum CSBlitFramebufferFilter : byte {
    CSBlitFramebufferFilterNearest,
    CSBlitFramebufferFilterLinear
};

enum CSTextureTarget : byte {
    CSTextureTarget2D,
    CSTextureTarget3D,
    CSTextureTargetCubeMap,
    CSTextureTargetCubeMapPositiveX,
    CSTextureTargetCubeMapNegativeX,
    CSTextureTargetCubeMapPositiveY,
    CSTextureTargetCubeMapNegativeY,
    CSTextureTargetCubeMapPositiveZ,
    CSTextureTargetCubeMapNegativeZ,
    CSTextureTarget2DMultisample
};

enum CSPixelFormat : byte {
    CSPixelFormatNone,
    CSPixelFormatRed,
    CSPixelFormatGreen,
    CSPixelFormatBlue,
    CSPixelFormatAlpha,
    CSPixelFormatRgb,
    CSPixelFormatRgba,
    CSPixelFormatLuminance,
    CSPixelFormatLuminanceAlpha,
    CSPixelFormatRg,
    CSPixelFormatRgInteger,
    CSPixelFormatDepthComponent,
    CSPixelFormatDepthStencil,
    CSPixelFormatRedInteger,
    CSPixelFormatGreenInteger,
    CSPixelFormatBlueInteger,
    CSPixelFormatAlphaInteger,
    CSPixelFormatRgbInteger,
    CSPixelFormatRgbaInteger
};

enum CSPixelType : byte {
    CSPixelTypeNone,
    CSPixelTypeByte,
    CSPixelTypeUnsignedByte,
    CSPixelTypeShort,
    CSPixelTypeUnsignedShort,
    CSPixelTypeInt,
    CSPixelTypeUnsignedInt,
    CSPixelTypeFloat,
    CSPixelTypeHalfFloat,
    CSPixelTypeUnsignedShort4444,
    CSPixelTypeUnsignedShort5551,
    CSPixelTypeUnsignedInt8888,
    CSPixelTypeUnsignedShort565,
    CSPixelTypeUnsignedInt248,
    CSPixelTypeFloat32UnsignedInt248Rev
};

enum CSTextureWrapMode : byte {
    CSTextureWrapRepeat,
    CSTextureWrapClampToBorder,
    CSTextureWrapClampToEdge,
    CSTextureWrapMirroredRepeat
};

enum CSTextureMinFilter : byte {
    CSTextureMinFilterNearest,
    CSTextureMinFilterLinear,
    CSTextureMinFilterNearestMipmapNearest,
    CSTextureMinFilterLinearMipmapNearest,
    CSTextureMinFilterNearestMipmapLinear,
    CSTextureMinFilterLinearMipmapLinear,
};

enum CSTextureMagFilter : byte {
    CSTextureMagFilterNearest,
    CSTextureMagFilterLinear
};

enum CSTextureAccess : byte {
    CSTextureAccessReadOnly,
    CSTextureAccessWriteOnly,
    CSTextureAccessReadWrite
};

struct CSRawFormatEncoding {
public:
    CSPixelFormat pixelFormat;
    CSPixelType pixelType;
    byte pixelBpp;
    byte compressBlock;
    byte compressBlockLength;
    bool compressed;

    CSRawFormatEncoding() = delete;
#ifdef CDK_IMPL
    CSRawFormatEncoding(CSPixelFormat pixelFormat, CSPixelType pixelType, int pixelBpp);
    CSRawFormatEncoding(int compressBlock, int compressBlockLength);
#endif
};

class CSRawFormat {
public:
    enum Value : byte {
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
    };
private:
    Value _value;
public:
    CSRawFormat() = default;
    constexpr CSRawFormat(Value value) : _value(value) {}
    constexpr CSRawFormat(int value) : _value((Value)value) {}
    constexpr operator Value() const { return _value; }
    explicit operator bool() const = delete;

    const CSRawFormatEncoding& encoding() const;
    bool isSupported() const;

    inline uint hash() const {
        return std::hash<byte>()(_value);
    }
};

#endif
