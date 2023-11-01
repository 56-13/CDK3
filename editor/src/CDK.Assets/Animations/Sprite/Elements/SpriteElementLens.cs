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
    public class SpriteElementLens : SpriteElement
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
        public AnimationFloat Convex { private set; get; }

        public SpriteElementLens()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            Radius = new AnimationFloat(this, 0, 10000, 0);
            Convex = new AnimationFloat(this, -1, 1, 0);
        }

        public SpriteElementLens(SpriteElementLens other)
        {
            _Position = other._Position;
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            Radius = new AnimationFloat(this, other.Radius);
            Convex = new AnimationFloat(this, other.Convex);
        }
        public override SpriteElementType Type => SpriteElementType.Lens;
        public override SpriteElement Clone() => new SpriteElementLens(this);

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

        private float GetContext(float progress, int random)
        { 
            return Convex.GetValue(progress, RandomUtil.ToFloatSequenced(random, 4));
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
                        var convex = GetContext(param.Progress, param.Random);

                        if (convex != 0) param.Graphics.Lens(sphere.Center, sphere.Radius, convex);
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
            writer.WriteStartElement("lens");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Radius.Save(writer, "radius");
            Convex.Save(writer, "convex");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Radius.Load(node, "radius");
            Convex.Load(node, "convex");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            Radius.Build(writer, false);
            Convex.Build(writer, false);
        }
    }
}
