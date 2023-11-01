using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.IO;

namespace CDK.Assets.Attributes
{
    public class AttributeElementItem
    {
        private string _Name;
        public string Name
        {
            set => _Name = value ?? string.Empty;
            get => _Name;
        }
        private string _Value;
        public string Value
        {
            set => _Value = value ?? string.Empty;
            get => _Value;
        }
    }

    public enum AttributeElementListType
    {
        None,
        Mask,
        List
    }

    public class AttributeElement : AssetElement
    {
        public Attribute Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public override string GetLocation() => $"{Parent.GetLocation()}.{_Name}";
        public override IEnumerable<AssetElement> GetSiblings()
        {
            var index = Parent.Elements.IndexOf(this);

            if (index < 0) return new AssetElement[0];

            return Parent.GetSiblings().
                Where(a => ((Attribute)a).Elements.Count == Parent.Elements.Count).
                Select(a => ((Attribute)a).Elements[index]);
        }

        public string Key { private set; get; }

        private string _Name;
        public string Name
        {
            set
            {
                if (SetSharedProperty(ref _Name, value ?? string.Empty)) OnPropertyChanged("NameValue");
            }
            get => _Name;
        }

        public string NameValue => $"{_Name} ({Value})";

        private LocaleString _Value;
        public string Value
        {
            set => _Value.Value = value;
            get => _Value.Value;
        }

        public bool Locale
        {
            set
            {
                if (_Value.Locale != value)
                {
                    _Value.Locale = value;

                    if (AssetManager.Instance.RetrieveEnabled)
                    {
                        using (new AssetRetrieveHolder())
                        {
                            foreach (AttributeElement sibling in GetSiblings()) sibling.Locale = value;
                        }
                    }
                }
            }
            get => _Value.Locale;
        }

        private ElementType _Type;
        public ElementType Type
        {
            set => SetSharedProperty(ref _Type, value);
            get => _Type;
        }

        private string _Description;
        public string Description
        {
            set => SetProperty(ref _Description, value ?? string.Empty);
            get => _Description;
        }

        private AttributeElementListType _ListType;
        public AttributeElementListType ListType
        {
            set => SetSharedProperty(ref _ListType, value);
            get => _ListType;
        }

        private AttributeElementItem[] _Items;
        public AttributeElementItem[] Items
        {
            set => SetSharedProperty(ref _Items, value);
            get => _Items;
        }

        public int Index => Parent.Elements.IndexOf(this);

        public AttributeElement(Attribute parent)
        {
            Parent = parent;

            Key = AssetManager.NewKey();

            _Name = string.Empty;
            _Value = new LocaleString(this);
            _Value.PropertyChanged += Value_PropertyChanged;
            _Description = string.Empty;
        }

        public AttributeElement(Attribute parent, AttributeElement element, bool content)
        {
            Parent = parent;

            Key = AssetManager.NewKey();

            _Name = element._Name;
            _Value = new LocaleString(this, element._Value, content, true);
            _Value.PropertyChanged += Value_PropertyChanged;
            _Type = element._Type;
            _Description = element._Description;
            _ListType = element._ListType;
            _Items = element._Items;

            AssetManager.Instance.AddRedirection(element, this);
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Locale":
                case "Value":
                    OnPropertyChanged(e.PropertyName);
                    OnPropertyChanged("NameValue");
                    break;
            }
        }

        public void ReplaceValue(string from, string to)
        {
            if (Value == from) Value = to;

            foreach (AttributeElement sibling in GetSiblings())
            {
                if (sibling.Value == from) sibling.Value = to;
            }
        }

        internal bool Compare(AttributeElement element)
        {
            if (_Name != element._Name || _Type != element._Type || _ListType != element._ListType || Locale != element.Locale) return false;

            if (_Items == null)
            {
                if (element._Items != null) return false;
            }
            else if (element._Items == null) return false;
            else if (_Items.Length != element._Items.Length) return false;
            else
            {
                for (var i = 0; i < _Items.Length; i++)
                {
                    if (_Items[i].Name != element._Items[i].Name || _Items[i].Value != element._Items[i].Value) return false;
                }
            }
            return true;
        }

        private string ConvertedValue(string value)
        {
            if (_Items != null && value != string.Empty)
            {
                var convertedValue = _Items.FirstOrDefault(i => i.Name == value)?.Value;

                if (convertedValue != null) value = convertedValue;
                else throw new IOException();
            }
            return value;
        }

        internal void Build(BinaryWriter writer) => Build(writer, _Value);

        internal void Build(BinaryWriter writer, LocaleString value)
        {
            switch (_ListType)
            {
                case AttributeElementListType.None:
                    switch (_Type)
                    {
                        case ElementType.String_UTF8:
                            value.Build(writer, "utf-8");
                            break;
                        case ElementType.String_UTF16:
                            value.Build(writer, "utf-16");
                            break;
                        case ElementType.String_UTF16BE:
                            value.Build(writer, "utf-16BE");
                            break;
                        case ElementType.String_UTF32:
                            value.Build(writer, "utf-32");
                            break;
                        case ElementType.String_UTF32BE:
                            value.Build(writer, "utf-32BE");
                            break;
                        case ElementType.None:
                            break;
                        default:
                            {
                                decimal v;

                                var cv = ConvertedValue(Value);
                                if (cv != string.Empty) v = 0;
                                else if (cv.StartsWith("0x")) v = uint.Parse(cv.Substring(2), NumberStyles.HexNumber);
                                else v = decimal.Parse(cv);

                                writer.Write(_Type.As<NumericElementType>(), v);
                            }
                            break;
                    }
                    break;
                case AttributeElementListType.List:
                    {
                        var origin = value.Value;
                        if (origin != string.Empty)
                        {
                            var values = origin.Split(',');
                            writer.WriteLength(values.Length);
                            foreach (var v in values) writer.Write(_Type, ConvertedValue(v));
                        }
                        else writer.WriteLength(0);
                    }
                    break;
                case AttributeElementListType.Mask:
                    {
                        var origin = value.Value;
                        var mask = 0;
                        if (origin != string.Empty)
                        {
                            var values = origin.Split(',');
                            foreach (var v in values) mask |= int.Parse(ConvertedValue(v));
                        }
                        writer.Write(_Type.As<IntegerElementType>(), mask);
                    }
                    break;
            }
        }

        public decimal GetConvertedNumericValue()
        {
            switch (_ListType)
            {
                case AttributeElementListType.None:
                    {
                        decimal v;

                        var value = ConvertedValue(Value);

                        if (value == string.Empty)
                        {
                            v = 0;
                        }
                        else if (value.StartsWith("0x"))
                        {
                            v = uint.Parse(value.Substring(2), NumberStyles.HexNumber);
                        }
                        else
                        {
                            v = decimal.Parse(value);
                        }
                        return v;
                    }
                case AttributeElementListType.Mask:
                    {
                        var origin = Value;

                        var mask = 0;

                        if (origin != string.Empty)
                        {
                            var values = origin.Split(',');
                            foreach (var value in values)
                            {
                                var v = int.Parse(ConvertedValue(value));

                                mask |= v;
                            }
                        }
                        return mask;
                    }
            }
            return 0;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("element");
            writer.WriteAttribute("key", Key);
            writer.WriteAttribute("name", _Name);
            writer.WriteAttribute("value", _Value.Save());
            writer.WriteAttribute("type", _Type);
            writer.WriteAttribute("description", _Description);
            writer.WriteAttribute("listType", _ListType);
            if (_Items != null)
            {
                writer.WriteStartElement("items");
                foreach (var item in _Items)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttribute("name", item.Name);
                    writer.WriteAttribute("value", item.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            using (new AssetRetrieveHolder())
            {
                Key = node.ReadAttributeString("key");

                Name = node.ReadAttributeString("name");

                _Value.Load(node.ReadAttributeString("value"));
                Type = node.ReadAttributeEnum<ElementType>("type");
                Description = node.ReadAttributeString("description");
                ListType = node.ReadAttributeEnum<AttributeElementListType>("listType");

                if (node.ChildNodes.Count > 0 && node.ChildNodes[0].Name.Equals("items"))
                {
                    var items = new AttributeElementItem[node.ChildNodes[0].ChildNodes.Count];
                    var i = 0;
                    foreach (XmlNode subnode in node.ChildNodes[0].ChildNodes)
                    {
                        var item = new AttributeElementItem
                        {
                            Name = subnode.ReadAttributeString("name"),
                            Value = subnode.ReadAttributeString("value")
                        };
                        items[i++] = item;
                    }
                    Items = items;
                }
            }
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            strings.Add(_Value);
        }
    }
}
