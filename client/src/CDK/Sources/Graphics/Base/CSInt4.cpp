#define CDK_IMPL

#include "CSInt4.h"

#include "CSBuffer.h"

CSInt4::CSInt4(CSBuffer* buffer) :
    x(buffer->readInt()),
    y(buffer->readInt()),
    z(buffer->readInt()),
    w(buffer->readInt()) 
{
}

CSInt4::CSInt4(const byte*& bytes) :
    x(readInt(bytes)),
    y(readInt(bytes)),
    z(readInt(bytes)),
    w(readInt(bytes)) 
{
}

uint CSInt4::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    hash.combine(w);
    return hash;
}