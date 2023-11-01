#define CDK_IMPL

#include "CSQuaternion.h"

#include "CSMatrix.h"
#include "CSBuffer.h"

const CSQuaternion CSQuaternion::Identity(0, 0, 0, 1);

CSQuaternion::CSQuaternion(CSBuffer* buffer) :
    x(buffer->readFloat()),
    y(buffer->readFloat()),
    z(buffer->readFloat()),
    w(buffer->readFloat()) 
{
}

CSQuaternion::CSQuaternion(const byte*& raw) :
    x(readFloat(raw)),
    y(readFloat(raw)),
    z(readFloat(raw)),
    w(readFloat(raw)) 
{
}

float CSQuaternion::angle() const {
    return lengthSquared() ? 2 * CSMath::acos(CSMath::clamp(w, -1.0f, 1.0f)) : 0.0f;
}

CSVector3 CSQuaternion::axis() const {
    float lengthSquared = this->lengthSquared();
    if (!lengthSquared) return CSVector3::UnitX;
    float inv = 1 / CSMath::sqrt(lengthSquared);
    return CSVector3(x * inv, y * inv, z * inv);
}

void CSQuaternion::invert() {
    float lengthSq = lengthSquared();
    if (lengthSq) {
        lengthSq = 1 / lengthSq;
        
        x = -x * lengthSq;
        y = -y * lengthSq;
        z = -z * lengthSq;
        w = w * lengthSq;
    }
}

void CSQuaternion::normalize(const CSQuaternion& qt, CSQuaternion& result) {
    float length = qt.length();
    if (length) {
        float inv = 1 / length;
        result.x = qt.x * inv;
        result.y = qt.y * inv;
        result.z = qt.z * inv;
        result.w = qt.w * inv;
    }
    else {
        result = qt;
    }
}

void CSQuaternion::normalize() {
    float length = this->length();
    if (length) {
        float inv = 1 / length;
        x *= inv;
        y *= inv;
        z *= inv;
        w *= inv;
    }
}

void CSQuaternion::lerp(const CSQuaternion& start, const CSQuaternion& end, float amount, CSQuaternion& result) {
    float inverse = 1 - amount;
    
    if (dot(start, end) >= 0) {
        result.x = (inverse * start.x) + (amount * end.x);
        result.y = (inverse * start.y) + (amount * end.y);
        result.z = (inverse * start.z) + (amount * end.z);
        result.w = (inverse * start.w) + (amount * end.w);
    }
    else
    {
        result.x = (inverse * start.x) - (amount * end.x);
        result.y = (inverse * start.y) - (amount * end.y);
        result.z = (inverse * start.z) - (amount * end.z);
        result.w = (inverse * start.w) - (amount * end.w);
    }
    
    result.normalize();
}

void CSQuaternion::rotationAxis(const CSVector3& axis, float angle, CSQuaternion& result) {
    CSVector3 normalized;
    CSVector3::normalize(axis, normalized);
    
    float half = angle * 0.5f;
    float sin = CSMath::sin(half);
    float cos = CSMath::cos(half);
    
    result.x = normalized.x * sin;
    result.y = normalized.y * sin;
    result.z = normalized.z * sin;
    result.w = cos;
}

void CSQuaternion::rotationMatrix(const CSMatrix& matrix, CSQuaternion& result) {
    float sqrt;
    float half;
    float scale = matrix.m11 + matrix.m22 + matrix.m33;
    
    if (scale > 0) {
        sqrt = CSMath::sqrt(scale + 1);
        result.w = sqrt * 0.5f;
        sqrt = 0.5f / sqrt;
        
        result.x = (matrix.m23 - matrix.m32) * sqrt;
        result.y = (matrix.m31 - matrix.m13) * sqrt;
        result.z = (matrix.m12 - matrix.m21) * sqrt;
    }
    else if ((matrix.m11 >= matrix.m22) && (matrix.m11 >= matrix.m33)) {
        sqrt = CSMath::sqrt(1 + matrix.m11 - matrix.m22 - matrix.m33);
        half = 0.5f / sqrt;
        
        result.x = 0.5f * sqrt;
        result.y = (matrix.m12 + matrix.m21) * half;
        result.z = (matrix.m13 + matrix.m31) * half;
        result.w = (matrix.m23 - matrix.m32) * half;
    }
    else if (matrix.m22 > matrix.m33) {
        sqrt = CSMath::sqrt(1 + matrix.m22 - matrix.m11 - matrix.m33);
        half = 0.5f / sqrt;
        
        result.x = (matrix.m21 + matrix.m12) * half;
        result.y = 0.5f * sqrt;
        result.z = (matrix.m32 + matrix.m23) * half;
        result.w = (matrix.m31 - matrix.m13) * half;
    }
    else {
        sqrt = CSMath::sqrt(1 + matrix.m33 - matrix.m11 - matrix.m22);
        half = 0.5f / sqrt;
        
        result.x = (matrix.m31 + matrix.m13) * half;
        result.y = (matrix.m32 + matrix.m23) * half;
        result.z = 0.5f * sqrt;
        result.w = (matrix.m12 - matrix.m21) * half;
    }
}

void CSQuaternion::lookAtLH(const CSVector3& eye, const CSVector3& target, const CSVector3& up, CSQuaternion& result) {
    CSMatrix mat;
    CSMatrix::lookAtLH(eye, target, up, mat);
    rotationMatrix(mat, result);
}

void CSQuaternion::lookAtRH(const CSVector3& eye, const CSVector3& target, const CSVector3& up, CSQuaternion& result) {
    CSMatrix mat;
    CSMatrix::lookAtRH(eye, target, up, mat);
    rotationMatrix(mat, result);
}

void CSQuaternion::rotationYawPitchRoll(float yaw, float pitch, float roll, CSQuaternion& result) {
    float halfRoll = roll * 0.5f;
    float halfPitch = pitch * 0.5f;
    float halfYaw = yaw * 0.5f;
    
    float sinRoll = CSMath::sin(halfRoll);
    float cosRoll = CSMath::cos(halfRoll);
    float sinPitch = CSMath::sin(halfPitch);
    float cosPitch = CSMath::cos(halfPitch);
    float sinYaw = CSMath::sin(halfYaw);
    float cosYaw = CSMath::cos(halfYaw);
    
    result.x = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
    result.y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
    result.z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
    result.w = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
}

void CSQuaternion::slerp(const CSQuaternion& start, const CSQuaternion& end, float amount, CSQuaternion& result) {
    float opposite;
    float inverse;
    float dotValue = dot(start, end);
    
    if (dotValue) {
        inverse = 1 - amount;
        opposite = amount * CSMath::sign(dotValue);
    }
    else {
        float acos = CSMath::acos(CSMath::abs(dotValue));
        float invSin = (float)(1 / CSMath::sin(acos));
        
        inverse = CSMath::sin((1 - amount) * acos) * invSin;
        opposite = CSMath::sin(amount * acos) * invSin * CSMath::sign(dotValue);
    }
    
    result.x = (inverse * start.x) + (opposite * end.x);
    result.y = (inverse * start.y) + (opposite * end.y);
    result.z = (inverse * start.z) + (opposite * end.z);
    result.w = (inverse * start.w) + (opposite * end.w);
}


CSQuaternion CSQuaternion::operator *(const CSQuaternion& value) const {
    CSQuaternion result;
    float a = (y * value.z - z * value.y);
    float b = (z * value.x - x * value.z);
    float c = (x * value.y - y * value.x);
    float d = (x * value.x + y * value.y + z * value.z);
    result.x = (x * value.w + value.x * w) + a;
    result.y = (y * value.w + value.y * w) + b;
    result.z = (z * value.w + value.z * w) + c;
    result.w = w * value.w - d;
    return result;
}

uint CSQuaternion::hash() const {
    CSHash hash;
    hash.combine(x);
    hash.combine(y);
    hash.combine(z);
    hash.combine(w);
    return hash;
}

bool CSQuaternion::nearEqual(const CSQuaternion& a, const CSQuaternion& b) {
    return
        CSMath::nearEqual(a.x, b.x) &&
        CSMath::nearEqual(a.y, b.y) &&
        CSMath::nearEqual(a.z, b.z) &&
        CSMath::nearEqual(a.w, b.w);
}
