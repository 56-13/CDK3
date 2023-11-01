using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void TrailFix(XmlNode node, int version)
        {
            if (version < 30)
            {
                node.AddAttribute("localSpace", "False");
                node.AddAttribute("colorDuration", "0");
            }
            else if (version < 34)
            {
                node.AddAttribute("colorDuration", node.Attributes["colorInterval"].Value);
                node.RemoveAttribute("colorInterval");
            }
            if (version < 34)
            {
                node.AddAttribute("colorLoop", "None");
            }
            if (version < 39)
            {
                node.AddAttribute("repeatScroll", "0");
                node.AddAttribute("pointLife", node.Attributes["trailInterval"].Value);
                node.RemoveAttribute("trailInterval");
            }
            if (version < 40)
            {
                node.AddAttribute("imageLoop", "Rewind");
            }
            if (version < 64)
            {
                node.AddAttribute("billboard", "True");
            }
        }

        private static void TrailAssetFix(XmlNode node, int version)
        {
            if (version >= 64)
            {
                return;
            }
            TrailFix(node.ChildNodes[1], version);
        }
    }
}
