#define CDK_IMPL

#include "CSBounds3.h"

#include "CSEntry.h"

const CSBounds3 CSBounds3::Zero(0, 0, 0, 0, 0, 0);

CSBounds3& CSBounds3::intersect(const CSBounds3& bounds) {
    x = CSMath::max(left(), bounds.left());
    y = CSMath::max(top(), bounds.top());
    z = CSMath::max(znear(), bounds.znear());
    width = CSMath::max(CSMath::min(right(), bounds.right()) - x, 0);
    height = CSMath::max(CSMath::min(bottom(), bounds.bottom()) - y, 0);
    depth = CSMath::max(CSMath::min(zfar(), bounds.zfar()) - z, 0);
    return *this;
}

CSBounds3& CSBounds3::inflate(int w, int h, int d) {
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
    if (depth + d * 2 <= 0) {
        z += depth / 2;
        depth = 0;
    }
    else {
        z -= d;
        depth += d * 2;
    }
    return *this;
}

uint CSBounds3::hash() const {
	CSHash hash;
	hash.combine(x);
	hash.combine(y);
	hash.combine(z);
	hash.combine(width);
	hash.combine(height);
	hash.combine(depth);
	return hash;
}