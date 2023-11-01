using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        public Vector3 Center;
        public float Radius;

        public BoundingSphere(in Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public CollisionResult Intersects(in Vector3 point, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsPoint(this, point, flags, out hit);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.SphereIntersectsSegment(this, seg, flags, out near, out hit);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsTriangle(this, tri, flags, out hit);
        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsSphere(ray, this, flags, out distance, out hit);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsSphere(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsSphere(this, sphere, flags, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.SphereIntersectsCapsule(this, capsule, flags, out near, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsABox(this, box, flags, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsOBox(this, box, flags, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Hit hit) => Collision.SphereIntersectsMesh(this, mesh, flags, out hit);

        public float GetZ(in Vector3 point) => Collision.SphereGetZ(this, point);

        public static void Transform(in BoundingSphere sphere, in Matrix4x4 transform, out BoundingSphere result)
        {
            result.Center = Vector3.Transform(sphere.Center, transform);
            result.Radius = sphere.Radius;
            if (Matrix4x4.Decompose(transform, out var scale, out _, out _)) result.Radius *= Math.Max(Math.Max(scale.X, scale.Y), scale.Z);
        }

        public static BoundingSphere Transform(in BoundingSphere sphere, in Matrix4x4 transform)
        {
            Transform(sphere, transform, out var result);
            return result;
        }

        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static void FromPoints(Vector3[] points, int start, int count, out BoundingSphere result)
        {
            var end = start + count;

            var center = Vector3.Zero;
            for (var i = start; i < end; ++i) center += points[i];
            center /= count;

            var radius = 0f;
            for (var i = start; i < end; ++i)
            {
                var distance = Vector3.DistanceSquared(center, points[i]);

                if (distance > radius) radius = distance;
            }

            radius = (float)Math.Sqrt(radius);

            result.Center = center;
            result.Radius = radius;
        }

        public static void FromPoints(Vector3[] points, out BoundingSphere result)
        {
            FromPoints(points, 0, points.Length, out result);
        }

        public static BoundingSphere FromPoints(Vector3[] points)
        {
            FromPoints(points, out var result);
            return result;
        }

        public static void FromBox(in ABoundingBox box, out BoundingSphere result)
        {
            result.Center = box.Center;
            result.Radius = box.Extent.Length();
        }

        public static BoundingSphere FromBox(in ABoundingBox box)
        {
            FromBox(box, out var result);
            return result;
        }

        public static void FromBox(in OBoundingBox box, out BoundingSphere result)
        {
            result.Center = box.Center;
            result.Radius = box.Extent.Length();
        }

        public static BoundingSphere FromBox(in OBoundingBox box)
        {
            FromBox(box, out var result);
            return result;
        }

        public void Append(in Vector3 point)
        {
            var radius = Vector3.Distance(Center, point);

            if (radius > Radius) Radius = radius;
        }

        public static void Append(in BoundingSphere value, in Vector3 point, out BoundingSphere result)
        {
            result = value;
            result.Append(point);
        }

        public static BoundingSphere Append(in BoundingSphere value, in Vector3 point)
        {
            Append(value, point, out var result);
            return result;
        }

        public void Append(in BoundingSphere value)
        {
            var diff = Center - value.Center;

            var length = diff.Length();

            if (Radius + value.Radius >= length)
            {
                if (Radius - value.Radius >= length)
                {
                    return;
                }

                if (value.Radius - Radius >= length)
                {
                    Center = value.Center;
                    Radius = value.Radius;
                    return;
                }
            }

            var vector = diff / length;
            var min = Math.Min(-Radius, length - value.Radius);
            var max = (Math.Max(Radius, length + value.Radius) - min) * 0.5f;

            Center = Center + vector * (max + min);
            Radius = max;
        }

        public static void Append(in BoundingSphere value1, in BoundingSphere value2, out BoundingSphere result)
        {
            result = value1;
            result.Append(value2);
        }

        public static BoundingSphere Union(in BoundingSphere value1, in BoundingSphere value2)
        {
            Append(value1, value2, out var result);
            return result;
        }

        public static bool operator ==(in BoundingSphere left, in BoundingSphere right) => left.Equals(right);
        public static bool operator !=(in BoundingSphere left, in BoundingSphere right) => !left.Equals(right);

        public override string ToString() => $"Center:{Center} Radius:{Radius}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Center.GetHashCode());
            hash.Combine(Radius.GetHashCode());
            return hash;
        }

        public bool Equals(BoundingSphere other) => Center == other.Center && Radius == other.Radius;
        public override bool Equals(object obj) => obj is BoundingSphere other && Equals(other);
    }
}
