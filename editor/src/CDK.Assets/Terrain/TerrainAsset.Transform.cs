using System;
using System.Numerics;
using System.Collections.Generic;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        private IEnumerable<Triangle> GetTriangles(in ABoundingBox aabb)
        {
            var wtov = (float)_VertexCell / _Grid;
            var vtow = 1f / wtov;

            var vminx = Math.Max((int)(aabb.Minimum.X * wtov), 0);
            var vmaxx = Math.Min((int)(aabb.Maximum.X * wtov + 1), _Width * _VertexCell);
            var vminy = Math.Max((int)(aabb.Minimum.Y * wtov), 0);
            var vmaxy = Math.Min((int)(aabb.Maximum.Y * wtov + 1), _Height * _VertexCell);

            var triangles = new List<Triangle>((vmaxy - vminy) * (vmaxx - vminx) * 2);

            for (var vy = vminy; vy < vmaxy; vy++)
            {
                for (var vx = vminx; vx < vmaxx; vx++)
                {
                    var a0 = _altitudes[vx, vy] * _Grid;
                    var a1 = _altitudes[vx + 1, vy] * _Grid;
                    var a2 = _altitudes[vx, vy + 1] * _Grid;
                    var a3 = _altitudes[vx + 1, vy + 1] * _Grid;

                    var p0 = new Vector3(vx * vtow, vy * vtow, a0);
                    var p1 = new Vector3((vx + 1) * vtow, vy * vtow, a1);
                    var p2 = new Vector3(vx * vtow, (vy + 1) * vtow, a2);
                    var p3 = new Vector3((vx + 1) * vtow, (vy + 1) * vtow, a3);

                    if (IsZQuad(a0, a1, a2, a3))
                    {
                        if (Math.Max(Math.Max(a0, a1), a2) > aabb.Minimum.Z && Math.Min(Math.Min(a0, a1), a2) < aabb.Maximum.Z) triangles.Add(new Triangle(p0, p1, p2));
                        if (Math.Max(Math.Max(a1, a3), a2) > aabb.Minimum.Z && Math.Min(Math.Min(a1, a3), a2) < aabb.Maximum.Z) triangles.Add(new Triangle(p1, p3, p2));
                    }
                    else
                    {
                        if (Math.Max(Math.Max(a0, a1), a3) > aabb.Minimum.Z && Math.Min(Math.Min(a0, a1), a3) < aabb.Maximum.Z) triangles.Add(new Triangle(p0, p1, p3));
                        if (Math.Max(Math.Max(a0, a3), a2) > aabb.Minimum.Z && Math.Min(Math.Min(a0, a3), a2) < aabb.Maximum.Z) triangles.Add(new Triangle(p0, p3, p2));
                    }
                }
            }
            return triangles;
        }

        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit)
        {
            Load();

            hit = Hit.Zero;

            var hitFlag = (flags & CollisionFlags.Hit) != 0;
            
            var bottomPos = ray.Position;
            if (ray.Intersects(new Plane(Vector3.UnitZ, 0), out distance)) bottomPos += ray.Direction * distance;
            else return false;

            var topPos = ray.Position;
            if (ray.Intersects(new Plane(Vector3.UnitZ, -_Altitude * _Grid), out var d)) topPos += ray.Direction * d;

            var minx = Math.Min(topPos.X, bottomPos.X);
            var maxx = Math.Max(topPos.X, bottomPos.X);
            var diff = bottomPos - topPos;

            var wtov = (float)_VertexCell / _Grid;
            var vtow = 1f / wtov;

            var vw = _Width * _VertexCell - 1;
            var vh = _Height * _VertexCell - 1;
            var vx0 = MathUtil.Clamp((int)(minx * wtov), 0, vw);
            var vx1 = MathUtil.Clamp((int)(maxx * wtov), 0, vw);

            for (var vx = vx0; vx <= vx1; vx++)
            {
                var y0 = topPos.Y;
                var y1 = bottomPos.Y;

                if (diff.X != 0)
                {
                    var x0 = Math.Max(vx * vtow, minx);
                    var x1 = Math.Min((vx + 1) * vtow, maxx);
                    y0 = topPos.Y + diff.Y * (x0 - topPos.X) / diff.X;
                    y1 = topPos.Y + diff.Y * (x1 - topPos.X) / diff.X;
                }
                if (y0 > y1)
                {
                    var temp = y0;
                    y0 = y1;
                    y1 = temp;
                }
                var vy0 = MathUtil.Clamp((int)(y0 * wtov), 0, vh);
                var vy1 = MathUtil.Clamp((int)(y1 * wtov), 0, vh);

                for (var vy = vy0; vy <= vy1; vy++)
                {
                    var a0 = _altitudes[vx, vy] * _Grid;
                    var a1 = _altitudes[vx + 1, vy] * _Grid;
                    var a2 = _altitudes[vx, vy + 1] * _Grid;
                    var a3 = _altitudes[vx + 1, vy + 1] * _Grid;

                    var p0 = new Vector3(vx * vtow, vy * vtow, a0);
                    var p1 = new Vector3((vx + 1) * vtow, vy * vtow, a1);
                    var p2 = new Vector3(vx * vtow, (vy + 1) * vtow, a2);
                    var p3 = new Vector3((vx + 1) * vtow, (vy + 1) * vtow, a3);

                    Triangle tri0, tri1;

                    if (IsZQuad(a0, a1, a2, a3))
                    {
                        tri0 = new Triangle(p0, p1, p2);
                        tri1 = new Triangle(p1, p3, p2);
                    }
                    else
                    {
                        tri0 = new Triangle(p0, p1, p3);
                        tri1 = new Triangle(p0, p3, p2);
                    }
                    if (ray.Intersects(tri0, out d) && d < distance)
                    {
                        distance = d;
                        if (hitFlag) hit.Direction = tri0.Normal;
                    }
                    else if(ray.Intersects(tri1, out d) && d < distance)
                    {
                        distance = d;
                        if (hitFlag) hit.Direction = tri1.Normal;
                    }
                }
            }

            if (hitFlag) hit.Position = ray.Position + ray.Direction * distance;
            
            return true;
        }
        /*
        public bool Intersects(in Sphere sphere, CollisionFlags flags, out Ray hitRay)
        {
            Load();

            hitRay = Ray.Empty;
            flags &= CollisionFlags.Hit;

            AlignedBox.FromSphere(sphere, out var aabb);

            foreach (var tri in GetTriangles(aabb))
            {
                if (sphere.Intersects(tri, flags, out hitRay) != CollisionType.Front) return true;
            }
            return false;
        }

        public bool Intersects(in Capsule capsule, CollisionFlags flags, out Vector3 near, out Ray hitRay)
        {
            Load();

            near = capsule.Position0;
            hitRay = Ray.Empty;
            flags &= CollisionFlags.Hit;
            AlignedBox.FromCapsule(capsule, out var aabb);

            foreach (var tri in GetTriangles(aabb))
            {
                if (capsule.Intersects(tri, flags, out near, out hitRay) != CollisionType.Front) return true;
            }
            return false;
        }

        public bool Intersects(in AlignedBox box, CollisionFlags flags, out Ray hitRay)
        {
            Load();

            hitRay = Ray.Empty;
            flags &= CollisionFlags.Hit;

            foreach (var tri in GetTriangles(box))
            {
                if (box.Intersects(tri, flags, out hitRay) != CollisionType.Front) return true;
            }
            return false;
        }

        public bool Intersects(in OrientedBox box, CollisionFlags flags, out Ray hitRay)
        {
            Load();

            hitRay = Ray.Empty;
            flags &= CollisionFlags.Hit;
            var aabb = (AlignedBox)box;

            foreach (var tri in GetTriangles(aabb))
            {
                if (box.Intersects(tri, flags, out hitRay) != CollisionType.Front) return true;
            }
            return false;
        }

        public bool Intersects(Shape shape, CollisionFlags flags, out Ray hitRay)
        {
            Load();

            hitRay = Ray.Empty;
            flags &= CollisionFlags.Hit;
            
            AlignedBox.FromPoints(shape.Vertices, out var aabb);
            foreach (var tri in GetTriangles(aabb))
            {
                if (shape.Intersects(tri, flags, out hitRay) != CollisionType.Front) return true;
            }
            return false;
        }
        */
        public bool ConvertToMapSpace(in Ray ray, out Vector2 pos)
        {
            if (Intersects(ray, CollisionFlags.None, out var distance, out _))
            {
                pos = (ray.Position + ray.Direction * distance).ToVector2() / _Grid;
                return true;
            }
            pos = Vector2.Zero;
            return false;
        }

        public Vector3 ConvertToWorldSpace(in Vector2 pos)
        {
            Load();

            return new Vector3(pos.X, pos.Y, GetAltitude(pos)) * _Grid;
        }
    }
}
