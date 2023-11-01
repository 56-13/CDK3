using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float SphereGetZ(in BoundingSphere sphere, in Vector3 point)
        {
            var d = Vector2.Distance(sphere.Center.ToVector2(), point.ToVector2());

            if (d <= sphere.Radius)
            {
                var z = sphere.Center.Z + (float)Math.Sqrt(sphere.Radius * sphere.Radius - d * d);

                if (point.Z >= z) return z;
            }

            return 0;
        }

        public static CollisionResult SphereIntersectsPoint(in BoundingSphere sphere, in Vector3 point, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var diff = sphere.Center - point;
            var d2 = diff.LengthSquared();
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2 - MathUtil.ZeroTolerance) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = point;

                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = sphere.Radius - d;
                }
            }

            if (d2 < r2 + MathUtil.ZeroTolerance) return CollisionResult.Back;
            return CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsSegment(in BoundingSphere sphere, in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            SegmentToPoint(seg, sphere.Center, out near);

            var diff = sphere.Center - near;
            var d2 = Vector3.DistanceSquared(sphere.Center, near);
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near;

                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = sphere.Radius - d;
                }
            }

            if ((flags & CollisionFlags.Back) == 0) return CollisionResult.Intersects;

            d2 = Vector3.DistanceSquared(sphere.Center, seg.Position0);
            if (d2 >= r2) return CollisionResult.Intersects;
            d2 = Vector3.DistanceSquared(sphere.Center, seg.Position1);
            if (d2 >= r2) return CollisionResult.Intersects;
            return CollisionResult.Back;
        }

        public static CollisionResult SphereIntersectsTriangle(in BoundingSphere sphere, in Triangle tri, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var a = tri.Position0 - sphere.Center;
            var b = tri.Position1 - sphere.Center;
            var c = tri.Position2 - sphere.Center;
            var aa = Vector3.Dot(a, a);
            var ab = Vector3.Dot(a, b);
            var ac = Vector3.Dot(a, c);
            var bb = Vector3.Dot(b, b);
            var bc = Vector3.Dot(b, c);
            var cc = Vector3.Dot(c, c);
            var r2 = sphere.Radius * sphere.Radius;

            var back = aa < r2 && bb < r2 && cc < r2;

            if (!back)
            {
                var v = Vector3.Cross(b - a, c - a);
                var d = Vector3.Dot(a, v);
                var e = Vector3.Dot(v, v);

                if ((d * d > r2 * e) ||
                    (aa > r2 && ab > aa && ac > aa) ||
                    (bb > r2 && ab > bb && bc > bb) ||
                    (cc > r2 && ac > cc && bc > cc))
                {
                    return CollisionResult.Front;
                }

                var abv = b - a;
                var bcv = c - b;
                var cav = a - c;
                var d1 = ab - aa;
                var d2 = bc - bb;
                var d3 = ac - cc;
                var e1 = Vector3.Dot(abv, abv);
                var e2 = Vector3.Dot(bcv, bcv);
                var e3 = Vector3.Dot(cav, cav);
                var q1 = a * e1 - d1 * abv;
                var q2 = b * e2 - d2 * bcv;
                var q3 = c * e3 - d3 * cav;
                var qc = c * e1 - q1;
                var qa = a * e2 - q2;
                var qb = b * e3 - q3;
                if ((Vector3.Dot(q1, q1) > r2 * e1 * e1 && Vector3.Dot(q1, qc) > 0) ||
                    (Vector3.Dot(q2, q2) > r2 * e2 * e2 && Vector3.Dot(q2, qa) > 0) ||
                    (Vector3.Dot(q3, q3) > r2 * e3 * e3 && Vector3.Dot(q3, qb) > 0))
                {
                    return CollisionResult.Front;
                }
            }

            if ((flags & CollisionFlags.Hit) != 0)
            {
                TriangleClosestPoint(tri, sphere.Center, out hit.Position);
                hit.Direction = tri.Normal;
                hit.Distance = sphere.Radius - Vector3.Dot(hit.Direction, tri.Position0 - sphere.Center);
            }

            return back ? CollisionResult.Back : CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsSphere(in BoundingSphere sphere0, in BoundingSphere sphere1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var diff = sphere0.Center - sphere1.Center;
            var d2 = diff.LengthSquared();
            var r2 = (sphere0.Radius + sphere1.Radius) * (sphere0.Radius + sphere1.Radius);

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = sphere0.Radius + sphere1.Radius - d;
                    hit.Position = sphere1.Center + hit.Direction * Math.Min(d, sphere1.Radius);
                }
                else hit.Position = sphere1.Center;
            }

            if ((flags & CollisionFlags.Back) != 0 && sphere0.Radius > sphere1.Radius)
            {
                r2 = (sphere0.Radius - sphere1.Radius) * (sphere0.Radius - sphere1.Radius);

                if (d2 < r2) return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsCapsule(in BoundingSphere sphere, in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            SegmentToPoint(new Segment(capsule.Position0, capsule.Position1), sphere.Center, out near);

            var diff = sphere.Center - near;
            var d2 = diff.LengthSquared();
            var r2 = (sphere.Radius + capsule.Radius) * (sphere.Radius + capsule.Radius);

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = sphere.Radius + capsule.Radius - d;
                    hit.Position = near + hit.Direction * Math.Min(d, capsule.Radius);
                }
                else hit.Position = near;
            }

            if ((flags & CollisionFlags.Back) != 0 && sphere.Radius > capsule.Radius)
            {
                r2 = (sphere.Radius - capsule.Radius) * (sphere.Radius - capsule.Radius);

                if (Vector3.DistanceSquared(sphere.Center, capsule.Position0) < r2 && Vector3.DistanceSquared(sphere.Center, capsule.Position1) < r2)
                {
                    return CollisionResult.Back;
                }
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsABox(in BoundingSphere sphere, in ABoundingBox box, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var near = Vector3.Clamp(sphere.Center, box.Minimum, box.Maximum);
            var d2 = Vector3.DistanceSquared(sphere.Center, near);
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near;
                ABoxClosestNormal(box, sphere.Center, out hit.Direction, out hit.Distance);
                hit.Distance += sphere.Radius;
            }

            if ((flags & CollisionFlags.Back) != 0)
            {
                foreach (var p in box.GetCorners())
                {
                    if (Vector3.DistanceSquared(p, sphere.Center) >= r2) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsOBox(in BoundingSphere sphere, in OBoundingBox box, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            OBoxClosestPoint(box, sphere.Center, out var proj, out var near);
            var diff = near - sphere.Center;
            var d2 = diff.LengthSquared();
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near;
                OBoxClosestNormal(box, sphere.Center, proj, out hit.Direction, out hit.Distance);
                hit.Distance += sphere.Radius;
            }

            if ((flags & CollisionFlags.Back) != 0)
            {
                foreach (var p in box.GetCorners())
                {
                    if (Vector3.DistanceSquared(p, sphere.Center) >= r2) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }

        public static CollisionResult SphereIntersectsMesh(in BoundingSphere sphere, BoundingMesh mesh, CollisionFlags flags, out Hit hit)
        {
            if (MeshIntersectsSphere(mesh, sphere, flags & CollisionFlags.Hit, out hit) == CollisionResult.Front) return CollisionResult.Front;

            hit.Direction = -hit.Direction;

            if ((flags & CollisionFlags.Back) != 0)
            {
                foreach (var p in mesh.Vertices)
                {
                    if (SphereIntersectsPoint(sphere, p, CollisionFlags.Back, out _) != CollisionResult.Back) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }
    }
}
