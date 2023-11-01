using System.Xml;
using System.IO;

namespace CDK.Assets.Scenes
{
    public class Shadow : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;


        private int _Resolution;
        public int Resolution
        {
            set => SetProperty(ref _Resolution, value);
            get => _Resolution;
        }

        private bool _Pixel32;
        public bool Pixel32
        {
            set => SetProperty(ref _Pixel32, value);
            get => _Pixel32;
        }

        private float _Blur;
        public float Blur
        {
            set => SetProperty(ref _Blur, value);
            get => _Blur;
        }

        private float _Bias;
        public float Bias
        {
            set => SetProperty(ref _Bias, value);
            get => _Bias;
        }

        private float _Bleeding;
        public float Bleeding
        {
            set => SetProperty(ref _Bleeding, value);
            get => _Bleeding;
        }

        internal Shadow(AssetElement parent)
        {
            Parent = parent;

            _Resolution = Drawing.Shadow.DefaultResolution;
            _Blur = Drawing.Shadow.DefaultBlur;
            _Bias = Drawing.Shadow.DefaultBias;
            _Bleeding = Drawing.Shadow.DefaultBleeding;
        }

        internal Shadow(AssetElement parent, Shadow other, bool binding) : base(other, binding)
        {
            Parent = parent;

            _Resolution = other._Resolution;
            _Pixel32 = other._Pixel32;
            _Blur = other._Blur;
            _Bias = other._Bias;
            _Bleeding = other._Bleeding;
        }

        public static implicit operator Drawing.Shadow(Shadow shadow)
        {
            return new Drawing.Shadow(shadow._Pixel32, shadow._Resolution, shadow._Blur, shadow._Bias, shadow._Bleeding);
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("shadow");
            writer.WriteAttribute("resolution", _Resolution, Drawing.Shadow.DefaultResolution);
            writer.WriteAttribute("pixel32", _Pixel32, true);
            writer.WriteAttribute("blur", _Blur, Drawing.Shadow.DefaultBlur);
            writer.WriteAttribute("bias", _Bias, Drawing.Shadow.DefaultBias);
            writer.WriteAttribute("bleeding", _Bleeding, Drawing.Shadow.DefaultBleeding);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "shadow") throw new XmlException();

            Resolution = node.ReadAttributeInt("resolution", Drawing.Shadow.DefaultResolution);
            Pixel32 = node.ReadAttributeBool("pixel32", true);
            Blur = node.ReadAttributeFloat("blur", Drawing.Shadow.DefaultBlur);
            Bias = node.ReadAttributeFloat("bias", Drawing.Shadow.DefaultBias);
            Bleeding = node.ReadAttributeFloat("bleeding", Drawing.Shadow.DefaultBleeding);
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write((ushort)_Resolution);
            writer.Write(_Pixel32);
            writer.Write(_Blur);
            writer.Write(_Bias);
            writer.Write(_Bleeding);
        }
    }
}
