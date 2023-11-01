#ifndef __CDK__CSVertexLayout__
#define __CDK__CSVertexLayout__

#include "CSGBuffer.h"

struct CSVertexLayout {
public:
    CSPtr<CSGBuffer> buffer;
    sbyte bufferIndex;
    byte attrib;
    ushort size;
    CSVertexAttribType type;
    bool normalized;
    ushort stride;
    ushort offset;
    byte divisor;
    bool enabledByDefault;

    CSVertexLayout() = default;
private:
    CSVertexLayout(CSGBuffer* buffer, int bufferIndex, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault);
public:
    CSVertexLayout(CSGBuffer* buffer, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault);
    CSVertexLayout(int bufferIndex, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault);

    uint hash() const;

    bool operator ==(const CSVertexLayout& other) const;
    inline bool operator !=(const CSVertexLayout& other) const {
        return !(*this == other);
    }
};

#endif