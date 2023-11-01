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

float CSCollision::ABoxGetZ(const CSABoundingBox& box, const CSVector3& point) {
    if (box.minimum.x <= point.x && point.x <= box.maximum.x &&
        box.minimum.y <= point.y && point.y <= box.maximum.y &&
        box.maximum.z <= point.z) {
        return box.maximum.z;
    }
    return 0;
}

void CSCollision::ABoxClosestPoint(const CSABoundingBox& box, const CSVector3& point, CSVector3& result) {
    CSVector3::clamp(point, box.minimum, box.maximum, result);
}

static const CSVector3 ABoxNormals[] = {
    CSVector3::UnitX, CSVector3::UnitY, CSVector3::UnitZ,
    -CSVector3::UnitX, -CSVector3::UnitY, -CSVector3::UnitZ
};

void CSCollision::ABoxClosestNormalInternal(const CSABoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance) {
    float distances[] = {
        box.maximum.x - point.x,
        box.maximum.y - point.y,
        box.maximum.z - point.z,
        point.x - box.minimum.x,
        point.y - box.minimum.y,
        point.z - box.minimum.z
    };
    distance = distances[0];
    normal = ABoxNormals[0];

    for (int i = 1; i < 6; i++) {
        if (distance > distances[i]) {
            distance = distances[i];
            normal = ABoxNormals[i];
        }
    }
}

static CSCollisionResult ABoxIntersects(const CSABoundingBox& box, const CSVector3* points, int count, CSVector3& inter) {
    inter = CSVector3::Zero;

    bool back = true;
    CSVector3 min(FloatMax);
    CSVector3 max(FloatMin);
    for (int i = 0; i < count; i++) {
        const CSVector3& p = points[i];
        min = CSVector3::min(min, p);
        max = CSVector3::max(max,p);
        back &=
            box.minimum.x < p.x&& p.x < box.maximum.x&&
            box.minimum.y < p.y&& p.y < box.maximum.y&&
            box.minimum.z < p.z&& p.z < box.maximum.z;
    }

    if (box.maximum.x < min.x || box.minimum.x > max.x ||
        box.maximum.y < min.y || box.minimum.y > max.y ||
        box.maximum.z < min.z || box.minimum.z > max.z) {
        
        return CSCollisionResultFront;
    }

    inter = (CSVector3::max(box.minimum, min) + CSVector3::min(box.maximum, max)) * 0.5f;

    return back ? CSCollisionResultBack : CSCollisionResultIntersects;
}

void CSCollision::ABoxClosestNormal(const CSABoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance) {
    CSVector3 boxNear = ABoxClosestPoint(box, point);
    CSVector3 diff = point - boxNear;
    float d2 = diff.lengthSquared();

    if (CSMath::nearZero(d2)) ABoxClosestNormalInternal(box, point, normal, distance);
    else {
        float d = CSMath::sqrt(d2);
        distance = -d;
        normal = diff / d;
    }
}

CSCollisionResult CSCollision::ABoxIntersectsPoint(const CSABoundingBox& box, const CSVector3& point, CSCollisionFlags flags, CSHit* hit) {
    if (box.minimum.x <= point.x && point.x <= box.maximum.x &&
        box.minimum.y <= point.y && point.y <= box.maximum.y &&
        box.minimum.z <= point.z && point.z <= box.maximum.z) {
        if ((flags & CSCollisionFlagHit) && hit) {
            hit->position = point;
            ABoxClosestNormalInternal(box, hit->position, hit->direction, hit->distance);
            hit->direction = -hit->direction;
        }

        if ((flags & CSCollisionFlagBack) &&
            point.x >= box.minimum.x + CSMath::ZeroTolerance &&
            point.x <= box.maximum.x - CSMath::ZeroTolerance &&
            point.y >= box.minimum.y + CSMath::ZeroTolerance &&
            point.y <= box.maximum.y - CSMath::ZeroTolerance &&
            point.z >= box.minimum.z + CSMath::ZeroTolerance &&
            point.z <= box.maximum.z - CSMath::ZeroTolerance) {
            return CSCollisionResultBack;
        }

        return CSCollisionResultIntersects;
    }

    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::ABoxIntersectsSegment(const CSABoundingBox& box, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit){
    CSVector3 diff = seg.position1 - seg.position0;
    float d2 = diff.lengthSquared();

    if (!CSMath::nearZero(d2)) {
        float d = CSMath::sqrt(d2);
        CSRay ray(seg.position0, diff / d);
        
        float distance;
        bool intersects = RayIntersectsABox(ray, box, CSCollisionFlagNear, distance, NULL);

        if (distance <= d) {
            segNear = ray.position + ray.direction * distance;

            if (intersects) {
                if ((flags & CSCollisionFlagHit) && hit) {
                    hit->position = segNear;
                    ABoxClosestNormalInternal(box, hit->position, hit->direction, hit->distance);
                    hit->direction = -hit->direction;
                }
                return CSCollisionResultIntersects;
            }
        }
        else segNear = seg.position1;
    }
    else segNear = seg.position0;

    return ABoxIntersectsPoint(box, segNear, flags, hit);
}

CSCollisionResult CSCollision::ABoxIntersectsTriangle(const CSABoundingBox& box, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) != 0 && hit;

    CSVector3 boxCorners[8];
    box.getCorners(boxCorners);
    CSVector3 triNormal = tri.normal();
    float triOffset = CSVector3::dot(triNormal, tri.position0);
    float boxMin, boxMax;
    project(triNormal, boxCorners, 8, boxMin, boxMax);
    if (boxMax < triOffset || boxMin > triOffset) return CSCollisionResultFront;

    CSVector3 inter0;
    switch (ABoxIntersects(box, tri.positions(), 3, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                TriangleClosestPoint(tri, box.center(), hit->position);
                hit->direction = triNormal;
                hit->distance = triOffset - boxMin;
            }
            return CSCollisionResultBack;
    }


    CSVector3 triEdges[] = {
        tri.position0 - tri.position1,
        tri.position1 - tri.position2,
        tri.position2 - tri.position0
    };

    CSVector3 inter1(CSVector3::Zero);
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
            CSVector3 axis = CSVector3::cross(triEdges[i], ABoxNormals[j]);
            float triBoxMin, triBoxMax, triMin, triMax;
            project(axis, boxCorners, 8, triBoxMin, triBoxMax);
            project(axis, tri.positions(), 3, triMin, triMax);
            if (triBoxMax <= triMin || triBoxMin >= triMax) return CSCollisionResultFront;
            if (hitFlag) inter1 += axis * ((CSMath::max(triBoxMin, triMin) + CSMath::min(triBoxMax, triMax)) * 0.5f);
        }
    }

    if (hitFlag) {
        inter1 /= 3;
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        TriangleClosestPoint(tri, inter, hit->position);
        hit->direction = tri.normal();
        hit->distance = triOffset - boxMin;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::ABoxIntersectsSphere(const CSABoundingBox& box, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 boxNear = ABoxClosestPoint(box, sphere.center);
    float d2 = CSVector3::distanceSquared(sphere.center, boxNear);
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;
        ABoxClosestNormal(box, sphere.center, hit->direction, hit->distance);
        hit->distance += sphere.radius;
        hit->direction = -hit->direction;
    }

    if ((flags & CSCollisionFlagBack) &&
        box.minimum.x + sphere.radius < sphere.center.x && sphere.center.x < box.maximum.x - sphere.radius &&
        box.minimum.y + sphere.radius < sphere.center.y && sphere.center.y < box.maximum.y - sphere.radius &&
        box.minimum.z + sphere.radius < sphere.center.z && sphere.center.z < box.maximum.z - sphere.radius) 
    {
        return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::ABoxIntersectsCapsule(const CSABoundingBox& box, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    CSCollisionResult result = ABoxIntersectsSegment(box, capsule.segment(), flags | CSCollisionFlagBack, segNear, hit);
    
    if (result == CSCollisionResultIntersects) return CSCollisionResultIntersects;

    if (result == CSCollisionResultBack) {
        if (flags & CSCollisionFlagBack &&
            box.minimum.x < segNear.x - capsule.radius && segNear.x + capsule.radius < box.maximum.x &&
            box.minimum.y < segNear.y - capsule.radius && segNear.y + capsule.radius < box.maximum.y &&
            box.minimum.z < segNear.z - capsule.radius && segNear.z + capsule.radius < box.maximum.z) {
            return CSCollisionResultBack;
        }
        return CSCollisionResultIntersects;
    }

    CSVector3 boxNear = ABoxClosestPoint(box, segNear);
    CSVector3 diff = boxNear - segNear;
    float d2 = diff.lengthSquared();
    float r2 = capsule.radius * capsule.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;

        if (!CSMath::nearZero(d2)) {
            float d = CSMath::sqrt(d2);
            hit->direction = diff / d;
            hit->distance = capsule.radius - d;
        }
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::ABoxIntersectsABox(const CSABoundingBox& box0, const CSABoundingBox& box1, CSCollisionFlags flags, CSHit* hit) {
    if (box0.maximum.x < box1.minimum.x || box0.minimum.x > box1.maximum.x ||
        box0.maximum.y < box1.minimum.y || box0.minimum.y > box1.maximum.y ||
        box0.maximum.z < box1.minimum.z || box0.minimum.z > box1.maximum.z) {
        return CSCollisionResultFront;
    }

    if (flags & CSCollisionFlagHit) {
        CSVector3 min = CSVector3::max(box0.minimum, box1.minimum);
        CSVector3 max = CSVector3::min(box0.maximum, box1.maximum);
        hit->position = (min + max) * 0.5f;
        float distance;
        ABoxClosestNormalInternal(box1, hit->position, hit->direction, distance);
        hit->distance = CSVector3::dot(hit->direction, max - min);
    }

    if (flags & CSCollisionFlagBack &&
        box0.minimum.x < box1.minimum.x && box0.maximum.x > box1.maximum.x &&
        box0.minimum.y < box1.minimum.y && box0.maximum.y > box1.maximum.y &&
        box0.minimum.z < box1.minimum.z && box0.maximum.z > box1.maximum.z) {
        return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::ABoxIntersectsOBox(const CSABoundingBox& box0, const CSOBoundingBox& box1, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 corners0[8];
    CSVector3 corners1[8];
    box0.getCorners(corners0);
    box1.getCorners(corners1);

    CSVector3 inter0;
    switch (ABoxIntersects(box0, corners1, 8, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (flags & CSCollisionFlagHit) {
                CSVector3 boxCenter0 = box0.center();
                float distance;
                OBoxClosestPoint(box1, boxCenter0, hit->position);
                OBoxClosestNormal(box1, boxCenter0, hit->direction, distance);

                float boxMin0, boxMax0, boxMin1, boxMax1;
                project(hit->direction, corners0, 8, boxMin0, boxMax0);
                project(hit->direction, corners1, 8, boxMin1, boxMax1);
                hit->distance = boxMax1 - boxMin0;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (OBoxIntersects(box1, corners0, 8, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if (flags & CSCollisionFlagHit) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        OBoxClosestPoint(box1, inter, hit->position);
        OBoxClosestNormal(box1, inter, hit->direction, distance);

        float boxMin0, boxMax0, boxMin1, boxMax1;
        project(hit->direction, corners0, 8, boxMin0, boxMax0);
        project(hit->direction, corners1, 8, boxMin1, boxMax1);
        hit->distance = boxMax1 - boxMin0;
    }
    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::ABoxIntersectsMesh(const CSABoundingBox& box, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 corners[8];

    box.getCorners(corners);

    CSVector3 inter0;
    switch (ABoxIntersects(box, mesh.vertices()->pointer(), mesh.vertices()->count(), inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (flags & CSCollisionFlagHit) {
                CSVector3 boxCenter = box.center();
                float distance;
                MeshClosestPoint(mesh, boxCenter, hit->position);
                MeshClosestNormal(mesh, boxCenter, hit->direction, distance);

                float meshMin, meshMax, boxMin, boxMax;
                project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
                project(hit->direction, corners, 8, boxMin, boxMax);
                hit->distance = boxMax - meshMin;
            }
            return CSCollisionResultBack;
    }
    CSVector3 inter1;
    if (MeshIntersects(mesh, corners, 8, 0, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if (flags & CSCollisionFlagHit) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        MeshClosestPoint(mesh, inter, hit->position);
        MeshClosestNormal(mesh, inter, hit->direction, distance);

        float meshMin, meshMax, boxMin, boxMax;
        project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
        project(hit->direction, corners, 8, boxMin, boxMax);
        hit->distance = boxMax - meshMin;
    }

    return CSCollisionResultIntersects;
}
