using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationDerivationEmission : AnimationDerivation
    {
        private float _EmissionDelay;
        public float EmissionDelay
        {
            set => SetProperty(ref _EmissionDelay, value);
            get => _EmissionDelay;
        }

        private int _EmissionCount;
        public int EmissionCount
        {
            set => SetProperty(ref _EmissionCount, value);
            get => _EmissionCount;
        }

        private bool _Prewarm;
        public bool Prewarm
        {
            set => SetProperty(ref _Prewarm, value); 
            get => _Prewarm;
        }

        public AnimationDerivationEmission(AnimationFragment parent) : base(parent)
        {
            _EmissionDelay = 0.1f;
        }

        public AnimationDerivationEmission(AnimationFragment parent, AnimationDerivationEmission other) : base(parent)
        {
            _EmissionDelay = other._EmissionDelay;
            _EmissionCount = other._EmissionCount;
            _Prewarm = other._Prewarm;
        }

        public override AnimationDerivationType Type => AnimationDerivationType.Emission;
        public override AnimationDerivation Clone(AnimationFragment animation) => new AnimationDerivationEmission(animation, this);

        internal override void Save(XmlWriter writer)
        {
            writer.WriteAttribute("emissionDelay", _EmissionDelay);
            writer.WriteAttribute("emissionCount", _EmissionCount);
            writer.WriteAttribute("prewarm", _Prewarm);
        }

        internal override void Load(XmlNode node)
        {
            EmissionDelay = node.ReadAttributeFloat("emissionDelay");
            EmissionCount = node.ReadAttributeInt("emissionCount");
            Prewarm = node.ReadAttributeBool("prewarm");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_EmissionDelay);
            writer.Write(_EmissionCount);
            writer.Write(_Prewarm);
        }

        public override AnimationObjectDerivation CreateObject(AnimationObjectFragment parent) => new AnimationObjectDerivationEmission(parent);
    }
}
