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
    public class BoxObject : SceneObject, IAnimationSubstance
    {
        private Vector3 _Extent;
        [Binding]
        public Vector3 Extent
        {
            set 
            {
                if (SetProperty(ref _Extent, value)) AddUpdateFlags(UpdateFlags.AABB);
            }
            get => _Extent; 
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

        public const float DefaultRadius = 100;

        public BoxObject()
        {
            _Extent = new Vector3(DefaultRadius);

            Material = new Texturing.Material(this, Texturing.MaterialUsage.Mesh);
        }

        public BoxObject(Texturing.Material material, bool origin)
        {
            _Extent = new Vector3(DefaultRadius);

            if (origin)
            {
                Material = material;
                Material.AddWeakRefresh(Material_Refresh);
            }
            else Material = new Texturing.Material(this, material, Texturing.MaterialUsage.Mesh);
        }

        public BoxObject(BoxObject other, bool binding) : base(other, binding, true)
        {
            _Extent = other._Extent;
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
        public override SceneComponent Clone(bool binding) => new BoxObject(this, binding);
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
                var box = new ABoundingBox(-_Extent, _Extent);
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
                var box = new OBoundingBox(Vector3.Zero, _Extent, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);
                box.Transform(transform);
                result.Add(box);
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
                    var box = VertexArrays.GetBox(1, -Vector3.One, Vector3.One, Rectangle.ZeroToOne, out var aabb);
                    graphics.Transform(Matrix4x4.CreateScale(_Extent) * transform);
                    graphics.DrawVertices(box, PrimitiveMode.Triangles, aabb);
                }
                graphics.Pop();
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            writer.WriteAttribute("extent", _Extent, new Vector3(DefaultRadius));
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
            Extent = node.ReadAttributeVector3("extent", new Vector3(DefaultRadius));
        }

        protected override void LoadChildren(XmlNode node)
        {
            Material.Load(node.ChildNodes[0]);

            base.LoadChildren(node.ChildNodes[1]);
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Material.Build(writer);

            writer.Write(_Extent);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new BoxObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new BoxObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => BuildContent(writer, null);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
