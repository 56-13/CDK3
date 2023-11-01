using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class AssetTriggerElement : TriggerElement
    {
        public int Depth { private set; get; }
        public Asset Root { private set; get; }
        public bool AllowEmpty { private set; get; }

        private Asset _Selection;
        public Asset Selection
        {
            set => SetProperty(ref _Selection, value);
            get => _Selection;
        }

        public AssetTriggerElement(TriggerUnit parent, XmlNode node) : base(parent, node)
        {
            Depth = node.ReadAttributeInt("depth");

            var rootPath = node.ReadAttributeString("root");

            if (rootPath.StartsWith("relative,"))
            {
                var root = Owner;

                foreach (var indexStr in rootPath.Substring(9).Split(','))
                {
                    var index = int.Parse(indexStr);

                    if (index < 0)
                    {
                        for (var i = 0; i < -index; i++)
                        {
                            root = root.Parent;

                            if (root == null) throw new XmlException();
                        }
                    }
                    else root = root.Children[index];
                }
                Root = root;
            }
            else Root = AssetManager.Instance.GetAsset(rootPath);

            if (Depth < 1 || Root == null) throw new XmlException();

            AllowEmpty = node.ReadAttributeBool("allowEmpty");

            var selection = node.ReadAttributeString("selection");

            if (selection != null) _Selection = AssetManager.Instance.GetAsset(selection);
        }

        public override TriggerElementType Type => TriggerElementType.Asset;

        internal override void AddRetains(ICollection<string> retains)
        {
            if (Root != null) retains.Add(Root.Key);

            if (_Selection != null) retains.Add(_Selection.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Root == element || _Selection == element)
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
            AssetManager.Instance.InvokeRedirection(() =>
            {
                Selection = AssetManager.Instance.GetRedirection(((AssetTriggerElement)src).Selection);
            });
        }

        internal override void Load(XmlNode node)
        {
            if (node.LocalName == "asset")
            {
                var selection = node.ReadAttributeString("selection");

                Selection = selection != null ? AssetManager.Instance.GetAsset(selection) : null;
            }
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("asset");
            writer.WriteAttributeString("name", Name);
            if (_Selection != null) writer.WriteAttributeString("selection", _Selection.Key);
            writer.WriteEndElement();
        }

        internal override void Build(BinaryWriter writer)
        {
            BuildReference(writer, Root, _Selection, Depth, AllowEmpty);
        }

        internal override int Size => Depth * sizeof(short);
        public override bool IsValid => IsValidReference(Root, _Selection, Depth, AllowEmpty);
        public override string ToString()
        {
            return _Selection != null ? (_Selection.Tag ?? _Selection.Name) : "미입력";
        }
    }
}
