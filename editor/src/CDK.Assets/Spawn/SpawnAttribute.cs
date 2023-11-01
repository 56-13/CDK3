using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace CDK.Assets.Spawn
{
    public class SpawnAttribute : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public SpawnAsset Asset { private set; get; }

        private Dictionary<string, LocaleString> _values;

        public SpawnAttribute(AssetElement parent, SpawnAsset asset)
        {
            Parent = parent;

            Asset = asset;
            Asset.Attribute.Elements.AddWeakListChanged(Elements_ListChanged);

            _values = new Dictionary<string, LocaleString>();
        }

        public SpawnAttribute(AssetElement parent, SpawnAttribute other)
        {
            Parent = parent;

            AssetManager.Instance.InvokeRedirection(() =>
            {
                Asset = AssetManager.Instance.GetRedirection(other.Asset);
                Asset.Attribute.Elements.AddWeakListChanged(Elements_ListChanged);
            });

            _values = new Dictionary<string, LocaleString>();

            foreach (var v in other._values)
            {
                _values.Add(v.Key, new LocaleString(this, v.Value, true, true));
            }
        }

        private void Elements_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.PropertyDescriptor != null && e.PropertyDescriptor.Name == "Locale")
            {
                var element = Asset.Attribute.Elements[e.NewIndex];

                switch (e.PropertyDescriptor.Name)
                {
                    case "Locale":
                        if (_values.TryGetValue(element.Key, out var value)) value.Locale = element.Locale;
                        OnPropertyChanged(element.Key);
                        break;
                    case "Value":
                        OnPropertyChanged(element.Key);
                        break;
                }
            }
        }

        public void SetValue(string key, string value)
        {
            if (_values.TryGetValue(key, out var str)) str.Value = value;
            else
            {
                var element = Asset.Attribute.Elements.FirstOrDefault(e => e.Key == key);

                if (element != null)
                {
                    str = new LocaleString(this, element.Locale)
                    {
                        Value = value
                    };
                    _values.Add(key, str);
                }
            }
            OnPropertyChanged(key);
        }

        public string GetValue(string key)
        {
            if (_values.TryGetValue(key, out var str))
            {
                var value = str.Value;

                if (!string.IsNullOrEmpty(value)) return value;
            }

            var element = Asset.Attribute.Elements.FirstOrDefault(e => e.Key == key);

            return element?.Value;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("attributes");
            foreach (var element in Asset.Attribute.Elements)
            {
                if (_values.TryGetValue(element.Key, out var str))
                {
                    writer.WriteStartElement("attributeValue");
                    writer.WriteAttribute("key", element.Key);
                    writer.WriteAttribute("value", str.Save());
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "attributes") throw new XmlException();

            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName != "attributeValue") throw new XmlException();

                var key = subnode.ReadAttributeString("key");

                var element = Asset.Attribute.Elements.FirstOrDefault(e => e.Key == key);

                if (element != null)
                {
                    if (!_values.TryGetValue(key, out var str))
                    {
                        str = new LocaleString(this, element.Locale);
                        _values.Add(key, str);
                    }

                    str.Load(subnode.ReadAttributeString("value"));
                }
            }
        }

        internal void Build(BinaryWriter writer)
        {
            foreach (var element in Asset.Attribute.Elements)
            {
                if (_values.TryGetValue(element.Key, out var str)) element.Build(writer, str);
                else element.Build(writer);
            }
        }
    }
}
