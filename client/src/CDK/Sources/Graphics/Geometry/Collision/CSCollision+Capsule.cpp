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

float CSCollision::CapsuleGetZ(const CSBoundingCapsule& capsule, const CSVector3& point) {
    CSVector2 v = (CSVector2)capsule.position1 - (CSVector2)capsule.position0;

    float len2 = v.lengthSquared();

    CSVector3 segNear;

    if (CSMath::nearZero(len2)) {
        segNear = capsule.position0;
    }
    else {
        CSVector2 pv = (CSVector2)point - (CSVector2)capsule.position0;

        float t = CSVector2::dot(pv, v) / len2;

        if (t >= 1) segNear = capsule.position1;
        else if (t > 0) segNear = CSVector3::lerp(capsule.position0, capsule.position1, t);
        else segNear = capsule.position0;
    }

    float d = CSVector2::distance((CSVector2)point, (CSVector2)segNear);

    if (d <= capsule.radius) {
        float z = segNear.z + CSMath::sqrt(capsule.radius * capsule.radius - d * d);

        if (point.z >= z) return z;
    }

    return 0;
}

CSCollisionResult CSCollision::CapsuleIntersectsPoint(const CSBoundingCapsule& capsule, const CSVector3& point, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    SegmentToPoint(capsule.segment(), point, segNear);
    CSVector3 diff = segNear - point;
    float d2 = diff.lengthSquared();
    float r2 = capsule.radius * capsule.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = point;
        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = capsule.radius - d;
        }
    }

    if (d2 < r2 + CSMath::ZeroTolerance) return CSCollisionResultBack;

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsSegment(const CSBoundingCapsule& capsule, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit) {
    CSSegment cseg = capsule.segment();
    SegmentToSegment(cseg, seg, segNear0, segNear1);
    CSVector3 diff = segNear0 - segNear1;
    float d2 = diff.lengthSquared();
    float r2 = capsule.radius * capsule.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = segNear1;

        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = capsule.radius - d;
        }
    }

    if (flags & CSCollisionFlagBack) {
        CSVector3 pnear;
        SegmentToPoint(cseg, seg.position0, pnear);
        if (CSVector3::distanceSquared(seg.position0, pnear) < r2) {
            SegmentToPoint(cseg, seg.position1, pnear);
            if (CSVector3::distanceSquared(seg.position1, pnear) < r2) return CSCollisionResultBack;
        }
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsTriangle(const CSBoundingCapsule& capsule, const CSTriangle& tri, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    segNear = capsule.position0;

    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 e0 = tri.position1 - tri.position0;
    CSVector3 e1 = tri.position2 - tri.position0;
    CSVector3 e0e1 = CSVector3::cross(e0, e1);
    CSVector3 pn = CSVector3::normalize(e0e1);

    float cl = capsule.length();
    CSVector3 cn = capsule.normal();

    float denom = CSVector3::dot(pn, cn);
    if (CSMath::nearZero(denom)) return CSCollisionResultFront;

    CSVector3 po = tri.position0 + pn * (denom < 0 ? capsule.radius : -capsule.radius);

    float u = CSVector3::dot(pn, po - capsule.position0) / denom;

    if (u > cl) {
        segNear = capsule.position1;
        return CSCollisionResultFront;
    }

    CSVector3 p = capsule.position0 + cn * u;

    segNear = p;

    CSVector3 w = p - tri.position0;
    float d = CSVector3::dot(e0e1, e0e1);
    float y = CSVector3::dot(CSVector3::cross(e0, w), e0e1) / d; // γ=[(u×w)⋅n]/n²
    float b = CSVector3::dot(CSVector3::cross(w, e1), e0e1) / d; // β=[(w×v)⋅n]/n²
    float a = 1 - y - b;

    CSVector3 tempNear;

    if (a < 0 || a > 1 || b < 0 || b > 1 || y < 0 || y > 1) {
        CSVector3 np0, np1, np2;
        SegmentToPoint(CSSegment(tri.position0, tri.position1), p, np0);
        SegmentToPoint(CSSegment(tri.position1, tri.position2), p, np1);
        SegmentToPoint(CSSegment(tri.position2, tri.position0), p, np2);

        float d0 = CSVector3::distanceSquared(p, np0);
        float d1 = CSVector3::distanceSquared(p, np1);
        float d2 = CSVector3::distanceSquared(p, np2);

        CSVector3 va = tri.position0;
        CSVector3 vb = tri.position1;
        float dt = d0;

        if (d1 < dt) {
            dt = d1;
            va = tri.position1;
            vb = tri.position2;
        }
        if (d2 < dt) {
            va = tri.position2;
            vb = tri.position0;
        }

        if (!RayIntersectsCapsule(CSRay(capsule.position0, cn), CSBoundingCapsule(va, vb, capsule.radius), 0, u, tempNear, NULL) || u > cl) return CSCollisionResultFront;

        if (hitFlag) {
            p = capsule.position0 + cn * u;

            CSVector3 ba = vb - va;
            CSVector3 pa = p - va;
            float h = CSMath::clamp(CSVector3::dot(pa, ba) / CSVector3::dot(ba, ba), 0.0f, 1.0f);
            pn = (pa - ba * h) / capsule.radius;
        }
    }

    if (u < 0) return CSCollisionResultFront;

    if (hitFlag) {
        hit->position = p;
        hit->direction = pn;
        hit->distance = capsule.radius - CSVector3::distance(segNear, p);
    }

    if ((flags & CSCollisionFlagBack) &&
        CapsuleIntersectsPoint(capsule, tri.position0, CSCollisionFlagBack, tempNear, NULL) == CSCollisionResultBack &&
        CapsuleIntersectsPoint(capsule, tri.position1, CSCollisionFlagBack, tempNear, NULL) == CSCollisionResultBack &&
        CapsuleIntersectsPoint(capsule, tri.position2, CSCollisionFlagBack, tempNear, NULL) == CSCollisionResultBack) {
        return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsSphere(const CSBoundingCapsule& capsule, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    SegmentToPoint(capsule.segment(), sphere.center, segNear);
    CSVector3 diff = segNear - sphere.center;
    float d2 = diff.lengthSquared();
    float r2 = (capsule.radius + sphere.radius) * (capsule.radius + sphere.radius);

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->position = sphere.center + hit->direction * CSMath::min(d, sphere.radius);
            hit->distance = sphere.radius + capsule.radius - d;
        }
        else hit->position = sphere.center;
    }

    if ((flags & CSCollisionFlagBack) != 0 && capsule.radius > sphere.radius) {
        r2 = (capsule.radius - sphere.radius) * (capsule.radius - sphere.radius);

        if (d2 < r2) return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsCapsule(const CSBoundingCapsule& capsule0, const CSBoundingCapsule& capsule1, CSCollisionFlags flags, CSVector3& segNear0, CSVector3& segNear1, CSHit* hit) {
    CSSegment cseg0(capsule0.position0, capsule0.position1);
    CSSegment cseg1(capsule1.position0, capsule1.position1);
    SegmentToSegment(cseg0, cseg1, segNear0, segNear1);
    CSVector3 diff = segNear0 - segNear1;
    float d2 = diff.lengthSquared();
    float r2 = (capsule0.radius * capsule1.radius) * (capsule0.radius * capsule1.radius);

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->position = segNear1 + hit->direction * CSMath::min(d, capsule1.radius);
            hit->distance = capsule0.radius + capsule1.radius - d;
        }
        else hit->position = segNear1;
    }

    if ((flags & CSCollisionFlagBack) && capsule0.radius > capsule1.radius) {
        r2 = (capsule0.radius - capsule1.radius) * (capsule0.radius - capsule1.radius);

        if (d2 <= r2) {
            CSVector3 pnear0, pnear1;
            SegmentToPoint(cseg0, capsule1.position0, pnear0);
            if (CSVector3::distanceSquared(capsule1.position0, pnear0) < r2) {
                SegmentToPoint(cseg0, capsule1.position1, pnear1);
                if (CSVector3::distanceSquared(capsule1.position1, pnear1) < r2) return CSCollisionResultBack;
            }
        }
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsABox(const CSBoundingCapsule& capsule, const CSABoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    if (ABoxIntersectsCapsule(box, capsule, flags & CSCollisionFlagHit, segNear, hit) == CSCollisionResultFront) return CSCollisionResultFront;

    hit->direction = -hit->direction;

    if (flags & CSCollisionFlagBack) {
        CSSegment cseg = capsule.segment();
        float r2 = capsule.radius * capsule.radius;

        CSVector3 boxCorners[8];
        box.getCorners(boxCorners);
        for (int i = 0; i < 8; i++) {
            const CSVector3& p = boxCorners[i];
            CSVector3 cnear;
            SegmentToPoint(cseg, p, cnear);
            if (CSVector3::distanceSquared(p, cnear) >= r2) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsOBox(const CSBoundingCapsule& capsule, const CSOBoundingBox& box, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    if (OBoxIntersectsCapsule(box, capsule, flags & CSCollisionFlagHit, segNear, hit) == CSCollisionResultFront) return CSCollisionResultFront;

    hit->direction = -hit->direction;

    if (flags & CSCollisionFlagBack) {
        CSSegment cseg = capsule.segment();
        float r2 = capsule.radius * capsule.radius;
        CSVector3 boxCorners[8];
        box.getCorners(boxCorners);
        for (int i = 0; i < 8; i++) {
            const CSVector3& p = boxCorners[i];
            CSVector3 cnear;
            SegmentToPoint(cseg, p, cnear);
            if (CSVector3::distanceSquared(p, cnear) >= r2) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::CapsuleIntersectsMesh(const CSBoundingCapsule& capsule, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    if (MeshIntersectsCapsule(mesh, capsule, flags & CSCollisionFlagHit, segNear, hit) == CSCollisionResultFront) return CSCollisionResultFront;

    hit->direction = -hit->direction;

    if (flags & CSCollisionFlagBack) {
        foreach (const CSVector3&, p, mesh.vertices()) {
            CSVector3 pnear;
            if (CapsuleIntersectsPoint(capsule, p, CSCollisionFlagBack, pnear, NULL) != CSCollisionResultBack) return CSCollisionResultIntersects;
        }
        return CSCollisionResultBack;
    }
    else return CSCollisionResultIntersects;
}
