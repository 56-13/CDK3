#ifndef __CDK__CSRenderTarget__
#define __CDK__CSRenderTarget__

#include "CSBounds2.h"
#include "CSRenderTargetDescription.h"
#include "CSRenderBuffer.h"
#include "CSTexture.h"

struct CSSystemRenderTargetDescription {
    byte redBit;
    byte greenBit;
    byte blueBit;
    byte alphaBit;
    byte depthBit;
    byte stencilBit;
    byte samples;
};

class CSRenderTarget : public CSGraphicsResource {
private:
    int _framebuffer = 0;
    ushort _width;
    ushort _height;
    mutable CSRenderTargetDescription _description;

    struct Buffer {
        CSFramebufferAttachment attachment;
        CSRenderBuffer* renderbuffer;
        CSTexture* texture;
        byte textureLayer;
        bool own;

        ~Buffer();

        inline const CSGraphicsResource* resource() const {
            return renderbuffer ? static_cast<CSGraphicsResource*>(renderbuffer) : texture;
        }
    };
    CSArray<Buffer> _buffers;
    bool _systemBuffer;
    bool _bloomSupported = false;
    mutable byte _drawBufferCount = 1;
    mutable byte _drawBuffers[8] = {};
    mutable bool _descriptionUpdated = false;
    CSBounds2 _viewport[2];
public:
    CSRenderTarget(int width, int height);
    CSRenderTarget(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc);
    CSRenderTarget(const CSRenderTargetDescription& desc);
private:
    ~CSRenderTarget();
public:
    static inline CSRenderTarget* target(int width, int height) {
        return autorelease(new CSRenderTarget(width, height));
    }
    static inline CSRenderTarget* target(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc) {
        return autorelease(new CSRenderTarget(width, height, framebuffer, desc));
    }
    static inline CSRenderTarget* target(const CSRenderTargetDescription& desc) {
        return autorelease(new CSRenderTarget(desc));
    }
public:
    inline CSResourceType resourceType() const override {
        return CSResourceTypeRenderTarget;
    }
    int resourceCost() const override;

    inline int framebuffer() const {
        return _framebuffer;
    }
    inline int width() const {
        return _width;
    }
    inline int height() const {
        return _height;
    }
    inline bool isSystemBuffer() const {
        return _systemBuffer;
    }
    int samples() const;

    const CSRenderTargetDescription& description() const;
private:
    void attachImpl(CSFramebufferAttachment attachment, CSRenderBuffer* renderbuffer, bool own);
    void attachImpl(CSFramebufferAttachment attachment, CSTexture* texture, int layer, bool own);
    void detachImpl(CSFramebufferAttachment attachment);
public:
    void attach(CSFramebufferAttachment attachment, CSRenderBuffer* renderbuffer, bool own);
    void attach(CSFramebufferAttachment attachment, CSTexture* texture, int layer, bool own);
    void detach(CSFramebufferAttachment attachment);

    CSTexture* textureAttachment(CSFramebufferAttachment attachment);
    CSRenderBuffer* renderbufferAttachment(CSFramebufferAttachment attachment);

    inline const CSTexture* textureAttachment(CSFramebufferAttachment attachment) const {
        return const_cast<CSRenderTarget*>(this)->textureAttachment(attachment);
    }
    inline const CSRenderBuffer* renderbufferAttachment(CSFramebufferAttachment attachment) const {
        return const_cast<CSRenderTarget*>(this)->renderbufferAttachment(attachment);
    }
    CSRawFormat format(CSFramebufferAttachment attachment) const;

    inline bool bloomSupported() const {
        return _bloomSupported;
    }

    bool batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const override;
private:
    const Buffer* buffer(CSFramebufferAttachment attachment) const;
public:
    void clearColor(int layer, const CSColor& color);
    void clearDepthStencil();
    void clearDepth();

    const CSBounds2& viewport() const;
    void setViewport(CSBounds2 viewport);
    void clearViewport();
    bool hasViewport() const;

    void resize(int width, int height);
    void focus(CSGraphicsApi* api);
    void setDrawBuffer(CSGraphicsApi* api, int buf);
    void setDrawBuffers(CSGraphicsApi* api, int count, ...);

    inline CSTexture* captureColor(int buf, bool temporary) const {
        return captureColor(buf, viewport(), temporary);
    }
    CSTexture* captureColor(int buf, CSBounds2 bounds, bool temporary) const;
    inline CSTexture* captureDepth(bool temporary) const {
        return captureDepth(viewport(), temporary);
    }
    CSTexture* captureDepth(CSBounds2 bounds, bool temporary) const;

    inline void blit(CSRenderTarget* target, CSClearBufferMask mask, CSBlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0) const {
        blit(target, viewport(), target->viewport(), mask, filter, srcbuf, destbuf);
    }
    inline void blit(CSRenderTarget* target, const CSBounds2& bounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0) const {
        blit(target, viewport(), bounds, mask, filter, srcbuf, destbuf);
    }
    void blit(CSRenderTarget* target, CSBounds2 srcbounds, CSBounds2 destbounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter, int srcbuf = 0, int destbuf = 0) const;
private:
    bool screenshotImpl(CSGraphicsApi* api, const char* path) const;
public:
    bool screenshot(const char* path) const;
};

#endif
