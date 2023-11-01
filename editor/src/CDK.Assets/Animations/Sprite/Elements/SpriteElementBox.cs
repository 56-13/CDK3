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
    public class SpriteElementBox : SpriteElement
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
        public AnimationFloat XDist { private set; get; }
        public AnimationFloat YDist { private set; get; }
        public AnimationFloat ZDist { private set; get; }

        private bool _Collision;
        public bool Collision
        {
            set => SetProperty(ref _Collision, value);
            get => _Collision;
        }

        public Material Material { private set; get; }
        
        public SpriteElementBox()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            XDist = new AnimationFloat(this, -10000, 10000, 100);
            YDist = new AnimationFloat(this, -10000, 10000, 100);
            ZDist = new AnimationFloat(this, -10000, 10000, 100);

            Material = new Material(this, MaterialUsage.Mesh);
        }

        public SpriteElementBox(SpriteElementBox other)
        {
            _Position = other._Position;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            XDist = new AnimationFloat(this, other.XDist);
            YDist = new AnimationFloat(this, other.YDist);
            ZDist = new AnimationFloat(this, other.ZDist);

            _Collision = other._Collision;

            Material = new Material(this, other.Material, MaterialUsage.Mesh);
        }

        public override SpriteElementType Type => SpriteElementType.Box;
        public override SpriteElement Clone() => new SpriteElementBox(this);
        internal override void AddRetains(ICollection<string> retains) => Material.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Material.IsRetaining(element, out from);
        
        private ABoundingBox GetBox(float progress, int random)
        {
            var pos = _Position;
            pos.X += X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            pos.Y += Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            pos.Z += Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));

            var dist = new Vector3(
                XDist.GetValue(progress, RandomUtil.ToFloatSequenced(random, 3)),
                YDist.GetValue(progress, RandomUtil.ToFloatSequenced(random, 4)),
                ZDist.GetValue(progress, RandomUtil.ToFloatSequenced(random, 5)));

            return new ABoundingBox(pos - dist, pos + dist);
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            var box = GetBox(param.Progress, param.Random);
            box.Transform(param.Transform);
            result.Append(box);
            return true;
        }

        internal override void AddCollider(ref TransformParam param, ref Collider result)
        {
            if (_Collision) {
                if (result == null) result = new Collider(4);
                var box = (OBoundingBox)GetBox(param.Progress, param.Random);
                box.Transform(param.Transform);
                result.Add(box);
            }
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 || X.IsAnimating || Y.IsAnimating || Z.IsAnimating || XDist.IsAnimating || YDist.IsAnimating || ZDist.IsAnimating) outflags |= UpdateFlags.Transform;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            var box = (OBoundingBox)GetBox(param.Progress, param.Random);
            box.Transform(param.Transform);
            if (ray.Intersects(box, CollisionFlags.None, out var d, out _) && d < distance)
            {
                distance = d;
                return true;
            }
            return false;
        }

        internal override ShowFlags ShowFlags => Material.ShowFlags;

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible && Material.Apply(param.Graphics, param.Progress, param.Random, param.Layer, false))
            {
                var box = GetBox(param.Progress, param.Random);

                var vertices = VertexArrays.GetBox(1, new Vector3(-0.5f), new Vector3(0.5f), Rectangle.ZeroToOne, out _);

                var transform = Matrix4x4.CreateScale(box.Extent);
                transform.Translation = box.Center;

                var prev = param.Graphics.World;
                param.Graphics.World = transform * prev;
                param.Graphics.DrawVertices(vertices, PrimitiveMode.Triangles, box);
                param.Graphics.World = prev;
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
            writer.WriteStartElement("box");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            XDist.Save(writer, "xdist");
            YDist.Save(writer, "ydist");
            ZDist.Save(writer, "zdist");
            writer.WriteAttribute("collision", _Collision);
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            XDist.Load(node, "xdist");
            YDist.Load(node, "ydist");
            ZDist.Load(node, "zdist");
            Collision = node.ReadAttributeBool("collision");
            Material.Load(node);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            XDist.Build(writer, false);
            YDist.Build(writer, false);
            ZDist.Build(writer, true);
            writer.Write(_Collision);
            Material.Build(writer);
        }
    }
}
