#define CDK_IMPL

#include "CSFixed2.h"

#include "CSBuffer.h"

const CSFixed2 CSFixed2::Zero(fixed::Zero, fixed::Zero);
const CSFixed2 CSFixed2::UnitX(fixed::One, fixed::Zero);
const CSFixed2 CSFixed2::UnitY(fixed::Zero, fixed::One);
const CSFixed2 CSFixed2::One(fixed::One, fixed::One);

CSFixed2::CSFixed2(CSBuffer* buffer) :
    x(buffer->readFixed()),
    y(buffer->readFixed()) 
{
}

CSFixed2::CSFixed2(const byte*& bytes) :
    x(readFixed(bytes)),
    y(readFixed(bytes)) 
{
}

void CSFixed2::normalize() {
    if (x || y) {
        x <<= 8;        //for accuration
        y <<= 8;
        fixed length = this->length();
        if (length) operator /=(length);
    }
}

fixed CSFixed2::distanceSquaredToSegment(const CSFixed2& p0, const CSFixed2& p1, CSFixed2* np) const {
    CSFixed2 v = p1 - p0;

    fixed len2 = v.lengthSquared();

    if (!len2) {
        if (np) *np = p0;
        return distanceSquared(p0);
    }

    CSFixed2 pv = *this - p0;

    fixed t = (pv.x * v.x + pv.y * v.y) / len2;

    CSFixed2 p2;
    if (t > fixed::One) {
        p2 = p1;
    }
    else if (t > fixed::Zero) {
        p2 = p0 + v * t;
    }
    else {
        p2 = p0;
    }
    if (np) *np = p2;
    return distanceSquared(p2);
}

bool CSFixed2::intersectsSegment(const CSFixed2& p0, const CSFixed2& p1, const CSFixed2& p2, const CSFixed2& p3, CSFixed2* cp) {
    //=========================================================================================================
    //optimization code
    fixed x0 = p0.x < p1.x ? p0.x : p1.x;
    fixed x1 = p0.x > p1.x ? p0.x : p1.x;
    fixed x2 = p2.x < p3.x ? p2.x : p3.x;
    fixed x3 = p2.x > p3.x ? p2.x : p3.x;
    if ((x0 > x2 ? x0 : x2) > (x1 < x3 ? x1 : x3)) {
        return false;
    }
    fixed y0 = p0.y < p1.y ? p0.y : p1.y;
    fixed y1 = p0.y > p1.y ? p0.y : p1.y;
    fixed y2 = p2.y < p3.y ? p2.y : p3.y;
    fixed y3 = p2.y > p3.y ? p2.y : p3.y;
    if ((y0 > y2 ? y0 : y2) > (y1 < y3 ? y1 : y3)) {
        return false;
    }
    //=========================================================================================================

    CSFixed2 v0 = p1 - p0;
    CSFixed2 v1 = p3 - p2;
    fixed delta = v1.x * v0.y - v1.y * v0.x;
    if (!delta) {
        return false;
    }
    CSFixed2 v2 = p2 - p0;
    fixed s = (v0.x * v2.y - v0.y * v2.x) / delta;
    if (s < fixed::Zero || s > fixed::One) {
        return false;
    }
    fixed t = (v1.x * v2.y - v1.y * v2.x) / delta;
    if (t < fixed::Zero || t > fixed::One) {
        return false;
    }
    if (cp) *cp = p0 + (p1 - p0) * t;
    return true;
}

bool CSFixed2::intersectsSegment(const CSFixed2& cp, const CSFixed2& p0, const CSFixed2& p1) const {
    CSFixed2 a = p0 - cp;
    CSFixed2 c = p1 - cp;
    CSFixed2 b = *this - cp;

    fixed at = CSMath::atan2(a.y, a.x);
    fixed bt = CSMath::atan2(b.y, b.x);
    fixed ct = CSMath::atan2(c.y, c.x);

    if (ct > at) {
        return bt > ct || bt < at;
    }
    else {
        return bt < at&& bt > ct;
    }
}

void CSFixed2::rotate(fixed a) {
    fixed cosq = CSMath::cos(a);
    fixed sinq = CSMath::sin(a);
    fixed rx = x * cosq - y * sinq;
    fixed ry = y * cosq + x * sinq;
    x = rx;
    y = ry;
}

uint CSFixed2::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    return hash;
}