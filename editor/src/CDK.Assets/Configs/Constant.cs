using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Configs
{
    public class Constant
    {
        public string Name { private set; get; }
        public string Value { private set; get; }
        public Color4 Color { private set; get; }

        public Constant(XmlNode node)
        {
            Name = node.ReadAttributeString("name");
            Value = node.ReadAttributeString("value");
            Color = node.ReadAttributeColor4("color", true, Color4.Black);
        }
    }
}
