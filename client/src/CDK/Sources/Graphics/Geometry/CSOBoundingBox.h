#ifndef __CDK__CSOBoundingBox__
#define __CDK__CSOBoundingBox__

#include "CSCollision.h"

#include "CSABoundingBox.h"

struct CSOBoundingBox {
    CSVector3 center, extent;
    CSVector3 axis[3];

    static const CSOBoundingBox Zero;

    CSOBoundingBox() = default;
    CSOBoundingBox(const CSVector3& center, const CSVector3& extent, const CSVector3& axisx, const CSVector3& axisy, const CSVector3& axisz);
    CSOBoundingBox(const CSABoundingBox& other);
    explicit CSOBoundingBox(CSBuffer* buffer);
    explicit  CSOBoundingBox(const byte*& raw);

    float radius() const;
    
    void getCorners(CSVector3* result) const;

    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsPoint(*this, point, flags, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::OBoxIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsTriangle(*this, tri, flags, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsOBox(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsOBox(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSPlane& plane) const {
        return CSCollision::PlaneIntersectsOBox(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsSphere(*this, sphere, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::OBoxIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsABox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsOBox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::OBoxIntersectsMesh(*this, mesh, flags, hit);
    }
    
    inline float getZ(const CSVector3& point) const {
        return CSCollision::OBoxGetZ(*this, point);
    }

    static void transform(const CSOBoundingBox& box, const CSQuaternion& rotation, CSOBoundingBox& result);
    static void transform(const CSOBoundingBox& box, const CSMatrix& trans, CSOBoundingBox& result);

    static inline CSOBoundingBox transform(const CSOBoundingBox& box, const CSQuaternion& rotation) {
        CSOBoundingBox result;
        transform(box, rotation, result);
        return result;
    }
    static inline CSOBoundingBox transform(const CSOBoundingBox& box, const CSMatrix& trans) {
        CSOBoundingBox result;
        transform(box, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    bool isAligned() const;

    void toAligned(CSABoundingBox& result) const;

    inline CSABoundingBox toAligned() const {
        CSABoundingBox result;
        toAligned(result);
        return result;
    }

    inline explicit operator CSABoundingBox() const {
        return toAligned();
    }

    uint hash() const;

    inline bool operator ==(const CSOBoundingBox& other) const {
        return memcmp(this, &other, sizeof(CSOBoundingBox)) == 0;
    }
    inline bool operator !=(const CSOBoundingBox& other) const {
        return !(*this == other);
    }
};

#endif
