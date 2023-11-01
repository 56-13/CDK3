using System.Text;
using System.Globalization;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationColorChannel : AnimationColorImpl
    {
        public AnimationFloat Red { private set; get; }
        public AnimationFloat Green { private set; get; }
        public AnimationFloat Blue { private set; get; }
        public AnimationFloat Alpha { private set; get; }

        private bool _FixedChannel;
        public bool FixedChannel
        {
            set => SetProperty(ref _FixedChannel, value);
            get => _FixedChannel;
        }

        public AnimationColorChannel(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor) : base(parent, normalized, alphaChannel, defaultColor)
        {
            Red = new AnimationFloat(this, 0, 1, defaultColor.R);
            Green = new AnimationFloat(this, 0, 1, defaultColor.G);
            Blue = new AnimationFloat(this, 0, 1, defaultColor.B);
            if (alphaChannel) Alpha = new AnimationFloat(this, 0, 1, defaultColor.A);
        }

        public AnimationColorChannel(AssetElement parent, AnimationColorChannel other) : base(parent, other)
        {
            Red = new AnimationFloat(this, other.Red);
            Green = new AnimationFloat(this, other.Green);
            Blue = new AnimationFloat(this, other.Blue);
            if (AlphaChannel) Alpha = new AnimationFloat(this, other.Alpha);
            _FixedChannel = other._FixedChannel;
        }

        public override AnimationColorType Type => AnimationColorType.Channel;
        public override bool HasOpacity => AlphaChannel && (Alpha.Type != AnimationFloatType.None || DefaultColor.A < 1);
        public override Color4 GetColor(float t, in Color4 r)
        {
            var random = _FixedChannel ? new Color4(r.R, r.R, r.R, r.R) : r;

            var red = Red.GetValue(t, random.R);
            var green = Green.GetValue(t, random.G);
            var blue = Blue.GetValue(t, random.B);
            var alpha = AlphaChannel ? MathUtil.Clamp(Alpha.GetValue(t, random.A), 0, 1) : 1;
            if (Normalized)
            {
                red = MathUtil.Clamp(red, 0, 1);
                green = MathUtil.Clamp(green, 0, 1);
                blue = MathUtil.Clamp(blue, 0, 1);
            }
            return new Color4(red, green, blue, alpha);
        }

        internal override string SaveToString() 
        {
            var strbuf = new StringBuilder();
            strbuf.Append(Red.SaveToString());
            strbuf.Append('|');
            strbuf.Append(Green.SaveToString());
            strbuf.Append('|');
            strbuf.Append(Blue.SaveToString());
            if (AlphaChannel)
            {
                strbuf.Append('|');
                strbuf.Append(Alpha.SaveToString());
            }
            return strbuf.ToString();
        }

        internal override void LoadFromString(string str)
        {
            var ps = str.Split('|');
            Red.LoadFromString(ps[0]);
            Green.LoadFromString(ps[1]);
            Blue.LoadFromString(ps[2]);
            if (AlphaChannel) Alpha.LoadFromString(ps[3]);
        }

        internal override void Build(BinaryWriter writer)
        {
            Red.Build(writer, false);
            Green.Build(writer, false);
            Blue.Build(writer, false);
            if (AlphaChannel) Alpha.Build(writer, false);
        }

        public bool Equals(AnimationColorChannel other) => Red.Equals(other.Red) && Green.Equals(other.Green) && Blue.Equals(other.Blue) && (!AlphaChannel || Alpha.Equals(other.Alpha));
        public override AnimationColorImpl Clone(AssetElement parent) => new AnimationColorChannel(parent, this);
    }
}
