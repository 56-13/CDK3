using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Animations;

namespace CDK.Assets.Scenes
{
    public class CapsuleObject : SceneObject, IAnimationSubstance
    {
        private float _Height;
        [Binding]
        public float Height
        {
            set 
            {
                if (SetProperty(ref _Height, value)) AddUpdateFlags(UpdateFlags.AABB);
            }
            get => _Height; 
        }

        private float _Radius;
        [Binding]
        public float Radius
        {
            set
            {
                if (SetProperty(ref _Radius, value)) AddUpdateFlags(UpdateFlags.AABB);
            }
            get => _Radius;
        }

        private bool _Collision;
        [Binding]
        public bool Collision
        {
            set => SetProperty(ref _Collision, value);
            get => _Collision;
        }

        public Texturing.Material Material { private set; get; }

        private float _progress;
        private int _random;

        public const float DefaultHeight = 100;
        public const float DefaultRadius = 100;

        public CapsuleObject()
        {
            _Height = DefaultHeight;
            _Radius = DefaultRadius;

            Material = new Texturing.Material(this, Texturing.MaterialUsage.Mesh);
        }

        public CapsuleObject(Texturing.Material material, bool origin)
        {
            _Height = DefaultHeight;
            _Radius = DefaultRadius;

            if (origin)
            {
                Material = material;
                Material.AddWeakRefresh(Material_Refresh);
            }
            else Material = new Texturing.Material(this, material, Texturing.MaterialUsage.Mesh);
        }

        public CapsuleObject(CapsuleObject other, bool binding) : base(other, binding, true)
        {
            _Height = other._Height;
            _Radius = other._Radius;
            _Collision = other._Collision;

            if (binding)
            {
                Material = other.Material;
                Material.AddWeakRefresh(Material_Refresh);
            }
            else Material = new Texturing.Material(this, other.Material, Texturing.MaterialUsage.Mesh);
        }

        private void Material_Refresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        public override SceneComponentType Type => SceneComponentType.Box;
        public override SceneComponent Clone(bool binding) => new CapsuleObject(this, binding);
        public override SceneComponentType[] SubTypes => Material.Usage == Texturing.MaterialUsage.Origin ? new SceneComponentType[0] : base.SubTypes;
        public override bool AddSubEnabled(SceneComponent obj) => Material.Usage != Texturing.MaterialUsage.Origin && base.AddSubEnabled(obj);
        public override void AddSub(SceneComponentType type)
        {
            if (Material.Usage != Texturing.MaterialUsage.Origin) base.AddSub(type);
        }
        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (GetTransform(out var transform))
            {
                var abbr = new Vector3(_Radius, _Radius, _Radius + _Height);
                var box = new ABoundingBox(-abbr, abbr);
                box.Transform(transform);
                result.Append(box);
                return true;
            }
            return false;
        }

        internal override void AddCollider(ref Collider result)
        {
            if (_Collision && GetTransform(out var transform))
            {
                if (result == null) result = new Collider(1);
                var capsule = new BoundingCapsule(new Vector3(0, 0, -_Height), new Vector3(0, 0, _Height), _Radius);
                capsule.Transform(transform);
                result.Add(capsule);
            }
        }

        internal override void Rewind()
        {
            _progress = 0;
            _random = RandomUtil.Next();
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            _progress += delta;
            if ((flags & UpdateFlags.Transform) != 0) flags |= UpdateFlags.AABB;
            return UpdateState.None;
        }

        internal override ShowFlags Show() => Material.ShowFlags;

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == InstanceLayer.Cursor && Transform.Draw(graphics)) return;

            if (Material.Apply(graphics, _progress, _random, layer, true))
            {
                if (GetTransform(out var transform))
                {
                    var capsule = VertexArrays.GetCapsule(1, Vector3.Zero, _Height, _Radius, Rectangle.ZeroToOne, out var aabb);
                    graphics.Transform(transform);
                    graphics.DrawVertices(capsule, PrimitiveMode.Triangles, aabb);
                }
                graphics.Pop();
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            writer.WriteAttribute("height", _Height, DefaultHeight);
            writer.WriteAttribute("radius", _Radius, DefaultRadius);
        }

        protected override void SaveChildren(XmlWriter writer)
        {
            Material.Save(writer);

            writer.WriteStartElement("children");
            base.SaveChildren(writer);
            writer.WriteEndElement();
        }

        protected override void LoadContent(XmlNode node)
        {
            Height = node.ReadAttributeFloat("height", DefaultHeight);
            Radius = node.ReadAttributeFloat("radius", DefaultRadius);
        }

        protected override void LoadChildren(XmlNode node)
        {
            Material.Load(node.ChildNodes[0]);

            base.LoadChildren(node.ChildNodes[1]);
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Material.Build(writer);

            writer.Write(_Height);
            writer.Write(_Radius);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new CapsuleObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new CapsuleObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => BuildContent(writer, null);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
