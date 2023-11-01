using System.Collections.Generic;
using System.IO;

namespace CDK.Assets.Containers
{
    public class BlockListAsset : ListAsset
    {
        public override AssetType Type => AssetType.BlockList;

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            if (writer != null)
            {
                var builds = new List<byte[]>(Children.Count);

                var position = 0;

                writer.WriteLength(Children.Count);

                using (var subms = new MemoryStream())
                {
                    using (var subwriter = new BinaryWriter(subms))
                    {
                        foreach (var child in Children)
                        {
                            child.BuildContent(subwriter, path, platform);

                            var bytes = subms.ToArray();

                            writer.Write(position);
                            writer.Write(bytes.Length);

                            builds.Add(bytes);

                            position += bytes.Length;

                            subms.Position = 0;
                            subms.SetLength(0);
                        }
                    }
                }

                foreach (var bytes in builds) writer.Write(bytes);
            }
            else base.BuildContent(writer, path, platform);
        }
    }
}
