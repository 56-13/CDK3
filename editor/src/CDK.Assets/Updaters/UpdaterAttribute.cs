using System;
using System.Xml;

using CDK.Assets.Support;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void AttributeAssetFix(XmlNode node, int version)
        {
            if (version >= 30)
            {
                return;
            }
            if (version < 30)
            {
                node.AddAttribute("keySeed", node.ChildNodes.Count.ToString());
                node.AddAttribute("isBuildChecked", "True");
            }
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode subnode = node.ChildNodes[i];

                if (version < 30)
                {
                    subnode.AddAttribute("key", (i + 1).ToString());
                }
                if (version < 20)
                {
                    string value = subnode.Attributes["value"].Value;

                    if (version < 8)
                    {
                        value = value.Replace(";", ",");
                    }
                    subnode.Attributes["value"].Value = CSV.Make(new string[] { value });
                }
            }
        }
    }
}
