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

bool CSCollision::RayIntersectsPoint(const CSRay& ray, const CSVector3& point, float& distance) {
    CSVector3 m = ray.position - point;
    float b = CSVector3::dot(m, ray.direction);
    float c = CSVector3::dot(m, m) - CSMath::ZeroTolerance;

    distance = CSMath::max(-b, 0.0f);

    if (c > 0 && b > 0) return false;

    float discriminant = b * b - c;

    if (discriminant < 0) return false;

    return true;
}

bool CSCollision::RayIntersectsSegment(const CSRay& ray, const CSSegment& seg, float& distance, CSVector3& segNear) {
    CSVector3 diff = seg.position1 - seg.position0;
    float length = diff.length();

    if (!CSMath::nearZero(length)) {
        CSRay ray1(seg.position0, diff / length);
        float distance1;
        bool intersects = RayIntersectsRay(ray, ray1, distance, distance1);
        if (distance1 <= length) {
            segNear = ray1.position + ray1.direction * distance1;
            if (intersects) return true;
        }
        else segNear = seg.position1;
        return false;
    }

    segNear = seg.position0;
    return RayIntersectsPoint(ray, seg.position0, distance);
}

bool CSCollision::RayIntersectsTriangle(const CSRay& ray, const CSTriangle& tri, float& distance) {
    CSVector3 v0v1 = tri.position1 - tri.position0;
    CSVector3 v0v2 = tri.position2 - tri.position0;
    CSVector3 normal = CSVector3::cross(v0v1, v0v2);

    float ndotd = CSVector3::dot(normal, ray.direction);
    if (CSMath::nearZero(ndotd)) {
        distance = CSMath::max(CSVector3::dot(ray.direction, tri.center()), 0.0f);
        return false;
    }

    float d = -CSVector3::dot(normal, tri.position0);

    distance = -(CSVector3::dot(normal, ray.position) + d) / ndotd;

    if (distance < 0) {
        distance = 0;
        return false;
    }

    CSVector3 p = ray.position + ray.direction * distance;

    CSVector3 edge0 = tri.position1 - tri.position0;
    CSVector3 vp0 = p - tri.position0;
    CSVector3 c = CSVector3::cross(edge0, vp0);
    if (CSVector3::dot(normal, c) < 0) return false;

    CSVector3 edge1 = tri.position2 - tri.position1;
    CSVector3 vp1 = p - tri.position1;
    c = CSVector3::cross(edge1, vp1);
    if (CSVector3::dot(normal, c) < 0) return false;

    CSVector3 edge2 = tri.position0 - tri.position2;
    CSVector3 vp2 = p - tri.position2;
    c = CSVector3::cross(edge2, vp2);
    if (CSVector3::dot(normal, c) < 0) return false;

    return true;
}

bool CSCollision::RayIntersectsRay(const CSRay& ray0, const CSRay& ray1, float& distance0, float& distance1) {
    CSVector3 cross = CSVector3::cross(ray0.direction, ray1.direction);
    float denominator = cross.length();

    if (CSMath::nearZero(denominator)) {
        if (ray0.position.nearEqual(ray1.position)) {
            distance0 = distance1 = 0;
            return true;
        }
    }

    denominator *= denominator;

    float m11 = ray1.position.x - ray0.position.x;
    float m12 = ray1.position.y - ray0.position.y;
    float m13 = ray1.position.z - ray0.position.z;
    float m21 = ray1.direction.x;
    float m22 = ray1.direction.y;
    float m23 = ray1.direction.z;
    float m31 = cross.x;
    float m32 = cross.y;
    float m33 = cross.z;

    float dets =
        m11 * m22 * m33 +
        m12 * m23 * m31 +
        m13 * m21 * m32 -
        m11 * m23 * m32 -
        m12 * m21 * m33 -
        m13 * m22 * m31;

    m21 = ray0.direction.x;
    m22 = ray0.direction.y;
    m23 = ray0.direction.z;

    float dett =
        m11 * m22 * m33 +
        m12 * m23 * m31 +
        m13 * m21 * m32 -
        m11 * m23 * m32 -
        m12 * m21 * m33 -
        m13 * m22 * m31;

    distance0 = dets / denominator;
    distance1 = dett / denominator;

    CSVector3 near0 = ray0.position + ray0.direction * distance0;
    CSVector3 near1 = ray1.position + ray1.direction * distance1;

    return near0.nearEqual(near1);
}

bool CSCollision::RayIntersectsPlane(const CSRay& ray, const CSPlane& plane, float& distance) {
    float direction = CSVector3::dot(plane.normal, ray.direction);

    if (CSMath::nearZero(direction)) {
        distance = 0;
        return false;
    }

    float position = CSVector3::dot(plane.normal, ray.position);

    distance = (-plane.d - position) / direction;

    if (distance < 0) {
        distance = 0;
        return false;
    }
    return true;
}

bool CSCollision::RayIntersectsSphere(const CSRay& ray, const CSBoundingSphere& sphere, CSCollisionFlags flags, float& distance, CSHit* hit) {
    CSVector3 m = ray.position - sphere.center;
    float b = CSVector3::dot(m, ray.direction);
    float c = CSVector3::dot(m, m) - (sphere.radius * sphere.radius);

    distance = CSMath::max(-b, 0.0f);

    if (c > 0 && b > 0) return false;

    float discriminant = b * b - c;

    if (discriminant < 0) return false;

    distance = CSMath::max(distance - CSMath::sqrt(discriminant), 0.0f);

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = ray.position + ray.direction * distance;
        hit->direction = CSVector3::normalize(ray.position - sphere.center);
    }

    return true;
}

bool CSCollision::RayIntersectsCapsule(const CSRay& ray, const CSBoundingCapsule& capsule, CSCollisionFlags flags, float& distance, CSVector3& segNear, CSHit* hit) {
    if (RayIntersectsSegment(ray, capsule.segment(), distance, segNear)) {
        distance = CSMath::max(distance - capsule.radius, 0.0f);

        if ((flags & CSCollisionFlagHit) && hit) {
            hit->position = segNear;
        }

        return true;
    }

    CSVector3 rayNear = ray.position + ray.direction * distance;
    float r2 = capsule.radius * capsule.radius;
    float d2 = CSVector3::distanceSquared(rayNear, segNear);

    if (d2 > r2) return false;

    if ((flags & CSCollisionFlagHit) && hit) {
        hit->position = ray.position + ray.direction * distance;
        hit->direction = CSVector3::normalize(ray.position - segNear);
    }

    distance -= CSMath::sqrt(r2 - d2);
    if (distance < 0) distance = 0;

    return true;
}

static bool Clip(float denom, float numer, float& t0, float& t1) {
    if (denom > 0) {
        if (numer > denom * t1) return false;
        if (numer > denom * t0) t0 = numer / denom;
        return true;
    }
    else if (denom < 0) {
        if (numer > denom * t0) return false;
        if (numer > denom * t1) t1 = numer / denom;
        return true;
    }
    else {
        return numer <= 0;
    }
}

bool CSCollision::RayIntersectsABox(const CSRay& ray, const CSABoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit) {
    CSVector3 origin = ray.position - box.center();
    CSVector3 extent = box.extent();

    float t0 = FloatMin;
    float t1 = FloatMax;

    if (Clip(ray.direction.x, -origin.x - extent.x, t0, t1) &&
        Clip(-ray.direction.x, origin.x - extent.x, t0, t1) &&
        Clip(ray.direction.y, -origin.y - extent.y, t0, t1) &&
        Clip(-ray.direction.y, origin.y - extent.y, t0, t1) &&
        Clip(ray.direction.z, -origin.z - extent.z, t0, t1) &&
        Clip(-ray.direction.z, origin.z - extent.z, t0, t1)) {
        distance = t0;

        if ((flags & CSCollisionFlagHit) && hit) {
            hit->position = ray.position + ray.direction * distance;
            float boxDistance;
            ABoxClosestNormalInternal(box, hit->position, hit->direction, boxDistance);
        }
        return true;
    }

    distance = 0;

    if (flags & CSCollisionFlagNear) {
        CSVector3 segDistance = box.maximum - box.minimum;

        struct Edge {
            CSRay ray;
            float distance;

            Edge(const CSRay& ray, float distance) : ray(ray), distance(distance) {}
        };

        Edge edges[] = {
            Edge(CSRay(box.minimum, CSVector3::UnitX), segDistance.x),
            Edge(CSRay(CSVector3(box.minimum.x, box.maximum.y, box.minimum.z), CSVector3::UnitX), segDistance.x),
            Edge(CSRay(CSVector3(box.minimum.x, box.minimum.y, box.maximum.z), CSVector3::UnitX), segDistance.x),
            Edge(CSRay(CSVector3(box.minimum.x, box.maximum.y, box.maximum.z), CSVector3::UnitX), segDistance.x),

            Edge(CSRay(box.minimum, CSVector3::UnitY), segDistance.y),
            Edge(CSRay(CSVector3(box.maximum.x, box.minimum.y, box.minimum.z), CSVector3::UnitY), segDistance.y),
            Edge(CSRay(CSVector3(box.minimum.x, box.minimum.y, box.maximum.z), CSVector3::UnitY), segDistance.y),
            Edge(CSRay(CSVector3(box.maximum.x, box.minimum.y, box.maximum.z), CSVector3::UnitY), segDistance.y),

            Edge(CSRay(box.minimum, CSVector3::UnitZ), segDistance.z),
            Edge(CSRay(CSVector3(box.maximum.x, box.minimum.y, box.minimum.z), CSVector3::UnitZ), segDistance.z),
            Edge(CSRay(CSVector3(box.minimum.x, box.maximum.y, box.minimum.z), CSVector3::UnitZ), segDistance.z),
            Edge(CSRay(CSVector3(box.maximum.x, box.maximum.y, box.minimum.z), CSVector3::UnitZ), segDistance.z),
        };

        float diff2 = FloatMax;

        for (int i = 0; i < 12; i++) {
            const Edge& e = edges[i];
            float d0, d1;
            bool flag = RayIntersectsRay(ray, e.ray, d0, d1);
            CSVector3 near0 = ray.position + ray.direction * d0;
            CSVector3 near1 = e.ray.position + e.ray.direction * CSMath::min(d1, e.distance);
            float d2 = CSVector3::distanceSquared(near0, near1);
            if (d2 < diff2) {
                diff2 = d2;
                distance = d0;
            }
        }
    }

    return false;
}

bool CSCollision::RayIntersectsOBox(const CSRay& ray, const CSOBoundingBox& box, CSCollisionFlags flags, float& distance, CSHit* hit) {
    CSVector3 diff = ray.position - box.center;
    CSVector3 origin(
        CSVector3::dot(diff, box.axis[0]),
        CSVector3::dot(diff, box.axis[1]),
        CSVector3::dot(diff, box.axis[2])
    );
    CSVector3 direction(
        CSVector3::dot(ray.direction, box.axis[0]),
        CSVector3::dot(ray.direction, box.axis[1]),
        CSVector3::dot(ray.direction, box.axis[2])
    );

    float t0 = FloatMin;
    float t1 = FloatMax;

    if (Clip(direction.x, -origin.x - box.extent.x, t0, t1) &&
        Clip(-direction.x, origin.x - box.extent.x, t0, t1) &&
        Clip(direction.y, -origin.y - box.extent.y, t0, t1) &&
        Clip(-direction.y, origin.y - box.extent.y, t0, t1) &&
        Clip(direction.z, -origin.z - box.extent.z, t0, t1) &&
        Clip(-direction.z, origin.z - box.extent.z, t0, t1)) {
        CSVector3 cp = origin + direction * t0;
        distance = CSVector3::distance(ray.position, cp);

        if ((flags & CSCollisionFlagHit) && hit) {
            hit->position = cp;
            CSVector3 proj;
            float boxDistance;
            OBoxProject(box, cp, proj);
            OBoxClosestNormalInternal(box, proj, hit->direction, boxDistance);
        }

        return true;
    }

    distance = 0;

    if (flags & CSCollisionFlagNear) {
        CSVector3 segDistance = box.extent * 2;

        CSVector3 dx = box.axis[0] * box.extent.x;
        CSVector3 dy = box.axis[1] * box.extent.y;
        CSVector3 dz = box.axis[2] * box.extent.z;

        struct Edge {
            CSRay ray;
            float distance;

            Edge(const CSRay& ray, float distance) : ray(ray), distance(distance) {}
        };

        Edge edges[] = {
            Edge(CSRay(box.center - dx - dy - dz, box.axis[0]), segDistance.x),
            Edge(CSRay(box.center - dx + dy - dz, box.axis[0]), segDistance.x),
            Edge(CSRay(box.center - dx - dy + dz, box.axis[0]), segDistance.x),
            Edge(CSRay(box.center - dx + dy + dz, box.axis[0]), segDistance.x),

            Edge(CSRay(box.center - dx - dy - dz, box.axis[1]), segDistance.y),
            Edge(CSRay(box.center + dx - dy - dz, box.axis[1]), segDistance.y),
            Edge(CSRay(box.center - dx - dy + dz, box.axis[1]), segDistance.y),
            Edge(CSRay(box.center + dx - dy + dz, box.axis[1]), segDistance.y),

            Edge(CSRay(box.center - dx - dy - dz, box.axis[2]), segDistance.z),
            Edge(CSRay(box.center + dx - dy - dz, box.axis[2]), segDistance.z),
            Edge(CSRay(box.center - dx + dy - dz, box.axis[2]), segDistance.z),
            Edge(CSRay(box.center + dx + dy - dz, box.axis[2]), segDistance.z),
        };

        float diff2 = FloatMax;

        for (int i = 0; i < 12; i++) {
            const Edge& e = edges[i];
            float d0, d1;
            bool flag = RayIntersectsRay(ray, e.ray, d0, d1);
            CSVector3 near0 = ray.position + ray.direction * d0;
            CSVector3 near1 = e.ray.position + e.ray.direction * CSMath::min(d1, e.distance);
            float d2 = CSVector3::distanceSquared(near0, near1);
            if (d2 < diff2) {
                diff2 = d2;
                distance = d0;
            }
        }
    }

    return false;
}

static bool RayIntersectsMeshFace(const CSRay& ray, const CSBoundingMesh& mesh, const CSBoundingMesh::Face& face, float& distance) {            //TODO:check back face
    const CSVector3& origin = mesh.vertices()->objectAtIndex(face.indices[0]);

    distance = CSVector3::dot(origin - ray.position, face.normal);

    float denom = CSVector3::dot(ray.direction - origin, face.normal);
    if (CSMath::nearZero(denom)) return false;

    CSVector3 rayNear = ray.position + ray.direction * distance;

    CSVector3 v0 = rayNear - origin;
    CSVector3 v1 = CSVector3::cross(face.normal, v0);

    bool inside = false;

    CSVector3 p = mesh.vertices()->objectAtIndex(face.indices[1]) - rayNear;
    bool pp0 = CSVector3::dot(v0, p) > 0;
    bool pp1 = CSVector3::dot(v1, p) > 0;

    bool p1 = false;

    int index = 2;

    while (index < face.indexCount) {
        p = mesh.vertices()->objectAtIndex(face.indices[index]) - rayNear;

        float p0 = CSVector3::dot(v0, p) > 0;

        if (pp0 || p0) {
            p1 = CSVector3::dot(v1, p) > 0;

            if (pp1 ^ p1) inside = !inside;
        }
        pp0 = p0;
        pp1 = p1;
        index++;
    }

    return inside;
}

bool CSCollision::RayIntersectsMesh(const CSRay& ray, const CSBoundingMesh& mesh, CSCollisionFlags flags, float& distance, CSHit* hit) {
    distance = FloatMax;

    foreach (const CSBoundingMesh::Face&, face, mesh.faces()) {
        float d;
        if (RayIntersectsMeshFace(ray, mesh, face, d) && d < distance) {
            distance = d;
            hit->direction = face.normal;
        }
    }

    if (distance != FloatMax) {
        hit->position = ray.position + ray.direction * distance;
        return true;
    }
    distance = 0;
    return false;
}

bool CSCollision::RayIntersectsFrustum(const CSRay& ray, const CSBoundingFrustum& frustum, float& distance0, float& distance1) {
    if (FrustumIntersectsPoint(frustum, ray.position) != CSCollisionResultFront) {
        float nearstPlaneDistance = FloatMax;
        for (int i = 0; i < 6; i++) {
            const CSPlane& plane = frustum.planes[i];
            float distance;
            if (RayIntersectsPlane(ray, plane, distance) && distance < nearstPlaneDistance) {
                nearstPlaneDistance = distance;
            }
        }

        distance0 = distance1 = nearstPlaneDistance;
        return true;
    }
    else {
        float minDist = FloatMax;
        float maxDist = FloatMin;
        for (int i = 0; i < 6; i++) {
            const CSPlane& plane = frustum.planes[i];
            float distance;
            if (RayIntersectsPlane(ray, plane, distance)) {
                minDist = CSMath::min(minDist, distance);
                maxDist = CSMath::max(maxDist, distance);
            }
        }

        CSVector3 minPoint = ray.position + ray.direction * minDist;
        CSVector3 maxPoint = ray.position + ray.direction * maxDist;
        CSVector3 center = (minPoint + maxPoint) * 0.5f;
        if (FrustumIntersectsPoint(frustum, center) != CSCollisionResultFront) {
            distance0 = minDist;
            distance1 = maxDist;
            return true;
        }
        return false;
    }
}
