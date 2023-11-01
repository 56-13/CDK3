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

CSCollisionResult CSCollision::FrustumIntersectsPoint(const CSBoundingFrustum& frustum, const CSVector3& point) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsPoint(frustum.planes[i], point)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsSegment(const CSBoundingFrustum& frustum, const CSSegment& seg) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        CSVector3 segNear;
        switch (PlaneIntersectsSegment(frustum.planes[i], seg, segNear)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsTriangle(const CSBoundingFrustum& frustum, const CSTriangle& tri) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsTriangle(frustum.planes[i], tri)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsSphere(const CSBoundingFrustum& frustum, const CSBoundingSphere& sphere) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsSphere(frustum.planes[i], sphere)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsCapsule(const CSBoundingFrustum& frustum, const CSBoundingCapsule& capsule) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        CSVector3 segNear;
        switch (PlaneIntersectsCapsule(frustum.planes[i], capsule, segNear)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsABox(const CSBoundingFrustum& frustum, const CSABoundingBox& box) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsABox(frustum.planes[i], box)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsOBox(const CSBoundingFrustum& frustum, const CSOBoundingBox& box) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsOBox(frustum.planes[i], box)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsMesh(const CSBoundingFrustum& frustum, const CSBoundingMesh& mesh) {
    CSCollisionResult result = CSCollisionResultBack;

    for (int i = 0; i < 6; i++) {
        switch (PlaneIntersectsMesh(frustum.planes[i], mesh)) {
            case CSCollisionResultBack:
                return CSCollisionResultFront;
            case CSCollisionResultIntersects:
                result = CSCollisionResultIntersects;
                break;
        }
    }
    return result;
}

CSCollisionResult CSCollision::FrustumIntersectsFrustum(const CSBoundingFrustum& frustum0, const CSBoundingFrustum& frustum1) {
    CSVector3 frustumCorners1[8];
    frustum1.getCorners(frustumCorners1);

    CSCollisionResult result = FrustumIntersectsPoint(frustum0, frustumCorners1[0]);
    for (int i = 1; i < 8; i++) {
        if (FrustumIntersectsPoint(frustum0, frustumCorners1[i]) != result) return CSCollisionResultIntersects;
    }
    return result;
}
