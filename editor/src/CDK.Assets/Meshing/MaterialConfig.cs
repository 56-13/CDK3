using System.Collections.Generic;
using System.Xml;

using CDK.Assets.Texturing;

namespace CDK.Assets.Meshing
{
    public class MaterialConfig : AssetElement
    {
        public MeshGeometry Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public string Name { private set; get; }

        private MaterialAsset _Origin;
        public MaterialAsset Origin
        {
            set => SetProperty(ref _Origin, value);
            get => _Origin;
        }

        public MaterialConfig(MeshGeometry parent, string name)
        {
            Parent = parent;

            Name = name;
        }

        public MaterialConfig(MeshGeometry parent, string name, MaterialConfig other)
        {
            Parent = parent;

            Name = name;

            AssetManager.Instance.InvokeRedirection(() =>
            {
                _Origin = AssetManager.Instance.GetRedirection(other._Origin);
            });
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Origin != null) retains.Add(_Origin.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (element == _Origin)
            {
                from = this;
                return true;
            }

            from = null;
            return false;
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "material") throw new XmlException();

            Origin = (MaterialAsset)node.ReadAttributeAsset("origin");
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("material");
            writer.WriteAttribute("origin", _Origin);
            writer.WriteEndElement();
        }
    }
}
