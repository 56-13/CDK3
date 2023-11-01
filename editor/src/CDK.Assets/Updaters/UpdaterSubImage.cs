using System;
using System.Xml;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void SubImageAssetFix(XmlNode node, int version)
        {
            if (version < 2)
            {
                throw new XmlException("Unsupported version");
            }
            if (version < 43)
            {
                string frame = node.Attributes["frame"].Value;

                XmlNode subnode = node.OwnerDocument.CreateNode("element", "subImageElement", "");
                subnode.AddAttribute("frame", frame);

                node.AppendChild(subnode);
            }
            if (version < 48)
            {
                node.AddAttribute("padding", "2,2");
                node.AddAttribute("border", "0,0");
            }
            else if (version < 52)
            {
                string[] ps = node.Attributes["padding"].Value.Split(',');
                int paddingX = int.Parse(ps[0]);
                int paddingY = int.Parse(ps[1]);
                if (paddingX == 3 && paddingY == 3)
                {
                    node.RemoveAttribute("padding");
                    node.AddAttribute("padding", "2,2");
                }
            }
        }
    }
}
