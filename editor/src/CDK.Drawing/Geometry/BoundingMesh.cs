using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public class BoundingMesh
    {
        private List<Vector3> _Vertices;
        private List<BoundingMeshFace> _Faces;
        public IEnumerable<Vector3> Vertices => _Vertices;
        public IEnumerable<BoundingMeshFace> Faces => _Faces;

        public Vector3 Center
        {
            get
            {
                var center = Vector3.Zero;
                if (_Vertices.Count > 0)
                {
                    foreach (var p in _Vertices) center += p;
                    center /= _Vertices.Count;
                }
                return center;
            }
        }

        public float Radius
        {
            get
            {
                var center = Center;
                var radius = (float)Math.Sqrt(_Vertices.Max(p => Vector3.DistanceSquared(center, p)));
                return radius;
            }
        }

        public BoundingMesh() : this(8, 6)      //default is box
        {
            
        }

        public BoundingMesh(int vertexCapacity, int indexCapacity)
        {
            _Vertices = new List<Vector3>(vertexCapacity);
            _Faces = new List<BoundingMeshFace>(indexCapacity);
        }

        public BoundingMesh(BoundingMesh other)
        {
            _Vertices = new List<Vector3>(other._Vertices);
            _Faces = new List<BoundingMeshFace>(other._Faces.Count);
            foreach(var face in other._Faces) _Faces.Add(new BoundingMeshFace(face));
        
        }

        //TODO:ADD CHECK CONVEX HULL (IsValid)

        public int VertexCount => _Vertices.Count;
        public Vector3 GetVertex(int i) => _Vertices[i];
        public void GetVertex(int i, out Vector3 vertex) => vertex = _Vertices[i];
        public void AddVertex(in Vector3 vertex)
        {
            foreach(var face in _Faces) 
            {
                var t = Vector3.Dot(vertex, face.Normal);
                if (t < face.Min) face.Min = t;
                if (t > face.Max) face.Max = t;
            }
            _Vertices.Add(vertex);
        }

        public bool AddFace(params int[] indices)
        {
            var normal = Vector3.Cross(_Vertices[indices[1]] - _Vertices[indices[0]], _Vertices[indices[2]] - _Vertices[indices[0]]);

            for (var i = 1; i < indices.Length - 2; i++)
            {
                var n = Vector3.Cross(_Vertices[indices[i + 1]] - _Vertices[indices[i]], _Vertices[indices[i + 2]] - _Vertices[indices[i]]);

                if (!VectorUtil.NearEqual(normal, n)) return false;
            }

            var min = float.MaxValue;
            var max = float.MinValue;
            foreach(var vertex in _Vertices)
            {
                var t = Vector3.Dot(vertex, normal);
                if (t < min) min = t;
                if (t > max) max = t;
            }
            _Faces.Add(new BoundingMeshFace(normal, min, max, indices));

            return true;
        }

        public CollisionResult Intersects(in Vector3 point, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsPoint(this, point, flags, out hit);
        public CollisionResult Intersects(in Segment seg, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.MeshIntersectsSegment(this, seg, flags, out near, out hit);
        public CollisionResult Intersects(in Triangle tri, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsTriangle(this, tri, flags, out hit);
        public bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit) => Collision.RayIntersectsMesh(ray, this, flags, out distance, out hit);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsMesh(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsSphere(this, sphere, flags, out hit);
        public CollisionResult Intersects(in BoundingCapsule capsule, CollisionFlags flags, out Vector3 near, out Hit hit) => Collision.MeshIntersectsCapsule(this, capsule, flags, out near, out hit);
        public CollisionResult Intersects(in ABoundingBox box, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsABox(this, box, flags, out hit);
        public CollisionResult Intersects(in OBoundingBox box, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsOBox(this, box, flags, out hit);
        public CollisionResult Intersects(BoundingMesh mesh, CollisionFlags flags, out Hit hit) => Collision.MeshIntersectsMesh(this, mesh, flags, out hit);

        public float GetZ(in Vector3 point) => Collision.MeshGetZ(this, point);

        public void Transform(in Quaternion rotation)
        {
            for (var i = 0; i < _Vertices.Count; i++) _Vertices[i] = Vector3.Transform(_Vertices[i], rotation);

            foreach (var face in _Faces)
            {
                face.Normal = Vector3.Transform(face.Normal, rotation);
                face.Min = float.MaxValue;
                face.Max = float.MinValue;
                foreach (var p in _Vertices)
                {
                    var t = Vector3.Dot(p, face.Normal);
                    if (t < face.Min) face.Min = t;
                    if (t > face.Max) face.Max = t;
                }
            }
        }

        public void Transform(in Matrix4x4 transform)
        {
            for (var i = 0; i < _Vertices.Count; i++) _Vertices[i] = Vector3.Transform(_Vertices[i], transform);

            foreach(var face in _Faces)
            {
                face.Normal = Vector3.Normalize(Vector3.TransformNormal(face.Normal, transform));
                face.Min = float.MaxValue;
                face.Max = float.MinValue;
                foreach (var p in _Vertices)
                {
                    var t = Vector3.Dot(p, face.Normal);
                    if (t < face.Min) face.Min = t;
                    if (t > face.Max) face.Max = t;
                }
            }
        }

        public static BoundingMesh Transform(BoundingMesh mesh, in Quaternion rotation)
        {
            var result = new BoundingMesh(mesh);
            result.Transform(rotation);
            return result;
        }

        public static BoundingMesh Transform(BoundingMesh mesh, in Matrix4x4 transform)
        {
            var result = new BoundingMesh(mesh);
            result.Transform(transform);
            return result;
        }

        public override string ToString() => $"Vertices:{_Vertices.Count} Faces:{_Faces.Count}";
    }
}
