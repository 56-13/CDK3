#ifndef __CDK__CSFixed4__
#define __CDK__CSFixed4__

#include "CSFixed3.h"

#include "CSVector4.h"

class CSBuffer;

struct CSFixed4 {
    fixed x, y, z, w;

    static const CSFixed4 Zero;
    static const CSFixed4 UnitX;
    static const CSFixed4 UnitY;
    static const CSFixed4 UnitZ;
    static const CSFixed4 UnitW;
    static const CSFixed4 One;

    CSFixed4() = default;
    inline explicit CSFixed4(fixed v) : x(v), y(v), z(v), w(v) {}
    inline CSFixed4(fixed x, fixed y, fixed z, fixed w) : x(x), y(y), z(z), w(w) {}
    inline CSFixed4(const CSFixed2& v, fixed z, fixed w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSFixed4(const CSFixed3& v, fixed w) : x(v.x), y(v.y), z(z), w(w) {}
    inline CSFixed4(const CSVector4& v) : x(v.x), y(v.y), z(v.z), w(v.w) {}

    explicit CSFixed4(CSBuffer* buffer);
    explicit CSFixed4(const byte*& bytes);

    inline fixed lengthSquared() const {
        return (x * x) + (y * y) + (z * z) + (w * w);
    }

    inline fixed length() const {
        return CSMath::sqrt(lengthSquared());
    }

    void normalize();

    static void normalize(const CSFixed4& vector, CSFixed4& result) {
        result = vector;
        result.normalize();
    }
    static inline CSFixed4 normalize(const CSFixed4& vector) {
        CSFixed4 result;
        normalize(vector, result);
        return result;
    }

    static inline void clamp(const CSFixed4& vector, const CSFixed4& min, const CSFixed4& max, CSFixed4& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
        result.z = CSMath::clamp(vector.z, min.z, max.z);
        result.w = CSMath::clamp(vector.w, min.w, max.w);
    }
    static inline CSFixed4 clamp(const CSFixed4& vector, const CSFixed4& min, const CSFixed4& max) {
        CSFixed4 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSFixed4& min, const CSFixed4& max) {
        clamp(*this, min, max, *this);
    }

    static inline fixed distanceSquared(const CSFixed4& left, const CSFixed4& right) {
        fixed x = left.x - right.x;
        fixed y = left.y - right.y;
        fixed z = left.z - right.z;
        fixed w = left.w - right.w;
        return (x * x) + (y * y) + (z * z) + (w * w);
    }
    static inline fixed distance(const CSFixed4& left, const CSFixed4& right) {
        return CSMath::sqrt(distanceSquared(left, right));
    }
    inline fixed distanceSquared(const CSFixed4& other) const {
        return distanceSquared(*this, other);
    }
    inline fixed distance(const CSFixed4& other) const {
        return distance(*this, other);
    }

    static inline void lerp(const CSFixed4& start, const CSFixed4& end, fixed amount, CSFixed4& result) {
        result.x = CSMath::lerp(start.x, end.x, amount);
        result.y = CSMath::lerp(start.y, end.y, amount);
        result.z = CSMath::lerp(start.z, end.z, amount);
        result.w = CSMath::lerp(start.w, end.w, amount);
    }
    static inline CSFixed4 lerp(const CSFixed4& start, const CSFixed4& end, fixed amount) {
        CSFixed4 result;
        lerp(start, end, amount, result);
        return result;
    }
    static inline void smoothStep(const CSFixed4& start, const CSFixed4& end, fixed amount, CSFixed4& result) {
        amount = CSMath::smoothStep(amount);
        lerp(start, end, amount, result);
    }
    static inline CSFixed4 smoothStep(const CSFixed4& start, const CSFixed4& end, fixed amount) {
        CSFixed4 result;
        smoothStep(start, end, amount, result);
        return result;
    }

    static inline void max(const CSFixed4& left, const CSFixed4 right, CSFixed4& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
        result.z = CSMath::max(left.z, right.z);
    }
    static inline CSFixed4 max(const CSFixed4& left, const CSFixed4 right) {
        CSFixed4 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSFixed4& left, const CSFixed4& right, CSFixed4& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
        result.z = CSMath::min(left.z, right.z);
        result.w = CSMath::min(left.w, right.w);
    }
    static inline CSFixed4 min(const CSFixed4& left, const CSFixed4 right) {
        CSFixed4 result;
        min(left, right, result);
        return result;
    }

    inline CSFixed4 operator +(const CSFixed4& vector) const {
        return CSFixed4(x + vector.x, y + vector.y, z + vector.z, w + vector.w);
    }

    template<typename T>
    inline CSFixed4 operator +(T scalar) const {
        return CSFixed4(x + scalar, y + scalar, z + scalar, w + scalar);
    }

    inline CSFixed4 operator -(const CSFixed4& vector) const {
        return CSFixed4(x - vector.x, y - vector.y, z - vector.z, w - vector.w);
    }

    template<typename T>
    inline CSFixed4 operator -(T scalar) const {
        return CSFixed4(x - scalar, y - scalar, z - scalar, w - scalar);
    }

    inline CSFixed4 operator *(const CSFixed4& vector) const {
        return CSFixed4(x * vector.x, y * vector.y, z + vector.z, w + vector.w);
    }

    template<typename T>
    inline CSFixed4 operator *(T scale) const {
        return CSFixed4(x * scale, y * scale, z * scale, w * scale);
    }

    inline CSFixed4 operator /(const CSFixed4& vector) const {
        return CSFixed4(x / vector.x, y / vector.y, z / vector.z, w / vector.w);
    }

    template<typename T>
    inline CSFixed4 operator /(T scale) const {
        return CSFixed4(x / scale, y / scale, z / scale, w / scale);
    }

    inline CSFixed4 operator -() const {
        return CSFixed4(-x, -y, -z, -w);
    }

    inline CSFixed4& operator +=(const CSFixed4& vector) {
        x += vector.x;
        y += vector.y;
        z += vector.z;
        w += vector.w;
        return *this;
    }

    template<typename T>
    inline CSFixed4& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        z += scalar;
        w += scalar;
        return *this;
    }

    inline CSFixed4& operator -=(const CSFixed4& vector) {
        x -= vector.x;
        y -= vector.y;
        z -= vector.z;
        w -= vector.w;
        return *this;
    }

    template<typename T>
    inline CSFixed4& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        z -= scalar;
        w -= scalar;
        return *this;
    }

    inline CSFixed4& operator *=(const CSFixed4& vector) {
        x *= vector.x;
        y *= vector.y;
        z *= vector.z;
        w *= vector.w;
        return *this;
    }

    template<typename T>
    inline CSFixed4& operator *=(T scale) {
        x *= scale;
        y *= scale;
        z *= scale;
        w *= scale;
        return *this;
    }

    inline CSFixed4& operator /=(const CSFixed4& vector) {
        x /= vector.x;
        y /= vector.y;
        z /= vector.z;
        w /= vector.w;
        return *this;
    }

    template<typename T>
    inline CSFixed4& operator /=(T scale) {
        x /= scale;
        y /= scale;
        z /= scale;
        w /= scale;
        return *this;
    }

    inline explicit operator CSFixed2() const {
        return CSFixed2(x, y);
    }

    inline explicit operator CSFixed3() const {
        return CSFixed3(x, y, z);
    }

    inline operator CSVector4() const {
        return CSVector4(x, y, z, w);
    }

    uint hash() const;

    inline bool operator ==(const CSFixed4& other) const {
        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    inline bool operator !=(const CSFixed4& other) const {
        return !(*this == other);
    }
};

#endif
