using System;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationFloatCurvePoint : AssetElement
    {
        public AnimationFloatCurve Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private float _T;
        public float T
        {
            set
            {
                value = MathUtil.Clamp(value, 0, 1);

                if (_T != value && Parent.ReorderPoint(this, value)) SetProperty(ref _T, value);
            }
            get => _T;
        }

        private float _V;
        public float V
        {
            set
            {
                value = MathUtil.Clamp(value, Parent.MinValue, Parent.MaxValue);

                SetProperty(ref _V, value);
            }
            get => _V;
        }

        private float _VVar;
        public float VVar
        {
            set
            {
                value = MathUtil.Clamp(value, 0, (Parent.MaxValue - Parent.MinValue) * 0.5f);

                SetProperty(ref _VVar, value);
            }
            get => _VVar;
        }

        
        private float _LeftAngle;
        public float LeftAngle
        {
            set
            {
                value = MathUtil.Clamp(value, -90, 90);

                SetProperty(ref _LeftAngle, value);
            }
            get => _LeftAngle;
        }

        private float _RightAngle;
        public float RightAngle
        {
            set
            {
                value = MathUtil.Clamp(value, -90, 90);

                SetProperty(ref _RightAngle, value);
            }
            get => _RightAngle;
        }

        public AnimationFloatCurvePoint(AnimationFloatCurve parent, float t, float v, float vvar, float leftAngle, float rightAngle)
        {
            Parent = parent;

            _T = t;
            _V = v;
            _VVar = vvar;
            _LeftAngle = leftAngle;
            _RightAngle = rightAngle;
        }

        public void LinearRotate(bool left, bool right)
        {
            Parent.LinearRotatePoint(this, left, right);
        }

        public void LinearRotate()
        {
            Parent.LinearRotatePoint(this, true, true);
        }

        public bool Equals(AnimationFloatCurvePoint other)
        {
            return _T == other._T &&
                _V == other._V &&
                _VVar == other._VVar &&
                _LeftAngle == other._LeftAngle &&
                _RightAngle == other._RightAngle;
        }
    }
}
