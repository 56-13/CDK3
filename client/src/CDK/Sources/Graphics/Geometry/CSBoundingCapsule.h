#ifndef __CDK__CSBoundingCapsule__
#define __CDK__CSBoundingCapsule__

#include "CSCollision.h"

#include "CSMatrix.h"

#include "CSSegment.h"

struct CSBoundingCapsule {
    CSVector3 position0, position1;
    float radius;

    CSBoundingCapsule() = default;
    inline CSBoundingCapsule(const CSVector3& p0, const CSVector3& p1, float radius) : position0(p0), position1(p1), radius(radius) {}
    explicit CSBoundingCapsule(CSBuffer* buffer);
    explicit CSBoundingCapsule(const byte*& raw);

    inline CSVector3 center() const {
        return (position0 + position1) * 0.5f;
    }
    inline CSVector3 normal() const {
        return CSVector3::normalize(position1 - position0);
    }
    inline CSSegment segment() const {
        return CSSegment(position0, position1);
    }
    inline float lengthSquared() const {
        return CSVector3::distanceSquared(position0, position1);
    }
    inline float length() const {
        return CSVector3::distance(position0, position1);
    }

    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsPoint(*this, point, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsPoint(*this, point, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear0, segNear1;
        return CSCollision::CapsuleIntersectsSegment(*this, seg, flags, segNear0, segNear1, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsSegment(*this, seg, flags, segNear0, segNear1, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsTriangle(*this, tri, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsTriangle(*this, tri, flags, segNear, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        CSVector3 segNear;
        return CSCollision::RayIntersectsCapsule(ray, *this, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::RayIntersectsCapsule(ray, *this, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsCapsule(ray, *this, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsCapsule(ray, *this, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSPlane& plane) const {
        CSVector3 segNear;
        return CSCollision::PlaneIntersectsCapsule(plane, *this, segNear) == CSCollisionResultIntersects;
    }
    inline bool intersects(const CSPlane& plane, CSVector3& segNear) const {
        return CSCollision::PlaneIntersectsCapsule(plane, *this, segNear) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsSphere(*this, sphere, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsSphere(*this, sphere, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule1, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear0, segNear1;
        return CSCollision::CapsuleIntersectsCapsule(*this, capsule1, flags, segNear0, segNear1, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule1, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsCapsule(*this, capsule1, flags, segNear0, segNear1, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsABox(*this, box, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsABox(*this, box, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsOBox(*this, box, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsOBox(*this, box, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::CapsuleIntersectsMesh(*this, mesh, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::CapsuleIntersectsMesh(*this, mesh, flags, segNear, hit);
    }
    
    inline float getZ(const CSVector3& point) const {
        return CSCollision::CapsuleGetZ(*this, point);
    }

    static void transform(const CSBoundingCapsule& capsule, const CSQuaternion& rotation, CSBoundingCapsule& result);
    static void transform(const CSBoundingCapsule& capsule, const CSMatrix& trans, CSBoundingCapsule& result);

    static inline CSBoundingCapsule transform(const CSBoundingCapsule& capsule, const CSQuaternion& rotation) {
        CSBoundingCapsule result;
        transform(capsule, rotation, result);
        return result;
    }
    static inline CSBoundingCapsule transform(const CSBoundingCapsule& capsule, const CSMatrix& trans) {
        CSBoundingCapsule result;
        transform(capsule, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    uint hash() const;

    inline bool operator ==(const CSBoundingCapsule& other) const {
        return position0 == other.position0 && position1 == other.position1 && radius == other.radius;
    }
    inline bool operator !=(const CSBoundingCapsule& other) const {
        return !(*this == other);
    }
};

#endif
