#define CDK_IMPL

#include "CSRenderBuffer.h"

#include "CSGraphicsContext.h"

CSRenderBuffer::CSRenderBuffer(int width, int height, CSRawFormat format, int samples, bool systemBuffer) : _width(width), _height(height), _format(format), _samples(samples), _systemBuffer(systemBuffer) {
	CSAssert(!format.encoding().compressed);

	if (!systemBuffer) create();
}

CSRenderBuffer::CSRenderBuffer(const CSRenderBufferDescription& desc) : _systemBuffer(false) {
    desc.validate();

    _width = desc.width;
    _height = desc.height;
    _format = desc.format;
    _samples = desc.samples;

    create();
}

CSRenderBuffer::~CSRenderBuffer() {
    int object = _object;

    if (object && !_systemBuffer) {
        CSGraphicsContext::sharedContext()->invoke(false , [object](CSGraphicsApi* api) { api->deleteRenderbuffer(object); });
    }
}

void CSRenderBuffer::create() {
    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        _object = api->genRenderbuffer();
        api->bindRenderbuffer(_object);
        if (api->renderbufferStorage(_format, _samples, _width, _height)) {
            CSErrorLog("renderbuffer storage fail");
        }
    }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);
}

int CSRenderBuffer::resourceCost() const {
    return sizeof(CSRenderBuffer) + _width * _height * _format.encoding().pixelBpp;
}

CSRenderBufferDescription CSRenderBuffer::description() const {
    CSRenderBufferDescription desc;
    desc.width = _width;
    desc.height = _height;
    desc.format = _format;
    desc.samples = _samples;
    return desc;
}

void CSRenderBuffer::resize(int width, int height) {
    _width = width;
    _height = height;

    if (_object && !_systemBuffer) {
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, width, height](CSGraphicsApi* api) {
            api->bindRenderbuffer(_object);
            if (!api->renderbufferStorage(_format, _samples, width, height)) {
                CSErrorLog("renderbuffer storage fail");
            }
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }
}
