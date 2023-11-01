using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Triggers
{
    public class ColorTriggerElement : TriggerElement
    {
        private Color4 _Color;
        public Color4 Color
        {
            set => SetProperty(ref _Color, value);
            get => _Color;
        }

        public ColorTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            _Color = Color4.White;
        }

        public override TriggerElementType Type => TriggerElementType.Color;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            Color = ((ColorTriggerElement)src).Color;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "color")
            {
                Color = node.ReadAttributeColor4("color", true);
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("color");
            writer.WriteAttribute("name", Name);
            writer.WriteAttribute("color", _Color, true, Color4.White);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Color, true);
        }

        internal override int Size => 4;
        public override string ToString() => _Color.ToString();
    }
}
