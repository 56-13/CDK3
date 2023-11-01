using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace CDK.Assets.Attributes
{
    public class Attribute : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public string ParentPropertyName { private set; get; }
        public override AssetElement GetParent() => Parent;
        public override string GetLocation() => $"{Parent.GetLocation()}.{ParentPropertyName}";
        public override IEnumerable<AssetElement> GetSiblings()
        {
            var pinfo = Parent.GetType().GetProperty(ParentPropertyName);
            if (pinfo == null) return new AssetElement[0];
            return Parent.GetSiblings().Select(s => (AssetElement)pinfo.GetValue(s));
        }

        public AssetElementList<AttributeElement> Elements { private set; get; }

        public Attribute(AssetElement parent, string parentPropertyName)
        {
            Parent = parent;
            ParentPropertyName = parentPropertyName;

            Elements = new AssetElementList<AttributeElement>(this);
            Elements.ListChanged += Elements_ListChanged;
            Elements.AddingNew += Elements_AddingNew;
        }

        public Attribute(AssetElement parent, string parentPropertyName, Attribute other, bool content)
        {
            Parent = parent;
            ParentPropertyName = parentPropertyName;

            Elements = new AssetElementList<AttributeElement>(this);
            Elements.BeforeListChanged += Elements_BeforeListChanged;
            Elements.ListChanged += Elements_ListChanged;
            Elements.AddingNew += Elements_AddingNew;

            using (new AssetCommandHolder())
            using (new AssetRetrieveHolder())
            {
                foreach (var element in other.Elements)
                {
                    Elements.Add(new AttributeElement(this, element, content));
                }
            }
        }

        private void Elements_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject = new AttributeElement(this);
        }

        private void Elements_BeforeListChanged(object sender, BeforeListChangedEventArgs<AttributeElement> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (Elements[e.NewIndex].IsRetained()) e.Cancel = true;
                    break;
                case ListChangedType.Reset:
                    foreach (var element in Elements)
                    {
                        if (element.IsRetained())
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    break;
            }
        }

        private void Elements_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (Elements[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (Elements[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var element in Elements)
                    {
                        if (element.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
            if (AssetManager.Instance.RetrieveEnabled)
            {
                using (new AssetRetrieveHolder())
                {
                    foreach (Attribute sibling in GetSiblings())
                    {
                        switch (e.ListChangedType)
                        {
                            case ListChangedType.ItemAdded:
                                if (e.NewIndex <= sibling.Elements.Count)
                                {
                                    sibling.Elements.Insert(e.NewIndex, new AttributeElement(sibling, Elements[e.NewIndex], false));
                                }
                                break;
                            case ListChangedType.ItemDeleted:
                                if (e.NewIndex < sibling.Elements.Count)
                                {
                                    sibling.Elements.RemoveAt(e.NewIndex);
                                }
                                break;
                            case ListChangedType.ItemMoved:
                                if (e.OldIndex < sibling.Elements.Count && e.NewIndex < sibling.Elements.Count)
                                {
                                    sibling.Elements.Move(e.OldIndex, e.NewIndex);
                                }
                                break;
                            case ListChangedType.ItemChanged:
                                if (e.PropertyDescriptor == null && e.NewIndex < sibling.Elements.Count)
                                {
                                    sibling.Elements[e.NewIndex] = new AttributeElement(sibling, Elements[e.NewIndex], false);
                                }
                                break;
                            case ListChangedType.Reset:
                                sibling.Elements.Clear();

                                foreach (var element in Elements)
                                {
                                    sibling.Elements.Add(new AttributeElement(sibling, element, false));
                                }
                                break;
                        }
                    }
                }
            }
        }

        internal bool Compare(Attribute other)
        {
            if (Elements.Count != other.Elements.Count) return false;

            for (var i = 0; i < Elements.Count; i++)
            {
                if (!Elements[i].Compare(other.Elements[i])) return false;
            }
            return true;
        }

        internal void Build(BinaryWriter writer)
        {
            try
            {
                foreach (var element in Elements) element.Build(writer);
            }
            catch
            {
                throw new AssetException(Owner, $"{GetLocation()} 의 데이터에 잘못된 값이 있습니다.");
            }
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("attribute");
            foreach (var element in Elements) element.Save(writer);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "attribute") throw new XmlException();

            var elements = Elements.ToArray();

            Elements.BeforeListChanged -= Elements_BeforeListChanged;

            Elements.Clear();
            foreach (XmlNode subnode in node.ChildNodes)
            {
                var key = subnode.ReadAttributeString("key");
                var element = elements.FirstOrDefault(e => e.Key == key) ?? new AttributeElement(this);
                element.Load(subnode);
                Elements.Add(element);
            }

            Elements.BeforeListChanged += Elements_BeforeListChanged;
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var element in Elements) element.GetLocaleStrings(strings);
        }
    }
}
