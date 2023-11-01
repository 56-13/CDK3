using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementTranslate : SpriteElement
    {
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }

        public SpriteElementTranslate()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
        }

        public SpriteElementTranslate(SpriteElementTranslate other)
        {
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
        }

        public override SpriteElementType Type => SpriteElementType.Translate;
        public override SpriteElement Clone() => new SpriteElementTranslate(this);

        private Vector3 GetTranslation(float progress, int random)
        {
            return new Vector3(
                X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0)),
                Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1)),
                Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2)));
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            param.Transform = Matrix4x4.CreateTranslation(GetTranslation(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if (X.IsAnimating || Y.IsAnimating || Z.IsAnimating) param.Inflags |= UpdateFlags.Transform;
        }

        internal override bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result)
        {
            param.Transform = Matrix4x4.CreateTranslation(GetTranslation(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            param.Transform = Matrix4x4.CreateTranslation(GetTranslation(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override void Draw(ref DrawParam param)
        {
            param.Graphics.Translate(GetTranslation(param.Progress, param.Random));
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("translate");
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
        }

        internal override void Build(BinaryWriter writer)
        {
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
        }
    }
}
