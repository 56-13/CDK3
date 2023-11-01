#ifndef __CDK__CSGraphicsApi__
#define __CDK__CSGraphicsApi__

#include "CSGEnums.h"

#include "CSRect.h"
#include "CSColor.h"
#include "CSBounds2.h"
#include "CSBounds3.h"

class CSRenderTarget;

class CSGraphicsApi {
private:
    CSRenderTarget* _currentTarget;
protected:
    inline CSGraphicsApi() : _currentTarget(NULL) {}
    virtual ~CSGraphicsApi() = default;

    CSGraphicsApi(const CSGraphicsApi&) = delete;
    CSGraphicsApi& operator=(const CSGraphicsApi&) = delete;
public:
    virtual void applyPolygonMode(CSPolygonMode mode) = 0;
    virtual void applyCullMode(CSCullMode mode) = 0;
    virtual void applyDepthMode(CSDepthMode mode) = 0;
    virtual void applyStencilMode(CSStencilMode mode, int depth) = 0;
    virtual void applyBlendMode(CSBlendMode mode) = 0;
    virtual void applyScissor(const CSBounds2& scissor) = 0;
    virtual void applyLineWidth(float width) = 0;

    virtual int genBuffer() = 0;
    virtual void deleteBuffer(int obj) = 0;
    virtual void bindBuffer(CSGBufferTarget target, int obj) = 0;
    virtual void bindBufferBase(CSGBufferTarget target, int binding, int obj) = 0;
    virtual bool bufferData(CSGBufferTarget target, int size, const void* data, CSGBufferUsageHint hint) = 0;
    virtual void bufferSubData(CSGBufferTarget target, int offset, int size, const void* data) = 0;
    virtual int genVertexArray() = 0;
    virtual void deleteVertexArray(int obj) = 0;
    virtual void bindVertexArray(int obj) = 0;
    virtual int vertexArrayBinding() = 0;
    virtual void vertexAttribDivisor(int index, int divisor) = 0;
    virtual void vertexAttribPointer(int index, int size, CSVertexAttribType type, bool normalized, int stride, int offset) = 0;
    virtual void setVertexAttribEnabled(int index, bool enabled) = 0;
    virtual void clearBufferColor(int layer, const CSColor& color) = 0;
    virtual void clearBufferDepthStencil() = 0;
    virtual void clearBufferDepth() = 0;
    virtual void clear(const CSColor& color) = 0;
    virtual void clearStencil(const CSRect& bounds, int depth) = 0;
    virtual void drawArrays(CSPrimitiveMode mode, int first, int count) = 0;
    virtual void drawElements(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices) = 0;
    virtual void drawElementsInstanced(CSPrimitiveMode mode, int count, CSDrawElementsType type, int indices, int instanceCount) = 0;
    virtual int genRenderbuffer() = 0;
    virtual void deleteRenderbuffer(int obj) = 0;
    virtual void bindRenderbuffer(int obj) = 0;
    virtual bool renderbufferStorage(CSRawFormat format, int samples, int width, int height) = 0;
    virtual int genFramebuffer() = 0;
    virtual void deleteFramebuffer(int obj) = 0;
    virtual void bindFramebuffer(CSFramebufferTarget target, int obj) = 0;
    virtual void framebufferRenderbuffer(CSFramebufferTarget target, CSFramebufferAttachment attachment, int renderbuffer) = 0;
    virtual void framebufferTexture(CSFramebufferTarget target, CSFramebufferAttachment attachment, int texture, int layer) = 0;
    virtual void readBuffer(int buf) = 0;
    virtual void drawBuffer(int buf) = 0;
    virtual void drawBuffers(const byte* bufs, int count) = 0;
    virtual void viewport(const CSBounds2& viewport) = 0;
    virtual void bindTexture(CSTextureTarget target, int texture) = 0;
    virtual void bindTextureBase(CSTextureTarget target, int binding, int texture) = 0;
    virtual void bindImageTexture(int binding, int texture, int level, bool layered, int layer, CSTextureAccess access, CSRawFormat format) = 0;
    virtual bool copyTexImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds) = 0;
    virtual void blitFramebuffer(const CSBounds2& srcbounds, const CSBounds2& destbounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter) = 0;
    virtual void readPixels(const CSBounds2& bounds, CSPixelFormat format, CSPixelType type, void* pixels) = 0;
    virtual void texParameterWrap(CSTextureTarget target, int axis, CSTextureWrapMode param) = 0;
    virtual void texParameterMinFilter(CSTextureTarget target, CSTextureMinFilter param) = 0;
    virtual void texParameterMagFilter(CSTextureTarget target, CSTextureMagFilter param) = 0;
    virtual void texParameterBorderColor(CSTextureTarget target, const CSColor& param) = 0;
    virtual int genTexture() = 0;
    virtual void deleteTexture(int obj) = 0;
    virtual bool texImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, const void* data) = 0;
    virtual bool texImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, const void* data) = 0;
    virtual bool texImage2DMultisample(CSTextureTarget target, int samples, CSRawFormat format, int width, int height) = 0;
    virtual void texSubImage2D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds2& bounds, const void* data) = 0;
    virtual void texSubImage3D(CSTextureTarget target, int level, CSRawFormat format, const CSBounds3& bounds, const void* data) = 0;
    virtual bool compressedTexImage2D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int size, const void* data) = 0;
    virtual bool compressedTexImage3D(CSTextureTarget target, int level, CSRawFormat format, int width, int height, int depth, int size, const void* data) = 0;
    virtual bool generateMipmap(CSTextureTarget target, int maxLevel) = 0;
    virtual int createProgram() = 0;
    virtual void deleteProgram(int program) = 0;
    virtual void attachShader(int program, int shader) = 0;
    virtual void detachShader(int program, int shader) = 0;
    virtual void linkProgram(int program) = 0;
    virtual void useProgram(int program) = 0;
    virtual int createShader(CSShaderType type, const char* const* sources, int count) = 0;
    virtual void deleteShader(int shader) = 0;

    inline CSRenderTarget* currentTarget() {
        return _currentTarget;
    }
#ifdef CDK_IMPL
    inline void setCurrentTarget(CSRenderTarget* target) {
        _currentTarget = target;
    }
#endif
};

#endif
