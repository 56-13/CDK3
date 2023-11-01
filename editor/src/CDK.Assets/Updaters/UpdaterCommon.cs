using System.Xml;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void AddAttribute(this XmlNode node, string name, string value)
        {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.SetNamedItem(attr);
        }
        private static void RemoveAttribute(this XmlNode node, string name)
        {
            node.Attributes.RemoveNamedItem(name);
        }
    }
}
