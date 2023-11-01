using System.Numerics;

namespace CDK.Drawing
{
    public struct Hit
    {
        public Vector3 Position;
        public Vector3 Direction;
        public float Distance;

        public static Hit Zero = new Hit();
    }
}
