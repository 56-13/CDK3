#define CDK_IMPL

#include "CSBoundingCapsule.h"

#include "CSEntry.h"

#include "CSBuffer.h"

CSBoundingCapsule::CSBoundingCapsule(CSBuffer* buffer) : position0(buffer), position1(buffer), radius(buffer->readFloat()) {

}

CSBoundingCapsule::CSBoundingCapsule(const byte*& raw) : position0(raw), position1(raw), radius(readFloat(raw)) {

}

void CSBoundingCapsule::transform(const CSBoundingCapsule& capsule, const CSQuaternion& rotation, CSBoundingCapsule& result) {
    CSVector3::transform(capsule.position0, rotation, result.position0);
    CSVector3::transform(capsule.position1, rotation, result.position1);
    result.radius = capsule.radius;
}

void CSBoundingCapsule::transform(const CSBoundingCapsule& capsule, const CSMatrix& trans, CSBoundingCapsule& result) {
    CSVector3::transformCoordinate(capsule.position0, trans, result.position0);
    CSVector3::transformCoordinate(capsule.position1, trans, result.position1);
    result.radius = capsule.radius;

    CSVector3 scale;
    CSQuaternion rotation;
    CSVector3 translation;

    if (trans.decompose(scale, rotation, translation)) result.radius *= CSMath::max(CSMath::max(scale.x, scale.y), scale.z);
}

uint CSBoundingCapsule::hash() const {
    CSHash hash;
    hash.combine(position0);
    hash.combine(position1);
    hash.combine(radius);
    return hash;
}
