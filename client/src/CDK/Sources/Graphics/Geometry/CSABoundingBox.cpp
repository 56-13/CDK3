#define CDK_IMPL

#include "CSABoundingBox.h"

#include "CSOBoundingBox.h"
#include "CSBoundingSphere.h"
#include "CSBoundingCapsule.h"

#include "CSBuffer.h"

const CSABoundingBox CSABoundingBox::Zero(CSVector3::Zero, CSVector3::Zero);
const CSABoundingBox CSABoundingBox::None(CSVector3(FloatMax, FloatMax, FloatMax), CSVector3(FloatMin, FloatMin, FloatMin));
const CSABoundingBox CSABoundingBox::ViewSpace(-CSVector3::One, CSVector3::One);

CSABoundingBox::CSABoundingBox(CSBuffer* buffer) : minimum(buffer), maximum(buffer) {

}

CSABoundingBox::CSABoundingBox(const byte*& raw) : minimum(raw), maximum(raw) {

}

float CSABoundingBox::radius() const {
    return CSMath::max(CSMath::max(maximum.x - minimum.x, maximum.y - minimum.y), maximum.z - minimum.z) * 0.5f;
}

void CSABoundingBox::getCorners(const CSVector3& min, const CSVector3& max, CSVector3* result) {
    result[0] = CSVector3(min.x, max.y, max.z);
    result[1] = CSVector3(max.x, max.y, max.z);
    result[2] = CSVector3(max.x, min.y, max.z);
    result[3] = CSVector3(min.x, min.y, max.z);
    result[4] = CSVector3(min.x, max.y, min.z);
    result[5] = CSVector3(max.x, max.y, min.z);
    result[6] = CSVector3(max.x, min.y, min.z);
    result[7] = CSVector3(min.x, min.y, min.z);
}

void CSABoundingBox::transform(const CSABoundingBox& box, const CSQuaternion& rotation, CSABoundingBox& result) {
    CSVector3 corners[8];
    box.getCorners(corners);
    result = None;
    for (int i = 0; i < 8; i++) result.append(CSVector3::transform(corners[i], rotation));
}

void CSABoundingBox::transform(const CSABoundingBox& box, const CSMatrix& trans, CSABoundingBox& result) {
    CSVector3 corners[8];
    box.getCorners(corners);
    result = None;
    for (int i = 0; i < 8; i++) result.append(CSVector3::transformCoordinate(corners[i], trans));
}

void CSABoundingBox::fromPoints(const CSVector3* points, int count, CSABoundingBox& result) {
    result = None;
    for (int i = 0; i < count; i++) {
        const CSVector3& p = points[i];
        CSVector3::min(result.minimum, p, result.minimum);
        CSVector3::max(result.maximum, p, result.maximum);
    }
}

void CSABoundingBox::fromSphere(const CSBoundingSphere& sphere, CSABoundingBox& result) {
    result.minimum = sphere.center - sphere.radius;
    result.maximum = sphere.center + sphere.radius;
}

void CSABoundingBox::fromCapsule(const CSBoundingCapsule& capsule, CSABoundingBox& result) {
    CSVector3::min(capsule.position0, capsule.position1, result.minimum);
    CSVector3::max(capsule.position0, capsule.position1, result.maximum);
    result.minimum -= capsule.radius;
    result.maximum += capsule.radius;
}

void CSABoundingBox::append(const CSVector3& point) {
    CSVector3::min(minimum, point, minimum);
    CSVector3::max(maximum, point, maximum);
}

void CSABoundingBox::append(const CSVector3& point, const CSMatrix& worldViewProjection) {
    CSVector4 vp = CSVector3::transform(point, worldViewProjection);

    if (vp.w) append((CSVector3)vp / CSMath::abs(vp.w));
    else {
        CSVector3 cvp = (CSVector3)vp;
        if (cvp.x < 0) cvp.x = -2;
        else if (cvp.x > 0) cvp.x = 2;
        if (cvp.y < 0) cvp.y = -2;
        else if (cvp.y > 0) cvp.y = 2;
        if (cvp.z < 0) cvp.z = -2;
        else if (cvp.z > 0) cvp.z = 2;
        append(cvp);
    }
}

void CSABoundingBox::append(const CSABoundingBox& box) {
    CSVector3::min(minimum, box.minimum, minimum);
    CSVector3::max(maximum, box.maximum, maximum);
}

void CSABoundingBox::append(const CSABoundingBox& value1, const CSABoundingBox& value2, CSABoundingBox& result) {
     CSVector3::min(value1.minimum, value2.minimum, result.minimum);
     CSVector3::max(value1.maximum, value2.maximum, result.maximum);
}

uint CSABoundingBox::hash() const {
    CSHash hash;
    hash.combine(minimum);
    hash.combine(maximum);
    return hash;
}
