#define CDK_IMPL

#include "CSFixed3.h"

#include "CSBuffer.h"

const CSFixed3 CSFixed3::Zero(fixed::Zero, fixed::Zero, fixed::Zero);
const CSFixed3 CSFixed3::UnitX(fixed::One, fixed::Zero, fixed::Zero);
const CSFixed3 CSFixed3::UnitY(fixed::Zero, fixed::One, fixed::Zero);
const CSFixed3 CSFixed3::UnitZ(fixed::Zero, fixed::Zero, fixed::One);
const CSFixed3 CSFixed3::One(fixed::One, fixed::One, fixed::One);

CSFixed3::CSFixed3(CSBuffer* buffer) :
    x(buffer->readFixed()),
    y(buffer->readFixed()),
    z(buffer->readFixed()) 
{
}

CSFixed3::CSFixed3(const byte*& bytes) :
    x(readFixed(bytes)),
    y(readFixed(bytes)),
    z(readFixed(bytes)) 
{
}

void CSFixed3::normalize() {
    if (x || y || z) {
        x <<= 8;        //for accuration
        y <<= 8;
        z <<= 8;
        fixed length = this->length();
        if (length) operator /=(length);
    }
}

uint CSFixed3::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    return hash;
}