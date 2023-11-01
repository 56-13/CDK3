using System;
using System.Numerics;

namespace CDK.Drawing.Meshing
{
    internal static class Conversions
    {
        public static Vector2 ToVector2(this Assimp.Vector2D v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToVector3(this Assimp.Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Half2 ToHalf2(this Assimp.Vector2D v)
        {
            return new Half2((Half)v.X, (Half)v.Y);
        }

        public static Half3 ToHalf3(this Assimp.Vector3D v)
        {
            return new Half3((Half)v.X, (Half)v.Y, (Half)v.Z);
        }

        public static Color3 ToColor3(this Assimp.Color3D v)
        {
            return new Color3(v.R, v.G, v.B);
        }

        public static Color4 ToColor4(this Assimp.Color4D v)
        {
            return new Color4(v.R, v.G, v.B, v.A);
        }

        public static Half3 ToHalf3(this Assimp.Color3D v)
        {
            return new Half3((Half)v.R, (Half)v.G, (Half)v.B);
        }

        public static Half4 ToHalf4(this Assimp.Color4D v)
        {
            return new Half4((Half)v.R, (Half)v.G, (Half)v.B, (Half)v.A);
        }

        public static Quaternion ToQuaternion(this Assimp.Quaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static Matrix4x4 ToMatrix(this Assimp.Matrix4x4 m)
        {
            return new Matrix4x4(
                m.A1, m.B1, m.C1, m.D1,
                m.A2, m.B2, m.C2, m.D2,
                m.A3, m.B3, m.C3, m.D3,
                m.A4, m.B4, m.C4, m.D4);
        }
    }
}
