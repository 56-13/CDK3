#ifndef __CDK__CSABoundingBox__
#define __CDK__CSABoundingBox__

#include "CSCollision.h"

#include "CSArray.h"

#include "CSMatrix.h"

struct CSABoundingBox {
    CSVector3 minimum, maximum;
    
    static const CSABoundingBox Zero;
    static const CSABoundingBox None;
    static const CSABoundingBox ViewSpace;

    CSABoundingBox() = default;
    inline CSABoundingBox(const CSVector3& pos) : minimum(pos), maximum(pos) {}
    inline CSABoundingBox(const CSVector3& min, const CSVector3& max) : minimum(min), maximum(max) {}
    explicit CSABoundingBox(CSBuffer* buffer);
    explicit CSABoundingBox(const byte*& raw);

    inline CSVector3 center() const {
        return (maximum + minimum) * 0.5f;
    }
    inline CSVector3 extent() const {
        return (maximum - minimum) * 0.5f;
    }

    float radius() const;

    static void getCorners(const CSVector3& min, const CSVector3& max, CSVector3* result);
    inline void getCorners(CSVector3* result) const {
        getCorners(minimum, maximum, result);
    }

    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsPoint(*this, point, flags, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::ABoxIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsTriangle(*this, tri, flags, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsABox(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsABox(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSPlane& plane) const {
        return CSCollision::PlaneIntersectsABox(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsSphere(*this, sphere, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::ABoxIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsABox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsOBox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::ABoxIntersectsMesh(*this, mesh, flags, hit);
    }
    
    inline float getZ(const CSVector3& point) const {
        return CSCollision::ABoxGetZ(*this, point);
    }

    static void transform(const CSABoundingBox& box, const CSQuaternion& rotation, CSABoundingBox& result);
    static void transform(const CSABoundingBox& box, const CSMatrix& trans, CSABoundingBox& result);

    static inline CSABoundingBox transform(const CSABoundingBox& box, const CSQuaternion& rotation) {
        CSABoundingBox result;
        transform(box, rotation, result);
        return result;
    }
    static inline CSABoundingBox transform(const CSABoundingBox& box, const CSMatrix& trans) {
        CSABoundingBox result;
        transform(box, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }

    static void fromPoints(const CSVector3* points, int count, CSABoundingBox& result);

    static inline CSABoundingBox fromPoints(const CSVector3* points, int count) {
        CSABoundingBox result;
        fromPoints(points, count, result);
        return result;
    }
    static inline void fromPoints(const CSArray<CSVector3>* points, CSABoundingBox& result) {
        fromPoints(points->pointer(), points->count(), result);
    }
    static inline CSABoundingBox fromPoints(const CSArray<CSVector3>* points) {
        CSABoundingBox result;
        fromPoints(points->pointer(), points->count(), result);
        return result;
    }
    static inline void fromPoints(const CSArray<CSVector3>* points, int offset, int count, CSABoundingBox& result) {
        CSAssert(offset >= 0 && offset + count < points->count(), "out of range");
        fromPoints(points->pointer() + offset, count, result);
    }
    static inline CSABoundingBox fromPoints(const CSArray<CSVector3>* points, int offset, int count) {
        CSABoundingBox result;
        fromPoints(points, offset, count, result);
        return result;
    }

    static void fromSphere(const CSBoundingSphere& sphere, CSABoundingBox& result);

    static inline CSABoundingBox fromSphere(const CSBoundingSphere& sphere) {
        CSABoundingBox result;
        fromSphere(sphere, result);
        return result;
    }

    static void fromCapsule(const CSBoundingCapsule& capsule, CSABoundingBox& result);

    static inline CSABoundingBox fromCapsule(const CSBoundingCapsule& capsule) {
        CSABoundingBox result;
        fromCapsule(capsule, result);
        return result;
    }

    void append(const CSVector3& point);
    void append(const CSVector3& point, const CSMatrix& worldViewProjection);

    void append(const CSABoundingBox& box);
    static void append(const CSABoundingBox& value1, const CSABoundingBox& value2, CSABoundingBox& result);
    static inline CSABoundingBox append(const CSABoundingBox& value1, const CSABoundingBox& value2) {
        CSABoundingBox result;
        append(value1, value2, result);
        return result;
    }

    uint hash() const;
    
    inline bool operator ==(const CSABoundingBox& other) const {
        return minimum == other.minimum && maximum == other.maximum;
    }
    inline bool operator !=(const CSABoundingBox& other) const {
        return !(*this == other);
    }
};

#endif
