using System;
using System.ComponentModel;
using System.Xml;

using CDK.Assets.Scenes;

namespace CDK.Assets.Meshing
{
    public class MeshAnimationsComponent : SceneComponent
    {
        public MeshAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Animations";
        }

        private bool _bindingChanging;

        public MeshAnimationsComponent(MeshAsset asset)
        {
            Fixed = true;

            Asset = asset;

            _bindingChanging = true;
            foreach (var animation in Asset.Animations)
            {
                Children.Add(new MeshAnimationComponent(Asset, animation));
            }
            _bindingChanging = false;

            Asset.Animations.AddWeakListChanged(Animations_ListChanged);

            Children.ListChanged += Objects_ListChanged;
        }

        private void Objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Asset.Animations.Insert(e.NewIndex, ((MeshAnimationComponent)Children[e.NewIndex]).Animation);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Asset.Animations[e.NewIndex] = ((MeshAnimationComponent)Children[e.NewIndex]).Animation;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Asset.Animations.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    Asset.Animations.Move(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    Asset.Animations.Clear();
                    foreach (MeshAnimationComponent obj in Children)
                    {
                        Asset.Animations.Add(obj.Animation);
                    }
                    break;
            }

            _bindingChanging = false;
        }

        private void Animations_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Children.Insert(e.NewIndex, new MeshAnimationComponent(Asset, Asset.Animations[e.NewIndex]));
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Children[e.NewIndex] = new MeshAnimationComponent(Asset, Asset.Animations[e.NewIndex]);
                    }
                    break;
                case ListChangedType.ItemMoved:
                    {
                        var obj = Children[e.OldIndex];
                        Children.RemoveAt(e.OldIndex);
                        Children.Insert(e.NewIndex, obj);
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Children.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    Children.Clear();
                    foreach (var animation in Asset.Animations)
                    {
                        Children.Add(new MeshAnimationComponent(Asset, animation));
                    }
                    break;
            }

            _bindingChanging = false;
        }

        public override SceneComponentType Type => SceneComponentType.MeshAnimations;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj)
        {
            return _bindingChanging && obj is MeshAnimationComponent animation && animation.Asset == Asset;
        }
        public override void AddSub(SceneComponentType type) { }
        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
