using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Components
{
    public enum AnimationColorType
    {
        None,
        Constant,
        Linear,
        Curve,
        Channel
    }

    public abstract class AnimationColorImpl : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public bool Normalized { private set; get; }
        public bool AlphaChannel { private set; get; }
        public Color4 DefaultColor { private set; get; }
        
        protected AnimationColorImpl(AssetElement parent, bool normalized, bool alphaChannel, in Color4 defaultColor)
        {
            Parent = parent;

            Normalized = normalized;
            AlphaChannel = alphaChannel;
            DefaultColor = defaultColor;
        }

        protected AnimationColorImpl(AssetElement parent, AnimationColorImpl other)
        {
            Parent = parent;

            Normalized = other.Normalized;
            AlphaChannel = other.AlphaChannel;
            DefaultColor = other.DefaultColor;
        }

        public abstract AnimationColorType Type { get; }
        public abstract bool HasOpacity { get; }
        public abstract Color4 GetColor(float t, in Color4 r);
        internal abstract string SaveToString();
        internal abstract void LoadFromString(string str);
        internal void Save(XmlWriter writer, string name)
        {
            writer.WriteAttributeString(name, SaveToString());
        }
        internal void Load(XmlNode node, string name)
        {
            LoadFromString(node.Attributes[name].Value);
        }
        internal abstract void Build(BinaryWriter writer);
        public abstract AnimationColorImpl Clone(AssetElement parent);
    }
}
