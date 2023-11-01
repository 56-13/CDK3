#ifndef __CDK__CSZRect__
#define __CDK__CSZRect__

#include "CSRect.h"

#include "CSVector3.h"

class CSBuffer;

struct CSZRect {
    float x, y, z, width, height;
    
    static const CSZRect Zero;

    CSZRect() = default;
    inline CSZRect(float x, float y, float z, float width, float height) : x(x), y(y), z(z), width(width), height(height) {}
    inline CSZRect(const CSVector3& origin, float width, float height) : x(origin.x), y(origin.y), z(origin.z), width(width), height(height) {}
    inline CSZRect(const CSVector3& origin, const CSVector2& size) : x(origin.x), y(origin.y), z(origin.z), width(size.x), height(size.y) {}
    inline CSZRect(const CSRect& rect, float z = 0) : x(rect.x), y(rect.y), z(z), width(rect.width), height(rect.height) {}

    explicit CSZRect(CSBuffer* buffer);
    explicit CSZRect(const byte*& raw);
    
    inline float left() const {
        return x;
    }

    inline float right() const {
        return x + width;
    }

    inline float top() const {
        return y;
    }

    inline float bottom() const {
        return y + height;
    }

    inline float center() const {
        return x + width * 0.5f;
    }

    inline float middle() const {
        return y + height * 0.5f;
    }
    
    inline CSVector3 leftTop() const {
        return CSVector3(left(), top(), z);
    }

    inline CSVector3 centerTop() const {
        return CSVector3(center(), top(), z);
    }

    inline CSVector3 rightTop() const {
        return CSVector3(right(), top(), z);
    }

    inline CSVector3 leftMiddle() const {
        return CSVector3(left(), middle(), z);
    }

    inline CSVector3 centerMiddle() const {
        return CSVector3(center(), middle(), z);
    }

    inline CSVector3 rightMiddle() const {
        return CSVector3(right(), middle(), z);
    }

    inline CSVector3 leftBottom() const {
        return CSVector3(left(), bottom(), z);
    }

    inline CSVector3 centerBottom() const {
        return CSVector3(center(), bottom(), z);
    }

    inline CSVector3 rightBottom() const {
        return CSVector3(right(), bottom(), z);
    }

    inline CSVector3& origin() {
        return *(CSVector3*)&x;
    }

    inline const CSVector3& origin() const {
        return *(CSVector3*)&x;
    }

    inline CSVector2& size() {
        return *(CSVector2*)&width;
    }

    inline const CSVector2& size() const {
        return *(CSVector2*)&width;
    }

    inline CSVector2 halfSize() const {
        return CSVector2(width * 0.5f, height * 0.5f);
    }
    
    inline bool contains(const CSVector3& point) const {
        return point.x >= left() && point.x <= right() && point.y >= top() && point.y <= bottom() && CSMath::nearEqual(z, point.z);
    }

    inline bool contains(const CSZRect& other) const {
        return left() <= other.left() && other.right() <= right() && top() <= other.top() && other.bottom() <= bottom() && CSMath::nearEqual(z, other.z);
    }

    inline bool intersects(const CSZRect& other) const {
        return other.left() < right() && left() < other.right() && other.top() < bottom() && top() < other.bottom() && CSMath::nearEqual(z, other.z);
    }

    inline CSZRect& offset(float x, float y, float z) {
        this->x += x;
        this->y += y;
        this->z += z;
        return *this;
    }

    inline CSZRect& offset(const CSVector3& v) {
        return offset(v.x, v.y, v.z);
    }

    inline CSZRect offsetRect(float x, float y, float z) const {
        return CSZRect(*this).offset(x, y, z);
    }

    inline CSZRect offsetRect(const CSVector3& v) const {
        return offsetRect(v.x, v.y, v.z);
    }

    CSZRect& inflate(float w, float h);

    inline CSZRect& inflate(const CSVector2& v) {
        return inflate(v.x, v.y);
    }

    inline CSZRect inflateRect(float w, float h) const {
        return CSZRect(*this).inflate(w, h);
    }

    inline CSZRect inflateRect(const CSVector2& v) const {
        return inflateRect(v.x, v.y);
    }
    
    inline explicit operator CSRect() const {
        return CSRect(x, y, width, height);
    }
    
    uint hash() const;

    inline bool operator ==(const CSZRect& other) const {
        return x == other.x && y == other.y && z == other.z && width == other.width && height == other.height;
    }

    inline bool operator !=(const CSZRect& other) const {
        return !(*this == other);
    }

};

#endif
