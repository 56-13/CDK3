using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float CapsuleGetZ(in BoundingCapsule capsule, in Vector3 point)
        {
            var v = capsule.Position1.ToVector2() - capsule.Position0.ToVector2();

            var len2 = v.LengthSquared();

            Vector3 near;

            if (MathUtil.NearZero(len2))
            {
                near = capsule.Position0;
            }
            else
            {
                var pv = point.ToVector2() - capsule.Position0.ToVector2();

                var t = Vector2.Dot(pv, v) / len2;

                if (t >= 1) near = capsule.Position1;
                else if (t > 0) near = Vector3.Lerp(capsule.Position0, capsule.Position1, t);
                else near = capsule.Position0;
            }

            var d = Vector2.Distance(point.ToVector2(), near.ToVector2());

            if (d <= capsule.Radius)
            {
                var z = near.Z + (float)Math.Sqrt(capsule.Radius * capsule.Radius - d * d);

                if (point.Z >= z) return z;
            }

            return 0;
        }

        public static CollisionResult CapsuleIntersectsPoint(in BoundingCapsule capsule, in Vector3 point, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            SegmentToPoint(capsule.Segment, point, out near);
            var diff = near - point;
            var d2 = diff.LengthSquared();
            var r2 = capsule.Radius * capsule.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = point;
                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = capsule.Radius - d;
                }
            }

            if (d2 < r2 + MathUtil.ZeroTolerance) return CollisionResult.Back;
            
            return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsSegment(in BoundingCapsule capsule, in Segment seg, CollisionFlags flags, out Vector3 near0, out Vector3 near1, out Hit hit)
        {
            hit = Hit.Zero;

            var cseg = capsule.Segment;
            SegmentToSegment(cseg, seg, out near0, out near1);
            var diff = near0 - near1;
            var d2 = diff.LengthSquared();
            var r2 = capsule.Radius * capsule.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near1;

                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = capsule.Radius - d;
                }
            }

            if ((flags & CollisionFlags.Back) != 0)
            {
                SegmentToPoint(cseg, seg.Position0, out var pnear);
                if (Vector3.DistanceSquared(seg.Position0, pnear) < r2)
                {
                    SegmentToPoint(cseg, seg.Position1, out pnear);
                    if (Vector3.DistanceSquared(seg.Position1, pnear) < r2) return CollisionResult.Back;
                }
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsTriangle(in BoundingCapsule capsule, in Triangle tri, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            near = capsule.Position0;
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var e0 = tri.Position1 - tri.Position0;
            var e1 = tri.Position2 - tri.Position0;
            var e0e1 = Vector3.Cross(e0, e1);
            var pn = Vector3.Normalize(e0e1);

            var cl = capsule.Length();
            var cn = capsule.Normal;

            var denom = Vector3.Dot(pn, cn);
            if (MathUtil.NearZero(denom)) return CollisionResult.Front;

            var po = tri.Position0 + pn * (denom < 0 ? capsule.Radius : -capsule.Radius);

            var u = Vector3.Dot(pn, po - capsule.Position0) / denom;

            if (u > cl)
            {
                near = capsule.Position1;
                return CollisionResult.Front;
            }
            
            var p = capsule.Position0 + cn * u;

            near = p;

            var w = p - tri.Position0;
            var d = Vector3.Dot(e0e1, e0e1);
            var y = Vector3.Dot(Vector3.Cross(e0, w), e0e1) / d; // γ=[(u×w)⋅n]/n²
            var b = Vector3.Dot(Vector3.Cross(w, e1), e0e1) / d; // β=[(w×v)⋅n]/n²
            var a = 1 - y - b;

            if (a < 0 || a > 1 || b < 0 || b > 1 || y < 0 || y > 1)
            {
                SegmentToPoint(new Segment(tri.Position0, tri.Position1), p, out var np0);
                SegmentToPoint(new Segment(tri.Position1, tri.Position2), p, out var np1);
                SegmentToPoint(new Segment(tri.Position2, tri.Position0), p, out var np2);

                var d0 = Vector3.DistanceSquared(p, np0);
                var d1 = Vector3.DistanceSquared(p, np1);
                var d2 = Vector3.DistanceSquared(p, np2);

                var va = tri.Position0;
                var vb = tri.Position1;
                var dt = d0;

                if (d1 < dt) 
                { 
                    dt = d1; 
                    va = tri.Position1; 
                    vb = tri.Position2; 
                }
                if (d2 < dt) 
                { 
                    va = tri.Position2; 
                    vb = tri.Position0; 
                }

                if (!RayIntersectsCapsule(new Ray(capsule.Position0, cn), new BoundingCapsule(va, vb, capsule.Radius), CollisionFlags.None, out u, out _, out _) || u > cl) return CollisionResult.Front;

                if (hitFlag)
                {
                    p = capsule.Position0 + cn * u;

                    var ba = vb - va;
                    var pa = p - va;
                    var h = MathUtil.Clamp(Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba), 0, 1);
                    pn = (pa - ba * h) / capsule.Radius;
                }
            }

            if (u < 0) return CollisionResult.Front;

            if (hitFlag)
            {
                hit.Position = p;
                hit.Direction = pn;
                hit.Distance = capsule.Radius - Vector3.Distance(near, p);
            }

            if ((flags & CollisionFlags.Back) != 0 &&
                CapsuleIntersectsPoint(capsule, tri.Position0, CollisionFlags.Back, out _, out _) == CollisionResult.Back &&
                CapsuleIntersectsPoint(capsule, tri.Position1, CollisionFlags.Back, out _, out _) == CollisionResult.Back &&
                CapsuleIntersectsPoint(capsule, tri.Position2, CollisionFlags.Back, out _, out _) == CollisionResult.Back) 
            {
                return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsSphere(in BoundingCapsule capsule, in BoundingSphere sphere, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            SegmentToPoint(capsule.Segment, sphere.Center, out near);
            var diff = near - sphere.Center;
            var d2 = diff.LengthSquared();
            var r2 = (capsule.Radius + sphere.Radius) * (capsule.Radius + sphere.Radius);

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Position = sphere.Center + hit.Direction * Math.Min(d, sphere.Radius);
                    hit.Distance = sphere.Radius + capsule.Radius - d;
                }
                else hit.Position = sphere.Center;
            }

            if ((flags & CollisionFlags.Back) != 0 && capsule.Radius > sphere.Radius)
            {
                r2 = (capsule.Radius - sphere.Radius) * (capsule.Radius - sphere.Radius);

                if (d2 < r2) return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsCapsule(in BoundingCapsule capsule0, in BoundingCapsule capsule1, CollisionFlags flags, out Vector3 near0, out Vector3 near1, out Hit hit)
        {
            hit = Hit.Zero;

            var cseg0 = new Segment(capsule0.Position0, capsule0.Position1);
            var cseg1 = new Segment(capsule1.Position0, capsule1.Position1);
            SegmentToSegment(cseg0, cseg1, out near0, out near1);
            var diff = near0 - near1;
            var d2 = diff.LengthSquared();
            var r2 = (capsule0.Radius * capsule1.Radius) * (capsule0.Radius * capsule1.Radius);

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Position = near1 + hit.Direction * Math.Min(d, capsule1.Radius);
                    hit.Distance = capsule0.Radius + capsule1.Radius - d;
                }
                else hit.Position = near1;
            }

            if ((flags & CollisionFlags.Back) != 0 && capsule0.Radius > capsule1.Radius)
            {
                r2 = (capsule0.Radius - capsule1.Radius) * (capsule0.Radius - capsule1.Radius);

                if (d2 <= r2)
                {
                    SegmentToPoint(cseg0, capsule1.Position0, out var pnear0);
                    if (Vector3.DistanceSquared(capsule1.Position0, pnear0) < r2)
                    {
                        SegmentToPoint(cseg0, capsule1.Position1, out var pnear1);
                        if (Vector3.DistanceSquared(capsule1.Position1, pnear1) < r2) return CollisionResult.Back;
                    }
                }
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsABox(in BoundingCapsule capsule, in ABoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            if (ABoxIntersectsCapsule(box, capsule, flags & CollisionFlags.Hit, out near, out hit) == CollisionResult.Front) return CollisionResult.Front;

            hit.Direction = -hit.Direction;

            if ((flags & CollisionFlags.Back) != 0)
            {
                var cseg = capsule.Segment;
                var r2 = capsule.Radius * capsule.Radius;
                foreach (var p in box.GetCorners())
                {
                    SegmentToPoint(cseg, p, out var cnear);
                    if (Vector3.DistanceSquared(p, cnear) >= r2) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsOBox(in BoundingCapsule capsule, in OBoundingBox box, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            if (OBoxIntersectsCapsule(box, capsule, flags & CollisionFlags.Hit, out near, out hit) == CollisionResult.Front) return CollisionResult.Front;

            hit.Direction = -hit.Direction;

            if ((flags & CollisionFlags.Back) != 0)
            {
                var cseg = capsule.Segment;
                var r2 = capsule.Radius * capsule.Radius;
                foreach (var p in box.GetCorners())
                {
                    SegmentToPoint(cseg, p, out var cnear);
                    if (Vector3.DistanceSquared(p, cnear) >= r2) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }

        public static CollisionResult CapsuleIntersectsMesh(in BoundingCapsule capsule, BoundingMesh mesh, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            if (MeshIntersectsCapsule(mesh, capsule, flags & CollisionFlags.Hit, out near, out hit) == CollisionResult.Front) return CollisionResult.Front;

            hit.Direction = -hit.Direction;

            if ((flags & CollisionFlags.Back) != 0)
            {
                foreach(var p in mesh.Vertices)
                {
                    if (CapsuleIntersectsPoint(capsule, p, CollisionFlags.Back, out _, out _) != CollisionResult.Back) return CollisionResult.Intersects;
                }
                return CollisionResult.Back;
            }
            else return CollisionResult.Intersects;
        }
    }
}
