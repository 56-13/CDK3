using System.Xml;
using System.IO;

namespace CDK.Assets.Terrain
{
    public class TerrainWallInstancePoint : AssetElement
    {
        public TerrainWallInstance Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public int X { private set; get; }
        public int Y { private set; get; }

        private float _Z;
        public float Z
        {
            set
            {
                if (SetProperty(ref _Z, value)) Parent.Update();
            }
            get => _Z;
        }

        internal TerrainWallInstancePoint(TerrainWallInstance parent, int x, int y, float z)
        {
            Parent = parent;

            X = x;
            Y = y;
            _Z = z;
        }

        internal TerrainWallInstancePoint(TerrainWallInstance parent, XmlNode node)
        {
            Parent = parent;

            X = node.ReadAttributeInt("x");
            Y = node.ReadAttributeInt("y");
            _Z = node.ReadAttributeFloat("z");
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("wallInstancePoint");
            writer.WriteAttribute("x", X);
            writer.WriteAttribute("y", Y);
            writer.WriteAttribute("z", _Z);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}
