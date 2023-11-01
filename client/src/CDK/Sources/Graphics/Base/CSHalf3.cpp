#define CDK_IMPL

#include "CSHalf3.h"

#include "CSBuffer.h"

const CSHalf3 CSHalf3::Zero(0, 0, 0);
const CSHalf3 CSHalf3::UnitX(1, 0, 0);
const CSHalf3 CSHalf3::UnitY(0, 1, 0);
const CSHalf3 CSHalf3::UnitZ(0, 0, 1);
const CSHalf3 CSHalf3::One(1, 1, 1);

CSHalf3::CSHalf3(CSBuffer* buffer) :
    x(buffer->readHalf()),
    y(buffer->readHalf()),
    z(buffer->readHalf())
{
}

CSHalf3::CSHalf3(const byte*& bytes) :
    x(readHalf(bytes)),
    y(readHalf(bytes)),
    z(readHalf(bytes))
{
}

uint CSHalf3::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    return hash;
}