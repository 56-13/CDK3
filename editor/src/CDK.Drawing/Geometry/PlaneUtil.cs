using System.Numerics;

namespace CDK.Drawing
{
    public static class PlaneUtil
    {
        public static CollisionResult Intersects(this Plane plane, in Vector3 point) => Collision.PlaneIntersectsPoint(plane, point);
        public static CollisionResult Intersects(this Plane plane, in Segment seg, out Vector3 near) => Collision.PlaneIntersectsSegment(plane, seg, out near);
        public static CollisionResult Intersects(this Plane plane, in Triangle tri) => Collision.PlaneIntersectsTriangle(plane, tri);
        public static bool Intersects(this Plane plane, in Ray ray, out float distance) => Collision.RayIntersectsPlane(ray, plane, out distance);
        public static bool Intersects(this Plane plane, in Plane other) => Collision.PlaneIntersectsPlane(plane, other);
        public static bool Intersects(this Plane plane, in Plane other, out Ray line) => Collision.PlaneIntersectsPlane(plane, other, out line);
        public static CollisionResult Intersects(this Plane plane, in BoundingSphere sphere) => Collision.PlaneIntersectsSphere(plane, sphere);
        public static CollisionResult Intersects(this Plane plane, in BoundingCapsule capsule, out Vector3 near) => Collision.PlaneIntersectsCapsule(plane, capsule, out near);
        public static CollisionResult Intersects(this Plane plane, in ABoundingBox box) => Collision.PlaneIntersectsABox(plane, box);
        public static CollisionResult Intersects(this Plane plane, in OBoundingBox box) => Collision.PlaneIntersectsOBox(plane, box);
        public static CollisionResult Intersects(this Plane plane, BoundingMesh mesh) => Collision.PlaneIntersectsMesh(plane, mesh);
        public static CollisionResult Intersects(this Plane plane, in BoundingFrustum frustum) => Collision.PlaneIntersectsFrustum(plane, frustum);
        public static float GetZ(this Plane plane, in Vector3 point) => Collision.PlaneGetZ(plane, point);
    }
}
