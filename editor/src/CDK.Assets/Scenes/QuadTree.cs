using System;
using System.Numerics;
using System.Collections.Generic;

using CDK.Drawing;

namespace CDK.Assets.Scenes
{
    internal class QuadTree
    {
        private QuadTreeNode _top;
        public ABoundingBox Space => _top.Space;
        public int Grid { private set; get; }
        public int Depth { private set; get; }

        public QuadTree(in ABoundingBox space, int grid)
        {
            _top = new QuadTreeNode(space);

            Grid = grid;

            Depth = GetDepthFromGrid(space, grid);
        }

        private static int GetDepthFromGrid(in ABoundingBox space, int grid)
        {
            if (grid <= 0) return 0;
            var d = space.Maximum - space.Minimum;
            var dd = Math.Min(Math.Min(d.X, d.Y), d.Z);
            var gd = (int)(dd / grid);

            var depth = 0;
            while (gd > 1) {
                gd >>= 1;
                depth++;
            }
            return depth;
        }

        public void Resize(in ABoundingBox space, int grid)
        {
            int depth;
            if (_top.Space != space) depth = GetDepthFromGrid(space, grid);
            else if (Grid != grid)
            {
                depth = GetDepthFromGrid(space, grid);
                if (Depth == depth)
                {
                    Grid = grid;
                    return;
                }
            }
            else return;

            var objs = SelectAll();
            _top = new QuadTreeNode(space);
            Grid = grid;
            Depth = depth;
            lock (this)
            {
                foreach (var obj in objs)
                {
                    if (obj.GetAABB(out var aabb)) _top.Locate(obj, aabb, aabb.GetCorners(), depth);
                }
            }
        }

        public void Locate(SceneObject obj)
        {
            if (obj.GetAABB(out var aabb))
            {
                lock (this)
                {
                    _top.Locate(obj, aabb, aabb.GetCorners(), Depth);
                }
            }
        }

        public void Unlocate(SceneObject obj)
        {
            if (obj.GetAABB(out var aabb))
            {
                lock (this)
                {
                    _top.Unlocate(obj, aabb, aabb.GetCorners(), Depth);
                }
            }
        }

        public void Relocate(SceneObject obj, in ABoundingBox naabb)
        {
            if (obj.GetAABB(out var paabb))
            {
                var center = _top.Space.Center;
                var r = 1 << Depth;

                if ((int)((paabb.Minimum.X - center.X) / r) != (int)((naabb.Minimum.X - center.X) / r) ||
                    (int)((paabb.Minimum.Y - center.Y) / r) != (int)((naabb.Minimum.Y - center.Y) / r) ||
                    (int)((paabb.Minimum.Z - center.Z) / r) != (int)((naabb.Minimum.Z - center.Z) / r) ||
                    (int)((paabb.Maximum.X - center.X) / r) != (int)((naabb.Maximum.X - center.X) / r) ||
                    (int)((paabb.Maximum.Y - center.Y) / r) != (int)((naabb.Maximum.Y - center.Y) / r) ||
                    (int)((paabb.Maximum.Z - center.Z) / r) != (int)((naabb.Maximum.Z - center.Z) / r))
                {
                    lock (this)
                    {
                        _top.Unlocate(obj, paabb, paabb.GetCorners(), Depth);
                        _top.Locate(obj, naabb, naabb.GetCorners(), Depth);
                    }
                }
            }
        }

        public HashSet<SceneObject> SelectAll()
        {
            var objs = new HashSet<SceneObject>();
            SelectAll(objs);
            return objs;
        }

        public HashSet<SceneObject> Select(in Vector3 pos)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, pos);
            return objs;
        }

        public HashSet<SceneObject> Select(in BoundingFrustum frustum)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, frustum);
            return objs;
        }

        public HashSet<SceneObject> Select(in Ray ray)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, ray);
            return objs;
        }

        public HashSet<SceneObject> Select(in BoundingSphere sphere)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, sphere);
            return objs;
        }

        public HashSet<SceneObject> Select(in BoundingCapsule capsule)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, capsule);
            return objs;
        }

        public HashSet<SceneObject> Select(in ABoundingBox box)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, box);
            return objs;
        }

        public HashSet<SceneObject> Select(in OBoundingBox box)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, box);
            return objs;
        }

        public HashSet<SceneObject> Select(BoundingMesh shape)
        {
            var objs = new HashSet<SceneObject>();
            Select(objs, shape);
            return objs;
        }

        public void SelectAll(HashSet<SceneObject> objs)
        {
            lock (this)
            {
                _top.SelectAll(objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, Vector3 pos)
        {
            CollisionResult func(in ABoundingBox aabb) 
            { 
                if (aabb.Minimum.X <= pos.X && pos.X <= aabb.Maximum.X &&
                    aabb.Minimum.Y <= pos.Y && pos.Y <= aabb.Maximum.Y &&
                    aabb.Minimum.Z <= pos.Z)
                {
                    return CollisionResult.Intersects;
                }
                return CollisionResult.Front;
            };
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, BoundingFrustum frustum)
        {
            CollisionResult func(in ABoundingBox aabb) => frustum.Intersects(aabb);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, Ray ray)
        {
            CollisionResult func(in ABoundingBox aabb) => ray.Intersects(aabb, CollisionFlags.None, out _, out _) ? CollisionResult.Intersects : CollisionResult.Front;
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, BoundingSphere sphere)
        {
            CollisionResult func(in ABoundingBox aabb) => sphere.Intersects(aabb, CollisionFlags.Back, out _);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, BoundingCapsule capsule)
        {
            CollisionResult func(in ABoundingBox aabb) => capsule.Intersects(aabb, CollisionFlags.Back, out _, out _);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, ABoundingBox box)
        {
            CollisionResult func(in ABoundingBox aabb) => box.Intersects(aabb, CollisionFlags.Back, out _);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, OBoundingBox box)
        {
            CollisionResult func(in ABoundingBox aabb) => box.Intersects(aabb, CollisionFlags.Back, out _);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }

        public void Select(HashSet<SceneObject> objs, BoundingMesh mesh)
        {
            CollisionResult func(in ABoundingBox aabb) => mesh.Intersects(aabb, CollisionFlags.Back, out _);
            lock (this)
            {
                _top.Select(func, objs);
            }
        }
    }
}
