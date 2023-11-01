#define CDK_IMPL

#include "CSCollision.h"

#include "CSRay.h"
#include "CSPlane.h"
#include "CSSegment.h"
#include "CSTriangle.h"
#include "CSABoundingBox.h"
#include "CSOBoundingBox.h"
#include "CSBoundingSphere.h"
#include "CSBoundingCapsule.h"
#include "CSBoundingMesh.h"
#include "CSBoundingFrustum.h"

float CSCollision::PlaneGetZ(const CSPlane& plane, const CSVector3& point) {
    CSRay ray(point, -CSVector3::UnitZ);
    float distance;
    if (RayIntersectsPlane(ray, plane, distance)) return point.z - distance;
    return 0;
}

CSCollisionResult CSCollision::PlaneIntersectsPoint(const CSPlane& plane, const CSVector3& point) {
    float distance = CSPlane::dotCoordinate(plane, point);

    if (distance > CSMath::ZeroTolerance) return CSCollisionResultFront;
    if (distance < -CSMath::ZeroTolerance) return CSCollisionResultBack;
    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::PlaneIntersectsSegment(const CSPlane& plane, const CSSegment& seg, CSVector3& segNear) {
    CSVector3 diff = seg.position1 - seg.position0;
    float length = diff.length();

    segNear = seg.position0;

    if (!CSMath::nearZero(length)) {
        CSRay ray(seg.position0, diff / length);
        float distance;
        if (RayIntersectsPlane(ray, plane, distance)) {
            if (distance <= length) {
                segNear = ray.position + ray.direction * distance;
                return CSCollisionResultIntersects;
            }
            segNear = seg.position1;
        }
    }

    return PlaneIntersectsPoint(plane, seg.position0);
}

CSCollisionResult CSCollision::PlaneIntersectsTriangle(const CSPlane& plane, const CSTriangle& tri) {
    CSCollisionResult test0 = PlaneIntersectsPoint(plane, tri.position0);
    CSCollisionResult test1 = PlaneIntersectsPoint(plane, tri.position1);
    CSCollisionResult test2 = PlaneIntersectsPoint(plane, tri.position2);

    if (test0 == CSCollisionResultFront && test1 == CSCollisionResultFront && test2 == CSCollisionResultFront) return CSCollisionResultFront;
    if (test0 == CSCollisionResultBack && test1 == CSCollisionResultBack && test2 == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultIntersects;
}

bool CSCollision::PlaneIntersectsPlane(const CSPlane& plane1, const CSPlane& plane2, CSRay* line) {
    CSVector3 direction = CSVector3::cross(plane1.normal, plane2.normal);

    float denom = CSVector3::dot(direction, direction);

    if (CSMath::nearZero(denom)) return false;

    if (line) {
        CSVector3 temp = plane2.normal * plane1.d - plane1.normal * plane2.d;
        line->position = CSVector3::cross(temp, direction);
        line->direction = CSVector3::normalize(direction);
    }
    return true;
}

CSCollisionResult CSCollision::PlaneIntersectsSphere(const CSPlane& plane, const CSBoundingSphere& sphere) {
    float distance = CSPlane::dotCoordinate(plane, sphere.center);

    if (distance > sphere.radius) return CSCollisionResultFront;
    if (distance < -sphere.radius) return CSCollisionResultBack;
    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::PlaneIntersectsCapsule(const CSPlane& plane, const CSBoundingCapsule& capsule, CSVector3& segNear) {
    CSVector3 diff = capsule.position1 - capsule.position0;
    float length = diff.length();

    if (!CSMath::nearZero(length)) {
        CSRay ray(capsule.position0, diff / length);

        float distance;
        if (RayIntersectsPlane(ray, plane, distance)) {
            if (distance <= length) {
                segNear = ray.position + ray.direction * distance;
                return CSCollisionResultIntersects;
            }
            else segNear = capsule.position1;

            return PlaneIntersectsSphere(plane, CSBoundingSphere(capsule.position1, capsule.radius));
        }
        else segNear = capsule.position0;
    }
    else segNear = capsule.position0;

    return PlaneIntersectsSphere(plane, CSBoundingSphere(capsule.position0, capsule.radius));
}

CSCollisionResult CSCollision::PlaneIntersectsABox(const CSPlane& plane, const CSABoundingBox& box) {
    CSVector3 min;
    CSVector3 max;
    float distance;

    max.x = (plane.normal.x >= 0) ? box.minimum.x : box.maximum.x;
    max.y = (plane.normal.y >= 0) ? box.minimum.y : box.maximum.y;
    max.z = (plane.normal.z >= 0) ? box.minimum.z : box.maximum.z;
    min.x = (plane.normal.x >= 0) ? box.maximum.x : box.minimum.x;
    min.y = (plane.normal.y >= 0) ? box.maximum.y : box.minimum.y;
    min.z = (plane.normal.z >= 0) ? box.maximum.z : box.minimum.z;

    distance = CSPlane::dotCoordinate(plane, max);
    if (distance > 0) return CSCollisionResultFront;

    distance = CSPlane::dotCoordinate(plane, min);
    if (distance < 0) return CSCollisionResultBack;

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::PlaneIntersectsOBox(const CSPlane& plane, const CSOBoundingBox& box) {
    float radius = box.extent.x * CSVector3::dot(plane.normal, box.axis[0]) +
        box.extent.y * CSVector3::dot(plane.normal, box.axis[1]) +
        box.extent.z * CSVector3::dot(plane.normal, box.axis[2]);

    float distance = CSPlane::dotCoordinate(plane, box.center);

    if (distance > radius) return CSCollisionResultFront;
    if (distance < radius) return CSCollisionResultBack;
    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::PlaneIntersectsMesh(const CSPlane& plane, const CSBoundingMesh& mesh) {
    CSCollisionResult result = PlaneIntersectsPoint(plane, mesh.vertices()->objectAtIndex(0));

    if (result == CSCollisionResultIntersects) return CSCollisionResultIntersects;

    for (int i = 1; i < mesh.vertices()->count(); i++) {
        if (PlaneIntersectsPoint(plane, mesh.vertices()->objectAtIndex(i)) != result) return CSCollisionResultIntersects;
    }

    return result;
}

CSCollisionResult CSCollision::PlaneIntersectsFrustum(const CSPlane& plane, const CSBoundingFrustum& frustum) {
    CSVector3 frustumCorners[8];
    frustum.getCorners(frustumCorners);

    CSCollisionResult result = PlaneIntersectsPoint(plane, frustumCorners[0]);

    if (result == CSCollisionResultIntersects) return CSCollisionResultIntersects;

    for (int i = 1; i < 8; i++) {
        if (result != PlaneIntersectsPoint(plane, frustumCorners[i])) return CSCollisionResultIntersects;
    }
    return result;
}
