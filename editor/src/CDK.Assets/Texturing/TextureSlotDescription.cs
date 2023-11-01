using System;
using System.Text;
using System.Globalization;
using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Texturing
{
    public class TextureSlotDescription : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public TextureTarget Target { private set; get; }

        private TextureWrapMode _WrapS;
        public TextureWrapMode WrapS
        {
            set => SetProperty(ref _WrapS, value);
            get => _WrapS;
        }

        private TextureWrapMode _WrapT;
        public TextureWrapMode WrapT
        {
            set => SetProperty(ref _WrapT, value);
            get => _WrapT;
        }

        private TextureWrapMode _WrapR;
        public TextureWrapMode WrapR
        {
            set => SetProperty(ref _WrapR, value);
            get => _WrapR;
        }

        private TextureMinFilter _MinFilter;
        public TextureMinFilter MinFilter
        {
            set => SetProperty(ref _MinFilter, value);
            get => _MinFilter;
        }

        private TextureMagFilter _MagFilter;
        public TextureMagFilter MagFilter
        {
            set => SetProperty(ref _MagFilter, value);
            get => _MagFilter;
        }

        private Color4 _BorderColor;
        public Color4 BorderColor
        {
            set => SetProperty(ref _BorderColor, value);
            get => _BorderColor;
        }

        private int _MipmapCount;
        public int MipmapCount
        {
            set => SetProperty(ref _MipmapCount, value);
            get => _MipmapCount;
        }

        public const int MaxMipmapCount = 10;

        public TextureDescription GetDescription(RawFormat[] formats = null)
        {
            var desc = new TextureDescription()
            {
                Target = Target,
                WrapS = _WrapS,
                WrapT = _WrapT,
                WrapR = _WrapR,
                MinFilter = _MinFilter,
                MagFilter = _MagFilter,
                BorderColor = _BorderColor,
                MipmapCount = _MipmapCount
            };
            if (formats != null)
            {
                foreach (var format in formats)
                {
                    if (format.IsSupported())
                    {
                        desc.Format = format;
                        break;
                    }
                }
            }
            return desc;
        }

        public TextureSlotDescription(AssetElement parent, TextureTarget target)
        {
            Parent = parent;

            Target = target;

            _WrapS = TextureWrapMode.Repeat;
            _WrapT = TextureWrapMode.Repeat;
            _WrapR = TextureWrapMode.Repeat;
            _MinFilter = TextureMinFilter.Linear;
            _MagFilter = TextureMagFilter.Linear;
            _MipmapCount = 1;
        }

        public TextureSlotDescription(AssetElement parent, TextureSlotDescription other)
        {
            Parent = parent;

            Target = other.Target;

            _WrapS = other._WrapS;
            _WrapT = other._WrapT;
            _WrapR = other._WrapR;
            _MinFilter = other._MinFilter;
            _MagFilter = other._MagFilter;
            _BorderColor = other._BorderColor;
            _MipmapCount = other._MipmapCount;
        }

        internal void Save(XmlWriter writer, string name)
        {
            var str = new StringBuilder();
            str.Append(_WrapS);
            str.Append(',');
            str.Append(_WrapT);
            str.Append(',');
            str.Append(_WrapR);
            str.Append(',');
            str.Append(_MinFilter);
            str.Append(',');
            str.Append(_MagFilter);
            str.Append(',');
            str.Append($"{_BorderColor.ToRgba():X8}");
            str.Append(',');
            str.Append(_MipmapCount);
            writer.WriteAttribute(name, str.ToString());
        }

        internal void Load(XmlNode node, string name)
        {
            var value = node.ReadAttributeString(name);

            var ps = value.Split(',');
            WrapS = (TextureWrapMode)Enum.Parse(typeof(TextureWrapMode), ps[0]);
            WrapT = (TextureWrapMode)Enum.Parse(typeof(TextureWrapMode), ps[1]);
            WrapR = (TextureWrapMode)Enum.Parse(typeof(TextureWrapMode), ps[2]);
            MinFilter = (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), ps[3]);
            MagFilter = (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), ps[4]);
            BorderColor = new Color4(uint.Parse(ps[5], NumberStyles.HexNumber));
            MipmapCount = int.Parse(ps[6]);
        }
    }
}
