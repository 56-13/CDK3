#define CDK_IMPL

#include "CSZRect.h"

#include "CSBuffer.h"

const CSZRect CSZRect::Zero(0, 0, 0, 0, 0);

CSZRect::CSZRect(CSBuffer* buffer) :
    x(buffer->readFloat()),
    y(buffer->readFloat()),
    z(buffer->readFloat()),
    width(buffer->readFloat()),
    height(buffer->readFloat()) {
}

CSZRect::CSZRect(const byte*& raw) :
    x(readFloat(raw)),
    y(readFloat(raw)),
    z(readFloat(raw)),
    width(readFloat(raw)),
    height(readFloat(raw)) {
}

CSZRect& CSZRect::inflate(float w, float h) {
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

uint CSZRect::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    hash.combine(width);
    hash.combine(height);
    return hash;
}