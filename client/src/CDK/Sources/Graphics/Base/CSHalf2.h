#ifndef __CDK__CSHalf2__
#define __CDK__CSHalf2__

#include "CSHalf.h"

#include "CSVector2.h"

class CSBuffer;

struct CSHalf2 {
    half x, y;

    static const CSHalf2 Zero;
    static const CSHalf2 UnitX;
    static const CSHalf2 UnitY;
    static const CSHalf2 One;

    CSHalf2() = default;
    inline explicit CSHalf2(half v) : x(v), y(v) {}
    inline CSHalf2(half x, half y) : x(x), y(y) {}
    inline CSHalf2(const CSVector2& v) : x(v.x), y(v.y) {}

    explicit CSHalf2(CSBuffer* buffer);
    explicit CSHalf2(const byte*& bytes);

    inline operator CSVector2() const {
        return CSVector2(x, y);
    }

    uint hash() const;

    inline bool operator ==(const CSHalf2& other) const {
        return x == other.x && y == other.y;
    }

    inline bool operator !=(const CSHalf2& other) const {
        return !(*this == other);
    }
};

#endif
