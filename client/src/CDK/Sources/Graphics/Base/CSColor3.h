#ifndef __CDK__CSColor3__
#define __CDK__CSColor3__

#include "CSMath.h"

#include "CSVector3.h"

class CSBuffer;

struct CSColor3 {
    float r, g, b;

    static const CSColor3 White;
	static const CSColor3 Black;
	static const CSColor3 Gray;
	static const CSColor3 DarkGray;
	static const CSColor3 LightGray;
	static const CSColor3 Red;
	static const CSColor3 DarkRed;
	static const CSColor3 LightRed;
	static const CSColor3 Green;
	static const CSColor3 DarkGreen;
	static const CSColor3 LightGreen;
	static const CSColor3 Blue;
	static const CSColor3 DarkBlue;
	static const CSColor3 LightBlue;
	static const CSColor3 Orange;
	static const CSColor3 Yellow;
	static const CSColor3 Magenta;
	static const CSColor3 Brown;
    
    static const CSColor3 LumCoeff;
	
    CSColor3() = default;
    inline CSColor3(float r, float g, float b) : r(r), g(g), b(b) {}
    inline CSColor3(int r, int g, int b) : r(r / 255.0f), g(g / 255.0f), b(b / 255.0f) {}
    inline CSColor3(uint rgba) : r((rgba >> 24) / 255.0f), g((rgba >> 16 & 0xff) / 255.0f), b((rgba >> 8 & 0xff) / 255.0f) {}
    inline CSColor3(const CSVector3& v) : r(v.x), g(v.y), b(v.z) {}

    explicit CSColor3(CSBuffer* buffer, bool normalized = true);
    explicit CSColor3(const byte*& raw, bool normalized = true);
    
    static inline void lerp(const CSColor3& start, const CSColor3& end, float amount, CSColor3& result) {
        result.r = CSMath::lerp(start.r, end.r, amount);
        result.g = CSMath::lerp(start.g, end.g, amount);
        result.b = CSMath::lerp(start.b, end.b, amount);
    }
    static inline CSColor3 lerp(const CSColor3& start, const CSColor3& end, float amount) {
        CSColor3 result;
        lerp(start, end, amount, result);
        return result;
    }

    inline float amplification() const {
        return CSMath::max(CSMath::max(CSMath::max(r, g), b), 1.0f);
    }

    inline CSColor3 normalized() const {
        float invamp = 1.0f / amplification();
        return CSColor3(r * invamp, g * invamp, b * invamp);
    }

    inline float brightness() const {
        return (r * LumCoeff.r) + (g * LumCoeff.g) + (b * LumCoeff.b);
    }
    
    inline CSColor3& operator *=(const CSColor3& color) {
        r *= color.r;
        g *= color.g;
        b *= color.b;
        return *this;
    }
    
    inline CSColor3 operator *(const CSColor3& color) const {
        return CSColor3(*this) *= color;
    }
    
    inline CSColor3& operator /=(const CSColor3& color) {
        r /= color.r;
        g /= color.g;
        b /= color.b;
        return *this;
    }
    
    inline CSColor3 operator /(const CSColor3& color) const {
        return CSColor3(*this) /= color;
    }
    
    inline CSColor3& operator +=(const CSColor3& color) {
        r += color.r;
        g += color.g;
        b += color.b;
        return *this;
    }
    
    inline CSColor3 operator +(const CSColor3& color) const {
        return CSColor3(*this) += color;
    }
    
    inline CSColor3& operator -=(const CSColor3& color) {
        r -= color.r;
        g -= color.g;
        b -= color.b;
        return *this;
    }
    
    inline CSColor3 operator-(const CSColor3& color) const {
        return CSColor3(*this) -= color;
    }
    
    inline CSColor3& operator *=(float scalar) {
        r *= scalar;
        g *= scalar;
        b *= scalar;
        return *this;
    }
    
    inline CSColor3 operator *(float scalar) const {
        return CSColor3(*this) *= scalar;
    }
    
    inline CSColor3& operator /=(float scalar) {
        r /= scalar;
        g /= scalar;
        b /= scalar;
        return *this;
    }
    
    inline CSColor3 operator /(float scalar) const {
        return CSColor3(*this) /= scalar;
    }
    
    inline CSColor3& operator +=(float scalar) {
        r += scalar;
        g += scalar;
        b += scalar;
        return *this;
    }
    
    inline CSColor3 operator +(float scalar) const {
        return CSColor3(*this) += scalar;
    }
    
    inline CSColor3& operator -=(float scalar) {
        r -= scalar;
        g -= scalar;
        b -= scalar;
        return *this;
    }
    
    inline CSColor3 operator-(float scalar) const {
        return CSColor3(*this) -= scalar;
    }
    
    inline operator const float*() const {
        return &r;
    }
    
    inline operator CSVector3() const {
        return CSVector3(r, g, b);
    }

    uint rgba() const;

    inline uint hash() const {
        return rgba();
    }

    inline bool operator ==(const CSColor3& other) const {
        return r == other.r && g == other.g && b == other.b;
    }

    inline bool operator !=(const CSColor3& other) const {
        return !(*this == other);
    }
};

#endif
