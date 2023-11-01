using System.IO;

namespace CDK.Assets.Containers
{
    public class ListAsset : Asset
    {
        public override AssetType Type => AssetType.List;
        
        public override bool IsDirty
        {
            set { }
            get => false;
        }

        public override bool IsListed => true;

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            writer?.WriteLength(Children.Count);

            base.BuildContent(writer, path, platform);
        }
    }
}
