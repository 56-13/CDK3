using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static float MeshGetZ(BoundingMesh mesh, in Vector3 point)
        {
            var ray = new Ray(point, -Vector3.UnitZ);
            if (!RayIntersectsMesh(ray, mesh, CollisionFlags.None, out var distance, out _)) return 0;
            return Math.Max(point.Z - distance, 0);
        }

        public static void MeshClosestPoint(BoundingMesh mesh, in Vector3 point, out Vector3 result)
        {
            result = point;
            foreach (var face in mesh.Faces)
            {
                var t = Vector3.Dot(face.Normal, result);
                if (t < face.Min) result += face.Normal * (face.Min - t);
                else if (t > face.Max) result += face.Normal * (face.Max - t);
            }
        }

        public static void MeshClosestNormal(BoundingMesh mesh, in Vector3 point, out Vector3 normal, out float distance)
        {
            distance = float.MinValue;
            normal = Vector3.Zero;

            foreach (var face in mesh.Faces)
            {
                var t = Vector3.Dot(face.Normal, point);
                var d = Math.Max(t - face.Max, face.Min - t);

                if (d > distance)
                {
                    distance = d;
                    normal = face.Normal;
                }
            }
        }

        private static CollisionResult MeshIntersects(BoundingMesh mesh, IEnumerable<Vector3> corners, float radius, out Vector3 inter)
        {
            inter = Vector3.Zero;

            var back = true;
            foreach (var face in mesh.Faces)
            {
                Project(face.Normal, corners, out var min, out var max);
                min -= radius;
                max += radius;
                if (max < face.Min || min > face.Max) return CollisionResult.Front;
                back &= face.Min < min && max <= face.Max;
                inter += face.Normal * (Math.Max(min, face.Min) + Math.Min(max, face.Max));
            }

            inter /= mesh.Faces.Count();

            if (back) return CollisionResult.Back;

            return CollisionResult.Intersects;
        }

        public static CollisionResult MeshIntersectsPoint(BoundingMesh mesh, in Vector3 point, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var result = MeshIntersects(mesh, new Vector3[] { point }, 0, out _);

            if (result != CollisionResult.Front && (flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = point;
                MeshClosestNormal(mesh, point, out hit.Direction, out hit.Distance);
                hit.Direction = -hit.Direction;
            }

            return result;
        }

        public static CollisionResult MeshIntersectsSegment(BoundingMesh mesh, in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            var diff = seg.Position1 - seg.Position0;
            var d2 = diff.LengthSquared();

            if (!MathUtil.NearZero(d2))
            {
                var d = (float)Math.Sqrt(d2);
                var ray = new Ray(seg.Position0, diff / d);
                var intersects = RayIntersectsMesh(ray, mesh, flags & (CollisionFlags.Near | CollisionFlags.Hit), out var distance, out hit);
                hit.Direction = -hit.Direction;

                if (distance <= d)
                {
                    near = ray.Position + ray.Direction * distance;
                    if (intersects) return CollisionResult.Intersects;
                }
                else near = seg.Position1;
            }
            else near = seg.Position0;

            return MeshIntersectsPoint(mesh, near, flags, out hit);
        }

        public static CollisionResult MeshIntersectsTriangle(BoundingMesh mesh, in Triangle tri, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var triNormal = tri.Normal;
            var triOffset = Vector3.Dot(triNormal, tri.Position0);
            Project(triNormal, mesh.Vertices, out var meshMin, out var meshMax);
            if (meshMax < triOffset || meshMin > triOffset) return CollisionResult.Front;

            var triPoints = tri.Positions;

            switch (MeshIntersects(mesh, triPoints, 0, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if (hitFlag)
                    {
                        TriangleClosestPoint(tri, mesh.Center, out hit.Position);
                        hit.Direction = triNormal;
                        hit.Distance = triOffset - meshMin;
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
                foreach (var face in mesh.Faces)
                {
                    var axis = Vector3.Cross(triEdges[i], face.Normal);
                    Project(axis, mesh.Vertices, out var triMeshMin, out var triMeshMax);
                    Project(axis, triPoints, out var triMin, out var triMax);
                    if (triMeshMax <= triMin || triMeshMin >= triMax) return CollisionResult.Front;
                    if (hitFlag) inter1 += axis * ((Math.Max(triMeshMin, triMin) + Math.Min(triMeshMax, triMin)) * 0.5f);
                }
            }

            if (hitFlag)
            {
                inter1 /= 3 * mesh.Faces.Count();
                var inter = (inter0 + inter1) * 0.5f;
                TriangleClosestPoint(tri, inter, out hit.Position);
                hit.Direction = triNormal;
                hit.Distance = triOffset - meshMin;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult MeshIntersectsSphere(BoundingMesh mesh, in BoundingSphere sphere, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var result = MeshIntersects(mesh, new Vector3[] { sphere.Center }, sphere.Radius, out var inter);

            if (result != CollisionResult.Front && (flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = inter;
                MeshClosestNormal(mesh, inter, out hit.Direction, out hit.Distance);
                hit.Distance += sphere.Radius;
                hit.Direction = -hit.Direction;
            }

            return result;
        }

        public static CollisionResult MeshIntersectsCapsule(BoundingMesh mesh, in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit)
        {
            var result = MeshIntersectsSegment(mesh, capsule.Segment, flags | CollisionFlags.Back, out near, out hit);

            if (result == CollisionResult.Intersects) return CollisionResult.Intersects;

            if (result == CollisionResult.Back)
            {
                if (MeshIntersects(mesh, new Vector3[] { near }, capsule.Radius, out _) == CollisionResult.Back) return CollisionResult.Back;

                return CollisionResult.Intersects;
            }

            MeshClosestPoint(mesh, near, out var meshNear);
            var diff = near - meshNear;
            var d2 = diff.LengthSquared();
            var r2 = capsule.Radius * capsule.Radius;

            if (d2 > r2) return CollisionResult.Front;

            if ((flags & CollisionFlags.Hit) != 0)
            {
                hit.Position = meshNear;

                if (MathUtil.NearZero(d2)) hit.Direction = Vector3.Normalize(mesh.Center - meshNear);
                else hit.Direction = diff / (float)Math.Sqrt(d2);
                Project(hit.Direction, mesh.Vertices, near, out var meshMin, out _);
                hit.Distance = -meshMin + capsule.Radius;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult MeshIntersectsABox(BoundingMesh mesh, in ABoundingBox box, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var corners = box.GetCorners();

            switch (MeshIntersects(mesh, corners, 0, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if (hitFlag)
                    {
                        ABoxClosestPoint(box, mesh.Center, out hit.Position);
                        MeshClosestNormal(mesh, box.Center, out hit.Direction, out _);
                        hit.Direction = -hit.Direction;

                        Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                        Project(hit.Direction, corners, out _, out var boxMax);
                        hit.Distance = boxMax - meshMin;
                    }
                    return CollisionResult.Back;
            }

            if (ABoxIntersects(box, mesh.Vertices, out var inter1) == CollisionResult.Front) return CollisionResult.Front;

            if (hitFlag)
            {
                var inter = (inter0 + inter1) * 0.5f;
                ABoxClosestPoint(box, inter, out hit.Position);
                MeshClosestNormal(mesh, inter, out hit.Direction, out _);
                hit.Direction = -hit.Direction;

                Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                Project(hit.Direction, corners, out _, out var boxMax);
                hit.Distance = boxMax - meshMin;
            }

            return CollisionResult.Intersects;
        }

        public static CollisionResult MeshIntersectsOBox(BoundingMesh mesh, in OBoundingBox box, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            var corners = box.GetCorners();

            switch (MeshIntersects(mesh, corners, 0, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if (hitFlag)
                    {
                        OBoxClosestPoint(box, box.Center, out hit.Position);
                        MeshClosestNormal(mesh, box.Center, out hit.Direction, out _);
                        hit.Direction = -hit.Direction;

                        Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                        Project(hit.Direction, corners, out _, out var boxMax);
                        hit.Distance = boxMax - meshMin;
                    }
                    return CollisionResult.Back;
            }

            if (OBoxIntersects(box, mesh.Vertices, out var inter1) == CollisionResult.Front) return CollisionResult.Front;

            if (hitFlag)
            {
                var inter = (inter0 + inter1) * 0.5f;
                OBoxClosestPoint(box, inter, out hit.Position);
                MeshClosestNormal(mesh, inter, out hit.Direction, out _);
                hit.Direction = -hit.Direction;

                Project(hit.Direction, mesh.Vertices, out var meshMin, out _);
                Project(hit.Direction, corners, out _, out var boxMax);
                hit.Distance = boxMax - meshMin;
            }

            return CollisionResult.Intersects;
        }


        public static CollisionResult MeshIntersectsMesh(BoundingMesh mesh0, BoundingMesh mesh1, CollisionFlags flags, out Hit hit)
        {
            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;

            switch (MeshIntersects(mesh0, mesh1.Vertices, 0, out var inter0))
            {
                case CollisionResult.Front:
                    return CollisionResult.Front;
                case CollisionResult.Back:
                    if (hitFlag)
                    {
                        var center = mesh0.Center;
                        MeshClosestPoint(mesh1, center, out hit.Position);
                        MeshClosestNormal(mesh1, center, out hit.Direction, out _);

                        Project(hit.Direction, mesh0.Vertices, out var meshMin0, out _);
                        Project(hit.Direction, mesh1.Vertices, out _, out var meshMax1);
                        hit.Distance = meshMax1 - meshMin0;
                    }
                    return CollisionResult.Back;
            }

            if (MeshIntersects(mesh1, mesh0.Vertices, 0, out var inter1) == CollisionResult.Front) return CollisionResult.Front;

            if (hitFlag)
            {
                var inter = (inter0 + inter1) * 0.5f;
                MeshClosestPoint(mesh1, inter, out hit.Position);
                MeshClosestNormal(mesh1, inter, out hit.Direction, out _);

                Project(hit.Direction, mesh0.Vertices, out var meshMin0, out _);
                Project(hit.Direction, mesh1.Vertices, out _, out var meshMax1);
                hit.Distance = meshMax1 - meshMin0;
            }

            return CollisionResult.Intersects;
        }
    }
}
