using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.Triggers
{
    public class TriggerAsset : Asset
    {
        private Trigger _Trigger;
        public Trigger Trigger
        {
            get
            {
                Load();
                return _Trigger;
            }
        }

        public TriggerAsset()
        {
            _Trigger = new Trigger(this);
            _Trigger.PropertyChanged += Trigger_PropertyChanged;
        }

        public TriggerAsset(TriggerAsset other, bool content) : base(other, content)
        {
            other.Load();

            _Trigger = new Trigger(this, other._Trigger, content);
            _Trigger.PropertyChanged += Trigger_PropertyChanged;
        }

        private void Trigger_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Format":
                    if (AssetManager.Instance.RetrieveEnabled)
                    {
                        using (new AssetRetrieveHolder())
                        {
                            foreach (TriggerAsset sibling in GetSiblings())
                            {
                                sibling.Load();
                                sibling._Trigger.Format = _Trigger.Format;
                            }
                        }
                    }
                    break;
                case "Dimension":
                    if (AssetManager.Instance.RetrieveEnabled)
                    {
                        using (new AssetRetrieveHolder())
                        {
                            foreach (TriggerAsset sibling in GetSiblings())
                            {
                                sibling.Load();
                                sibling._Trigger.Dimension = _Trigger.Dimension;
                            }
                        }
                    }
                    break;
            }
        }
        public override AssetType Type => AssetType.Trigger;
        public override Asset Clone(bool content) => new TriggerAsset(this, content);
        internal override void AddRetains(ICollection<string> retains) => Trigger.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Trigger.IsRetaining(element, out from);

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (TriggerAsset)asset;

            other.Load();

            return _Trigger.Compare(other._Trigger);
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
                        _Trigger.Build(writer);
                    }
                }
            }
            else _Trigger.Build(writer);
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("triggerAsset");
                writer.WriteAttributeString("version", Updater.AssetVersion.ToString()); 

                _Trigger.Save(writer);

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

                if (node.LocalName != "triggerAsset") throw new XmlException();

                Updater.ValidateTriggerAsset(node);

                _Trigger.Load(node.ChildNodes[0]);
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            _Trigger.GetLocaleStrings(strings);
        }
    }
}
