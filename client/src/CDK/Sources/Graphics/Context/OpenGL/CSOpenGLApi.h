#ifdef CDK_IMPL

#ifndef __CDK__CSOpenGLApi__
#define __CDK__CSOpenGLApi__

#include "CSGraphicsApi.h"

#ifdef CDK_ANDROID
# include <GLES3/gl32.h>
# include <GLES3/gl3ext.h>
#elif defined(CDK_IOS)
# include <OpenGLES/ES3/gl.h>
# include <OpenGLES/ES3/glext.h>
#elif defined(CDK_WINDOWS)
# include "GL/glew.h"
#else
#error "unknown platform"
#endif

class CSOpenGLApi : public CSGraphicsApi {
private:
    CSPolygonMode _polygonMode = CSPolygonFill;
    CSCullMode _cullMode = CSCullNone;
    CSDepthMode _depthMode = CSDepthNone;
    CSStencilMode _stencilMode = CSStencilNone;
    byte _stencilDepth = 0;
    CSBlendMode _blendMode = CSBlendNone;
    CSBounds2 _scissor = CSBounds2::Zero;
    float _lineWidth = 1;
    int _vertexArrayBinding = 0;
public:
    void applyPolygonMode(CSPolygonMode mode) override;
    void applyCullMode(CSCullMode mode) override;
    void applyDepthMode(CSDepthMode mode) override;
    void applyStencilMode(CSStencilMode mode, int depth) override;
    void applyBlendMode(CSBlendMode mode) override;
    void applyScissor(const CSBounds2& scissor) override;
    void applyLineWidth(float width) override;

    int genBuffer() override;
    void deleteBuffer(int obj) override;
    void bindBuffer(CSGBufferTarget target, int obj) override;
    void bindBufferBase(CSGBufferTarget target, int binding, int obj) override;
    bool bufferData(CSGBufferTarget target, int size, const void* data, CSGBufferUsageHint hint) override;
    void bufferSubData(CSGBufferTarget target, int offset, int size, const void* data) override;
    int genVertexArray() override;
    void deleteVertexArray(int obj) override;
    void bindVertexArray(int obj) override;
    int vertexArrayBinding() override;
    void vertexAttribDivisor(int index, int divisor) override;
    void vertexAttribPointer(int index, int size, CSVertexAttribType type, bool normalized, int stride, int offset) override;
    void setVertexAttribEnabled(int index, bool enabled) override;
    void clearBufferColor(int layer, const CSColor& color) override;
    void clearBufferDepthStencil() override;
    void clearBufferDepth() override;
    void clear(const CSColor& color) override;
    void clearStencil(const CSRect& bounds, int depth) override;
    void drawArrays(CSPrimitiveMode mode, int first, int count) override;
    void drawElements(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices) override;
    void drawElementsInstanced(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices, int instanceCount) override;
    int genRenderbuffer() override;
    void deleteRenderbuffer(int obj) override;
    void bindRenderbuffer(int obj) override;
    bool renderbufferStorage(CSRawFormat format, int samples, int width, int height) override;
    int genFramebuffer() override;
    void deleteFramebuffer(int obj) override;
    void bindFramebuffer(CSFramebufferTarget target, int obj) override;
    void framebufferRenderbuffer(CSFramebufferTarget target, CSFramebufferAttachment attachment, int renderbuffer) override;
    void framebufferTexture(CSFramebufferTarget target, CSFramebufferAttachment attachment, int texture, int layer) override;
    void readBuffer(int buf) override;
    void drawBuffer(int buf) override;
    void drawBuffers(const byte* bufs, int count) override;
    void viewport(const CSBounds2& viewport) override;
    void bindTexture(CSTextureTarget target, int texture) override;
    void bindTextureBase(CSTextureTarget target, int binding, int texture) override;
    void bindImageTexture(int binding, int texture, int level, bool layered, int layer, CSTextureAccess access, CSRawFormat format) override;
    bool copyTexImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds) override;
    void blitFramebuffer(const CSBounds2& srcbounds, const CSBounds2& destbounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter) override;
    void readPixels(const CSBounds2& bounds, CSPixelFormat format, CSPixelType type, void* pixels) override;
    void texParameterWrap(CSTextureTarget target, int axis, CSTextureWrapMode param) override;
    void texParameterMinFilter(CSTextureTarget target, CSTextureMinFilter param) override;
    void texParameterMagFilter(CSTextureTarget target, CSTextureMagFilter param) override;
    void texParameterBorderColor(CSTextureTarget target, const CSColor& param) override;
    int genTexture() override;
    void deleteTexture(int obj) override;
    bool texImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, const void* data) override;
    bool texImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, const void* data) override;
    bool texImage2DMultisample(CSTextureTarget target, int samples, CSRawFormat format, int width, int height) override;
    void texSubImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds, const void* data) override;
    void texSubImage3D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds3& bounds, const void* data) override;
    bool compressedTexImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int size, const void* data) override;
    bool compressedTexImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, int size, const void* data) override;
    bool generateMipmap(CSTextureTarget target, int maxLevel) override;
    int createProgram() override;
    void deleteProgram(int program) override;
    void attachShader(int program, int shader) override;
    void detachShader(int program, int shader) override;
    void linkProgram(int program) override;
    void useProgram(int program) override;
    int createShader(CSShaderType type, const char* const* sources, int count) override;
    void deleteShader(int shader) override;
};

#endif

#endif