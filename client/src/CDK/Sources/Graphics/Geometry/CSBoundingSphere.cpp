#define CDK_IMPL

#include "CSBoundingSphere.h"

#include "CSABoundingBox.h"
#include "CSOBoundingBox.h"

#include "CSBuffer.h"

CSBoundingSphere::CSBoundingSphere(CSBuffer* buffer) : center(buffer), radius(buffer->readFloat()) {

}

CSBoundingSphere::CSBoundingSphere(const byte*& raw) : center(raw), radius(readFloat(raw)) {

}

void CSBoundingSphere::transform(const CSBoundingSphere& sphere, const CSMatrix& trans, CSBoundingSphere& result) {
    CSVector3::transformCoordinate(sphere.center, trans, result.center);
    result.radius = sphere.radius;

    CSVector3 scale;
    CSQuaternion rotation;
    CSVector3 translation;

    if (trans.decompose(scale, rotation, translation)) result.radius *= CSMath::max(CSMath::max(scale.x, scale.y), scale.z);
}

void CSBoundingSphere::fromPoints(const CSVector3* points, int count, CSBoundingSphere& result) {
    CSVector3 center(CSVector3::Zero);
    for (int i = 0; i < count; ++i) center += points[i];
    center = center / count;

    float radius = 0;
    for (int i = 0; i < count; ++i) {
        float r = CSVector3::distanceSquared(center, points[i]);
        if (r > radius) radius = r;
    }
    radius = CSMath::sqrt(radius);

    result.center = center;
    result.radius = radius;
}

void CSBoundingSphere::fromBox(const CSABoundingBox& box, CSBoundingSphere& result) {
    result.center = box.center();
    result.radius = box.extent().length();
}

void CSBoundingSphere::fromBox(const CSOBoundingBox& box, CSBoundingSphere& result) {
    result.center = box.center;
    result.radius = box.extent.length();
}

void CSBoundingSphere::append(const CSVector3& point) {
    float d = CSVector3::distance(center, point);

    if (d > radius) radius = d;
}

void CSBoundingSphere::append(const CSBoundingSphere& value1, const CSBoundingSphere& value2, CSBoundingSphere& result) {
    CSVector3 diff = value2.center - value1.center;
    
    float length = diff.length();
    float radius = value1.radius;
    float radius2 = value2.radius;
    
    if (radius + radius2 >= length) {
        if (radius - radius2 >= length) {
            result = value1;
            return;
        }
        
        if (radius2 - radius >= length) {
            result = value2;
            return;
        }
    }
    
    CSVector3 vector = diff * (1.0f / length);
    float min = CSMath::min(-radius, length - radius2);
    float max = (CSMath::max(radius, length + radius2) - min) * 0.5f;
    
    result.center = value1.center + vector * (max + min);
    result.radius = max;
}

uint CSBoundingSphere::hash() const {
    CSHash hash;
    hash.combine(center);
    hash.combine(radius);
    return hash;
}