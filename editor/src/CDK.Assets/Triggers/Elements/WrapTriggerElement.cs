using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class WrapTriggerElement : TriggerElement
    {
        public TriggerUnit Item { private set; get; }

        public WrapTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            TriggerFormat format;

            if (node.HasAttribute("wrap"))
            {
                format = Parent.Parent.Parent.Format.GetWrap(node.ReadAttributeString("wrap"));
            }
            else
            {
                format = new TriggerFormat(node.ChildNodes[0], 0, false);
            }

            Item = format.Create(parent.Parent);
            Item.Refresh += Item_Refresh;
            if (parent.Linked) Item.Linked = true;
        }

        private void Item_Refresh(object sender, EventArgs e)
        {
            Parent.OnRefresh();
        }

        public override TriggerElementType Type => TriggerElementType.Wrap;

        internal override void Link(bool linked)
        {
            Item.Linked = linked;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            Item.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            return Item.IsRetaining(element, out from);
        }

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            Item.CopyFrom(((WrapTriggerElement)src).Item, oldUnits, newUnits, isNew);
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "wrap")
            {
                Item.Load(node.ChildNodes[0]);
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("wrap");
            writer.WriteAttributeString("name", Name);
            Item.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            Item.Build(writer);
        }

        internal override int Size => Item.Size;

        public override bool IsValid
        {
            get
            {
                foreach (var element in Item.Elements)
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
                foreach (var element in Item.Elements)
                {
                    if (!element.IsEmpty) return false;
                }
                return true;
            }
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Item.GetLocaleStrings(strings);
        }

        public override string ToString() => Item.ToString();
    }
}
