using System;
using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Sources
{
    public enum AnimationSourceType
    {
        Image,
        Mesh
    }

    public abstract class AnimationSource : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        protected AnimationSource(AssetElement parent)
        {
            Parent = parent;
        }

        public abstract AnimationSourceType Type { get; }
        public abstract AnimationSource Clone(AssetElement parent);
        internal abstract void Save(XmlWriter writer);
        internal abstract void Load(XmlNode node);
        internal abstract void Build(BinaryWriter writer);

        public static AnimationSource Create(AssetElement parent, AnimationSourceType type)
        {
            switch (type)
            {
                case AnimationSourceType.Image:
                    return new AnimationSourceImage(parent);
                case AnimationSourceType.Mesh:
                    return new AnimationSourceMesh(parent);
            }
            throw new NotImplementedException();
        }

        internal static AnimationSource Load(AssetElement parent, XmlNode node)
        {
            var name = node.LocalName;
            name = name.Substring(0, 1).ToUpper() + name.Substring(1);
            var type = (AnimationSourceType)Enum.Parse(typeof(AnimationSourceType), name);
            var source = Create(parent, type);
            using (new AssetCommandHolder()) source.Load(node);
            return source;
        }
    }
}
