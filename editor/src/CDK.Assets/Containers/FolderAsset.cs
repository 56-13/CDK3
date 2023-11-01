using System.IO;

namespace CDK.Assets.Containers
{
    public class FolderAsset : Asset
    {
        public override AssetType Type => AssetType.Folder;

        public override bool IsDirty
        {
            set { }
            get => false;
        }
        
        internal override string BuildDirPath => Path.Combine(base.BuildDirPath, Name);
        
        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }
    }
}
