#ifndef __CDK__CSBoundingMesh__
#define __CDK__CSBoundingMesh__

#include "CSCollision.h"

#include "CSArray.h"

#include "CSMatrix.h"

struct CSBoundingMesh {
    struct Face {
        CSVector3 normal;
        float min;
        float max;
        int indices[8];
        int indexCount;

        Face(const CSVector3& normal, float min, float max, const int* indices, int indexCount);

        uint hash() const;
        bool operator ==(const Face& other) const;
        inline bool operator !=(const Face& other) const {
            return !(*this == other);
        }
    };
private:
    CSArray<CSVector3> _vertices;
    CSArray<Face> _faces;
public:
    CSBoundingMesh();
    CSBoundingMesh(int vertexCapacity, int faceCapacity);
    CSBoundingMesh(const CSBoundingMesh& other);
    explicit CSBoundingMesh(CSBuffer* buffer);
    explicit CSBoundingMesh(const byte*& raw);

    CSVector3 center() const;
    float radius() const;

    inline const CSArray<CSVector3>* vertices() const {
        return &_vertices;
    }
    inline const CSArray<Face>* faces() const {
        return &_faces;
    }
    void addVertex(const CSVector3& vertex);
    bool addFace(int indexCount, int index, ...);
    bool addFace(const int* indices, int indexCount);

    inline CSCollisionResult intersects(const CSVector3& point, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsPoint(*this, point, flags, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::MeshIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsSegment(*this, seg, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsTriangle(*this, tri, flags, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, CSHit* hit = NULL) const {
        float distance;
        return CSCollision::RayIntersectsMesh(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const {
        return CSCollision::RayIntersectsMesh(ray, *this, flags, distance, hit);
    }
    inline bool intersects(const CSPlane& plane) const {
        return CSCollision::PlaneIntersectsMesh(plane, *this) == CSCollisionResultIntersects;
    }
    inline CSCollisionResult intersects(const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsSphere(*this, sphere, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSHit* hit = NULL) const {
        CSVector3 segNear;
        return CSCollision::MeshIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsCapsule(*this, capsule, flags, segNear, hit);
    }
    inline CSCollisionResult intersects(const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsABox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsOBox(*this, box, flags, hit);
    }
    inline CSCollisionResult intersects(const CSBoundingMesh& shape, CSCollisionFlags flags, CSHit* hit = NULL) const {
        return CSCollision::MeshIntersectsMesh(*this, shape, flags, hit);
    }
    
    inline float getZ(const CSVector3& point) const {
        return CSCollision::MeshGetZ(*this, point);
    }

    void transform(const CSQuaternion& rotation);
    void transform(const CSMatrix& trans);

    CSBoundingMesh& operator =(const CSBoundingMesh& other);

    uint hash() const;

    bool operator ==(const CSBoundingMesh& other) const;
    inline bool operator !=(const CSBoundingMesh& other) const {
        return !(*this == other);
    }
};

#endif
