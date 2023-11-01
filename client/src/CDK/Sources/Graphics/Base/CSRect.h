#ifndef __CDK__CSRect__
#define __CDK__CSRect__

#include "CSVector2.h"

class CSBuffer;

struct CSRect {
    float x, y, width, height;

	static const CSRect Zero;
    static const CSRect ZeroToOne;
    static const CSRect ScreenNone;
    static const CSRect ScreenFull;

	CSRect() = default;
    inline CSRect(float x, float y, float width, float height) : x(x), y(y), width(width), height(height) {}
    inline CSRect(const CSVector2& origin, float width, float height) : x(origin.x), y(origin.y), width(width), height(height) {}
    inline CSRect(const CSVector2& origin, const CSVector2& size) : x(origin.x), y(origin.y), width(size.x), height(size.y) {}
    
    explicit CSRect(CSBuffer* buffer);
    explicit CSRect(const byte*& raw);
    
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
    
    inline CSVector2 leftTop() const {
        return CSVector2(left(), top());
    }

    inline CSVector2 centerTop() const {
        return CSVector2(center(), top());
    }

    inline CSVector2 rightTop() const {
        return CSVector2(right(), top());
    }

    inline CSVector2 leftMiddle() const {
        return CSVector2(left(), middle());
    }

    inline CSVector2 centerMiddle() const {
        return CSVector2(center(), middle());
    }

    inline CSVector2 rightMiddle() const {
        return CSVector2(right(), middle());
    }

    inline CSVector2 leftBottom() const {
        return CSVector2(left(), bottom());
    }

    inline CSVector2 centerBottom() const {
        return CSVector2(center(), bottom());
    }

    inline CSVector2 rightBottom() const {
        return CSVector2(right(), bottom());
    }

    inline CSVector2& origin() {
        return *(CSVector2*)&x;
    }

    inline const CSVector2& origin() const {
        return *(CSVector2*)&x;
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

    inline bool contains(const CSVector2& point) const {
        return point.x >= left() && point.x <= right() && point.y >= top() && point.y <= bottom();
    }
    
    inline bool contains(const CSRect& other) const {
        return left() <= other.left() && other.right() <= right() && top() <= other.top() && other.bottom() <= bottom();
    }

    inline bool intersects(const CSRect& other) const {
        return other.left() < right() && left() < other.right() && other.top() < bottom() && top() < other.bottom();
    }

    inline CSRect& offset(float x, float y) {
        this->x += x;
        this->y += y;
        return *this;
    }

    inline CSRect& offset(const CSVector2& v) {
        return offset(v.x, v.y);
    }

    inline CSRect offsetRect(float x, float y) const {
        return CSRect(*this).offset(x, y);
    }

    inline CSRect offsetRect(const CSVector2& v) const {
        return offsetRect(v.x, v.y);
    }

    CSRect& inflate(float w, float h);

    inline CSRect& inflate(const CSVector2& v) {
        return inflate(v.x, v.y);
    }

    inline CSRect inflateRect(float w, float h) const {
        return CSRect(*this).inflate(w, h);
    }

    inline CSRect inflateRect(const CSVector2& v) const {
        return inflateRect(v.x, v.y);
    }

    CSRect& intersect(const CSRect& rect);

    inline CSRect intersectRect(const CSRect& rect) const {
        return CSRect(*this).intersect(rect);
    }

    inline bool onScreen() const {
        return left() <= 1 && right() >= -1 && top() <= 1 && bottom() >= -1;
    }

    inline bool fullScreen() const {
        return left() <= -1 && right() >= 1 && top() <= -1 && bottom() >= 1;
    }

    void clipScreen();
    
    void append(const CSVector2& p);
    
    inline void append(const CSRect& value) {
        append(*this, value, *this);
    }

    CSRect append(const CSRect& value1, const CSRect& value2) {
        CSRect result;
        append(value1, value2, result);
        return result;
    }

    static void append(const CSRect& value1, const CSRect& value2, CSRect& result);

    static void lerp(const CSRect& start, const CSRect& end, float amount, CSRect& result);

    CSRect lerp(const CSRect& start, const CSRect& end, float amount) {
        CSRect result;
        lerp(start, end, amount, result);
        return result;
    }

    uint hash() const;

    inline bool operator ==(const CSRect& other) const {
        return x == other.x && y == other.y && width == other.width && height == other.height;
    }

    inline bool operator !=(const CSRect& other) const {
        return !(*this == other);
    }
};

#endif
