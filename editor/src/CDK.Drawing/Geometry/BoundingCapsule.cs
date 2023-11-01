using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct BoundingCapsule : IEquatable<BoundingCapsule>
    {
        public Vector3 Position0;
        public Vector3 Position1;
        public float Radius;
        public Vector3 Center => (Position0 + Position1) * 0.5f;
        public Vector3 Normal => Vector3.Normalize(Position1 - Position0);
        public Segment Segment => new Segment(Position0, Position1);

        public BoundingCapsule(in Vector3 p0, in Vector3 p1, float radius)
        {
            Position0 = p0;
            Position1 = p1;
            Radius = radius;
        }

        public float LengthSquared() => Vector3.DistanceSquared(Position0, Position1);
        public float Length() => Vector3.Distance(Position0, Position1);

        public CollisionResult Intersects(in Vector3 point, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsPoint(this, point, flags, out near, out hit);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near0, out Vector3 near1, out Hit hit) => Collision.CapsuleIntersectsSegment(this, seg, flags, out near0, out near1, out hit);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsTriangle(this, tri, flags, out near, out hit);
        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Vector3 near, out Hit hit) => Collision.RayIntersectsCapsule(ray, this, flags, out distance, out near, out hit);
        public bool Intersects(in Plane plane, out Vector3 near) => Collision.PlaneIntersectsCapsule(plane, this, out near) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsSphere(this, sphere, flags, out near, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near0, out Vector3 near1, out Hit hit) => Collision.CapsuleIntersectsCapsule(this, capsule, flags, out near0, out near1, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsABox(this, box, flags, out near, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsOBox(this, box, flags, out near, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.CapsuleIntersectsMesh(this, mesh, flags, out near, out hit);

        public float GetZ(in Vector3 point) => Collision.CapsuleGetZ(this, point);

        public static void Transform(in BoundingCapsule capsule, in Quaternion rotation, out BoundingCapsule result)
        {
            result.Position0 = Vector3.Transform(capsule.Position0, rotation);
            result.Position1 = Vector3.Transform(capsule.Position1, rotation);
            result.Radius = capsule.Radius;
        }

        public static void Transform(in BoundingCapsule capsule, in Matrix4x4 transform, out BoundingCapsule result)
        {
            result.Position0 = Vector3.Transform(capsule.Position0, transform);
            result.Position1 = Vector3.Transform(capsule.Position1, transform);
            result.Radius = capsule.Radius;
            if (Matrix4x4.Decompose(transform, out var scale, out _, out _)) result.Radius *= Math.Max(Math.Max(scale.X, scale.Y), scale.Z);
        }

        public static BoundingCapsule Transform(in BoundingCapsule capsule, in Quaternion rotation)
        {
            Transform(capsule, rotation, out var result);
            return result;
        }

        public static BoundingCapsule Transform(in BoundingCapsule capsule, in Matrix4x4 transform)
        {
            Transform(capsule, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static bool operator ==(in BoundingCapsule left, in BoundingCapsule right) => left.Equals(right);
        public static bool operator !=(in BoundingCapsule left, in BoundingCapsule right) => !left.Equals(right);

        public override string ToString() => $"P0:{Position0} P1:{Position1} Range:{Radius}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position0.GetHashCode());
            hash.Combine(Position1.GetHashCode());
            hash.Combine(Radius.GetHashCode());
            return hash;
        }

        public bool Equals(BoundingCapsule other) => Position0 == other.Position0 && Position1 == other.Position1 && Radius == other.Radius;
        public override bool Equals(object obj) => obj is BoundingCapsule other && Equals(other);
    }
}
