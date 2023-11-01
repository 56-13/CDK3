#ifndef __CDK__CSRay__
#define __CDK__CSRay__

#include "CSCollision.h"

struct CSRay {
    CSVector3 position, direction;
    
    CSRay() = default;
    inline CSRay(const CSVector3& pos, const CSVector3& dir) : position(pos), direction(dir) {}
    explicit CSRay(CSBuffer* buffer);
    explicit CSRay(const byte*& raw);

    inline float distance(const CSVector3& point) const {
        return CSVector3::cross(direction, point - position).length();
    }
    
    inline bool intersects(const CSSegment& seg) const {
        float distance;
        CSVector3 segNear;
        return CSCollision::RayIntersectsSegment(*this, seg, distance, segNear);
    }
    inline bool intersects(const CSSegment& seg, float& distance) const {
        CSVector3 segNear;
        return CSCollision::RayIntersectsSegment(*this, seg, distance, segNear);
    }
    inline bool intersects(const CSSegment& seg, float& distance, CSVector3& segNear) const {
        return CSCollision::RayIntersectsSegment(*this, seg, distance, segNear);
    }
    inline bool intersects(const CSSegment& seg, CSVector3& segNear) const {
        float distance;
        return CSCollision::RayIntersectsSegment(*this, seg, distance, segNear);
    }
    inline bool intersects(const CSTriangle& tri) const {
        float distance;
        return CSCollision::RayIntersectsTriangle(*this, tri, distance);
    }
    inline bool intersects(const CSTriangle& tri, float& distance) const {
        return CSCollision::RayIntersectsTriangle(*this, tri, distance);
    }
    inline bool intersects(const CSRay& ray) const {
        float distance0, distance1;
        return CSCollision::RayIntersectsRay(*this, ray, distance0, distance1);
    }
    inline bool intersects(const CSRay& ray, float& distance0, float& distance1) const {
        return CSCollision::RayIntersectsRay(*this, ray, distance0, distance1);
    }
    inline bool intersects(const CSPlane& plane) const {
        float distance;
        return CSCollision::RayIntersectsPlane(*this, plane, distance);
    }
    inline bool intersects(const CSPlane& plane, float& distance) const {
        return CSCollision::RayIntersectsPlane(*this, plane, distance);
    }
    inline bool intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsSphere(*this, sphere, flags, distance, hit);
    }
    inline bool intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsSphere(*this, sphere, flags, distance, hit);
    }
    inline bool intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        CSVector3 segNear;
        return CSCollision::RayIntersectsCapsule(*this, capsule, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::RayIntersectsCapsule(*this, capsule, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsCapsule(*this, capsule, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, float& distance, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsCapsule(*this, capsule, flags, distance, segNear, hit);
    }
    inline bool intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsABox(*this, box, flags, distance, hit);
    }
    inline bool intersects(const CSABoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsABox(*this, box, flags, distance, hit);
    }
    inline bool intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsOBox(*this, box, flags, distance, hit);
    }
    inline bool intersects(const CSOBoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsOBox(*this, box, flags, distance, hit);
    }
    inline bool intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsMesh(*this, mesh, flags, distance, hit);
    }
    inline bool intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsMesh(*this, mesh, flags, distance, hit);
    }
    inline bool intersects(const CSBoundingFrustum& frustum) const {
        float distance0, distance1;
        return CSCollision::RayIntersectsFrustum(*this, frustum, distance0, distance1);
    }
    inline bool intersects(const CSBoundingFrustum& frustum, float& distance0, float& distance1) const {
        return CSCollision::RayIntersectsFrustum(*this, frustum, distance0, distance1);
    }

    static void transform(const CSRay& ray, const CSQuaternion& rotation, CSRay& result);
    static void transform(const CSRay& ray, const CSMatrix& trans, CSRay& result);

    static inline CSRay transform(const CSRay& ray, const CSQuaternion& rotation) {
        CSRay result;
        transform(ray, rotation, result);
        return result;
    }
    static inline CSRay transform(const CSRay& ray, const CSMatrix& trans) {
        CSRay result;
        transform(ray, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    uint hash() const;
    
    inline bool operator ==(const CSRay& value) const {
        return position == value.position && direction == value.direction;
    }
    inline bool operator !=(const CSRay& value) const {
        return !(*this == value);
    }
};

#endif
