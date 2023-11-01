using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class MaskTriggerElement : TriggerElement
    {
        private IntegerElementType _boundary;

        public TriggerElementItem[] Items { private set; get; }

        private int[] _CheckedIndices;
        public int[] CheckedIndices
        {
            set => SetProperty(ref _CheckedIndices, value);
            get => _CheckedIndices;
        }

        private int Value
        {
            get
            {
                var v = 0;
                foreach (var index in _CheckedIndices) v |= Items[index].Code;
                return v;
            }
        }

        public string Selection
        {
            get
            {
                var str = new StringBuilder();
                var first = true;

                foreach (int index in _CheckedIndices)
                {
                    if (first) first = false;
                    else str.Append(',');
                    str.Append(Items[index].Name);
                }
                return str.ToString();
            }
        }

        public MaskTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            _boundary = node.ReadAttributeEnum<IntegerElementType>("boundary");

            var items = new List<TriggerElementItem>();

            if (node.HasAttribute("items"))
            {
                var pitems = Parent.Parent.Parent.Format.GetItems(node.ReadAttributeString("items"));

                if (pitems != null) items.AddRange(pitems);
            }

            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName == "item")
                {
                    items.Add(new TriggerElementItem(subnode, items.Count));
                }
            }
            Items = items.ToArray();

            _CheckedIndices = new int[0];
        }

        private int[] ParseIndices(string str)
        {
            var indices = new List<int>();

            var selections = str.Split(new char[1] { ',' });

            foreach (var selection in selections)
            {
                for (var i = 0; i < Items.Length; i++)
                {
                    if (Items[i].Name == selection)
                    {
                        indices.Add(i);
                        break;
                    }
                }
            }
            return indices.ToArray();
        }

        public override TriggerElementType Type => TriggerElementType.Mask;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            CheckedIndices = ParseIndices(((MaskTriggerElement)src).Selection);
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "mask")
            {
                CheckedIndices = ParseIndices(node.Attributes["selection"].Value);
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("mask");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("selection", Selection);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_boundary, Value);
        }

        internal override int Size => _boundary.GetSize();
        public override bool IsEmpty => _CheckedIndices.Length == 0;
        public override string ToString() => $"({Selection})";
    }
}
