using System.Xml;

namespace CDK.Assets.Triggers
{
    public class TriggerElementItem
    {
        public int Code { private set; get; }
        public string Name { private set; get; }

        public TriggerElementItem(XmlNode node, int autoCode)
        {
            Code = node.ReadAttributeInt("code", autoCode);
            Name = node.ReadAttributeString("name");
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("item");
            writer.WriteAttribute("code", Code);
            writer.WriteAttribute("name", Name);
            writer.WriteEndElement();
        }

        public override string ToString() => Name;
    }
}
