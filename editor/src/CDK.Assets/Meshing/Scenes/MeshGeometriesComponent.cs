using System;
using System.ComponentModel;
using System.Xml;

using CDK.Assets.Scenes;

namespace CDK.Assets.Meshing
{
    public class MeshGeometriesComponent : SceneComponent
    {
        public MeshAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Meshes";
        }

        private bool _bindingChanging;

        public MeshGeometriesComponent(MeshAsset asset)
        {
            Fixed = true;

            Asset = asset;

            _bindingChanging = true;
            foreach (var geometry in Asset.Geometries)
            {
                Children.Add(new MeshGeometryComponent(geometry));
            }
            _bindingChanging = false;

            Asset.Geometries.AddWeakListChanged(Geometries_ListChanged);

            Children.ListChanged += Objects_ListChanged;
        }

        private void Objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Asset.Geometries.Insert(e.NewIndex, ((MeshGeometryComponent)Children[e.NewIndex]).Geometry);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Asset.Geometries[e.NewIndex] = ((MeshGeometryComponent)Children[e.NewIndex]).Geometry;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Asset.Geometries.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    Asset.Geometries.Move(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    Asset.Geometries.Clear();
                    foreach (MeshGeometryComponent obj in Children)
                    {
                        Asset.Geometries.Add(obj.Geometry);
                    }
                    break;
            }

            _bindingChanging = false;
        }

        private void Geometries_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Children.Insert(e.NewIndex, new MeshGeometryComponent(Asset.Geometries[e.NewIndex]));
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Children[e.NewIndex] = new MeshGeometryComponent(Asset.Geometries[e.NewIndex]);
                    }
                    break;
                case ListChangedType.ItemMoved:
                    Children.Move(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    Children.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    Children.Clear();
                    foreach (var geometry in Asset.Geometries)
                    {
                        Children.Add(new MeshGeometryComponent(geometry));
                    }
                    break;
            }

            _bindingChanging = false;
        }

        public override SceneComponentType Type => SceneComponentType.MeshGeometries;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj)
        {
            return _bindingChanging && obj is MeshGeometryComponent geometry && geometry.Geometry.Parent == Asset;
        }
        public override void AddSub(SceneComponentType type) { }
        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
