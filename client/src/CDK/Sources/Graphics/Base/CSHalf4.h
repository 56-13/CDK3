#ifndef __CDK__CSHalf4__
#define __CDK__CSHalf4__

#include "CSHalf3.h"

#include "CSColor.h"

class CSBuffer;

struct CSHalf4 {
    half x, y, z, w;

    static const CSHalf4 Zero;
    static const CSHalf4 UnitX;
    static const CSHalf4 UnitY;
    static const CSHalf4 UnitZ;
    static const CSHalf4 UnitW;
    static const CSHalf4 One;

    CSHalf4() = default;
    inline explicit CSHalf4(half v) : x(v), y(v), z(v), w(v) {}
    inline CSHalf4(half x, half y, half z, half w) : x(x), y(y), z(z), w(w) {}
    inline CSHalf4(const CSHalf2& v, half z, half w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSHalf4(const CSHalf3& v, half w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSHalf4(const CSVector4& v) : x(v.x), y(v.y), z(v.z), w(v.w) {}
    inline CSHalf4(const CSColor& v) : x(v.r), y(v.g), z(v.b), w(v.a) {}

    explicit CSHalf4(CSBuffer* buffer);
    explicit CSHalf4(const byte*& bytes);

    inline explicit operator CSHalf2() const {
        return CSHalf2(x, y);
    }

    inline explicit operator CSHalf3() const {
        return CSHalf3(x, y, z);
    }

    inline operator CSVector4() const {
        return CSVector4(x, y, z, w);
    }

    inline operator CSColor() const {
        return CSColor(x, y, z, w);
    }

    uint hash() const;

    inline bool operator ==(const CSHalf4& other) const {
        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    inline bool operator !=(const CSHalf4& other) const {
        return !(*this == other);
    }
};

#endif
