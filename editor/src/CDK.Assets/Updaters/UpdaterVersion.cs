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
        private static void VersionAssetFix(XmlNode node, int version)
        {
            if (version < 42)
            {
                XmlNode schemesnode = null;

                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "entries":
                            foreach (XmlNode entrynode in subnode.ChildNodes)
                            {
                                string schemes = entrynode.Attributes["schemes"].Value;
                                entrynode.RemoveAttribute("schemes");
                                entrynode.AddAttribute("local", schemes.Equals("1") ? "False" : "True");
                            }
                            break;
                        case "schemes":
                            schemesnode = subnode;
                            break;
                    }
                }
                node.RemoveChild(schemesnode);
            }
            if (version < 47)
            {
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "entries":
                            foreach (XmlNode entrynode in subnode.ChildNodes)
                            {
                                entrynode.AddAttribute("multiPlatforms", "False");
                            }
                            break;
                    }
                }
            }
            if (version < 64)
            {
                node.AddAttribute("format", "1");
            }
        }
    }
}
