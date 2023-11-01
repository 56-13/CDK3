using System.Xml;

using CDK.Assets.Support;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void StringAssetFix(XmlNode node, int version)
        {
            if (version < 20)
            {
                node.InnerText = CSV.Make(new string[] { node.InnerText });
            }
        }
    }
}
