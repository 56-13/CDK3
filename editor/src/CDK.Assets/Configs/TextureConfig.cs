using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Configs
{
    public class TextureConfig
    {
        public RawFormat[] CompressedRgba { private set; get; }
        public RawFormat[] CompressedRgb { private set; get; }
        public RawFormat[] CompressedSrgbA { private set; get; }
        public RawFormat[] CompressedSrgb { private set; get; }

        public TextureConfig(XmlNode node)
        {
            CompressedRgba = node.ReadAttributeEnums<RawFormat>("compressedRgba");
            CompressedRgb = node.ReadAttributeEnums<RawFormat>("compressedRgb");
            CompressedSrgbA = node.ReadAttributeEnums<RawFormat>("compressedSrgbA");
            CompressedSrgb = node.ReadAttributeEnums<RawFormat>("compressedSrgb");
        }
    }
}
