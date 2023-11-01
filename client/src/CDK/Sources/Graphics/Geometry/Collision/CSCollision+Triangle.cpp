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

float CSCollision::TriangleGetZ(const CSTriangle& tri, const CSVector3& point) {        //TODO:테스트필요
    CSVector2 p0 = (CSVector2)tri.position0;
    CSVector2 v0 = (CSVector2)tri.position2 - p0;
    CSVector2 v1 = (CSVector2)tri.position1 - p0;
    CSVector2 v2 = (CSVector2)point - p0;

    float dot00 = CSVector2::dot(v0, v0);
    float dot01 = CSVector2::dot(v0, v1);
    float dot02 = CSVector2::dot(v0, v2);
    float dot11 = CSVector2::dot(v1, v1);
    float dot12 = CSVector2::dot(v1, v2);

    float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
    float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
    float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

    if (u >= 0 && v >= 0 && u + v < 1) {
        float z = tri.position0.z + u * (tri.position2.z - tri.position0.z) + v * (tri.position1.z - tri.position0.z);

        return z;
    }
    return 0;
}

void CSCollision::TriangleClosestPoint(const CSTriangle& tri, const CSVector3& point, CSVector3& result) {
    CSVector3 ab = tri.position1 - tri.position0;
    CSVector3 ac = tri.position2 - tri.position0;
    CSVector3 ap = point - tri.position0;

    float d1 = CSVector3::dot(ab, ap);
    float d2 = CSVector3::dot(ac, ap);
    if (d1 <= 0 && d2 <= 0) {
        result = tri.position0;
        return;
    }

    CSVector3 bp = point - tri.position1;
    float d3 = CSVector3::dot(ab, bp);
    float d4 = CSVector3::dot(ac, bp);
    if (d3 >= 0 && d4 <= d3) {
        result = tri.position1;
        return;
    }

    float vc = d1 * d4 - d3 * d2;
    if (vc <= 0 && d1 >= 0 && d3 <= 0) {
        float v = d1 / (d1 - d3);
        result = tri.position0 + ab * v;
        return;
    }

    CSVector3 cp = point - tri.position2;
    float d5 = CSVector3::dot(ab, cp);
    float d6 = CSVector3::dot(ac, cp);
    if (d6 >= 0 && d5 <= d6) {
        result = tri.position2;
        return;
    }

    float vb = d5 * d2 - d1 * d6;
    if (vb <= 0 && d2 >= 0 && d6 <= 0) {
        float w = d2 / (d2 - d6);
        result = tri.position0 + ac * w;
        return;
    }

    float va = d3 * d6 - d5 * d4;
    if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0) {
        float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
        result = tri.position1 + (tri.position2 - tri.position1) * w;
        return;
    }

    float denom = 1 / (va + vb + vc);
    float v2 = vb * denom;
    float w2 = vc * denom;
    result = tri.position0 + ab * v2 + ac * w2;
}

CSCollisionResult CSCollision::TriangleIntersectsPoint(const CSTriangle& tri, const CSVector3& point) {
    CSPlane plane = tri.plane();
    CSCollisionResult result = PlaneIntersectsPoint(plane, point);

    if (result == CSCollisionResultIntersects) {
        CSVector3 a = tri.position0 - point;
        CSVector3 b = tri.position1 - point;
        CSVector3 c = tri.position2 - point;

        CSVector3 u = CSVector3::cross(b, c);
        CSVector3 v = CSVector3::cross(c, a);
        CSVector3 w = CSVector3::cross(a, b);

        if (CSVector3::dot(u, v) < 0 || CSVector3::dot(u, w) < 0) result = CSCollisionResultFront;
    }

    return result;
}

CSCollisionResult CSCollision::TriangleIntersectsSegment(const CSTriangle& tri, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear) {
    CSVector3 diff = seg.position1 - seg.position0;
    float length = diff.length();

    if (!CSMath::nearZero(length)) {
        CSRay ray(seg.position0, diff / length);
        float distance;
        bool intersects = RayIntersectsTriangle(ray, tri, distance);
        if (distance <= length) {
            segNear = ray.position + ray.direction * distance;
            if (intersects) return CSCollisionResultIntersects;
        }
        else segNear = seg.position1;

        if ((flags & CSCollisionFlagBack) != 0) {
            CSPlane plane = tri.plane();
            if (PlaneIntersectsPoint(plane, seg.position0) == CSCollisionResultBack && PlaneIntersectsPoint(plane, seg.position1) == CSCollisionResultBack) return CSCollisionResultBack;
        }
        return CSCollisionResultFront;
    }
    else {
        segNear = seg.position0;
        return TriangleIntersectsPoint(tri, segNear);
    }
}

static void TriangleScaleProjectOntoLine(const CSTriangle& tri, const CSVector3& direction, float& ext0, float& ext1) {
    float t = CSVector3::dot(direction, tri.position0);

    ext0 = ext1 = t;

    t = CSVector3::dot(direction, tri.position1);
    if (t < ext0) ext0 = t;
    else if (t > ext1) ext1 = t;

    t = CSVector3::dot(direction, tri.position2);
    if (t < ext0) ext0 = t;
    else if (t > ext1) ext1 = t;
}

static bool TriangleIntersectsTriangle(CSTriangle tri0, CSTriangle tri1) {
    CSVector3 origin = tri0.position0;
    tri0.position0 = CSVector3::Zero;
    tri0.position1 -= origin;
    tri0.position2 -= origin;
    tri1.position0 -= origin;
    tri1.position1 -= origin;
    tri1.position2 -= origin;

    CSVector3 e0[] = {
        tri0.position1 - tri0.position0,
        tri0.position2 - tri0.position1,
        tri0.position0 - tri0.position2
    };

    CSVector3 n0 = CSVector3::cross(e0[0], e0[1]);

    float ext00, ext01;
    TriangleScaleProjectOntoLine(tri1, n0, ext00, ext01);

    if (0 < ext00 || ext01 < 0) return false;

    CSVector3 e1[] = {
        tri1.position1 - tri1.position0,
        tri1.position2 - tri1.position1,
        tri1.position0 - tri1.position2
    };

    CSVector3 n1 = CSVector3::cross(e1[0], e1[1]);

    float proj = CSVector3::dot(n1, tri1.position0);
    float ext10, ext11;
    TriangleScaleProjectOntoLine(tri0, n1, ext10, ext11);
    if (proj < ext10 || ext11 < proj) return false;

    CSVector3 n0xn1 = CSVector3::cross(n0, n1);

    if (CSVector3::dot(n0xn1, n0xn1) > CSMath::ZeroTolerance) {
        for (int i1 = 0; i1 < 3; i1++) {
            for (int i0 = 0; i0 < 3; i0++) {
                CSVector3 direction = CSVector3::cross(e0[i0], e1[i1]);
                TriangleScaleProjectOntoLine(tri0, direction, ext00, ext01);
                TriangleScaleProjectOntoLine(tri1, direction, ext10, ext11);
                if (ext01 < ext10 || ext11 < ext00) return false;
            }
        }
    }
    else {
        for (int i0 = 0; i0 < 3; i0++) {
            CSVector3 direction = CSVector3::cross(n0, e0[i0]);
            TriangleScaleProjectOntoLine(tri0, direction, ext00, ext01);
            TriangleScaleProjectOntoLine(tri1, direction, ext10, ext11);
            if (ext01 < ext10 || ext11 < ext00) return false;
        }
        for (int i1 = 0; i1 < 3; i1++) {
            CSVector3 direction = CSVector3::cross(n1, e1[i1]);
            TriangleScaleProjectOntoLine(tri0, direction, ext00, ext01);
            TriangleScaleProjectOntoLine(tri1, direction, ext10, ext11);
            if (ext01 < ext10 || ext11 < ext00) return false;
        }
    }

    return true;
}

CSCollisionResult CSCollision::TriangleIntersectsTriangle(const CSTriangle& tri0, const CSTriangle& tri1, CSCollisionFlags flags) {
    if (::TriangleIntersectsTriangle(tri0, tri1)) return CSCollisionResultIntersects;

    if (flags & CSCollisionFlagBack) {
        CSPlane plane = tri0.plane();

        if (PlaneIntersectsPoint(plane, tri1.position0) == CSCollisionResultBack &&
            PlaneIntersectsPoint(plane, tri1.position1) == CSCollisionResultBack &&
            PlaneIntersectsPoint(plane, tri1.position2) == CSCollisionResultBack) {
            return CSCollisionResultBack;
        }
    }
    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::TriangleIntersectsSphere(const CSTriangle& tri, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) {
    if (SphereIntersectsTriangle(sphere, tri, flags & CSCollisionFlagHit, hit) != CSCollisionResultFront) {
        if (hit) hit->direction = -hit->direction;
        return CSCollisionResultIntersects;
    }
    if ((flags & CSCollisionFlagBack) && PlaneIntersectsSphere(tri.plane(), sphere) == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::TriangleIntersectsCapsule(const CSTriangle& tri, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    if (CapsuleIntersectsTriangle(capsule, tri, flags & CSCollisionFlagHit, segNear, hit) != CSCollisionResultFront) {
        if (hit) hit->direction = -hit->direction;
        return CSCollisionResultIntersects;
    }
    CSVector3 pnear;
    if ((flags & CSCollisionFlagBack) && PlaneIntersectsCapsule(tri.plane(), capsule, pnear) == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::TriangleIntersectsABox(const CSTriangle& tri, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    if (ABoxIntersectsTriangle(box, tri, flags & CSCollisionFlagHit, hit) != CSCollisionResultFront) {
        if (hit) hit->direction = -hit->direction;
        return CSCollisionResultIntersects;
    }
    if ((flags & CSCollisionFlagBack) && PlaneIntersectsABox(tri.plane(), box) == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::TriangleIntersectsOBox(const CSTriangle& tri, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    if (OBoxIntersectsTriangle(box, tri, flags & CSCollisionFlagHit, hit) != CSCollisionResultFront) {
        if (hit) hit->direction = -hit->direction;
        return CSCollisionResultIntersects;
    }
    if ((flags & CSCollisionFlagBack) && PlaneIntersectsOBox(tri.plane(), box) == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::TriangleIntersectsMesh(const CSTriangle& tri, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) {
    if (MeshIntersectsTriangle(mesh, tri, flags & CSCollisionFlagHit, hit) != CSCollisionResultFront) {
        if (hit) hit->direction = -hit->direction;
        return CSCollisionResultIntersects;
    }
    if ((flags & CSCollisionFlagBack) && PlaneIntersectsMesh(tri.plane(), mesh) == CSCollisionResultBack) return CSCollisionResultBack;
    return CSCollisionResultFront;
}
