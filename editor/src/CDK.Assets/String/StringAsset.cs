using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.String
{
    public class StringAsset : Asset
    {
        private LocaleString _Value;
        public LocaleString Value
        {
            get
            {
                Load();
                return _Value;
            }
        }
        
        private string _Encoding;
        public string Encoding
        {
            set
            {
                Load();
                SetSharedProperty(ref _Encoding, value);
            }
            get
            {
                Load();
                return _Encoding;
            }
        }
        
        public StringAsset()
        {
            _Value = new LocaleString(this);
            _Value.PropertyChanged += Value_PropertyChanged;
            _Encoding = "utf-8";
        }

        public StringAsset(StringAsset other, bool content)
            : base(other, content)
        {
            other.Load();

            _Value = new LocaleString(this, other._Value, content, true);
            _Value.PropertyChanged += Value_PropertyChanged;

            _Encoding = other._Encoding;
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Value":
                    OnPropertyChanged("Description");
                    break;
                case "Locale":
                    if (AssetManager.Instance.RetrieveEnabled)
                    {
                        using (new AssetRetrieveHolder())
                        {
                            foreach (StringAsset sibling in GetSiblings())
                            {
                                sibling.Load();

                                sibling._Value.Locale = _Value.Locale;
                            }
                        }
                    }
                    break;
            }
        }

        public override AssetType Type => AssetType.String;
        public override Asset Clone(bool content) => new StringAsset(this, content);
        public override string Description => Value.Value;

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (StringAsset)asset;

            other.Load();

            return _Encoding == other._Encoding && _Value.Locale == other._Value.Locale;
        }

        public override void Import(string path)
        {
            Load();

            _Value.Value = File.ReadAllText(path);
        }

        public override void Export(string dirpath)
        {
            Load();

            var path = Path.Combine(dirpath, $"{Name}.txt");

            File.WriteAllText(path, _Value.Value);
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        _Value.Build(writer, _Encoding);
                    }
                }
            }
            else _Value.Build(writer, _Encoding);
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("stringAsset");
                writer.WriteAttribute("version", Updater.AssetVersion); 
                writer.WriteAttribute("encoding", _Encoding);
                writer.WriteString(_Value.Save());
                writer.WriteEndElement();
            }
            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "stringAsset") throw new XmlException();

                Updater.ValidateStringAsset(node);

                Encoding = node.ReadAttributeString("encoding");

                _Value.Load(node.InnerText);
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            strings.Add(_Value);
        }
    }
}

