#ifndef __CDK__CSBoundingFrustum__
#define __CDK__CSBoundingFrustum__

#include "CSCollision.h"

#include "CSMatrix.h"

#include "CSPlane.h"

struct CSBoundingFrustum {
    CSMatrix matrix;
    CSPlane planes[6];      //left, right, top, bottom, znear, zfar
    
    CSBoundingFrustum() = default;
    CSBoundingFrustum(const CSMatrix& matrix);
    
    void getCorners(CSVector3* corners) const;
    
    inline CSCollisionResult intersects(const CSVector3& point) const {
        return CSCollision::FrustumIntersectsPoint(*this, point);
    }
    inline CSCollisionResult intersects(const CSSegment& seg) const {
        return CSCollision::FrustumIntersectsSegment(*this, seg);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri) const {
        return CSCollision::FrustumIntersectsTriangle(*this, tri);
    }
    inline bool intersects(const CSRay& ray) const {
        float distance0, distance1;
        return CSCollision::RayIntersectsFrustum(ray, *this, distance0, distance1);
    }
    inline bool intersects(const CSRay& ray, float& distance0, float& distance1) const {
        return CSCollision::RayIntersectsFrustum(ray, *this, distance0, distance1);
    }
    inline bool intersects(const CSPlane& plane) const {
        return CSCollision::PlaneIntersectsFrustum(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere) const {
        return CSCollision::FrustumIntersectsSphere(*this, sphere);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule) const {
        return CSCollision::FrustumIntersectsCapsule(*this, capsule);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box) const {
        return CSCollision::FrustumIntersectsABox(*this, box);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box) const {
        return CSCollision::FrustumIntersectsOBox(*this, box);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& mesh) const {
        return CSCollision::FrustumIntersectsMesh(*this, mesh);
    }
    inline CSCollisionResult intersects(const CSBoundingFrustum& frustum) const {
        return CSCollision::FrustumIntersectsFrustum(*this, frustum);
    }

    inline bool isOrthographic() const {
        return CSVector3::nearEqual(planes[0].normal, -planes[1].normal) && CSVector3::nearEqual(planes[2].normal, -planes[3].normal);
    }

    inline uint hash() const {
        return matrix.hash();
    }
    inline bool operator ==(const CSBoundingFrustum& other) const {
        return matrix == other.matrix;
    }
    inline bool operator !=(const CSBoundingFrustum& other) const {
        return matrix != other.matrix;
    }
};

#endif
