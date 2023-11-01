using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class Collision
    {
        public static void SegmentToPoint(in Segment seg, in Vector3 point, out Vector3 near)
        {
            var v = seg.Position1 - seg.Position0;

            var len2 = v.LengthSquared();

            if (MathUtil.NearZero(len2))
            {
                near = seg.Position0;
                return;
            }

            var pv = point - seg.Position0;

            var t = Vector3.Dot(pv, v) / len2;

            if (t > 1) near = seg.Position1;
            else if (t > 0) near = seg.Position0 + v * t;
            else near = seg.Position0;
        }

        public static void SegmentToSegment(in Segment seg0, in Segment seg1, out Vector3 near0, out Vector3 near1)
        {
            var r = seg1.Position0 - seg0.Position0;
            var u = seg0.Position1 - seg0.Position0;
            var v = seg1.Position1 - seg1.Position0;

            var ru = Vector3.Dot(r, u);
            var rv = Vector3.Dot(r, v);
            var uu = Vector3.Dot(u, u);
            var uv = Vector3.Dot(u, v);
            var vv = Vector3.Dot(v, v);

            var det = uu * vv - uv * uv;

            float s, t;

            if (det < MathUtil.ZeroTolerance * uu * vv)
            {
                s = MathUtil.Clamp(ru / uu, 0, 1);
                t = 0;
            }
            else
            {
                s = MathUtil.Clamp((ru * vv - rv * uv) / det, 0, 1);
                t = MathUtil.Clamp((ru * uv - rv * uu) / det, 0, 1);
            }

            var a = MathUtil.Clamp((t * uv + ru) / uu, 0, 1);
            var b = MathUtil.Clamp((s * uv - rv) / vv, 0, 1);

            near0 = seg0.Position0 + a * u;
            near1 = seg1.Position0 + b * v;
        }
    }
}
