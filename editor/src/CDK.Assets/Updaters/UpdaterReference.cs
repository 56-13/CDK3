using System;
using System.Xml;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void ReferenceAssetFix(XmlNode node, int version)
        {
            if (version < 49)
            {
                node.AddAttribute("allowEmpty", "False");
            }
        }
    }
}
