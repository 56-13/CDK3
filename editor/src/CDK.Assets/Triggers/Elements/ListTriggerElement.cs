using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class ListTriggerElement : TriggerElement
    {
        public TriggerFormat ItemFormat { private set; get; }

        private TriggerUnit[] _Items;
        public TriggerUnit[] Items
        {
            set
            {
                var prevs = _Items;

                if (SetProperty(ref _Items, value))
                {
                    foreach (var prev in prevs)
                    {
                        prev.Refresh -= Item_Refresh;
                        if (Parent.Linked && Array.IndexOf(_Items, prev) < 0) prev.Linked = false;
                    }
                    foreach (var next in _Items)
                    {
                        next.Refresh += Item_Refresh;
                        if (Parent.Linked && Array.IndexOf(prevs, next) < 0) next.Linked = true;
                    }
                }
            }
            get => _Items;
        }

        private void Item_Refresh(object sender, EventArgs e)
        {
            Parent.OnRefresh();
        }

        public ListTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            _Items = new TriggerUnit[0];

            ItemFormat = new TriggerFormat(node.ChildNodes[0], 0, false);
        }

        public override TriggerElementType Type => TriggerElementType.List;

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var item in _Items) item.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var item in _Items)
            {
                if (item.IsRetaining(element,  out from)) return true;
            }
            from = null;
            return false;
        }

        internal override void Link(bool linked)
        {
            foreach (var item in _Items) item.Linked = linked;
        }

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var other = (ListTriggerElement)src;

            var items = new TriggerUnit[((ListTriggerElement)src).Items.Length];

            for (var i = 0; i < items.Length; i++)
            {
                items[i] = ItemFormat.Create(Parent.Parent);
                items[i].CopyFrom(other.Items[i], oldUnits, newUnits, isNew);
            }
            Items = items;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "list")
            {
                var items = new TriggerUnit[node.ChildNodes.Count];

                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = ItemFormat.Create(Parent.Parent);
                    items[i].Load(node.ChildNodes[i]);
                }
                Items = items;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("list");
            writer.WriteAttributeString("name", Name);
            foreach (var item in _Items) item.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.WriteLength(_Items.Length);

            foreach (var item in _Items) item.Build(writer);
        }

        internal override int Size
        {
            get
            {
                var size = TypeUtil.GetLengthSize(_Items.Length);
                foreach (var item in _Items) size += item.Size;
                return size;
            }
        }

        public override bool IsValid
        {
            get
            {
                foreach (var item in _Items)
                {
                    foreach (var itemElement in item.Elements)
                    {
                        if (!itemElement.IsValid) return false;
                    }
                }
                return true;
            }
        }

        public override bool IsEmpty => _Items.Length == 0;

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var item in _Items) item.GetLocaleStrings(strings);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append('(');
            var first = true;
            foreach (var item in _Items)
            {
                if (first) first = false;
                else str.Append(' ');
                str.Append(item.ToString());
            }
            str.Append(')');
            return str.ToString();
        }
    }
}
