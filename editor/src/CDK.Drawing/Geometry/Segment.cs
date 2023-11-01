using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct Segment : IEquatable<Segment>
    {
        public Vector3 Position0;
        public Vector3 Position1;

        public Vector3 Center => (Position0 + Position1) * 0.5f;

        public Segment(in Vector3 position0, in Vector3 position1)
        {
            Position0 = position0;
            Position1 = position1;
        }
        
        public float LengthSquared() => Vector3.DistanceSquared(Position0, Position1);
        public float Length() => Vector3.Distance(Position0, Position1);

        public bool Intersects(in Vector3 point, out Vector3 near)
        {
            Collision.SegmentToPoint(this, point, out near);
            return VectorUtil.NearEqual(point, near);
        }

        public bool Intersects(in Segment seg, out Vector3 near0, out Vector3 near1)
        {
            Collision.SegmentToSegment(this, seg, out near0, out near1);
            return VectorUtil.NearEqual(near0, near1);
        }

        public bool Intersects(in Triangle tri, out Vector3 near) => Collision.TriangleIntersectsSegment(tri, this, CollisionFlags.None, out near) == CollisionResult.Intersects;
        public bool Intersects(in Ray ray, out float distance, out Vector3 near) => Collision.RayIntersectsSegment(ray, this, out distance, out near);
        public bool Intersects(in Plane plane, out Vector3 near) => Collision.PlaneIntersectsSegment(plane, this, out near) == CollisionResult.Intersects;
        public bool Intersects(in BoundingSphere sphere, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            return Collision.SphereIntersectsSegment(sphere, this, flags, out near, out hit) != CollisionResult.Front;
        }
        public bool Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near0, out Vector3 near1, out Hit hit)
        {
            return Collision.CapsuleIntersectsSegment(capsule, this, flags, out near1, out near0, out hit) != CollisionResult.Front;
        }
        public bool Intersects(in ABoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            return Collision.ABoxIntersectsSegment(box, this, flags, out near, out hit) != CollisionResult.Front;
        }
        public bool Intersects(in OBoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            return Collision.OBoxIntersectsSegment(box, this, flags, out near, out hit) != CollisionResult.Front;
        }
        public bool Intersects(BoundingMesh mesh, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            return Collision.MeshIntersectsSegment(mesh, this, flags, out near, out hit) != CollisionResult.Front;
        }

        public static void Transform(in Segment seg, in Quaternion rotation, out Segment result)
        {
            result.Position0 = Vector3.Transform(seg.Position0, rotation);
            result.Position1 = Vector3.Transform(seg.Position1, rotation);
        }

        public static void Transform(in Segment seg, in Matrix4x4 transform, out Segment result)
        {
            result.Position0 = Vector3.Transform(seg.Position0, transform);
            result.Position1 = Vector3.Transform(seg.Position1, transform);
        }

        public static Segment Transform(in Segment seg, in Quaternion rotation)
        {
            Transform(seg, rotation, out var result);
            return result;
        }

        public static Segment Transform(in Segment seg, in Matrix4x4 transform)
        {
            Transform(seg, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public static bool operator ==(in Segment left, in Segment right) => left.Equals(right);
        public static bool operator !=(in Segment left, in Segment right) => !left.Equals(right);

        public override string ToString() => $"Position0:{Position0} Position1:{Position1}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Position0.GetHashCode());
            hash.Combine(Position1.GetHashCode());
            return hash;
        }

        public bool Equals(Segment other) => Position0 == other.Position0 && Position1 == other.Position1;
        public override bool Equals(object obj) => obj is Segment other && Equals(other);
    }
}
