#define CDK_IMPL

#include "CSInt3.h"

#include "CSBuffer.h"

CSInt3::CSInt3(CSBuffer* buffer) :
    x(buffer->readInt()),
    y(buffer->readInt()),
    z(buffer->readInt()) 
{
}

CSInt3::CSInt3(const byte*& bytes) :
    x(readInt(bytes)),
    y(readInt(bytes)),
    z(readInt(bytes)) 
{
}

uint CSInt3::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    return hash;
}