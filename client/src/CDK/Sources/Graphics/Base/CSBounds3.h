#ifndef __CDK__CSBounds3__
#define __CDK__CSBounds3__

#include "CSInt3.h"

struct CSBounds3 {
    int x;
    int y;
    int z;
    int width;
    int height;
    int depth;

    static const CSBounds3 Zero;

    CSBounds3() = default;
    inline CSBounds3(int x, int y, int z, int width, int height, int depth) : x(x), y(y), z(z), width(width), height(height), depth(depth) {}

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

    inline int znear() const {
        return z;
    }

    inline int zfar() const {
        return z + depth;
    }

    inline CSInt3 min() const {
        return CSInt3(left(), top(), znear());
    }

    inline CSInt3 max() const {
        return CSInt3(right(), bottom(), zfar());
    }

    inline CSInt3& origin() {
        return *(CSInt3*)&x;
    }

    inline const CSInt3& origin() const {
        return *(CSInt3*)&x;
    }

    inline CSInt3& size() {
        return *(CSInt3*)&width;
    }

    inline const CSInt3& size() const {
        return *(CSInt3*)&width;
    }

    inline bool contains(const CSInt3& point) const {
        return point.x >= left() && point.x <= right() && point.y >= top() && point.y <= bottom() && point.z >= znear() && point.z <= zfar();
    }

    inline bool contains(const CSBounds3& other) const {
        return left() <= other.left() && other.right() <= right() && top() <= other.top() && other.bottom() <= bottom() && znear() <= other.znear() && other.zfar() <= zfar();
    }

    inline bool intersects(const CSBounds3& other) const {
        return other.left() < right() && left() < other.right() && other.top() < bottom() && top() < other.bottom() && other.znear() < zfar() && znear() < other.zfar();
    }

    inline CSBounds3& offset(int x, int y, int z) {
        this->x += x;
        this->y += y;
        this->z += z;
        return *this;
    }

    inline CSBounds3& offset(const CSInt3& v) {
        return offset(v.x, v.y, v.z);
    }

    inline CSBounds3 offsetBounds(int x, int y, int z) const {
        return CSBounds3(*this).offset(x, y, z);
    }

    inline CSBounds3 offsetBounds(const CSInt3& v) const {
        return offsetBounds(v.x, v.y, v.z);
    }

    CSBounds3& inflate(int w, int h, int d);

    inline CSBounds3& inflate(const CSInt3& v) {
        return inflate(v.x, v.y, v.z);
    }

    inline CSBounds3 inflateRect(int w, int h, int d) const {
        return CSBounds3(*this).inflate(w, h, d);
    }

    inline CSBounds3 inflateRect(const CSInt3& v) const {
        return inflateRect(v.x, v.y, v.z);
    }

    CSBounds3& intersect(const CSBounds3& bounds);

    inline CSBounds3 intersectBounds(const CSBounds3& bounds) const {
        return CSBounds3(*this).intersect(bounds);
    }

    uint hash() const;

    inline bool operator ==(const CSBounds3& other) const {
        return x == other.x && y == other.y && z == other.z && width == other.width && height == other.height && depth == other.depth;
    }
    inline bool operator !=(const CSBounds3& other) const {
        return !(*this == other);
    }
};

#endif