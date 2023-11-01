using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationDerivationLinked : AnimationDerivation
    {
        private int _LoopCount;
        public int LoopCount
        {
            set => SetProperty(ref _LoopCount, value);
            get => _LoopCount;
        }

        public AnimationDerivationLinked(AnimationFragment parent) : base(parent)
        {
            _LoopCount = 1;
        }

        public AnimationDerivationLinked(AnimationFragment parent, AnimationDerivationLinked other) : base(parent)
        {
            _LoopCount = other._LoopCount;
        }

        public override AnimationDerivationType Type => AnimationDerivationType.Linked;
        public override AnimationDerivation Clone(AnimationFragment parent) => new AnimationDerivationLinked(parent, this);

        internal override void Save(XmlWriter writer)
        {
            writer.WriteAttribute("loopCount", _LoopCount);
        }

        internal override void Load(XmlNode node)
        {
            LoopCount = node.ReadAttributeInt("loopCount");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_LoopCount);
        }

        public override AnimationObjectDerivation CreateObject(AnimationObjectFragment parent) => new AnimationObjectDerivationLinked(parent);
    }
}
