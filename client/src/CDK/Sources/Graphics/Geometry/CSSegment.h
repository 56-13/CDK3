#ifndef __CDK__CSSegment__
#define __CDK__CSSegment__

#include "CSCollision.h"

struct CSSegment {
    CSVector3 position0, position1;

    CSSegment() = default;
    inline CSSegment(const CSVector3& pos0, const CSVector3& pos1) : position0(pos0), position1(pos1) {}
    explicit CSSegment(CSBuffer* buffer);
    explicit CSSegment(const byte*& raw);

    inline CSVector3 center() const {
        return (position0 + position1) * 0.5f;
    }
    inline float lengthSquared() const {
        return CSVector3::distanceSquared(position0, position1);
    }
    inline float length() const {
        return CSVector3::distance(position0, position1);
    }

    inline bool Intersects(const CSVector3& point) const {
        CSVector3 segNear;
        CSCollision::SegmentToPoint(*this, point, segNear);
        return point.nearEqual(segNear);
    }
    inline bool Intersects(const CSVector3& point, CSVector3& segNear) const {
        CSCollision::SegmentToPoint(*this, point, segNear);
        return point.nearEqual(segNear);
    }
    inline bool Intersects(const CSSegment& seg) const {
        CSVector3 segNear0, segNear1;
        CSCollision::SegmentToSegment(*this, seg, segNear0, segNear1);
        return CSVector3::nearEqual(segNear0, segNear1);
    }
    inline bool Intersects(const CSSegment& seg, CSVector3& segNear0, CSVector3& segNear1) const {
        CSCollision::SegmentToSegment(*this, seg, segNear0, segNear1);
        return CSVector3::nearEqual(segNear0, segNear1);
    }
    inline bool Intersects(const CSTriangle& tri) const {
        CSVector3 segNear;
        return CSCollision::TriangleIntersectsSegment(tri, *this, CSCollisionFlagNone, segNear) == CSCollisionResultIntersects;
    }
    inline bool Intersects(const CSTriangle& tri, CSVector3& segNear) const {
        return CSCollision::TriangleIntersectsSegment(tri, *this, CSCollisionFlagNone, segNear) == CSCollisionResultIntersects;
    }
    inline bool Intersects(const CSRay& ray) const {
        float distance;
        CSVector3 segNear;
        return CSCollision::RayIntersectsSegment(ray, *this, distance, segNear);
    }
    inline bool Intersects(const CSRay& ray, float& distance) const {
        CSVector3 segNear;
        return CSCollision::RayIntersectsSegment(ray, *this, distance, segNear);
    }
    inline bool Intersects(const CSRay& ray, CSVector3& segNear) const {
        float distance;
        return CSCollision::RayIntersectsSegment(ray, *this, distance, segNear);
    }
    inline bool Intersects(const CSRay& ray, float& distance, CSVector3& segNear) const {
        return CSCollision::RayIntersectsSegment(ray, *this, distance, segNear);
    }
    inline bool Intersects(const CSPlane& plane) const {
        CSVector3 segNear;
        return CSCollision::PlaneIntersectsSegment(plane, *this, segNear) == CSCollisionResultIntersects;
    }
    inline bool Intersects(const CSPlane& plane, CSVector3& segNear) const {
        return CSCollision::PlaneIntersectsSegment(plane, *this, segNear) == CSCollisionResultIntersects;
    }
    inline bool Intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear;
        return CSCollision::SphereIntersectsSegment(sphere, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) const {
        return CSCollision::SphereIntersectsSegment(sphere, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear0, segNear1;
        return CSCollision::CapsuleIntersectsSegment(capsule, *this, flags, segNear0, segNear1, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit) const {
        return CSCollision::CapsuleIntersectsSegment(capsule, *this, flags, segNear0, segNear1, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear;
        return CSCollision::ABoxIntersectsSegment(box, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) const {
        return CSCollision::ABoxIntersectsSegment(box, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear;
        return CSCollision::OBoxIntersectsSegment(box, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) const {
        return CSCollision::OBoxIntersectsSegment(box, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) const {
        CSVector3 segNear;
        return CSCollision::MeshIntersectsSegment(mesh, *this, flags, segNear, hit) != CSCollisionResultFront;
    }
    inline bool Intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) const {
        return CSCollision::MeshIntersectsSegment(mesh, *this, flags, segNear, hit) != CSCollisionResultFront;
    }

    static void transform(const CSSegment& seg, const CSQuaternion& rotation, CSSegment& result);
    static void transform(const CSSegment& seg, const CSMatrix& trans, CSSegment& result);

    static inline CSSegment transform(const CSSegment& seg, const CSQuaternion& rotation) {
        CSSegment result;
        transform(seg, rotation, result);
        return result;
    }
    static inline CSSegment transform(const CSSegment& seg, const CSMatrix& trans) {
        CSSegment result;
        transform(seg, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }
    
    uint hash() const;

    inline bool operator ==(const CSSegment& other) const {
        return position0 == other.position0 && position1 == other.position1;
    }
    inline bool operator !=(const CSSegment& other) const {
        return !(*this == other);
    }
};

#endif
