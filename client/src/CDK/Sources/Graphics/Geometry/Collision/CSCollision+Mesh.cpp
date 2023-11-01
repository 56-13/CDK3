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

float CSCollision::MeshGetZ(const CSBoundingMesh& mesh, const CSVector3& point) {
    CSRay ray(point, -CSVector3::UnitZ);
    float distance;
    if (!RayIntersectsMesh(ray, mesh, 0, distance, NULL)) return 0;
    return CSMath::max(point.z - distance, 0.0f);
}

void CSCollision::MeshClosestPoint(const CSBoundingMesh& mesh, const CSVector3& point, CSVector3& result) {
    result = point;
    foreach (const CSBoundingMesh::Face&, face, mesh.faces()) {
        float t = CSVector3::dot(face.normal, result);
        if (t < face.min) result += face.normal * (face.min - t);
        else if (t > face.max) result += face.normal * (face.max - t);
    }
}

void CSCollision::MeshClosestNormal(const CSBoundingMesh& mesh, const CSVector3& point, CSVector3& normal, float& distance) {
    distance = FloatMin;
    normal = CSVector3::Zero;

    foreach (const CSBoundingMesh::Face&, face, mesh.faces()) {
        float t = CSVector3::dot(face.normal, point);
        float d = CSMath::max(t - face.max, face.min - t);

        if (d > distance) {
            distance = d;
            normal = face.normal;
        }
    }
}

CSCollisionResult CSCollision::MeshIntersects(const CSBoundingMesh& mesh, const CSVector3* points, int count, float radius, CSVector3& inter) {
    inter = CSVector3::Zero;

    bool back = true;
    foreach (const CSBoundingMesh::Face&, face, mesh.faces()) {
        float min, max;
        project(face.normal, points, count, min, max);
        min -= radius;
        max += radius;
        if (max < face.min || min > face.max) return CSCollisionResultFront;
        back &= face.min < min&& max <= face.max;
        inter += face.normal * (CSMath::max(min, face.min) + CSMath::min(max, face.max));
    }

    inter /= mesh.faces()->count();

    if (back) return CSCollisionResultBack;

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::MeshIntersectsPoint(const CSBoundingMesh& mesh, const CSVector3& point, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 inter;
    CSCollisionResult result = MeshIntersects(mesh, &point, 1, 0, inter);

    if (result != CSCollisionResultFront && (flags & CSCollisionFlagHit) && hit) {
        hit->position = point;
        MeshClosestNormal(mesh, point, hit->direction, hit->distance);
        hit->direction = -hit->direction;
    }

    return result;
}

CSCollisionResult CSCollision::MeshIntersectsSegment(const CSBoundingMesh& mesh, const CSSegment& seg, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    CSVector3 diff = seg.position1 - seg.position0;
    float d2 = diff.lengthSquared();

    if (!CSMath::nearZero(d2)) {
        float d = CSMath::sqrt(d2);
        CSRay ray(seg.position0, diff / d);
        float distance;
        bool intersects = RayIntersectsMesh(ray, mesh, flags & (CSCollisionFlagNear | CSCollisionFlagHit), distance, hit);
        if (hit) hit->direction = -hit->direction;

        if (distance <= d) {
            segNear = ray.position + ray.direction * distance;
            if (intersects) return CSCollisionResultIntersects;
        }
        else segNear = seg.position1;
    }
    else segNear = seg.position0;

    return MeshIntersectsPoint(mesh, segNear, flags, hit);
}

CSCollisionResult CSCollision::MeshIntersectsTriangle(const CSBoundingMesh& mesh, const CSTriangle& tri, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 triNormal = tri.normal();
    float triOffset = CSVector3::dot(triNormal, tri.position0);
    float meshMin, meshMax;
    project(triNormal, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
    if (meshMax < triOffset || meshMin > triOffset) return CSCollisionResultFront;

    CSVector3 inter0;
    switch (MeshIntersects(mesh, tri.positions(), 3, 0, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                TriangleClosestPoint(tri, mesh.center(), hit->position);
                hit->direction = triNormal;
                hit->distance = triOffset - meshMin;
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
        foreach (const CSBoundingMesh::Face&, face, mesh.faces()) {
            CSVector3 axis = CSVector3::cross(triEdges[i], face.normal);
            float triMin, triMax, triMeshMin, triMeshMax;
            project(axis, mesh.vertices()->pointer(), mesh.vertices()->count(), triMeshMin, triMeshMax);
            project(axis, tri.positions(), 3, triMin, triMax);
            if (triMeshMax <= triMin || triMeshMin >= triMax) return CSCollisionResultFront;
            if (hitFlag) inter1 += axis * ((CSMath::max(triMeshMin, triMin) + CSMath::min(triMeshMax, triMin)) * 0.5f);
        }
    }

    if (hitFlag) {
        inter1 /= 3 * mesh.faces()->count();
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        TriangleClosestPoint(tri, inter, hit->position);
        hit->direction = triNormal;
        hit->distance = triOffset - meshMin;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::MeshIntersectsSphere(const CSBoundingMesh& mesh, const CSBoundingSphere& sphere, CSCollisionFlags flags, CSHit* hit) {
    CSVector3 inter;
    CSCollisionResult result = MeshIntersects(mesh, &sphere.center, 1, sphere.radius, inter);

    if (result != CSCollisionResultFront && (flags & CSCollisionFlagHit) != 0 && hit) {
        hit->position = inter;
        MeshClosestNormal(mesh, inter, hit->direction, hit->distance);
        hit->distance += sphere.radius;
        hit->direction = -hit->direction;
    }

    return result;
}

CSCollisionResult CSCollision::MeshIntersectsCapsule(const CSBoundingMesh& mesh, const CSBoundingCapsule& capsule, CSCollisionFlags flags, CSVector3& segNear, CSHit* hit) {
    CSCollisionResult result = MeshIntersectsSegment(mesh, capsule.segment(), flags | CSCollisionFlagBack, segNear, hit);

    if (result == CSCollisionResultIntersects) return CSCollisionResultIntersects;

    if (result == CSCollisionResultBack) {
        CSVector3 inter;
        if (MeshIntersects(mesh, &segNear, 1, capsule.radius, inter) == CSCollisionResultBack) return CSCollisionResultBack;

        return CSCollisionResultIntersects;
    }

    CSVector3 meshNear;
    MeshClosestPoint(mesh, segNear, meshNear);
    CSVector3 diff = segNear - meshNear;
    float d2 = diff.lengthSquared();
    float r2 = capsule.radius * capsule.radius;

    if (d2 > r2) return CSCollisionResultFront;

    if ((flags & CSCollisionFlagHit) != 0) {
        hit->position = meshNear;

        if (CSMath::nearZero(d2)) CSVector3::normalize(mesh.center() - meshNear, hit->direction);
        else hit->direction = diff / CSMath::sqrt(d2);
        
        float meshMin, meshMax;
        project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), segNear, meshMin, meshMax);
        hit->distance = -meshMin + capsule.radius;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::MeshIntersectsABox(const CSBoundingMesh& mesh, const CSABoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 boxCorners[8];
    box.getCorners(boxCorners);

    CSVector3 inter0;
    switch (MeshIntersects(mesh, boxCorners, 8, 0, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                float distance;
                ABoxClosestPoint(box, mesh.center(), hit->position);
                MeshClosestNormal(mesh, box.center(), hit->direction, distance);
                hit->direction = -hit->direction;

                float meshMin, meshMax, boxMin, boxMax;
                project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
                project(hit->direction, boxCorners, 8, boxMin, boxMax);
                hit->distance = boxMax - meshMin;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (ABoxIntersects(box, mesh.vertices()->pointer(), mesh.vertices()->count(), inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if (hitFlag) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        ABoxClosestPoint(box, inter, hit->position);
        MeshClosestNormal(mesh, inter, hit->direction, distance);
        hit->direction = -hit->direction;

        float meshMin, meshMax, boxMin, boxMax;
        project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
        project(hit->direction, boxCorners, 8, boxMin, boxMax);
        hit->distance = boxMax - meshMin;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::MeshIntersectsOBox(const CSBoundingMesh& mesh, const CSOBoundingBox& box, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 boxCorners[8];
    box.getCorners(boxCorners);

    CSVector3 inter0;
    switch (MeshIntersects(mesh, boxCorners, 8, 0, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                float distance;
                OBoxClosestPoint(box, box.center, hit->position);
                MeshClosestNormal(mesh, box.center, hit->direction, distance);
                hit->direction = -hit->direction;

                float meshMin, meshMax, boxMin, boxMax;
                project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
                project(hit->direction, boxCorners, 8, boxMin, boxMax);
                hit->distance = boxMax - meshMin;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (OBoxIntersects(box, mesh.vertices()->pointer(), mesh.vertices()->count(), inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if (hitFlag) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        OBoxClosestPoint(box, inter, hit->position);
        MeshClosestNormal(mesh, inter, hit->direction, distance);
        hit->direction = -hit->direction;

        float meshMin, meshMax, boxMin, boxMax;
        project(hit->direction, mesh.vertices()->pointer(), mesh.vertices()->count(), meshMin, meshMax);
        project(hit->direction, boxCorners, 8, boxMin, boxMax);
        hit->distance = boxMax - meshMin;
    }

    return CSCollisionResultIntersects;
}

CSCollisionResult CSCollision::MeshIntersectsMesh(const CSBoundingMesh& mesh0, const CSBoundingMesh& mesh1, CSCollisionFlags flags, CSHit* hit) {
    bool hitFlag = (flags & CSCollisionFlagHit) && hit;

    CSVector3 inter0;
    switch (MeshIntersects(mesh0, mesh1.vertices()->pointer(), mesh1.vertices()->count(), 0, inter0)) {
        case CSCollisionResultFront:
            return CSCollisionResultFront;
        case CSCollisionResultBack:
            if (hitFlag) {
                CSVector3 center = mesh0.center();
                float distance;
                MeshClosestPoint(mesh1, center, hit->position);
                MeshClosestNormal(mesh1, center, hit->direction, distance);

                float meshMin0, meshMax0, meshMin1, meshMax1;
                project(hit->direction, mesh0.vertices()->pointer(), mesh0.vertices()->count(), meshMin0, meshMax0);
                project(hit->direction, mesh1.vertices()->pointer(), mesh1.vertices()->count(), meshMin1, meshMax1);
                hit->distance = meshMax1 - meshMin0;
            }
            return CSCollisionResultBack;
    }

    CSVector3 inter1;
    if (MeshIntersects(mesh1, mesh0.vertices()->pointer(), mesh0.vertices()->count(), 0, inter1) == CSCollisionResultFront) return CSCollisionResultFront;

    if (hitFlag) {
        CSVector3 inter = (inter0 + inter1) * 0.5f;
        float distance;
        MeshClosestPoint(mesh1, inter, hit->position);
        MeshClosestNormal(mesh1, inter, hit->direction, distance);

        float meshMin0, meshMax0, meshMin1, meshMax1;
        project(hit->direction, mesh0.vertices()->pointer(), mesh0.vertices()->count(), meshMin0, meshMax0);
        project(hit->direction, mesh1.vertices()->pointer(), mesh1.vertices()->count(), meshMin1, meshMax1);
        hit->distance = meshMax1 - meshMin0;
    }

    return CSCollisionResultIntersects;
}
