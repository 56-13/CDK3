using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementInvert : SpriteElement
    {
        private bool _X;
        public bool X
        {
            set => SetProperty(ref _X, value);
            get => _X;
        }

        private bool _Y;
        public bool Y
        {
            set => SetProperty(ref _Y, value);
            get => _Y;
        }

        private bool _Z;
        public bool Z
        {
            set => SetProperty(ref _Z, value);
            get => _Z;
        }

        public SpriteElementInvert()
        {
            
        }

        public SpriteElementInvert(SpriteElementInvert other)
        {
            _X = other._X;
            _Y = other._Y;
            _Z = other._Z;
        }

        public override SpriteElementType Type => SpriteElementType.Invert;
        public override SpriteElement Clone() => new SpriteElementInvert(this);

        private Matrix4x4 GetTransform()
        {
            return Matrix4x4.CreateScale(new Vector3(_X ? -1 : 1, _Y ? -1 : 1, _Z ? -1 : 1));
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            param.Transform = GetTransform() * param.Transform;
            return false;
        }

        internal override bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result)
        {
            param.Transform = GetTransform() * param.Transform;
            return false;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            param.Transform = GetTransform() * param.Transform;
            return false;
        }

        internal override void Draw(ref DrawParam param)
        {
            param.Graphics.Transform(GetTransform());
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("invert");
            writer.WriteAttribute("x", _X);
            writer.WriteAttribute("y", _Y);
            writer.WriteAttribute("z", _Z);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            X = node.ReadAttributeBool("x");
            Y = node.ReadAttributeBool("y");
            Z = node.ReadAttributeBool("z");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_X);
            writer.Write(_Y);
            writer.Write(_Z);
        }
    }
}
