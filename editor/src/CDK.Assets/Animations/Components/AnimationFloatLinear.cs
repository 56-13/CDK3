using System;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationFloatLinear : AnimationFloatImpl
    {
        private float _StartValue;
        public float StartValue
        {
            set => SetProperty(ref _StartValue, value);
            get => _StartValue;
        }

        private float _StartValueVar;
        public float StartValueVar
        {
            set => SetProperty(ref _StartValueVar, value);
            get => _StartValueVar;
        }

        private float _EndValue;
        public float EndValue
        {
            set => SetProperty(ref _EndValue, value);
            get => _EndValue;
        }

        private float _EndValueVar;
        public float EndValueVar
        {
            set => SetProperty(ref _EndValueVar, value);
            get => _EndValueVar;
        }

        private bool _Smooth;
        public bool Smooth
        {
            set => SetProperty(ref _Smooth, value);
            get => _Smooth;
        }

        public AnimationFloatLinear(AssetElement parent, float minValue, float maxValue, float defaultValue) : base(parent, minValue, maxValue)
        {
            _StartValue = defaultValue;
            _EndValue = defaultValue;
        }

        public AnimationFloatLinear(AssetElement parent, AnimationFloatLinear other) : base(parent, other)
        {
            _StartValue = other._StartValue;
            _StartValueVar = other._StartValueVar;
            _EndValue = other._EndValue;
            _EndValueVar = other._EndValueVar;
            _Smooth = other._Smooth;
        }

        public override AnimationFloatType Type => AnimationFloatType.Linear;

        public override float GetValue(float t)
        {
            return MathUtil.Clamp(MathUtil.Lerp(_StartValue, _EndValue, t), MinValue, MaxValue);
        }

        public override float GetValue(float t, float r)
        {
            var s = MathUtil.Lerp(_StartValue - _StartValueVar, _StartValue + _StartValueVar, r);
            var e = MathUtil.Lerp(_EndValue - _EndValueVar, _EndValue + _EndValueVar, r);
            if (_Smooth) t = MathUtil.SmoothStep(t);
            return MathUtil.Clamp(MathUtil.Lerp(s, e, t), MinValue, MaxValue);
        }

        internal override string SaveToString() => $"{_StartValue},{_StartValueVar},{_EndValue},{_EndValueVar},{_Smooth}";
        internal override void LoadFromString(string str)
        {
            var ps = str.Split(',');
            StartValue = float.Parse(ps[0]);
            StartValueVar = float.Parse(ps[1]);
            EndValue = float.Parse(ps[2]);
            EndValueVar = float.Parse(ps[3]);
            Smooth = bool.Parse(ps[4]);
        }

        internal override void Build(BinaryWriter writer, bool asRadian)
        {
            var sv = _StartValue;
            var svv = _StartValueVar;
            var ev = _EndValue;
            var evv = _EndValueVar;

            if (asRadian)
            {
                sv *= MathUtil.ToRadians;
                svv *= MathUtil.ToRadians;
                ev *= MathUtil.ToRadians;
                evv *= MathUtil.ToRadians;
            }
            writer.Write(sv);
            writer.Write(svv);
            writer.Write(ev);
            writer.Write(evv);
            writer.Write(_Smooth);
        }

        public bool Equals(AnimationFloatLinear other)
        {
            return _StartValue == other._StartValue &&
                _StartValueVar == other._StartValueVar &&
                _EndValue == other._EndValue &&
                _EndValueVar == other._EndValueVar;
        }

        public override AnimationFloatImpl Clone(AssetElement owner) => new AnimationFloatLinear(owner, this);
    }
}
