using System.Collections.Generic;
using System.Xml;

namespace CDK.Assets.Triggers
{
    public class TriggerFormat
    {
        public int Code { private set; get; }
        public string Name { private set; get; }
        public string Text { private set; get; }
        public bool Independent { private set; get; }

        private List<TriggerElementFormat> _elements;

        internal TriggerFormat(string name, int code, string text, List<TriggerElementFormat> elements)
        {
            Name = name;
            Code = code;
            Text = text;

            _elements = elements;

            Independent = true;
        }

        internal TriggerFormat(XmlNode node, int autoCode, bool independent)
        {
            if (node.LocalName != "triggerFormat") throw new XmlException();

            Independent = independent;

            Name = node.ReadAttributeString("name");

            Code = node.ReadAttributeInt("code", autoCode);

            Text = node.ReadAttributeString("text");

            _elements = new List<TriggerElementFormat>();

            foreach (XmlNode child in node.ChildNodes)
            {
                _elements.Add(new TriggerElementFormat(child));
            }
        }

        public TriggerUnit Create(TriggerUnitSet set) => new TriggerUnit(set, this);

        internal List<TriggerElement> CreateElements(TriggerUnit unit)
        {
            var elements = new List<TriggerElement>();

            foreach (var element in this._elements)
            {
                elements.Add(element.Create(unit));
            }
            return elements;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("triggerFormat");
            writer.WriteAttribute("code", Code);
            writer.WriteAttribute("name", Name);
            writer.WriteAttribute("text", Text);
            foreach (var element in _elements) element.Write(writer);
            writer.WriteEndElement();
        }

        public override string ToString() => Name;
    }
}
