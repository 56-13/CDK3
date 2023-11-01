#define CDK_IMPL

#include "CSRay.h"

#include "CSEntry.h"

#include "CSBuffer.h"

CSRay::CSRay(CSBuffer* buffer) : position(buffer), direction(buffer) {

}

CSRay::CSRay(const byte*& raw) : position(raw), direction(raw) {

}

void CSRay::transform(const CSRay& ray, const CSQuaternion& rotation, CSRay& result) {
    CSVector3::transform(ray.direction, rotation, result.direction);
}

void CSRay::transform(const CSRay& ray, const CSMatrix& trans, CSRay& result) {
    CSVector3::transformCoordinate(ray.position, trans, result.position);
    CSVector3::transformNormal(ray.direction, trans, result.direction);
    result.direction.normalize();
}

uint CSRay::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(direction);
    return hash;
}
