using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementGradientRect : SpriteElement
    {
        private Vector3 _Position;
        public Vector3 Position
        {
            set => SetProperty(ref _Position, value);
            get => _Position;
        }
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }
        public AnimationFloat Width { private set; get; }
        public AnimationFloat Height { private set; get; }
        public AnimationColor Color0 { private set; get; }
        public AnimationColor Color1 { private set; get; }

        private Align _Align;
        public Align Align
        {
            set => SetProperty(ref _Align, value);
            get => _Align;
        }

        private bool _Horizontal;
        public bool Horizontal
        {
            set => SetProperty(ref _Horizontal, value);
            get => _Horizontal;
        }

        private bool _Fill;
        public bool Fill
        {
            set => SetProperty(ref _Fill, value);
            get => _Fill;
        }

        public Material Material { private set; get; }

        public SpriteElementGradientRect()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            Width = new AnimationFloat(this, 0, 10000, 100);
            Height = new AnimationFloat(this, 0, 10000, 100);
            Color0 = new AnimationColor(this, true, true, Color4.White);
            Color1 = new AnimationColor(this, true, true, Color4.White);

            _Align = Align.CenterMiddle;

            Material = new Material(this, MaterialUsage.Mesh)
            {
                Shader = MaterialShader.NoLight
            };
        }

        public SpriteElementGradientRect(SpriteElementGradientRect other)
        {
            _Position = other._Position;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            Width = new AnimationFloat(this, other.Width);
            Height = new AnimationFloat(this, other.Height);
            Color0 = new AnimationColor(this, other.Color0);
            Color1 = new AnimationColor(this, other.Color1);

            _Align = other._Align;
            _Horizontal = other._Horizontal;
            _Fill = other._Fill;

            Material = new Material(this, other.Material, MaterialUsage.Mesh);
        }

        public override SpriteElementType Type => SpriteElementType.GradientRect;
        public override SpriteElement Clone() => new SpriteElementGradientRect(this);
        internal override void AddRetains(ICollection<string> retains) => Material.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Material.IsRetaining(element, out from);

        private ZRectangle GetRect(float progress, int random)
        {
            var rect = new ZRectangle();
            rect.Origin = _Position;
            rect.X += X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            rect.Y += Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            rect.Z += Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));
            rect.Width = Width.GetValue(progress, RandomUtil.ToFloatSequenced(random, 3));
            rect.Height = Height.GetValue(progress, RandomUtil.ToFloatSequenced(random, 4));
            rect.Origin = _Align.Adjust(rect.Origin, rect.Width, rect.Height);
            return rect;
        }

        private void GetColor(float progress, int random, out Color4 color0, out Color4 color1)
        { 
            color0 = Color0.GetColor(progress, new Color4(
                RandomUtil.ToFloatSequenced(random, 5),
                RandomUtil.ToFloatSequenced(random, 6),
                RandomUtil.ToFloatSequenced(random, 7),
                RandomUtil.ToFloatSequenced(random, 8)));

            color1 = Color1.GetColor(progress, new Color4(
                RandomUtil.ToFloatSequenced(random, 9),
                RandomUtil.ToFloatSequenced(random, 10),
                RandomUtil.ToFloatSequenced(random, 11),
                RandomUtil.ToFloatSequenced(random, 12)));
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            var rect = GetRect(param.Progress, param.Random);

            result.Append(Vector3.Transform(rect.LeftTop, param.Transform));
            result.Append(Vector3.Transform(rect.RightTop, param.Transform));
            result.Append(Vector3.Transform(rect.LeftBottom, param.Transform));
            result.Append(Vector3.Transform(rect.RightTop, param.Transform));

            return true;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 || X.IsAnimating || Y.IsAnimating || Z.IsAnimating || Width.IsAnimating || Height.IsAnimating) outflags |= UpdateFlags.AABB;
        }

        internal override ShowFlags ShowFlags => Material.ShowFlags;

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible && Material.Apply(param.Graphics, param.Progress, param.Random, param.Layer, false))
            {
                var rect = GetRect(param.Progress, param.Random);

                if (param.Layer != InstanceLayer.Cursor)
                {
                    GetColor(param.Progress, param.Random, out var c0, out var c1);

                    if (_Horizontal) param.Graphics.DrawGradientRectH(rect, c0, c1, _Fill);
                    else param.Graphics.DrawGradientRectV(rect, c0, c1, _Fill);
                }
                else
                {
                    param.Graphics.DrawRect(rect, false);
                }
            }
        }
        internal override bool MouseMove(SpriteObject parent, MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey && !shiftKey)
            {
                Position += parent.Scene.Camera.Right * (e.X - prevX) - parent.Scene.Camera.Up * (e.Y - prevY);
                return true;
            }
            return false;
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("gradientRect");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Width.Save(writer, "width");
            Height.Save(writer, "height");
            Color0.Save(writer, "color0");
            Color1.Save(writer, "color1");
            writer.WriteAttribute("align", _Align, Align.CenterMiddle);
            writer.WriteAttribute("horizontal", _Horizontal);
            writer.WriteAttribute("fill", _Fill);
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Width.Load(node, "width");
            Height.Load(node, "height");
            Color0.Load(node, "color0");
            Color1.Load(node, "color1");
            Align = node.ReadAttributeEnum("align", Align.CenterMiddle);
            Horizontal = node.ReadAttributeBool("horizontal");
            Fill = node.ReadAttributeBool("fill");
            Material.Load(node);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            Width.Build(writer, false);
            Height.Build(writer, false);
            Color0.Build(writer);
            Color1.Build(writer);
            writer.Write((byte)_Align);
            writer.Write(_Horizontal);
            writer.Write(_Fill);
            Material.Build(writer);
        }
    }
}
