#define CDK_IMPL

#include "CSInt2.h"

#include "CSBuffer.h"

CSInt2::CSInt2(CSBuffer* buffer) :
    x(buffer->readInt()),
    y(buffer->readInt()) 
{
}

CSInt2::CSInt2(const byte*& bytes) :
    x(readInt(bytes)),
    y(readInt(bytes)) 
{
}

uint CSInt2::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    return hash;
}