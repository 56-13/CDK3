using System;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float ABoxGetZ(in ABoundingBox box, in Vector3 point)
        {
            if (box.Minimum.X <= point.X && point.X <= box.Maximum.X &&
                box.Minimum.Y <= point.Y && point.Y <= box.Maximum.Y &&
                box.Maximum.Z <= point.Z)
            {
                return box.Maximum.Z;
            }
            return 0;
        }

        public static void ABoxClosestPoint(in ABoundingBox box, in Vector3 point, out Vector3 result)
        {
            result = Vector3.Clamp(point, box.Minimum, box.Maximum);
        }

        private static Vector3[] BoxNormals =
        {
            Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ,
            -Vector3.UnitX, -Vector3.UnitY, -Vector3.UnitZ
        };

        private static void ABoxClosestNormalInternal(in ABoundingBox box, in Vector3 point, out Vector3 normal, out float distance)
        {
            var distances = new float[]
            {
                box.Maximum.X - point.X,
                box.Maximum.Y - point.Y,
                box.Maximum.Z - point.Z,
                point.X - box.Minimum.X,
                point.Y - box.Minimum.Y,
                point.Z - box.Minimum.Z
            };
            distance = distances[0];
            normal = BoxNormals[0];

            for (var i = 1; i < 6; i++)
            {
                if (distance > distances[i])
                {
                    distance = distances[i];
                    normal = BoxNormals[i];
                }
            }
        }

        public static void ABoxClosestNormal(in ABoundingBox box, in Vector3 point, out Vector3 normal, out float distance)
        {
            ABoxClosestPoint(box, point, out var near);

            var diff = point - near;
            var d2 = diff.LengthSquared();

            if (MathUtil.NearZero(d2)) ABoxClosestNormalInternal(box, point, out normal, out distance);
            else
            {
                var d = (float)Math.Sqrt(d2);
                distance = -d;
                normal = diff / d;
            }
        }

        private static CollisionResult ABoxIntersects(in ABoundingBox box, IEnumerable<Vector3> corners, out Vector3 inter)
        {
            inter = Vector3.Zero;

            var back = true;
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var corner in corners)
            {
                min = Vector3.Min(min, corner);
                max = Vector3.Max(max, corner);
                back &=
                    box.Minimum.X < corner.X && corner.X < box.Maximum.X &&
                    box.Minimum.Y < corner.Y && corner.Y < box.Maximum.Y &&
                    box.Minimum.Z < corner.Z && corner.Z < box.Maximum.Z;
            }

            if (box.Maximum.X < min.X || box.Minimum.X > max.X ||
                box.Maximum.Y < min.Y || box.Minimum.Y > max.Y ||
                box.Maximum.Z < min.Z || box.Minimum.Z > max.Z)
            {
                return CollisionResult.Front;
            }

            inter = (Vector3.Max(box.Minimum, min) + Vector3.Min(box.Maximum, max)) * 0.5f;

            return back ? CollisionResult.Back : CollisionResult.Intersects;
        }

        public static CollisionResult ABoxIntersectsPoint(in ABoundingBox box, in Vector3 point, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            if (box.Minimum.X <= point.X && point.X <= box.Maximum.X &&
                box.Minimum.Y <= point.Y && point.Y <= box.Maximum.Y &&
                box.Minimum.Z <= point.Z && point.Z <= box.Maximum.Z)
            {
                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = point;
                    ABoxClosestNormalInternal(box, hit.Position, out hit.Direction, out hit.Distance);
                    hit.Direction = -hit.Direction;
                }

                if ((flags & CollisionFlags.Back) != 0 &&
                    point.X >= box.Minimum.X + MathUtil.ZeroTolerance &&
                    point.X <= box.Maximum.X - MathUtil.ZeroTolerance &&
                    point.Y >= box.Minimum.Y + MathUtil.ZeroTolerance &&
                    point.Y <= box.Maximum.Y - MathUtil.ZeroTolerance &&
                    point.Z >= box.Minimum.Z + MathUtil.ZeroTolerance &&
                    point.Z <= box.Maximum.Z - MathUtil.ZeroTolerance)
                {
                    return CollisionResult.Back;
                }

                return CollisionResult.Intersects;
            }
            
            return CollisionResult.Front;
        }

        public static CollisionResult ABoxIntersectsSegment(in ABoundingBox box, in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            var diff = seg.Position1 - seg.Position0;
            var d2 = diff.LengthSquared();

            if (!MathUtil.NearZero(d2))
            {
                var d = (float)Math.Sqrt(d2);
                var ray = new Ray(seg.Position0, diff / d);
                var intersects = RayIntersectsABox(ray, box, CollisionFlags.Near, out var distance, out _);

                if (distance <= d)
                {
                    near = ray.Position + ray.Direction * distance;
                    if (intersects)
                    {
                        if ((flags & CollisionFlags.Hit) != 0)
                        {
                            hit.Position = near;
                            ABoxClosestNormalInternal(box, hit.Position, out hit.Direction, out hit.Distance);
                            hit.Direction = -hit.Direction;
                        }
                        return CollisionResult.Intersects;
                    }
                }
                else near = seg.Position1;
            }
            else near = seg.Position0;

            return ABoxIntersectsPoint(box, near, flags, out hit);
        }

        public static CollisionResult ABoxIntersectsTriangle(in ABoundingBox box, in Triangle tri, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var boxCorners = box.GetCorners();
            var triNormal = tri.Normal;
            var triOffset = Vector3.Dot(triNormal, tri.Position0);
            Project(triNormal, boxCorners, out var boxMin, out var boxMax);
            if (boxMax < triOffset || boxMin > triOffset) return CollisionResult.Front;

            var triPoints = tri.Positions;

            switch (ABoxIntersects(box, triPoints, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if (hitFlag)
                    {
                        TriangleClosestPoint(tri, box.Center, out hit.Position);
                        hit.Direction = triNormal;
                        hit.Distance = triOffset - boxMin;
                    }
                    return CollisionResult.Back;
            }


            var triEdges = new Vector3[]
            {
                tri.Position0 - tri.Position1,
                tri.Position1 - tri.Position2,
                tri.Position2 - tri.Position0
            };

            var inter1 = Vector3.Zero;
            for (var i = 0; i < 3; i++) 
            { 
                for (var j = 0; j < 3; j++)
                {
                    var axis = Vector3.Cross(triEdges[i], BoxNormals[j]);
                    Project(axis, boxCorners, out var triBoxMin, out var triBoxMax);
                    Project(axis, triPoints, out var triMin, out var triMax);
                    if (triBoxMax <= triMin || triBoxMin >= triMax) return CollisionResult.Front;
                    if (hitFlag) inter1 += axis * ((Math.Max(triBoxMin, triMin) + Math.Min(triBoxMax, triMax)) * 0.5f);
                }
            }

            if (hitFlag)
            {
                inter1 /= 3;
                var inter = (inter0 + inter1) * 0.5f;
                TriangleClosestPoint(tri, inter, out hit.Position);
                hit.Direction = tri.Normal;
                hit.Distance = triOffset - boxMin;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult ABoxIntersectsSphere(in ABoundingBox box, in BoundingSphere sphere, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            ABoxClosestPoint(box, sphere.Center, out var near);
            var d2 = Vector3.DistanceSquared(sphere.Center, near);
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near;
                ABoxClosestNormal(box, sphere.Center, out hit.Direction, out hit.Distance);
                hit.Distance += sphere.Radius;
                hit.Direction = -hit.Direction;
            }

            if ((flags & CollisionFlags.Back) != 0 &&
                box.Minimum.X + sphere.Radius < sphere.Center.X && sphere.Center.X < box.Maximum.X - sphere.Radius &&
                box.Minimum.Y + sphere.Radius < sphere.Center.Y && sphere.Center.Y < box.Maximum.Y - sphere.Radius &&
                box.Minimum.Z + sphere.Radius < sphere.Center.Z && sphere.Center.Z < box.Maximum.Z - sphere.Radius)
            {
                return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult ABoxIntersectsCapsule(in ABoundingBox box, in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            var result = ABoxIntersectsSegment(box, capsule.Segment, flags | CollisionFlags.Back, out near, out hit);
            
            if (result == CollisionResult.Intersects) return CollisionResult.Intersects;

            if (result == CollisionResult.Back)
            {
                if ((flags & CollisionFlags.Back) != 0 &&
                    box.Minimum.X < near.X - capsule.Radius && near.X + capsule.Radius < box.Maximum.X &&
                    box.Minimum.Y < near.Y - capsule.Radius && near.Y + capsule.Radius < box.Maximum.Y &&
                    box.Minimum.Z < near.Z - capsule.Radius && near.Z + capsule.Radius < box.Maximum.Z)
                {
                    return CollisionResult.Back;
                }
                return CollisionResult.Intersects;
            }

            ABoxClosestPoint(box, near, out var boxNear);
            var diff = boxNear - near;
            var d2 = diff.LengthSquared();
            var r2 = capsule.Radius * capsule.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = boxNear;

                if (!MathUtil.NearZero(d2))
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Direction = diff / d;
                    hit.Distance = capsule.Radius - d;
                }
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult ABoxIntersectsABox(in ABoundingBox box0, in ABoundingBox box1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            if (box0.Maximum.X < box1.Minimum.X || box0.Minimum.X > box1.Maximum.X ||
                box0.Maximum.Y < box1.Minimum.Y || box0.Minimum.Y > box1.Maximum.Y ||
                box0.Maximum.Z < box1.Minimum.Z || box0.Minimum.Z > box1.Maximum.Z)
            {
                return CollisionResult.Front;
            }

            if ((flags & CollisionFlags.Hit) != 0)
            {
                var min = Vector3.Max(box0.Minimum, box1.Minimum);
                var max = Vector3.Min(box0.Maximum, box1.Maximum);
                hit.Position = (min + max) * 0.5f;
                ABoxClosestNormalInternal(box1, hit.Position, out hit.Direction, out _);
                hit.Distance = Vector3.Dot(hit.Direction, max - min);
            }

            if ((flags & CollisionFlags.Back) != 0 &&
                box0.Minimum.X < box1.Minimum.X && box0.Maximum.X > box1.Maximum.X &&
                box0.Minimum.Y < box1.Minimum.Y && box0.Maximum.Y > box1.Maximum.Y &&
                box0.Minimum.Z < box1.Minimum.Z && box0.Maximum.Z > box1.Maximum.Z)
            {
                return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult ABoxIntersectsOBox(in ABoundingBox box0, in OBoundingBox box1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var corners0 = box0.GetCorners();
            var corners1 = box1.GetCorners();

            switch (ABoxIntersects(box0, corners1, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if ((flags & CollisionFlags.Hit) != 0)
                    {
                        OBoxClosestPoint(box1, box0.Center, out hit.Position);
                        OBoxClosestNormal(box1, box0.Center, out hit.Direction, out _);

                        Project(hit.Direction, corners0, out var boxMin0, out _);
                        Project(hit.Direction, corners1, out _, out var boxMax1);
                        hit.Distance = boxMax1 - boxMin0;
                    }
                    return CollisionResult.Back;
            }

            if (OBoxIntersects(box1, corners0, out var inter1) == CollisionResult.Front) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                var inter = (inter0 + inter1) * 0.5f;

                OBoxClosestPoint(box1, inter, out hit.Position);
                OBoxClosestNormal(box1, inter, out hit.Direction, out _);

                Project(hit.Direction, corners0, out var boxMin0, out _);
                Project(hit.Direction, corners1, out _, out var boxMax1);
                hit.Distance = boxMax1 - boxMin0;
            }
            return CollisionResult.Intersects;
        }


        public static CollisionResult ABoxIntersectsMesh(in ABoundingBox box, BoundingMesh mesh, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var corners = box.GetCorners();

            switch (ABoxIntersects(box, mesh.Vertices, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if ((flags & CollisionFlags.Hit) != 0)
                    {
                        MeshClosestPoint(mesh, box.Center, out hit.Position);
                        MeshClosestNormal(mesh, box.Center, out hit.Direction, out _);

                        Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                        Project(hit.Direction, corners, out _, out var boxMax);
                        hit.Distance = boxMax - meshMin;
                    }
                    return CollisionResult.Back;
            }

            if (MeshIntersects(mesh, corners, 0, out var inter1) == CollisionResult.Front) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                var inter = (inter0 + inter1) * 0.5f;
                MeshClosestPoint(mesh, inter, out hit.Position);
                MeshClosestNormal(mesh, inter, out hit.Direction, out _);

                Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                Project(hit.Direction, corners, out _, out var boxMax);
                hit.Distance = boxMax - meshMin;
            }

            return CollisionResult.Intersects;
        }
    }
}
