using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void ChainFix(XmlNode node, int version)
        {
            if (version < 40)
            {
                node.AddAttribute("imageLoop", "Rewind");
            }
            if (version < 64)
            {
                node.AddAttribute("billboard", "True");
            }
        }
    }
}
