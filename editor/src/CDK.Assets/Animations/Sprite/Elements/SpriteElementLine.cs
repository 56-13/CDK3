using System;
using System.Numerics;
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
    public class SpriteElementLine : SpriteElement
    {
        private Vector3 _Position0;
        public Vector3 Position0
        {
            set => SetProperty(ref _Position0, value);
            get => _Position0;
        }
        public AnimationFloat X0 { private set; get; }
        public AnimationFloat Y0 { private set; get; }
        public AnimationFloat Z0 { private set; get; }

        private Vector3 _Position1;
        public Vector3 Position1
        {
            set => SetProperty(ref _Position1, value);
            get => _Position1;
        }
        public AnimationFloat X1 { private set; get; }
        public AnimationFloat Y1 { private set; get; }
        public AnimationFloat Z1 { private set; get; }
        public Material Material { private set; get; }

        public SpriteElementLine()
        {
            X0 = new AnimationFloat(this, -10000, 10000, 0);
            Y0 = new AnimationFloat(this, -10000, 10000, 0);
            Z0 = new AnimationFloat(this, -10000, 10000, 0);
            X1 = new AnimationFloat(this, -10000, 10000, 0);
            Y1 = new AnimationFloat(this, -10000, 10000, 0);
            Z1 = new AnimationFloat(this, -10000, 10000, 0);

            Material = new Material(this, MaterialUsage.Mesh)
            {
                Shader = MaterialShader.NoLight
            };
        }

        public SpriteElementLine(SpriteElementLine other)
        {
            _Position0 = other._Position0;
            X0 = new AnimationFloat(this, other.X0);
            Y0 = new AnimationFloat(this, other.Y0);
            Z0 = new AnimationFloat(this, other.Z0);

            _Position1 = other._Position1;
            X1 = new AnimationFloat(this, other.X1);
            Y1 = new AnimationFloat(this, other.Y1);
            Z1 = new AnimationFloat(this, other.Z1);

            Material = new Material(this, other.Material, MaterialUsage.Mesh);
        }

        public override SpriteElementType Type => SpriteElementType.Line;
        public override SpriteElement Clone() => new SpriteElementLine(this);

        private void GetPosition(float progress, int random, out Vector3 pos0, out Vector3 pos1)
        {
            pos0 = _Position0;
            pos0.X += X0.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            pos0.Y += Y0.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            pos0.Z += Z0.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));

            pos1 = _Position1;
            pos1.X += X1.GetValue(progress, RandomUtil.ToFloatSequenced(random, 3));
            pos1.Y += Y1.GetValue(progress, RandomUtil.ToFloatSequenced(random, 4));
            pos1.Z += Z1.GetValue(progress, RandomUtil.ToFloatSequenced(random, 5));
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            GetPosition(param.Progress, param.Random, out var pos0, out var pos1);

            result.Append(Vector3.Transform(pos0, param.Transform));
            result.Append(Vector3.Transform(pos1, param.Transform));

            return true;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 || X0.IsAnimating || Y0.IsAnimating || Z0.IsAnimating || X1.IsAnimating || Y1.IsAnimating || Z1.IsAnimating) outflags |= UpdateFlags.AABB;
        }

        internal override ShowFlags ShowFlags => Material.ShowFlags;

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible && Material.Apply(param.Graphics, param.Progress, param.Random, param.Layer, false))
            {
                GetPosition(param.Progress, param.Random, out var p0, out var p1);

                if (param.Layer != InstanceLayer.Cursor)
                {
                    param.Graphics.DrawLine(p0, p1);
                }
                else
                {
                    param.Graphics.DrawSphere(p0, 10);
                    param.Graphics.DrawSphere(p1, 10);
                }
            }
        }

        internal override bool MouseMove(SpriteObject parent, MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey)
            {
                var d = parent.Scene.Camera.Right * (e.X - prevX) - parent.Scene.Camera.Up * (e.Y - prevY);
                if (shiftKey) Position1 += d;
                else Position0 += d;
            }
            return false;
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("line");
            writer.WriteAttribute("position0", _Position0);
            X0.Save(writer, "x0");
            Y0.Save(writer, "y0");
            Z0.Save(writer, "z0");
            writer.WriteAttribute("position1", _Position1);
            X1.Save(writer, "x1");
            Y1.Save(writer, "y1");
            Z1.Save(writer, "z1");
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position0 = node.ReadAttributeVector3("position0");
            X0.Load(node, "x0");
            Y0.Load(node, "y0");
            Z0.Load(node, "z0");
            Position1 = node.ReadAttributeVector3("position1");
            X1.Load(node, "x1");
            Y1.Load(node, "y1");
            Z1.Load(node, "z1");
            Material.Load(node);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position0);
            X0.Build(writer, false);
            Y0.Build(writer, false);
            Z0.Build(writer, false);
            writer.Write(_Position1);
            X1.Build(writer, false);
            Y1.Build(writer, false);
            Z1.Build(writer, false);
            Material.Build(writer);
        }
    }
}
