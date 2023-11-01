using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float PlaneGetZ(in Plane plane, in Vector3 point)
        {
            var ray = new Ray(point, -Vector3.UnitZ);
            if (RayIntersectsPlane(ray, plane, out float distance)) return point.Z - distance;
            return 0;
        }

        public static CollisionResult PlaneIntersectsPoint(in Plane plane, in Vector3 point)
        {
            var distance = Plane.DotCoordinate(plane, point);

            if (distance > MathUtil.ZeroTolerance) return CollisionResult.Front;
            if (distance < -MathUtil.ZeroTolerance) return CollisionResult.Back;
            return CollisionResult.Intersects;
        }

        public static CollisionResult PlaneIntersectsSegment(in Plane plane, in Segment seg, out Vector3 near)
        {
            var diff = seg.Position1 - seg.Position0;
            var length = diff.Length();

            near = seg.Position0;

            if (!MathUtil.NearZero(length))
            {
                var ray = new Ray(seg.Position0, diff / length);
                if (RayIntersectsPlane(ray, plane, out float distance))
                {
                    if (distance <= length)
                    {
                        near = ray.Position + ray.Direction * distance;
                        return CollisionResult.Intersects;
                    }
                    near = seg.Position1;
                }
            }

            return PlaneIntersectsPoint(plane, seg.Position0);
        }

        public static CollisionResult PlaneIntersectsTriangle(in Plane plane, in Triangle tri)
        {
            var test0 = PlaneIntersectsPoint(plane, tri.Position0);
            var test1 = PlaneIntersectsPoint(plane, tri.Position1);
            var test2 = PlaneIntersectsPoint(plane, tri.Position2);

            if (test0 == CollisionResult.Front && test1 == CollisionResult.Front && test2 == CollisionResult.Front) return CollisionResult.Front;
            if (test0 == CollisionResult.Back && test1 == CollisionResult.Back && test2 == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Intersects;
        }

        public static bool PlaneIntersectsPlane(in Plane plane1, in Plane plane2)
        {
            var direction = Vector3.Cross(plane1.Normal, plane2.Normal);

            var denom = Vector3.Dot(direction, direction);

            if (MathUtil.NearZero(denom)) return false;

            return true;
        }

        public static bool PlaneIntersectsPlane(in Plane plane1, in Plane plane2, out Ray line)
        {
            var direction = Vector3.Cross(plane1.Normal, plane2.Normal);

            var denominator = Vector3.Dot(direction, direction);

            if (MathUtil.NearZero(denominator))
            {
                line = Ray.Empty;
                return false;
            }

            var temp = plane1.D * plane2.Normal - plane2.D * plane1.Normal;
            var point = Vector3.Cross(temp, direction);

            line.Position = point;
            line.Direction = Vector3.Normalize(direction);

            return true;
        }

        public static CollisionResult PlaneIntersectsSphere(in Plane plane, in BoundingSphere sphere)
        {
            var distance = Plane.DotCoordinate(plane, sphere.Center);

            if (distance > sphere.Radius) return CollisionResult.Front;
            if (distance < -sphere.Radius) return CollisionResult.Back;
            return CollisionResult.Intersects;
        }

        public static CollisionResult PlaneIntersectsCapsule(in Plane plane, in BoundingCapsule capsule, out Vector3 near)
        {
            var diff = capsule.Position1 - capsule.Position0;
            var length = diff.Length();

            if (!MathUtil.NearZero(length))
            {
                var ray = new Ray(capsule.Position0, diff / length);

                if (RayIntersectsPlane(ray, plane, out var distance))
                {
                    if (distance <= length)
                    {
                        near = ray.Position + ray.Direction * distance;
                        return CollisionResult.Intersects;
                    }
                    else near = capsule.Position1;

                    return PlaneIntersectsSphere(plane, new BoundingSphere(capsule.Position1, capsule.Radius));
                }
                else near = capsule.Position0;
            }
            else near = capsule.Position0;

            return PlaneIntersectsSphere(plane, new BoundingSphere(capsule.Position0, capsule.Radius));
        }

        public static CollisionResult PlaneIntersectsABox(in Plane plane, in ABoundingBox box)
        {
            Vector3 min;
            Vector3 max;
            float distance;

            max.X = (plane.Normal.X >= 0) ? box.Minimum.X : box.Maximum.X;
            max.Y = (plane.Normal.Y >= 0) ? box.Minimum.Y : box.Maximum.Y;
            max.Z = (plane.Normal.Z >= 0) ? box.Minimum.Z : box.Maximum.Z;
            min.X = (plane.Normal.X >= 0) ? box.Maximum.X : box.Minimum.X;
            min.Y = (plane.Normal.Y >= 0) ? box.Maximum.Y : box.Minimum.Y;
            min.Z = (plane.Normal.Z >= 0) ? box.Maximum.Z : box.Minimum.Z;

            distance = Plane.DotCoordinate(plane, max);
            if (distance > 0) return CollisionResult.Front;

            distance = Plane.DotCoordinate(plane, min);
            if (distance < 0) return CollisionResult.Back;

            return CollisionResult.Intersects;
        }

        public static CollisionResult PlaneIntersectsOBox(in Plane plane, in OBoundingBox box)
        {
            var radius = box.Extent.X * Vector3.Dot(plane.Normal, box.AxisX) +
                box.Extent.Y * Vector3.Dot(plane.Normal, box.AxisY) +
                box.Extent.Z * Vector3.Dot(plane.Normal, box.AxisZ);

            var distance = Plane.DotCoordinate(plane, box.Center);

            if (distance > radius) return CollisionResult.Front;
            if (distance < radius) return CollisionResult.Back;
            return CollisionResult.Intersects;
        }

        public static CollisionResult PlaneIntersectsMesh(in Plane plane, BoundingMesh mesh)
        {
            var result = PlaneIntersectsPoint(plane, mesh.GetVertex(0));

            if (result == CollisionResult.Intersects) return CollisionResult.Intersects;

            for (var i = 1; i < mesh.VertexCount; i++)
            {
                if (PlaneIntersectsPoint(plane, mesh.GetVertex(i)) != result) return CollisionResult.Intersects;
            }

            return result;
        }

        public static CollisionResult PlaneIntersectsFrustum(in Plane plane, in BoundingFrustum frustum)
        {
            var frustumCorners = frustum.GetCorners();

            var result = PlaneIntersectsPoint(plane, frustumCorners[0]);

            if (result == CollisionResult.Intersects) return CollisionResult.Intersects;

            for (int i = 1; i < 8; i++)
            {
                if (result != PlaneIntersectsPoint(plane, frustumCorners[i])) return CollisionResult.Intersects;
            }
            return result;
        }
    }
}
