using System.Globalization;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationColorLinear : AnimationColorImpl
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

        private bool _Smooth;
        public bool Smooth
        {
            set => SetProperty(ref _Smooth, value);
            get => _Smooth;
        }

        public AnimationColorLinear(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor) : base(parent, normalized, alphaChannel, defaultColor)
        {
            _StartColor = _EndColor = defaultColor;
        }

        public AnimationColorLinear(AssetElement parent, AnimationColorLinear other) : base(parent, other)
        {
            _StartColor = other._StartColor;
            _EndColor = other._EndColor;
            _Smooth = other._Smooth;
        }

        public override AnimationColorType Type => AnimationColorType.Linear;
        public override bool HasOpacity => AlphaChannel && (_StartColor.A < 1 || _EndColor.A < 1);
        public override Color4 GetColor(float t, in Color4 r)
        {
            if (_Smooth) t = MathUtil.SmoothStep(t);
            return Color4.Lerp(_StartColor, _EndColor, t);
        }

        internal override string SaveToString()
        {
            if (Normalized)
            {
                return $"{_StartColor.ToRgba():X8}|{_EndColor.ToRgba():X8)}|{_Smooth}";
            }
            else
            {
                return $"{_StartColor.R},{_StartColor.G},{_StartColor.B},{_StartColor.A}|{_EndColor.R},{_EndColor.G},{_EndColor.B},{_EndColor.A}|{_Smooth}";
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
            Smooth = bool.Parse(ps[2]);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_StartColor, Normalized);
            writer.Write(_EndColor, Normalized);
            writer.Write(_Smooth);
        }

        public bool Equals(AnimationColorLinear other) => _StartColor == other._StartColor && _EndColor == other._EndColor && _Smooth == other._Smooth;
        public override AnimationColorImpl Clone(AssetElement parent) => new AnimationColorLinear(parent, this);
    }
}
