using System.Numerics;
using System.Collections.Generic;

using CDK.Drawing;

namespace CDK.Assets.Scenes
{
    internal class QuadTreeNode
    {
        public ABoundingBox Space { private set; get; }

        private LinkedList<SceneObject> _objects;
        private QuadTreeNode[] _nodes;

        public QuadTreeNode(in ABoundingBox space)
        {
            Space = space;
        }

        public void Locate(SceneObject obj, in ABoundingBox aabb, Vector3[] aabbCorners, int depth)
        {
            if (depth == 0)
            {
                if (_objects == null) _objects = new LinkedList<SceneObject>();
                _objects.AddLast(obj);
            }
            else
            {
                if (_nodes == null) _nodes = new QuadTreeNode[8];

                depth--;

                var center = Space.Center;
                var flags = 0;
                foreach (var corner in aabbCorners)
                {
                    var i = 0;
                    if (corner.X > center.X) i |= 1;
                    if (corner.Y > center.Y) i |= 2;
                    if (corner.Z > center.Z) i |= 4;
                    
                    var s = 1 << i;
                    if ((flags & s) == 0)
                    {
                        if (_nodes[i] == null)
                        {
                            var space = new ABoundingBox();
                            space.Minimum.X = (i & 1) == 0 ? Space.Minimum.X : center.X;
                            space.Maximum.X = (i & 1) == 0 ? center.X : Space.Maximum.X;
                            space.Minimum.Y = (i & 2) == 0 ? Space.Minimum.Y : center.Y;
                            space.Maximum.Y = (i & 2) == 0 ? center.Y : Space.Maximum.Y;
                            space.Minimum.Z = (i & 4) == 0 ? Space.Minimum.Z : center.Z;
                            space.Maximum.Z = (i & 4) == 0 ? center.Z : Space.Maximum.Z;
                            _nodes[i] = new QuadTreeNode(space);
                        }

                        _nodes[i].Locate(obj, aabb, aabbCorners, depth);

                        flags |= s;
                    }
                }
            }
        }

        public bool Unlocate(SceneObject obj, in ABoundingBox aabb, Vector3[] aabbCorners, int depth)
        {
            var center = Space.Center;

            if (depth == 0 || aabb.Intersects(center, CollisionFlags.None, out _) != CollisionResult.Front)
            {
                if (_objects != null && _objects.Remove(obj) && _objects.Count == 0) _objects = null;
                if (_nodes != null)
                {
                    depth--;

                    var clear = true;
                    for (var i = 0; i < 8; i++)
                    {
                        if (_nodes[i] != null)
                        {
                            if (_nodes[i].Unlocate(obj, aabb, aabbCorners, depth)) clear = false;
                            else _nodes[i] = null;
                        }
                    }
                    if (clear) _nodes = null;
                }
            }
            else if (_nodes != null)
            {
                depth--;

                var flags = 0;
                foreach (var corner in aabbCorners)
                {
                    var i = 0;
                    if (corner.X > center.X) i |= 1;
                    if (corner.Y > center.Y) i |= 2;
                    if (corner.Z > center.Z) i |= 4;

                    var s = 1 << i;
                    if ((flags & s) == 0)
                    {
                        if (_nodes[i] != null && !_nodes[i].Unlocate(obj, aabb, aabbCorners, depth)) _nodes[i] = null;

                        flags |= s;
                    }
                }
                foreach (var node in _nodes)
                {
                    if (node != null) return true;
                }
                _nodes = null;
            }
            return _objects != null || _nodes != null;
        }

        public void SelectAll(HashSet<SceneObject> objs)
        {
            if (_objects != null)
            {
                foreach (var obj in _objects) objs.Add(obj);
            }
            if (_nodes != null)
            {
                foreach (var node in _nodes) node?.SelectAll(objs);
            }
        }

        public delegate CollisionResult BoundingFunc(in ABoundingBox aabb);

        public void Select(BoundingFunc func, HashSet<SceneObject> objs)
        {
            switch (func(Space))
            {
                case CollisionResult.Back:
                    SelectAll(objs);
                    break;
                case CollisionResult.Intersects:
                    if (_objects != null)
                    {
                        foreach (var obj in _objects) objs.Add(obj);
                    }
                    if (_nodes != null)
                    {
                        foreach (var node in _nodes) node?.Select(func, objs);
                    }
                    break;
            }
        }
    }
}
