using System;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float OBoxGetZ(in OBoundingBox box, in Vector3 point)
        {
            var ray = new Ray(point, -Vector3.UnitZ);
            if (!RayIntersectsOBox(ray, box, CollisionFlags.None, out var distance, out _)) return 0;
            return Math.Max(point.Z - distance, 0);
        }

        private static void OBoxProject(in OBoundingBox box, in Vector3 point, out Vector3 proj)
        {
            proj = new Vector3(
                Vector3.Dot(point - box.Center, box.AxisX),
                Vector3.Dot(point - box.Center, box.AxisY),
                Vector3.Dot(point - box.Center, box.AxisZ));
        }
        
        private static void OBoxClosestPoint(in OBoundingBox box, in Vector3 point, out Vector3 proj, out Vector3 result)
        {
            OBoxProject(box, point, out proj);
            var diff = Vector3.Clamp(proj, -box.Extent, box.Extent);
            result = box.Center + diff.X * box.AxisX + diff.Y * box.AxisY + diff.Z * box.AxisZ;
        }

        public static void OBoxClosestPoint(in OBoundingBox box, in Vector3 point, out Vector3 result)
        {
            OBoxClosestPoint(box, point, out _, out result);
        }

        private static void OBoxClosestNormalInside(in OBoundingBox box, in Vector3 proj, out Vector3 normal, out float distance)
        {
            var distances = new float[]
            {
                box.Extent.X - proj.X,
                box.Extent.Y - proj.Y,
                box.Extent.Z - proj.Z,
                proj.X - box.Extent.X,
                proj.Y - box.Extent.Y,
                proj.Z - box.Extent.Z
            };
            var axis = new Vector3[]
            {
                box.AxisX, box.AxisY, box.AxisZ,
                -box.AxisX, -box.AxisY, -box.AxisZ
            };
            distance = distances[0];
            normal = axis[0];

            for (var i = 1; i < 6; i++)
            {
                if (distance > distances[i])
                {
                    distance = distances[i];
                    normal = axis[i];
                }
            }
        }

        private static void OBoxClosestNormal(in OBoundingBox box, in Vector3 point, in Vector3 proj, out Vector3 normal, out float distance)
        {
            if (-box.Extent.X <= proj.X && proj.X <= box.Extent.X &&
                -box.Extent.Y <= proj.Y && proj.Y <= box.Extent.Y &&
                -box.Extent.Z <= proj.Z && proj.Z <= box.Extent.Z)
            {
                OBoxClosestNormalInside(box, proj, out normal, out distance);
            }
            else
            {
                var offset = Vector3.Clamp(proj, -box.Extent, box.Extent);
                var near = box.Center + offset.X * box.AxisX + offset.Y * box.AxisY + offset.Z * box.AxisY;
                var diff = near - point;
                var d = diff.Length();
                distance = -d;
                normal = diff / d;
            }
        }

        public static void OBoxClosestNormal(in OBoundingBox box, in Vector3 point, out Vector3 normal, out float distance)
        {
            OBoxProject(box, point, out var proj);
            OBoxClosestNormal(box, point, proj, out normal, out distance);
        }
        

        private static CollisionResult OBoxIntersects(in OBoundingBox box, IEnumerable<Vector3> corners, out Vector3 inter)
        {
            inter = Vector3.Zero;
            var contains = true;
            for (var i = 0; i < 3; i++)
            {
                box.GetAxis(i, out var axis);
                Project(axis, corners, box.Center, out var min, out var max);
                var boxMin = -box.Extent.GetComponent(i);
                var boxMax = box.Extent.GetComponent(i);
                if (max < boxMin || min > boxMax) return CollisionResult.Front;
                contains &= boxMin < min && max < boxMax;
                inter += axis * ((Math.Max(boxMin, min) + Math.Min(boxMax, max)) * 0.5f);
            }

            inter += box.Center;

            return contains ? CollisionResult.Back : CollisionResult.Intersects;
        }

        public static CollisionResult OBoxIntersectsPoint(in OBoundingBox box, in Vector3 point, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            OBoxProject(box, point, out var proj);

            if (-box.Extent.X <= proj.X && proj.X <= box.Extent.X &&
                -box.Extent.Y <= proj.Y && proj.Y <= box.Extent.Y &&
                -box.Extent.Z <= proj.Z && proj.Z <= box.Extent.Z)
            {
                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = point;
                    OBoxClosestNormalInside(box, proj, out hit.Direction, out hit.Distance);
                    hit.Direction = -hit.Direction;
                }

                if ((flags & CollisionFlags.Back) != 0 &&
                    proj.X >= -box.Extent.X + MathUtil.ZeroTolerance &&
                    proj.X <= box.Extent.X - MathUtil.ZeroTolerance &&
                    proj.Y >= -box.Extent.Y + MathUtil.ZeroTolerance &&
                    proj.Y <= box.Extent.Y - MathUtil.ZeroTolerance &&
                    proj.Z >= -box.Extent.Z + MathUtil.ZeroTolerance &&
                    proj.Z <= box.Extent.Z - MathUtil.ZeroTolerance)
                {
                    return CollisionResult.Back;
                }

                return CollisionResult.Intersects;
            }

            return CollisionResult.Front;
        }

        public static CollisionResult OBoxIntersectsSegment(in OBoundingBox box, in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            hit = Hit.Zero;

            var diff = seg.Position1 - seg.Position0;
            var d2 = diff.LengthSquared();

            if (!MathUtil.NearZero(d2))
            {
                var d = (float)Math.Sqrt(d2);
                var ray = new Ray(seg.Position0, diff / d);
                var intersects = RayIntersectsOBox(ray, box, CollisionFlags.Near, out var distance, out _);

                if (distance <= d)
                {
                    near = ray.Position + ray.Direction * distance;
                    if (intersects)
                    {
                        if ((flags & CollisionFlags.Hit) != 0)
                        {
                            hit.Position = near;
                            OBoxClosestNormalInside(box, near, out hit.Direction, out hit.Distance);
                            hit.Direction = -hit.Direction;
                        }
                        return CollisionResult.Intersects;
                    }
                }
                else near = seg.Position1;
            }
            else near = seg.Position0;

            return OBoxIntersectsPoint(box, near, flags, out hit);
        }

        public static CollisionResult OBoxIntersectsTriangle(in OBoundingBox box, in Triangle tri, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var boxCorners = box.GetCorners();
            var triNormal = tri.Normal;
            var triOffset = Vector3.Dot(triNormal, tri.Position0);
            Project(triNormal, boxCorners, out var boxMin, out var boxMax);
            if (boxMax < triOffset || boxMin > triOffset) return CollisionResult.Front;

            var triPoints = tri.Positions;

            switch (OBoxIntersects(box, triPoints, out var inter0))
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
                    var axis = Vector3.Cross(triEdges[i], box.GetAxis(j));
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

        public static CollisionResult OBoxIntersectsSphere(in OBoundingBox box, in BoundingSphere sphere, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            OBoxClosestPoint(box, sphere.Center, out var proj, out var near);
            var diff = sphere.Center - near;
            var d2 = diff.LengthSquared();
            var r2 = sphere.Radius * sphere.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = near;
                if (MathUtil.NearZero(d2))
                {
                    OBoxClosestNormalInside(box, proj, out hit.Direction, out hit.Distance);
                    hit.Distance += sphere.Radius;
                    hit.Direction = -hit.Direction;
                }
                else
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Distance = sphere.Radius - d;
                    hit.Direction = diff / d;
                }
            }

            if ((flags & CollisionFlags.Back) != 0 &&
                -box.Extent.X + sphere.Radius < proj.X && proj.X < box.Extent.X - sphere.Radius &&
                -box.Extent.Y + sphere.Radius < proj.Y && proj.Y < box.Extent.Y - sphere.Radius &&
                -box.Extent.Z + sphere.Radius < proj.Z && proj.Z < box.Extent.Z - sphere.Radius)
            {
                return CollisionResult.Back;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult OBoxIntersectsCapsule(in OBoundingBox box, in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            var result = OBoxIntersectsSegment(box, capsule.Segment, flags | CollisionFlags.Back, out near, out hit);
            
            if (result == CollisionResult.Intersects) return CollisionResult.Intersects;

            OBoxProject(box, near, out var proj);

            if (result == CollisionResult.Back)
            {
                if (-box.Extent.X + capsule.Radius < proj.X && proj.X < box.Extent.X - capsule.Radius &&
                    -box.Extent.Y + capsule.Radius < proj.Y && proj.Y < box.Extent.Y - capsule.Radius &&
                    -box.Extent.Z + capsule.Radius < proj.Z && proj.Z < box.Extent.Z - capsule.Radius)
                {
                    return CollisionResult.Back;
                }
                return CollisionResult.Intersects;
            }

            var boxOffset = Vector3.Clamp(proj, -box.Extent, box.Extent);
            var boxNear = box.Center + boxOffset.X * box.AxisX + boxOffset.Y * box.AxisY + boxOffset.Z * box.AxisY;
            var diff = near - boxNear;
            var d2 = diff.LengthSquared();
            var r2 = capsule.Radius * capsule.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = boxNear;

                if (MathUtil.NearZero(d2))
                {
                    OBoxClosestNormalInside(box, proj, out hit.Direction, out hit.Distance);
                    hit.Distance += capsule.Radius;
                    hit.Direction = -hit.Direction;
                }
                else 
                {
                    var d = (float)Math.Sqrt(d2);
                    hit.Distance = capsule.Radius - d;
                    hit.Direction = diff / d;
                }
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult OBoxIntersectsABox(in OBoundingBox box0, in ABoundingBox box1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var corners0 = box0.GetCorners();
            var corners1 = box1.GetCorners();

            switch (OBoxIntersects(box0, corners1, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if ((flags & CollisionFlags.Hit) != 0)
                    {
                        ABoxClosestPoint(box1, box0.Center, out hit.Position);
                        ABoxClosestNormalInternal(box1, hit.Position, out hit.Direction, out _);

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
                ABoxClosestPoint(box1, inter, out hit.Position);
                ABoxClosestNormalInternal(box1, hit.Position, out hit.Direction, out _);

                Project(hit.Direction, corners0, out var boxMin0, out _);
                Project(hit.Direction, corners1, out _, out var boxMax1);
                hit.Distance = boxMax1 - boxMin0;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult OBoxIntersectsOBox(in OBoundingBox box0, in OBoundingBox box1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var corners0 = box0.GetCorners();
            var corners1 = box1.GetCorners();

            switch (OBoxIntersects(box0, corners1, out var inter0))
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

        public static CollisionResult OBoxIntersectsMesh(in OBoundingBox box, BoundingMesh mesh, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var corners = box.GetCorners();

            switch (OBoxIntersects(box, mesh.Vertices, out var inter0))
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
