#ifndef __CDK__CSInt4__
#define __CDK__CSInt4__

#include "CSInt3.h"

#include "CSVector4.h"

class CSBuffer;

struct CSInt4 {
    int x, y, z, w;

    CSInt4() = default;
    inline explicit CSInt4(int v) : x(v), y(v), z(v), w(v) {}
    inline CSInt4(int x, int y, int z, int w) : x(x), y(y), z(z), w(w) {}
    inline CSInt4(const CSInt2& v, int z, int w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSInt4(const CSInt3& v, int w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSInt4(const CSVector4& v) : x(v.x), y(v.y), z(v.z), w(v.w) {}

    explicit CSInt4(CSBuffer* buffer);
    explicit CSInt4(const byte*& bytes);

    static inline void clamp(const CSInt4& vector, const CSInt4& min, const CSInt4& max, CSInt4& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
        result.z = CSMath::clamp(vector.z, min.z, max.z);
        result.w = CSMath::clamp(vector.w, min.w, max.w);
    }
    static inline CSInt4 clamp(const CSInt4& vector, const CSInt4& min, const CSInt4& max) {
        CSInt4 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSInt4& min, const CSInt4& max) {
        clamp(*this, min, max, *this);
    }

    static inline void max(const CSInt4& left, const CSInt4 right, CSInt4& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
        result.z = CSMath::max(left.z, right.z);
    }
    static inline CSInt4 max(const CSInt4& left, const CSInt4 right) {
        CSInt4 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSInt4& left, const CSInt4& right, CSInt4& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
        result.z = CSMath::min(left.z, right.z);
        result.w = CSMath::min(left.w, right.w);
    }
    static inline CSInt4 min(const CSInt4& left, const CSInt4 right) {
        CSInt4 result;
        min(left, right, result);
        return result;
    }

    inline CSInt4 operator +(const CSInt4& vector) const {
        return CSInt4(x + vector.x, y + vector.y, z + vector.z, w + vector.w);
    }

    template<typename T>
    inline CSInt4 operator +(T scalar) const {
        return CSInt4(x + scalar, y + scalar, z + scalar, w + scalar);
    }

    inline CSInt4 operator -(const CSInt4& vector) const {
        return CSInt4(x - vector.x, y - vector.y, z - vector.z, w - vector.w);
    }

    template<typename T>
    inline CSInt4 operator -(T scalar) const {
        return CSInt4(x - scalar, y - scalar, z - scalar, w - scalar);
    }

    inline CSInt4 operator *(const CSInt4& vector) const {
        return CSInt4(x * vector.x, y * vector.y, z + vector.z, w + vector.w);
    }

    template<typename T>
    inline CSInt4 operator *(T scale) const {
        return CSInt4(x * scale, y * scale, z * scale, w * scale);
    }

    inline CSInt4 operator /(const CSInt4& vector) const {
        return CSInt4(x / vector.x, y / vector.y, z / vector.z, w / vector.w);
    }

    template<typename T>
    inline CSInt4 operator /(T scale) const {
        return CSInt4(x / scale, y / scale, z / scale, w / scale);
    }

    inline CSInt4 operator -() const {
        return CSInt4(-x, -y, -z, -w);
    }

    inline CSInt4& operator +=(const CSInt4& vector) {
        x += vector.x;
        y += vector.y;
        z += vector.z;
        w += vector.w;
        return *this;
    }

    template<typename T>
    inline CSInt4& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        z += scalar;
        w += scalar;
        return *this;
    }

    inline CSInt4& operator -=(const CSInt4& vector) {
        x -= vector.x;
        y -= vector.y;
        z -= vector.z;
        w -= vector.w;
        return *this;
    }

    template<typename T>
    inline CSInt4& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        z -= scalar;
        w -= scalar;
        return *this;
    }

    inline CSInt4& operator *=(const CSInt4& vector) {
        x *= vector.x;
        y *= vector.y;
        z *= vector.z;
        w *= vector.w;
        return *this;
    }

    template<typename T>
    inline CSInt4& operator *=(T scale) {
        x *= scale;
        y *= scale;
        z *= scale;
        w *= scale;
        return *this;
    }

    inline CSInt4& operator /=(const CSInt4& vector) {
        x /= vector.x;
        y /= vector.y;
        z /= vector.z;
        w /= vector.w;
        return *this;
    }

    template<typename T>
    inline CSInt4& operator /=(T scale) {
        x /= scale;
        y /= scale;
        z /= scale;
        w /= scale;
        return *this;
    }

    inline explicit operator CSInt2() const {
        return CSInt2(x, y);
    }

    inline explicit operator CSInt3() const {
        return CSInt3(x, y, z);
    }

    inline operator CSVector4() const {
        return CSVector4(x, y, z, w);
    }

    uint hash() const;

    inline bool operator ==(const CSInt4& other) const {
        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    inline bool operator !=(const CSInt4& other) const {
        return !(*this == other);
    }
};

#endif
