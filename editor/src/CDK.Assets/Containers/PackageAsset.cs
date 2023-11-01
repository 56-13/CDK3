namespace CDK.Assets.Containers
{
    public class PackageAsset : Asset
    {
        public override AssetType Type => AssetType.Package;

        public override bool IsDirty
        {
            set { }
            get => false;
        }

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }
    }
}
