#define CDK_IMPL

#include "CSRect.h"

#include "CSBuffer.h"

const CSRect CSRect::Zero(0, 0, 0, 0);
const CSRect CSRect::ZeroToOne(0, 0, 1, 1);
const CSRect CSRect::ScreenNone(2, 2, -4, -4);
const CSRect CSRect::ScreenFull(-1, -1, 2, 2);

CSRect::CSRect(CSBuffer* buffer) :
    x(buffer->readFloat()),
    y(buffer->readFloat()),
    width(buffer->readFloat()),
    height(buffer->readFloat())
{
}

CSRect::CSRect(const byte*& raw) :
    x(readFloat(raw)),
    y(readFloat(raw)),
    width(readFloat(raw)),
    height(readFloat(raw))
{
}

CSRect& CSRect::intersect(const CSRect& rect) {
    x = CSMath::max(left(), rect.left());
    y = CSMath::max(top(), rect.top());
    width = CSMath::max(CSMath::min(right(), rect.right()) - x, 0.0f);
    height = CSMath::max(CSMath::min(bottom(), rect.bottom()) - y, 0.0f);
    return *this;
}

CSRect& CSRect::inflate(float w, float h) {
    if (width + w * 2 <= 0) {
        x += w * 0.5f;
        width = 0;
    }
    else {
        x -= w;
        width += w * 2;
    }
    if (height + h * 2 <= 0) {
        y += height * 0.5f;
        height = 0;
    }
    else {
        y -= h;
        height += h * 2;
    }
    return *this;
}

void CSRect::clipScreen() {
    if (left() < -1) x = -1;
    if (right() > 1) width = 1 - x;
    if (top() < -1) y = -1;
    if (bottom() > 1) height = 1 - y;
}

void CSRect::append(const CSVector2& p) {
    if (left() > p.x) x = p.x;
    if (right() < p.x) width = p.x - x;
    if (top() > p.y) y = p.y;
    if (bottom() < p.y) height = p.y - y;
}

void CSRect::append(const CSRect& value1, const CSRect& value2, CSRect& result) {
    float left = CSMath::min(value1.left(), value2.left());
    float right = CSMath::max(value1.right(), value2.right());
    float top = CSMath::min(value1.top(), value2.top());
    float bottom = CSMath::max(value1.bottom(), value2.bottom());
    result = CSRect(left, top, right - left, bottom - top);
}

void CSRect::lerp(const CSRect& start, const CSRect& end, float amount, CSRect& result) {
    result.x = CSMath::lerp(start.left(), end.left(), amount);
    result.y = CSMath::lerp(start.right(), end.right(), amount);
    result.width = CSMath::lerp(start.top(), end.top(), amount) - result.x;
    result.height = CSMath::lerp(start.bottom(), end.bottom(), amount) - result.y;
}

uint CSRect::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(width);
    hash.combine(height);
    return hash;
}