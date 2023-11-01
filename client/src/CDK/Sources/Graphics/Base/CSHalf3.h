#ifndef __CDK__CSHalf3__
#define __CDK__CSHalf3__

#include "CSHalf2.h"

#include "CSColor3.h"

class CSBuffer;

struct CSHalf3 {
    half x, y, z;

    static const CSHalf3 Zero;
    static const CSHalf3 UnitX;
    static const CSHalf3 UnitY;
    static const CSHalf3 UnitZ;
    static const CSHalf3 One;

    CSHalf3() = default;
    inline explicit CSHalf3(half v) : x(v), y(v), z(v) {}
    inline CSHalf3(half x, half y, half z) : x(x), y(y), z(z) {}
    inline CSHalf3(const CSHalf2& v, half z = 0) : x(v.x), y(v.y), z(z) {}
    inline CSHalf3(const CSVector3& v) : x(v.x), y(v.y), z(v.z) {}
    inline CSHalf3(const CSColor3& v) : x(v.r), y(v.g), z(v.b) {}

    explicit CSHalf3(CSBuffer* buffer);
    explicit CSHalf3(const byte*& bytes);

    inline explicit operator CSHalf2() const {
        return CSHalf2(x, y);
    }

    inline operator CSVector3() const {
        return CSVector3(x, y, z);
    }

    inline operator CSColor3() const {
        return CSColor3(x, y, z);
    }

    uint hash() const;

    inline bool operator ==(const CSHalf3& other) const {
        return x == other.x && y == other.y && z == other.z;
    }

    inline bool operator !=(const CSHalf3& other) const {
        return !(*this == other);
    }
};

#endif
