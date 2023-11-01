using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct BoundingFrustum : IEquatable<BoundingFrustum>
    {
        private Matrix4x4 pMatrix;
        private Plane pNear;
        private Plane pFar;
        private Plane pLeft;
        private Plane pRight;
        private Plane pTop;
        private Plane pBottom;

        public Matrix4x4 Matrix
        {
            set
            {
                pMatrix = value;
                GetPlanesFromMatrix(pMatrix, out pNear, out pFar, out pLeft, out pRight, out pTop, out pBottom);
            }
            get => pMatrix;
        }
        public Plane Near => pNear;
        public Plane Far => pFar;
        public Plane Left => pLeft;
        public Plane Right => pRight;
        public Plane Top => pTop;
        public Plane Bottom => pBottom;

        public BoundingFrustum(in Matrix4x4 matrix)
        {
            pMatrix = matrix;
            GetPlanesFromMatrix(pMatrix, out pNear, out pFar, out pLeft, out pRight, out pTop, out pBottom);
        }

        public Plane GetPlane(int index)
        {
            switch (index)
            {
                case 0: return pLeft;
                case 1: return pRight;
                case 2: return pTop;
                case 3: return pBottom;
                case 4: return pNear;
                case 5: return pFar;
            }
            throw new ArgumentOutOfRangeException();
        }

        private static void GetPlanesFromMatrix(in Matrix4x4 matrix, out Plane near, out Plane far, out Plane left, out Plane right, out Plane top, out Plane bottom)
        {
            left.Normal.X = matrix.M14 + matrix.M11;
            left.Normal.Y = matrix.M24 + matrix.M21;
            left.Normal.Z = matrix.M34 + matrix.M31;
            left.D = matrix.M44 + matrix.M41;
            left = Plane.Normalize(left);

            right.Normal.X = matrix.M14 - matrix.M11;
            right.Normal.Y = matrix.M24 - matrix.M21;
            right.Normal.Z = matrix.M34 - matrix.M31;
            right.D = matrix.M44 - matrix.M41;
            right = Plane.Normalize(right);

            top.Normal.X = matrix.M14 - matrix.M12;
            top.Normal.Y = matrix.M24 - matrix.M22;
            top.Normal.Z = matrix.M34 - matrix.M32;
            top.D = matrix.M44 - matrix.M42;
            top = Plane.Normalize(top);

            bottom.Normal.X = matrix.M14 + matrix.M12;
            bottom.Normal.Y = matrix.M24 + matrix.M22;
            bottom.Normal.Z = matrix.M34 + matrix.M32;
            bottom.D = matrix.M44 + matrix.M42;
            bottom = Plane.Normalize(bottom);

            near.Normal.X = matrix.M13;
            near.Normal.Y = matrix.M23;
            near.Normal.Z = matrix.M33;
            near.D = matrix.M43;

            far.Normal.X = matrix.M14 - matrix.M13;
            far.Normal.Y = matrix.M24 - matrix.M23;
            far.Normal.Z = matrix.M34 - matrix.M33;
            far.D = matrix.M44 - matrix.M43;
            far = Plane.Normalize(far);
        }

        private static void Get3PlanesInterPoint(in Plane p1, in Plane p2, in Plane p3, out Vector3 v)
        {
            v = -p1.D * Vector3.Cross(p2.Normal, p3.Normal) / Vector3.Dot(p1.Normal, Vector3.Cross(p2.Normal, p3.Normal))
                -p2.D * Vector3.Cross(p3.Normal, p1.Normal) / Vector3.Dot(p2.Normal, Vector3.Cross(p3.Normal, p1.Normal))
                -p3.D * Vector3.Cross(p1.Normal, p2.Normal) / Vector3.Dot(p3.Normal, Vector3.Cross(p1.Normal, p2.Normal));
        }

        public Vector3[] GetCorners()
        {
            var corners = new Vector3[8];
            GetCorners(corners);
            return corners;
        }

        public void GetCorners(Vector3[] corners)
        {
            Get3PlanesInterPoint(pNear, pBottom, pRight, out corners[0]); 
            Get3PlanesInterPoint(pNear, pTop, pRight, out corners[1]); 
            Get3PlanesInterPoint(pNear, pTop, pLeft, out corners[2]); 
            Get3PlanesInterPoint(pNear, pBottom, pLeft, out corners[3]); 
            Get3PlanesInterPoint(pFar, pBottom, pRight, out corners[4]); 
            Get3PlanesInterPoint(pFar, pTop, pRight, out corners[5]); 
            Get3PlanesInterPoint(pFar, pTop, pLeft, out corners[6]); 
            Get3PlanesInterPoint(pFar, pBottom, pLeft, out corners[7]);
        }

        public CollisionResult Intersects(in Vector3 point) => Collision.FrustumIntersectsPoint(this, point);
        public CollisionResult Intersects(in Segment seg) => Collision.FrustumIntersectsSegment(this, seg);
        public CollisionResult Intersects(in Triangle tri) => Collision.FrustumIntersectsTriangle(this, tri);
        public bool Intersects(in Ray ray, out float distance0, out float distance1) => Collision.RayIntersectsFrustum(ray, this, out distance0, out distance1);
        public bool Intersects(in Plane plane) => Collision.PlaneIntersectsFrustum(plane, this) == CollisionResult.Intersects;
        public CollisionResult Intersects(in BoundingSphere sphere) => Collision.FrustumIntersectsSphere(this, sphere);
        public CollisionResult Intersects(in BoundingCapsule capsule) => Collision.FrustumIntersectsCapsule(this, capsule);
        public CollisionResult Intersects(in ABoundingBox box) => Collision.FrustumIntersectsABox(this, box);
        public CollisionResult Intersects(in OBoundingBox box) => Collision.FrustumIntersectsOBox(this, box);
        public CollisionResult Intersects(BoundingMesh mesh) => Collision.FrustumIntersectsMesh(this, mesh);
        public CollisionResult Intersects(in BoundingFrustum frustum) => Collision.FrustumIntersectsFrustum(this, frustum);
        
        public bool IsOrthographic => VectorUtil.NearEqual(pLeft.Normal, -pRight.Normal) && VectorUtil.NearEqual(pTop.Normal, -pBottom.Normal);

        public static bool operator ==(in BoundingFrustum left, in BoundingFrustum right) => left.Equals(right);
        public static bool operator !=(in BoundingFrustum left, in BoundingFrustum right) => !left.Equals(right);

        public override int GetHashCode() => pMatrix.GetHashCode();
        public bool Equals(BoundingFrustum other) => pMatrix == other.pMatrix;
        public override bool Equals(object obj) => obj is BoundingFrustum other && Equals(other);
    }
}
