#ifndef __CDK__CSFixed3__
#define __CDK__CSFixed3__

#include "CSFixed2.h"

#include "CSVector3.h"

class CSBuffer;

struct CSFixed3 {
    fixed x, y, z;

    static const CSFixed3 Zero;
    static const CSFixed3 UnitX;
    static const CSFixed3 UnitY;
    static const CSFixed3 UnitZ;
    static const CSFixed3 One;

    CSFixed3() = default;
    inline explicit CSFixed3(fixed v) : x(v), y(v), z(v) {}
    inline CSFixed3(fixed x, fixed y, fixed z) : x(x), y(y), z(z) {}
    inline CSFixed3(const CSFixed2& v, fixed z = fixed::Zero) : x(v.x), y(v.y), z(z) {}
    inline CSFixed3(const CSVector3& v) : x(v.x), y(v.y), z(v.z) {}
   
    explicit CSFixed3(CSBuffer* buffer);
    explicit CSFixed3(const byte*& bytes);

    inline fixed lengthSquared() const {
        return (x * x) + (y * y) + (z * z);
    }

    inline fixed length() const {
        return CSMath::sqrt(lengthSquared());
    }

    void normalize();

    static void normalize(const CSFixed3& vector, CSFixed3& result) {
        result = vector;
        result.normalize();
    }
    static inline CSFixed3 normalize(const CSFixed3& vector) {
        CSFixed3 result;
        normalize(vector, result);
        return result;
    }

    static inline void clamp(const CSFixed3& vector, const CSFixed3& min, const CSFixed3& max, CSFixed3& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
        result.z = CSMath::clamp(vector.z, min.z, max.z);
    }
    static inline CSFixed3 clamp(const CSFixed3& vector, const CSFixed3& min, const CSFixed3& max) {
        CSFixed3 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSFixed3& min, const CSFixed3& max) {
        clamp(*this, min, max, *this);
    }

    static inline fixed distanceSquared(const CSFixed3& left, const CSFixed3& right) {
        fixed x = left.x - right.x;
        fixed y = left.y - right.y;
        fixed z = left.z - right.z;
        return (x * x) + (y * y) + (z * z);
    }
    static inline fixed distance(const CSFixed3& left, const CSFixed3& right) {
        return CSMath::sqrt(distanceSquared(left, right));
    }
    inline fixed distanceSquared(const CSFixed3& other) const {
        return distanceSquared(*this, other);
    }
    inline fixed distance(const CSFixed3& other) const {
        return distance(*this, other);
    }

    static inline void lerp(const CSFixed3& start, const CSFixed3& end, fixed amount, CSFixed3& result) {
        result.x = CSMath::lerp(start.x, end.x, amount);
        result.y = CSMath::lerp(start.y, end.y, amount);
        result.z = CSMath::lerp(start.z, end.z, amount);
    }
    static inline CSFixed3 lerp(const CSFixed3& start, const CSFixed3& end, fixed amount) {
        CSFixed3 result;
        lerp(start, end, amount, result);
        return result;
    }
    static inline void smoothStep(const CSFixed3& start, const CSFixed3& end, fixed amount, CSFixed3& result) {
        amount = CSMath::smoothStep(amount);
        lerp(start, end, amount, result);
    }
    static inline CSFixed3 smoothStep(const CSFixed3& start, const CSFixed3& end, fixed amount) {
        CSFixed3 result;
        smoothStep(start, end, amount, result);
        return result;
    }

    static inline void max(const CSFixed3& left, const CSFixed3 right, CSFixed3& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
        result.z = CSMath::max(left.z, right.z);
    }
    static inline CSFixed3 max(const CSFixed3& left, const CSFixed3 right) {
        CSFixed3 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSFixed3& left, const CSFixed3& right, CSFixed3& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
        result.z = CSMath::min(left.z, right.z);
    }
    static inline CSFixed3 min(const CSFixed3& left, const CSFixed3 right) {
        CSFixed3 result;
        min(left, right, result);
        return result;
    }

    inline CSFixed3 operator +(const CSFixed3& vector) const {
        return CSFixed3(x + vector.x, y + vector.y, z + vector.z);
    }

    template<typename T>
    inline CSFixed3 operator +(T scalar) const {
        return CSFixed3(x + scalar, y + scalar, z + scalar);
    }

    inline CSFixed3 operator -(const CSFixed3& vector) const {
        return CSFixed3(x - vector.x, y - vector.y, z - vector.z);
    }

    template<typename T>
    inline CSFixed3 operator -(T scalar) const {
        return CSFixed3(x - scalar, y - scalar, z - scalar);
    }

    inline CSFixed3 operator *(const CSFixed3& vector) const {
        return CSFixed3(x * vector.x, y * vector.y , z + vector.z);
    }

    template<typename T>
    inline CSFixed3 operator *(T scale) const {
        return CSFixed3(x * scale, y * scale, z * scale);
    }

    inline CSFixed3 operator /(const CSFixed3& vector) const {
        return CSFixed3(x / vector.x, y / vector.y, z / vector.z);
    }

    template<typename T>
    inline CSFixed3 operator /(T scale) const {
        return CSFixed3(x / scale, y / scale, z / scale);
    }

    inline CSFixed3 operator -() const {
        return CSFixed3(-x, -y, -z);
    }

    inline CSFixed3& operator +=(const CSFixed3& vector) {
        x += vector.x;
        y += vector.y;
        z += vector.z;
        return *this;
    }

    template<typename T>
    inline CSFixed3& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        z += scalar;
        return *this;
    }

    inline CSFixed3& operator -=(const CSFixed3& vector) {
        x -= vector.x;
        y -= vector.y;
        z -= vector.z;
        return *this;
    }

    template<typename T>
    inline CSFixed3& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        z -= scalar;
        return *this;
    }

    inline CSFixed3& operator *=(const CSFixed3& vector) {
        x *= vector.x;
        y *= vector.y;
        z *= vector.z;
        return *this;
    }

    template<typename T>
    inline CSFixed3& operator *=(T scale) {
        x *= scale;
        y *= scale;
        z *= scale;
        return *this;
    }

    inline CSFixed3& operator /=(const CSFixed3& vector) {
        x /= vector.x;
        y /= vector.y;
        z /= vector.z;
        return *this;
    }

    template<typename T>
    inline CSFixed3& operator /=(T scale) {
        x /= scale;
        y /= scale;
        z /= scale;
        return *this;
    }

    inline explicit operator CSFixed2() const {
        return CSFixed2(x, y);
    }

    inline operator CSVector3() const {
        return CSVector3(x, y, z);
    }

    uint hash() const;

    inline bool operator ==(const CSFixed3& other) const {
        return x == other.x && y == other.y && z == other.z;
    }

    inline bool operator !=(const CSFixed3& other) const {
        return !(*this == other);
    }
};

#endif
