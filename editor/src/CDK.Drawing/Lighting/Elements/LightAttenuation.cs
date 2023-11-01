using System.IO;

namespace CDK.Drawing
{
    public class LightAttenuation
    {
        public string Name { private get; set; }
        public float Range { private set; get; }
        public float Constant { private set; get; }
        public float Linear { private set; get; }
        public float Quadratic { private set; get; }

        private LightAttenuation(string name, float range, float constant, float linear, float quadratic)
        {
            Name = name;
            Range = range;
            Constant = constant;
            Linear = linear;
            Quadratic = quadratic;
        }

        public void Build(BinaryWriter writer)
        {
            writer.Write(Range);
            writer.Write(Constant);
            writer.Write(Linear);
            writer.Write(Quadratic);
        }

        public override string ToString() => Name;

        public static readonly LightAttenuation[] Items = new LightAttenuation[]
        {
            new LightAttenuation("None", 10000, 1, 0, 0),       //no attenuation
            new LightAttenuation("3250", 3250, 1, 0.0014f, 0.000007f),
            new LightAttenuation("600", 600, 1, 0.007f, 0.0002f),
            new LightAttenuation("325", 325, 1, 0.014f, 0.0007f),
            new LightAttenuation("200", 200, 1, 0.022f, 0.0019f),
            new LightAttenuation("160", 160, 1, 0.027f, 0.0028f),
            new LightAttenuation("100", 100, 1, 0.045f, 0.0075f),
            new LightAttenuation("65", 65, 1, 0.07f, 0.017f),
            new LightAttenuation("50", 50, 1, 0.09f, 0.032f),
            new LightAttenuation("32", 32, 1, 0.14f, 0.07f),
            new LightAttenuation("20", 20, 1, 0.22f, 0.20f),
            new LightAttenuation("13", 13, 1, 0.35f, 0.44f),
            new LightAttenuation("7", 7, 1, 0.7f, 1.8f)
        };
    }
}
