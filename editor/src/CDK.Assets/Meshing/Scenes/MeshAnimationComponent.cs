using System;
using System.Xml;

using CDK.Assets.Scenes;

namespace CDK.Assets.Meshing
{
    public class MeshAnimationComponent : SceneComponent
    {
        public MeshAsset Asset { private set; get; }
        public MeshAnimation Animation { private set; get; }

        public override string Name
        {
            set { }
            get => Animation.Name;

        }
        public MeshAnimationComponent(MeshAsset asset, MeshAnimation animation)
        {
            Asset = asset;

            Animation = animation;
        }

        public override bool IsRetained(bool retrieving, bool children, out AssetElement from, out AssetElement to)
        {
            return base.IsRetained(retrieving, children, out from, out to) || Animation.IsRetained(retrieving, children, out from, out to);
        }

        public override SceneComponentType Type => SceneComponentType.MeshAnimation;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        public override string ImportFilter => FileFilters.Mesh;
        public override void Import(string path) => Asset.Import(path, false, true);
        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
