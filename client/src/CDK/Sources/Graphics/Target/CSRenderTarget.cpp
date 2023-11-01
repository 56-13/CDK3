#define CDK_IMPL

#include "CSRenderTarget.h"

#include "CSTextureDescription.h"
#include "CSResourcePool.h"
#include "CSRenderTargets.h"
#include "CSTextures.h"
#include "CSGraphics.h"
#include "CSRaster.h"

CSRenderTarget::Buffer::~Buffer() {
    if (renderbuffer) renderbuffer->release();
    else if (texture) texture->release();
}

//=================================================================================

CSRenderTarget::CSRenderTarget(int width, int height) : 
    _width(width), 
    _height(height),
    _systemBuffer(false),
    _viewport { CSBounds2(0, 0, width, height), CSBounds2(0, 0, width, height) }
{
    _description.width = width;
    _description.height = height;

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) { _framebuffer = api->genFramebuffer(); }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);
}

CSRenderTarget::CSRenderTarget(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc) :
    _framebuffer(framebuffer),
    _width(width),
    _height(height),
    _systemBuffer(true),
    _descriptionUpdated(true),
    _viewport { CSBounds2(0, 0, width, height), CSBounds2(0, 0, width, height) }
{
    int samples = CSMath::max((int)desc.samples, 1);

    CSRawFormat colorFormat;
    if (desc.redBit == 8 && desc.greenBit == 8 && desc.blueBit == 8) {
        if (desc.alphaBit == 8) colorFormat = CSRawFormat::Rgba8;
        else if (desc.alphaBit == 0) colorFormat = CSRawFormat::Rgb8;
        else {
            CSErrorLog("unknown color format");
            abort();
        }
    }
    else {
        CSErrorLog("unknown color format");
        abort();
    }

    Buffer colorBuffer = {};
    colorBuffer.attachment = CSFramebufferAttachmentColor0;
    colorBuffer.renderbuffer = new CSRenderBuffer(width, height, colorFormat, samples, true);
    _buffers.addObject(colorBuffer);

    switch (desc.depthBit) {
        case 32:
            if (desc.stencilBit == 8) {
                Buffer depthStencilBuffer = {};
                depthStencilBuffer.attachment = CSFramebufferAttachmentDepthStencil;
                depthStencilBuffer.renderbuffer = new CSRenderBuffer(width, height, CSRawFormat::Depth32fStencil8, samples, true);
                _buffers.addObject(depthStencilBuffer);
            }
            else if (desc.stencilBit == 0) {
                Buffer depthBuffer = {};
                depthBuffer.attachment = CSFramebufferAttachmentDepth;
                depthBuffer.renderbuffer = new CSRenderBuffer(width, height, CSRawFormat::DepthComponent32f, samples, true);
                _buffers.addObject(depthBuffer);
            }
            else {
                CSErrorLog("unknown depth stencil format");
                abort();
            }
            break;
        case 24:
            if (desc.stencilBit == 8) {
                Buffer depthStencilBuffer = {};
                depthStencilBuffer.attachment = CSFramebufferAttachmentDepthStencil;
                depthStencilBuffer.renderbuffer = new CSRenderBuffer(width, height, CSRawFormat::Depth24Stencil8, samples, true);
                _buffers.addObject(depthStencilBuffer);
            }
            else if (desc.stencilBit == 0) {
                Buffer depthBuffer = {};
                depthBuffer.attachment = CSFramebufferAttachmentDepth;
                depthBuffer.renderbuffer = new CSRenderBuffer(width, height, CSRawFormat::DepthComponent24, samples, true);
                _buffers.addObject(depthBuffer);
            }
            else {
                CSErrorLog("unknown depth stencil format");
                abort();
            }
            break;
        case 16:
            if (desc.stencilBit == 0) {
                Buffer depthBuffer = {};
                depthBuffer.attachment = CSFramebufferAttachmentDepth;
                depthBuffer.renderbuffer = new CSRenderBuffer(width, height, CSRawFormat::DepthComponent16, samples, true);
                _buffers.addObject(depthBuffer);
            }
            else {
                CSErrorLog("unknown depth stencil format");
                abort();
            }
            break;
        case 0:
            break;
        default:
            CSErrorLog("unknown depth stencil format");
            abort();
            break;
    }
}

CSRenderTarget::CSRenderTarget(const CSRenderTargetDescription& desc) :
    _width(desc.width),
    _height(desc.height),
    _systemBuffer(false),
    _viewport{ CSBounds2(0, 0, _width, _height), CSBounds2(0, 0, _width, _height) }
{
    _description = desc;
    _description.validate();

    if (_description.attachments.count()) {
        foreach (const CSRenderTargetDescription::Attachment&, a, &_description.attachments) {
            CSFramebufferAttachment attachment = a.attachment;

            Buffer& buffer = _buffers.addObject();
            buffer.attachment = attachment;
            buffer.own = true;

            if (a.texture) {
                CSTextureDescription textureDesc;
                textureDesc.width = _width;
                textureDesc.height = _height;
                textureDesc.samples = a.samples;
                textureDesc.format = a.format;
                textureDesc.target = a.textureTarget;
                textureDesc.wrapS = a.textureWrapS;
                textureDesc.wrapT = a.textureWrapT;
                textureDesc.wrapR = a.textureWrapR;
                textureDesc.minFilter = a.textureMinFilter;
                textureDesc.magFilter = a.textureMagFilter;
                textureDesc.borderColor = a.textureBorderColor;

                buffer.renderbuffer = NULL;
                buffer.texture = new CSTexture(textureDesc);
                buffer.textureLayer = a.textureLayer;
            }
            else {
                buffer.renderbuffer = new CSRenderBuffer(_width, _height, a.format, a.samples);
                buffer.texture = NULL;
                buffer.textureLayer = 0;
            }
            if (attachment == CSFramebufferAttachmentColor1) _bloomSupported = true;
        }

        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
            _framebuffer = api->genFramebuffer();
            api->bindFramebuffer(CSFramebufferTargetFramebuffer, _framebuffer);
            foreach (const Buffer&, buffer, &_buffers) {
                if (buffer.renderbuffer) api->framebufferRenderbuffer(CSFramebufferTargetFramebuffer, buffer.attachment, buffer.renderbuffer->object());
                else api->framebufferTexture(CSFramebufferTargetFramebuffer, buffer.attachment, buffer.texture->object(), buffer.textureLayer);
            }
            api->setCurrentTarget(NULL);
        }, this);

        if (command) {
            command->addFence(this, CSGBatchFlagReadWrite);
            foreach (const Buffer&, buffer, &_buffers) command->addFence(buffer.resource(), CSGBatchFlagRead);
        }
    }
    else {
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) { _framebuffer = api->genFramebuffer(); }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }
}

CSRenderTarget::~CSRenderTarget() {
    if (!_systemBuffer) {
        int framebuffer = _framebuffer;

        CSGraphicsContext::sharedContext()->invoke(false, [framebuffer](CSGraphicsApi* api) {
            if (framebuffer) api->deleteFramebuffer(framebuffer);
        });
    }

    CSGraphicsContext::sharedContext()->clearTargets(this);
}

int CSRenderTarget::resourceCost() const {
    int cost = sizeof(CSRenderTarget) + _buffers.capacity() * sizeof(Buffer);
    foreach (const Buffer&, buffer, &_buffers) {
        if (buffer.own) {
            if (buffer.renderbuffer) cost += buffer.renderbuffer->resourceCost();
            else cost += buffer.texture->resourceCost();
        }
    }
    return cost;
}

int CSRenderTarget::samples() const {
    foreach (const Buffer&, buffer, &_buffers) {
        return buffer.renderbuffer ? buffer.renderbuffer->samples() : buffer.texture->samples();
    }
    return 0;
}

void CSRenderTarget::attachImpl(CSFramebufferAttachment attachment, CSRenderBuffer* renderbuffer, bool own) {
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, attachment, renderbuffer](CSGraphicsApi* api) {
        focus(api);
        api->framebufferRenderbuffer(CSFramebufferTargetFramebuffer, attachment, renderbuffer->object());
    }, this, renderbuffer);

    if (command) {
        command->addFence(renderbuffer, CSGBatchFlagRead);
        command->addFence(this, CSGBatchFlagReadWrite);
    }

    renderbuffer->retain();

    Buffer& buffer = _buffers.addObject();
    buffer.attachment = attachment;
    buffer.renderbuffer = renderbuffer;
    buffer.texture = NULL;
    buffer.textureLayer = 0;
    buffer.own = own;

    if (attachment == CSFramebufferAttachmentColor1) _bloomSupported = true;

    _descriptionUpdated = true;
}

void CSRenderTarget::attachImpl(CSFramebufferAttachment attachment, CSTexture* texture, int layer, bool own) {
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, attachment, texture, layer](CSGraphicsApi* api) {
        focus(api);
        api->framebufferTexture(CSFramebufferTargetFramebuffer, attachment, texture->object(), layer);
    }, this, texture);

    if (command) {
        command->addFence(texture, CSGBatchFlagRead);
        command->addFence(this, CSGBatchFlagReadWrite);
    }

    texture->retain();

    Buffer& buffer = _buffers.addObject();
    buffer.attachment = attachment;
    buffer.renderbuffer = NULL;
    buffer.texture = texture;
    buffer.textureLayer = layer;
    buffer.own = own;

    if (attachment == CSFramebufferAttachmentColor1) _bloomSupported = false;

    _descriptionUpdated = true;
}

void CSRenderTarget::detachImpl(CSFramebufferAttachment attachment) {
    for (int i = 0; i < _buffers.count(); i++) {
        Buffer& buffer = _buffers.objectAtIndex(i);

        if (buffer.attachment == attachment) {
            CSDelegateRenderCommand* command;

            if (buffer.renderbuffer) {
                command = CSGraphicsContext::sharedContext()->invoke(true, [this, attachment](CSGraphicsApi* api) {
                    focus(api);
                    api->framebufferRenderbuffer(CSFramebufferTargetFramebuffer, attachment, 0);
                }, this);
            }
            else
            {
                command = CSGraphicsContext::sharedContext()->invoke(true, [this, attachment](CSGraphicsApi* api) {
                    focus(api);
                    api->framebufferTexture(CSFramebufferTargetFramebuffer, attachment, 0, 0);
                }, this);
            }
            if (command) command->addFence(this, CSGBatchFlagReadWrite);

            _buffers.removeObjectAtIndex(i);
            break;
        }
    }
    
    _descriptionUpdated = true;
}

void CSRenderTarget::attach(CSFramebufferAttachment attachment, CSRenderBuffer* renderbuffer, bool own) {
    CSAssert(!_systemBuffer, "can not attach to system buffer");
    CSAssert(renderbuffer->width() == _width && renderbuffer->height() == _height, "can not attach different size buffer");
    CSAssert(buffer(attachment) == NULL, "already attached");

    attachImpl(attachment, renderbuffer, own);
}

void CSRenderTarget::attach(CSFramebufferAttachment attachment, CSTexture* texture, int level, bool own) {
    CSAssert(!_systemBuffer, "can not attach to system buffer");
    CSAssert(texture->width() == _width && texture->height() == _height, "can not attach different size buffer");
    CSAssert(buffer(attachment) == NULL, "already attached");

    attachImpl(attachment, texture, level, own);
}

void CSRenderTarget::detach(CSFramebufferAttachment attachment) {
    CSAssert(!_systemBuffer, "can not detach from system buffer");

    detachImpl(attachment);
}

const CSRenderTargetDescription& CSRenderTarget::description() const {
    if (_descriptionUpdated) {
        _description.width = _width;
        _description.height = _height;

        foreach (const Buffer&, buffer, &_buffers) {
            CSRenderTargetDescription::Attachment a;
            a.attachment = buffer.attachment;
            if (buffer.renderbuffer) {
                a.format = buffer.renderbuffer->format();
                a.samples = buffer.renderbuffer->samples();
            }
            else {
                const CSTextureDescription& textureDesc = buffer.texture->description();

                a.texture = true;
                a.format = textureDesc.format;
                a.samples = textureDesc.samples;
                a.textureTarget = textureDesc.target;
                a.textureWrapS = textureDesc.wrapS;
                a.textureWrapT = textureDesc.wrapT;
                a.textureWrapR = textureDesc.wrapR;
                a.textureMinFilter = textureDesc.minFilter;
                a.textureMagFilter = textureDesc.magFilter;
                a.textureBorderColor = textureDesc.borderColor;
                a.textureLayer = buffer.textureLayer;
            }
            _description.attachments.addObject(a);
        }
        _descriptionUpdated = false;
    }
    return _description;
}

const CSRenderTarget::Buffer* CSRenderTarget::buffer(CSFramebufferAttachment attachment) const {
    foreach (const Buffer&, buffer, &_buffers) {
        if (buffer.attachment == attachment) return &buffer;
    }
    return NULL;
}

CSTexture* CSRenderTarget::textureAttachment(CSFramebufferAttachment attachment) {
    const Buffer* buffer = this->buffer(attachment);
    return buffer ? buffer->texture : NULL;
}

CSRenderBuffer* CSRenderTarget::renderbufferAttachment(CSFramebufferAttachment attachment) {
    const Buffer* buffer = this->buffer(attachment);
    return buffer ? buffer->renderbuffer : NULL;
}

CSRawFormat CSRenderTarget::format(CSFramebufferAttachment attachment) const {
    const Buffer* buffer = this->buffer(attachment);
    if (buffer) {
        return buffer->renderbuffer ? buffer->renderbuffer->format() : buffer->texture->format();
    }
    return CSRawFormat::None;
}

bool CSRenderTarget::batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const {
    bool result = CSGraphicsResource::batch(reads, writes, flags);
    if (flags & CSGBatchFlagRetrieve) {
        foreach (const Buffer&, buffer, &_buffers) result &= buffer.resource()->batch(reads, writes, flags);
    }
    return result;
}

void CSRenderTarget::clearColor(int layer, const CSColor& color) {
    CSFramebufferAttachment attachment = (CSFramebufferAttachment)(CSFramebufferAttachmentColor0 + layer);
    CSAssert(format(attachment) != CSRawFormat::None);
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, layer, color = CSColor(color)](CSGraphicsApi* api) {
        focus(api);
        api->clearBufferColor(layer, color);
    }, this);

    if (command) command->addFence(buffer(attachment)->resource(), CSGBatchFlagReadWrite);
}

void CSRenderTarget::clearDepthStencil() {
    CSAssert(format(CSFramebufferAttachmentDepthStencil) != CSRawFormat::None);
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        focus(api);
        api->clearBufferDepthStencil();
    }, this);

    if (command) command->addFence(buffer(CSFramebufferAttachmentDepthStencil)->resource(), CSGBatchFlagReadWrite);
}

void CSRenderTarget::clearDepth() {
    CSAssert(format(CSFramebufferAttachmentDepth) != CSRawFormat::None);
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        focus(api);
        api->clearBufferDepth();
    }, this);

    if (command) command->addFence(buffer(CSFramebufferAttachmentDepth)->resource(), CSGBatchFlagReadWrite);
}

void CSRenderTarget::setDrawBuffers(CSGraphicsApi* api, int count, ...) {
    CSAssert(count <= countof(_drawBuffers) && api->currentTarget() == this);
    byte bufs[countof(_drawBuffers)];
    va_list ap;
    va_start(ap, count);
    for (int i = 0; i < count; i++) bufs[i] = va_arg(ap, int);
    va_end(ap);

    if (_drawBufferCount != count || memcmp(_drawBuffers, bufs, count) != 0) {
        _drawBufferCount = count;
        memcpy(_drawBuffers, bufs, count);
        api->drawBuffers(bufs, count);
    }
}

void CSRenderTarget::setDrawBuffer(CSGraphicsApi* api, int buf) {
    CSAssert(api->currentTarget() == this);

    if (_drawBufferCount != 1 || _drawBuffers[0] != buf) {
        _drawBufferCount = 1;
        _drawBuffers[0] = buf;
        api->drawBuffer(buf);
    }
}

const CSBounds2& CSRenderTarget::viewport() const {
    return _viewport[CSGraphicsContext::sharedContext()->isRenderThread()];
}

void CSRenderTarget::setViewport(CSBounds2 viewport) {
    if (viewport.x > _width) viewport.x = _width;
    if (viewport.y > _height) viewport.y = _height;
    if (viewport.x + viewport.width > _width) viewport.width = _width - viewport.x;
    if (viewport.y + viewport.height > _height) viewport.height = _height - viewport.y;

    if (viewport != _viewport[0]) {
        _viewport[0] = viewport;

        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, viewport](CSGraphicsApi* api) {
            if (api->currentTarget() == this) api->viewport(viewport);
            _viewport[1] = viewport;
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }
}

void CSRenderTarget::clearViewport() {
    setViewport(CSBounds2(0, 0, _width, _height));
}

bool CSRenderTarget::hasViewport() const {
    const CSBounds2& viewport = this->viewport();

    return viewport.x != 0 || viewport.y != 0 || viewport.width != _width || viewport.height != _height;
}

void CSRenderTarget::resize(int width, int height) {
    _description.width = _width = width;
    _description.height = _height = height;

    foreach (const Buffer&, buffer, &_buffers) {
        if (buffer.own) {
            if (buffer.renderbuffer) buffer.renderbuffer->resize(width, height);
            else buffer.texture->resize(width, height);
        }
    }

    clearViewport();
}

void CSRenderTarget::focus(CSGraphicsApi* api) {
    if (api->currentTarget() != this) {
        api->setCurrentTarget(this);
        api->viewport(_viewport[1]);
        api->bindFramebuffer(CSFramebufferTargetFramebuffer, _framebuffer);
    }
}

CSTexture* CSRenderTarget::captureColor(int buf, CSBounds2 bounds, bool temporary) const {
    bounds.intersect(viewport());

    if (bounds.width <= 0 || bounds.height <= 0) return NULL;

    CSFramebufferAttachment attachment = (CSFramebufferAttachment)(CSFramebufferAttachmentColor0 + buf);

    CSRawFormat format = this->format(attachment);

    if (format == CSRawFormat::None) return NULL;

    CSTextureDescription textureDesc;
    textureDesc.width = bounds.width;
    textureDesc.height = bounds.height;
    textureDesc.format = format;

    CSTexture* texture = temporary ? CSTextures::getTemporary(textureDesc, false) : CSTexture::texture(textureDesc, false);

    CSDelegateRenderCommand* command;

    if (samples() > 1) {
        command = CSGraphicsContext::sharedContext()->invoke(true, [this, buf, attachment, bounds, texture](CSGraphicsApi* api) {
            texture->allocate();

            CSRenderTargetDescription sampleDesc;
            sampleDesc.width = bounds.width;
            sampleDesc.height = bounds.height;

            CSRenderTarget* sampleTarget = CSRenderTargets::getTemporary(sampleDesc);
            sampleTarget->attach(attachment, texture, 0, false);
            blit(sampleTarget, bounds, CSBounds2(0, 0, bounds.width, bounds.height), CSClearBufferColor, CSBlitFramebufferFilterNearest, buf, buf);
            sampleTarget->detach(attachment);
            CSResourcePool::sharedPool()->remove(sampleTarget);
        }, this, texture);
        
    }
    else {
        command = CSGraphicsContext::sharedContext()->invoke(true, [this, buf, bounds, texture](CSGraphicsApi* api) {
            api->readBuffer(buf);
            api->bindTexture(CSTextureTarget2D, texture->object());
            api->bindFramebuffer(CSFramebufferTargetFramebuffer, _framebuffer);
            if (api->copyTexImage2D(CSTextureTarget2D, 0, texture->format(), bounds)) {
                CSErrorLog("copy tex image 2d fail");
            }
            if (api->currentTarget() != this) api->setCurrentTarget(NULL);
        }, this, texture);
    }

    if (command) {
        command->addFence(buffer(attachment)->resource(), CSGBatchFlagRead);
        command->addFence(texture, CSGBatchFlagReadWrite);
    }

    return texture;
}

CSTexture* CSRenderTarget::captureDepth(CSBounds2 bounds, bool temporary) const {
    bounds.intersect(viewport());

    if (bounds.width <= 0 || bounds.height <= 0) return NULL;

    CSFramebufferAttachment attachment = CSFramebufferAttachmentDepthStencil;

    CSRawFormat format = this->format(attachment);

    if (format == CSRawFormat::None) {
        attachment = CSFramebufferAttachmentDepth;

        format = this->format(attachment);

        if (format == CSRawFormat::None) return NULL;
    }

    CSTextureDescription textureDesc;
    textureDesc.width = bounds.width;
    textureDesc.height = bounds.height;
    textureDesc.format = format;

    CSTexture* texture = temporary ? CSTextures::getTemporary(textureDesc, false) : CSTexture::texture(textureDesc, false);

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, attachment, bounds, texture](CSGraphicsApi* api) {          //TODO:멀티샘플 동작하나?
        texture->allocate();

        CSRenderTargetDescription sampleDesc;
        sampleDesc.width = bounds.width;
        sampleDesc.height = bounds.height;

        CSRenderTarget* sampleTarget = CSRenderTargets::getTemporary(sampleDesc);
        sampleTarget->attach(attachment, texture, 0, false);
        blit(sampleTarget, bounds, CSBounds2(0, 0, bounds.width, bounds.height), CSClearBufferDepth, CSBlitFramebufferFilterNearest);
        sampleTarget->detach(attachment);
        CSResourcePool::sharedPool()->remove(sampleTarget);
    }, this, texture);

    if (command) {
        command->addFence(buffer(attachment)->resource(), CSGBatchFlagRead);
        command->addFence(texture, CSGBatchFlagReadWrite);
    }

    return texture;
}

void CSRenderTarget::blit(CSRenderTarget* target, CSBounds2 srcbounds, CSBounds2 destbounds, CSClearBufferMask mask, CSBlitFramebufferFilter filter, int srcbuf, int destbuf) const {
    srcbounds.intersect(viewport());
    destbounds.intersect(target->viewport());

    if (srcbounds.width <= 0 || srcbounds.height <= 0 || destbounds.width <= 0 || destbounds.height <= 0) return;

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, target, srcbounds, destbounds, mask, filter, srcbuf, destbuf](CSGraphicsApi* api) {
        api->applyScissor(CSBounds2::Zero);

        api->bindFramebuffer(CSFramebufferTargetReadFramebuffer, _framebuffer);
        api->bindFramebuffer(CSFramebufferTargetDrawFramebuffer, target->framebuffer());
        api->readBuffer(srcbuf);
        api->drawBuffer(destbuf);

        api->blitFramebuffer(srcbounds, destbounds, mask, filter);

        api->bindFramebuffer(CSFramebufferTargetFramebuffer, _framebuffer);

        _drawBufferCount = 1;
        _drawBuffers[0] = destbuf;

        if (api->currentTarget() != this) api->setCurrentTarget(NULL);
    }, this, target);

    if (command) {
        command->addFence(this, CSGBatchFlagRead | CSGBatchFlagRetrieve);
        command->addFence(target, CSGBatchFlagReadWrite);
    }
}

bool CSRenderTarget::screenshotImpl(CSGraphicsApi* api, const char* path) const {
    const CSBounds2& viewport = _viewport[1];

    if (viewport.width <= 0 || viewport.height <= 0) return false;

    const Buffer* buffer = this->buffer(CSFramebufferAttachmentColor0);
    if (!buffer) {
        CSErrorLog("no color attachment");
        return false;
    }
    CSRawFormat format = buffer->texture ? buffer->texture->format() : buffer->renderbuffer->format();
    if (format != CSRawFormat::Rgba8) {
        CSErrorLog("not implement format:%d", format);
        return false;
    }
    uint* originPixels = (uint*)malloc(viewport.width * viewport.height * 4);
    if (!originPixels) {
        CSErrorLog("not enough memory");
        return false;
    }
    uint* platformPixels = (uint*)malloc(viewport.width * viewport.height * 4);
    if (!platformPixels) {
        free(originPixels);
        CSErrorLog("not enough memory");
        return false;
    }

    api->bindFramebuffer(CSFramebufferTargetReadFramebuffer, _framebuffer);
    const CSRawFormatEncoding& encoding = format.encoding();
    api->readPixels(viewport, encoding.pixelFormat, encoding.pixelType, originPixels);

    int offset1, offset2;
    for (int i = 0; i < _height; i++) {
        offset1 = i * viewport.width;
        offset2 = (viewport.height - i - 1) * viewport.width;
        for (int j = 0; j < viewport.width; j++) {
            uint pixel = originPixels[offset1 + j];
#ifdef CDK_ANDROID      //argb source for android
            uint blue = (pixel >> 16) & 0xff;
            uint red = (pixel << 16) & 0x00ff0000;
            pixel = (pixel & 0xff00ff00) | red | blue;
#endif
            platformPixels[offset2 + j] = pixel;
        }
    }
    free(originPixels);
    bool result = CSRaster::saveBitmapWithRaw(path, platformPixels, viewport.width, viewport.height);
    free(platformPixels);
    return result;
}

bool CSRenderTarget::screenshot(const char* path) const {
    bool result = false;

    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        result = screenshotImpl(api, path);
    }
    else {
        synchronized(this) {
            CSGraphicsContext::sharedContext()->invoke(false, [this, path, &result](CSGraphicsApi* papi) {
                result = screenshotImpl(papi, path);
                synchronized(this) {
                    pulse();
                }
            });
            wait();
        }
    }
    return result;
}