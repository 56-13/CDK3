using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void TerrainAssetFix(XmlNode node, int version)
        {
            /*
            if (version < 30)
            {
                throw new XmlException("Unsupported version");
            }

            if (version < 50)
            {
                node.AddAttribute("occlusion", "3,2");
                node.RemoveAttribute("minimapFormat");
                node.AddAttribute("buildLight", "RawRGB565");
                node.AddAttribute("buildMinimap", "ETC2_PVRTC1_DXT1");
            }
            else if (version < 52)
            {
                node.RemoveAttribute("buildLight");
                node.AddAttribute("buildLight", "RawRGB565");
                node.RemoveAttribute("buildMinimap");
                node.AddAttribute("buildMinimap", "ETC2_PVRTC1_DXT1");
            }
            else if (version < 58)
            {
                node.RemoveAttribute("buildLight");
                node.AddAttribute("buildLight", "RawRGB565");
                node.RemoveAttribute("buildMinimap");
                node.AddAttribute("buildMinimap", "ETC2_PVRTC1_DXT1");
            }

            if (version < 57)
            {
                node.AddAttribute("tileCell", node.Attributes["tileSize"].Value);
                node.RemoveAttribute("tileSize");
            }

            if (version < 62)
            {
                node.AddAttribute("surfaceOffset", "0,0");
            }

            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName)
                {
                    case "mapSurfaces":
                        foreach (XmlNode surfaceNode in subnode.ChildNodes)
                        {
                            if (version < 50) surfaceNode.AddAttribute("material", Material.Default.ToString());
                            else if (version < 51) MaterialFix(surfaceNode, "material", version);
                            if (version < 53) { 
                                surfaceNode.AddAttribute("scale", "1");
                                surfaceNode.AddAttribute("rotation", "0");
                            }
                        }
                        break;
                    case "mapWaters":
                        foreach (XmlNode waterNode in subnode.ChildNodes)
                        {
                            if (version < 50)
                            {
                                waterNode.AddAttribute("perturbIntensity", waterNode.Attributes["perturbWeight"].Value);
                                waterNode.AddAttribute("foamIntensity", waterNode.Attributes["foamWeight"].Value);
                                waterNode.AddAttribute("specIntensity", waterNode.Attributes["specWeight"].Value);
                            }
                        }
                        break;
                }
            }
            */
        }
    }
}
