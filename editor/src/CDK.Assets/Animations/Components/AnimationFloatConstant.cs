using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationFloatConstant : AnimationFloatImpl
    {
        private float _Value;
        public float Value
        {
            set => SetProperty(ref _Value, value);
            get => _Value;
        }

        private float _ValueVar;
        public float ValueVar
        {
            set => SetProperty(ref _ValueVar, value);
            get => _ValueVar;
        }

        public AnimationFloatConstant(AssetElement parent, float minValue, float maxValue, float defaultValue) : base(parent, minValue, maxValue)
        {
            _Value = defaultValue;
        }

        public AnimationFloatConstant(AssetElement parent, AnimationFloatConstant other) : base(parent, other)
        {
            _Value = other._Value;
            _ValueVar = other._ValueVar;
        }

        public override AnimationFloatType Type => AnimationFloatType.Constant;

        public override float GetValue(float t) => MathUtil.Clamp(_Value, MinValue, MaxValue);
        public override float GetValue(float t, float r) => MathUtil.Clamp(MathUtil.Lerp(_Value - _ValueVar, _Value + _ValueVar, r), MinValue, MaxValue);

        internal override string SaveToString() => $"{_Value},{_ValueVar}";
        internal override void LoadFromString(string str)
        {
            var i = str.IndexOf(',');
            Value = float.Parse(str.Substring(0, i));
            ValueVar = float.Parse(str.Substring(i + 1));
        }

        internal override void Build(BinaryWriter writer, bool asRadian)
        {
            var v = _Value;
            var vv = _ValueVar;

            if (asRadian)
            {
                v *= MathUtil.ToRadians;
                vv *= MathUtil.ToRadians;
            }
            writer.Write(v);
            writer.Write(vv);
        }

        public bool Equals(AnimationFloatConstant other)
        {
            return _Value == other._Value && _ValueVar == other._ValueVar;
        }

        public override AnimationFloatImpl Clone(AssetElement owner) => new AnimationFloatConstant(owner, this);
    }
}
