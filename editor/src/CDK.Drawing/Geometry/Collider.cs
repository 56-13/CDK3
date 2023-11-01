using System;
using System.Collections.Generic;
using System.Numerics;

namespace CDK.Drawing
{
    public class Collider
    {
        private List<object> _shapes;

        public Collider()
        {
            _shapes = new List<object>();
        }

        public Collider(int capacity)
        {
            _shapes = new List<object>(capacity);
        }

        public Collider(Collider other)
        {
            _shapes = new List<object>(other._shapes);
        }

        public void Add(in OBoundingBox box) => _shapes.Add(box);
        public void Add(in BoundingSphere sphere) => _shapes.Add(sphere);
        public void Add(in BoundingCapsule capsule) => _shapes.Add(capsule);
        public void Add(BoundingMesh mesh) => _shapes.Add(mesh);
        public void Transform(in Matrix4x4 transform)
        {
            for (int i = 0; i < _shapes.Count; i++)
            {
                var shape = _shapes[i];
                if (shape is OBoundingBox box) _shapes[i] = OBoundingBox.Transform(box, transform);
                else if (shape is BoundingSphere sphere) _shapes[i] = BoundingSphere.Transform(sphere, transform);
                else if (shape is BoundingCapsule capsule) _shapes[i] = BoundingCapsule.Transform(capsule, transform);
                else if (shape is BoundingMesh mesh) _shapes[i] = BoundingMesh.Transform(mesh, transform);
                else throw new InvalidOperationException();
            }
        }

        public static Collider Transform(Collider origin, in Matrix4x4 transform)
        {
            var result = new Collider(origin);
            result.Transform(transform);
            return result;
        }

        public void GetZ(in Vector3 pos, ref float z)
        {
            for (int i = 0; i < _shapes.Count; i++)
            {
                var shape = _shapes[i];
                if (shape is OBoundingBox box) z = Math.Max(z, box.GetZ(pos));
                else if (shape is BoundingSphere sphere) z = Math.Max(z, sphere.GetZ(pos));
                else if (shape is BoundingCapsule capsule) z = Math.Max(z, capsule.GetZ(pos));
                else if (shape is BoundingMesh mesh) z = Math.Max(z, mesh.GetZ(pos));
                else throw new InvalidOperationException();
            }
        }

        public bool Intersects(in Ray ray, CollisionFlags flags, ref float distance, ref Hit hit)
        {
            var flag = false;

            bool c;
            float d;
            Hit h;

            foreach (var shape in _shapes) {
                if (shape is OBoundingBox box) c = box.Intersects(ray, flags, out d, out h);
                else if (shape is BoundingSphere sphere) c = sphere.Intersects(ray, flags, out d, out h);
                else if (shape is BoundingCapsule capsule) c = capsule.Intersects(ray, flags, out d, out _, out h);
                else if (shape is BoundingMesh mesh) c = mesh.Intersects(ray, flags, out d, out h);
                else throw new InvalidOperationException();

                if (c && d < distance)
                {
                    distance = d;
                    hit = h;
                    flag = true;
                }
            }
            return flag;
        }

        public CollisionResult Intersects(Collider other, CollisionFlags flags, ref float distance, ref Hit hit)
        {
            var result = CollisionResult.Front;
            flags |= CollisionFlags.Hit;

            CollisionResult r;
            Hit h;

            foreach (object s0 in _shapes) 
            {
                foreach (object s1 in other._shapes) 
                {
                    if (s0 is OBoundingBox box0)
                    {
                        if (s1 is OBoundingBox box1) r = box0.Intersects(box1, flags, out h);
                        else if (s1 is BoundingSphere sphere1) r = box0.Intersects(sphere1, flags, out h);
                        else if (s1 is BoundingCapsule capsule1) r = box0.Intersects(capsule1, flags, out _, out h);
                        else if (s1 is BoundingMesh mesh1) r = box0.Intersects(mesh1, flags, out h);
                        else throw new InvalidOperationException();
                    }
                    else if (s0 is BoundingSphere sphere0)
                    {
                        if (s1 is OBoundingBox box1) r = sphere0.Intersects(box1, flags, out h);
                        else if (s1 is BoundingSphere sphere1) r = sphere0.Intersects(sphere1, flags, out h);
                        else if (s1 is BoundingCapsule capsule1) r = sphere0.Intersects(capsule1, flags, out _, out h);
                        else if (s1 is BoundingMesh mesh1) r = sphere0.Intersects(mesh1, flags, out h);
                        else throw new InvalidOperationException();
                    }
                    else if (s0 is BoundingCapsule capsule0)
                    {
                        if (s1 is OBoundingBox box1) r = capsule0.Intersects(box1, flags, out _, out h);
                        else if (s1 is BoundingSphere sphere1) r = capsule0.Intersects(sphere1, flags, out _, out h);
                        else if (s1 is BoundingCapsule capsule1) r = capsule0.Intersects(capsule1, flags, out _, out _, out h);
                        else if (s1 is BoundingMesh mesh1) r = capsule0.Intersects(mesh1, flags, out _, out h);
                        else throw new InvalidOperationException();
                    }
                    else if (s0 is BoundingMesh mesh0)
                    {
                        if (s1 is OBoundingBox box1) r = mesh0.Intersects(box1, flags, out h);
                        else if (s1 is BoundingSphere sphere1) r = mesh0.Intersects(sphere1, flags, out h);
                        else if (s1 is BoundingCapsule capsule1) r = mesh0.Intersects(capsule1, flags, out _, out h);
                        else if (s1 is BoundingMesh mesh1) r = mesh0.Intersects(mesh1, flags, out h);
                        else throw new InvalidOperationException();
                    }
                    else throw new InvalidOperationException();

                    if (r != CollisionResult.Front && h.Distance < distance)
                    {
                        result = r;
                        distance = h.Distance;
                        hit = h;
                    }
                }
            }
            return result;
        }
    }
}
