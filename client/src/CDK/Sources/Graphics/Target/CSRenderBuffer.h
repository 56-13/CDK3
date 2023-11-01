#ifndef __CDK__CSRenderBuffer__
#define __CDK__CSRenderBuffer__

#include "CSGraphicsResource.h"

#include "CSRenderBufferDescription.h"

class CSRenderBuffer : public CSGraphicsResource {
private:
    int _object = 0;
    ushort _width;
    ushort _height;
    CSRawFormat _format;
    byte _samples;
    bool _systemBuffer;
public:
    CSRenderBuffer(int width, int height, CSRawFormat format, int samples = 1, bool systemBuffer = false);
    CSRenderBuffer(const CSRenderBufferDescription& desc);
private:
    ~CSRenderBuffer();
public:
    static inline CSRenderBuffer* buffer(int width, int height, CSRawFormat format, int samples = 1, bool systemBuffer = false) {
        return autorelease(new CSRenderBuffer(width, height, format, samples, systemBuffer));
    }
    static inline CSRenderBuffer* buffer(const CSRenderBufferDescription& desc) {
        return autorelease(new CSRenderBuffer(desc));
    }

    inline CSResourceType resourceType() const override {
        return CSResourceTypeRenderBuffer;
    }
    int resourceCost() const override;

    inline int object() const {
        return _object;
    }
    inline int width() const {
        return _width;
    }
    inline int height() const {
        return _height;
    }
    inline CSRawFormat format() const {
        return _format;
    }
    inline int samples() const {
        return _samples;
    }
    inline bool isSystemBuffer() const {
        return _systemBuffer;
    }
    CSRenderBufferDescription description() const;

    void resize(int width, int height);
private:
    void create();
};

#endif
