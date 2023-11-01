#define CDK_IMPL

#include "CSFixed4.h"

#include "CSBuffer.h"

const CSFixed4 CSFixed4::Zero(fixed::Zero, fixed::Zero, fixed::Zero, fixed::Zero);
const CSFixed4 CSFixed4::UnitX(fixed::One, fixed::Zero, fixed::Zero, fixed::Zero);
const CSFixed4 CSFixed4::UnitY(fixed::Zero, fixed::One, fixed::Zero, fixed::Zero);
const CSFixed4 CSFixed4::UnitZ(fixed::Zero, fixed::Zero, fixed::One, fixed::Zero);
const CSFixed4 CSFixed4::UnitW(fixed::Zero, fixed::Zero, fixed::Zero, fixed::One);
const CSFixed4 CSFixed4::One(fixed::One, fixed::One, fixed::One, fixed::One);

CSFixed4::CSFixed4(CSBuffer* buffer) :
    x(buffer->readFixed()),
    y(buffer->readFixed()),
    z(buffer->readFixed()),
    w(buffer->readFixed()) 
{
}

CSFixed4::CSFixed4(const byte*& bytes) :
    x(readFixed(bytes)),
    y(readFixed(bytes)),
    z(readFixed(bytes)),
    w(readFixed(bytes)) 
{
}

void CSFixed4::normalize() {
    if (x || y || z || w) {
        x <<= 8;        //for accuration
        y <<= 8;
        z <<= 8;
        w <<= 8;
        fixed length = this->length();
        if (length) operator /=(length);
    }
}

uint CSFixed4::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    hash.combine(w);
    return hash;
}