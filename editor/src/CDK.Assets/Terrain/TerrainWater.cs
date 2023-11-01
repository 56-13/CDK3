using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Terrain
{
    public class TerrainWater : AssetElement
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
            get => _Name ?? "Water";
        }

        public Material Material { private set; get; }

        private float _TextureScale;
        public float TextureScale
        {
            set => SetProperty(ref _TextureScale, value);
            get => _TextureScale;
        }

        private float _PerturbIntensity;
        public float PerturbIntensity
        {
            set => SetProperty(ref _PerturbIntensity, value);
            get => _PerturbIntensity;
        }

        private RootImageAsset _FoamTexture;
        public RootImageAsset FoamTexture
        {
            set => SetProperty(ref _FoamTexture, value);
            get => _FoamTexture;
        }

        private float _FoamScale;
        public float FoamScale
        {
            set => SetProperty(ref _FoamScale, value); 
            get => _FoamScale;
        }

        private float _FoamIntensity;
        public float FoamIntensity
        {
            set => SetProperty(ref _FoamIntensity, value);
            get => _FoamIntensity;
        }

        private float _FoamDepth;
        public float FoamDepth
        {
            set => SetProperty(ref _FoamDepth, value); 
            get => _FoamDepth;
        }

        private float _Angle;
        public float Angle
        {
            set => SetProperty(ref _Angle, value); 
            get => _Angle;
        }

        private float _ForwardSpeed;
        public float ForwardSpeed
        {
            set => SetProperty(ref _ForwardSpeed, value); 
            get => _ForwardSpeed;
        }

        private float _CrossSpeed;
        public float CrossSpeed
        {
            set => SetProperty(ref _CrossSpeed, value); 
            get => _CrossSpeed;
        }

        private float _WaveDistance;
        public float WaveDistance
        {
            set => SetProperty(ref _WaveDistance, value); 
            get => _WaveDistance;
        }

        private float _WaveAltitude;
        public float WaveAltitude
        {
            set => SetProperty(ref _WaveAltitude, value); 
            get => _WaveAltitude;
        }

        private float _DepthMax;
        public float DepthMax
        {
            set => SetProperty(ref _DepthMax, value); 
            get => _DepthMax;
        }

        private Color4 _ShallowColor;
        public Color4 ShallowColor
        {
            set => SetProperty(ref _ShallowColor, value); 
            get => _ShallowColor;
        }

        public int Index => Parent.Waters.IndexOf(this);

        public const float PerturbIntensityDefault = 0.05f;

        public TerrainWater(TerrainAsset parent)
        {
            Parent = parent;

            Material = new Material(this, MaterialUsage.TerrainWater);

            _TextureScale = 1;
            _PerturbIntensity = PerturbIntensityDefault;
            _FoamScale = 1;
            _FoamIntensity = 1;
            _ShallowColor = Color4.White;
        }

        public TerrainWater(TerrainAsset parent, TerrainWater other)
        {
            Parent = parent;

            _Name = other._Name;

            Material = new Material(this, other.Material, MaterialUsage.TerrainWater);

            _TextureScale = other._TextureScale;
            _PerturbIntensity = other._PerturbIntensity;
            _FoamScale = other._FoamScale;
            _FoamIntensity = other._FoamIntensity;
            _FoamDepth = other._FoamDepth;
            _Angle = other._Angle;
            _ForwardSpeed = other._ForwardSpeed;
            _CrossSpeed = other._CrossSpeed;
            _WaveDistance = other._WaveDistance;
            _WaveAltitude = other._WaveAltitude;
            _DepthMax = other._DepthMax;
            _ShallowColor = other._ShallowColor;
        }

        internal TerrainWater(TerrainAsset parent, XmlNode node)
        {
            Parent = parent;

            Material = new Material(this, MaterialUsage.TerrainWater);
            Material.Load(node.GetChildNode("material"));

            _Name = node.ReadAttributeString("name");
            _TextureScale = node.ReadAttributeFloat("textureScale", 1);
            _PerturbIntensity = node.ReadAttributeFloat("perturbIntensity", 1);
            _FoamTexture = (RootImageAsset)node.ReadAttributeAsset("foamTexture");
            _FoamScale = node.ReadAttributeFloat("foamScale", 1);
            _FoamIntensity = node.ReadAttributeFloat("foamIntensity", 1);
            _FoamDepth = node.ReadAttributeFloat("foamDepth");
            _Angle = node.ReadAttributeFloat("angle");
            _ForwardSpeed = node.ReadAttributeFloat("forwardSpeed");
            _CrossSpeed = node.ReadAttributeFloat("crossSpeed");
            _WaveDistance = node.ReadAttributeFloat("waveDistance");
            _WaveAltitude = node.ReadAttributeFloat("waveAltitude");
            _DepthMax = node.ReadAttributeFloat("depthMax");
            _ShallowColor = node.ReadAttributeColor4("shallowColor", true);
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_FoamTexture != null) retains.Add(_FoamTexture.Key);

            Material.AddRetains(retains);
        }
        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_FoamTexture == element)
            {
                from = this;
                return true;
            }

            return Material.IsRetaining(element, out from);
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("water");
            writer.WriteAttribute("name", _Name);
            writer.WriteAttribute("perturbScale", _TextureScale, 1);
            writer.WriteAttribute("perturbIntensity", _PerturbIntensity, 1);
            writer.WriteAttribute("foamTexture", _FoamTexture);
            writer.WriteAttribute("foamScale", _FoamScale, 1);
            writer.WriteAttribute("foamIntensity", _FoamIntensity, 1);
            writer.WriteAttribute("foamDepth", _FoamDepth);
            writer.WriteAttribute("angle", _Angle);
            writer.WriteAttribute("forwardSpeed", _ForwardSpeed);
            writer.WriteAttribute("crossSpeed", _CrossSpeed);
            writer.WriteAttribute("waveDistance", _WaveDistance);
            writer.WriteAttribute("waveAltitude", _WaveAltitude);
            writer.WriteAttribute("depthMax", _DepthMax);
            writer.WriteAttribute("shallowColor", _ShallowColor, true);

            Material.Save(writer);

            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(_TextureScale);
            writer.Write(_PerturbIntensity);
            BuildReference(writer, _FoamTexture);
            writer.Write(_FoamScale);
            writer.Write(_FoamIntensity);
            writer.Write(_FoamDepth);
            writer.Write(_Angle * MathUtil.ToRadians);
            writer.Write(_ForwardSpeed);
            writer.Write(_CrossSpeed);
            writer.Write(_WaveDistance * MathUtil.ToRadians);
            writer.Write(_WaveAltitude);
            writer.Write(_DepthMax);
            writer.Write(_ShallowColor, true);

            Material.Build(writer);
        }
    }
}
