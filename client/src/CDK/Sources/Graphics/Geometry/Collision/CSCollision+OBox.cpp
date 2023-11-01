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

float CSCollision::OBoxGetZ(const CSOBoundingBox& box, const CSVector3& point) {
    CSRay ray(point, -CSVector3::UnitZ);
    float distance;
    if (!RayIntersectsOBox(ray, box, 0, distance, NULL)) return 0;
    return CSMath::max(point.z - distance, 0.0f);
}

void CSCollision::OBoxProject(const CSOBoundingBox& box, const CSVector3& point, CSVector3& proj) {
    proj.x = CSVector3::dot(point - box.center, box.axis[0]);
    proj.y = CSVector3::dot(point - box.center, box.axis[1]);
    proj.z = CSVector3::dot(point - box.center, box.axis[2]);
}

void CSCollision::OBoxClosestPoint(const CSOBoundingBox& box, const CSVector3& point, CSVector3& proj, CSVector3& result) {
    OBoxProject(box, point, proj);
    CSVector3 diff = CSVector3::clamp(proj, -box.extent, box.extent);
    result = box.center + box.axis[0] * diff.x + box.axis[1] * diff.y + box.axis[2] * diff.z;
}

void CSCollision::OBoxClosestNormalInternal(const CSOBoundingBox& box, const CSVector3& proj, CSVector3& normal, float& distance) {
    float distances[] = {
        box.extent.x - proj.x,
        box.extent.y - proj.y,
        box.extent.z - proj.z,
        proj.x - box.extent.x,
        proj.y - box.extent.y,
        proj.z - box.extent.z
    };
    CSVector3 axis[] = {
        box.axis[0], box.axis[1], box.axis[2],
        -box.axis[0], -box.axis[1], -box.axis[2]
    };
    distance = distances[0];
    normal = axis[0];

    for (int i = 1; i < 6; i++) {
        if (distance > distances[i]) {
            distance = distances[i];
            normal = axis[i];
        }
    }
}

void CSCollision::OBoxClosestNormal(const CSOBoundingBox& box, const CSVector3& point, const CSVector3& proj, CSVector3& normal, float& distance) {
    if (-box.extent.x <= proj.x && proj.x <= box.extent.x &&
        -box.extent.y <= proj.y && proj.y <= box.extent.y &&
        -box.extent.z <= proj.z && proj.z <= box.extent.z) {
        OBoxClosestNormalInternal(box, proj, normal, distance);
    }
    else {
        CSVector3 offset = CSVector3::clamp(proj, -box.extent, box.extent);
        CSVector3 boxNear = box.center + box.axis[0] * offset.x + box.axis[1] * offset.y + box.axis[1] * offset.z;
        CSVector3 diff = boxNear - point;
        float d = diff.length();
        distance = -d;
        normal = diff / d;
    }
}

void CSCollision::OBoxClosestPoint(const CSOBoundingBox& box, const CSVector3& point, CSVector3& result) {
    CSVector3 proj;
    OBoxProject(box, point, proj);
    OBoxClosestPoint(box, point, proj, result);
}

void CSCollision::OBoxClosestNormal(const CSOBoundingBox& box, const CSVector3& point, CSVector3& normal, float& distance) {
    CSVector3 proj;
    OBoxProject(box, point, proj);
    OBoxClosestNormal(box, point, proj, normal, distance);
}

CSCollisionResult CSCollision::OBoxIntersects(const CSOBoundingBox& box, const CSVector3* points, int count, CSVector3& inter) {
    inter = CSVector3::Zero;
    
    bool contains = true;
    for (int i = 0; i < 3; i++) {
        const CSVector3& axis = box.axis[i];
        float min, max;
        project(axis, points, count, box.center, min, max);
        float boxMin = -box.extent[i];
        float boxMax = box.extent[i];
        if (max < boxMin || min > boxMax) return CSCollisionResultFront;
        contains &= boxMin < min&& max < boxMax;
        inter += axis * ((CSMath::max(boxMin, min) + CSMath::min(boxMax, max)) * 0.5f);
    }

    inter += box.center;

    return contains ? CSCollisionResultBack : CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::OBoxIntersectsPoint(const CSOBoundingBox& box, const CSVector3& point, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 proj;
    OBoxProject(box, point, proj);

    if (-box.extent.x <= proj.x && proj.x <= box.extent.x &&
        -box.extent.y <= proj.y && proj.y <= box.extent.y &&
        -box.extent.z <= proj.z && proj.z <= box.extent.z) {
        if ((flags & CSCollisionFlagHit) && hit) {
            hit->position = point;
            OBoxClosestNormalInternal(box, proj, hit->direction, hit->distance);
            hit->direction = -hit->direction;
        }

        if ((flags & CSCollisionFlagBack) &&
            proj.x >= -box.extent.x + CSMath::ZeroTolerance &&
            proj.x <= box.extent.x - CSMath::ZeroTolerance &&
            proj.y >= -box.extent.y + CSMath::ZeroTolerance &&
            proj.y <= box.extent.y - CSMath::ZeroTolerance &&
            proj.z >= -box.extent.z + CSMath::ZeroTolerance &&
            proj.z <= box.extent.z - CSMath::ZeroTolerance) 
        {
            return CSCollisionResultBack;
        }

        return CSCollisionResultIntersects;
    }

    return CSCollisionResultFront;
}

CSCollisionResult CSCollision::OBoxIntersectsSegment(const CSOBoundingBox& box, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    CSVector3 diff = seg.position1 - seg.position0;
    float d2 = diff.lengthSquared();

    if (!CSMath::nearZero(d2)) {
        float d = CSMath::sqrt(d2);
        CSRay ray(seg.position0, diff / d);
        float distance;
        bool intersects = RayIntersectsOBox(ray, box, CSCollisionFlagNear, distance, NULL);

        if (distance <= d) {
            segNear = ray.position + ray.direction * distance;
            if (intersects) {
                if ((flags & CSCollisionFlagHit) && hit) {
                    hit->position = segNear;
                    OBoxClosestNormalInternal(box, segNear, hit->direction, hit->distance);
                    hit->direction = -hit->direction;
                }
                return CSCollisionResultIntersects;
            }
        }
        else segNear = seg.position1;
    }
    else segNear = seg.position0;

    return OBoxIntersectsPoint(box, segNear, flags, hit);
}

CSCollisionResult CSCollision::OBoxIntersectsTriangle(const CSOBoundingBox& box, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 boxCorners[8];
    box.getCorners(boxCorners);
    CSVector3 triNormal = tri.normal();
    float triOffset = CSVector3::dot(triNormal, tri.position0);
    float boxMin, boxMax;
    project(triNormal, boxCorners, 8, boxMin, boxMax);
    if (boxMax < triOffset || boxMin > triOffset) return CSCollisionResultFront;

    CSVector3 inter0;
    switch (OBoxIntersects(box, tri.positions(), 3, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                TriangleClosestPoint(tri, box.center, hit->position);
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
            CSVector3 axis = CSVector3::cross(triEdges[i], box.axis[j]);
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

CSCollisionResult CSCollision::OBoxIntersectsSphere(const CSOBoundingBox& box, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 proj, boxNear;
    OBoxClosestPoint(box, sphere.center, proj, boxNear);
    CSVector3 diff = sphere.center - boxNear;
    float d2 = diff.lengthSquared();
    float r2 = sphere.radius * sphere.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;
        if (CSMath::nearZero(d2)) {
            OBoxClosestNormalInternal(box, proj, hit->direction, hit->distance);
            hit->distance += sphere.radius;
            hit->direction = -hit->direction;
        }
        else {
            float d = CSMath::sqrt(d2);
            hit->distance = sphere.radius - d;
            hit->direction = diff / d;
        }
    }

    if ((flags & CSCollisionFlagBack) &&
        -box.extent.x + sphere.radius < proj.x && proj.x < box.extent.x - sphere.radius &&
        -box.extent.y + sphere.radius < proj.y && proj.y < box.extent.y - sphere.radius &&
        -box.extent.z + sphere.radius < proj.z && proj.z < box.extent.z - sphere.radius) {
        return CSCollisionResultBack;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::OBoxIntersectsCapsule(const CSOBoundingBox& box, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    CSCollisionResult result = OBoxIntersectsSegment(box, capsule.segment(), flags | CSCollisionFlagBack, segNear, hit);

    if (result == CSCollisionResultIntersects) return CSCollisionResultIntersects;

    CSVector3 proj;
    OBoxProject(box, segNear, proj);

    if (result == CSCollisionResultBack) {
        if (-box.extent.x + capsule.radius < proj.x && proj.x < box.extent.x - capsule.radius &&
            -box.extent.y + capsule.radius < proj.y && proj.y < box.extent.y - capsule.radius &&
            -box.extent.z + capsule.radius < proj.z && proj.z < box.extent.z - capsule.radius) {
            return CSCollisionResultBack;
        }
        return CSCollisionResultIntersects;
    }

    CSVector3 boxOffset = CSVector3::clamp(proj, -box.extent, box.extent);
    CSVector3 boxNear = box.center + box.axis[0] * boxOffset.x + box.axis[1] * boxOffset.y + box.axis[1] * boxOffset.z;
    CSVector3 diff = segNear - boxNear;
    float d2 = diff.lengthSquared();
    float r2 = capsule.radius * capsule.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = boxNear;

        if (CSMath::nearZero(d2)) {
            OBoxClosestNormalInternal(box, proj, hit->direction, hit->distance);
            hit->distance += capsule.radius;
            hit->direction = -hit->direction;
        }
        else {
            float d = CSMath::sqrt(d2);
            hit->distance = capsule.radius - d;
            hit->direction = diff / d;
        }
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::OBoxIntersectsABox(const CSOBoundingBox& box0, const CSABoundingBox& box1, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 corners0[8];
    CSVector3 corners1[8];
    box0.getCorners(corners0);
    box1.getCorners(corners1);

    CSVector3 inter0;
    switch (OBoxIntersects(box0, corners1, 8, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if ((flags & CSCollisionFlagHit) && hit) {
                float distance;
                ABoxClosestPoint(box1, box0.center, hit->position);
                ABoxClosestNormalInternal(box1, hit->position, hit->direction, distance);

                float boxMin0, boxMax0, boxMin1, boxMax1;
                project(hit->direction, corners0, 8, boxMin0, boxMax0);
                project(hit->direction, corners1, 8, boxMin1, boxMax1);
                hit->distance = boxMax1 - boxMin0;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (OBoxIntersects(box1, corners0, 8, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        ABoxClosestPoint(box1, inter, hit->position);
        ABoxClosestNormalInternal(box1, hit->position, hit->direction, distance);

        float boxMin0, boxMax0, boxMin1, boxMax1;
        project(hit->direction, corners0, 8, boxMin0, boxMax0);
        project(hit->direction, corners1, 8, boxMin1, boxMax1);
        hit->distance = boxMax1 - boxMin0;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::OBoxIntersectsOBox(const CSOBoundingBox& box0, const CSOBoundingBox& box1, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 corners0[8];
    CSVector3 corners1[8];
    box0.getCorners(corners0);
    box1.getCorners(corners1);

    CSVector3 inter0;
    switch (OBoxIntersects(box0, corners1, 8, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if ((flags & CSCollisionFlagHit) && hit) {
                float distance;
                OBoxClosestPoint(box1, box0.center, hit->position);
                OBoxClosestNormal(box1, box0.center, hit->direction, distance);

                float boxMin0, boxMax0, boxMin1, boxMax1;
                project(hit->direction, corners0, 8, boxMin0, boxMax0);
                project(hit->direction, corners1, 8, boxMin1, boxMax1);
                hit->distance = boxMax1 - boxMin0;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (OBoxIntersects(box1, corners0, 8, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
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

CSCollisionResult CSCollision::OBoxIntersectsMesh(const CSOBoundingBox& box, const CSBoundingMesh& mesh, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 corners[8];
    box.getCorners(corners);

    CSVector3 inter0;
    switch (OBoxIntersects(box, mesh.vertices()->pointer(), mesh.vertices()->count(), inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if ((flags & CSCollisionFlagHit) && hit) {
                float distance;
                MeshClosestPoint(mesh, box.center, hit->position);
                MeshClosestNormal(mesh, box.center, hit->direction, distance);

                float meshMin, meshMax, boxMin, boxMax;
                project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
                project(hit->direction, corners, 8, boxMin, boxMax);
                hit->distance = boxMax - meshMin;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (MeshIntersects(mesh, corners, 8, 0, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) && hit) {
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
