#define CDK_IMPL

#include "CSBounds2.h"

#include "CSEntry.h"

const CSBounds2 CSBounds2::Zero(0, 0, 0, 0);

CSBounds2::CSBounds2(const CSRect& rect) : 
    x(CSMath::floor(rect.x)), 
    y(CSMath::floor(rect.y)), 
    width(CSMath::ceil(rect.right()) - x), 
    height(CSMath::ceil(rect.bottom()) - y) 
{

}

CSBounds2& CSBounds2::intersect(const CSBounds2& other) {
    x = CSMath::max(left(), other.left());
    y = CSMath::max(top(), other.top());
    width = CSMath::max(CSMath::min(right(), other.right()) - x, 0);
    height = CSMath::max(CSMath::min(bottom(), other.bottom()) - y, 0);
    return *this;
}

CSBounds2& CSBounds2::inflate(int w, int h) {
    if (width + w * 2 <= 0) {
        x += w / 2;
        width = 0;
    }
    else {
        x -= w;
        width += w * 2;
    }
    if (height + h * 2 <= 0) {
        y += height / 2;
        height = 0;
    }
    else {
        y -= h;
        height += h * 2;
    }
    return *this;
}

uint CSBounds2::hash() const {
	CSHash hash;
	hash.combine(x);
	hash.combine(y);
	hash.combine(width);
	hash.combine(height);
	return hash;
}