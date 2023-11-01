#define CDK_IMPL

#include "CSHalf4.h"

#include "CSBuffer.h"

const CSHalf4 CSHalf4::Zero(0, 0, 0, 0);
const CSHalf4 CSHalf4::UnitX(1, 0, 0, 0);
const CSHalf4 CSHalf4::UnitY(0, 1, 0, 0);
const CSHalf4 CSHalf4::UnitZ(0, 0, 1, 0);
const CSHalf4 CSHalf4::UnitW(0, 0, 0, 1);
const CSHalf4 CSHalf4::One(1, 1, 1, 1);

CSHalf4::CSHalf4(CSBuffer* buffer) :
    x(buffer->readHalf()),
    y(buffer->readHalf()),
    z(buffer->readHalf()),
    w(buffer->readHalf())
{
}

CSHalf4::CSHalf4(const byte*& bytes) :
    x(readHalf(bytes)),
    y(readHalf(bytes)),
    z(readHalf(bytes)),
    w(readHalf(bytes))
{
}

uint CSHalf4::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    hash.combine(w);
    return hash;
}