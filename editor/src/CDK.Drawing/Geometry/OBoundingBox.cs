using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct OBoundingBox : IEquatable<OBoundingBox>
    {
        public Vector3 Center;
        public Vector3 Extent;
        public Vector3 AxisX;
        public Vector3 AxisY;
        public Vector3 AxisZ;

        public float Radius => Math.Max(Math.Max(Extent.X, Extent.Y), Extent.Z);

        public OBoundingBox(in Vector3 center, in Vector3 extent, in Vector3 axisx, in Vector3 axisy, in Vector3 axisz)
        {
            Center = center;
            Extent = extent;
            AxisX = axisx;
            AxisY = axisy;
            AxisZ = axisz;
        }

        public void GetAxis(int i, out Vector3 axis)
        {
            switch (i)
            {
                case 0: axis = AxisX; return;
                case 1: axis = AxisY; return;
                case 2: axis = AxisZ; return;
            }
            throw new ArgumentOutOfRangeException();
        }

        public Vector3 GetAxis(int i)
        {
            GetAxis(i, out var result);
            return result;
        }

        public void GetCorners(Vector3[] corners)
        {
            var dx = Extent.X * AxisX + Extent.Y * AxisX * Extent.Z * AxisX;
            var dy = Extent.X * AxisY + Extent.Y * AxisY * Extent.Z * AxisY;
            var dz = Extent.X * AxisZ + Extent.Y * AxisZ * Extent.Z * AxisZ;
            corners[0] = Center - dx - dy - dz;
            corners[1] = Center + dx - dy - dz;
            corners[2] = Center - dx + dy - dz;
            corners[3] = Center + dx + dy - dz;
            corners[4] = Center - dx - dy + dz;
            corners[5] = Center + dx - dy + dz;
            corners[6] = Center - dx + dy + dz;
            corners[7] = Center + dx + dy + dz;
        }

        public Vector3[] GetCorners()
        {
            var results = new Vector3[8];
            GetCorners(results);
            return results;
        }

        public CollisionResult Intersects(in Vector3 point, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsPoint(this, point, flags, out hit);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.OBoxIntersectsSegment(this, seg, flags, out near, out hit);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsTriangle(this, tri, flags, out hit);
        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsOBox(ray, this, flags, out distance, out hit);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsOBox(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsSphere(this, sphere, flags, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.OBoxIntersectsCapsule(this, capsule, flags, out near, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsABox(this, box, flags, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsOBox(this, box, flags, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Hit hit) => Collision.OBoxIntersectsMesh(this, mesh, flags, out hit);

        public float GetZ(in Vector3 point) => Collision.OBoxGetZ(this, point);

        public static void Transform(in OBoundingBox box, in Quaternion rotation, out OBoundingBox result)
        {
            var axisX = Vector3.Transform(box.AxisX, rotation);
            var axisY = Vector3.Transform(box.AxisY, rotation);
            var axisZ = Vector3.Transform(box.AxisZ, rotation);
            result = new OBoundingBox(box.Center, box.Extent, axisX, axisY, axisZ);
        }

        public static void Transform(in OBoundingBox box, in Matrix4x4 transform, out OBoundingBox result)
        {
            if (!Matrix4x4.Decompose(transform, out var scale, out var rotation, out var translation))
            {
                result = box;
                return;
            }

            result.Center = box.Center + translation;
            result.Extent = box.Extent * scale;
            result.AxisX = Vector3.Transform(box.AxisX, rotation);
            result.AxisY = Vector3.Transform(box.AxisY, rotation);
            result.AxisZ = Vector3.Transform(box.AxisZ, rotation);
        }

        public static OBoundingBox Transform(in OBoundingBox box, in Quaternion rotation)
        {
            Transform(box, rotation, out var result);
            return result;
        }

        public static OBoundingBox Transform(in OBoundingBox box, in Matrix4x4 transform)
        {
            Transform(box, transform, out var result);
            return result;
        }

        public void Transform(in Quaternion rotation) => Transform(this, rotation, out this);
        public void Transform(in Matrix4x4 transform) => Transform(this, transform, out this);

        public bool IsAligned
        {
            get
            {
                return
                    (VectorUtil.NearEqual(AxisX, Vector3.UnitX) || VectorUtil.NearEqual(AxisX, Vector3.UnitY) || VectorUtil.NearEqual(AxisX, Vector3.UnitZ) ||
                    VectorUtil.NearEqual(AxisX, -Vector3.UnitX) || VectorUtil.NearEqual(AxisX, -Vector3.UnitY) || VectorUtil.NearEqual(AxisX, -Vector3.UnitZ)) &&
                    (VectorUtil.NearEqual(AxisY, Vector3.UnitX) || VectorUtil.NearEqual(AxisY, Vector3.UnitY) || VectorUtil.NearEqual(AxisY, Vector3.UnitZ) ||
                    VectorUtil.NearEqual(AxisY, -Vector3.UnitX) || VectorUtil.NearEqual(AxisY, -Vector3.UnitY) || VectorUtil.NearEqual(AxisY, -Vector3.UnitZ)) &&
                    (VectorUtil.NearEqual(AxisZ, Vector3.UnitX) || VectorUtil.NearEqual(AxisZ, Vector3.UnitY) || VectorUtil.NearEqual(AxisZ, Vector3.UnitZ) ||
                    VectorUtil.NearEqual(AxisZ, -Vector3.UnitX) || VectorUtil.NearEqual(AxisZ, -Vector3.UnitY) || VectorUtil.NearEqual(AxisZ, -Vector3.UnitZ));
            }
        }

        public void ToAligned(out ABoundingBox box) => ABoundingBox.FromPoints(GetCorners(), out box);
        public ABoundingBox ToAligned() => ABoundingBox.FromPoints(GetCorners());

        public static explicit operator ABoundingBox(in OBoundingBox value) => value.ToAligned();
        public static bool operator ==(in OBoundingBox left, in OBoundingBox right) => left.Equals(right);
        public static bool operator !=(in OBoundingBox left, in OBoundingBox right) => !left.Equals(right);

        public override string ToString() => $"Center:{Center} Extent:{Extent} AxisX:{AxisX} AxisY:{AxisY} AxisZ:{AxisZ}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Center.GetHashCode());
            hash.Combine(Extent.GetHashCode());
            hash.Combine(AxisX.GetHashCode());
            hash.Combine(AxisY.GetHashCode());
            hash.Combine(AxisZ.GetHashCode());
            return hash;
        }

        public bool Equals(OBoundingBox other) => Center == other.Center && Extent == other.Extent && AxisX == other.AxisX && AxisY == other.AxisY && AxisZ == other.AxisZ;
        public override bool Equals(object obj) => obj is OBoundingBox other && Equals(other);
    }
}
