using System.Xml;
using System.IO;

using CDK.Assets.Animations.Derivations;

namespace CDK.Assets.Animations
{
    public enum AnimationDerivationType
    {
        None,
        Multi,
        Linked,
        Emission,
        Random
    }

    public abstract class AnimationDerivation : AssetElement
    {
        public AnimationFragment Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        protected AnimationDerivation(AnimationFragment parent)
        {
            Parent = parent;
        }

        public abstract AnimationDerivationType Type { get; }
        public abstract AnimationDerivation Clone(AnimationFragment parent);
        internal abstract void Save(XmlWriter writer);
        internal abstract void Load(XmlNode node);
        internal abstract void Build(BinaryWriter writer);
        public abstract AnimationObjectDerivation CreateObject(AnimationObjectFragment parent);
        
        public static AnimationDerivation Create(AnimationFragment animation, AnimationDerivationType type)
        {
            switch (type)
            {
                case AnimationDerivationType.Multi:
                    return new AnimationDerivationMulti(animation);
                case AnimationDerivationType.Linked:
                    return new AnimationDerivationLinked(animation);
                case AnimationDerivationType.Emission:
                    return new AnimationDerivationEmission(animation);
                case AnimationDerivationType.Random:
                    return new AnimationDerivationRandom(animation);
            }
            return null;
        }
    }
}
