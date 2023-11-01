using System;
using System.Drawing;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class PointerTriggerElement : TriggerElement
    {
        private TriggerUnit _Destination;
        public TriggerUnit Destination
        {
            set => SetProperty(ref _Destination, value);
            get => _Destination;
        }

        public PointerTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
        }

        public override TriggerElementType Type => TriggerElementType.Pointer;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var other = (PointerTriggerElement)src;

            int idx;

            if (other.Destination == null) Destination = null;
            else if (oldUnits != null && newUnits != null && (idx = Array.IndexOf(oldUnits, other.Destination)) >= 0) Destination = newUnits[idx];
            else if (Parent.Parent == other.Parent.Parent) Destination = other.Destination;
            else
            {
                idx = ((PointerTriggerElement)src).Destination.Index;

                Destination = idx >= 0 && idx < Parent.Parent.Units.Count ? Parent.Parent.Units[idx] : null;
            }
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "pointer")
            {
                var attr = node.Attributes["destination"];

                Destination = attr != null ? Parent.Parent.Units[int.Parse(attr.Value)] : null;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("pointer");
            writer.WriteAttribute("name", Name);
            if (_Destination != null)
            {
                var idx = _Destination.Index;

                if (idx >= 0)
                {
                    writer.WriteAttributeString("destination", idx.ToString());
                }
            }
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            var idx = -1;

            var exists = false;

            if (_Destination != null)
            {
                idx = _Destination.Index;

                if (idx >= 0)
                {
                    var ridx = 0;
                    for (var i = 0; i < idx; i++)
                    {
                        var otherUnit = Parent.Parent.Units[i];
                        if (!otherUnit.Annotated) ridx++;
                    }
                    for (var i = idx; i < Parent.Parent.Units.Count; i++)
                    {
                        var otherUnit = Parent.Parent.Units[i];

                        if (!otherUnit.Annotated)
                        {
                            idx = ridx;
                            exists = true;
                            break;
                        }
                    }
                }
            }
            if (!exists)
            {
                idx = Parent.Parent.Units.Count;
            }
            writer.Write(Parent.Parent.Parent.Format.SetBoundary, idx);
        }

        internal override int Size => Parent.Parent.Parent.Format.SetBoundary.GetSize();
        public override bool IsEmpty => _Destination == null || !_Destination.Parent.Units.Contains(_Destination);
        public override Color TextColor => IsEmpty ? Color.DeepSkyBlue : Color.Blue;

        public override string ToString()
        {
            if (_Destination != null)
            {
                var idx = _Destination.Index;

                if (idx >= 0) return idx + "라인";
            }
            return "종료라인";
        }
    }
}
