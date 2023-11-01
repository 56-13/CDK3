using System.Globalization;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationColorConstant : AnimationColorImpl
    {
        private Color4 _Color;
        public Color4 Color
        {
            set
            {
                if (!AlphaChannel) value.A = 1;

                SetProperty(ref _Color, value);
            }
            get => _Color;
        }

        public AnimationColorConstant(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor) : base(parent, normalized, alphaChannel, defaultColor)
        {
            _Color = defaultColor;
        }

        public AnimationColorConstant(AssetElement parent, AnimationColorConstant other) : base(parent, other)
        {
            _Color = other._Color;
        }

        public override AnimationColorType Type => AnimationColorType.Constant;
        public override bool HasOpacity => AlphaChannel && _Color.A < 1;
        public override Color4 GetColor(float t, in Color4 r) => _Color;
        internal override string SaveToString() => Normalized ? _Color.ToRgba().ToString("X8") : $"{_Color.R},{_Color.G},{_Color.B},{_Color.A}";
        internal override void LoadFromString(string str)
        {
            if (Normalized) Color = new Color4(uint.Parse(str, NumberStyles.HexNumber));
            else
            {
                var vs = str.Split(',');
                Color = new Color4(float.Parse(vs[0]), float.Parse(vs[1]), float.Parse(vs[2]), float.Parse(vs[3]));
            }
        }
        internal override void Build(BinaryWriter writer) => writer.Write(_Color, Normalized);
        public bool Equals(AnimationColorConstant other) => _Color == other._Color;
        public override AnimationColorImpl Clone(AssetElement parent) => new AnimationColorConstant(parent, this);
    }
}
