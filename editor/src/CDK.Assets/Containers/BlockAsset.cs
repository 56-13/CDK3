using System.IO;

namespace CDK.Assets.Containers
{
    public class BlockAsset : Asset
    {
        public override bool IsDirty
        {
            set {}
            get => false;
        }

        public override AssetType Type => AssetType.Block;

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            if (writer != null)
            {
                using (var subms = new MemoryStream())
                {
                    using (var subwriter = new BinaryWriter(subms))
                    {
                        base.BuildContent(subwriter, path, platform);

                        var bytes = subms.ToArray();

                        writer.Write(bytes.Length);
                        writer.Write(bytes);
                    }
                }
            }
            else base.BuildContent(writer, path, platform);
        }
    }
}
