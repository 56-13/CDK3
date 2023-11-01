using System.Globalization;
using System.Xml;

using CDK.Drawing;

namespace CDK.Assets.Updaters
{
    static partial class Updater
    {
        private static void MaterialFix(XmlNode node, string name, int version)
        {
            /*
            string[] ps = node.Attributes[name].Value.Split(',');
            Material material = new Material();
            material.Diffuse = new Color4(uint.Parse(ps[0], NumberStyles.HexNumber));
            material.Specular = float.Parse(ps[1]);
            material.Shininess = float.Parse(ps[2]);
            material.Emission = new Color3(int.Parse(ps[3].Substring(2), NumberStyles.HexNumber));
            node.RemoveAttribute(name);
            node.AddAttribute(name, material.ToString());
            */
        }

        private static void LightFix(XmlNode node, string name, int version)
        {
            /*
            string[] ps = node.Attributes[name].Value.Split(',');
            Light light = new Light();
            light.Ambient = float.Parse(ps[0]);
            light.Color = new Color3(int.Parse(ps[1].Substring(2), NumberStyles.HexNumber));
            light.Color *= float.Parse(ps[2]);
            light.Direction = new Vector3(float.Parse(ps[3]), float.Parse(ps[4]), float.Parse(ps[5]));
            node.RemoveAttribute(name);
            node.AddAttribute(name, light.ToString());
            */
        }
    }
}
