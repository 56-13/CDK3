using System;
using System.Xml;

namespace CDK.Assets.Updaters
{
    internal static partial class Updater
    {
        public static readonly int AssetVersion = 100;

        public static void ValidateAnimationAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) AnimationAssetFix(node, version);
        }

        public static void ValidateMeshAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) MeshAssetFix(node, version);
        }
        
        public static void ValidateFileAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) FileAssetFix(node, version);
        }

        public static void ValidateBinaryAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");
        }

        public static void ValidateRootImageAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) RootImageAssetFix(node, version);
        }

        public static void ValidateTextureAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) TextureAssetFix(node, version);
        }

        public static void ValidateTerrainAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) TerrainAssetFix(node, version);
        }

        public static void ValidateMediaAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");
        }

        public static void ValidateReferenceAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) ReferenceAssetFix(node, version);
        }

        public static void ValidateStringAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) StringAssetFix(node, version);
        }

        public static void ValidateSpawnAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) SpawnAssetFix(node, version);
        }

        public static void ValidateAttributeAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) AttributeAssetFix(node, version);
        }

        public static void ValidateTriggerAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");
        }

        public static void ValidateVersionAsset(XmlNode node)
        {
            int version = int.Parse(node.Attributes["version"].Value);

            if (version > AssetVersion) throw new XmlException("에디터의 버전보다 애셋의 버전이 더 높습니다.");

            if (version < AssetVersion) VersionAssetFix(node, version);
        }
    }
}
