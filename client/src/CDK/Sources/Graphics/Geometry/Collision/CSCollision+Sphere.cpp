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

float CSCollision::SphereGetZ(const CSBoundingSphere& sphere, const CSVector3& point) {
    float d = CSVector2::distance((CSVector2)sphere.center, (CSVector2)point);

    if (d <= sphere.radius) {
        float z = sphere.center.z + CSMath::sqrt(sphere.radius * sphere.radius - d * d);

        if (point.z >= z) return z;
    }

    return 0;
}

CSCollisionResult CSCollision::SphereIntersectsPoint(const CSBoundingSphere& sphere, const CSVector3& point, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 diff = sphere.center - point;
    float d2 = diff.lengthSquared();
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2 - CSMath::ZeroTolerance) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = point;

        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = sphere.radius - d;
        }
    }

    if (d2 < r2 + CSMath::ZeroTolerance) return CSCollisionResultBack;
    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsSegment(const CSBoundingSphere& sphere, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    SegmentToPoint(seg, sphere.center, segNear);

    CSVector3 diff = sphere.center - segNear;
    float d2 = CSVector3::distanceSquared(sphere.center, segNear);
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = segNear;

        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = sphere.radius - d;
        }
    }

    if (flags & CSCollisionFlagBack) return CSCollisionResultIntersects;

    d2 = CSVector3::distanceSquared(sphere.center, seg.position0);
    if (d2 >= r2) return CSCollisionResultIntersects;
    d2 = CSVector3::distanceSquared(sphere.center, seg.position1);
    if (d2 >= r2) return CSCollisionResultIntersects;
    return CSCollisionResultBack;
}

CSCollisionResult CSCollision::SphereIntersectsTriangle(const CSBoundingSphere& sphere, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 a = tri.position0 - sphere.center;
    CSVector3 b = tri.position1 - sphere.center;
    CSVector3 c = tri.position2 - sphere.center;
    float aa = CSVector3::dot(a, a);
    float ab = CSVector3::dot(a, b);
    float ac = CSVector3::dot(a, c);
    float bb = CSVector3::dot(b, b);
    float bc = CSVector3::dot(b, c);
    float cc = CSVector3::dot(c, c);
    float r2 = sphere.radius * sphere.radius;

    bool back = aa < r2&& bb < r2&& cc < r2;

    if (!back) {
        CSVector3 v = CSVector3::cross(b - a, c - a);
        float d = CSVector3::dot(a, v);
        float e = CSVector3::dot(v, v);

        if ((d * d > r2 * e) ||
            (aa > r2 && ab > aa && ac > aa) ||
            (bb > r2 && ab > bb && bc > bb) ||
            (cc > r2 && ac > cc && bc > cc)) {
            return CSCollisionResultFront;
        }

        CSVector3 abv = b - a;
        CSVector3 bcv = c - b;
        CSVector3 cav = a - c;
        float d1 = ab - aa;
        float d2 = bc - bb;
        float d3 = ac - cc;
        float e1 = CSVector3::dot(abv, abv);
        float e2 = CSVector3::dot(bcv, bcv);
        float e3 = CSVector3::dot(cav, cav);
        CSVector3 q1 = a * e1 - abv * d1;
        CSVector3 q2 = b * e2 - bcv * d2;
        CSVector3 q3 = c * e3 - cav * d3;
        CSVector3 qc = c * e1 - q1;
        CSVector3 qa = a * e2 - q2;
        CSVector3 qb = b * e3 - q3;
        if ((CSVector3::dot(q1, q1) > r2 * e1 * e1 && CSVector3::dot(q1, qc) > 0) ||
            (CSVector3::dot(q2, q2) > r2 * e2 * e2 && CSVector3::dot(q2, qa) > 0) ||
            (CSVector3::dot(q3, q3) > r2 * e3 * e3 && CSVector3::dot(q3, qb) > 0)) {
            return CSCollisionResultFront;
        }
    }

    if ((flags & CSCollisionFlagHit) && hit) {
        TriangleClosestPoint(tri, sphere.center, hit->position);
        hit->direction = tri.normal();
        hit->distance = sphere.radius - CSVector3::dot(hit->direction, tri.position0 - sphere.center);
    }

    return back ? CSCollisionResultBack : CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsSphere(const CSBoundingSphere& sphere0, const CSBoundingSphere& sphere1, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 diff = sphere0.center - sphere1.center;
    float d2 = diff.lengthSquared();
    float r2 = (sphere0.radius + sphere1.radius) * (sphere0.radius + sphere1.radius);

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = sphere0.radius + sphere1.radius - d;
            hit->position = sphere1.center + hit->direction * CSMath::min(d, sphere1.radius);
        }
        else hit->position = sphere1.center;
    }

    if ((flags & CSCollisionFlagBack) && sphere0.radius > sphere1.radius) {
        r2 = (sphere0.radius - sphere1.radius) * (sphere0.radius - sphere1.radius);

        if (d2 < r2) return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsCapsule(const CSBoundingSphere& sphere, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    SegmentToPoint(CSSegment(capsule.position0, capsule.position1), sphere.center, segNear);

    CSVector3 diff = sphere.center - segNear;
    float d2 = diff.lengthSquared();
    float r2 = (sphere.radius + capsule.radius) * (sphere.radius + capsule.radius);

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = sphere.radius + capsule.radius - d;
            hit->position = segNear + hit->direction * CSMath::min(d, capsule.radius);
        }
        else hit->position = segNear;
    }

    if ((flags & CSCollisionFlagBack) && sphere.radius > capsule.radius) {
        r2 = (sphere.radius - capsule.radius) * (sphere.radius - capsule.radius);

        if (CSVector3::distanceSquared(sphere.center, capsule.position0) < r2 && CSVector3::distanceSquared(sphere.center, capsule.position1) < r2) {
            return CSCollisionResultBack;
        }
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsABox(const CSBoundingSphere& sphere, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 boxNear = CSVector3::clamp(sphere.center, box.minimum, box.maximum);
    float d2 = CSVector3::distanceSquared(sphere.center, boxNear);
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;
        ABoxClosestNormal(box, sphere.center, hit->direction, hit->distance);
        hit->distance += sphere.radius;
    }

    if (flags & CSCollisionFlagBack) {
        CSVector3 boxCorners[8];
        box.getCorners(boxCorners);
        for (int i = 0; i < 8; i++) {
            if (CSVector3::distanceSquared(boxCorners[i], sphere.center) >= r2) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsOBox(const CSBoundingSphere& sphere, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 proj, boxNear;
    OBoxClosestPoint(box, sphere.center, proj, boxNear);

    CSVector3 diff = boxNear - sphere.center;
    float d2 = diff.lengthSquared();
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;
        OBoxClosestNormal(box, sphere.center, proj, hit->direction, hit->distance);
        hit->distance += sphere.radius;
    }

    if (flags & CSCollisionFlagBack) {
        CSVector3 boxCorners[8];
        box.getCorners(boxCorners);
        for (int i = 0; i < 8; i++) {
            if (CSVector3::distanceSquared(boxCorners[i], sphere.center) >= r2) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::SphereIntersectsMesh(const CSBoundingSphere& sphere, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) {
    if (MeshIntersectsSphere(mesh, sphere, flags & CSCollisionFlagHit, hit) == CSCollisionResultFront) return CSCollisionResultFront;

    if (hit) hit->direction = -hit->direction;

    if (flags & CSCollisionFlagBack) {
        foreach (const CSVector3&, p, mesh.vertices()) {
            if (SphereIntersectsPoint(sphere, p, CSCollisionFlagBack, NULL) != CSCollisionResultBack) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}
