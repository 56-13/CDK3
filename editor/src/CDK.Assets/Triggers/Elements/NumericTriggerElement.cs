using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Attributes;

namespace CDK.Assets.Triggers
{
    public class NumericTriggerElement : TriggerElement
    {
        private decimal _Value;
        public decimal Value
        {
            set => SetProperty(ref _Value, value);
            get => _Reference != null ? _Reference.GetConvertedNumericValue() : _Value;
        }

        private decimal _emptyValue;

        private AttributeElement _Reference;
        public AttributeElement Reference
        {
            set
            {
                var prev = _Reference;
                if (SetProperty(ref _Reference, value))
                {
                    prev?.RemoveWeakPropertyChanged(Reference_PropertyChanged);
                    _Reference?.AddWeakPropertyChanged(Reference_PropertyChanged);

                    OnPropertyChanged("Value");
                }
            }
            get => _Reference;
        }

        private void Reference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Value");
        }

        public NumericElementType Boundary { private set; get; }
        public int DecimalPlaces { private set; get; }
        public decimal Min { private set; get; }
        public decimal Max { private set; get; }

        public NumericTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            Boundary = node.ReadAttributeEnum<NumericElementType>("boundary");

            _emptyValue = _Value = node.ReadAttributeDecimal("value");

            Min = node.ReadAttributeDecimal("min", Boundary.MinValue());
            Max = node.ReadAttributeDecimal("max", Boundary.MaxValue());
            DecimalPlaces = node.ReadAttributeInt("decimalPlaces");

            if (_Value < Min) _Value = Min;
            else if (_Value > Max) _Value = Max;
        }

        public override TriggerElementType Type => TriggerElementType.Numeric;

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Reference != null) retains.Add(_Reference.Owner.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Reference != null && element.Contains(_Reference))
            {
                from = this;
                return true;
            }
            from = null;
            return false;
        }

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var other = (NumericTriggerElement)src;

            Value = other.Value;
            AssetManager.Instance.InvokeRedirection(() =>
            {
                Reference = AssetManager.Instance.GetRedirection(other.Reference);
            });
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "numeric")
            {
                Value = node.ReadAttributeDecimal("value", _emptyValue);

                AttributeElement reference = null;

                var str = node.ReadAttributeString("reference");

                if (str != null)
                {
                    var index = str.LastIndexOf('/');

                    var asset = (AttributeAsset)AssetManager.Instance.GetAsset(str.Substring(0, index));

                    if (asset != null)
                    {
                        var subkey = str.Substring(index + 1);

                        reference = asset.Attribute.Elements.FirstOrDefault(e => e.Key == subkey);
                    }
                }
                Reference = reference;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("numeric");
            writer.WriteAttribute("name", Name);
            if (_Reference != null) writer.WriteAttribute("reference", $"{_Reference.Owner.Key}/{_Reference.Key}");
            else writer.WriteAttribute("value", _Value, _emptyValue);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            var value = Value;
            if (value < Min || value > Max)
            {
                throw new AssetException(Owner, "범위에 맞지 않는 값이 있습니다.");
            }
            writer.Write(Boundary, value);
        }

        internal override int Size => Boundary.GetSize();
        public override bool IsEmpty => Value == _emptyValue;

        public override string ToString()
        {
            var strbuf = new StringBuilder();
            strbuf.Append(Value.ToString("0.######"));
            if (_Reference != null)
            {
                strbuf.Append('(');
                strbuf.Append(_Reference.Owner.Tags);
                strbuf.Append('.');
                strbuf.Append(_Reference.Name);
                strbuf.Append(')');
            }
            return strbuf.ToString();
        }
    }
}
