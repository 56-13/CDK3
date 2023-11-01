#define CDK_IMPL

#include "CSVertexLayout.h"

CSVertexLayout::CSVertexLayout(CSGBuffer* buffer, int bufferIndex, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault) :
    buffer(buffer),
    bufferIndex(bufferIndex),
    attrib(attrib),
    size(size),
    type(type),
    normalized(normalized),
    stride(stride),
    offset(offset),
    divisor(divisor),
    enabledByDefault(enabledByDefault)
{
    
}

CSVertexLayout::CSVertexLayout(CSGBuffer* buffer, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault) :
    CSVertexLayout(buffer, -1, attrib, size, type, normalized, stride, offset, divisor, enabledByDefault)
{
    CSAssert(buffer->target() == CSGBufferTargetArray);
}

CSVertexLayout::CSVertexLayout(int bufferIndex, int attrib, int size, CSVertexAttribType type, bool normalized, int stride, int offset, int divisor, bool enabledByDefault) :
    CSVertexLayout(NULL, bufferIndex, attrib, size, type, normalized, stride, offset, divisor, enabledByDefault)
{

}

bool CSVertexLayout::operator ==(const CSVertexLayout& other) const {
    return buffer == other.buffer &&
        bufferIndex == other.bufferIndex &&
        attrib == other.attrib &&
        size == other.size &&
        type == other.type &&
        normalized == other.normalized &&
        stride == other.stride &&
        offset == other.offset &&
        divisor == other.divisor &&
        enabledByDefault == other.enabledByDefault;
}

uint CSVertexLayout::hash() const {
    CSHash hash;
    hash.combine(buffer ? buffer->hash() : bufferIndex);
    hash.combine(attrib);
    hash.combine(size);
    hash.combine(type);
    hash.combine(normalized);
    hash.combine(stride);
    hash.combine(offset);
    hash.combine(divisor);
    hash.combine(enabledByDefault);
    return hash;
}
