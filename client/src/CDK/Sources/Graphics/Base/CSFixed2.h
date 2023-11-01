#ifndef __CDK__CSFixed2__
#define __CDK__CSFixed2__

#include "CSFixed.h"

#include "CSVector2.h"

class CSBuffer;

struct CSFixed2 {
    fixed x, y;

    static const CSFixed2 Zero;
    static const CSFixed2 UnitX;
    static const CSFixed2 UnitY;
    static const CSFixed2 One;

    CSFixed2() = default;
    inline explicit CSFixed2(fixed v) : x(v), y(v) {}
    inline CSFixed2(fixed x, fixed y) : x(x), y(y) {}
    inline CSFixed2(const CSVector2& v) : x(v.x), y(v.y) {}
    
    explicit CSFixed2(CSBuffer* buffer);
    explicit CSFixed2(const byte*& bytes);

    inline fixed lengthSquared() const {
        return (x * x) + (y * y);
    }

    inline fixed length() const {
        return CSMath::sqrt(lengthSquared());
    }

    void normalize();

    static void normalize(const CSFixed2& vector, CSFixed2& result) {
        result = vector;
        result.normalize();
    }
    static inline CSFixed2 normalize(const CSFixed2& vector) {
        CSFixed2 result;
        normalize(vector, result);
        return result;
    }

    static inline void clamp(const CSFixed2& vector, const CSFixed2& min, const CSFixed2& max, CSFixed2& result) {
        result.x = CSMath::clamp(vector.x, min.x, max.x);
        result.y = CSMath::clamp(vector.y, min.y, max.y);
    }
    static inline CSFixed2 clamp(const CSFixed2& vector, const CSFixed2& min, const CSFixed2& max) {
        CSFixed2 result;
        clamp(vector, min, max, result);
        return result;
    }
    inline void clamp(const CSFixed2& min, const CSFixed2& max) {
        clamp(*this, min, max, *this);
    }

    static inline fixed distanceSquared(const CSFixed2& left, const CSFixed2& right) {
        fixed x = left.x - right.x;
        fixed y = left.y - right.y;
        return (x * x) + (y * y);
    }
    static inline fixed distance(const CSFixed2& left, const CSFixed2& right) {
        return CSMath::sqrt(distanceSquared(left, right));
    }
    inline fixed distanceSquared(const CSFixed2& other) const {
        return distanceSquared(*this, other);
    }
    inline fixed distance(const CSFixed2& other) const {
        return distance(*this, other);
    }

    fixed distanceSquaredToSegment(const CSFixed2& p0, const CSFixed2& p1, CSFixed2* np = NULL) const;
    inline fixed distanceToSegment(const CSFixed2& p0, const CSFixed2& p1, CSFixed2* np = NULL) const {
        return CSMath::sqrt(distanceSquaredToSegment(p0, p1, np));
    }
    static bool intersectsSegment(const CSFixed2& p0, const CSFixed2& p1, const CSFixed2& p2, const CSFixed2& p3, CSFixed2* cp = NULL);

    bool intersectsSegment(const CSFixed2& cp, const CSFixed2& p0, const CSFixed2& p1) const;

    static inline void lerp(const CSFixed2& start, const CSFixed2& end, fixed amount, CSFixed2& result) {
        result.x = CSMath::lerp(start.x, end.x, amount);
        result.y = CSMath::lerp(start.y, end.y, amount);
    }
    static inline CSFixed2 lerp(const CSFixed2& start, const CSFixed2& end, fixed amount) {
        CSFixed2 result;
        lerp(start, end, amount, result);
        return result;
    }
    static inline void smoothStep(const CSFixed2& start, const CSFixed2& end, fixed amount, CSFixed2& result) {
        amount = CSMath::smoothStep(amount);
        lerp(start, end, amount, result);
    }
    static inline CSFixed2 smoothStep(const CSFixed2& start, const CSFixed2& end, fixed amount) {
        CSFixed2 result;
        smoothStep(start, end, amount, result);
        return result;
    }

    static inline void max(const CSFixed2& left, const CSFixed2 right, CSFixed2& result) {
        result.x = CSMath::max(left.x, right.x);
        result.y = CSMath::max(left.y, right.y);
    }
    static inline CSFixed2 max(const CSFixed2& left, const CSFixed2 right) {
        CSFixed2 result;
        max(left, right, result);
        return result;
    }

    static inline void min(const CSFixed2& left, const CSFixed2& right, CSFixed2& result) {
        result.x = CSMath::min(left.x, right.x);
        result.y = CSMath::min(left.y, right.y);
    }
    static inline CSFixed2 min(const CSFixed2& left, const CSFixed2 right) {
        CSFixed2 result;
        min(left, right, result);
        return result;
    }

    void rotate(fixed a);

    inline CSFixed2 operator +(const CSFixed2& vector) const {
        return CSFixed2(x + vector.x, y + vector.y);
    }

    template<typename T>
    inline CSFixed2 operator +(T scalar) const {
        return CSFixed2(x + scalar, y + scalar);
    }

    inline CSFixed2 operator -(const CSFixed2& vector) const {
        return CSFixed2(x - vector.x, y - vector.y);
    }

    template<typename T>
    inline CSFixed2 operator -(T scalar) const {
        return CSFixed2(x - scalar, y - scalar);
    }

    inline CSFixed2 operator *(const CSFixed2& vector) const {
        return CSFixed2(x * vector.x, y * vector.y);
    }

    template<typename T>
    inline CSFixed2 operator *(T scale) const {
        return CSFixed2(x * scale, y * scale);
    }

    inline CSFixed2 operator /(const CSFixed2& vector) const {
        return CSFixed2(x / vector.x, y / vector.y);
    }

    template<typename T>
    inline CSFixed2 operator /(T scale) const {
        return CSFixed2(x / scale, y / scale);
    }

    inline CSFixed2 operator -() const {
        return CSFixed2(-x, -y);
    }

    inline CSFixed2& operator +=(const CSFixed2& vector) {
        x += vector.x;
        y += vector.y;
        return *this;
    }

    template<typename T>
    inline CSFixed2& operator +=(T scalar) {
        x += scalar;
        y += scalar;
        return *this;
    }

    inline CSFixed2& operator -=(const CSFixed2& vector) {
        x -= vector.x;
        y -= vector.y;
        return *this;
    }

    template<typename T>
    inline CSFixed2& operator -=(T scalar) {
        x -= scalar;
        y -= scalar;
        return *this;
    }

    inline CSFixed2& operator *=(const CSFixed2& vector) {
        x *= vector.x;
        y *= vector.y;
        return *this;
    }

    template<typename T>
    inline CSFixed2& operator *=(T scale) {
        x *= scale;
        y *= scale;
        return *this;
    }

    inline CSFixed2& operator /=(const CSFixed2& vector) {
        x /= vector.x;
        y /= vector.y;
        return *this;
    }

    template<typename T>
    inline CSFixed2& operator /=(T scale) {
        x /= scale;
        y /= scale;
        return *this;
    }

    inline operator CSVector2() const {
        return CSVector2(x, y);
    }

    uint hash() const;

    inline bool operator ==(const CSFixed2& other) const {
        return x == other.x && y == other.y;
    }

    inline bool operator !=(const CSFixed2& other) const {
        return !(*this == other);
    }
};

#endif
