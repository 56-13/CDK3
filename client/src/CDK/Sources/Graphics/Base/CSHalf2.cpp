#define CDK_IMPL

#include "CSHalf2.h"

#include "CSBuffer.h"

const CSHalf2 CSHalf2::Zero(0, 0);
const CSHalf2 CSHalf2::UnitX(1, 0);
const CSHalf2 CSHalf2::UnitY(0, 1);
const CSHalf2 CSHalf2::One(1, 1);

CSHalf2::CSHalf2(CSBuffer* buffer) : 
    x(buffer->readHalf()), 
    y(buffer->readHalf()) 
{
}

CSHalf2::CSHalf2(const byte*& bytes) : 
    x(readHalf(bytes)), 
    y(readHalf(bytes)) 
{
}

uint CSHalf2::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    return hash;
}