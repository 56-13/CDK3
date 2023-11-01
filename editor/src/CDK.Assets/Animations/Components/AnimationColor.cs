using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public class AnimationColor : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        
        private AnimationColorImpl _Impl;
        public AnimationColorImpl Impl
        {
            private set
            {
                SetProperty(ref _Impl, value);
                OnPropertyChanged("Type");
            }
            get => _Impl;
        }

        public AnimationColorType Type
        {
            set
            {
                if (Type != value) Impl = Create(value);
            }
            get => _Impl != null ? _Impl.Type : AnimationColorType.None;
        }
        public bool IsAnimating => _Impl != null && _Impl.Type != AnimationColorType.Constant;

        private bool _normalized;
        private bool _alphaChannel;
        private Color4 _defaultColor;

        public AnimationColor(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor)
        {
            Parent = parent;

            _normalized = normalized;
            _alphaChannel = alphaChannel;
            _defaultColor = defaultColor;
            if (!alphaChannel) _defaultColor.A = 1;
        }

        public AnimationColor(AssetElement parent, AnimationColor other)
        {
            Parent = parent;

            _normalized = other._normalized;
            _alphaChannel = other._alphaChannel;
            _defaultColor = other._defaultColor;

            _Impl = other._Impl?.Clone(this);
        }

        public bool HasOpacity => _alphaChannel && (_Impl?.HasOpacity ?? _defaultColor.A < 1);
        public Color4 GetColor(float t, in Color4 r) => _Impl?.GetColor(t, r) ?? _defaultColor;

        internal void Save(XmlWriter writer, string name)
        {
            if(_Impl != null)
            {
                writer.WriteAttribute(name, $"{_Impl.Type}|{_Impl.SaveToString()}");
            }
        }

        internal void Load(XmlNode node, string name)
        {
            if (node.Attributes[name] != null)
            {
                var str = node.Attributes[name].Value;
                var i = str.IndexOf('|');
                var type = (AnimationColorType)Enum.Parse(typeof(AnimationColorType), str.Substring(0, i));

                Impl = Create(type);

                _Impl.LoadFromString(str.Substring(i + 1));
            }
            else Impl = null;
        }

        internal void Build(BinaryWriter writer)
        {
            if (_Impl != null)
            {
                writer.Write((byte)_Impl.Type);
                _Impl.Build(writer);
            }
            else writer.Write((byte)AnimationColorType.None);
        }

        public void CopyFrom(AnimationColor other)
        {
            _normalized = other._normalized;
            _alphaChannel = other._alphaChannel;
            _defaultColor = other._defaultColor;

            Impl = other._Impl?.Clone(this);
        }

        public bool Equals(AnimationColor other)
        {
            if (Type == other.Type)
            {
                switch (Type)
                {
                    case AnimationColorType.None:
                        return _defaultColor == other._defaultColor;
                    case AnimationColorType.Constant:
                        return ((AnimationColorConstant)_Impl).Equals((AnimationColorConstant)other._Impl);
                    case AnimationColorType.Linear:
                        return ((AnimationColorLinear)_Impl).Equals((AnimationColorLinear)other._Impl);
                    case AnimationColorType.Curve:
                        return ((AnimationColorCurve)_Impl).Equals((AnimationColorCurve)other._Impl);
                    case AnimationColorType.Channel:
                        return ((AnimationColorChannel)_Impl).Equals((AnimationColorChannel)other._Impl);
                }
            }
            return false;
        }

        private AnimationColorImpl Create(AnimationColorType type)
        {
            switch (type)
            {
                case AnimationColorType.None:
                    return null;
                case AnimationColorType.Constant:
                    return new AnimationColorConstant(this, _normalized, _alphaChannel, _defaultColor);
                case AnimationColorType.Linear:
                    return new AnimationColorLinear(this, _normalized, _alphaChannel, _defaultColor);
                case AnimationColorType.Curve:
                    return new AnimationColorCurve(this, _normalized, _alphaChannel, _defaultColor);
                case AnimationColorType.Channel:
                    return new AnimationColorChannel(this, _normalized, _alphaChannel, _defaultColor);
            }
            throw new NotImplementedException();
        }
    }
}
