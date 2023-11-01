using System;
using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Components
{
    public enum AnimationFloatType
    {
        None,
        Constant,
        Linear,
        Curve
    }

    public abstract class AnimationFloatImpl : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public float MinValue { private set; get; }
        public float MaxValue { private set; get; }

        protected AnimationFloatImpl(AssetElement parent, float minValue, float maxValue)
        {
            Parent = parent;

            MinValue = minValue;
            MaxValue = maxValue;
        }

        protected AnimationFloatImpl(AssetElement parent, AnimationFloatImpl other)
        {
            Parent = parent;

            MinValue = other.MinValue;
            MaxValue = other.MaxValue;
        }

        public abstract AnimationFloatType Type { get; }
        public abstract float GetValue(float t);
        public abstract float GetValue(float t, float r);
        internal abstract string SaveToString();
        internal abstract void LoadFromString(string str);
        internal void Save(XmlWriter writer, string name) => writer.WriteAttributeString(name, SaveToString());
        internal void Load(XmlNode node, string name) => LoadFromString(node.Attributes[name].Value);
        internal abstract void Build(BinaryWriter writer, bool asRadian);
        public abstract AnimationFloatImpl Clone(AssetElement owner);
    }
}
