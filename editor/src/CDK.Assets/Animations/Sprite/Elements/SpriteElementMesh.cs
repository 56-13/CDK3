using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Meshing;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public class SpriteElementMesh : SpriteElement
    {
        public MeshSelection Selection { private set; get; }

        private Vector3 _Position;
        public Vector3 Position
        {
            set => SetProperty(ref _Position, value);
            get => _Position;
        }
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }

        public SpriteElementMesh()
        {
            Selection = new MeshSelection(this);

            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
        }

        public SpriteElementMesh(SpriteElementMesh other)
        {
            Selection = new MeshSelection(this, other.Selection);

            _Position = other._Position;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
        }

        public override SpriteElementType Type => SpriteElementType.Mesh;
        public override SpriteElement Clone() => new SpriteElementMesh(this);
        internal override void AddRetains(ICollection<string> retains) => Selection.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Selection.IsRetaining(element, out from);

        private Vector3 GetPosition(float progress, int random)
        {
            var pos = _Position;
            pos.X += X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            pos.Y += Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            pos.Z += Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));
            return pos;
        }

        private Instance GetInstance(SpriteObject parent, float progress, float duration)
        {
            parent.MeshInstances.TryGetValue(this, out var origin);
            var current = origin;
            Selection.UpdateInstance(ref current);
            if (current != origin)
            {
                if (current != null) parent.MeshInstances[this] = current;
                else parent.MeshInstances.Remove(this);
            }
            if (current?.Animation != null)
            {
                float animationDuration = current.Animation.Duration;
                if (Selection.Loop.Count != 0) current.Progress = Selection.Loop.GetProgress(progress * Selection.Loop.Count) * animationDuration;
                else current.Progress = Selection.Loop.GetProgress(duration * progress / animationDuration) * animationDuration;
            }
            return current;
        }

        internal override bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result)
        {
            var instance = GetInstance(param.Parent, param.Progress, param.Duration);

            if (instance != null && instance.GetNodeTransform(name, out result))
            {
                var pos = GetPosition(param.Progress, param.Random);

                if (pos != Vector3.Zero)
                {
                    var transform = param.Transform;
                    transform.M41 += pos.X;
                    transform.M42 += pos.Y;
                    transform.M43 += pos.Z;
                    result *= transform;
                }
                else result *= param.Transform;

                return true;
            }
            return false;
        }

        internal override void GetTransformNames(ICollection<string> names) => Selection.Geometry?.GetTransformNames(names);

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            var instance = GetInstance(param.Parent, param.Progress, param.Duration);

            if (instance != null && instance.GetAABB(out var laabb))
            {
                var pos = GetPosition(param.Progress, param.Random);

                if (pos != Vector3.Zero)
                {
                    var transform = param.Transform;
                    transform.M41 += pos.X;
                    transform.M42 += pos.Y;
                    transform.M43 += pos.Z;
                    foreach (var p in laabb.GetCorners()) result.Append(Vector3.Transform(p, transform));
                }
                else
                {
                    foreach (var p in laabb.GetCorners()) result.Append(Vector3.Transform(p, param.Transform));
                }
                return true;
            }
            return false;
        }

        internal override void AddCollider(ref TransformParam param, ref Collider result)
        {
            if (Selection.Collision)
            {
                var instance = GetInstance(param.Parent, param.Progress, param.Duration);

                if (instance != null) Selection.Geometry.AddCollider(instance, param.Transform, ref result);
            }
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            var instance = GetInstance(param.Parent, param.Progress1, param.Duration);

            if (instance != null && ((param.Inflags & UpdateFlags.Transform) != 0 || instance.Animation != null || X.IsAnimating || Y.IsAnimating || Z.IsAnimating))
            {
                outflags |= UpdateFlags.Transform | UpdateFlags.AABB;
            }
        }

        internal override bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            var instance = GetInstance(param.Parent, param.Progress, param.Duration);

            if (instance != null && instance.GetAABB(out var laabb))
            {
                OBoundingBox.Transform(laabb, param.Transform, out var aabb);
                if (ray.Intersects(aabb, CollisionFlags.None, out var d, out _) && d < distance)
                {
                    distance = d;
                    return true;
                }
            }
            return false;
        }

        internal override ShowFlags ShowFlags => Selection.ShowFlags;

        internal override void Draw(ref DrawParam param)
        {
            var instance = GetInstance(param.Parent, param.Progress, param.Duration);

            if (instance != null)
            {
                var pos = GetPosition(param.Progress, param.Random);

                if (pos != Vector3.Zero)
                {
                    var prev = param.Graphics.World;
                    param.Graphics.World = Matrix4x4.CreateTranslation(pos) * prev;
                    instance.Draw(param.Graphics, param.Layer, param.Progress, param.Random);
                    param.Graphics.World = prev;
                }
                else
                {
                    instance.Draw(param.Graphics, param.Layer, param.Progress, param.Random);
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
            writer.WriteStartElement("mesh");
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Selection.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Selection.Load(node.GetChildNode("selection"));
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
        }

        internal override void Build(BinaryWriter writer)
        {
            Selection.Build(writer);
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
        }
    }
}
