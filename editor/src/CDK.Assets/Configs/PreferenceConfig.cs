using System.Linq;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Configs
{
    public class PreferenceConfig : AssetElement
    {
        public SceneConfig Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public int Index => Parent.Preferences.IndexOf(this);

        public string Key { private set; get; }

        private string _Name;
        public string Name
        {
            set => SetProperty(ref _Name, GetNewName(value));
            get => _Name;
        }

        private LightMode _LightMode;
        public LightMode LightMode
        {
            set => SetProperty(ref _LightMode, value);
            get => _LightMode;
        }

        private int _Samples;
        public int Samples
        {
            set => SetProperty(ref _Samples, value);
            get => _Samples;
        }

        private bool _AllowShadow;
        public bool AllowShadow
        {
            set => SetProperty(ref _AllowShadow, value);
            get => _AllowShadow;
        }

        private bool _AllowShadowPixel32;
        public bool AllowShadowPixel32
        {
            set => SetProperty(ref _AllowShadowPixel32, value);
            get => _AllowShadowPixel32;
        }

        private int _MaxShadowResolution;
        public int MaxShadowResolution
        {
            set => SetProperty(ref _MaxShadowResolution, value);
            get => _MaxShadowResolution;
        }

        public PreferenceConfig(SceneConfig parent, XmlNode node)
        {
            Parent = parent;

            if (node.LocalName != "preference") throw new XmlException();

            _Name = node.ReadAttributeString("name");

            Key = node.ReadAttributeString("key", _Name);

            _LightMode = node.ReadAttributeEnum("lightMode", LightMode.CookGGX);
            _Samples = node.ReadAttributeInt("samples", 1);
            _AllowShadow = node.ReadAttributeBool("allowShadow", true);
            _AllowShadowPixel32 = node.ReadAttributeBool("allowShadowPixel32", true);
            _MaxShadowResolution = node.ReadAttributeInt("maxShadowResolution", 2048);
        }

        public PreferenceConfig(SceneConfig parent, PreferenceConfig other)
        {
            Parent = parent;

            Key = Parent.Preferences.Any(e => e.Key == other.Key) ? AssetManager.NewKey() : other.Key;
            _Name = GetNewName(other._Name);
            _LightMode = other._LightMode;
            _Samples = other._Samples;
            _AllowShadow = other._AllowShadow;
            _AllowShadowPixel32 = other._AllowShadowPixel32;
            _MaxShadowResolution = other._MaxShadowResolution;
        }

        private string GetNewName(string name)
        {
            var rname = name;
            var i = rname.LastIndexOf(' ');
            if (i > 0 && int.TryParse(rname.Substring(i + 1), out _)) rname = rname.Substring(0, i);
            i = 1;
            while (Parent.Preferences.Any(c => c != this && c.Name == rname)) rname = $"{name} {i++}";
            name = rname;
            return name;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("preference");

            writer.WriteAttribute("key", Key, _Name);
            writer.WriteAttribute("name", _Name);

            writer.WriteAttribute("lightMode", _LightMode, LightMode.CookGGX);
            writer.WriteAttribute("samples", _Samples, 1);
            writer.WriteAttribute("allowShadow", _AllowShadow, true);
            writer.WriteAttribute("allowShadowPixel32", _AllowShadowPixel32, true);
            writer.WriteAttribute("maxShadowResolution", _MaxShadowResolution, 2048);

            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write((byte)_LightMode);
            writer.Write((byte)_Samples);
            writer.Write(_AllowShadow);
            writer.Write(_AllowShadowPixel32);
            writer.Write((ushort)_MaxShadowResolution);
        }
    }
}
