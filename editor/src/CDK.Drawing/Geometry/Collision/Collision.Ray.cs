using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static bool RayIntersectsPoint(in Ray ray, in Vector3 point, out float distance)
        {
            var m = ray.Position - point;
            var b = Vector3.Dot(m, ray.Direction);
            var c = Vector3.Dot(m, m) - MathUtil.ZeroTolerance;

            distance = Math.Max(-b, 0);

            if (c > 0 && b > 0) return false;

            var discriminant = b * b - c;

            if (discriminant < 0) return false;

            return true;
        }

        public static bool RayIntersectsSegment(in Ray ray, in Segment seg, out float distance, out Vector3 near)
        {
            var diff = seg.Position1 - seg.Position0;
            var length = diff.Length();

            if (!MathUtil.NearZero(length))
            {
                var ray1 = new Ray(seg.Position0, diff / length);
                var intersects = RayIntersectsRay(ray, ray1, out distance, out var distance1);
                if (distance1 <= length)
                {
                    near = ray1.Position + ray1.Direction * distance1;
                    if (intersects) return true;
                }
                else near = seg.Position1;
                return false;
            }

            near = seg.Position0;
            return RayIntersectsPoint(ray, seg.Position0, out distance);
        }

        public static bool RayIntersectsTriangle(in Ray ray, in Triangle tri, out float distance)
        {
            var v0v1 = tri.Position1 - tri.Position0;
            var v0v2 = tri.Position2 - tri.Position0;
            var normal = Vector3.Cross(v0v1, v0v2);

            var ndotd = Vector3.Dot(normal, ray.Direction);
            if (MathUtil.NearZero(ndotd))
            {
                distance = Math.Max(Vector3.Dot(ray.Direction, tri.Center), 0);
                return false;
            }

            var d = -Vector3.Dot(normal, tri.Position0);

            distance = -(Vector3.Dot(normal, ray.Position) + d) / ndotd;

            if (distance < 0)
            {
                distance = 0;
                return false;
            }

            var p = ray.Position + ray.Direction * distance;

            var edge0 = tri.Position1 - tri.Position0;
            var vp0 = p - tri.Position0;
            var c = Vector3.Cross(edge0, vp0);
            if (Vector3.Dot(normal, c) < 0) return false;

            var edge1 = tri.Position2 - tri.Position1;
            var vp1 = p - tri.Position1;
            c = Vector3.Cross(edge1, vp1);
            if (Vector3.Dot(normal, c) < 0) return false;

            var edge2 = tri.Position0 - tri.Position2;
            var vp2 = p - tri.Position2;
            c = Vector3.Cross(edge2, vp2);
            if (Vector3.Dot(normal, c) < 0) return false;

            return true;
        }

        public static bool RayIntersectsRay(in Ray ray0, in Ray ray1, out float distance0, out float distance1)
        {
            var cross = Vector3.Cross(ray0.Direction, ray1.Direction);
            var denominator = cross.Length();

            if (MathUtil.NearZero(denominator))
            {
                if (VectorUtil.NearEqual(ray0.Position, ray1.Position))
                {
                    distance0 = distance1 = 0;
                    return true;
                }
            }

            denominator *= denominator;

            var m11 = ray1.Position.X - ray0.Position.X;
            var m12 = ray1.Position.Y - ray0.Position.Y;
            var m13 = ray1.Position.Z - ray0.Position.Z;
            var m21 = ray1.Direction.X;
            var m22 = ray1.Direction.Y;
            var m23 = ray1.Direction.Z;
            var m31 = cross.X;
            var m32 = cross.Y;
            var m33 = cross.Z;

            var dets =
                m11 * m22 * m33 +
                m12 * m23 * m31 +
                m13 * m21 * m32 -
                m11 * m23 * m32 -
                m12 * m21 * m33 -
                m13 * m22 * m31;

            m21 = ray0.Direction.X;
            m22 = ray0.Direction.Y;
            m23 = ray0.Direction.Z;

            var dett =
                m11 * m22 * m33 +
                m12 * m23 * m31 +
                m13 * m21 * m32 -
                m11 * m23 * m32 -
                m12 * m21 * m33 -
                m13 * m22 * m31;

            distance0 = dets / denominator;
            distance1 = dett / denominator;

            var near0 = ray0.Position + ray0.Direction * distance0;
            var near1 = ray1.Position + ray1.Direction * distance1;

            return VectorUtil.NearEqual(near0, near1);
        }

        public static bool RayIntersectsPlane(in Ray ray, in Plane plane, out float distance)
        {
            var direction = Vector3.Dot(plane.Normal, ray.Direction);

            if (MathUtil.NearZero(direction))
            {
                distance = 0;
                return false;
            }

            var position = Vector3.Dot(plane.Normal, ray.Position);
            distance = (-plane.D - position) / direction;

            if (distance < 0)
            {
                distance = 0;
                return false;
            }

            return true;
        }

        public static bool RayIntersectsSphere(in Ray ray, in BoundingSphere sphere, CollisionFlags flags, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            var m = ray.Position - sphere.Center;
            var b = Vector3.Dot(m, ray.Direction);
            var c = Vector3.Dot(m, m) - (sphere.Radius * sphere.Radius);

            distance = Math.Max(-b, 0);

            if (c > 0 && b > 0) return false;

            var discriminant = b * b - c;

            if (discriminant < 0) return false;

            distance = Math.Max(distance - (float)Math.Sqrt(discriminant), 0);

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = ray.Position + ray.Direction * distance;
                hit.Direction = Vector3.Normalize(ray.Position - sphere.Center);
            }

            return true;
        }

        public static bool RayIntersectsCapsule(in Ray ray, in BoundingCapsule capsule, CollisionFlags flags, out float distance, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            if (RayIntersectsSegment(ray, capsule.Segment, out distance, out near))
            {
                distance = Math.Max(distance - capsule.Radius, 0);

                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = near;
                }

                return true;
            }

            var rayNear = ray.Position + ray.Direction * distance;
            var r2 = capsule.Radius * capsule.Radius;
            var d2 = Vector3.DistanceSquared(rayNear, near);

            if (d2 > r2) return false;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = ray.Position + ray.Direction * distance;
                hit.Direction = Vector3.Normalize(ray.Position - near);
            }

            distance -= (float)Math.Sqrt(r2 - d2);
            if (distance < 0) distance = 0;

            return true;
        }

        private static bool Clip(float denom, float numer, ref float t0, ref float t1)
        {
            if (denom > 0)
            {
                if (numer > denom * t1) return false;
                if (numer > denom * t0) t0 = numer / denom;
                return true;
            }
            else if (denom < 0)
            {
                if (numer > denom * t0) return false;
                if (numer > denom * t1) t1 = numer / denom;
                return true;
            }
            else
            {
                return numer <= 0;
            }
        }

        public static bool RayIntersectsABox(in Ray ray, in ABoundingBox box, CollisionFlags flags, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            var origin = ray.Position - box.Center;
            var extent = box.Extent;

            var t0 = float.MinValue;
            var t1 = float.MaxValue;

            if (Clip(ray.Direction.X, -origin.X - extent.X, ref t0, ref t1) &&
                Clip(-ray.Direction.X, origin.X - extent.X, ref t0, ref t1) &&
                Clip(ray.Direction.Y, -origin.Y - extent.Y, ref t0, ref t1) &&
                Clip(-ray.Direction.Y, origin.Y - extent.Y, ref t0, ref t1) &&
                Clip(ray.Direction.Z, -origin.Z - extent.Z, ref t0, ref t1) &&
                Clip(-ray.Direction.Z, origin.Z - extent.Z, ref t0, ref t1))
            {
                distance = t0;

                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = ray.Position + ray.Direction * distance;
                    ABoxClosestNormalInternal(box, hit.Position, out hit.Direction, out _);
                }
                return true;
            }

            distance = 0;

            if ((flags & CollisionFlags.Near) != 0)
            {
                var segDistance = box.Maximum - box.Minimum;

                var segs = new (Ray ray, float distance)[]
                {
                    (new Ray(box.Minimum, Vector3.UnitX), segDistance.X),
                    (new Ray(new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z), Vector3.UnitX), segDistance.X),
                    (new Ray(new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z), Vector3.UnitX), segDistance.X),
                    (new Ray(new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z), Vector3.UnitX), segDistance.X),

                    (new Ray(box.Minimum, Vector3.UnitY), segDistance.Y),
                    (new Ray(new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z), Vector3.UnitY), segDistance.Y),
                    (new Ray(new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z), Vector3.UnitY), segDistance.Y),
                    (new Ray(new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z), Vector3.UnitY), segDistance.Y),

                    (new Ray(box.Minimum, Vector3.UnitZ), segDistance.Z),
                    (new Ray(new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z), Vector3.UnitZ), segDistance.Z),
                    (new Ray(new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z), Vector3.UnitZ), segDistance.Z),
                    (new Ray(new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z), Vector3.UnitZ), segDistance.Z),
                };

                var diff2 = float.MaxValue;

                foreach (var seg in segs)
                {
                    var flag = RayIntersectsRay(ray, seg.ray, out var d0, out var d1);
                    var near0 = ray.Position + ray.Direction * d0;
                    var near1 = seg.ray.Position + seg.ray.Direction * Math.Min(d1, seg.distance);
                    var d2 = Vector3.DistanceSquared(near0, near1);
                    if (d2 < diff2)
                    {
                        diff2 = d2;
                        distance = d0;
                    }
                }
            }

            return false;
        }

        public static bool RayIntersectsOBox(in Ray ray, in OBoundingBox box, CollisionFlags flags, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            var diff = ray.Position - box.Center;
            var origin = new Vector3(
                Vector3.Dot(diff, box.AxisX),
                Vector3.Dot(diff, box.AxisY),
                Vector3.Dot(diff, box.AxisZ)
            );
            var direction = new Vector3(
                Vector3.Dot(ray.Direction, box.AxisX),
                Vector3.Dot(ray.Direction, box.AxisY),
                Vector3.Dot(ray.Direction, box.AxisZ)
            );

            var t0 = float.MinValue;
            var t1 = float.MaxValue;

            if (Clip(direction.X, -origin.X - box.Extent.X, ref t0, ref t1) &&
                Clip(-direction.X, origin.X - box.Extent.X, ref t0, ref t1) &&
                Clip(direction.Y, -origin.Y - box.Extent.Y, ref t0, ref t1) &&
                Clip(-direction.Y, origin.Y - box.Extent.Y, ref t0, ref t1) &&
                Clip(direction.Z, -origin.Z - box.Extent.Z, ref t0, ref t1) &&
                Clip(-direction.Z, origin.Z - box.Extent.Z, ref t0, ref t1))
            {
                var cp = origin + direction * t0;
                distance = Vector3.Distance(ray.Position, cp);

                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = cp;
                    OBoxProject(box, cp, out var proj);
                    OBoxClosestNormalInside(box, proj, out hit.Direction, out _);
                }

                return true;
            }

            distance = 0;

            if ((flags & CollisionFlags.Near) != 0)
            {
                var segDistance = box.Extent * 2;

                var dx = box.Extent.X * box.AxisX;
                var dy = box.Extent.Y * box.AxisY;
                var dz = box.Extent.Z * box.AxisZ;

                var segs = new (Ray ray, float distance)[]
                {
                    (new Ray(box.Center - dx - dy - dz, box.AxisX), segDistance.X),
                    (new Ray(box.Center - dx + dy - dz, box.AxisX), segDistance.X),
                    (new Ray(box.Center - dx - dy + dz, box.AxisX), segDistance.X),
                    (new Ray(box.Center - dx + dy + dz, box.AxisX), segDistance.X),

                    (new Ray(box.Center - dx - dy - dz, box.AxisY), segDistance.Y),
                    (new Ray(box.Center + dx - dy - dz, box.AxisY), segDistance.Y),
                    (new Ray(box.Center - dx - dy + dz, box.AxisY), segDistance.Y),
                    (new Ray(box.Center + dx - dy + dz, box.AxisY), segDistance.Y),

                    (new Ray(box.Center - dx - dy - dz, box.AxisZ), segDistance.Z),
                    (new Ray(box.Center + dx - dy - dz, box.AxisZ), segDistance.Z),
                    (new Ray(box.Center - dx + dy - dz, box.AxisZ), segDistance.Z),
                    (new Ray(box.Center + dx + dy - dz, box.AxisZ), segDistance.Z),
                };

                var diff2 = float.MaxValue;

                foreach (var seg in segs)
                {
                    var flag = RayIntersectsRay(ray, seg.ray, out var d0, out var d1);
                    var near0 = ray.Position + ray.Direction * d0;
                    var near1 = seg.ray.Position + seg.ray.Direction * Math.Min(d1, seg.distance);
                    var d2 = Vector3.DistanceSquared(near0, near1);
                    if (d2 < diff2)
                    {
                        diff2 = d2;
                        distance = d0;
                    }
                }
            }

            return false;
        }

        private static bool RayIntersectsMeshFace(in Ray ray, BoundingMesh mesh, BoundingMeshFace face, out float distance)         //TODO:check backface
        {
            var origin = mesh.GetVertex(face.GetIndex(0));

            distance = Vector3.Dot(origin - ray.Position, face.Normal);

            var denom = Vector3.Dot(ray.Direction - origin, face.Normal);
            if (MathUtil.NearZero(denom)) return false;

            var near = ray.Position + ray.Direction * distance;

            var v0 = near - origin;
            var v1 = Vector3.Cross(face.Normal, v0);

            var inside = false;

            var p = mesh.GetVertex(face.GetIndex(1)) - near;
            var pp0 = Vector3.Dot(v0, p) > 0;
            var pp1 = Vector3.Dot(v1, p) > 0;

            var p1 = false;

            var index = 2;

            while (index < face.IndexCount)
            {
                p = mesh.GetVertex(face.GetIndex(index)) - near;
                var p0 = Vector3.Dot(v0, p) > 0;
        
                if (pp0 || p0)
                {
                    p1 = Vector3.Dot(v1, p) > 0;

                    if (pp1 ^ p1) inside = !inside;
                }
                pp0 = p0;
                pp1 = p1;
                index++;
            }

            return inside;
        }

        public static bool RayIntersectsMesh(in Ray ray, BoundingMesh mesh, CollisionFlags flags, out float distance, out Hit hit)
        {
            distance = float.MaxValue;
            hit = Hit.Zero;

            foreach (var face in mesh.Faces)
            {
                if (RayIntersectsMeshFace(ray, mesh, face, out var d) && d < distance)
                {
                    distance = d;
                    hit.Direction = face.Normal;
                }
            }

            if (distance != float.MaxValue)
            {
                hit.Position = ray.Position + ray.Direction * distance;
                return true;
            }
            distance = 0;
            return false;
        }

        public static bool RayIntersectsFrustum(in Ray ray, in BoundingFrustum frustum, out float inDistance, out float outDistance)
        {
            if (FrustumIntersectsPoint(frustum, ray.Position) != CollisionResult.Front)
            {
                var nearstPlaneDistance = float.MaxValue;
                for (var i = 0; i < 6; i++)
                {
                    var plane = frustum.GetPlane(i);
                    if (ray.Intersects(plane, out float distance) && distance < nearstPlaneDistance)
                    {
                        nearstPlaneDistance = distance;
                    }
                }

                inDistance = outDistance = nearstPlaneDistance;
                return true;
            }
            else
            {
                var minDist = float.MaxValue;
                var maxDist = float.MinValue;
                for (var i = 0; i < 6; i++)
                {
                    var plane = frustum.GetPlane(i);
                    if (ray.Intersects(plane, out float distance))
                    {
                        minDist = Math.Min(minDist, distance);
                        maxDist = Math.Max(maxDist, distance);
                    }
                }

                var minPoint = ray.Position + ray.Direction * minDist;
                var maxPoint = ray.Position + ray.Direction * maxDist;
                var center = (minPoint + maxPoint) * 0.5f;
                if (FrustumIntersectsPoint(frustum, center) != CollisionResult.Front)
                {
                    inDistance = minDist;
                    outDistance = maxDist;
                    return true;
                }
                else
                {
                    inDistance = float.MaxValue;
                    outDistance = float.MaxValue;
                    return false;
                }
            }
        }
    }
}
