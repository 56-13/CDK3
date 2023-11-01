using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class StringTriggerElement : TriggerElement
    {
        public LocaleString Value { private set; get; }
        public string Encoding { private set; get; }

        public StringTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            Value = new LocaleString(this, node.ReadAttributeBool("locale"));

            Encoding = node.ReadAttributeString("encoding");
        }

        public override TriggerElementType Type => TriggerElementType.String;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var other = ((StringTriggerElement)src).Value;
            Value.Copy(other, isNew, false);
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "string")
            {
                Value.Load(node.ReadAttributeString("value"));
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("string");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value.Save());
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            Value.Build(writer, Encoding);
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            strings.Add(Value);
        }

        internal override int Size => Value.BuildSize(Encoding);
        public override bool IsEmpty => Value.Value == string.Empty;
        public override string ToString() => $"({Value.Value})";
    }
}
