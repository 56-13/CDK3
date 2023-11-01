using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementScale : SpriteElement
    {
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }

        private bool _Each;
        public bool Each
        {
            set => SetProperty(ref _Each, value);
            get => _Each;
        }


        public SpriteElementScale()
        {
            X = new AnimationFloat(this, -0.01f, 100, 1);
            Y = new AnimationFloat(this, -0.01f, 100, 1);
            Z = new AnimationFloat(this, -0.01f, 100, 1);
        }

        public SpriteElementScale(SpriteElementScale other)
        {
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            _Each = other._Each;
        }

        public override SpriteElementType Type => SpriteElementType.Scale;
        public override SpriteElement Clone() => new SpriteElementScale(this);

        private Vector3 GetScale(float progress, int random)
        {
            if (_Each)
            {
                return new Vector3(
                    X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0)),
                    Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1)),
                    Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2)));
            }
            else
            {
                return new Vector3(X.GetValue(progress, RandomUtil.ToFloat(random)));
            }
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            param.Transform = Matrix4x4.CreateScale(GetScale(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if (X.IsAnimating || Y.IsAnimating || Z.IsAnimating) outflags |= UpdateFlags.Transform;
        }

        internal override bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result)
        {
            param.Transform = Matrix4x4.CreateScale(GetScale(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            param.Transform = Matrix4x4.CreateScale(GetScale(param.Progress, param.Random)) * param.Transform;
            return false;
        }

        internal override void Draw(ref DrawParam param)
        {
            param.Graphics.Scale(GetScale(param.Progress, param.Random));
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("scale");
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            writer.WriteAttribute("each", _Each);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Each = node.ReadAttributeBool("each");
        }

        internal override void Build(BinaryWriter writer)
        {
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            writer.Write(_Each);
        }
    }
}
