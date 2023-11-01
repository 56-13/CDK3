using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationDerivationRandom : AnimationDerivation
    {
        private bool _Loop;
        public bool Loop
        {
            set => SetProperty(ref _Loop, value);
            get => _Loop;
        }

        public AnimationDerivationRandom(AnimationFragment parent) : base(parent)
        {
            _Loop = true;
        }

        public AnimationDerivationRandom(AnimationFragment parent, AnimationDerivationRandom other) : base(parent)
        {
            _Loop = other._Loop;
        }

        public override AnimationDerivationType Type => AnimationDerivationType.Random;
        public override AnimationDerivation Clone(AnimationFragment parent) => new AnimationDerivationRandom(parent, this);

        internal override void Save(XmlWriter writer)
        {
            writer.WriteAttribute("loop", _Loop);
        }

        internal override void Load(XmlNode node)
        {
            Loop = node.ReadAttributeBool("loop");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Loop);
        }

        public override AnimationObjectDerivation CreateObject(AnimationObjectFragment parent) => new AnimationObjectDerivationRandom(parent);
    }
}
