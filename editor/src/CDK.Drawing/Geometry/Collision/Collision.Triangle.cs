using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float TriangleGetZ(in Triangle tri, in Vector3 point)     //TODO:테스트필요
        {
            var p0 = tri.Position0.ToVector2();
            var v0 = tri.Position2.ToVector2() - p0;
            var v1 = tri.Position1.ToVector2() - p0;
            var v2 = point.ToVector2() - p0;

            var dot00 = Vector2.Dot(v0, v0);
            var dot01 = Vector2.Dot(v0, v1);
            var dot02 = Vector2.Dot(v0, v2);
            var dot11 = Vector2.Dot(v1, v1);
            var dot12 = Vector2.Dot(v1, v2);

            var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            if (u >= 0 && v >= 0 && u + v < 1)
            {
                var z = tri.Position0.Z + u * (tri.Position2.Z - tri.Position0.Z) + v * (tri.Position1.Z - tri.Position0.Z);

                return z;
            }
            return 0;
        }

        public static void TriangleClosestPoint(in Triangle tri, in Vector3 point, out Vector3 result)
        {
            var ab = tri.Position1 - tri.Position0;
            var ac = tri.Position2 - tri.Position0;
            var ap = point - tri.Position0;

            var d1 = Vector3.Dot(ab, ap);
            var d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0 && d2 <= 0)
            {
                result = tri.Position0;
                return;
            }

            var bp = point - tri.Position1;
            var d3 = Vector3.Dot(ab, bp);
            var d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0 && d4 <= d3)
            {
                result = tri.Position1;
                return;
            }

            var vc = d1 * d4 - d3 * d2;
            if (vc <= 0 && d1 >= 0 && d3 <= 0)
            {
                var v = d1 / (d1 - d3);
                result = tri.Position0 + v * ab;
                return;
            }

            var cp = point - tri.Position2;
            var d5 = Vector3.Dot(ab, cp);
            var d6 = Vector3.Dot(ac, cp);
            if (d6 >= 0 && d5 <= d6)
            {
                result = tri.Position2;
                return;
            }

            var vb = d5 * d2 - d1 * d6;
            if (vb <= 0 && d2 >= 0 && d6 <= 0)
            {
                var w = d2 / (d2 - d6);
                result = tri.Position0 + w * ac;
                return;
            }

            var va = d3 * d6 - d5 * d4;
            if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0)
            {
                var w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                result = tri.Position1 + w * (tri.Position2 - tri.Position1);
                return;
            }

            var denom = 1 / (va + vb + vc);
            var v2 = vb * denom;
            var w2 = vc * denom;
            result = tri.Position0 + ab * v2 + ac * w2;
        }

        public static CollisionResult TriangleIntersectsPoint(in Triangle tri, in Vector3 point)
        {
            var plane = tri.Plane;
            var result = PlaneIntersectsPoint(plane, point);

            if (result == CollisionResult.Intersects)
            {
                var a = tri.Position0 - point;
                var b = tri.Position1 - point;
                var c = tri.Position2 - point;

                var u = Vector3.Cross(b, c);
                var v = Vector3.Cross(c, a);
                var w = Vector3.Cross(a, b);

                if (Vector3.Dot(u, v) < 0 || Vector3.Dot(u, w) < 0) result = CollisionResult.Front;
            }

            return result;
        }

        public static CollisionResult TriangleIntersectsSegment(in Triangle tri, in Segment seg, CollisionFlags flags, out Vector3 near)        
        {
            var diff = seg.Position1 - seg.Position0;
            var length = diff.Length();

            if (!MathUtil.NearZero(length))
            {
                var ray = new Ray(seg.Position0, diff / length);
                var intersects = RayIntersectsTriangle(ray, tri, out float distance);
                if (distance <= length)
                {
                    near = ray.Position + ray.Direction * distance;
                    if (intersects) return CollisionResult.Intersects;
                }
                else near = seg.Position1;

                if ((flags & CollisionFlags.Back) != 0)
                {
                    var plane = tri.Plane;
                    if (PlaneIntersectsPoint(plane, seg.Position0) == CollisionResult.Back && PlaneIntersectsPoint(plane, seg.Position1) == CollisionResult.Back) return CollisionResult.Back;
                }
                return CollisionResult.Front;
            }
            else
            {
                near = seg.Position0;
                return TriangleIntersectsPoint(tri, near);
            }
        }

        private static void TriangleScaleProjectOntoLine(in Triangle tri, in Vector3 direction, out float ext0, out float ext1)
        {
            var t = Vector3.Dot(direction, tri.Position0);

            ext0 = ext1 = t;

            t = Vector3.Dot(direction, tri.Position1);
            if (t < ext0) ext0 = t;
            else if (t > ext1) ext1 = t;

            t = Vector3.Dot(direction, tri.Position2);
            if (t < ext0) ext0 = t;
            else if (t > ext1) ext1 = t;
        }

        private static bool TriangleIntersectsTriangle(Triangle tri0, Triangle tri1)
        {
            var origin = tri0.Position0;
            tri0.Position0 = Vector3.Zero;
            tri0.Position1 -= origin;
            tri0.Position2 -= origin;
            tri1.Position0 -= origin;
            tri1.Position1 -= origin;
            tri1.Position2 -= origin;

            var e0 = new Vector3[]
            {
                tri0.Position1 - tri0.Position0,
                tri0.Position2 - tri0.Position1,
                tri0.Position0 - tri0.Position2
            };
            
            var n0 = Vector3.Cross(e0[0], e0[1]);

            TriangleScaleProjectOntoLine(tri1, n0, out var ext00, out var ext01);

            if (0 < ext00 || ext01 < 0) return false;

            var e1 = new Vector3[]
            {
                tri1.Position1 - tri1.Position0,
                tri1.Position2 - tri1.Position1,
                tri1.Position0 - tri1.Position2
            };

            var n1 = Vector3.Cross(e1[0], e1[1]);

            var proj = Vector3.Dot(n1, tri1.Position0);
            TriangleScaleProjectOntoLine(tri0, n1, out var ext10, out var ext11);
            if (proj < ext10 || ext11 < proj) return false;

            var n0xn1 = Vector3.Cross(n0, n1);

            if (Vector3.Dot(n0xn1, n0xn1) > MathUtil.ZeroTolerance)
            {
                for (var i1 = 0; i1 < 3; i1++)
                {
                    for (var i0 = 0; i0 < 3; i0++)
                    {
                        var direction = Vector3.Cross(e0[i0], e1[i1]);
                        TriangleScaleProjectOntoLine(tri0, direction, out ext00, out ext01);
                        TriangleScaleProjectOntoLine(tri1, direction, out ext10, out ext11);
                        if (ext01 < ext10 || ext11 < ext00) return false;
                    }
                }
            }
            else
            {
                for (var i0 = 0; i0 < 3; i0++)
                {
                    var direction = Vector3.Cross(n0, e0[i0]);
                    TriangleScaleProjectOntoLine(tri0, direction, out ext00, out ext01);
                    TriangleScaleProjectOntoLine(tri1, direction, out ext10, out ext11);
                    if (ext01 < ext10 || ext11 < ext00) return false;
                }
                for (var i1 = 0; i1 < 3; i1++)
                {
                    var direction = Vector3.Cross(n1, e1[i1]);
                    TriangleScaleProjectOntoLine(tri0, direction, out ext00, out ext01);
                    TriangleScaleProjectOntoLine(tri1, direction, out ext10, out ext11);
                    if (ext01 < ext10 || ext11 < ext00) return false;
                }
            }

            return true;
        }

        public static CollisionResult TriangleIntersectsTriangle(in Triangle tri0, in Triangle tri1, CollisionFlags flags) 
        {
            if (TriangleIntersectsTriangle(tri0, tri1)) return CollisionResult.Intersects;

            if ((flags & CollisionFlags.Back) != 0)
            {
                var plane = tri0.Plane;

                if (PlaneIntersectsPoint(plane, tri1.Position0) == CollisionResult.Back &&
                    PlaneIntersectsPoint(plane, tri1.Position1) == CollisionResult.Back &&
                    PlaneIntersectsPoint(plane, tri1.Position2) == CollisionResult.Back)
                {
                    return CollisionResult.Back;
                }
            }
            return CollisionResult.Front;
        }

        public static CollisionResult TriangleIntersectsSphere(in Triangle tri, in BoundingSphere sphere, CollisionFlags flags, out Hit hit)
        {
            if (SphereIntersectsTriangle(sphere, tri, flags & CollisionFlags.Hit, out hit) != CollisionResult.Front)
            {
                hit.Direction = -hit.Direction;
                return CollisionResult.Intersects;
            }
            if ((flags & CollisionFlags.Back) != 0 && PlaneIntersectsSphere(tri.Plane, sphere) == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Front;
        }

        public static CollisionResult TriangleIntersectsCapsule(in Triangle tri, in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            if (CapsuleIntersectsTriangle(capsule, tri, flags & CollisionFlags.Hit, out near, out hit) != CollisionResult.Front)
            {
                hit.Direction = -hit.Direction;
                return CollisionResult.Intersects;
            }
            if ((flags & CollisionFlags.Back) != 0 && PlaneIntersectsCapsule(tri.Plane, capsule, out _) == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Front;
        }

        public static CollisionResult TriangleIntersectsABox(in Triangle tri, in ABoundingBox box, CollisionFlags flags, out Hit hit)
        {
            if (ABoxIntersectsTriangle(box, tri, flags & CollisionFlags.Hit, out hit) != CollisionResult.Front)
            {
                hit.Direction = -hit.Direction;
                return CollisionResult.Intersects;
            }
            if ((flags & CollisionFlags.Back) != 0 && PlaneIntersectsABox(tri.Plane, box) == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Front;
        }

        public static CollisionResult TriangleIntersectsOBox(in Triangle tri, in OBoundingBox box, CollisionFlags flags, out Hit hit)
        {
            if (OBoxIntersectsTriangle(box, tri, flags & CollisionFlags.Hit, out hit) != CollisionResult.Front)
            {
                hit.Direction = -hit.Direction;
                return CollisionResult.Intersects;
            }
            if ((flags & CollisionFlags.Back) != 0 && PlaneIntersectsOBox(tri.Plane, box) == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Front;
        }

        public static CollisionResult TriangleIntersectsMesh(in Triangle tri, BoundingMesh mesh, CollisionFlags flags, out Hit hit)
        {
            if (MeshIntersectsTriangle(mesh, tri, flags & CollisionFlags.Hit, out hit) != CollisionResult.Front)
            {
                hit.Direction = -hit.Direction;
                return CollisionResult.Intersects;
            }
            if ((flags & CollisionFlags.Back) != 0 && PlaneIntersectsMesh(tri.Plane, mesh) == CollisionResult.Back) return CollisionResult.Back;
            return CollisionResult.Front;
        }
    }
}
