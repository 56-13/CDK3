#define CDK_IMPL

#include "CSOBoundingBox.h"

#include "CSMatrix.h"

#include "CSEntry.h"

const CSOBoundingBox CSOBoundingBox::Zero(CSVector3::Zero, CSVector3::Zero, CSVector3::UnitX, CSVector3::UnitY, CSVector3::UnitZ);

CSOBoundingBox::CSOBoundingBox(CSBuffer* buffer) : center(buffer), extent(buffer) {
    axis[0] = CSVector3(buffer);
    axis[1] = CSVector3(buffer);
    axis[2] = CSVector3(buffer);
}

CSOBoundingBox::CSOBoundingBox(const byte*& raw) : center(raw), extent(raw) {
    axis[0] = CSVector3(raw);
    axis[1] = CSVector3(raw);
    axis[2] = CSVector3(raw);
}

CSOBoundingBox::CSOBoundingBox(const CSVector3& center, const CSVector3& extent, const CSVector3& axisx, const CSVector3& axisy, const CSVector3& axisz) :
    center(center),
    extent(extent),
    axis { axisx, axisy, axisz }
{
}

CSOBoundingBox::CSOBoundingBox(const CSABoundingBox& other) : CSOBoundingBox(other.center(), other.extent(), CSVector3::UnitX, CSVector3::UnitY, CSVector3::UnitZ)  {
    
}

float CSOBoundingBox::radius() const {
    return CSMath::max(CSMath::max(extent.x, extent.y), extent.z);
}

void CSOBoundingBox::getCorners(CSVector3* result) const {
    CSVector3 dx = axis[0] * extent.x + axis[0] * extent.y * axis[0] * extent.z;
    CSVector3 dy = axis[1] * extent.x + axis[1] * extent.y * axis[1] * extent.z;
    CSVector3 dz = axis[2] * extent.x + axis[2] * extent.y * axis[2] * extent.z;
    result[0] = center - dx - dy - dz;
    result[1] = center + dx - dy - dz;
    result[2] = center - dx + dy - dz;
    result[3] = center + dx + dy - dz;
    result[4] = center - dx - dy + dz;
    result[5] = center + dx - dy + dz;
    result[6] = center - dx + dy + dz;
    result[7] = center + dx + dy + dz;
}

void CSOBoundingBox::transform(const CSOBoundingBox& box, const CSQuaternion& rotation, CSOBoundingBox& result) {
    result.center = box.center;
    result.extent = box.extent;
    CSVector3::transform(box.axis[0], rotation, result.axis[0]);
    CSVector3::transform(box.axis[1], rotation, result.axis[1]);
    CSVector3::transform(box.axis[2], rotation, result.axis[2]);
}

void CSOBoundingBox::transform(const CSOBoundingBox& box, const CSMatrix& trans, CSOBoundingBox& result) {
    CSVector3 scale;
    CSQuaternion rotation;
    CSVector3 translation;

    if (!trans.decompose(scale, rotation, translation)) {
        result = box;
        return;
    }

    result.center = box.center + translation;
    result.extent = box.extent * scale;
    CSVector3::transform(box.axis[0], rotation, result.axis[0]);
    CSVector3::transform(box.axis[1], rotation, result.axis[1]);
    CSVector3::transform(box.axis[2], rotation, result.axis[2]);
}

bool CSOBoundingBox::isAligned() const {
    return
        (axis[0].nearEqual(CSVector3::UnitX) || axis[0].nearEqual(CSVector3::UnitY) || axis[0].nearEqual(CSVector3::UnitZ) ||
            axis[0].nearEqual(-CSVector3::UnitX) || axis[0].nearEqual(-CSVector3::UnitY) || axis[0].nearEqual(-CSVector3::UnitZ)) &&
        (axis[1].nearEqual(CSVector3::UnitX) || axis[1].nearEqual(CSVector3::UnitY) || axis[1].nearEqual(CSVector3::UnitZ) ||
            axis[1].nearEqual(-CSVector3::UnitX) || axis[1].nearEqual(-CSVector3::UnitY) || axis[1].nearEqual(-CSVector3::UnitZ)) &&
        (axis[2].nearEqual(CSVector3::UnitX) || axis[2].nearEqual(CSVector3::UnitY) || axis[2].nearEqual(CSVector3::UnitZ) ||
            axis[2].nearEqual(-CSVector3::UnitX) || axis[2].nearEqual(-CSVector3::UnitY) || axis[2].nearEqual(-CSVector3::UnitZ));
}

void CSOBoundingBox::toAligned(CSABoundingBox& result) const {
    CSVector3 p[8];
    getCorners(p);
    CSABoundingBox::fromPoints(p, 8, result);
}

uint CSOBoundingBox::hash() const {
    CSHash hash;
    hash.combine(center);
    hash.combine(extent);
    hash.combine(axis[0]);
    hash.combine(axis[1]);
    hash.combine(axis[2]);
    return hash;
}

