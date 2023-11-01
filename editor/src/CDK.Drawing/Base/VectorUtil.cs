using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static class VectorUtil
    {
        public static float GetComponent(this Vector2 v, int i)
        {
            switch (i)
            {
                case 0:
                    return v.X;
                case 1:
                    return v.Y;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static float GetComponent(this Vector3 v, int i)
        {
            switch (i)
            {
                case 0:
                    return v.X;
                case 1:
                    return v.Y;
                case 2:
                    return v.Z;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static float GetComponent(this Vector4 v, int i)
        {
            switch (i)
            {
                case 0:
                    return v.X;
                case 1:
                    return v.Y;
                case 2:
                    return v.Z;
                case 3:
                    return v.W;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static void SetComponent(this Vector2 v, int i, float c)
        {
            switch (i)
            {
                case 0:
                    v.X = c;
                    return;
                case 1:
                    v.Y = c;
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static void SetComponent(this Vector3 v, int i, float c)
        {
            switch (i)
            {
                case 0:
                    v.X = c;
                    return;
                case 1:
                    v.Y = c;
                    return;
                case 2:
                    v.Z = c;
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static void SetComponent(this Vector4 v, int i, float c)
        {
            switch (i)
            {
                case 0:
                    v.X = c;
                    return;
                case 1:
                    v.Y = c;
                    return;
                case 2:
                    v.Z = c;
                    return;
                case 3:
                    v.W = c;
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static bool NearZero(this Vector2 v)
        {
            return MathUtil.NearZero(v.X) && MathUtil.NearZero(v.Y);
        }

        public static bool NearZero(this Vector3 v)
        {
            return MathUtil.NearZero(v.X) && MathUtil.NearZero(v.Y) && MathUtil.NearZero(v.Z);
        }

        public static bool NearZero(this Vector4 v)
        {
            return MathUtil.NearZero(v.X) && MathUtil.NearZero(v.Y) && MathUtil.NearZero(v.Z) && MathUtil.NearZero(v.W);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0);
        }

        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }


        public static bool NearEqual(in Vector2 v0, in Vector2 v1)
        {
            return MathUtil.NearEqual(v0.X, v1.X) && MathUtil.NearEqual(v0.Y, v1.Y);
        }

        public static bool NearEqual(in Vector3 v0, in Vector3 v1)
        {
            return MathUtil.NearEqual(v0.X, v1.X) && MathUtil.NearEqual(v0.Y, v1.Y) && MathUtil.NearEqual(v0.Z, v1.Z);
        }

        public static bool NearEqual(in Vector4 v0, in Vector4 v1)
        {
            return MathUtil.NearEqual(v0.X, v1.X) && MathUtil.NearEqual(v0.Y, v1.Y) && MathUtil.NearEqual(v0.Z, v1.Z) && MathUtil.NearEqual(v0.W, v1.W);
        }

        public static Vector3 TransformCoordinate(in Vector3 v, in Matrix4x4 m)
        {
            var vp = Vector4.Transform(v, m);

            return new Vector3(vp.X / vp.W, vp.Y / vp.W, vp.Z / vp.W);
        }
    }
}
