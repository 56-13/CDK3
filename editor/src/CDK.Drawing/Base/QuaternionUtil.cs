using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static class QuaternionUtil
    {
        public static void GetYawPitchRoll(this Quaternion v, out float yaw, out float pitch, out float roll)
        {
            var sinr_cosp = 2 * (v.W * v.X + v.Y * v.Z);
            var cosr_cosp = 1 - 2 * (v.X * v.X + v.Y * v.Y);
            roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            var sinp = 2 * (v.W * v.Y - v.Z * v.X);

            if (sinp <= -1) pitch = -MathUtil.PiOverTwo;
            else if (sinp >= 1) pitch = MathUtil.PiOverTwo;
            else pitch = (float)Math.Asin(sinp);

            var siny_cosp = 2 * (v.W * v.Z + v.X * v.Y);
            var cosy_cosp = 1 - 2 * (v.Y * v.Y + v.Z * v.Z);
            yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);
        }

        public static bool NearEqual(in Quaternion v0, in Quaternion v1)
        {
            return MathUtil.NearEqual(v0.X, v1.X) && MathUtil.NearEqual(v0.Y, v1.Y) && MathUtil.NearEqual(v0.Z, v1.Z) && MathUtil.NearEqual(v0.W, v1.W);
        }
    }
}
