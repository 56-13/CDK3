#ifndef __CDK__CSInt2__
#define __CDK__CSInt2__

#include "CSVector2.h"

class CSBuffer;

struct CSInt2 {
    int x, y;

    CSInt2() = default;
    inline explicit CSInt2(int v) : x(v), y(v) {}
    inline CSInt2(int x, int y) : x(x), y(y) {}
    inline CSInt2(const CSVector2& v) : x(v.x), y(v.y) {}

    explicit CSInt2(CSBuffer* buffer);
    explicit CSInt2(const byte*& bytes);

    static inline void clamp(const CSInt2& vector, const CSInt2& min, const CSInt2& max, CSInt2& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
    }
    static inline CSInt2 clamp(const CSInt2& vector, const CSInt2& min, const CSInt2& max) {
        CSInt2 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSInt2& min, const CSInt2& max) {
        clamp(*this, min, max, *this);
    }

    static inline void max(const CSInt2& left, const CSInt2 right, CSInt2& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
    }
    static inline CSInt2 max(const CSInt2& left, const CSInt2 right) {
        CSInt2 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSInt2& left, const CSInt2& right, CSInt2& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
    }
    static inline CSInt2 min(const CSInt2& left, const CSInt2 right) {
        CSInt2 result;
        min(left, right, result);
        return result;
    }

    inline CSInt2 operator +(const CSInt2& vector) const {
        return CSInt2(x + vector.x, y + vector.y);
    }

    template<typename T>
    inline CSInt2 operator +(T scalar) const {
        return CSInt2(x + scalar, y + scalar);
    }

    inline CSInt2 operator -(const CSInt2& vector) const {
        return CSInt2(x - vector.x, y - vector.y);
    }

    template<typename T>
    inline CSInt2 operator -(T scalar) const {
        return CSInt2(x - scalar, y - scalar);
    }

    inline CSInt2 operator *(const CSInt2& vector) const {
        return CSInt2(x * vector.x, y * vector.y);
    }

    template<typename T>
    inline CSInt2 operator *(T scale) const {
        return CSInt2(x * scale, y * scale);
    }

    inline CSInt2 operator /(const CSInt2& vector) const {
        return CSInt2(x / vector.x, y / vector.y);
    }

    template<typename T>
    inline CSInt2 operator /(T scale) const {
        return CSInt2(x / scale, y / scale);
    }

    inline CSInt2 operator -() const {
        return CSInt2(-x, -y);
    }

    inline CSInt2& operator +=(const CSInt2& vector) {
        x += vector.x;
        y += vector.y;
        return *this;
    }

    template<typename T>
    inline CSInt2& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        return *this;
    }

    inline CSInt2& operator -=(const CSInt2& vector) {
        x -= vector.x;
        y -= vector.y;
        return *this;
    }

    template<typename T>
    inline CSInt2& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        return *this;
    }

    inline CSInt2& operator *=(const CSInt2& vector) {
        x *= vector.x;
        y *= vector.y;
        return *this;
    }

    template<typename T>
    inline CSInt2& operator *=(T scale) {
        x *= scale;
        y *= scale;
        return *this;
    }

    inline CSInt2& operator /=(const CSInt2& vector) {
        x /= vector.x;
        y /= vector.y;
        return *this;
    }

    template<typename T>
    inline CSInt2& operator /=(T scale) {
        x /= scale;
        y /= scale;
        return *this;
    }

    inline operator CSVector2() const {
        return CSVector2(x, y);
    }

    uint hash() const;

    inline bool operator ==(const CSInt2& other) const {
        return x == other.x && y == other.y;
    }

    inline bool operator !=(const CSInt2& other) const {
        return !(*this == other);
    }
};

#endif
