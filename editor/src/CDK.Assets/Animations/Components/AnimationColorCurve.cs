using System.Globalization;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationColorCurve : AnimationColorImpl
    {
        private Color4 _StartColor;
        public Color4 StartColor
        {
            set
            {
                if (!AlphaChannel) value.A = 1;

                SetProperty(ref _StartColor, value);
            }
            get => _StartColor;
        }

        private Color4 _EndColor;
        public Color4 EndColor
        {
            set
            {
                if (!AlphaChannel) value.A = 1;

                SetProperty(ref _EndColor, value);
            }
            get => _EndColor;
        }

        public AnimationFloatCurve Curve { private set; get; }

        public AnimationColorCurve(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor) : base(parent, normalized, alphaChannel, defaultColor)
        {
            _StartColor = _EndColor = defaultColor;

            Curve = new AnimationFloatCurve(this, true, 0, 1, 0.5f);
        }

        public AnimationColorCurve(AssetElement parent, AnimationColorCurve other) : base(parent, other)
        {
            _StartColor = other._StartColor;
            _EndColor = other._EndColor;
            
            Curve = new AnimationFloatCurve(this, other.Curve);
        }

        public override AnimationColorType Type => AnimationColorType.Curve;
        public override bool HasOpacity => AlphaChannel && (_StartColor.A < 1 || _EndColor.A < 1);
        public override Color4 GetColor(float t, in Color4 r)
        {
            t = Curve.GetValue(t, r.R);

            return Color4.Lerp(_StartColor, _EndColor, t);
        }

        internal override string SaveToString() 
        {
            if (Normalized)
            {
                return $"{_StartColor.ToRgba():X8)}|{_EndColor.ToRgba():X8)}|{Curve.SaveToString()}";
            }
            else
            {
                return $"{_StartColor.R},{_StartColor.G},{_StartColor.B},{_StartColor.A}|{_EndColor.R},{_EndColor.G},{_EndColor.B},{_EndColor.A}|{Curve.SaveToString()}";
            }
        }

        internal override void LoadFromString(string str)
        {
            var ps = str.Split('|');

            if (Normalized)
            {
                StartColor = new Color4(uint.Parse(ps[0], NumberStyles.HexNumber));
                EndColor = new Color4(uint.Parse(ps[1], NumberStyles.HexNumber));
            }
            else
            {
                var vs0 = ps[0].Split(',');
                StartColor = new Color4(float.Parse(vs0[0]), float.Parse(vs0[1]), float.Parse(vs0[2]), float.Parse(vs0[3]));
                var vs1 = ps[1].Split(',');
                StartColor = new Color4(float.Parse(vs1[0]), float.Parse(vs1[1]), float.Parse(vs1[2]), float.Parse(vs1[3]));
            }
            Curve.LoadFromString(ps[2]);
        }

        internal override void Build(BinaryWriter writer)
        {
            if (AlphaChannel)
            {
                writer.Write(_StartColor, Normalized);
                writer.Write(_EndColor, Normalized);
            }
            else
            {
                writer.Write((Color3)_StartColor, Normalized);
                writer.Write((Color3)_EndColor, Normalized);
            }
            Curve.Build(writer, false);
        }

        public bool Equals(AnimationColorCurve other) => _StartColor == other._StartColor && _EndColor == other._EndColor && Curve.Equals(other.Curve);
        public override AnimationColorImpl Clone(AssetElement parent) => new AnimationColorCurve(parent, this);
    }
}
