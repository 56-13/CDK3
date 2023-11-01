using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationDerivationMulti : AnimationDerivation
    {
        public AnimationDerivationMulti(AnimationFragment parent) : base(parent)
        {

        }

        public override AnimationDerivationType Type => AnimationDerivationType.Multi;
        public override AnimationDerivation Clone(AnimationFragment parent) => new AnimationDerivationMulti(parent);
        internal override void Save(XmlWriter writer) { }
        internal override void Load(XmlNode node) { }
        internal override void Build(BinaryWriter writer) { }
        public override AnimationObjectDerivation CreateObject(AnimationObjectFragment parent) => new AnimationObjectDerivationMulti(parent);
    }
}
