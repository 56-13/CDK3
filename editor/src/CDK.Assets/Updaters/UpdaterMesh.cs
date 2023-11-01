using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void MeshAssetFix(XmlNode node, int version)
        {
            
        }

        private static void ModelFix(XmlNode node, int version)
        {
            if (version < 57)
            {
                node.AddAttribute("ambient", "FFFFFFFF");
                node.AddAttribute("color", "1");
                node.AddAttribute("mesh", node.Attributes["meshAsset"].Value);
                node.RemoveAttribute("meshAsset");
                node.AddAttribute("shadow", "False");
            }

            string[] textures = null;

            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (version < 57 && subnode.LocalName == "images")
                {
                    textures = new string[subnode.ChildNodes.Count];
                    int i = 0;
                    foreach (XmlNode imageNode in subnode.ChildNodes)
                    {
                        textures[i++] = imageNode.Attributes["image"].Value;
                    }
                }
                else if (subnode.LocalName == "materials")
                {
                    int i = 0;
                    foreach (XmlNode materialNode in subnode.ChildNodes)
                    {
                        if (version < 57)
                        {
                            if (materialNode.Attributes["texture"] != null)
                            {
                                int textureIndex = int.Parse(materialNode.Attributes["texture"].Value);
                                materialNode.RemoveAttribute("texture");
                                materialNode.AddAttribute("texture", textures[textureIndex]);
                            }
                            materialNode.AddAttribute("name", string.Format("Material {0}", i));
                            materialNode.AddAttribute("usingOpacity", "False");
                        }
                        i++;
                    }
                }
            }
        }

        private static void ModelAssetFix(XmlNode node, int version)
        {
            ModelFix(node.ChildNodes[1], version);
        }
    }
}
