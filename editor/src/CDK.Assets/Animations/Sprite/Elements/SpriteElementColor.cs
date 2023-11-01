using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementColor : SpriteElement
    {
        public AnimationColor Color { private set; get; }

        public SpriteElementColor()
        {
            Color = new AnimationColor(this, true, true, Color4.White);
        }

        public SpriteElementColor(SpriteElementColor other)
        {
            Color = new AnimationColor(this, other.Color);
        }

        public override SpriteElementType Type => SpriteElementType.Color;
        public override SpriteElement Clone() => new SpriteElementColor(this);

        private Color4 GetColor(float progress, int random)
        {
            return Color.GetColor(progress, new Color4(
                        RandomUtil.ToFloatSequenced(random, 0),
                        RandomUtil.ToFloatSequenced(random, 1),
                        RandomUtil.ToFloatSequenced(random, 2),
                        RandomUtil.ToFloatSequenced(random, 3)));
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
                    param.Graphics.Color = GetColor(param.Progress, param.Random);
                    break;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("color");
            Color.Save(writer, "color");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Color.Load(node, "color");
        }

        internal override void Build(BinaryWriter writer)
        {
            Color.Build(writer);
        }
    }
}
