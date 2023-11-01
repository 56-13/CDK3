using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementRotate : SpriteElement
    {
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }

        public SpriteElementRotate()
        {
            X = new AnimationFloat(this, -3600, 3600, 0);
            Y = new AnimationFloat(this, -3600, 3600, 0);
            Z = new AnimationFloat(this, -3600, 3600, 0);
        }

        public SpriteElementRotate(SpriteElementRotate other)
        {
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
        }
        public override SpriteElementType Type => SpriteElementType.Rotate;
        public override SpriteElement Clone() => new SpriteElementRotate(this);

        private Matrix4x4 GetTransform(float progress, int random)
        {
            return Matrix4x4.CreateFromYawPitchRoll(
                Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1)) * MathUtil.ToRadians,
                X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0)) * MathUtil.ToRadians,
                Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2)) * MathUtil.ToRadians);
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result) 
        {
            param.Transform = GetTransform(param.Progress, param.Random) * param.Transform;
            return false;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if (X.IsAnimating || Y.IsAnimating || Z.IsAnimating) param.Inflags |= UpdateFlags.Transform;
        }

        internal override bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result)
        {
            param.Transform = GetTransform(param.Progress, param.Random) * param.Transform;
            return false;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            param.Transform = GetTransform(param.Progress, param.Random) * param.Transform;
            return false;
        }

        internal override void Draw(ref DrawParam param)
        {
            param.Graphics.Transform(GetTransform(param.Progress, param.Random));
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("rotate");
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
            X.Build(writer, true);
            Y.Build(writer, true);
            Z.Build(writer, true);
        }
    }
}
