#ifndef __CDK__CSPlane__
#define __CDK__CSPlane__

#include "CSCollision.h"

#include "CSMatrix.h"

struct CSPlane {
    CSVector3 normal;
    float d;

    CSPlane() = default;
    inline CSPlane(float a, float b, float c, float d) : normal(a, b, c), d(d) {}
    inline CSPlane(const CSVector3& point, const CSVector3& normal) : normal(normal), d(-CSVector3::dot(normal, point)) {}
    inline CSPlane(const CSVector3& normal, float d) : normal(normal), d(d) {}
    CSPlane(const CSVector3& point1, const CSVector3& point2, const CSVector3& point3);
    explicit CSPlane(CSBuffer* buffer);
    explicit CSPlane(const byte*& raw);

    static void normalize(const CSPlane& plane, CSPlane& result);
    static inline CSPlane normalize(const CSPlane& plane) {
        CSPlane result;
        normalize(plane, result);
        return result;
    }
    inline void normalize() {
        normalize(*this, *this);
    }

    inline CSCollisionResult intersects(const CSVector3& point) const {
        return CSCollision::PlaneIntersectsPoint(*this, point);
    }
    inline CSCollisionResult intersects(const CSSegment& seg) const {
        CSVector3 segNear;
        return CSCollision::PlaneIntersectsSegment(*this, seg, segNear);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSVector3& segNear) const {
        return CSCollision::PlaneIntersectsSegment(*this, seg, segNear);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri) const {
        return CSCollision::PlaneIntersectsTriangle(*this, tri);
    }
    inline bool intersects(const CSRay& ray) const {
        float distance;
        return CSCollision::RayIntersectsPlane(ray, *this, distance);
    }
    inline bool intersects(const CSRay& ray, float& distance) const {
        return CSCollision::RayIntersectsPlane(ray, *this, distance);
    }
    inline bool intersects(const CSPlane& plane, CSRay* line = NULL) const {
        return CSCollision::PlaneIntersectsPlane(plane, *this, line);
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere) const {
        return CSCollision::PlaneIntersectsSphere(*this, sphere);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule) const {
        CSVector3 segNear;
        return CSCollision::PlaneIntersectsCapsule(*this, capsule, segNear);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSVector3& segNear) const {
        return CSCollision::PlaneIntersectsCapsule(*this, capsule, segNear);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box) const {
        return CSCollision::PlaneIntersectsABox(*this, box);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box) const {
        return CSCollision::PlaneIntersectsOBox(*this, box);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh) const {
        return CSCollision::PlaneIntersectsMesh(*this, mesh);
    }
    inline CSCollisionResult intersects(const CSBoundingFrustum& frustum) const {
        return CSCollision::PlaneIntersectsFrustum(*this, frustum);
    }

    inline float getZ(const CSVector3& point) const {
        return CSCollision::PlaneGetZ(*this, point);
    }

    void reflection(CSMatrix& result) const;
    inline CSMatrix reflection() const {
        CSMatrix result;
        reflection(result);
        return result;
    }
    
    static inline float dot(const CSPlane& left, const CSVector4& right) {
        return (left.normal.x * right.x) + (left.normal.y * right.y) + (left.normal.z * right.z) + (left.d * right.w);
    }
    
    static inline float dotCoordinate(const CSPlane& left, const CSVector3& right) {
        return (left.normal.x * right.x) + (left.normal.y * right.y) + (left.normal.z * right.z) + left.d;
    }
    
    static inline float dotNormal(const CSPlane& left, const CSVector3& right) {
        return (left.normal.x * right.x) + (left.normal.y * right.y) + (left.normal.z * right.z);
    }
    
    static void transform(const CSPlane& plane, const CSQuaternion& rotation, CSPlane& result);
    static void transform(const CSPlane& plane, const CSMatrix& trans, CSPlane& result);
    
    static inline CSPlane transform(const CSPlane& plane, const CSQuaternion& rotation) {
        CSPlane result;
        transform(plane, rotation, result);
        return result;
    }
    static inline CSPlane transform(const CSPlane& plane, const CSMatrix& trans) {
        CSPlane result;
        transform(plane, trans, result);
        return result;
    }
    inline void transform(const CSQuaternion& rotation) {
        transform(*this, rotation, *this);
    }
    inline void transform(const CSMatrix& trans) {
        transform(*this, trans, *this);
    }
    
    void shadow(const CSVector4& light, CSMatrix& result) const;
    inline CSMatrix shadow(const CSVector4& light) const {
        CSMatrix result;
        shadow(light, result);
        return result;
    }
    
    inline CSPlane operator *(float scale) const {
        return CSPlane(normal.x * scale, normal.y * scale, normal.z * scale, d * scale);
    }
    inline CSPlane& operator *=(float scale) {
        normal.x *= scale;
        normal.y *= scale;
        normal.z *= scale;
        d *= scale;
        return *this;
    }

    uint hash() const;
    
    inline bool operator ==(const CSPlane& other) const {
        return normal == other.normal && d == other.d;
    }
    inline bool operator !=(const CSPlane& other) const {
        return !(*this == other);
    }
};

#endif
