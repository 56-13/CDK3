using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Meshing;
using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementSphere : SpriteElement
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

        private bool _Collision;
        public bool Collision
        {
            set => SetProperty(ref _Collision, value);
            get => _Collision;
        }

        public Material Material { private set; get; }

        public SpriteElementSphere()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            Radius = new AnimationFloat(this, -10000, 10000, 100);

            Material = new Material(this, MaterialUsage.Mesh);
        }

        public SpriteElementSphere(SpriteElementSphere other)
        {
            _Position = other._Position;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            Radius = new AnimationFloat(this, other.Radius);

            _Collision = other._Collision;

            Material = new Material(this, other.Material, MaterialUsage.Mesh);
        }

        public override SpriteElementType Type => SpriteElementType.Sphere;
        public override SpriteElement Clone() => new SpriteElementSphere(this);
        internal override void AddRetains(ICollection<string> retains) => Material.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Material.IsRetaining(element, out from);

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

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            var sphere = GetSphere(param.Progress, param.Random);
            sphere.Transform(param.Transform);
            result.Append(ABoundingBox.FromSphere(sphere));
            return true;
        }

        internal override void AddCollider(ref TransformParam param, ref Collider result)
        {
            if (_Collision)
            {
                if (result == null) result = new Collider(4);
                var sphere = GetSphere(param.Progress, param.Random);
                sphere.Transform(param.Transform);
                result.Add(sphere);
            }
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 || X.IsAnimating || Y.IsAnimating || Z.IsAnimating || Radius.IsAnimating) outflags |= UpdateFlags.AABB;
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            var sphere = GetSphere(param.Progress, param.Random);
            sphere.Transform(param.Transform);

            if (ray.Intersects(sphere, CollisionFlags.None, out var d, out _) && d < distance)
            {
                distance = d;
                return true;
            }
            return false;
        }

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible && Material.Apply(param.Graphics, param.Progress, param.Random, param.Layer, false))
            {
                var sphere = GetSphere(param.Progress, param.Random);

                if (sphere.Radius > 0)
                {
                    if (Radius.IsAnimating)
                    {
                        param.Graphics.DrawSphere(sphere.Center, sphere.Radius);
                    }
                    else
                    {
                        var vertices = VertexArrays.GetSphere(1, sphere.Center, sphere.Radius, Rectangle.ZeroToOne, out var aabb);
                        param.Graphics.DrawVertices(vertices, PrimitiveMode.Triangles, aabb);
                    }
                }
            }
        }

        internal override ShowFlags ShowFlags => Material.ShowFlags;

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
            writer.WriteStartElement("sphere");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Radius.Save(writer, "radius");
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Radius.Load(node, "radius");
            Material.Load(node);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            Radius.Build(writer, false);
            Material.Build(writer);
        }
    }
}
