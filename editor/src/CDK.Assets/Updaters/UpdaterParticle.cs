using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void ParticleFix(XmlNode node, int version)
        {
            if (version < 34)
            {
                node.AddAttribute("finish", "True");
            }
        }

        private static void ParticleAssetFix(XmlNode node, int version)
        {
            if (version >= 34)
            {
                return;
            }
            else if (version < 28)
            {
                throw new XmlException("Unsupported version");
            }
            ParticleFix(node.ChildNodes[1], version);
        }
    }
}
