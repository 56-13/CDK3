using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static class MatrixUtil
    {
        public static Matrix3x3 ToMatrix3x3(this Matrix4x4 m)
        {
            return new Matrix3x3(
                m.M11, m.M12, m.M13,
                m.M21, m.M22, m.M23,
                m.M31, m.M32, m.M33
            );
        }

        public static Vector3 Up(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = m.M21;
            vector3.Y = m.M22;
            vector3.Z = m.M23;
            return vector3;
        }

        public static Vector3 Down(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = -m.M21;
            vector3.Y = -m.M22;
            vector3.Z = -m.M23;
            return vector3;
        }

        public static Vector3 Right(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = m.M11;
            vector3.Y = m.M12;
            vector3.Z = m.M13;
            return vector3;
        }

        public static Vector3 Left(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = -m.M11;
            vector3.Y = -m.M12;
            vector3.Z = -m.M13;
            return vector3;
        }

        public static Vector3 Forward(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = -m.M31;
            vector3.Y = -m.M32;
            vector3.Z = -m.M33;
            return vector3;
        }

        public static Vector3 Backward(this Matrix4x4 m)
        {
            Vector3 vector3;
            vector3.X = m.M31;
            vector3.Y = m.M32;
            vector3.Z = m.M33;
            return vector3;
        }

        public static void CreateLookAtLH(in Vector3 eye, in Vector3 target, in Vector3 up, out Matrix4x4 result)
        {
            var zaxis = Vector3.Normalize(target - eye);
            var xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            var yaxis = Vector3.Cross(zaxis, xaxis);

            result = Matrix4x4.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            result.M41 = -Vector3.Dot(xaxis, eye);
            result.M42 = -Vector3.Dot(yaxis, eye);
            result.M43 = -Vector3.Dot(zaxis, eye);
        }

        public static Matrix4x4 CreateLookAtLH(in Vector3 eye, in Vector3 target, in Vector3 up)
        {
            CreateLookAtLH(eye, target, up, out var result);
            return result;
        }

        public static void CreateLookAtRH(in Vector3 eye, in Vector3 target, in Vector3 up, out Matrix4x4 result)
        {
            var zaxis = Vector3.Normalize(eye - target);
            var xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            var yaxis = Vector3.Cross(zaxis, xaxis);

            result = Matrix4x4.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            result.M41 = -Vector3.Dot(xaxis, eye);
            result.M42 = -Vector3.Dot(yaxis, eye);
            result.M43 = -Vector3.Dot(zaxis, eye);
        }

        public static Matrix4x4 CreateLookAtRH(in Vector3 eye, in Vector3 target, in Vector3 up)
        {
            CreateLookAtRH(eye, target, up, out var result);
            return result;
        }

        public static void CreateOrthoLH(float width, float height, float znear, float zfar, out Matrix4x4 result)
        {
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;

            CreateOrthoOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreateOrthoLH(float width, float height, float znear, float zfar)
        {
            CreateOrthoLH(width, height, znear, zfar, out var result);
            return result;
        }

        public static void CreateOrthoRH(float width, float height, float znear, float zfar, out Matrix4x4 result)
        {
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;

            CreateOrthoOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreateOrthoRH(float width, float height, float znear, float zfar)
        {
            CreateOrthoRH(width, height, znear, zfar, out var result);
            return result;
        }

        public static void CreateOrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix4x4 result)
        {
            result = Matrix4x4.Identity;
            result.M11 = 2 / (right - left);
            result.M22 = 2 / (top - bottom);
            result.M33 = 2 / (zfar - znear);
            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = -(zfar + znear) / (zfar - znear);
        }

        public static Matrix4x4 CreateOrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            CreateOrthoOffCenterLH(left, right, bottom, top, znear, zfar, out var result);
            return result;
        }

        public static void CreateOrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix4x4 result)
        {
            CreateOrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            result.M33 *= -1;
        }

        public static Matrix4x4 CreateOrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            CreateOrthoOffCenterRH(left, right, bottom, top, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveLH(float width, float height, float znear, float zfar, out Matrix4x4 result)
        {
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;

            CreatePerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreatePerspectiveLH(float width, float height, float znear, float zfar)
        {
            CreatePerspectiveLH(width, height, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveRH(float width, float height, float znear, float zfar, out Matrix4x4 result)
        {
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;

            CreatePerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreatePerspectiveRH(float width, float height, float znear, float zfar)
        {
            CreatePerspectiveRH(width, height, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveFovLH(float fov, float aspect, float znear, float zfar, out Matrix4x4 result)
        {
            var yScale = (float)(1.0 / Math.Tan(fov * 0.5f));
            var xScale = yScale / aspect;

            var halfWidth = znear / xScale;
            var halfHeight = znear / yScale;

            CreatePerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreatePerspectiveFovLH(float fov, float aspect, float znear, float zfar)
        {
            CreatePerspectiveFovLH(fov, aspect, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveFovRH(float fov, float aspect, float znear, float zfar, out Matrix4x4 result)
        {
            var yScale = (float)(1.0 / Math.Tan(fov * 0.5f));
            var xScale = yScale / aspect;

            var halfWidth = znear / xScale;
            var halfHeight = znear / yScale;

            CreatePerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        public static Matrix4x4 CreatePerspectiveFovRH(float fov, float aspect, float znear, float zfar)
        {
            CreatePerspectiveFovRH(fov, aspect, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix4x4 result)
        {
            result = new Matrix4x4
            {
                M11 = 2 * znear / (right - left),
                M22 = 2 * znear / (top - bottom),
                M31 = (left + right) / (left - right),
                M32 = (top + bottom) / (bottom - top),
                M33 = (zfar + znear) / (zfar - znear),
                M34 = 1,
                M43 = -2 * zfar * znear / (zfar - znear)
            };
        }

        public static Matrix4x4 CreatePerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            CreatePerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out var result);
            return result;
        }

        public static void CreatePerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix4x4 result)
        {
            CreatePerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            result.M31 *= -1;
            result.M32 *= -1;
            result.M33 *= -1;
            result.M34 *= -1;
        }

        public static Matrix4x4 CreatePerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            CreatePerspectiveOffCenterRH(left, right, bottom, top, znear, zfar, out var result);
            return result;
        }

        public static void Billboard(this Matrix4x4 m, in Matrix4x4 world, out Matrix4x4 result)
        {
            var rot = world.BillboardRotation();

            result.M11 = m.M11;
            result.M12 = m.M21;
            result.M13 = m.M31;
            result.M14 = 0;
            result.M21 = -m.M12;
            result.M22 = -m.M22;
            result.M23 = -m.M32;
            result.M24 = 0;
            result.M31 = -m.M13;
            result.M32 = -m.M23;
            result.M33 = -m.M33;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            result *= rot;
        }

        public static Matrix4x4 Billboard(this Matrix4x4 m, in Matrix4x4 world)
        {
            m.Billboard(world, out var result);
            return result;
        }

        public static void VerticalBillboard(this Matrix4x4 m, in Matrix4x4 world, out Matrix4x4 result)
        {
            var rot = world.BillboardRotation();

            result.M11 = m.M11;
            result.M12 = m.M21;
            result.M13 = m.M31;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 0;
            result.M23 = -1;
            result.M24 = 0;
            result.M31 = -m.M13;
            result.M32 = -m.M23;
            result.M33 = -m.M33;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            result *= rot;
        }

        public static Matrix4x4 VerticalBillboard(this Matrix4x4 m, in Matrix4x4 world)
        {
            m.VerticalBillboard(world, out var result);
            return result;
        }

        public static void HorizontalBillboard(this Matrix4x4 m, in Matrix4x4 world, out Matrix4x4 result)
        {
            var rot = world.BillboardRotation();

            result.M11 = m.M11;
            result.M12 = m.M21;
            result.M13 = m.M31;
            result.M14 = 0;
            result.M21 = -m.M21;
            result.M22 = m.M11;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = m.M12;
            result.M32 = m.M22;
            result.M33 = m.M32;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            result *= rot;
        }
        public static Matrix4x4 HorizontalBillboard(this Matrix4x4 m, in Matrix4x4 world)
        {
            m.HorizontalBillboard(world, out var result);
            return result;
        }

        public static bool StretchBillboard(this Matrix4x4 m, in Matrix4x4 world, in Vector3 dir, float rate, out Matrix4x4 result)
        {
            var length = dir.Length();
            if (length == 0)
            {
                result = Matrix4x4.Identity;
                return false;
            }
            var ndir = dir / length;
            var forward = Vector3.Transform(new Vector3(m.M13, m.M23, m.M33), world.BillboardRotation());
            var xaxis = Vector3.Normalize(Vector3.Cross(-forward, ndir));
            var zaxis = Vector3.Normalize(Vector3.Cross(ndir, xaxis));

            result.M11 = xaxis.X;
            result.M12 = xaxis.Y;
            result.M13 = xaxis.Z;
            result.M14 = 0;
            result.M21 = -ndir.X;
            result.M22 = -ndir.Y;
            result.M23 = -ndir.Z;
            result.M24 = 0;
            result.M31 = zaxis.X;
            result.M32 = zaxis.Y;
            result.M33 = zaxis.Z;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            if (rate != 0)
            {
                rate *= length;
                result.M21 *= rate;
                result.M22 *= rate;
                result.M23 *= rate;
            }
            return true;
        }

        private static Matrix4x4 BillboardRotation(this Matrix4x4 m)
        {
            var scale = new Vector3(
                (float)Math.Sqrt(m.M11 * m.M11 + m.M12 * m.M12 + m.M13 * m.M13),
                (float)Math.Sqrt(m.M21 * m.M21 + m.M22 * m.M22 + m.M23 * m.M23),
                (float)Math.Sqrt(m.M31 * m.M31 + m.M32 * m.M32 + m.M33 * m.M33));

            if (scale.X == 0 || scale.Y == 0 || scale.Z == 0)
            {
                return Matrix4x4.Identity;
            }

            var rotation = new Matrix4x4
            {
                M11 = m.M11 / scale.X,
                M21 = m.M12 / scale.X,
                M31 = m.M13 / scale.X,
                M41 = 0,

                M12 = m.M21 / scale.Y,
                M22 = m.M22 / scale.Y,
                M32 = m.M23 / scale.Y,
                M42 = 0,

                M13 = m.M31 / scale.Z,
                M23 = m.M32 / scale.Z,
                M33 = m.M33 / scale.Z,
                M43 = 0,

                M14 = 0,
                M24 = 0,
                M34 = 0,
                M44 = 1
            };

            return rotation;
        }

        public static bool NearEqual(in Matrix3x3 v0, in Matrix3x3 v1)
        {
            return MathUtil.NearEqual(v0.M11, v1.M11) &&
                MathUtil.NearEqual(v0.M12, v1.M12) &&
                MathUtil.NearEqual(v0.M13, v1.M13) &&
                MathUtil.NearEqual(v0.M21, v1.M21) &&
                MathUtil.NearEqual(v0.M22, v1.M22) &&
                MathUtil.NearEqual(v0.M23, v1.M23) &&
                MathUtil.NearEqual(v0.M31, v1.M31) &&
                MathUtil.NearEqual(v0.M32, v1.M32) &&
                MathUtil.NearEqual(v0.M33, v1.M33);
        }

        public static bool NearEqual(in Matrix4x4 v0, in Matrix4x4 v1)
        {
            return MathUtil.NearEqual(v0.M11, v1.M11) &&
                MathUtil.NearEqual(v0.M12, v1.M12) &&
                MathUtil.NearEqual(v0.M13, v1.M13) &&
                MathUtil.NearEqual(v0.M14, v1.M14) &&
                MathUtil.NearEqual(v0.M21, v1.M21) &&
                MathUtil.NearEqual(v0.M22, v1.M22) &&
                MathUtil.NearEqual(v0.M23, v1.M23) &&
                MathUtil.NearEqual(v0.M24, v1.M24) &&
                MathUtil.NearEqual(v0.M31, v1.M31) &&
                MathUtil.NearEqual(v0.M32, v1.M32) &&
                MathUtil.NearEqual(v0.M33, v1.M33) &&
                MathUtil.NearEqual(v0.M34, v1.M34) &&
                MathUtil.NearEqual(v0.M41, v1.M41) &&
                MathUtil.NearEqual(v0.M42, v1.M42) &&
                MathUtil.NearEqual(v0.M43, v1.M43) &&
                MathUtil.NearEqual(v0.M44, v1.M44);
        }
    }
}
