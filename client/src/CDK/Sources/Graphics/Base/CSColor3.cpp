#define CDK_IMPL

#include "CSColor3.h"

#include "CSBuffer.h"

const CSColor3 CSColor3::White(1.0f, 1.0f, 1.0f);
const CSColor3 CSColor3::Black(0.0f, 0.0f, 0.0f);
const CSColor3 CSColor3::Gray(0.5f, 0.5f, 0.5f);
const CSColor3 CSColor3::DarkGray(0.25f, 0.25f, 0.25f);
const CSColor3 CSColor3::LightGray(0.75f, 0.75f, 0.75f);
const CSColor3 CSColor3::Red(1.0f, 0.0f, 0.0f);
const CSColor3 CSColor3::DarkRed(0.5f, 0.0f, 0.0f);
const CSColor3 CSColor3::LightRed(1.0f, 0.5f, 0.5f);
const CSColor3 CSColor3::Green(0.0f, 1.0f, 0.0f);
const CSColor3 CSColor3::DarkGreen(0.0f, 0.5f, 0.0f);
const CSColor3 CSColor3::LightGreen(0.5f, 1.0f, 0.5f);
const CSColor3 CSColor3::Blue(0.0f, 0.0f, 1.0f);
const CSColor3 CSColor3::DarkBlue(0.0f, 0.0f, 0.5f);
const CSColor3 CSColor3::LightBlue(0.0f, 1.0f, 1.0f);
const CSColor3 CSColor3::Orange(1.0f, 0.5f, 0.0f);
const CSColor3 CSColor3::Yellow(1.0f, 1.0f, 0.0f);
const CSColor3 CSColor3::Magenta(1.0f, 0.0f, 1.0f);
const CSColor3 CSColor3::Brown(0.282f, 0.227f, 0.176f);

const CSColor3 CSColor3::LumCoeff(0.2126f, 0.7152f, 0.0722f);

CSColor3::CSColor3(CSBuffer* buffer, bool normalized) {
    if (normalized) {
        r = (buffer->readByte() & 0xff) / 255.0f;
        g = (buffer->readByte() & 0xff) / 255.0f;
        b = (buffer->readByte() & 0xff) / 255.0f;
    }
    else {
        r = buffer->readFloat();
        g = buffer->readFloat();
        b = buffer->readFloat();
    }
}

CSColor3::CSColor3(const byte*& raw, bool normalized) {
    if (normalized) {
        r = (readByte(raw) & 0xff) / 255.0f;
        g = (readByte(raw) & 0xff) / 255.0f;
        b = (readByte(raw) & 0xff) / 255.0f;
    }
    else {
        r = readFloat(raw);
        g = readFloat(raw);
        b = readFloat(raw);
    }
}

uint CSColor3::rgba() const {
    return (CSMath::clamp((int)(r * 255), 0, 255) << 24) |
        (CSMath::clamp((int)(g * 255), 0, 255) << 16) |
        (CSMath::clamp((int)(b * 255), 0, 255) << 8) |
        255;
}

