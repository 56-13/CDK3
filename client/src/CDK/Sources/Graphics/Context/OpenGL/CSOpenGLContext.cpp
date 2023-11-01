#define CDK_IMPL

#include "CSOpenGLContext.h"

#include "CSThread.h"
#include "CSDiagnostics.h"
#include "CSGraphics.h"

static const char* GLInternalFormatSupportExtensions[] = {
    NULL,   //None,
    NULL,   //Alpha8,
    NULL,   //Luminance8,
    NULL,   //Luminance8Alpha8,
    NULL,   //R8,
    NULL,   //R8i,
    NULL,   //R8ui,
    NULL,   //R8Snorm,
    NULL,   //R16,
    NULL,   //R16f,
    NULL,   //R16i,
    NULL,   //R16ui,
    NULL,   //R16Snorm,
    NULL,   //R32f,
    NULL,   //R32i,
    NULL,   //R32ui,
    NULL,   //Rg8,
    NULL,   //Rg8i,
    NULL,   //Rg8ui,
    NULL,   //Rg8Snorm,
    NULL,   //Rg16,
    NULL,   //Rg16f,
    NULL,   //Rg16i,
    NULL,   //Rg16ui,
    NULL,   //Rg16Snorm,
    NULL,   //Rg32f,
    NULL,   //Rg32i,
    NULL,   //Rg32ui,
    NULL,   //Rgb5,
    NULL,   //Rgb8,
    NULL,   //Rgb8i,
    NULL,   //Rgb8ui,
    NULL,   //Rgb8Snorm,
    NULL,   //Srgb8,
    NULL,   //Rgb16,
    NULL,   //Rgb16f,
    NULL,   //Rgb16i,
    NULL,   //Rgb16ui,
    NULL,   //Rgb16Snorm,
    NULL,   //Rgb32i,
    NULL,   //Rgb32ui,
    NULL,   //Rgba4,
    NULL,   //Rgb5A1,
    NULL,   //Rgba8,
    NULL,   //Rgba8i,
    NULL,   //Rgba8ui,
    NULL,   //Rgba8Snorm,
    NULL,   //Srgb8Alpha8,
    NULL,   //Rgba16,
    NULL,   //Rgba16f,
    NULL,   //Rgba16i,
    NULL,   //Rgba16ui,
    NULL,   //Rgba32f,
    NULL,   //Rgba32i,
    NULL,   //Rgba32ui,
    NULL,   //DepthComponent16,
    NULL,   //DepthComponent24,
    NULL,   //Depth24Stencil8,
    NULL,   //DepthComponent32f,
    NULL,   //Depth32fStencil8,
    "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt1Ext,
    "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt3Ext,
    "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt5Ext,
    "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt1Ext,
    "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt3Ext,
    "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt5Ext,
    "GL_OES_compressed_ETC2_RGB8_texture",                      //CompressedRgb8Etc2,
    "GL_OES_compressed_ETC2_sRGB8_texture",                     //CompressedSrgb8Etc2,
    "GL_OES_compressed_ETC2_punchthroughA_RGBA8_texture",       //CompressedRgb8PunchthroughAlpha1Etc2,
    "GL_OES_compressed_ETC2_punchthroughA_sRGB8_alpha_texture", //CompressedSrgb8PunchthroughAlpha1Etc2,
    "GL_OES_compressed_ETC2_RGBA8_texture",                     //CompressedRgba8Etc2Eac,
    "GL_OES_compressed_ETC2_sRGB8_alpha8_texture",              //CompressedSrgb8Alpha8Etc2Eac,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc4x4,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc5x5,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc6x6,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc8x8,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc10x10,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc12x12,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc4x4,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc5x5,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc6x6,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc8x8,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc10x10,
    "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc12x12
};

CSOpenGLContext::CSOpenGLContext() {
    CSThread::renderThread()->run<void>([this]() {
        glEnable(GL_TEXTURE_CUBE_MAP_SEAMLESS);
        glPixelStorei(GL_PACK_ALIGNMENT, 1);
        glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
        glGetIntegerv(GL_MAX_UNIFORM_BLOCK_SIZE, &_maxUniformBlockSize);
        _extensions = (const char*)glGetString(GL_EXTENSIONS);
    });
}

void CSOpenGLContext::clearTargets(CSRenderTarget* target) {
    if (_api.currentTarget() == target) _api.setCurrentTarget(NULL);
}

bool CSOpenGLContext::isSupportRawFormat(CSRawFormat format) const {
    const char* ext = GLInternalFormatSupportExtensions[(int)format];
    return !ext || strstr(_extensions, ext) != NULL;
}

bool CSOpenGLContext::isRenderThread(CSGraphicsApi** api) {
    if (CSThread::renderThread()->isActive()) {
        if (api) *api = &_api;
        return true;
    }
    if (api) *api = NULL;
    return false;
}

CSGraphicsApi* CSOpenGLContext::attachRenderThread() {
    CSErrorLog("unsupported operation");
    abort();
    return NULL;
}

void CSOpenGLContext::detachRenderThread() {
    CSErrorLog("unsupported operation");
    abort();
}

CSDelegateRenderCommand* CSOpenGLContext::invoke(bool gsync, const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0, const CSObject* retain1, const CSObject* retain2) {
    if (CSThread::renderThread()->isActive()) {
        CSDiagnostics::renderInvoke(1);
        inv(&_api);
        return NULL;
    }
    if (gsync) {
        CSGraphics* graphics = currentGraphics();
        if (graphics) return graphics->command(inv, retain0, retain1, retain2);
    }

    CSThread::renderThread()->run<void>([this, inv, retain0 = CSPtr<const CSObject>(retain0), retain1 = CSPtr<const CSObject>(retain1), retain2 = CSPtr<const CSObject>(retain2)]() {
        CSDiagnostics::renderInvoke(1);
        inv(&_api);
    });
    return NULL;
}
