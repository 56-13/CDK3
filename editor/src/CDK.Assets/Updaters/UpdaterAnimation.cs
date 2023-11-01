using System;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Animations;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        public static void AnimationAssetFix(XmlNode node, int version)
        {
            if (version < 30)
            {
                throw new XmlException("Unsupported version");
            }
            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName)
                {
                    case "keys":
                        foreach (XmlNode keyNode in subnode.ChildNodes)
                        {
                            if (version < 35) keyNode.AddAttribute("globalPosition", "None");
                        }
                        break;
                    case "animation":
                        AnimationFix(subnode, version);
                        break;
                }
            }
        }

        public static void AnimationFix(XmlNode node, int version)
        {
            if (version < 34)
            {
                node.AddAttribute("derivationFinish", "True");
            }
            if (version < 35)
            {
                node.RemoveAttribute("globalStartPosition");
                node.RemoveAttribute("globalEndPosition");
            }
            if (version < 33)
            {
                node.AddAttribute("closing", "False");
            }
            if (version < 31)
            {
                node.AddAttribute("isBuildChecked", "True");
            }
            if (version < 43)
            {
                node.AddAttribute("localeVisible", "True");
                node.AddAttribute("locales", "");
            }

            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName){
                    case "sound":
                        if (version < 35) subnode.AddAttribute("essential", "False");
                        break;
                    case "substance":
                        switch (subnode.Attributes["type"].Value)
                        {
                            case "Sprite":
                                SpriteFix(subnode.ChildNodes[0], version);
                                break;
                            case "Particle":
                                ParticleFix(subnode.ChildNodes[0], version);
                                break;
                            case "Trail":
                                TrailFix(subnode.ChildNodes[0], version);
                                break;
                            case "Chain":
                                ChainFix(subnode.ChildNodes[0], version);
                                break;
                            case "Model":
                                ModelFix(subnode.ChildNodes[0], version);
                                break;
                        }
                        break;
                    case "derivation":
                        switch (subnode.Attributes["type"].Value)
                        {
                            case "Random":
                                if (version < 32) subnode.AddAttribute("loop", "True");
                                break;
                            case "Linked":
                                if (version < 39)
                                {
                                    bool loop = bool.Parse(subnode.Attributes["loop"].Value);
                                    subnode.RemoveAttribute("loop");
                                    subnode.AddAttribute("loopCount", loop ? "0" : "1");
                                }
                                break;
                        }
                        foreach (XmlNode derivationNode in subnode.ChildNodes)
                        {
                            AnimationFix(derivationNode, version);
                        }
                        break;
                }
            }
        }
    }
}
