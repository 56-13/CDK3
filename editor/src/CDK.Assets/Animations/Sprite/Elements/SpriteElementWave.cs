using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementWave : SpriteElement
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
        public AnimationFloat Radius { private set; get; }
        public AnimationFloat Thickness { private set; get; }

        public SpriteElementWave()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            Radius = new AnimationFloat(this, 0, 10000, 0);
            Thickness = new AnimationFloat(this, 0, 10000, 0);
        }

        public SpriteElementWave(SpriteElementWave other)
        {
            _Position = other._Position;
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            Radius = new AnimationFloat(this, other.Radius);
            Thickness = new AnimationFloat(this, other.Thickness);
        }

        public override SpriteElementType Type => SpriteElementType.Wave;
        public override SpriteElement Clone() => new SpriteElementWave(this);

        private BoundingSphere GetSphere(float progress, int random)
        {
            var sphere = new BoundingSphere();
            sphere.Center = _Position;
            sphere.Center.X += X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            sphere.Center.Y += Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            sphere.Center.Z += Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));
            sphere.Radius = Radius.GetValue(progress, RandomUtil.ToFloatSequenced(random, 3));
            return sphere;
        }

        private float GetThickness(float progress, int random)
        {
            return Thickness.GetValue(progress, RandomUtil.ToFloatSequenced(random, 4));
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result) 
        {
            var sphere = GetSphere(param.Progress, param.Random);

            result.Append(Vector3.Transform(new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z), param.Transform));
            result.Append(Vector3.Transform(new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z), param.Transform));
            result.Append(Vector3.Transform(new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z), param.Transform));
            result.Append(Vector3.Transform(new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z), param.Transform));

            return true;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 || X.IsAnimating || Y.IsAnimating || Z.IsAnimating || Radius.IsAnimating) outflags |= UpdateFlags.AABB;
        }

        internal override ShowFlags ShowFlags => Scenes.ShowFlags.Distortion;

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible)
            {
                if (param.Layer == InstanceLayer.Distortion)
                {
                    var sphere = GetSphere(param.Progress, param.Random);

                    if (sphere.Radius > 0)
                    {
                        var thickness = GetThickness(param.Progress, param.Random);

                        if (thickness > 0) param.Graphics.Wave(sphere.Center, sphere.Radius * param.Progress, thickness * (1 - param.Progress));
                    }
                }
                else if (param.Layer == InstanceLayer.Cursor)
                {
                    var sphere = GetSphere(param.Progress, param.Random);

                    if (sphere.Radius > 0) param.Graphics.DrawCircle(sphere.Center, sphere.Radius, false);
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
            writer.WriteStartElement("wave");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Radius.Save(writer, "radius");
            Thickness.Save(writer, "thickness");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Radius.Load(node, "radius");
            Thickness.Load(node, "thickness");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            Radius.Build(writer, false);
            Thickness.Build(writer, false);
        }
    }
}
