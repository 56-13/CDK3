using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct Triangle : IEquatable<Triangle>
    {
        public Vector3 Position0;
        public Vector3 Position1;
        public Vector3 Position2;

        public Vector3 Normal => Vector3.Normalize(Vector3.Cross(Position1 - Position0, Position2 - Position0));
        public Vector3 Center => (Position0 + Position1 + Position2) / 3;
        public Plane Plane => Plane.CreateFromVertices(Position0, Position1, Position2);
        public Vector3[] Positions => new Vector3[] { Position0, Position1, Position2 };

        public Triangle(in Vector3 position0, in Vector3 position1, in Vector3 position2)
        {
            Position0 = position0;
            Position1 = position1;
            Position2 = position2;
        }

        public CollisionResult Intersects(in Vector3 point) => Collision.TriangleIntersectsPoint(this, point);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near) => Collision.TriangleIntersectsSegment(this, seg, flags, out near);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags) => Collision.TriangleIntersectsTriangle(this, tri, flags);
        public bool Intersects(in Ray ray, out float distance) => Collision.RayIntersectsTriangle(ray, this, out distance);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsTriangle(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Hit hit) => Collision.TriangleIntersectsSphere(this, sphere, flags, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.TriangleIntersectsCapsule(this, capsule, flags, out near, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Hit hit) => Collision.TriangleIntersectsABox(this, box, flags, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Hit hit) => Collision.TriangleIntersectsOBox(this, box, flags, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Hit hit) => Collision.TriangleIntersectsMesh(this, mesh, flags, out hit);

        public float GetZ(in Vector3 point) => Collision.TriangleGetZ(this, point);

        public static void Transform(in Triangle tri, in Quaternion rotation, out Triangle result)
        {
            result.Position0 = Vector3.Transform(tri.Position0, rotation);
            result.Position1 = Vector3.Transform(tri.Position1, rotation);
            result.Position2 = Vector3.Transform(tri.Position2, rotation);
        }

        public static void Transform(in Triangle tri, in Matrix4x4 transform, out Triangle result)
        {
            result.Position0 = Vector3.Transform(tri.Position0, transform);
            result.Position1 = Vector3.Transform(tri.Position1, transform);
            result.Position2 = Vector3.Transform(tri.Position2, transform);
        }

        public static Triangle Transform(in Triangle tri, in Quaternion rotation)
        {
            Transform(tri, rotation, out var result);
            return result;
        }

        public static Triangle Transform(in Triangle tri, in Matrix4x4 transform)
        {
            Transform(tri, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static bool operator ==(in Triangle left, in Triangle right) => left.Equals(right);
        public static bool operator !=(in Triangle left, in Triangle right) => !left.Equals(right);

        public override string ToString() => $"Position0:{Position0} Position1:{Position1} Position2:{Position2}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position0.GetHashCode());
            hash.Combine(Position1.GetHashCode());
            hash.Combine(Position2.GetHashCode());
            return hash;
        }

        public bool Equals(Triangle other) => Position0 == other.Position0 && Position1 == other.Position1 && Position2 == other.Position2;
        public override bool Equals(object obj) => obj is Triangle other && Equals(other);
    }
}
