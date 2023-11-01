#define CDK_IMPL

#include "CSMatrix.h"

#include "CSBuffer.h"

const CSMatrix CSMatrix::Identity(
    1, 0, 0, 0,
    0, 1, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1);

CSMatrix::CSMatrix(float m11, float m12, float m13, float m14,
                   float m21, float m22, float m23, float m24,
                   float m31, float m32, float m33, float m34,
                   float m41, float m42, float m43, float m44) :
    m11(m11), m12(m12), m13(m13), m14(m14),
    m21(m21), m22(m22), m23(m23), m24(m24),
    m31(m31), m32(m32), m33(m33), m34(m34),
    m41(m41), m42(m42), m43(m43), m44(m44)
{
}

CSMatrix::CSMatrix(CSBuffer* buffer) {
    buffer->read(this, sizeof(CSMatrix));
}

CSMatrix::CSMatrix(const byte*& bytes) {
    memcpy(this, bytes, sizeof(CSMatrix));
    bytes += sizeof(CSMatrix);
}

float CSMatrix::determinant() const {
    float temp1 = (m33 * m44) - (m34 * m43);
    float temp2 = (m32 * m44) - (m34 * m42);
    float temp3 = (m32 * m43) - (m33 * m42);
    float temp4 = (m31 * m44) - (m34 * m41);
    float temp5 = (m31 * m43) - (m33 * m41);
    float temp6 = (m31 * m42) - (m32 * m41);
    
    return ((((m11 * (((m22 * temp1) - (m23 * temp2)) + (m24 * temp3))) - (m12 * (((m21 * temp1) -
              (m23 * temp4)) + (m24 * temp5)))) + (m13 * (((m21 * temp2) - (m22 * temp4)) + (m24 * temp6)))) -
            (m14 * (((m21 * temp3) - (m22 * temp5)) + (m23 * temp6))));
}

bool CSMatrix::decompose(CSVector3& scale, CSQuaternion& rotation, CSVector3& trans) const {
    trans.x = m41;
    trans.y = m42;
    trans.z = m43;
    
    //Scaling is the length of the rows.
    scale.x = CSMath::sqrt((m11 * m11) + (m12 * m12) + (m13 * m13));
    scale.y = CSMath::sqrt((m21 * m21) + (m22 * m22) + (m23 * m23));
    scale.z = CSMath::sqrt((m31 * m31) + (m32 * m32) + (m33 * m33));
    
    //If any of the scaling factors are zero, than the rotation matrix can not exist.
    if (!scale.x || !scale.y || !scale.z) {
        rotation = CSQuaternion::Identity;
        return false;
    }
    
    //The rotation is the left over matrix after dividing out the scaling.
    CSMatrix rotationmatrix;
    rotationmatrix.m11 = m11 / scale.x;
    rotationmatrix.m12 = m12 / scale.x;
    rotationmatrix.m13 = m13 / scale.x;
    rotationmatrix.m14 = 0;
    
    rotationmatrix.m21 = m21 / scale.y;
    rotationmatrix.m22 = m22 / scale.y;
    rotationmatrix.m23 = m23 / scale.y;
    rotationmatrix.m24 = 0;
    
    rotationmatrix.m31 = m31 / scale.z;
    rotationmatrix.m32 = m32 / scale.z;
    rotationmatrix.m33 = m33 / scale.z;
    rotationmatrix.m34 = 0;
    
    rotationmatrix.m41 = 0;
    rotationmatrix.m42 = 0;
    rotationmatrix.m43 = 0;
    rotationmatrix.m44 = 1;
    
    CSQuaternion::rotationMatrix(rotationmatrix, rotation);
    return true;
}

bool CSMatrix::decomposeUniformScale(float& scale, CSQuaternion& rotation, CSVector3& trans) const {
    trans.x = m41;
    trans.y = m42;
    trans.z = m43;
    
    scale = CSMath::sqrt((m11 * m11) + (m12 * m12) + (m13 * m13));
    
    float invscale = 1 / scale;
    
    if (CSMath::abs(scale) < 1e-6f) {
        rotation = CSQuaternion::Identity;
        return false;
    }
    
    CSMatrix rotationmatrix;
    rotationmatrix.m11 = m11 * invscale;
    rotationmatrix.m12 = m12 * invscale;
    rotationmatrix.m13 = m13 * invscale;
    
    rotationmatrix.m21 = m21 * invscale;
    rotationmatrix.m22 = m22 * invscale;
    rotationmatrix.m23 = m23 * invscale;
    
    rotationmatrix.m31 = m31 * invscale;
    rotationmatrix.m32 = m32 * invscale;
    rotationmatrix.m33 = m33 * invscale;
    
    rotationmatrix.m44 = 1;
    
    CSQuaternion::rotationMatrix(rotationmatrix, rotation);
    return true;
}

void CSMatrix::lerp(const CSMatrix& start, const CSMatrix& end, float amount, CSMatrix& result) {
    result.m11 = CSMath::lerp(start.m11, end.m11, amount);
    result.m12 = CSMath::lerp(start.m12, end.m12, amount);
    result.m13 = CSMath::lerp(start.m13, end.m13, amount);
    result.m14 = CSMath::lerp(start.m14, end.m14, amount);
    result.m21 = CSMath::lerp(start.m21, end.m21, amount);
    result.m22 = CSMath::lerp(start.m22, end.m22, amount);
    result.m23 = CSMath::lerp(start.m23, end.m23, amount);
    result.m24 = CSMath::lerp(start.m24, end.m24, amount);
    result.m31 = CSMath::lerp(start.m31, end.m31, amount);
    result.m32 = CSMath::lerp(start.m32, end.m32, amount);
    result.m33 = CSMath::lerp(start.m33, end.m33, amount);
    result.m34 = CSMath::lerp(start.m34, end.m34, amount);
    result.m41 = CSMath::lerp(start.m41, end.m41, amount);
    result.m42 = CSMath::lerp(start.m42, end.m42, amount);
    result.m43 = CSMath::lerp(start.m43, end.m43, amount);
    result.m44 = CSMath::lerp(start.m44, end.m44, amount);
}

void CSMatrix::transpose(const CSMatrix& value, CSMatrix& result) {
    CSMatrix temp;
    temp.m11 = value.m11;
    temp.m12 = value.m21;
    temp.m13 = value.m31;
    temp.m14 = value.m41;
    temp.m21 = value.m12;
    temp.m22 = value.m22;
    temp.m23 = value.m32;
    temp.m24 = value.m42;
    temp.m31 = value.m13;
    temp.m32 = value.m23;
    temp.m33 = value.m33;
    temp.m34 = value.m43;
    temp.m41 = value.m14;
    temp.m42 = value.m24;
    temp.m43 = value.m34;
    temp.m44 = value.m44;
    result = temp;
}

void CSMatrix::invert(const CSMatrix& value, CSMatrix& result) {
    float b0 = (value.m31 * value.m42) - (value.m32 * value.m41);
    float b1 = (value.m31 * value.m43) - (value.m33 * value.m41);
    float b2 = (value.m34 * value.m41) - (value.m31 * value.m44);
    float b3 = (value.m32 * value.m43) - (value.m33 * value.m42);
    float b4 = (value.m34 * value.m42) - (value.m32 * value.m44);
    float b5 = (value.m33 * value.m44) - (value.m34 * value.m43);
    
    float d11 = value.m22 * b5 + value.m23 * b4 + value.m24 * b3;
    float d12 = value.m21 * b5 + value.m23 * b2 + value.m24 * b1;
    float d13 = value.m21 * -b4 + value.m22 * b2 + value.m24 * b0;
    float d14 = value.m21 * b3 + value.m22 * -b1 + value.m23 * b0;
    
    float det = value.m11 * d11 - value.m12 * d12 + value.m13 * d13 - value.m14 * d14;
    
    if (CSMath::abs(det) == 0) {
        CSErrorLog("matrix invert fail");
        abort();
    }
    
    det = 1 / det;
    
    float a0 = (value.m11 * value.m22) - (value.m12 * value.m21);
    float a1 = (value.m11 * value.m23) - (value.m13 * value.m21);
    float a2 = (value.m14 * value.m21) - (value.m11 * value.m24);
    float a3 = (value.m12 * value.m23) - (value.m13 * value.m22);
    float a4 = (value.m14 * value.m22) - (value.m12 * value.m24);
    float a5 = (value.m13 * value.m24) - (value.m14 * value.m23);
    
    float d21 = value.m12 * b5 + value.m13 * b4 + value.m14 * b3;
    float d22 = value.m11 * b5 + value.m13 * b2 + value.m14 * b1;
    float d23 = value.m11 * -b4 + value.m12 * b2 + value.m14 * b0;
    float d24 = value.m11 * b3 + value.m12 * -b1 + value.m13 * b0;
    
    float d31 = value.m42 * a5 + value.m43 * a4 + value.m44 * a3;
    float d32 = value.m41 * a5 + value.m43 * a2 + value.m44 * a1;
    float d33 = value.m41 * -a4 + value.m42 * a2 + value.m44 * a0;
    float d34 = value.m41 * a3 + value.m42 * -a1 + value.m43 * a0;
    
    float d41 = value.m32 * a5 + value.m33 * a4 + value.m34 * a3;
    float d42 = value.m31 * a5 + value.m33 * a2 + value.m34 * a1;
    float d43 = value.m31 * -a4 + value.m32 * a2 + value.m34 * a0;
    float d44 = value.m31 * a3 + value.m32 * -a1 + value.m33 * a0;
    
    result.m11 = d11 * det;
    result.m12 = -d21 * det;
    result.m13 = d31 * det;
    result.m14 = -d41 * det;
    result.m21 = -d12 * det;
    result.m22 = d22 * det;
    result.m23 = -d32 * det;
    result.m24 = d42 * det;
    result.m31 = d13 * det;
    result.m32 = -d23 * det;
    result.m33 = d33 * det;
    result.m34 = -d43 * det;
    result.m41 = -d14 * det;
    result.m42 = d24 * det;
    result.m43 = -d34 * det;
    result.m44 = d44 * det;
}

void CSMatrix::lookAtLH(const CSVector3& eye, const CSVector3& target, const CSVector3& up, CSMatrix& result) {
    CSVector3 xaxis, yaxis, zaxis;
    
    zaxis = target - eye;
    zaxis.normalize();
    CSVector3::cross(up, zaxis, xaxis);
    xaxis.normalize();
    CSVector3::cross(zaxis, xaxis, yaxis);
    
    result = Identity;
    result.m11 = xaxis.x;
    result.m21 = xaxis.y;
    result.m31 = xaxis.z;
    result.m12 = yaxis.x;
    result.m22 = yaxis.y;
    result.m32 = yaxis.z;
    result.m13 = zaxis.x;
    result.m23 = zaxis.y;
    result.m33 = zaxis.z;
    
    result.m41 = -CSVector3::dot(xaxis, eye);
    result.m42 = -CSVector3::dot(yaxis, eye);
    result.m43 = -CSVector3::dot(zaxis, eye);
}

void CSMatrix::lookAtRH(const CSVector3& eye, const CSVector3& target, const CSVector3& up, CSMatrix& result) {
    CSVector3 xaxis, yaxis, zaxis;
    
    zaxis = eye - target;
    zaxis.normalize();
    CSVector3::cross(up, zaxis, xaxis);
    xaxis.normalize();
    CSVector3::cross(zaxis, xaxis, yaxis);
    
    result = Identity;
    result.m11 = xaxis.x;
    result.m21 = xaxis.y;
    result.m31 = xaxis.z;
    result.m12 = yaxis.x;
    result.m22 = yaxis.y;
    result.m32 = yaxis.z;
    result.m13 = zaxis.x;
    result.m23 = zaxis.y;
    result.m33 = zaxis.z;
    
    result.m41 = -CSVector3::dot(xaxis, eye);
    result.m42 = -CSVector3::dot(yaxis, eye);
    result.m43 = -CSVector3::dot(zaxis, eye);
}

void CSMatrix::orthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, CSMatrix& result) {
    float zrange = 1 / (zfar - znear);
    
    result = Identity;
    result.m11 = 2 / (right - left);
    result.m22 = 2 / (top - bottom);
    result.m33 = zrange;
    result.m41 = (left + right) / (left - right);
    result.m42 = (top + bottom) / (bottom - top);
    result.m43 = -znear * zrange;
}

void CSMatrix::perspectiveFovLH(float fov, float aspect, float znear, float zfar, CSMatrix& result) {
    float yScale = (float)(1 / CSMath::tan(fov * 0.5f));
    float xScale = yScale / aspect;
    
    float halfWidth = znear / xScale;
    float halfHeight = znear / yScale;
    
    return perspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, result);
}

void CSMatrix::perspectiveFovRH(float fov, float aspect, float znear, float zfar, CSMatrix& result) {
    float yScale = (float)(1 / CSMath::tan(fov * 0.5f));
    float xScale = yScale / aspect;
    
    float halfWidth = znear / xScale;
    float halfHeight = znear / yScale;
    
    return perspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, result);
}

void CSMatrix::perspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, CSMatrix& result) {
    float zrange = zfar / (zfar - znear);
    
    result.m11 = 2 * znear / (right - left);
    result.m12 = 0;
    result.m13 = 0;
    result.m14 = 0;
    result.m21 = 0;
    result.m22 = 2 * znear / (top - bottom);
    result.m23 = 0;
    result.m24 = 0;
    result.m31 = (left + right) / (left - right);
    result.m32 = (top + bottom) / (bottom - top);
    result.m33 = zrange;
    result.m34 = 1;
    result.m41 = 0;
    result.m42 = 0;
    result.m43 = -znear * zrange;
    result.m44 = 0;
}

void CSMatrix::rotationX(float angle, CSMatrix& result) {
    float cos = CSMath::cos(angle);
    float sin = CSMath::sin(angle);
    
    result = Identity;
    result.m22 = cos;
    result.m23 = sin;
    result.m32 = -sin;
    result.m33 = cos;
}


void CSMatrix::rotationY(float angle, CSMatrix& result) {
    float cos = CSMath::cos(angle);
    float sin = CSMath::sin(angle);
    
    result = Identity;
    result.m11 = cos;
    result.m13 = -sin;
    result.m31 = sin;
    result.m33 = cos;
}

void CSMatrix::rotationZ(float angle, CSMatrix& result) {
    float cos = CSMath::cos(angle);
    float sin = CSMath::sin(angle);
    
    result = Identity;
    result.m11 = cos;
    result.m12 = sin;
    result.m21 = -sin;
    result.m22 = cos;
}

void CSMatrix::rotationAxis(const CSVector3& axis, float angle, CSMatrix& result) {
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;
    float cos = CSMath::cos(angle);
    float sin = CSMath::sin(angle);
    float xx = x * x;
    float yy = y * y;
    float zz = z * z;
    float xy = x * y;
    float xz = x * z;
    float yz = y * z;
    
    result = Identity;
    result.m11 = xx + (cos * (1 - xx));
    result.m12 = (xy - (cos * xy)) + (sin * z);
    result.m13 = (xz - (cos * xz)) - (sin * y);
    result.m21 = (xy - (cos * xy)) - (sin * z);
    result.m22 = yy + (cos * (1 - yy));
    result.m23 = (yz - (cos * yz)) + (sin * x);
    result.m31 = (xz - (cos * xz)) + (sin * y);
    result.m32 = (yz - (cos * yz)) - (sin * x);
    result.m33 = zz + (cos * (1 - zz));
}

void CSMatrix::rotationQuaternion(const CSQuaternion& rotation, CSMatrix& result) {
    float xx = rotation.x * rotation.x;
    float yy = rotation.y * rotation.y;
    float zz = rotation.z * rotation.z;
    float xy = rotation.x * rotation.y;
    float zw = rotation.z * rotation.w;
    float zx = rotation.z * rotation.x;
    float yw = rotation.y * rotation.w;
    float yz = rotation.y * rotation.z;
    float xw = rotation.x * rotation.w;
    
    result = Identity;
    result.m11 = 1 - (2 * (yy + zz));
    result.m12 = 2 * (xy + zw);
    result.m13 = 2 * (zx - yw);
    result.m21 = 2 * (xy - zw);
    result.m22 = 1 - (2 * (zz + xx));
    result.m23 = 2 * (yz + xw);
    result.m31 = 2 * (zx + yw);
    result.m32 = 2 * (yz - xw);
    result.m33 = 1 - (2 * (yy + xx));
}

CSMatrix CSMatrix::operator +(const CSMatrix& mat) const {
    return CSMatrix(m11 + mat.m11, m12 + mat.m12, m13 + mat.m13, m14 + mat.m14,
                    m21 + mat.m21, m22 + mat.m22, m23 + mat.m23, m24 + mat.m24,
                    m31 + mat.m31, m32 + mat.m32, m33 + mat.m33, m34 + mat.m34,
                    m41 + mat.m41, m42 + mat.m42, m43 + mat.m43, m44 + mat.m44);
}

CSMatrix CSMatrix::operator -(const CSMatrix& mat) const {
    return CSMatrix(m11 - mat.m11, m12 - mat.m12, m13 + mat.m13, m14 + mat.m14,
                    m21 - mat.m21, m22 - mat.m22, m23 + mat.m23, m24 + mat.m24,
                    m31 - mat.m31, m32 - mat.m32, m33 + mat.m33, m34 + mat.m34,
                    m41 - mat.m41, m42 - mat.m42, m43 + mat.m43, m44 + mat.m44);
}

CSMatrix CSMatrix::operator -() const {
    return CSMatrix(-m11, -m12, -m13, -m14,
                    -m21, -m22, -m23, -m24,
                    -m31, -m32, -m33, -m34,
                    -m41, -m42, -m43, -m44);
}

CSMatrix CSMatrix::operator *(const CSMatrix& mat) const {
    CSMatrix temp;
    temp.m11 = (m11 * mat.m11) + (m12 * mat.m21) + (m13 * mat.m31) + (m14 * mat.m41);
    temp.m12 = (m11 * mat.m12) + (m12 * mat.m22) + (m13 * mat.m32) + (m14 * mat.m42);
    temp.m13 = (m11 * mat.m13) + (m12 * mat.m23) + (m13 * mat.m33) + (m14 * mat.m43);
    temp.m14 = (m11 * mat.m14) + (m12 * mat.m24) + (m13 * mat.m34) + (m14 * mat.m44);
    temp.m21 = (m21 * mat.m11) + (m22 * mat.m21) + (m23 * mat.m31) + (m24 * mat.m41);
    temp.m22 = (m21 * mat.m12) + (m22 * mat.m22) + (m23 * mat.m32) + (m24 * mat.m42);
    temp.m23 = (m21 * mat.m13) + (m22 * mat.m23) + (m23 * mat.m33) + (m24 * mat.m43);
    temp.m24 = (m21 * mat.m14) + (m22 * mat.m24) + (m23 * mat.m34) + (m24 * mat.m44);
    temp.m31 = (m31 * mat.m11) + (m32 * mat.m21) + (m33 * mat.m31) + (m34 * mat.m41);
    temp.m32 = (m31 * mat.m12) + (m32 * mat.m22) + (m33 * mat.m32) + (m34 * mat.m42);
    temp.m33 = (m31 * mat.m13) + (m32 * mat.m23) + (m33 * mat.m33) + (m34 * mat.m43);
    temp.m34 = (m31 * mat.m14) + (m32 * mat.m24) + (m33 * mat.m34) + (m34 * mat.m44);
    temp.m41 = (m41 * mat.m11) + (m42 * mat.m21) + (m43 * mat.m31) + (m44 * mat.m41);
    temp.m42 = (m41 * mat.m12) + (m42 * mat.m22) + (m43 * mat.m32) + (m44 * mat.m42);
    temp.m43 = (m41 * mat.m13) + (m42 * mat.m23) + (m43 * mat.m33) + (m44 * mat.m43);
    temp.m44 = (m41 * mat.m14) + (m42 * mat.m24) + (m43 * mat.m34) + (m44 * mat.m44);
    return temp;
}

CSMatrix CSMatrix::operator *(float value) const {
    return CSMatrix(m11 * value, m12 * value, m13 * value, m14 * value,
                    m21 * value, m22 * value, m23 * value, m24 * value,
                    m31 * value, m32 * value, m33 * value, m34 * value,
                    m41 * value, m42 * value, m43 * value, m44 * value);
}

CSMatrix CSMatrix::operator /(const CSMatrix& mat) const {
    return CSMatrix(m11 / mat.m11, m12 / mat.m12, m13 / mat.m13, m14 / mat.m14,
                    m21 / mat.m21, m22 / mat.m22, m23 / mat.m23, m24 / mat.m24,
                    m31 / mat.m31, m32 / mat.m32, m33 / mat.m33, m34 / mat.m34,
                    m41 / mat.m41, m42 / mat.m42, m43 / mat.m43, m44 / mat.m44);
}

CSMatrix CSMatrix::operator /(float value) const {
    float inv = 1 / value;
    
    return CSMatrix(m11 * inv, m12 * inv, m13 * inv, m14 * inv,
                    m21 * inv, m22 * inv, m23 * inv, m24 * inv,
                    m31 * inv, m32 * inv, m33 * inv, m34 * inv,
                    m41 * inv, m42 * inv, m43 * inv, m44 * inv);
}

uint CSMatrix::hash() const {
    CSHash hash;
    hash.combine(m11);
    hash.combine(m12);
    hash.combine(m13);
    hash.combine(m14);
    hash.combine(m21);
    hash.combine(m22);
    hash.combine(m23);
    hash.combine(m24);
    hash.combine(m31);
    hash.combine(m32);
    hash.combine(m33);
    hash.combine(m34);
    hash.combine(m41);
    hash.combine(m42);
    hash.combine(m43);
    hash.combine(m44);
    return hash;
}

bool CSMatrix::nearEqual(const CSMatrix& a, const CSMatrix& b) {
    return
        CSMath::nearEqual(a.m11, b.m11) &&
        CSMath::nearEqual(a.m12, b.m12) &&
        CSMath::nearEqual(a.m13, b.m13) &&
        CSMath::nearEqual(a.m14, b.m14) &&
        CSMath::nearEqual(a.m21, b.m21) &&
        CSMath::nearEqual(a.m22, b.m22) &&
        CSMath::nearEqual(a.m23, b.m23) &&
        CSMath::nearEqual(a.m24, b.m24) &&
        CSMath::nearEqual(a.m31, b.m31) &&
        CSMath::nearEqual(a.m32, b.m32) &&
        CSMath::nearEqual(a.m33, b.m33) &&
        CSMath::nearEqual(a.m34, b.m34) &&
        CSMath::nearEqual(a.m41, b.m41) &&
        CSMath::nearEqual(a.m42, b.m42) &&
        CSMath::nearEqual(a.m43, b.m43) &&
        CSMath::nearEqual(a.m44, b.m44);
}
