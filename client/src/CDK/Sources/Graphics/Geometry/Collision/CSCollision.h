#ifndef __CDK__CSCollision__
#define __CDK__CSCollision__

#include "CSVector3.h"

struct CSRay;
struct CSPlane;
struct CSSegment;
struct CSTriangle;
struct CSABoundingBox;
struct CSOBoundingBox;
struct CSBoundingSphere;
struct CSBoundingCapsule;
struct CSBoundingMesh;
struct CSBoundingFrustum;

enum CSCollisionResult {
    CSCollisionResultFront,
    CSCollisionResultBack,
    CSCollisionResultIntersects
};

enum CSCollisionFlag {
    CSCollisionFlagNone = 0,
    CSCollisionFlagBack = 1,
    CSCollisionFlagNear = 2,
    CSCollisionFlagHit = 4,
    CSCollisionFlagAll = 7
};

typedef int CSCollisionFlags;

struct CSHit {
public:
    CSVector3 position;
    CSVector3 direction;
    float distance;
};

class CSCollision {
private:
    //Common
    static void project(const CSVector3& axis, const CSVector3* points, int count, float& min, float& max);
    static void project(const CSVector3& axis, const CSVector3* points, int count, const CSVector3& offset, float& min, float& max);
public:
    //Ray
    static bool RayIntersectsPoint(const CSRay& ray, const CSVector3& point, float& distance);
    static bool RayIntersectsSegment(const CSRay& ray, const CSSegment& seg, float& distance, CSVector3& segNear);
    static bool RayIntersectsTriangle(const CSRay& ray, const CSTriangle& tri, float& distance);
    static bool RayIntersectsRay(const CSRay& ray0, const CSRay& ray1, float& distance0, float& distance1);
    static bool RayIntersectsPlane(const CSRay& ray, const CSPlane& plane, float& distance);
    static bool RayIntersectsSphere(const CSRay& ray, const CSBoundingSphere& sphere, CSCollisionFlags flags, float& distance, CSHit* hit);
    static bool RayIntersectsCapsule(const CSRay& ray, const CSBoundingCapsule& capsule, CSCollisionFlags flags, float& distance, CSVector3& segNear, CSHit* hit);
    static bool RayIntersectsABox(const CSRay& ray, const CSABoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit);
    static bool RayIntersectsOBox(const CSRay& ray, const CSOBoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit);
    static bool RayIntersectsMesh(const CSRay& ray, const CSBoundingMesh& mesh, CSCollisionFlags flags, float& distance, CSHit* hit);
    static bool RayIntersectsFrustum(const CSRay& ray, const CSBoundingFrustum& frustum, float& distance0, float& distance1);

    //Plane
    static float PlaneGetZ(const CSPlane& plane, const CSVector3& point);
    static CSCollisionResult PlaneIntersectsPoint(const CSPlane& plane, const CSVector3& point);
    static CSCollisionResult PlaneIntersectsSegment(const CSPlane& plane, const CSSegment& seg, CSVector3& segNear);
    static CSCollisionResult PlaneIntersectsTriangle(const CSPlane& plane, const CSTriangle& tri);
    static bool PlaneIntersectsPlane(const CSPlane& plane1, const CSPlane& plane2, CSRay* line);
    static CSCollisionResult PlaneIntersectsSphere(const CSPlane& plane, const CSBoundingSphere& sphere);
    static CSCollisionResult PlaneIntersectsCapsule(const CSPlane& plane, const CSBoundingCapsule& capsule, CSVector3& segNear);
    static CSCollisionResult PlaneIntersectsABox(const CSPlane& plane, const CSABoundingBox& box);
    static CSCollisionResult PlaneIntersectsOBox(const CSPlane& plane, const CSOBoundingBox& box);
    static CSCollisionResult PlaneIntersectsMesh(const CSPlane& plane, const CSBoundingMesh& mesh);
    static CSCollisionResult PlaneIntersectsFrustum(const CSPlane& plane, const CSBoundingFrustum& frustum);

    //Segment
    static void SegmentToPoint(const CSSegment& seg, const CSVector3& point, CSVector3& segNear);
    static void SegmentToSegment(const CSSegment& seg0, const CSSegment& seg1, CSVector3& segNear0, CSVector3& segNear1);

    //Triangle
    static float TriangleGetZ(const CSTriangle& tri, const CSVector3& point);
    static void TriangleClosestPoint(const CSTriangle& tri, const CSVector3& point, CSVector3& result);
    static inline CSVector3 TriangleClosestPoint(const CSTriangle& tri, const CSVector3& point) {
        CSVector3 result;
        TriangleClosestPoint(tri, point, result);
        return result;
    }
    static CSCollisionResult TriangleIntersectsPoint(const CSTriangle& tri, const CSVector3& point);
    static CSCollisionResult TriangleIntersectsSegment(const CSTriangle& tri, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear);
    static CSCollisionResult TriangleIntersectsTriangle(const CSTriangle& tri0, const CSTriangle& tri1, CSCollisionFlags flags);
    static CSCollisionResult TriangleIntersectsSphere(const CSTriangle& tri, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult TriangleIntersectsCapsule(const CSTriangle& tri, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult TriangleIntersectsABox(const CSTriangle& tri, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult TriangleIntersectsOBox(const CSTriangle& tri, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult TriangleIntersectsMesh(const CSTriangle& tri, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit);

    //Sphere
    static float SphereGetZ(const CSBoundingSphere& sphere, const CSVector3& point);
    static CSCollisionResult SphereIntersectsPoint(const CSBoundingSphere& sphere, const CSVector3& point, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult SphereIntersectsSegment(const CSBoundingSphere& sphere, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult SphereIntersectsTriangle(const CSBoundingSphere& sphere, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult SphereIntersectsSphere(const CSBoundingSphere& sphere0, const CSBoundingSphere& sphere1, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult SphereIntersectsCapsule(const CSBoundingSphere& sphere, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult SphereIntersectsABox(const CSBoundingSphere& sphere, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult SphereIntersectsOBox(const CSBoundingSphere& sphere, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult SphereIntersectsMesh(const CSBoundingSphere& sphere, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit);

    //Capsule
    static float CapsuleGetZ(const CSBoundingCapsule& capsule, const CSVector3& point);
    static CSCollisionResult CapsuleIntersectsPoint(const CSBoundingCapsule& capsule, const CSVector3& point, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsSegment(const CSBoundingCapsule& capsule, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsTriangle(const CSBoundingCapsule& capsule, const CSTriangle& tri, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsSphere(const CSBoundingCapsule& capsule, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsCapsule(const CSBoundingCapsule& capsule0, const CSBoundingCapsule& capsule1, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsABox(const CSBoundingCapsule& capsule, const CSABoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsOBox(const CSBoundingCapsule& capsule, const CSOBoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult CapsuleIntersectsMesh(const CSBoundingCapsule& capsule, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);

    //ABox
    static float ABoxGetZ(const CSABoundingBox& box, const CSVector3& point);
private:
    static void ABoxClosestNormalInternal(const CSABoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance);
public:
    static void ABoxClosestPoint(const CSABoundingBox& box, const CSVector3& point, CSVector3& result);
    static inline CSVector3 ABoxClosestPoint(const CSABoundingBox& box, const CSVector3& point) {
        CSVector3 result;
        ABoxClosestPoint(box, point, result);
        return result;
    }
    static void ABoxClosestNormal(const CSABoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance);
    static CSCollisionResult ABoxIntersects(const CSABoundingBox& box, const CSVector3* points, int count, CSVector3& inter);
    static CSCollisionResult ABoxIntersectsPoint(const CSABoundingBox& box, const CSVector3& point, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult ABoxIntersectsSegment(const CSABoundingBox& box, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult ABoxIntersectsTriangle(const CSABoundingBox& box, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult ABoxIntersectsSphere(const CSABoundingBox& box, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult ABoxIntersectsCapsule(const CSABoundingBox& box, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult ABoxIntersectsABox(const CSABoundingBox& box0, const CSABoundingBox& box1, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult ABoxIntersectsOBox(const CSABoundingBox& box0, const CSOBoundingBox& box1, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult ABoxIntersectsMesh(const CSABoundingBox& box, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit);

    //OBox
    static float OBoxGetZ(const CSOBoundingBox& box, const CSVector3& point);
private:
    static void OBoxProject(const CSOBoundingBox& box, const CSVector3& point, CSVector3& proj);
    static void OBoxClosestPoint(const CSOBoundingBox& box, const CSVector3& point, CSVector3& proj, CSVector3& result);
    static void OBoxClosestNormalInternal(const CSOBoundingBox& box, const CSVector3& proj, CSVector3& normal, float& distance);
    static void OBoxClosestNormal(const CSOBoundingBox& box, const CSVector3& point, const CSVector3& proj, CSVector3& normal, float& distance);
public:
    static void OBoxClosestPoint(const CSOBoundingBox& box, const CSVector3& point, CSVector3& result);
    static inline CSVector3 OBoxClosestPoint(const CSOBoundingBox& box, const CSVector3& point) {
        CSVector3 result;
        OBoxClosestPoint(box, point, result);
        return result;
    }
    static void OBoxClosestNormal(const CSOBoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance);
    static CSCollisionResult OBoxIntersects(const CSOBoundingBox& box, const CSVector3* points, int count, CSVector3& inter);
    static CSCollisionResult OBoxIntersectsPoint(const CSOBoundingBox& box, const CSVector3& point, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult OBoxIntersectsSegment(const CSOBoundingBox& box, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult OBoxIntersectsTriangle(const CSOBoundingBox& box, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult OBoxIntersectsSphere(const CSOBoundingBox& box, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult OBoxIntersectsCapsule(const CSOBoundingBox& box, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult OBoxIntersectsABox(const CSOBoundingBox& box0, const CSABoundingBox& box1, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult OBoxIntersectsOBox(const CSOBoundingBox& box0, const CSOBoundingBox& box1, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult OBoxIntersectsMesh(const CSOBoundingBox& box, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit);

    //Mesh
    static float MeshGetZ(const CSBoundingMesh& mesh, const CSVector3& point);
    static void MeshClosestPoint(const CSBoundingMesh& mesh, const CSVector3& point, CSVector3& result);
    static inline CSVector3 MeshClosestPoint(const CSBoundingMesh& mesh, const CSVector3& point) {
        CSVector3 result;
        MeshClosestPoint(mesh, point, result);
        return result;
    }
    static void MeshClosestNormal(const CSBoundingMesh& mesh, const CSVector3& point, CSVector3& normal, float& distance);
    static CSCollisionResult MeshIntersects(const CSBoundingMesh& mesh, const CSVector3* points, int count, float radius, CSVector3& inter);
    static CSCollisionResult MeshIntersectsPoint(const CSBoundingMesh& mesh, const CSVector3& point, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult MeshIntersectsSegment(const CSBoundingMesh& mesh, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult MeshIntersectsTriangle(const CSBoundingMesh& mesh, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult MeshIntersectsSphere(const CSBoundingMesh& mesh, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult MeshIntersectsCapsule(const CSBoundingMesh& mesh, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit);
    static CSCollisionResult MeshIntersectsABox(const CSBoundingMesh& mesh, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult MeshIntersectsOBox(const CSBoundingMesh& mesh, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit);
    static CSCollisionResult MeshIntersectsMesh(const CSBoundingMesh& mesh0, const CSBoundingMesh& mesh1, CSCollisionFlags flags, CSHit* hit);

    //Frustum
    static CSCollisionResult FrustumIntersectsPoint(const CSBoundingFrustum& frustum, const CSVector3& point);
    static CSCollisionResult FrustumIntersectsSegment(const CSBoundingFrustum& frustum, const CSSegment& seg);
    static CSCollisionResult FrustumIntersectsTriangle(const CSBoundingFrustum& frustum, const CSTriangle& tri);
    static CSCollisionResult FrustumIntersectsSphere(const CSBoundingFrustum& frustum, const CSBoundingSphere& sphere);
    static CSCollisionResult FrustumIntersectsCapsule(const CSBoundingFrustum& frustum, const CSBoundingCapsule& capsule);
    static CSCollisionResult FrustumIntersectsABox(const CSBoundingFrustum& frustum, const CSABoundingBox& box);
    static CSCollisionResult FrustumIntersectsOBox(const CSBoundingFrustum& frustum, const CSOBoundingBox& box);
    static CSCollisionResult FrustumIntersectsMesh(const CSBoundingFrustum& frustum, const CSBoundingMesh& mesh);
    static CSCollisionResult FrustumIntersectsFrustum(const CSBoundingFrustum& frustum0, const CSBoundingFrustum& frustum1);
};

#endif
