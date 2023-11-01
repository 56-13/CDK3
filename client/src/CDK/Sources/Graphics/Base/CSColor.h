#ifndef __CDK__CSColor__
#define __CDK__CSColor__

#include "CSColor3.h"

#include "CSVector4.h"

class CSBuffer;

struct CSColor {
    float r, g, b, a;

    static const CSColor White;
	static const CSColor Black;
	static const CSColor Transparent;
	static const CSColor Gray;
	static const CSColor DarkGray;
	static const CSColor LightGray;
	static const CSColor Red;
	static const CSColor DarkRed;
	static const CSColor LightRed;
	static const CSColor Green;
	static const CSColor DarkGreen;
	static const CSColor LightGreen;
	static const CSColor Blue;
	static const CSColor DarkBlue;
	static const CSColor LightBlue;
	static const CSColor Orange;
	static const CSColor Yellow;
	static const CSColor Magenta;
	static const CSColor Brown;
	static const CSColor TranslucentBlack;
	static const CSColor TranslucentWhite;
	
    CSColor() = default;
    inline CSColor(float r, float g, float b, float a) : r(r), g(g), b(b), a(a) {}
    inline CSColor(int r, int g, int b, int a) : r(r / 255.0f), g(g / 255.0f), b(b / 255.0f), a(a / 255.0f) {}
    inline CSColor(uint rgba) : r((rgba >> 24) / 255.0f), g((rgba >> 16 & 0xff) / 255.0f), b((rgba >> 8 & 0xff) / 255.0f), a((rgba & 0xff) / 255.0f) {}
    inline CSColor(const CSColor3& rgb, float a = 1) : r(rgb.r), g(rgb.g), b(rgb.b), a(a) {}
    inline CSColor(const CSVector4& v) : r(v.x), g(v.y), b(v.z), a(v.w) {}

    explicit CSColor(CSBuffer* buffer, bool normalized = true);
    explicit CSColor(const byte*& raw, bool normalized = true);
    
    static inline void lerp(const CSColor& start, const CSColor& end, float amount, CSColor& result) {
        result.r = CSMath::lerp(start.r, end.r, amount);
        result.g = CSMath::lerp(start.g, end.g, amount);
        result.b = CSMath::lerp(start.b, end.b, amount);
        result.a = CSMath::lerp(start.a, end.a, amount);
    }
    static inline CSColor lerp(const CSColor& start, const CSColor& end, float amount) {
        CSColor result;
        lerp(start, end, amount, result);
        return result;
    }

    inline float amplification() const {
        return CSMath::max(CSMath::max(CSMath::max(r, g), b), 1.0f);
    }

    inline CSColor normalized() const {
        float invamp = 1.0f / amplification();
        return CSColor(r * invamp, g * invamp, b * invamp, a);
    }

    inline float brightness() const {
        return (r * CSColor3::LumCoeff.r) + (g * CSColor3::LumCoeff.g) + (b * CSColor3::LumCoeff.b);
    }
    
    inline CSColor& operator *= (const CSColor& color) {
        r *= color.r;
        g *= color.g;
        b *= color.b;
        a *= color.a;
        return *this;
    }
    
    inline CSColor operator *(const CSColor& color) const {
        return CSColor(*this) *= color;
    }
    
    inline CSColor& operator /= (const CSColor& color) {
        r /= color.r;
        g /= color.g;
        b /= color.b;
        a /= color.a;
        return *this;
    }
    
    inline CSColor operator /(const CSColor& color) const {
        return CSColor(*this) /= color;
    }
    
    inline CSColor& operator +=(const CSColor& color) {
        r += color.r;
        g += color.g;
        b += color.b;
        a += color.a;
        return *this;
    }
    
    inline CSColor operator +(const CSColor& color) const {
        return CSColor(*this) += color;
    }
    
    inline CSColor& operator -=(const CSColor& color) {
        r -= color.r;
        g -= color.g;
        b -= color.b;
        a -= color.a;
        return *this;
    }
    
    inline CSColor operator -(const CSColor& color) const {
        return CSColor(*this) -= color;
    }
    
    inline CSColor& operator *= (float scalar) {
        r *= scalar;
        g *= scalar;
        b *= scalar;
        a *= scalar;
        return *this;
    }
    
    inline CSColor operator *(float scalar) const {
        return CSColor(*this) *= scalar;
    }
    
    inline CSColor& operator /= (float scalar) {
        r /= scalar;
        g /= scalar;
        b /= scalar;
        a /= scalar;
        return *this;
    }
    
    inline CSColor operator /(float scalar) const {
        return CSColor(*this) /= scalar;
    }
    
    inline CSColor& operator +=(float scalar) {
        r += scalar;
        g += scalar;
        b += scalar;
        a += scalar;
        return *this;
    }
    
    inline CSColor operator +(float scalar) const {
        return CSColor(*this) += scalar;
    }
    
    inline CSColor& operator -=(float scalar) {
        r -= scalar;
        g -= scalar;
        b -= scalar;
        a -= scalar;
        return *this;
    }
    
    inline CSColor operator -(float scalar) const {
        return CSColor(*this) -= scalar;
    }
    
    inline operator const float*() const {
        return &r;
    }
    
    inline explicit operator CSColor3() const {
        return CSColor3(r, g, b);
    }

    inline operator CSVector4() const {
        return CSVector4(r, g, b, a);
    }

    uint rgba() const;

    inline uint hash() const {
        return rgba();
    }

    inline bool operator ==(const CSColor& other) const {
        return r == other.r && g == other.g && b == other.b && a == other.a;
    }

    inline bool operator !=(const CSColor& other) const {
        return !(*this == other);
    }
};

#endif
