using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationFloat : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private AnimationFloatImpl _Impl;
        public AnimationFloatImpl Impl
        {
            private set
            {
                SetProperty(ref _Impl, value);
                OnPropertyChanged("Type");
            }
            get => _Impl;
        }

        public AnimationFloatType Type
        {
            set
            {
                if (Type != value) Impl = Create(value);
            }
            get => _Impl != null ? _Impl.Type : AnimationFloatType.None;
        }

        public bool IsAnimating => _Impl != null && _Impl.Type != AnimationFloatType.Constant;

        private float _minValue;
        private float _maxValue;
        private float _defaultValue;

        public AnimationFloat(AssetElement parent, float minValue, float maxValue, float defaultValue)
        {
            Parent = parent;

            _minValue = minValue;
            _maxValue = maxValue;
            _defaultValue = defaultValue;
        }

        public AnimationFloat(AssetElement parent, AnimationFloat other)
        {
            Parent = parent;

            _minValue = other._minValue;
            _maxValue = other._maxValue;
            _defaultValue = other._defaultValue;

            _Impl = other._Impl?.Clone(this);
        }

        public float GetValue(float t) => _Impl?.GetValue(t) ?? _defaultValue;
        public float GetValue(float t, float r) => _Impl?.GetValue(t, r) ?? _defaultValue;
        
        internal string SaveToString() => _Impl != null ? $"{_Impl.Type};{_Impl.SaveToString()}" : "None";
        internal void Save(XmlWriter writer, string name)
        {
            if (_Impl != null)
            {
                writer.WriteAttribute(name, SaveToString());
            }
        }

        internal void LoadFromString(string str)
        {
            var i = str.IndexOf(';');
            var type = (AnimationFloatType)Enum.Parse(typeof(AnimationFloatType), str.Substring(0, i));

            Impl = Create(type);

            _Impl?.LoadFromString(str.Substring(i + 1));
        }

        internal void Load(XmlNode node, string name)
        {
            if (node.Attributes[name] != null) LoadFromString(node.Attributes[name].Value);
            else Impl = null;
        }

        internal void Build(BinaryWriter writer, bool asRadian) 
        {
            if (_Impl != null)
            {
                writer.Write((byte)_Impl.Type);
                _Impl.Build(writer, asRadian);
            }
            else writer.Write((byte)AnimationFloatType.None);
        }

        public void CopyFrom(AnimationFloat other)
        {
            _minValue = other._minValue;
            _maxValue = other._maxValue;
            _defaultValue = other._defaultValue;

            Impl = other._Impl?.Clone(this);
        }

        public bool Equals(AnimationFloat other)
        {
            if (Type == other.Type)
            {
                switch (Type)
                {
                    case AnimationFloatType.None:
                        return _defaultValue == other._defaultValue;
                    case AnimationFloatType.Constant:
                        return ((AnimationFloatConstant)_Impl).Equals((AnimationFloatConstant)other._Impl);
                    case AnimationFloatType.Linear:
                        return ((AnimationFloatLinear)_Impl).Equals((AnimationFloatLinear)other._Impl);
                    case AnimationFloatType.Curve:
                        return ((AnimationFloatCurve)_Impl).Equals((AnimationFloatCurve)other._Impl);
                }
            }
            return false;
        }

        private AnimationFloatImpl Create(AnimationFloatType type)
        {
            switch (type)
            {
                case AnimationFloatType.None:
                    return null;
                case AnimationFloatType.Constant:
                    return new AnimationFloatConstant(this, _minValue, _maxValue, _defaultValue);
                case AnimationFloatType.Linear:
                    return new AnimationFloatLinear(this, _minValue, _maxValue, _defaultValue);
                case AnimationFloatType.Curve:
                    return new AnimationFloatCurve(this, true, _minValue, _maxValue, _defaultValue);
            }
            throw new NotImplementedException();
        }
    }
}
