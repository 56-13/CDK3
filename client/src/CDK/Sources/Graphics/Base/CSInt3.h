#ifndef __CDK__CSInt3__
#define __CDK__CSInt3__

#include "CSInt2.h"

#include "CSVector3.h"

class CSBuffer;

struct CSInt3 {
    int x, y, z;

    CSInt3() = default;
    inline explicit CSInt3(int v) : x(v), y(v), z(v) {}
    inline CSInt3(int x, int y, int z) : x(x), y(y), z(z) {}
    inline CSInt3(const CSInt2& v, int z = 0) : x(v.x), y(v.y), z(z) {}
    inline CSInt3(const CSVector3& v) : x(v.x), y(v.y), z(v.z) {}

    explicit CSInt3(CSBuffer* buffer);
    explicit CSInt3(const byte*& bytes);

    static inline void clamp(const CSInt3& vector, const CSInt3& min, const CSInt3& max, CSInt3& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
        result.z = CSMath::clamp(vector.z, min.z, max.z);
    }
    static inline CSInt3 clamp(const CSInt3& vector, const CSInt3& min, const CSInt3& max) {
        CSInt3 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSInt3& min, const CSInt3& max) {
        clamp(*this, min, max, *this);
    }

    static inline void max(const CSInt3& left, const CSInt3 right, CSInt3& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
        result.z = CSMath::max(left.z, right.z);
    }
    static inline CSInt3 max(const CSInt3& left, const CSInt3 right) {
        CSInt3 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSInt3& left, const CSInt3& right, CSInt3& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
        result.z = CSMath::min(left.z, right.z);
    }
    static inline CSInt3 min(const CSInt3& left, const CSInt3 right) {
        CSInt3 result;
        min(left, right, result);
        return result;
    }

    inline CSInt3 operator +(const CSInt3& vector) const {
        return CSInt3(x + vector.x, y + vector.y, z + vector.z);
    }

    template<typename T>
    inline CSInt3 operator +(T scalar) const {
        return CSInt3(x + scalar, y + scalar, z + scalar);
    }

    inline CSInt3 operator -(const CSInt3& vector) const {
        return CSInt3(x - vector.x, y - vector.y, z - vector.z);
    }

    template<typename T>
    inline CSInt3 operator -(T scalar) const {
        return CSInt3(x - scalar, y - scalar, z - scalar);
    }

    inline CSInt3 operator *(const CSInt3& vector) const {
        return CSInt3(x * vector.x, y * vector.y, z + vector.z);
    }

    template<typename T>
    inline CSInt3 operator *(T scale) const {
        return CSInt3(x * scale, y * scale, z * scale);
    }

    inline CSInt3 operator /(const CSInt3& vector) const {
        return CSInt3(x / vector.x, y / vector.y, z / vector.z);
    }

    template<typename T>
    inline CSInt3 operator /(T scale) const {
        return CSInt3(x / scale, y / scale, z / scale);
    }

    inline CSInt3 operator -() const {
        return CSInt3(-x, -y, -z);
    }

    inline CSInt3& operator +=(const CSInt3& vector) {
        x += vector.x;
        y += vector.y;
        z += vector.z;
        return *this;
    }

    template<typename T>
    inline CSInt3& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        z += scalar;
        return *this;
    }

    inline CSInt3& operator -=(const CSInt3& vector) {
        x -= vector.x;
        y -= vector.y;
        z -= vector.z;
        return *this;
    }

    template<typename T>
    inline CSInt3& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        z -= scalar;
        return *this;
    }

    inline CSInt3& operator *=(const CSInt3& vector) {
        x *= vector.x;
        y *= vector.y;
        z *= vector.z;
        return *this;
    }

    template<typename T>
    inline CSInt3& operator *=(T scale) {
        x *= scale;
        y *= scale;
        z *= scale;
        return *this;
    }

    inline CSInt3& operator /=(const CSInt3& vector) {
        x /= vector.x;
        y /= vector.y;
        z /= vector.z;
        return *this;
    }

    template<typename T>
    inline CSInt3& operator /=(T scale) {
        x /= scale;
        y /= scale;
        z /= scale;
        return *this;
    }

    inline explicit operator CSInt2() const {
        return CSInt2(x, y);
    }

    inline operator CSVector3() const {
        return CSVector3(x, y, z);
    }

    uint hash() const;

    inline bool operator ==(const CSInt3& other) const {
        return x == other.x && y == other.y && z == other.z;
    }

    inline bool operator !=(const CSInt3& other) const {
        return !(*this == other);
    }
};

#endif
