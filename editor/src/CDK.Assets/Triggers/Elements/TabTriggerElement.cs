using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CDK.Assets.Triggers
{
    public class TabTriggerElement : TriggerElement
    {
        public TriggerFormat[] ItemFormats { private set; get; }

        private TriggerUnit _SelectedItem;
        public TriggerUnit SelectedItem
        {
            set
            {
                var prev = _SelectedItem;

                if (SetProperty(ref _SelectedItem, value))
                {
                    if (prev != null)
                    {
                        prev.Refresh -= SelectedItem_Refresh;
                        if (Parent.Linked) prev.Linked = false;
                    }
                    if (_SelectedItem != null)
                    {
                        _SelectedItem.Refresh += SelectedItem_Refresh;
                        if (Parent.Linked) prev.Linked = true;
                    }
                }
            }
            get => _SelectedItem;
        }

        private void SelectedItem_Refresh(object sender, EventArgs e)
        {
            Parent.OnRefresh();
        }

        private IntegerElementType _codeBoundary;

        public TabTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            _codeBoundary = node.ReadAttributeEnum<IntegerElementType>("codeBoundary");

            var itemFormats = new List<TriggerFormat>();

            foreach (XmlNode subnode in node.ChildNodes)
            {
                itemFormats.Add(new TriggerFormat(subnode, itemFormats.Count, false));
            }
            
            ItemFormats = itemFormats.ToArray();

            if (ItemFormats.Length != 0)
            {
                _SelectedItem = new TriggerUnit(Parent.Parent, ItemFormats[0]);

                if (Parent.Linked) _SelectedItem.Linked = true;
            }
        }

        public override TriggerElementType Type => TriggerElementType.Tab;

        internal override void Link(bool linked)
        {
            if (_SelectedItem != null) _SelectedItem.Linked = linked;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            _SelectedItem?.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_SelectedItem != null && _SelectedItem.IsRetaining(element, out from)) return true;
            from = null;
            return false;
        }

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            TriggerUnit srcItem = ((TabTriggerElement)src).SelectedItem;

            if (srcItem != null)
            {
                foreach (TriggerFormat itemFormat in ItemFormats)
                {
                    if (itemFormat.Name.Equals(srcItem.Format.Name))
                    {
                        SelectedItem = itemFormat.Create(Parent.Parent);

                        _SelectedItem.CopyFrom(srcItem, oldUnits, newUnits, isNew);

                        return;
                    }
                }
            }
            SelectedItem = null;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "tab")
            {
                if (node.Attributes["format"] != null)
                {
                    string formatName = node.Attributes["format"].Value;

                    foreach (TriggerFormat itemFormat in ItemFormats)
                    {
                        if (itemFormat.Name.Equals(formatName))
                        {
                            SelectedItem = itemFormat.Create(Parent.Parent);

                            _SelectedItem.Load(node.ChildNodes[0]);

                            return;
                        }
                    }
                }
                SelectedItem = null;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("tab");
            writer.WriteAttributeString("name", Name);

            if (_SelectedItem != null)
            {
                writer.WriteAttributeString("format", _SelectedItem.Format.Name);

                _SelectedItem.Save(writer);
            }
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            if (_SelectedItem == null) throw new AssetException(Owner, "입력되지 않은 트리거가 있습니다.");

            writer.Write(_codeBoundary, _SelectedItem.Format.Code);

            SelectedItem.Build(writer);
        }

        internal override int Size
        {
            get
            {
                var size = _codeBoundary.GetSize();
                if (_SelectedItem != null) size += _SelectedItem.Size;
                return size;
            }
        }
        public override bool IsValid
        {
            get
            {
                if (_SelectedItem == null) return false;
                foreach (var element in _SelectedItem.Elements)
                {
                    if (!element.IsValid) return false;
                }
                return true;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (_SelectedItem == null) return false;
                foreach (var element in _SelectedItem.Elements)
                {
                    if (!element.IsEmpty) return false;
                }
                return true;
            }
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            _SelectedItem?.GetLocaleStrings(strings);
        }

        public override string ToString() => _SelectedItem?.ToString() ?? "미입력";
    }
}
