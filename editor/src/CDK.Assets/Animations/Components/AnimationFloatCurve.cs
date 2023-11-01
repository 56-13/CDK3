using System;
using System.Text;
using System.IO;

using CDK.Drawing;


namespace CDK.Assets.Animations.Components
{
    public enum CurveValueType
    {
        Center,
        Min,
        Max
    }
    public class AnimationFloatCurve : AnimationFloatImpl
    {
        private AssetElementList<AnimationFloatCurvePoint> points;
        public bool Variant { private set; get; }

        private float _defaultValue;

        public AnimationFloatCurve(AssetElement parent, bool variant, float minValue, float maxValue, float defaultValue)
            : base(parent, minValue, maxValue)
        {
            Variant = variant;

            points = new AssetElementList<AnimationFloatCurvePoint>(this);

            _defaultValue = defaultValue;

            using (new AssetCommandHolder())
            {
                points.Add(new AnimationFloatCurvePoint(this, 0, _defaultValue, 0, 0, 0));
                points.Add(new AnimationFloatCurvePoint(this, 1, _defaultValue, 0, 0, 0));
                LinearRotatePoint(points[0], true, true);
                LinearRotatePoint(points[1], true, true);
            }
        }

        public AnimationFloatCurve(AssetElement parent, AnimationFloatCurve other)
            : base(parent, other.MinValue, other.MaxValue)
        {
            Variant = other.Variant;
            _defaultValue = other._defaultValue;

            points = new AssetElementList<AnimationFloatCurvePoint>(this);

            using (new AssetCommandHolder())
            {
                foreach (var otherPoint in other.points)
                {
                    points.Add(new AnimationFloatCurvePoint(this, otherPoint.T, otherPoint.V, otherPoint.VVar, otherPoint.LeftAngle, otherPoint.RightAngle));
                }
            }
        }

        public override AnimationFloatType Type => AnimationFloatType.Curve;

        internal override string SaveToString()
        {
            var strbuf = new StringBuilder();
            foreach (var p in points)
            {
                strbuf.Append(p.T);
                strbuf.Append(',');
                strbuf.Append(p.V);
                strbuf.Append(',');
                strbuf.Append(p.VVar);
                strbuf.Append(',');
                strbuf.Append(p.LeftAngle);
                strbuf.Append(',');
                strbuf.Append(p.RightAngle);
                strbuf.Append(';');
            }
            strbuf.Append(MaxValue - MinValue);

            return strbuf.ToString();
        }
        internal override void LoadFromString(string str)
        {
            points.Clear();
            foreach (var s in str.Split(';'))
            {
                var ps = s.Split(',');

                if (ps.Length == 5)
                {
                    points.Add(new AnimationFloatCurvePoint(this, float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]), float.Parse(ps[3]), float.Parse(ps[4])));
                }
                else
                {
                    var pd = float.Parse(s);
                    var nd = MaxValue - MinValue;

                    if (pd != nd)
                    {
                        foreach (var p in points)
                        {
                            var la = p.LeftAngle * MathUtil.ToRadians;
                            var ra = p.RightAngle * MathUtil.ToRadians;

                            la = (float)Math.Atan(Math.Tan(la) * pd / nd);
                            ra = (float)Math.Atan(Math.Tan(ra) * pd / nd);

                            p.LeftAngle = la * MathUtil.ToDegrees;
                            p.RightAngle = ra * MathUtil.ToDegrees;
                        }
                    }
                    break;
                }
            }
        }

        internal override void Build(BinaryWriter writer, bool asRadian)
        {
            writer.WriteLength(points.Count);
            foreach (var p in points)
            {
                var v = p.V;
                var vv = p.VVar;
                var la = p.LeftAngle * MathUtil.ToRadians;
                var ra = p.RightAngle * MathUtil.ToRadians;
                if (asRadian)
                {
                    v *= MathUtil.ToRadians;
                    vv *= MathUtil.ToRadians;
                    la = (float)Math.Atan((float)(Math.Tan(la) * (MaxValue - MinValue)) * MathUtil.ToRadians);
                    ra = (float)Math.Atan((float)(Math.Tan(ra) * (MaxValue - MinValue)) * MathUtil.ToRadians);
                }
                else
                {
                    la = (float)Math.Atan(Math.Tan(la) * (MaxValue - MinValue));
                    ra = (float)Math.Atan(Math.Tan(ra) * (MaxValue - MinValue));
                }
                writer.Write(p.T);
                writer.Write(v);
                writer.Write(vv);
                writer.Write(la);
                writer.Write(ra);
            }
        }

        private float GetValue(AnimationFloatCurvePoint p, CurveValueType loc)
        {
            var v = p.V;
            if (Variant)
            {
                if (loc == CurveValueType.Min)
                {
                    v -= p.VVar;
                }
                else if (loc == CurveValueType.Max)
                {
                    v += p.VVar;
                }
            }
            return v;
        }

        public float GetValue(float t, CurveValueType loc)
        {
            if (t <= 0)
            {
                return GetValue(points[0], loc);
            }
            if (t >= 1)
            {
                return GetValue(points[points.Count - 1], loc);
            }
            int i;
            for (i = 1; i < points.Count - 1; i++)
            {
                var p = points[i];

                if (p.T == t)
                {
                    return GetValue(p, loc);
                }
                else if (p.T > t)
                {
                    break;
                }
            }
            var p0 = points[i - 1];
            var p3 = points[i];

            var v0 = GetValue(p0, loc);
            var v3 = GetValue(p3, loc);

            if (p0.RightAngle == 90 || p3.LeftAngle == 90)
            {
                return Math.Max(v0, v3);
            }
            if (p0.RightAngle == -90 || p3.LeftAngle == -90)
            {
                return Math.Min(v0, v3);
            }

            var d = p3.T - p0.T;
            var v1 = v0 + (float)(Math.Tan(p0.RightAngle * MathUtil.ToRadians) * (MaxValue - MinValue) * (d / 3));
            var v2 = v3 - (float)(Math.Tan(p3.LeftAngle * MathUtil.ToRadians) * (MaxValue - MinValue) * (d / 3));

            t = (t - p0.T) / d;

            var rt = 1 - t;

            var v = (rt * rt * rt * v0) + (3 * rt * rt * t * v1) + (3 * rt * t * t * v2) + t * t * t * v3;

            return v;
        }
        public override float GetValue(float t)
        {
            return MathUtil.Clamp(GetValue(t, CurveValueType.Center), MinValue, MaxValue);
        }

        public override float GetValue(float t, float r)
        {
            return MathUtil.Clamp(Variant ? MathUtil.Lerp(GetValue(t, CurveValueType.Min), GetValue(t, CurveValueType.Max), r) : GetValue(t, CurveValueType.Center), MinValue, MaxValue);
        }

        public int PointCount => points.Count;

        public AnimationFloatCurvePoint GetPoint(int i) => points[i];

        public bool ContainsPoint(AnimationFloatCurvePoint p) => points.Contains(p);

        public void ResetPoints()
        {
            points.Clear();
            points.Add(new AnimationFloatCurvePoint(this, 0, _defaultValue, 0, 0, 0));
            points.Add(new AnimationFloatCurvePoint(this, 1, _defaultValue, 0, 0, 0));
            LinearRotatePoint(points[0], true, true);
            LinearRotatePoint(points[1], true, true);
        }

        public AnimationFloatCurvePoint AddPoint(float t)
        {
            if (t <= 0 || t >= 1)
            {
                return null;
            }
            int i;
            for (i = 0; i < points.Count; i++)
            {
                var p = points[i];

                if (p.T == t)
                {
                    return null;
                }
                else if (p.T > t)
                {
                    break;
                }
            }
            {
                var p0 = points[i - 1];
                var p1 = points[i];
                var v = p0.V + (p1.V - p0.V) * (t - p0.T) / (p1.T - p0.T);
                var a = (float)(Math.Atan2((p1.V - p0.V) / (MaxValue - MinValue), p1.T - p0.T)) * MathUtil.ToRadians;
                var p = new AnimationFloatCurvePoint(this, t, v, 0, a, a);
                points.Insert(i, p);

                return p;
            }
        }

        public void RemovePoint(float t)
        {
            for (var i = 1; i < points.Count - 1; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i + 1];
                var p = points[i];

                if (t > p.T - (p.T - p0.T) / 2 && t < p.T + (p1.T - p.T) / 2)
                {
                    points.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemovePoint(AnimationFloatCurvePoint p)
        {
            if (p != points[0] && p != points[points.Count - 1])
            {
                points.Remove(p);
            }
        }

        internal bool ReorderPoint(AnimationFloatCurvePoint p, float t)
        {
            var index = points.IndexOf(p);

            if (index > 0 && index < points.Count - 1)
            {
                for (var i = 1; i < points.Count; i++)
                {
                    if (t == points[i].T)
                    {
                        return false;
                    }
                    if (t < points[i].T)
                    {
                        if (i < index)
                        {
                            points.RemoveAt(index);
                            points.Insert(i, p);
                        }
                        else if (i > index)
                        {
                            points.RemoveAt(index);
                            points.Insert(i - 1, p);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public AnimationFloatCurvePoint SelectPoint(float t)
        {
            var p = points[0];
            var p1 = points[1];
            if (t < p.T + (p1.T - p.T) / 2)
            {
                return p;
            }
            var p0 = points[points.Count - 2];
            p = points[points.Count - 1];
            if (t > p.T - (p.T - p0.T) / 2)
            {
                return p;
            }
            for (var i = 1; i < points.Count - 1; i++)
            {
                p0 = points[i - 1];
                p1 = points[i + 1];
                p = points[i];

                if (t > p.T - (p.T - p0.T) / 2 && t < p.T + (p1.T - p.T) / 2)
                {
                    return p;
                }
            }
            return null;
        }

        internal void LinearRotatePoint(AnimationFloatCurvePoint p, bool left, bool right)
        {
            var index = points.IndexOf(p);

            float a0 = 0;
            float a1 = 0;
            if (index > 0)
            {
                a0 = (float)Math.Atan2((p.V - points[index - 1].V) / (MaxValue - MinValue), p.T - points[index - 1].T);
            }
            if (index < points.Count - 1)
            {
                a1 = (float)Math.Atan2((points[index + 1].V - p.V) / (MaxValue - MinValue), points[index + 1].T - p.T);
            }
            if (index == 0)
            {
                a0 = a1;
            }
            else if (index == points.Count - 1)
            {
                a1 = a0;
            }
            if (left && right)
            {
                p.LeftAngle = p.RightAngle = ((a0 + a1) * 0.5f) * MathUtil.ToDegrees;
            }
            else if (left)
            {
                p.LeftAngle = a0 * MathUtil.ToDegrees;
            }
            else if (right)
            {
                p.RightAngle = a1 * MathUtil.ToDegrees;
            }
        }

        public bool Equals(AnimationFloatCurve other)
        {
            if (points.Count != other.points.Count) return false;

            for (var i = 0; i < points.Count; i++)
            {
                if (!points[i].Equals(other.points[i])) return false;
            }
            return true;
        }

        public override AnimationFloatImpl Clone(AssetElement owner) => new AnimationFloatCurve(owner, this);
    }
}
