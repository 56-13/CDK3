#define CDK_IMPL

#include "CSOpenGLApi.h"

#include "CSDiagnostics.h"

#include "CSVertexArrays.h"
#include "CSShaders.h"

static const GLenum GLPrimitiveTypes[] = {
    GL_POINTS,
    GL_LINES,
    GL_LINE_LOOP,
    GL_LINE_STRIP,
    GL_TRIANGLES,
    GL_TRIANGLE_STRIP,
    GL_TRIANGLE_FAN
};

static const GLenum GLVertexAttribTypes[] = {
    GL_BYTE,
    GL_UNSIGNED_BYTE,
    GL_SHORT,
    GL_UNSIGNED_SHORT,
    GL_INT,
    GL_UNSIGNED_INT,
    GL_FLOAT,
    GL_DOUBLE,
    GL_HALF_FLOAT
};

static const GLenum GLBufferTargets[] = {
    GL_ARRAY_BUFFER,
    GL_ELEMENT_ARRAY_BUFFER,
    GL_PIXEL_PACK_BUFFER,
    GL_PIXEL_UNPACK_BUFFER,
    GL_UNIFORM_BUFFER,
    GL_TEXTURE_BUFFER,
    GL_TRANSFORM_FEEDBACK_BUFFER,
    GL_COPY_READ_BUFFER,
    GL_COPY_WRITE_BUFFER,
    GL_DRAW_INDIRECT_BUFFER,
    GL_SHADER_STORAGE_BUFFER,
    GL_DISPATCH_INDIRECT_BUFFER,
    GL_QUERY_BUFFER,
    GL_ATOMIC_COUNTER_BUFFER
};

static const GLenum GLBufferUsageHints[] = {
    GL_STREAM_DRAW,
    GL_STREAM_READ,
    GL_STREAM_COPY,
    GL_STATIC_DRAW,
    GL_STATIC_READ,
    GL_STATIC_COPY,
    GL_DYNAMIC_DRAW,
    GL_DYNAMIC_READ,
    GL_DYNAMIC_COPY
};

static const GLenum GLDrawElementsTypes[] = {
    GL_UNSIGNED_BYTE,
    GL_UNSIGNED_SHORT,
    GL_UNSIGNED_INT
};

static const GLenum GLShaderTypes[] = {
    GL_FRAGMENT_SHADER,
    GL_VERTEX_SHADER,
    GL_GEOMETRY_SHADER,
    GL_TESS_EVALUATION_SHADER,
    GL_TESS_CONTROL_SHADER,
    GL_COMPUTE_SHADER
};

static const GLenum GLInternalFormats[] = {
    0,
    GL_ALPHA8,
    GL_LUMINANCE8,
    GL_LUMINANCE8_ALPHA8,
    GL_R8,
    GL_R8I,
    GL_R8UI,
    GL_R8_SNORM,
    GL_R16,
    GL_R16F,
    GL_R16I,
    GL_R16UI,
    GL_R16_SNORM,
    GL_R32F,
    GL_R32I,
    GL_R32UI,
    GL_RG8,
    GL_RG8I,
    GL_RG8UI,
    GL_RG8_SNORM,
    GL_RG16,
    GL_RG16F,
    GL_RG16I,
    GL_RG16UI,
    GL_RG16_SNORM,
    GL_RG32F,
    GL_RG32I,
    GL_RG32UI,
    GL_RGB5,
    GL_RGB8,
    GL_RGB8I,
    GL_RGB8UI,
    GL_RGB8_SNORM,
    GL_SRGB8,
    GL_RGB16,
    GL_RGB16F,
    GL_RGB16I,
    GL_RGB16UI,
    GL_RGB16_SNORM,
    GL_RGB32I,
    GL_RGB32UI,
    GL_RGBA4,
    GL_RGB5_A1,
    GL_RGBA8,
    GL_RGBA8I,
    GL_RGBA8UI,
    GL_RGBA8_SNORM,
    GL_SRGB8_ALPHA8,
    GL_RGBA16,
    GL_RGBA16F,
    GL_RGBA16I,
    GL_RGBA16UI,
    GL_RGBA32F,
    GL_RGBA32I,
    GL_RGBA32UI,
    GL_DEPTH_COMPONENT16,
    GL_DEPTH_COMPONENT24,
    GL_DEPTH24_STENCIL8,
    GL_DEPTH_COMPONENT32F,
    GL_DEPTH32F_STENCIL8,
    GL_COMPRESSED_RGBA_S3TC_DXT1_EXT,
    GL_COMPRESSED_RGBA_S3TC_DXT3_EXT,
    GL_COMPRESSED_RGBA_S3TC_DXT5_EXT,
    GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT,
    GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT,
    GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT,
    GL_COMPRESSED_RGB8_ETC2,
    GL_COMPRESSED_SRGB8_ETC2,
    GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2,
    GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2,
    GL_COMPRESSED_RGBA8_ETC2_EAC,
    GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC,
    GL_COMPRESSED_RGBA_ASTC_4x4_KHR,
    GL_COMPRESSED_RGBA_ASTC_5x5_KHR,
    GL_COMPRESSED_RGBA_ASTC_6x6_KHR,
    GL_COMPRESSED_RGBA_ASTC_8x8_KHR,
    GL_COMPRESSED_RGBA_ASTC_10x10_KHR,
    GL_COMPRESSED_RGBA_ASTC_12x12_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x8_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x10_KHR,
    GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x12_KHR
};

static const GLenum GLFramebufferAttachments[] = {
    GL_COLOR_ATTACHMENT0,
    GL_COLOR_ATTACHMENT1,
    GL_COLOR_ATTACHMENT2,
    GL_COLOR_ATTACHMENT3,
    GL_COLOR_ATTACHMENT4,
    GL_COLOR_ATTACHMENT5,
    GL_COLOR_ATTACHMENT6,
    GL_COLOR_ATTACHMENT7,
    GL_DEPTH_STENCIL_ATTACHMENT,
    GL_DEPTH_ATTACHMENT,
    GL_STENCIL_ATTACHMENT
};

static const GLenum GLFramebufferTargets[] = {
    GL_READ_FRAMEBUFFER,
    GL_DRAW_FRAMEBUFFER,
    GL_FRAMEBUFFER
};

static const GLenum GLBlitFramebufferFilters[] = {
    GL_NEAREST,
    GL_LINEAR
};

static const GLenum GLTextureTargets[] = {
    GL_TEXTURE_2D,
    GL_TEXTURE_3D,
    GL_TEXTURE_CUBE_MAP,
    GL_TEXTURE_CUBE_MAP_POSITIVE_X,
    GL_TEXTURE_CUBE_MAP_NEGATIVE_X,
    GL_TEXTURE_CUBE_MAP_POSITIVE_Y,
    GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
    GL_TEXTURE_CUBE_MAP_POSITIVE_Z,
    GL_TEXTURE_CUBE_MAP_NEGATIVE_Z,
    GL_TEXTURE_2D_MULTISAMPLE
};

static const GLenum GLPixelFormats[] = {
    0,
    GL_RED,
    GL_GREEN,
    GL_BLUE,
    GL_ALPHA,
    GL_RGB,
    GL_RGBA,
    GL_LUMINANCE,
    GL_LUMINANCE_ALPHA,
    GL_RG,
    GL_RG_INTEGER,
    GL_DEPTH_COMPONENT,
    GL_DEPTH_STENCIL,
    GL_RED_INTEGER,
    GL_GREEN_INTEGER,
    GL_BLUE_INTEGER,
    GL_ALPHA_INTEGER,
    GL_RGB_INTEGER,
    GL_RGBA_INTEGER
};

static const GLenum GLPixelTypes[] = {
    0,
    GL_BYTE,
    GL_UNSIGNED_BYTE,
    GL_SHORT,
    GL_UNSIGNED_SHORT,
    GL_INT,
    GL_UNSIGNED_INT,
    GL_FLOAT,
    GL_HALF_FLOAT,
    GL_UNSIGNED_SHORT_4_4_4_4,
    GL_UNSIGNED_SHORT_5_5_5_1,
    GL_UNSIGNED_INT_8_8_8_8,
    GL_UNSIGNED_SHORT_5_6_5,
    GL_FLOAT_32_UNSIGNED_INT_24_8_REV
};

static const GLenum GLTextureWrapModes[] = {
    GL_REPEAT,
    GL_CLAMP_TO_BORDER,
    GL_CLAMP_TO_EDGE,
    GL_MIRRORED_REPEAT
};

static const GLenum GLTextureMinFilters[] = {
    GL_NEAREST,
    GL_LINEAR,
    GL_NEAREST_MIPMAP_NEAREST,
    GL_LINEAR_MIPMAP_NEAREST,
    GL_NEAREST_MIPMAP_LINEAR,
    GL_LINEAR_MIPMAP_LINEAR,
};

static const GLenum GLTextureMagFilters[] = {
    GL_NEAREST,
    GL_LINEAR
};

static const GLenum GLTextureParameterAxisNames[] = {
    GL_TEXTURE_WRAP_S,
    GL_TEXTURE_WRAP_T,
    GL_TEXTURE_WRAP_R
};

static const GLenum GLTextureAccesses[] = {
    GL_READ_ONLY,
    GL_WRITE_ONLY,
    GL_READ_WRITE
};

#ifdef CDK_WINDOWS
static const char* ShaderVersion = "#version 330\n";
#else
static const char* ShaderVersion = "#version 330 es\n";
#endif

static bool checkMemoryAlloc() {
    int err = glGetError();
#ifdef CS_GL_DEBUG
    CSAssert(err == 0 || err == GL_OUT_OF_MEMORY, "gl error:%d", err);
#endif
    return err == 0;
}

#ifdef CS_GL_DEBUG
static void assertError() {
	int err = glGetError();
    CSAssert(err == 0, "gl error:%d", err);
}
static void assertFramebufferStatus() {
	int status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    CSAssert(status == GL_FRAMEBUFFER_COMPLETE, "invalid frame buffer status:%d", status);
}
#else
#define assertError();
#define assertFramebufferStatus();
#endif

static GLenum getGLPolygonMode(int v) {
    switch (v) {
        case 1:
            return GL_LINE;
        case 2:
            return GL_POINT;
    }
    return GL_FILL;
}

void CSOpenGLApi::applyPolygonMode(CSPolygonMode mode) {
    if (mode != _polygonMode) {
        int front = mode >> 2;
        int back = mode & 3;

        if (front == back) {
            glPolygonMode(GL_FRONT_AND_BACK, getGLPolygonMode(front));
        }
        else {
            glPolygonMode(GL_FRONT, getGLPolygonMode(front));
            glPolygonMode(GL_BACK, getGLPolygonMode(back));
        }
        _polygonMode = mode;
    }
}

void CSOpenGLApi::applyCullMode(CSCullMode mode) {
    if (_cullMode != mode) {
        switch (mode) {
            case CSCullNone:
                glDisable(GL_CULL_FACE);
                break;
            case CSCullBack:
                glEnable(GL_CULL_FACE);
                glFrontFace(GL_CW);
                break;
            case CSCullFront:
                glEnable(GL_CULL_FACE);
                glFrontFace(GL_CCW);
                break;
        }
        _cullMode = mode;
    }
}

void CSOpenGLApi::applyDepthMode(CSDepthMode mode) {
    if (_depthMode != mode) {
        if (mode != CSDepthNone) {
            glEnable(GL_DEPTH_TEST);
            glDepthFunc((mode & CSDepthRead) ? GL_LEQUAL : GL_ALWAYS);
            glDepthMask((mode & CSDepthWrite) != 0);
        }
        else {
            glDisable(GL_DEPTH_TEST);
            glDepthMask(false);
        }
        _depthMode = mode;
    }
}

void CSOpenGLApi::applyStencilMode(CSStencilMode mode, int depth) {
    if (_stencilMode != mode || _stencilDepth != depth) {
        switch (mode) {
            case CSStencilNone:
                if (depth != 0) {
                    glEnable(GL_STENCIL_TEST);
                    glStencilFunc(GL_EQUAL, _stencilDepth, 0xff);
                }
                else {
                    glDisable(GL_STENCIL_TEST);
                }
                glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
                glColorMask(true, true, true, true);
                break;
            case CSStencilInclusive:
                glEnable(GL_STENCIL_TEST);
                glStencilFunc(GL_EQUAL, depth - 1, 0xff);
                glStencilOp(GL_KEEP, GL_KEEP, GL_INCR);
                glColorMask(false, false, false, false);
                break;
            case CSStencilExclusive:
                if (depth != 0) {
                    glEnable(GL_STENCIL_TEST);
                    glStencilFunc(GL_EQUAL, depth, 0xff);
                    glStencilOp(GL_KEEP, GL_KEEP, GL_DECR);
                }
                else {
                    glDisable(GL_STENCIL_TEST);
                    glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
                }
                glColorMask(false, false, false, false);
                break;
        }

        _stencilMode = mode;
        _stencilDepth = depth;
    }
}
void CSOpenGLApi::applyBlendMode(CSBlendMode mode) {
    if (_blendMode != mode) {
        switch (mode) {
            case CSBlendNone:
                glDisable(GL_BLEND);
                break;
            case CSBlendAdd:
                glEnable(GL_BLEND);
                glBlendFuncSeparate(GL_ONE, GL_ONE, GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                glBlendEquation(GL_ADD);
                break;
            case CSBlendSubstract:
                glEnable(GL_BLEND);
                glBlendFuncSeparate(GL_ONE, GL_ONE, GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                glBlendEquationSeparate(GL_SUBTRACT, GL_ADD);
                break;
            case CSBlendMultiply:
                glEnable(GL_BLEND);
                glBlendFuncSeparate(GL_DST_COLOR, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                glBlendEquation(GL_ADD);
                break;
            case CSBlendScreen:
                glEnable(GL_BLEND);
                glBlendFuncSeparate(GL_ONE, GL_ONE_MINUS_SRC_COLOR, GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                glBlendEquation(GL_ADD);
                break;
            default:
                glEnable(GL_BLEND);
                glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                glBlendEquation(GL_ADD);
                break;
        }
        _blendMode = mode;
    }
}

void CSOpenGLApi::applyScissor(const CSBounds2& scissor) {
    if (_scissor != scissor) {
        if (scissor != CSBounds2::Zero) {
            glEnable(GL_SCISSOR_TEST);
            glScissor(scissor.x, currentTarget()->height() - scissor.height - scissor.y, scissor.width, scissor.height);      //opengl coordinate system (bottom zero)
        }
        else glDisable(GL_SCISSOR_TEST);
        _scissor = scissor;
    }
}

void CSOpenGLApi::applyLineWidth(float width) {
    if (_lineWidth != width) {
        glLineWidth(width);
        _lineWidth = width;
    }
}

int CSOpenGLApi::genBuffer() {
    uint result;
    glGenBuffers(1, &result);
    return result;
}

void CSOpenGLApi::deleteBuffer(int obj) {
    glDeleteBuffers(1, (uint*) & obj);
    assertError();
}

void CSOpenGLApi::bindBuffer(CSGBufferTarget target, int obj) {
    glBindBuffer(GLBufferTargets[target], obj);
    assertError();
}

void CSOpenGLApi::bindBufferBase(CSGBufferTarget target, int binding, int obj) {
    glBindBufferBase(GLBufferTargets[target], binding, obj);
    assertError();
}

bool CSOpenGLApi::bufferData(CSGBufferTarget target, int size, const void* data, CSGBufferUsageHint hint) {
    glBufferData(GLBufferTargets[target], size, data, GLBufferUsageHints[hint]);
    return checkMemoryAlloc();
}

void CSOpenGLApi::bufferSubData(CSGBufferTarget target, int offset, int size, const void* data) {
    glBufferSubData(GLBufferTargets[target], offset, size, data);
    assertError();
}

int CSOpenGLApi::genVertexArray() {
    uint result;
    glGenVertexArrays(1, &result);
    return result;
}

void CSOpenGLApi::deleteVertexArray(int obj) {
    glDeleteVertexArrays(1, (uint*)&obj);
    assertError();
}

void CSOpenGLApi::bindVertexArray(int obj) {
    glBindVertexArray(obj);
    _vertexArrayBinding = obj;
    assertError();
}

int CSOpenGLApi::vertexArrayBinding() {
    return _vertexArrayBinding;
}

void CSOpenGLApi::vertexAttribDivisor(int index, int divisor) {
    glVertexAttribDivisor(index, divisor);
    assertError();
}

void CSOpenGLApi::vertexAttribPointer(int index, int size, CSVertexAttribType type, bool normalized, int stride, int offset) {
    switch (type) {
        case CSVertexAttribTypeFloat:
        case CSVertexAttribTypeDouble:
        case CSVertexAttribTypeHalfFloat:
            glVertexAttribPointer(index, size, GLVertexAttribTypes[type], normalized, stride, (const void*)(int64)offset);
            break;
        default:
            glVertexAttribIPointer(index, size, GLVertexAttribTypes[type], stride, (const void*)(int64)offset);
            break;
    }
    assertError();
}

void CSOpenGLApi::setVertexAttribEnabled(int index, bool enabled) {
    if (enabled) glEnableVertexAttribArray(index);
    else glDisableVertexAttribArray(index);
    assertError();
}

void CSOpenGLApi::clearBufferColor(int layer, const CSColor& color) {
    glClearBufferfv(GL_COLOR, layer, color);
    assertError();
}

void CSOpenGLApi::clearBufferDepthStencil() {
    glClearBufferfi(GL_DEPTH_STENCIL, 0, 1, 0);
    assertError();
}

void CSOpenGLApi::clearBufferDepth() {
    float depth = 1;
    glClearBufferfv(GL_DEPTH, 0, &depth);
}

void CSOpenGLApi::clear(const CSColor& color) {
    glColorMask(true, true, true, true);
    glDepthMask(true);
    glClearColor(color.r, color.g, color.b, color.a);
    glClearDepthf(1);
    glClearStencil(0);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
    if (_depthMode & CSDepthWrite) glDepthMask(false);
    if (_stencilMode != CSStencilNone) glColorMask(false, false, false, false);
    assertError();
}

void CSOpenGLApi::clearStencil(const CSRect& bounds, int depth) {
    int clearByShader = depth > 1;

    if (!clearByShader) {
        float size = bounds.width * bounds.height;
        const float clearByShaderMaxSize = 4 * 0.36f;        //벤치마크에서 전체화면의 36% 정도까지 쉐이더가 더 빠름 (ipad 4)
        if (size <= clearByShaderMaxSize) clearByShader = true;
    }
    if (clearByShader) {
        applyStencilMode(CSStencilExclusive, depth);

        CSShaders::empty()->draw(this, CSVertexArrayDraw::array(CSVertexArrays::get2D(bounds), CSPrimitiveTriangleStrip, 0, 4));
    }
    else {
        glClearStencil(0);
        glClear(GL_STENCIL_BUFFER_BIT);
    }
    assertError();
}

void CSOpenGLApi::drawArrays(CSPrimitiveMode mode, int first, int count) {
    glDrawArrays(GLPrimitiveTypes[mode], first, count);
    assertError();

    CSDiagnostics::renderDraw(count);
}

void CSOpenGLApi::drawElements(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices) {
    glDrawElements(GLPrimitiveTypes[mode], count, GLDrawElementsTypes[type], (const void*)(int64)indices);
    assertError();

    CSDiagnostics::renderDraw(count);
}

void CSOpenGLApi::drawElementsInstanced(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices, int instanceCount) {
    glDrawElementsInstanced(GLPrimitiveTypes[mode], count, GLDrawElementsTypes[type], (const void*)(int64)indices, instanceCount);
    assertError();

    CSDiagnostics::renderDraw(count * instanceCount);
}

int CSOpenGLApi::genRenderbuffer() {
    uint result;
    glGenRenderbuffers(1, &result);
    return result;
}

void CSOpenGLApi::deleteRenderbuffer(int obj) {
    glDeleteRenderbuffers(1, (uint*)&obj);
    assertError();
}

void CSOpenGLApi::bindRenderbuffer(int obj) {
    glBindRenderbuffer(GL_RENDERBUFFER, obj);
    assertError();
}

bool CSOpenGLApi::renderbufferStorage(CSRawFormat format, int samples, int width, int height) {
    GLenum internalFormat = GLInternalFormats[format];
    if (samples <= 1) glRenderbufferStorage(GL_RENDERBUFFER, internalFormat, width, height);
    else glRenderbufferStorageMultisample(GL_RENDERBUFFER, samples, internalFormat, width, height);
    return checkMemoryAlloc();
}

int CSOpenGLApi::genFramebuffer() {
    uint result;
    glGenFramebuffers(1, &result);
    return result;
}

void CSOpenGLApi::deleteFramebuffer(int obj) {
    glDeleteFramebuffers(1, (uint*)&obj);
    assertError();
}

void CSOpenGLApi::bindFramebuffer(CSFramebufferTarget target, int obj) {
    glBindFramebuffer(GLFramebufferTargets[target], obj);
    assertError();
}

void CSOpenGLApi::framebufferRenderbuffer(CSFramebufferTarget target, CSFramebufferAttachment attachment, int renderbuffer) {
    glFramebufferRenderbuffer(GLFramebufferTargets[target], GLFramebufferAttachments[attachment], GL_RENDERBUFFER, renderbuffer);
    assertError();
}

void CSOpenGLApi::framebufferTexture(CSFramebufferTarget target, CSFramebufferAttachment attachment, int texture, int layer) {
    glFramebufferTexture(GLFramebufferTargets[target], GLFramebufferAttachments[attachment], texture, layer);
    assertError();
}

void CSOpenGLApi::readBuffer(int buf) {
    glReadBuffer(GL_COLOR_ATTACHMENT0 + buf);
    assertError();
}

void CSOpenGLApi::drawBuffer(int buf) {
    glDrawBuffer(GL_COLOR_ATTACHMENT0 + buf);
    assertError();
}

void CSOpenGLApi::drawBuffers(const byte* bufs, int count) {
    GLenum* bufs_ = (GLenum*)alloca(count * sizeof(GLenum));
    for (int i = 0; i < count; i++) bufs_[i] = GL_COLOR_ATTACHMENT0 + bufs[i];
    glDrawBuffers(count, bufs_);
    assertError();
}

void CSOpenGLApi::viewport(const CSBounds2& viewport) {
    glViewport(viewport.x, currentTarget()->height() - viewport.height - viewport.y, viewport.width, viewport.height);      //opengl coordinate system (bottom zero)
    assertError();
}

void CSOpenGLApi::bindTexture(CSTextureTarget target, int texture) {
    glBindTexture(GLTextureTargets[target], texture);
    assertError();
}

void CSOpenGLApi::bindTextureBase(CSTextureTarget target, int binding, int texture) {
    glActiveTexture(GL_TEXTURE0 + binding);
    glBindTexture(GLTextureTargets[target], texture);
    assertError();
}

void CSOpenGLApi::bindImageTexture(int binding, int texture, int level, bool layered, int layer, CSTextureAccess access, CSRawFormat format) {
    glBindImageTexture(binding, texture, level, layered, layer, GLTextureAccesses[access], GLInternalFormats[format]);
}

bool CSOpenGLApi::copyTexImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds) {
    glCopyTexImage2D(GLTextureTargets[target], level, GLInternalFormats[format], bounds.x, bounds.y, bounds.width, bounds.height, 0);
    return checkMemoryAlloc();
}

void CSOpenGLApi::blitFramebuffer(const CSBounds2& srcbounds, const CSBounds2& destbounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter) {
    glBlitFramebuffer(srcbounds.left(), srcbounds.top(), srcbounds.right(), srcbounds.bottom(), destbounds.left(), destbounds.top(), destbounds.right(), destbounds.bottom(), mask, GLBlitFramebufferFilters[filter]);
    assertError();
}

void CSOpenGLApi::readPixels(const CSBounds2& bounds, CSPixelFormat format, CSPixelType type, void* pixels) {
    glReadPixels(bounds.x, bounds.y, bounds.width, bounds.height, GLPixelFormats[format], GLPixelTypes[type], pixels);
    assertError();
}

void CSOpenGLApi::texParameterWrap(CSTextureTarget target, int axis, CSTextureWrapMode param) {
    glTexParameteri(GLTextureTargets[target], GLTextureParameterAxisNames[axis], GLTextureWrapModes[param]);
    assertError();
}

void CSOpenGLApi::texParameterMinFilter(CSTextureTarget target, CSTextureMinFilter param) {
    glTexParameteri(GLTextureTargets[target], GL_TEXTURE_MIN_FILTER, GLTextureMinFilters[param]);
    assertError();
}

void CSOpenGLApi::texParameterMagFilter(CSTextureTarget target, CSTextureMagFilter param) {
    glTexParameteri(GLTextureTargets[target], GL_TEXTURE_MAG_FILTER, GLTextureMagFilters[param]);
    assertError();
}

void CSOpenGLApi::texParameterBorderColor(CSTextureTarget target, const CSColor& param) {
    glTexParameterfv(GLTextureTargets[target], GL_TEXTURE_BORDER_COLOR, param);
    assertError();
}

int CSOpenGLApi::genTexture() {
    uint result;
    glGenTextures(1, &result);
    return result;
}

void CSOpenGLApi::deleteTexture(int obj) {
    glDeleteTextures(1, (uint*)&obj);
    assertError();
}

bool CSOpenGLApi::texImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(!enc.compressed);
    glTexImage2D(GLTextureTargets[target], level, GLInternalFormats[format], width, height, 0, GLPixelFormats[enc.pixelFormat], GLPixelTypes[enc.pixelType], data);
    return checkMemoryAlloc();
}

bool CSOpenGLApi::texImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(!enc.compressed);
    glTexImage3D(GLTextureTargets[target], level, GLInternalFormats[format], width, height, depth, 0, GLPixelFormats[enc.pixelFormat], GLPixelTypes[enc.pixelType], data);
    return checkMemoryAlloc();
}

bool CSOpenGLApi::texImage2DMultisample(CSTextureTarget target, int samples, CSRawFormat format, int width, int height) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(!enc.compressed);
    glTexImage2DMultisample(GLTextureTargets[target], samples, GLInternalFormats[format], width, height, true);
    return checkMemoryAlloc();
}

void CSOpenGLApi::texSubImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(!enc.compressed);
    glTexSubImage2D(GLTextureTargets[target], level, bounds.x, bounds.y, bounds.width, bounds.height, GLPixelFormats[enc.pixelFormat], GLPixelTypes[enc.pixelType], data);
    assertError();
}

void CSOpenGLApi::texSubImage3D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds3& bounds, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(!enc.compressed);
    glTexSubImage3D(GLTextureTargets[target], level, bounds.x, bounds.y, bounds.z, bounds.width, bounds.height, bounds.depth, GLPixelFormats[enc.pixelFormat], GLPixelTypes[enc.pixelType], data);
    assertError();
}

bool CSOpenGLApi::compressedTexImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int size, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(enc.compressed);
    glCompressedTexImage2D(GLTextureTargets[target], level, GLInternalFormats[format], width, height, 0, size, data);
    return checkMemoryAlloc();
}

bool CSOpenGLApi::compressedTexImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, int size, const void* data) {
    const CSRawFormatEncoding& enc = format.encoding();
    CSAssert(enc.compressed);
    glCompressedTexImage3D(GLTextureTargets[target], level, GLInternalFormats[format], width, height, depth, 0, size, data);
    return checkMemoryAlloc();
}

bool CSOpenGLApi::generateMipmap(CSTextureTarget target, int maxLevel) {
    GLenum gltarget = GLTextureTargets[target];
    glTexParameteri(gltarget, GL_TEXTURE_MAX_LEVEL, maxLevel);
    glGenerateMipmap(gltarget);
    return checkMemoryAlloc();
}

int CSOpenGLApi::createProgram() {
    return glCreateProgram();
}

void CSOpenGLApi::deleteProgram(int program) {
    glDeleteProgram(program);
    assertError();
}

void CSOpenGLApi::attachShader(int program, int shader) {
    glAttachShader(program, shader);
    assertError();
}

void CSOpenGLApi::detachShader(int program, int shader) {
    glDetachShader(program, shader);
    assertError();
}

void CSOpenGLApi::linkProgram(int program) {
    glLinkProgram(program);

    assertError();

    int status;
    glGetProgramiv(program, GL_LINK_STATUS, &status);

    if (status == 0) {
#ifdef CS_GL_DEBUG
        int logLength;
        glGetProgramiv(program, GL_INFO_LOG_LENGTH, &logLength);
        if (logLength > 0) {
            logLength++;
            char* log = (char*)alloca(logLength);
            glGetProgramInfoLog(program, logLength, &logLength, log);
            CSErrorLog("link log:\n%s", log);
        }
#endif
        abort();
    }
}

void CSOpenGLApi::useProgram(int program) {
    glUseProgram(program);
    assertError();
}

int CSOpenGLApi::createShader(CSShaderType type, const char* const* sources, int count) {
    uint shader = glCreateShader(GLShaderTypes[type]);

    static const int ShaderVersionLen = strlen(ShaderVersion);

    int* length = (int*)alloca((count + 1) * sizeof(int));
    length[0] = ShaderVersionLen;
    for (int i = 0; i < count; i++) length[i + 1] = strlen(sources[i]);

    const char** sources_ = (const char**)alloca((count + 1) * sizeof(const char*));
    sources_[0] = ShaderVersion;
    memcpy(sources_ + 1, sources, count * sizeof(const char*));
    count++;

    glShaderSource(shader, count, sources_, length);
    glCompileShader(shader);
    assertError();

    int status;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &status);
    if (status == 0) {
#ifdef CS_GL_DEBUG
        int logLength;
        glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &logLength);
        if (logLength > 0) {
            logLength++;
            char* log = (char*)alloca(logLength);
            glGetShaderInfoLog(shader, logLength, &logLength, log);
            CSErrorLog("compile log:\n%s", log);
        }
#endif
        abort();
    }
    return shader;
}

void CSOpenGLApi::deleteShader(int shader) {
    glDeleteShader(shader);
    assertError();
}