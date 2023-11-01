using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void SpriteFix(XmlNode node, int version)
        {
            foreach (XmlNode layerNode in node.ChildNodes)
            {
                foreach (XmlNode layerFrameNode in layerNode.ChildNodes)
                {
                    foreach (XmlNode elementNode in layerFrameNode.ChildNodes)
                    {
                        switch (elementNode.LocalName)
                        {
                            case "string":
                                if (version < 63)
                                {
                                    bool usingSize = bool.Parse(elementNode.Attributes["usingSize"].Value);
                                    elementNode.RemoveAttribute("usingSize");
                                    elementNode.AddAttribute("usingSize", usingSize ? "Both" : "None");
                                }
                                break;
                            case "blur":
                                if (version < 50) elementNode.AddAttribute("intensity", elementNode.Attributes["weight"].Value);
                                if (version < 60)
                                {
                                    elementNode.AddAttribute("blurType", "Intensity");
                                    elementNode.AddAttribute("direction", "10,10");
                                    elementNode.AddAttribute("center", "0,0,0");
                                    elementNode.AddAttribute("range", "100");
                                }
                                break;
                            case "image":
                                if (version < 55)
                                {
                                    if (elementNode.Attributes["shadow"] != null)
                                    {
                                        string[] ps = elementNode.Attributes["position"].Value.Split(',');
                                        elementNode.RemoveAttribute("position");
                                        elementNode.AddAttribute("position", string.Format("{0},{1},0", ps[0], ps[1]));

                                        string shadow = elementNode.Attributes["shadow"].Value;
                                        if (version < 54) shadow += ",False,False";

                                        string[] ss = shadow.Split(',');
                                        elementNode.RemoveAttribute("shadow");
                                        elementNode.AddAttribute("shadow", string.Format("Flat,{0},{1},{2},{3},{4}", ps[2], ss[0], ss[1], ss[3], ss[4]));
                                    }
                                }
                                else if (version < 57)
                                {
                                    if (elementNode.Attributes["shadow"] != null)
                                    {
                                        string shadow = elementNode.Attributes["shadow"].Value;
                                        if (version < 56 && shadow.StartsWith("Rotate"))
                                        {
                                            shadow += ",0";
                                        }
                                        string[] ss = shadow.Split(new[] { ',' }, 4);
                                        elementNode.RemoveAttribute("shadow");
                                        elementNode.AddAttribute("shadow", string.Format("{0},{1},{2}", ss[0], ss[1], ss[3]));
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void SpriteAssetFix(XmlNode node, int version)
        {
            if (version < 5)
            {
                throw new XmlException("Unsupported version");
            }
            SpriteFix(node.ChildNodes[1], version);
        }
    }
}
