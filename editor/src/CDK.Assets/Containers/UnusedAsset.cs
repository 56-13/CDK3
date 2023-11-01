using System.Collections.Generic;
using System.IO;

namespace CDK.Assets.Containers
{
    public class UnusedAsset : Asset
    {
        public override AssetType Type => AssetType.Unused;

        public override bool IsDirty
        {
            set { }
            get => false;
        }

        public override bool IsUnused => true;

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
           
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            
        }
    }
}
