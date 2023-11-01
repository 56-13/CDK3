#ifndef __CDK__CSBoundingSphere__
#define __CDK__CSBoundingSphere__

#include "CSCollision.h"

#include "CSArray.h"

#include "CSMatrix.h"

struct CSBoundingSphere {
    CSVector3 center;
    float radius;
    
    CSBoundingSphere() = default;
    inline CSBoundingSphere(const CSVector3& center, float radius) : center(center), radius(radius) {}
    explicit  CSBoundingSphere(CSBuffer* buffer);
    explicit CSBoundingSphere(const byte*& raw);
    
    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsPoint(*this, point, flags, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::SphereIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsTriangle(*this, tri, flags, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsSphere(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsSphere(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSPlane& plane) {
        return CSCollision::PlaneIntersectsSphere(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsSphere(*this, sphere, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::SphereIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsABox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsOBox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::SphereIntersectsMesh(*this, mesh, flags, hit);
    }
    
    inline float getZ(const CSVector3& point) const {
        return CSCollision::SphereGetZ(*this, point);
    }

    static void transform(const CSBoundingSphere& sphere, const CSMatrix& trans, CSBoundingSphere& result);
    
    static inline CSBoundingSphere transform(const CSBoundingSphere& sphere, const CSMatrix& trans) {
        CSBoundingSphere result;
        transform(sphere, trans, result);
        return result;
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    static void fromPoints(const CSVector3* points, int count, CSBoundingSphere& result);
    static inline CSBoundingSphere fromPoints(const CSVector3* points, int count) {
        CSBoundingSphere result;
        fromPoints(points, count, result);
        return result;
    }
    static inline void fromPoints(const CSArray<CSVector3>* points, CSBoundingSphere& result) {
        fromPoints(points->pointer(), points->count(), result);
    }
    static inline CSBoundingSphere fromPoints(const CSArray<CSVector3>* points) {
        CSBoundingSphere result;
        fromPoints(points, result);
        return result;
    }
    static inline void fromPoints(const CSArray<CSVector3>* points, int start, int count, CSBoundingSphere& result) {
        CSAssert(start >= 0 && start + count < points->count(), "out of range");
        fromPoints(points->pointer() + start, count, result);
    }
    static inline CSBoundingSphere fromPoints(const CSArray<CSVector3>* points, int start, int count) {
        CSBoundingSphere result;
        fromPoints(points, start, count, result);
        return result;
    }

    static void fromBox(const CSABoundingBox& box, CSBoundingSphere& result);
    static inline CSBoundingSphere fromBox(const CSABoundingBox& box) {
        CSBoundingSphere result;
        fromBox(box, result);
        return result;
    }

    static void fromBox(const CSOBoundingBox& box, CSBoundingSphere& result);
    static inline CSBoundingSphere fromBox(const CSOBoundingBox& box) {
        CSBoundingSphere result;
        fromBox(box, result);
        return result;
    }

    void append(const CSVector3& point);
    static inline void append(const CSBoundingSphere& value, const CSVector3& point, CSBoundingSphere& result) {
        result = value;
        result.append(point);
    }
    static inline CSBoundingSphere append(const CSBoundingSphere& value, const CSVector3& point) {
        CSBoundingSphere result;
        append(value, point, result);
        return result;
    }
    static void append(const CSBoundingSphere& value1, const CSBoundingSphere& value2, CSBoundingSphere& result);
    static inline CSBoundingSphere append(const CSBoundingSphere& value1, const CSBoundingSphere& value2) {
        CSBoundingSphere result;
        append(value1, value2, result);
        return result;
    }

    uint hash() const;
    
    inline bool operator ==(const CSBoundingSphere& other) const {
        return center == other.center && radius == other.radius;
    }
    inline bool operator !=(const CSBoundingSphere& other) const {
        return !(*this == other);
    }
};

#endif
