using System.IO;
using System.Xml;

namespace CDK.Assets.Triggers
{
    public class InvalidTriggerElement : TriggerElement
    {
        public InvalidTriggerElement(TriggerUnit parent)
            : base(parent, "unknown")
        {
        }

        public override TriggerElementType Type => TriggerElementType.Unknown;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            
        }

        internal override void Load(XmlNode node)
        {
            
        }

        internal override void Save(XmlWriter writer)
        {
            throw new XmlException();
        }

        internal override void Build(BinaryWriter writer)
        {
            throw new IOException();
        }

        internal override int Size => 0;
        public override bool IsValid => false;
        public override string ToString() => string.Empty;
    }
}
