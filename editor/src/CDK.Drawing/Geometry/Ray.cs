using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct Ray : IEquatable<Ray>
    {
        public Vector3 Position;
        public Vector3 Direction;

        public static readonly Ray Empty = new Ray();

        public Ray(in Vector3 position, in Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        public float Distance(in Vector3 point)
        {
            return Vector3.Cross(Direction, point - Position).Length();
        }

        public bool Intersects(in Vector3 point, out float distance) => Collision.RayIntersectsPoint(this, point, out distance);
        public bool Intersects(in Segment seg, out float distance, out Vector3 near) => Collision.RayIntersectsSegment(this, seg, out distance, out near);
        public bool Intersects(in Triangle tri, out float distance) => Collision.RayIntersectsTriangle(this, tri, out distance);
        public bool Intersects(in Ray ray, out float distance0, out float distance1) => Collision.RayIntersectsRay(this, ray, out distance0, out distance1);
        public bool Intersects(in Plane plane, out float distance) => Collision.RayIntersectsPlane(this, plane, out distance);
        public bool Intersects(in BoundingSphere sphere, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsSphere(this, sphere, flags, out distance, out hit);
        public bool Intersects(in BoundingCapsule capsule, CollisionFlags flags, out float distance, out Vector3 near, out Hit hit) => Collision.RayIntersectsCapsule(this, capsule, flags, out distance, out near, out hit);
        public bool Intersects(in ABoundingBox box, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsABox(this, box, flags, out distance, out hit);
        public bool Intersects(in OBoundingBox box, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsOBox(this, box, flags, out distance, out hit);
        public bool Intersects(BoundingMesh mesh, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsMesh(this, mesh, flags, out distance, out hit);
        public bool Intersects(in BoundingFrustum frustum, out float distance0, out float distance1) => Collision.RayIntersectsFrustum(this, frustum, out distance0, out distance1);

        public static void Transform(in Ray ray, in Quaternion rotation, out Ray result)
        {
            result.Position = ray.Position;
            result.Direction = Vector3.Transform(ray.Direction, rotation);
        }

        public static void Transform(in Ray ray, in Matrix4x4 transform, out Ray result)
        {
            result.Position = Vector3.Transform(ray.Position, transform);
            result.Direction = Vector3.Normalize(Vector3.TransformNormal(ray.Direction, transform));
        }

        public static Ray Transform(in Ray ray, in Quaternion rotation)
        {
            Transform(ray, rotation, out var result);
            return result;
        }

        public static Ray Transform(in Ray ray, in Matrix4x4 transform)
        {
            Transform(ray, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static bool operator ==(in Ray left, in Ray right) => left.Equals(right);
        public static bool operator !=(in Ray left, in Ray right) => !left.Equals(right);

        public override string ToString()
        {
            return $"Position:{Position} Direction:{Direction}";
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position.GetHashCode());
            hash.Combine(Direction.GetHashCode());
            return hash;
        }

        public bool Equals(Ray other) => Position == other.Position && Direction == other.Direction;
        public override bool Equals(object obj) => obj is Ray other && Equals(other);
    }
}
