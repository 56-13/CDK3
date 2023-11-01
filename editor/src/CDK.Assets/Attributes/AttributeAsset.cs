using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.Attributes
{
    public class AttributeAsset : Asset
    {
        private Attribute _Attribute;
        public Attribute Attribute
        {
            get
            {
                Load();
                return _Attribute;
            }
        }

        private bool _BuildChecked;
        public bool BuildChecked
        {
            set
            {
                Load();
                SetSharedProperty(ref _BuildChecked, value);
            }
            get
            {
                Load();
                return _BuildChecked;
            }
        }

        public AttributeAsset()
        {
            _BuildChecked = true;

            _Attribute = new Attribute(this, "Attribute");
        }

        public AttributeAsset(AttributeAsset other, bool content)
            : base(other, content)
        {
            other.Load();

            _BuildChecked = other._BuildChecked;

            _Attribute = new Attribute(this, "Attribute", other._Attribute, content);
        }

        public override AssetType Type => AssetType.Attribute;
        public override Asset Clone(bool content) => new AttributeAsset(this, content);

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (AttributeAsset)asset;

            other.Load();

            if (_BuildChecked != other._BuildChecked) return false;

            return _Attribute.Compare(other._Attribute);
        }

        public string[][] ExportToCells(string[] names = null)
        {
            var assets = GetSiblings().Select(a => (AttributeAsset)a).ToList();
            assets.Add(this);
            assets.Sort(delegate(AttributeAsset x, AttributeAsset y)
            {
                return x.Location.CompareTo(y.Location);
            });

            if (names == null) names = _Attribute.Elements.Select(e => e.Name).ToArray();

            var cells = new string[assets.Count + 1][];
            cells[0] = new string[2 + names.Length];
            cells[0][0] = "key";
            cells[0][1] = "location";
            Array.Copy(names, 0, cells[0], 2, names.Length);

            for (int r = 0; r < assets.Count; r++)
            {
                var asset = assets[r];

                asset.Load();

                var row = cells[r + 1] = new string[cells[0].Length];

                row[0] = asset.Key;
                row[1] = asset.Location;
                for (var c = 2; c < cells[0].Length; c++)
                {
                    var name = cells[0][c];

                    var value = asset._Attribute.Elements.FirstOrDefault(e => e.Name == name)?.Value ?? string.Empty;

                    row[c] = value;
                }
            }
            return cells;
        }

        public void ImportFromCells(string[][] cells)
        {
            var assets = GetSiblings().Select(a => (AttributeAsset)a).ToList();
            assets.Add(this);
            assets.Sort(delegate(AttributeAsset x, AttributeAsset y)
            {
                return x.Location.CompareTo(y.Location);
            });
            for (int r = 1; r < cells.Length; r++)
            {
                AttributeAsset asset = null;
                foreach (var a in assets)
                {
                    if (a.Key.Equals(cells[r][0]))
                    {
                        asset = a;
                        break;
                    }
                }
                if (asset != null)
                {
                    asset.Load();

                    for (var c = 2; c < cells[0].Length; c++)
                    {
                        var name = cells[0][c];

                        var element = asset._Attribute.Elements.FirstOrDefault(e => e.Name == name);

                        if (element != null) element.Value = cells[r][c].Replace("\n", "\r\n").Replace("\r\r", "\r");
                    }
                }
            }
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (_BuildChecked)
            {
                if (writer == null)
                {
                    var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (writer = new BinaryWriter(fs))
                        {
                            _Attribute.Build(writer);
                        }
                    }
                }
                else _Attribute.Build(writer);
            }
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("attributeAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("buildChecked", _BuildChecked, true);
                _Attribute.Save(writer);
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

                if (node.LocalName != "attributeAsset") throw new XmlException();

                Updater.ValidateAttributeAsset(node);

                BuildChecked = node.ReadAttributeBool("buildChecked");

                _Attribute.Load(node.GetChildNode("attribute"));
            }
        }
        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            _Attribute.GetLocaleStrings(strings);
        }
    }
}
