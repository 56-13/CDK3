using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Meshing;

namespace CDK.Assets.Terrain
{
    public class TerrainWall : AssetElement
    {
        public TerrainAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _Name;
        public string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;
                SetProperty(ref _Name, value);
            }
            get => _Name ?? "Wall";
        }
        public MeshSelection Selection { private set; get; }
        public AssetElementList<string> Bones { private set; get; }

        public int Index => Parent.Walls.IndexOf(this);

        public TerrainWall(TerrainAsset parent)
        {
            Parent = parent;

            Selection = new MeshSelection(this);
            Bones = new AssetElementList<string>(this);
        }

        public TerrainWall(TerrainAsset parent, TerrainWall other)
        {
            Parent = parent;
            
            _Name = other.Name;

            Selection = new MeshSelection(this, other.Selection);
            Bones = new AssetElementList<string>(this, other.Bones, false, false);
        }

        internal TerrainWall(TerrainAsset parent, XmlNode node)
        {
            Parent = parent;

            _Name = node.ReadAttributeString("name");
            Selection = new MeshSelection(this);
            Selection.Load(node.GetChildNode("selection"));

            Bones = new AssetElementList<string>(this);
            foreach (var bone in node.ReadAttributeStrings("bones")) Bones.Add(bone);
        }

        internal override void AddRetains(ICollection<string> retains) => Selection.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Selection.IsRetaining(element, out from);

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("wall");
            writer.WriteAttribute("name", _Name);
            writer.WriteAttributes("bones", Bones);
            Selection.Save(writer);
            writer.WriteEndElement();
        }
    }
}
