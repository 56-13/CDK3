using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class CallTriggerElement : TriggerElement
    {
        private TriggerUnitSet _Destination;
        public TriggerUnitSet Destination
        {
            set => SetProperty(ref _Destination, value);
            get => _Destination;
        }

        public CallTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
        }

        public override TriggerElementType Type => TriggerElementType.Call;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var other = (CallTriggerElement)src;

            if (Parent.Parent == src.Parent.Parent)
            {
                Destination = other.Destination;
            }
            else if (other.Destination != null)
            {
                var idx = src.Parent.Parent.Parent.Sets.IndexOf(other.Destination);

                Destination = idx >= 0 && idx < Parent.Parent.Parent.Sets.Count ? Parent.Parent.Parent.Sets[idx] : null;
            }
            else Destination = null;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "call")
            {
                var attr = node.Attributes["destination"];

                Destination = attr != null ? Parent.Parent.Parent.Sets[int.Parse(attr.Value)] : null;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("call");
            writer.WriteAttribute("name", Name);
            if (_Destination != null && _Destination.Index >= 0)
            {
                writer.WriteAttribute("destination", _Destination.Index);
            }
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            if (_Destination != null && _Destination.Index >= 0)
            {
                writer.Write(Parent.Parent.Parent.Format.ContainerBoundary, _Destination.Index);
            }
            else
            {
                throw new AssetException(Owner, "입력되지 않은 트리거가 있습니다.");
            }
        }

        internal override int Size => Parent.Parent.Parent.Format.ContainerBoundary.GetSize();
        public override bool IsValid => _Destination != null && _Destination.Index >= 0;
        public override string ToString() => _Destination?.Name ?? "미입력";
    }
}
