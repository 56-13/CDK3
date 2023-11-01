using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class SelectionTriggerElement : TriggerElement
    {
        private IntegerElementType boundary;
        
        public TriggerElementItem[] Items { private set; get; }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            set => SetProperty(ref _SelectedIndex, value);
            get => _SelectedIndex;
        }

        public TriggerElementItem SelectedItem => _SelectedIndex >= 0 ? Items[_SelectedIndex] : null;

        private int _emptySelectedIndex;

        public SelectionTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            boundary = (IntegerElementType)Enum.Parse(typeof(IntegerElementType), node.Attributes["boundary"].Value);

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

            if (Items.Length == 0)
            {
                _SelectedIndex = -1;
            }
            else if (node.HasAttribute("selection"))
            {
                var name = node.ReadAttributeString("selection");

                for (var i = 0; i < Items.Length; i++)
                {
                    if (Items[i].Name == name)
                    {
                        _emptySelectedIndex = _SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public override TriggerElementType Type => TriggerElementType.Selection;

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var selectedItem = ((SelectionTriggerElement)src).SelectedItem;

            if (selectedItem != null)
            {
                var name = selectedItem.Name;

                for (var i = 0; i < Items.Length; i++)
                {
                    if (Items[i].Name == name)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
            else SelectedIndex = Items.Length == 0 ? -1 : 0;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "selection")
            {
                if (node.HasAttribute("value"))
                {
                    var name = node.ReadAttributeString("value");

                    for (var i = 0; i < Items.Length; i++)
                    {
                        if (Items[i].Name == name)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
                else SelectedIndex = Items.Length == 0 ? -1 : 0;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            var selectedItem = SelectedItem;

            writer.WriteStartElement("selection");
            writer.WriteAttributeString("name", Name);
            if (selectedItem != null) writer.WriteAttributeString("value", SelectedItem.Name);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            var selectedItem = SelectedItem;

            if (selectedItem == null) throw new IOException();

            writer.Write(boundary, selectedItem.Code);
        }

        internal override int Size => boundary.GetSize();
        public override bool IsValid => _SelectedIndex >= 0;
        public override bool IsEmpty => _SelectedIndex == _emptySelectedIndex;
        public override string ToString() => SelectedItem?.ToString() ?? "미입력";
    }
}
