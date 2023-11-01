using System.Collections.Generic;
using System.IO;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Terrain
{
    public class TerrainSurface : AssetElement
    {
        public TerrainAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private float _Scale;
        public float Scale
        {
            set => SetProperty(ref _Scale, value);
            get => _Scale;
        }

        private float _Rotation;
        public float Rotation
        {
            set => SetProperty(ref _Rotation, value);
            get => _TriPlaner ? 0 : _Rotation;
        }

        private bool _TriPlaner;
        public bool TriPlaner
        {
            set 
            {
                if (SetProperty(ref _TriPlaner, value)) OnPropertyChanged("Rotation");
            }
            get => _TriPlaner;
        }

        public Material Material { private set; get; }

        internal int Key { private set; get; }

        private static int _keySeed;

        public TerrainSurface(TerrainAsset parent)
        {
            Parent = parent;

            Key = ++_keySeed;

            _Scale = 1;

            Material = new Material(this, MaterialUsage.TerrainSurface);
        }

        public TerrainSurface(TerrainAsset parent, TerrainSurface other)
        {
            Parent = parent;

            Key = ++_keySeed;

            _Scale = other._Scale;
            _Rotation = other._Rotation;
            _TriPlaner = other._TriPlaner;

            Material = new Material(this, other.Material, MaterialUsage.TerrainSurface);
        }

        internal TerrainSurface(TerrainAsset parent, XmlNode node)
        {
            Parent = parent;

            Key = ++_keySeed;

            _Scale = node.ReadAttributeFloat("scale", 1);
            _Rotation = node.ReadAttributeFloat("rotation");
            _TriPlaner = node.ReadAttributeBool("triPlaner");

            Material = new Material(this, MaterialUsage.TerrainSurface);
            Material.Load(node.GetChildNode("material"));
        }

        internal override void AddRetains(ICollection<string> retains) => Material.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Material.IsRetaining(element, out from);

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("surface");
            writer.WriteAttribute("scale", _Scale, 1);
            writer.WriteAttribute("rotation", _Rotation);
            writer.WriteAttribute("triPlaner", _TriPlaner);
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(_Scale);
            writer.Write(_Rotation * MathUtil.ToRadians);
            writer.Write(_TriPlaner);
            Material.Build(writer);
        }
    }
}
