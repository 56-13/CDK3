using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.IO;
using System.Xml;

namespace CDK.Assets.Triggers
{
    public enum TriggerElementType
    {
        Call,
        Color,
        List,
        Mask,
        Numeric,
        Pointer,
        Selection,
        String,
        Tab,
        Wrap,
        Asset,
        Member,
        MapObject,
        Unknown
    }

    public abstract class TriggerElement : AssetElement
    {
        public TriggerUnit Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public string Name { private set; get; }

        protected TriggerElement(TriggerUnit parent, XmlNode node)
        {
            Parent = parent;

            Name = node.ReadAttributeString("name");
        }

        protected TriggerElement(TriggerUnit parent, string name)
        {
            Parent = parent;

            Name = name;
        }

        public abstract TriggerElementType Type { get; }
        internal virtual void Link(bool linked) { }
        internal abstract void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew);
        internal abstract void Load(XmlNode node);
        internal abstract void Save(XmlWriter writer);
        internal abstract void Build(BinaryWriter writer);
        internal abstract int Size { get; }
        public virtual bool IsValid => true;
        public virtual bool IsEmpty => false;
        public virtual Color TextColor => IsValid ? Color.Blue : Color.Red;
        internal virtual void GetLocaleStrings(ICollection<LocaleString> strings) { }
        internal static TriggerElement Create(TriggerElementType type, TriggerUnit unit, XmlNode node)
        {
            try
            {
                switch (type)
                {
                    case TriggerElementType.Call:
                        return new CallTriggerElement(unit, node);
                    case TriggerElementType.Color:
                        return new ColorTriggerElement(unit, node);
                    case TriggerElementType.List:
                        return new ListTriggerElement(unit, node);
                    case TriggerElementType.Mask:
                        return new MaskTriggerElement(unit, node);
                    case TriggerElementType.Numeric:
                        return new NumericTriggerElement(unit, node);
                    case TriggerElementType.Pointer:
                        return new PointerTriggerElement(unit, node);
                    case TriggerElementType.Selection:
                        return new SelectionTriggerElement(unit, node);
                    case TriggerElementType.String:
                        return new StringTriggerElement(unit, node);
                    case TriggerElementType.Tab:
                        return new TabTriggerElement(unit, node);
                    case TriggerElementType.Wrap:
                        return new WrapTriggerElement(unit, node);
                    case TriggerElementType.Asset:
                        return new AssetTriggerElement(unit, node);
                    case TriggerElementType.Member:
                        return new MemberTriggerElement(unit, node);
                    //case TriggerElementType.MapObject:
                        //return new MapObjectTriggerElement(unit, node);
                }
            }
            catch { }

            return new InvalidTriggerElement(unit);
        }
    }
}
