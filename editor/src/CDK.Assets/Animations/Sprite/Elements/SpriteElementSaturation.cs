using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementSaturation : SpriteElement
    {
        public AnimationFloat Value { private set; get; }

        public SpriteElementSaturation()
        {
            Value = new AnimationFloat(this, 0, 2, 1);
        }

        public SpriteElementSaturation(SpriteElementSaturation other)
        {
            Value = new AnimationFloat(this, other.Value);
        }

        public override SpriteElementType Type => SpriteElementType.Saturation;
        public override SpriteElement Clone() => new SpriteElementSaturation(this);

        private float GetValue(float progress, int random)
        {
            return Value.GetValue(progress, RandomUtil.ToFloat(random));
        }

        internal override void Draw(ref DrawParam param)
        {
            switch (param.Layer)
            {
                case InstanceLayer.None:
                case InstanceLayer.Base:
                case InstanceLayer.BlendBottom:
                case InstanceLayer.BlendMiddle:
                case InstanceLayer.BlendTop:
                    param.Graphics.Saturation = GetValue(param.Progress, param.Random);
                    break;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("saturation");
            Value.Save(writer, "value");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Value.Load(node, "value");
        }

        internal override void Build(BinaryWriter writer)
        {
            Value.Build(writer, false);
        }
    }
}
