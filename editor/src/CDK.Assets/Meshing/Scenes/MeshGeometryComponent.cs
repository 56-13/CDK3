using System;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Meshing
{
    public class MeshGeometryComponent : SceneComponent
    {
        public MeshGeometry Geometry { private set; get; }

        public override string Name
        {
            set { }
            get => Geometry.Origin.Name;
        }

        private MeshCollider _SelectedCollider;
        public MeshCollider SelectedCollider
        {
            set => SetProperty(ref _SelectedCollider, value, false);
            get => _SelectedCollider;
        }

        public MeshGeometryComponent(MeshGeometry geometry)
        {
            Geometry = geometry;
        }

        public override bool IsRetained(bool retrieving, bool children, out AssetElement from, out AssetElement to)
        {
            return base.IsRetained(retrieving, children, out from, out to) || Geometry.IsRetained(retrieving, children, out from, out to);
        }

        public override SceneComponentType Type => SceneComponentType.MeshGeometry;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        public override string ImportFilter => FileFilters.Mesh;
        public override void Import(string path) => Geometry.Parent.Import(path, true, false);
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor || _SelectedCollider == null) return;

            var parent = GetAncestor<MeshObject>(false);

            parent?.DrawCollider(graphics, _SelectedCollider);
        }

        internal override void Select(bool focus)
        {
            if (focus)
            {
                var mesh = GetAncestor<MeshObject>(false);

                if (mesh != null) mesh.Selection.Geometry = Geometry;
            }
        }

        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
