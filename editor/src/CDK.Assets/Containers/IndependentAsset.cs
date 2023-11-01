namespace CDK.Assets.Containers
{
    public class IndependentAsset : Asset
    {
        public override AssetType Type => AssetType.Independent;
        
        public override bool IsDirty
        {
            set { }
            get => false;
        }

        public override bool IsIndependent => true;

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }
    }
}
