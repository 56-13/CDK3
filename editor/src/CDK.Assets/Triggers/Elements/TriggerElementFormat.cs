using System;
using System.Xml;

namespace CDK.Assets.Triggers
{
    public class TriggerElementFormat
    {
        private XmlNode node;

        public TriggerElementFormat(XmlNode node)
        {
            this.node = node;

            if (node.Attributes["name"] == null) throw new XmlException();
        }

        public TriggerElement Create(TriggerUnit unit)
        {
            var name = node.LocalName;

            name = name.Substring(0, 1).ToUpper() + name.Substring(1);

            var type = (TriggerElementType)Enum.Parse(typeof(TriggerElementType), name);

            return TriggerElement.Create(type, unit, node);
        }

        internal void Write(XmlWriter writer)
        {
            node.WriteTo(writer);
        }
    }
}
