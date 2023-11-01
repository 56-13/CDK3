using System.Linq;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class MemberTriggerElement : TriggerElement
    {
        private TriggerMember _Value;
        public TriggerMember Value
        {
            set => SetProperty(ref _Value, value);
            get => _Value;
        }

        public MemberTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
        }

        public override TriggerElementType Type => TriggerElementType.Member;

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Value == element)
            {
                from = this;
                return true;
            }
            else
            {
                from = null;
                return false;
            }
        }

        internal override void CopyFrom(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            var value = ((MemberTriggerElement)src).Value;

            if (value != null)
            {
                var trigger = Parent.Parent.Parent;

                var member = trigger.Members.FirstOrDefault(m => m.Name == value.Name);

                if (member != null) Value = member;
                else
                {
                    var newMember = new TriggerMember(trigger, value.Name);
                    trigger.Members.Add(newMember);
                    Value = newMember;
                }
            }
            else Value = null;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "member")
            {
                var attr = node.Attributes["value"];

                if (attr != null)
                {
                    var name = attr.Value;

                    Value = Parent.Parent.Parent.Members.FirstOrDefault(m => m.Name == name);
                }
                else Value = null;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("member");
            writer.WriteAttribute("name", Name);
            if (_Value != null) writer.WriteAttribute("value", _Value.Name);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            if (_Value != null)
            {
                var index = Parent.Parent.Parent.Members.IndexOf(_Value);

                if (index >= 0)
                {
                    writer.Write((short)index);
                    return;
                }
            }
            throw new AssetException(Owner, "입력되지 않은 트리거가 있습니다.");
        }

        internal override int Size => 2;
        public override bool IsValid => _Value != null && Parent.Parent.Parent.Members.Contains(_Value);
        public override string ToString() => _Value?.Name ?? "미입력";
    }
}
