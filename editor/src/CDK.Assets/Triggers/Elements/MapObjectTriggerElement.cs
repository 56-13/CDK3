using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

//using CDK.Assets.Map;
//using CDK.Assets.Map.Element;

namespace CDK.Assets.Triggers
{
    /*
    public class MapObjectTriggerElement : TriggerElement
    {
        public MapElement Root { private set; get; }

        public int Depth { private set; get; }

        public IntegerElementType Boundary { private set; get; }

        private MapElement _Selection;
        public MapElement Selection
        {
            set
            {
                SetProperty(true, "Selection", ref _Selection, value);
            }
            get
            {
                return _Selection;
            }
        }

        public MapObjectTriggerElement(TriggerUnit parent, XmlNode node)
            : base(parent, node)
        {
            if (!(Owner is MapAsset))
            {
                throw new XmlException();
            }
            MapElement root = ((MapAsset)Owner).Element;

            foreach (string str in node.Attributes["root"].Value.Split(','))
            {
                int idx = int.Parse(str);

                root = root.Children[idx];
            }
            Root = root;

            Depth = int.Parse(node.Attributes["depth"].Value);

            Boundary = (IntegerElementType)Enum.Parse(typeof(IntegerElementType), node.Attributes["boundary"].Value);
        }

        public override TriggerElementType Type
        {
            get
            {
                return TriggerElementType.MapObject;
            }
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Root == element || _Selection == element) {
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

        internal override void Copy(TriggerElement src, TriggerUnit[] oldUnits, TriggerUnit[] newUnits, bool isNew)
        {
            MapObjectTriggerElement other = (MapObjectTriggerElement)src;

            if (Unit.Set == src.Unit.Set)
            {
                Selection = other.Selection;
            }
            else if (other.Selection != null)
            {
                Stack<int> indices = new Stack<int>(Depth);

                MapElement root = other.Root;
                MapElement current = other.Selection;

                while (current != root && current.Parent != null)
                {
                    if (current.Parent.IsListed)
                    {
                        indices.Push(current.Index);
                    }
                    current = current.Parent;
                }
                current = Root;
                while (indices.Count != 0)
                {
                    int idx = indices.Pop();
                    if (idx >= current.Children.Count)
                    {
                        current = null;
                        break;
                    }
                    current = current.Children[idx];
                }
                Selection = current;
            }
            else Selection = null;
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "mapObject")
            {
                XmlAttribute attr = node.Attributes["selection"];

                if (attr != null)
                {
                    MapElement selection = Root;

                    if (!node.Attributes["selection"].Value== string.Empty)
                    {
                        foreach (string str in node.Attributes["selection"].Value.Split(','))
                        {
                            int idx = int.Parse(str);

                            selection = selection.Children[idx];
                        }
                    }
                    Selection = selection;
                }
                else
                {
                    Selection = null;
                }
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("mapObject");
            writer.WriteAttributeString("name", Name);
            if (_Selection != null)
            {
                MapElement current = _Selection;

                Stack<int> indices = new Stack<int>(Depth);

                while (current != Root)
                {
                    if (current.Parent == null)
                    {
                        throw new AssetException(Owner, "잘못된 오브젝트 참조 트리거가 있습니다.");
                    }
                    if (current.Parent.IsListed)
                    {
                        indices.Push(current.Index);
                    }
                    current = current.Parent;
                }
                StringBuilder builder = new StringBuilder();
                current = Root;
                while (indices.Count != 0)
                {
                    int idx = indices.Pop();

                    builder.Append(idx);

                    if (indices.Count != 0)
                    {
                        builder.Append(',');
                    }
                }
                writer.WriteAttributeString("selection", builder.ToString());
            }
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            if (_Selection != null)
            {
                MapElement current = _Selection;

                Stack<int> indices = new Stack<int>(Depth);

                while (current != Root)
                {
                    if (current.Parent == null)
                    {
                        throw new AssetException(Owner, "잘못된 오브젝트 참조 트리거가 있습니다.");
                    }
                    if (current.Parent.IsListed)
                    {
                        indices.Push(current.Index);
                    }
                    current = current.Parent;
                }
                if (indices.Count != Depth)
                {
                    throw new AssetException(Owner, "잘못된 오브젝트 참조 트리거가 있습니다.");
                }
                while (indices.Count != 0)
                {
                    int idx = indices.Pop();

                    writer.Write(Boundary, idx);
                }
            }
            else
            {
                throw new AssetException(Owner, "입력되지 않은 트리거가 있습니다.");
            }
        }

        internal override int Size
        {
            get
            {
                return Boundary.GetSize() * Depth;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (_Selection == null)
                {
                    return false;
                }
                MapElement current = _Selection;

                int count = 0;
                while (current != Root)
                {
                    if (current.Parent == null)
                    {
                        return false;
                    }
                    if (current.Parent.IsListed)
                    {
                        count++;
                    }
                    current = current.Parent;
                }
                return count == Depth;
            }
        }
        public override string ToString()
        {
            return _Selection != null ? _Selection.Name : "미입력";
        }
    }
    */
}
