using System;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public struct ABoundingBox : IEquatable<ABoundingBox>
    {
        public Vector3 Minimum;
        public Vector3 Maximum;
        public Vector3 Center => (Maximum + Minimum) * 0.5f;
        public Vector3 Extent => (Maximum - Minimum) * 0.5f;
        public float Radius => Math.Max(Math.Max(Maximum.X - Minimum.X, Maximum.Y - Minimum.Y), Maximum.Z - Minimum.Z) * 0.5f;

        public static readonly ABoundingBox Zero = new ABoundingBox();
        public static readonly ABoundingBox None = new ABoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
        public static readonly ABoundingBox ViewSpace = new ABoundingBox(-Vector3.One, Vector3.One);

        public ABoundingBox(in Vector3 pos)
        {
            Minimum = pos;
            Maximum = pos;
        }

        public ABoundingBox(in Vector3 minimum, in Vector3 maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public Vector3[] GetCorners() => GetCorners(Minimum, Maximum);
        public void GetCorners(Vector3[] corners) => GetCorners(Minimum, Maximum, corners);
        public static Vector3[] GetCorners(in Vector3 min, in Vector3 max)
        {
            var results = new Vector3[8];
            GetCorners(min, max, results);
            return results;
        }

        public static void GetCorners(in Vector3 min, in Vector3 max, Vector3[] corners)
        {
            corners[0] = min;
            corners[1] = new Vector3(max.X, min.Y, min.Z);
            corners[2] = new Vector3(min.X, max.Y, min.Z);
            corners[3] = new Vector3(max.X, max.Y, min.Z);
            corners[4] = new Vector3(min.X, min.Y, max.Z);
            corners[5] = new Vector3(max.X, min.Y, max.Z);
            corners[6] = new Vector3(min.X, max.Y, max.Z);
            corners[7] = max;
        }

        public CollisionResult Intersects(in Vector3 point, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsPoint(this, point, flags, out hit);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.ABoxIntersectsSegment(this, seg, flags, out near, out hit);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsTriangle(this, tri, flags, out hit);
        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsABox(ray, this, flags, out distance, out hit);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsABox(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsSphere(this, sphere, flags, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.ABoxIntersectsCapsule(this, capsule, flags, out near, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsABox(this, box, flags, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsOBox(this, box, flags, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Hit hit) => Collision.ABoxIntersectsMesh(this, mesh, flags, out hit);

        public float GetZ(in Vector3 point) => Collision.ABoxGetZ(this, point);

        public static void Transform(in ABoundingBox box, in Quaternion rotation, out ABoundingBox result)
        {
            var corners = box.GetCorners();
            result = None;
            foreach (var p in corners) result.Append(Vector3.Transform(p, rotation));
        }

        public static void Transform(in ABoundingBox box, in Matrix4x4 transform, out ABoundingBox result)
        {
            var corners = box.GetCorners();
            result = None;
            foreach (var p in corners) result.Append(Vector3.Transform(p, transform));
        }

        public static ABoundingBox Transform(in ABoundingBox box, in Quaternion rotation)
        {
            Transform(box, rotation, out var result);
            return result;
        }

        public static ABoundingBox Transform(in ABoundingBox box, in Matrix4x4 transform)
        {
            Transform(box, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static void FromPoints(IEnumerable<Vector3> points, out ABoundingBox result)
        {
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            foreach (var p in points)
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }

            result = new ABoundingBox(min, max);
        }

        public static ABoundingBox FromPoints(IEnumerable<Vector3> points)
        {
            FromPoints(points, out var result);
            return result;
        }

        public static void FromSphere(in BoundingSphere sphere, out ABoundingBox result)
        {
            var radius = new Vector3(sphere.Radius);
            result.Minimum = sphere.Center - radius;
            result.Maximum = sphere.Center + radius;
        }

        public static ABoundingBox FromSphere(in BoundingSphere sphere)
        {
            var radius = new Vector3(sphere.Radius);

            return new ABoundingBox(sphere.Center - radius, sphere.Center + radius);
        }

        public static void FromCapsule(in BoundingCapsule capsule, out ABoundingBox result)
        {
            result.Minimum = Vector3.Min(capsule.Position0, capsule.Position1);
            result.Maximum = Vector3.Max(capsule.Position0, capsule.Position1);
            var radius = new Vector3(capsule.Radius);
            result.Minimum -= radius;
            result.Maximum += radius;
        }

        public static ABoundingBox FromCapsule(in BoundingCapsule capsule)
        {
            FromCapsule(capsule, out var result);
            return result;
        }

        public void Append(in Vector3 point)
        {
            Minimum = Vector3.Min(Minimum, point);
            Maximum = Vector3.Max(Maximum, point);
        }

        public void Append(in Vector3 point, in Matrix4x4 worldViewProjection)
        {
            var vp = Vector4.Transform(point, worldViewProjection);
            if (vp.W != 0)
            {
                Append(vp.ToVector3() / Math.Abs(vp.W));
            }
            else
            {
                var cvp = vp.ToVector3();
                if (cvp.X < 0) cvp.X = -2;
                else if (cvp.X > 0) cvp.X = 2;
                if (cvp.Y < 0) cvp.Y = -2;
                else if (cvp.Y > 0) cvp.Y = 2;
                if (cvp.Z < 0) cvp.Z = -2;
                else if (cvp.Z > 0) cvp.Z = 2;
                Append(cvp);
            }
        }

        public void Append(in ABoundingBox box)
        {
            Minimum = Vector3.Min(Minimum, box.Minimum);
            Maximum = Vector3.Max(Maximum, box.Maximum);
        }

        public static void Append(in ABoundingBox value1, in ABoundingBox value2, out ABoundingBox result)
        {
            result.Minimum = Vector3.Min(value1.Minimum, value2.Minimum);
            result.Maximum = Vector3.Max(value1.Maximum, value2.Maximum);
        }

        public static ABoundingBox Append(in ABoundingBox value1, in ABoundingBox value2)
        {
            Append(value1, value2, out var result);
            return result;
        }

        public static implicit operator OBoundingBox(in ABoundingBox value) => new OBoundingBox(value.Center, value.Extent, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

        public static bool operator ==(in ABoundingBox left, in ABoundingBox right) => left.Equals(right);
        public static bool operator !=(in ABoundingBox left, in ABoundingBox right) => !left.Equals(right);

        public override string ToString() => $"Minimum:{Minimum} Maximum:{Maximum}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Minimum.GetHashCode());
            hash.Combine(Maximum.GetHashCode());
            return hash;
        }

        public bool Equals(ABoundingBox other) => Minimum == other.Minimum && Maximum == other.Maximum;
        public override bool Equals(object obj) => obj is ABoundingBox other && Equals(other);
    }
}
