using System.Xml;

namespace CDK.Assets.Spawn
{
    public class SpawnCollisionTarget : AssetElement
    {
        public SpawnAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _Source;
        public string Source
        {
            set => SetProperty(ref _Source, value);
            get => _Source;
        }

        private float _Distance;
        public float Distance
        {
            set => SetProperty(ref _Distance, value);
            get => _Distance;
        }

        private bool _Square;
        public bool Square
        {
            set => SetProperty(ref _Square, value);
            get => _Square;
        }

        public SpawnCollisionTarget(SpawnAsset parent)
        {
            Parent = parent;
        }

        public SpawnCollisionTarget(SpawnAsset parent, SpawnCollisionTarget other)
        {
            Parent = parent;

            _Source = other._Source;
            _Distance = other._Distance;
            _Square = other._Square;
        }

        internal SpawnCollisionTarget(SpawnAsset parent, XmlNode node)
        {
            if (node.LocalName != "target") throw new XmlException();

            Parent = parent;

            _Source = node.ReadAttributeString("source");
            _Distance = node.ReadAttributeFloat("distance");
            _Square = node.ReadAttributeBool("square");
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("target");
            writer.WriteAttribute("source", _Source);
            writer.WriteAttribute("distance", _Distance);
            writer.WriteAttribute("square", _Square);
            writer.WriteEndElement();
        }
    }
}
