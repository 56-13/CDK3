#ifndef __CDK__CSTriangle__
#define __CDK__CSTriangle__

#include "CSCollision.h"

#include "CSMatrix.h"
#include "CSPlane.h"

struct CSTriangle {
    CSVector3 position0, position1, position2;

    CSTriangle() = default;
    inline CSTriangle(const CSVector3& pos0, const CSVector3& pos1, const CSVector3& pos2) : position0(pos0), position1(pos1), position2(pos2) {}
    explicit CSTriangle(CSBuffer* buffer);
    explicit CSTriangle(const byte*& raw);

    inline CSVector3 normal() const {
        return CSVector3::normalize(CSVector3::cross(position1 - position0, position2 - position0));
    }
    inline CSVector3 center() const {
        return (position0 + position1 + position2) / 3;
    }
    inline CSPlane plane() const {
        return CSPlane(position0, position1, position2);
    }
    inline CSVector3* positions() {
        return &position0;
    }
    inline const CSVector3* positions() const {
        return &position0;
    }

    inline CSCollisionResult intersects(const CSVector3& point) const {
        return CSCollision::TriangleIntersectsPoint(*this, point);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags) const {
        CSVector3 segNear;
        return CSCollision::TriangleIntersectsSegment(*this, seg, flags, segNear);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear) const {
        return CSCollision::TriangleIntersectsSegment(*this, seg, flags, segNear);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags) const {
        return CSCollision::TriangleIntersectsTriangle(*this, tri, flags);
    }
    inline bool intersects(const CSRay& ray) const {
        float distance;
        return CSCollision::RayIntersectsTriangle(ray, *this, distance);
    }
    inline bool intersects(const CSRay& ray, float& distance) const {
        return CSCollision::RayIntersectsTriangle(ray, *this, distance);
    }
    inline bool intersects(const CSPlane& plane) const {
        return CSCollision::PlaneIntersectsTriangle(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) const {
        return CSCollision::TriangleIntersectsSphere(*this, sphere, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear;
        return CSCollision::TriangleIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) const {
        return CSCollision::TriangleIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit) const {
        return CSCollision::TriangleIntersectsABox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit) const {
        return CSCollision::TriangleIntersectsOBox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) const {
        return CSCollision::TriangleIntersectsMesh(*this, mesh, flags, hit);
    }
    
    inline float getZ(const CSVector3 point) const {
        return CSCollision::TriangleGetZ(*this, point);
    }

    static void transform(const CSTriangle& tri, const CSQuaternion& rotation, CSTriangle& result);
    static void transform(const CSTriangle& tri, const CSMatrix& trans, CSTriangle& result);

    static inline CSTriangle transform(const CSTriangle& tri, const CSQuaternion& rotation) {
        CSTriangle result;
        transform(tri, rotation, result);
        return result;
    }
    static inline CSTriangle transform(const CSTriangle& tri, const CSMatrix& trans) {
        CSTriangle result;
        transform(tri, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    uint hash() const;

    inline bool operator ==(const CSTriangle& other) const {
        return position0 == other.position0 && position1 == other.position1 && position2 == other.position2;
    }
    inline bool operator !=(const CSTriangle& other) const {
        return !(*this == other);
    }
};

#endif
