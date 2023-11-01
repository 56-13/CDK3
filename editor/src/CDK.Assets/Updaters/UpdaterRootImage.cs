using System.Text;
using System.Xml;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        public static string RawFormatFix(string format, int version)
        {
            if (version < 46)
            {
                string[] fs = format.Split(',');

                switch (fs[0])
                {
                    case "RawRGBA5551":
                    case "RawRGBA4444":
                    case "RawRGB565CA":
                    case "ImageRGBA5551":
                    case "ImageRGBA4444":
                    case "ImageRGB565CA":
                        fs[0] = "ETC2_RGBA+PVRTC1_RGBA+DXT5";
                        break;
                    case "RawRGB565":
                    case "ImageRGB565":
                        fs[0] = "ETC2_RGB+PVRTC1_RGB+DXT1";
                        break;
                    case "CRGB":
                        fs[0] = "ETC2_RGB+PVRTC1_RGB+DXT1";
                        break;
                    case "CRGBCA":
                        fs[0] = "ETC2_RGBA+PVRTC1_RGBA+DXT5";
                        break;
                    case "RawL8CA":
                    case "ImageL8CA":
                        fs[0] = "RawL8A8";
                        break;
                }
                return string.Format("{0},{1},{2},{3},{4},{5}", fs[0], fs[2], fs[3], fs[4], fs[5], fs[6]);
            }
            else if (version < 47)
            {
                string[] fs = format.Split(',');

                if (fs[0].Equals("ETC2_RGBA+PVRTC1_RGBA")) fs[0] += "+DXT5";
                else if (fs[0].Equals("ETC2_RGB+PVRTC1_RGB")) fs[0] += "+DXT1";
                return string.Format("{0},{1},{2},{3},{4},{5}", fs[0], fs[2], fs[3], fs[4], fs[5], fs[6]);
            }
            else if (version < 58)
            {
                string[] fs = format.Split(',');

                if (fs[0].Equals("ETC2_RGBA+PVRTC1_RGBA")) fs[0] += "+DXT5";
                else if (fs[0].Equals("ETC2_RGB+PVRTC1_RGB")) fs[0] += "+DXT1";
                return string.Format("{0},{1},{2},{3},{4},{5}", fs[0], fs[1], fs[2], fs[3], fs[4], fs[5]);
            }
            else return format;
        }

        private static void RootImageAssetFix(XmlNode node, int version)
        {
            if (version < 11)
            {
                throw new XmlException("Unsupported version");
            }
            else if (version < 58)
            {
                string format = node.Attributes["format"].Value;
                node.RemoveAttribute("format");
                node.AddAttribute("format", RawFormatFix(format, version));
            }
            if (version < 59)
            {
                node.AddAttribute("hasSubImages", "False");
            }
        }
    }
}
