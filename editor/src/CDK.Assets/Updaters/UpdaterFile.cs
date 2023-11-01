using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void FileAssetFix(XmlNode node, int version)
        {
            if (version < 20)
            {
                node.AddAttribute("revision", "0");
            }
            else if (version < 21)
            {
                node.Attributes["revision"].Value = "0";
            }
            if (version < 47)
            {
                node.AddAttribute("multiPlatforms", "False");
            }
        }
    }
}
