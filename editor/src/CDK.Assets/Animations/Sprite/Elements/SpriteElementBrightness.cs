using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementBrightness : SpriteElement
    {
        public AnimationFloat Value { private set; get; }

        public SpriteElementBrightness()
        {
            Value = new AnimationFloat(this, 0, 2, 1);
        }

        public SpriteElementBrightness(SpriteElementBrightness other)
        {
            Value = new AnimationFloat(this, other.Value);
        }

        public override SpriteElementType Type => SpriteElementType.Brightness;
        public override SpriteElement Clone() => new SpriteElementBrightness(this);
        
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
                    param.Graphics.Brightness = GetValue(param.Progress, param.Random);
                    break;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("brightness");
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
