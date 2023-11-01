using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class TriggerUnit : AssetElement
    {
        public TriggerUnitSet Parent { private set; get; }
        public override AssetElement GetParent() => _Linked ? Parent : null;

        private List<TriggerElement> _elements;

        public string Name => _Format.Name;
        public string Text => _Format.Text;

        private string _Tag;
        public string Tag
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;

                SetProperty(ref _Tag, value);
            }
            get => _Tag;
        }

        private bool _Annotated;
        public bool Annotated
        {
            set => SetProperty(ref _Annotated, value);
            get => _Annotated;
        }

        public int Index => Parent.Units.IndexOf(this);

        private TriggerFormat _Format;
        public TriggerFormat Format
        {
            set
            {
                if (SetProperty(ref _Format, value))
                {
                    if (_Linked)
                    {
                        foreach (var element in _elements) element.Link(false);
                    }

                    _elements = _Format.CreateElements(this);

                    if (_Linked)
                    {
                        foreach (var element in _elements) element.Link(true);
                    }
                }
            }
            get => _Format;
        }

        public class TriggerElementCollection : IEnumerable<TriggerElement>, IListSource
        {
            private TriggerUnit _trigger;
            public TriggerElement this[int idx] => _trigger._elements[idx];
            public TriggerElement this[string name] => _trigger._elements.FirstOrDefault(e => e.Name == name);
            public int Count => _trigger._elements.Count;

            internal TriggerElementCollection(TriggerUnit trigger)
            {
                _trigger = trigger;
            }

            bool IListSource.ContainsListCollection => false;
            IList IListSource.GetList() => _trigger._elements.AsReadOnly();
            IEnumerator<TriggerElement> IEnumerable<TriggerElement>.GetEnumerator() => _trigger._elements.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _trigger._elements.GetEnumerator();
        }

        public TriggerElementCollection Elements { private set; get; }

        private bool _Linked;
        public bool Linked
        {
            set
            {
                if (_Linked != value)
                {
                    _Linked = value;
                    foreach (var element in _elements) element.Link(_Linked);
                }
            }
            get => _Linked;
        }

        public TriggerUnit(TriggerUnitSet parent, TriggerFormat format)
        {
            Parent = parent;
            _Format = format;
            
            _elements = format.CreateElements(this);

            Elements = new TriggerElementCollection(this);
        }

        private class ResetCommand : IAssetCommand
        {
            private TriggerUnit _unit;
            private TriggerFormat _format;
            private List<TriggerElement> _elements;

            public Asset Asset => _unit.Owner;

            public ResetCommand(TriggerUnit unit)
            {
                _unit = unit;
                _format = unit._Format;
                _elements = unit._elements;
            }

            public void Undo()
            {
                var prevFormat = _unit._Format;
                var prevElements = _unit._elements;

                _unit._Format = _format;
                _unit._elements = _elements;

                if (_unit._Linked)
                {
                    foreach (var element in _unit._elements) element.Link(false);
                }

                _format = prevFormat;
                _elements = prevElements;

                if (_unit._Linked)
                {
                    foreach (var element in _unit._elements) element.Link(true);
                }

                _unit.OnRefresh();
            }

            public void Redo()
            {
                Undo();
            }

            public bool Merge(IAssetCommand other) => false;
        }

        internal void ResetFormat()
        {
            _Format = Parent.Parent.Format.Formats[Format.Name];

            var elements = _Format.CreateElements(this);

            foreach (var se in _elements)
            {
                elements.FirstOrDefault(de => se.Name == de.Name && se.GetType() == de.GetType())?.CopyFrom(se, null, null, false);
            }

            if (_Linked)
            {
                foreach (var element in _elements) element.Link(false);
            }

            _elements = elements;

            if (_Linked)
            {
                foreach (var element in _elements) element.Link(true);
            }
        }

        internal void CopyFrom(TriggerUnit src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            Tag = src.Tag;
            Annotated = src.Annotated;

            foreach (var se in src._elements)
            {
                _elements.FirstOrDefault(de => se.Name == de.Name && se.GetType() == de.GetType())?.CopyFrom(se, oldUnits, newUnits, isNew);
            }
            OnRefresh();
        }

        public void CopyFrom(TriggerUnit src, bool isNew) => CopyFrom(src, null, null, isNew);

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var e in Elements) e.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var e in Elements)
            {
                if (e.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("triggerUnit");
            if (_Format.Independent) writer.WriteAttribute("name", _Format.Name);
            writer.WriteAttribute("tag", _Tag);
            writer.WriteAttribute("annotated", _Annotated);
            foreach (var element in _elements) element.Save(writer);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "triggerUnit") throw new XmlException();

            Tag = node.ReadAttributeString("tag");
            Annotated = node.ReadAttributeBool("annotated");

            foreach (var element in _elements)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var name = child.ReadAttributeString("name");

                    if (element.Name == name)
                    {
                        element.Load(child);
                        break;
                    }
                }
            }
        }

        internal void Build(BinaryWriter writer)
        {
            if (_Format.Independent)
            {
                writer.WriteLength(ContentSize);

                writer.Write(Parent.Parent.Format.CodeBoundary, _Format.Code);
            }

            foreach (var element in _elements) element.Build(writer);
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var element in _elements) element.GetLocaleStrings(strings);
        }

        private int ContentSize
        {
            get
            {
                var contentSize = _Format.Independent ? Parent.Parent.Format.CodeBoundary.GetSize() : 0;
                foreach (var element in _elements) contentSize += element.Size;
                return contentSize;
            }
        }

        internal int Size
        {
            get
            {
                var size = ContentSize;
                if (_Format.Independent) size += TypeUtil.GetLengthSize(size);
                return size;
            }
        }

        public string ToString(bool tag)
        {
            var textState = 0;
            var elementState = false;

            var token = new StringBuilder();
            var str = new StringBuilder();

            if (tag && _Annotated) str.Append("#FFC0C0C0// ");

            for (var i = 0; i < Text.Length; i++)
            {
                if (Text[i] == '$')
                {
                    switch (textState)
                    {
                        case 2:
                            if (token.Length > 0)
                            {
                                var element = Elements[token.ToString()];

                                if (element == null || element.IsEmpty) textState = 3;
                                else textState = 1;
                            }
                            else
                            {
                                textState = 0;
                                str.Append('$');
                            }
                            token.Clear();
                            break;
                        case 0:
                            str.Append(token.ToString());

                            token.Clear();
                            textState = 2;
                            break;
                        case 1:
                            textState = 0;
                            break;
                        case 3:
                            token.Clear();
                            textState = 0;
                            break;
                    }
                }
                else if (Text[i] == '%')
                {
                    if (textState <= 1)
                    {
                        if (elementState)
                        {
                            if (token.Length > 0)
                            {
                                var element = Elements[token.ToString()];

                                if (element != null)
                                {
                                    if (tag && !_Annotated)
                                    {
                                        str.Append('#');
                                        str.Append(element.TextColor.ToArgb().ToString("X8"));
                                    }
                                    str.Append(element.ToString());
                                    if (tag && !_Annotated)
                                    {
                                        str.Append('@');
                                    }
                                }
                            }
                            else str.Append('%');
                        }
                        else str.Append(token.ToString());
                    }
                    token.Clear();
                    elementState = !elementState;
                }
                else token.Append(Text[i]);
            }
            str.Append(token.ToString());

            if (tag && _Annotated) str.Append('@');

            if (_Tag != null)
            {
                str.Append("\r\n");
                if (tag)
                {
                    str.Append('#');
                    str.Append(Color.Green.ToArgb().ToString("X8"));
                    str.Append(_Tag);
                    str.Append('@');
                }
                else str.Append(_Tag);
            }
            return str.ToString();
        }

        public override string ToString() => ToString(false);
    }
}
