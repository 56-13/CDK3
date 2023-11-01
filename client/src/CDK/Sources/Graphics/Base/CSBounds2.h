#ifndef __CDK__CSBounds2__
#define __CDK__CSBounds2__

#include "CSInt2.h"

#include "CSRect.h"

struct CSBounds2 {
    int x;
    int y;
    int width;
    int height;

    static const CSBounds2 Zero;

    CSBounds2() = default;
    inline CSBounds2(int x, int y, int width, int height) : x(x), y(y), width(width), height(height) {}
    CSBounds2(const CSRect& rect);

    inline int left() const {
        return x;
    }

    inline int right() const {
        return x + width;
    }

    inline int top() const {
        return y;
    }

    inline int bottom() const {
        return y + height;
    }

    inline float center() const {
        return x + width * 0.5f;
    }

    inline float middle() const {
        return y + height * 0.5f;
    }

    inline CSInt2 leftTop() const {
        return CSInt2(left(), top());
    }

    inline CSInt2 rightTop() const {
        return CSInt2(right(), top());
    }

    inline CSInt2 leftBottom() const {
        return CSInt2(left(), bottom());
    }

    inline CSInt2 rightBottom() const {
        return CSInt2(right(), bottom());
    }

    inline CSInt2& origin() {
        return *(CSInt2*)&x;
    }

    inline const CSInt2& origin() const {
        return *(CSInt2*)&x;
    }

    inline CSInt2& size() {
        return *(CSInt2*)&width;
    }

    inline const CSInt2& size() const {
        return *(CSInt2*)&width;
    }

    inline bool contains(const CSInt2& point) const {
        return point.x >= left() && point.x <= right() && point.y >= top() && point.y <= bottom();
    }

    inline bool contains(const CSBounds2& other) const {
        return left() <= other.left() && other.right() <= right() && top() <= other.top() && other.bottom() <= bottom();
    }

    inline bool intersects(const CSBounds2& other) const {
        return other.left() < right() && left() < other.right() && other.top() < bottom() && top() < other.bottom();
    }

    inline CSBounds2& offset(int x, int y) {
        this->x += x;
        this->y += y;
        return *this;
    }

    inline CSBounds2& offset(const CSInt2& v) {
        return offset(v.x, v.y);
    }

    inline CSBounds2 offsetBounds(int x, int y) const {
        return CSBounds2(*this).offset(x, y);
    }

    inline CSBounds2 offsetBounds(const CSInt2& v) const {
        return offsetBounds(v.x, v.y);
    }

    CSBounds2& inflate(int w, int h);

    inline CSBounds2& inflate(const CSInt2& v) {
        return inflate(v.x, v.y);
    }

    inline CSBounds2 inflateBounds(int w, int h) const {
        return CSBounds2(*this).inflate(w, h);
    }

    inline CSBounds2 inflateBounds(const CSInt2& v) const {
        return inflateBounds(v.x, v.y);
    }

    CSBounds2& intersect(const CSBounds2& other);

    inline CSBounds2 intersectBounds(const CSBounds2& bounds) const {
        return CSBounds2(*this).intersect(bounds);
    }

    uint hash() const;

    inline bool operator ==(const CSBounds2& other) const {
        return x == other.x && y == other.y && width == other.width && height == other.height;
    }
    inline bool operator !=(const CSBounds2& other) const {
        return !(*this == other);
    }
};

#endif