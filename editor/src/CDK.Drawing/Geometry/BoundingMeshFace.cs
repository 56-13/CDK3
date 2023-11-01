using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public class BoundingMeshFace
    {
        public Vector3 Normal { internal set; get; }
        public float Min { internal set; get; }
        public float Max { internal set; get; }

        private int[] _Indices;
        public IEnumerable<int> Indices => _Indices;
        public int IndexCount => _Indices.Length;
        public int GetIndex(int i) => _Indices[i];

        internal BoundingMeshFace(in Vector3 normal, float min, float max, params int[] indices)
        {
            Normal = normal;
            Min = min;
            Max = max;
            _Indices = indices;
        }

        internal BoundingMeshFace(BoundingMeshFace other)
        {
            Normal = other.Normal;
            Min = other.Min;
            Max = other.Max;
            _Indices = other._Indices;
        }
    }
}
