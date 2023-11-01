using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Sources;
using CDK.Assets.Animations.Particle.Shapes;

namespace CDK.Assets.Animations.Particle
{
    public enum ParticleView
    {
        None,
        Billboard,
        HorizontalBillboard,
        VerticalBillboard,
        StretchBillboard
    }

    public class Particle  : AssetElement, IAnimationSubstance
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public AssetElementList<AnimationSource> Sources { private set; get; }

        public ParticleShapeType ShapeType
        {
            set
            {
                if (ShapeType != value) Shape = ParticleShape.Create(this, value);
            }
            get => _Shape.Type;
        }

        private ParticleShape _Shape;
        public ParticleShape Shape 
        {
            private set
            {
                if (SetProperty(ref _Shape, value)) OnPropertyChanged("ShapeType");
            }
            get => _Shape;
        }

        private bool _ShapeShell;
        public bool ShapeShell 
        {
            set => SetProperty(ref _ShapeShell, value);
            get => _ShapeShell;
        }

        public AnimationColor Color { private set; get; }
        public AnimationFloat Radial { private set; get; }
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }
        public AnimationFloat BillboardX { private set; get; }
        public AnimationFloat BillboardY { private set; get; }
        public AnimationFloat BillboardZ { private set; get; }
        public AnimationFloat RotationX { private set; get; }
        public AnimationFloat RotationY { private set; get; }
        public AnimationFloat RotationZ { private set; get; }
        public AnimationFloat ScaleX { private set; get; }
        public AnimationFloat ScaleY { private set; get; }
        public AnimationFloat ScaleZ { private set; get; }

        private bool _ScaleEach;
        public bool ScaleEach
        {
            set => SetProperty(ref _ScaleEach, value);
            get => _ScaleEach;
        }

        private ParticleView _View;
        public ParticleView View
        {
            set => SetProperty(ref _View, value);
            get => _View;
        }

        private float _StretchRate;
        public float StretchRate
        {
            set => SetProperty(ref _StretchRate, value);
            get => _StretchRate;
        }

        private bool _LocalSpace;
        public bool LocalSpace
        {
            set => SetProperty(ref _LocalSpace, value);
            get => _LocalSpace;
        }

        private bool _Prewarm;
        public bool Prewarm
        {
            set => SetProperty(ref _Prewarm, value);
            get => _Prewarm;
        }

        private bool _Finish;
        public bool Finish
        {
            set => SetProperty(ref _Finish, value);
            get => _Finish;
        }

        private bool _Clear;
        public bool Clear
        {
            set => SetProperty(ref _Clear, value);
            get => _Clear;
        }

        public AnimationFloatConstant Life { private set; get; }
        
        private float _EmissionRate;
        public float EmissionRate
        {
            set => SetProperty(ref _EmissionRate, value);
            get => _EmissionRate;
        }

        private int _EmissionMax;
        public int EmissionMax
        {
            set => SetProperty(ref _EmissionMax, value);
            get => _EmissionMax;
        }

        public Particle(AssetElement parent)
        {
            Parent = parent;

            Sources = new AssetElementList<AnimationSource>(this);

            _Shape = new ParticleShapeSphere(this);
            
            Color = new AnimationColor(this, true, true, Color4.White);

            Radial = new AnimationFloat(this, -10000, 10000, 0);
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            BillboardX = new AnimationFloat(this, -10000, 10000, 0);
            BillboardY = new AnimationFloat(this, -10000, 10000, 0);
            BillboardZ = new AnimationFloat(this, -10000, 10000, 0);

            RotationX = new AnimationFloat(this, -3600, 3600, 0);
            RotationY = new AnimationFloat(this, -3600, 3600, 0);
            RotationZ = new AnimationFloat(this, -3600, 3600, 0);

            ScaleX = new AnimationFloat(this, 0.01f, 100, 1);
            ScaleY = new AnimationFloat(this, 0.01f, 100, 1);
            ScaleZ = new AnimationFloat(this, 0.01f, 100, 1);

            _LocalSpace = true;

            Life = new AnimationFloatConstant(this, 0, 60, 0);
        }

        public Particle(AssetElement parent, Particle other)
        {
            Parent = parent;

            Sources = new AssetElementList<AnimationSource>(this);
            using (new AssetCommandHolder())
            {
                foreach (var source in other.Sources) Sources.Add(source.Clone(this));
            }

            _Shape = other._Shape.Clone(this);
            _ShapeShell = other._ShapeShell;

            Life = new AnimationFloatConstant(this, other.Life);
            _EmissionRate = other._EmissionRate;
            _EmissionMax = other._EmissionMax;
            _Prewarm = other._Prewarm;

            Color = new AnimationColor(this, other.Color);
            Radial = new AnimationFloat(this, other.Radial);
            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            BillboardX = new AnimationFloat(this, other.BillboardX);
            BillboardY = new AnimationFloat(this, other.BillboardY);
            BillboardZ = new AnimationFloat(this, other.BillboardZ);

            RotationX = new AnimationFloat(this, other.RotationX);
            RotationY = new AnimationFloat(this, other.RotationY);
            RotationZ = new AnimationFloat(this, other.RotationZ);

            ScaleX = new AnimationFloat(this, other.ScaleX);
            ScaleY = new AnimationFloat(this, other.ScaleY);
            ScaleZ = new AnimationFloat(this, other.ScaleZ);
            _ScaleEach = other._ScaleEach;

            _View = other._View;
            _StretchRate = other._StretchRate;
            _LocalSpace = other._LocalSpace;
            _Prewarm = other._Prewarm;
            _Finish = other._Finish;
            _Clear = other._Clear;

            Life = new AnimationFloatConstant(this, other.Life);
            _EmissionRate = other._EmissionRate;
            _EmissionMax = other._EmissionMax;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var source in Sources) source.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var source in Sources)
            {
                if (source.IsRetaining(element, out from)) return true;
            }
            from = this;
            return false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("particle");

            _Shape.Save(writer);
            writer.WriteAttribute("shapeShell", _ShapeShell);

            Color.Save(writer, "color");

            Radial.Save(writer, "radial");
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            BillboardX.Save(writer, "billboardX");
            BillboardY.Save(writer, "billboardY");
            BillboardZ.Save(writer, "billboardZ");

            RotationX.Save(writer, "rotationX");
            RotationY.Save(writer, "rotationY");
            RotationZ.Save(writer, "rotationZ");

            ScaleX.Save(writer, "scaleX");
            ScaleY.Save(writer, "scaleY");
            ScaleZ.Save(writer, "scaleZ");
            writer.WriteAttribute("scaleEach", _ScaleEach);

            writer.WriteAttribute("view", _View);
            writer.WriteAttribute("stretchRate", _StretchRate);
            writer.WriteAttribute("localSpace", _LocalSpace);
            writer.WriteAttribute("prewarm", _Prewarm);
            writer.WriteAttribute("finish", _Finish, true);
            writer.WriteAttribute("clear", _Clear);
            Life.Save(writer, "life");
            writer.WriteAttribute("emissionRate", _EmissionRate);
            writer.WriteAttribute("emissionMax", _EmissionMax);

            writer.WriteStartElement("sources");
            foreach (var source in Sources) source.Save(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            Shape = ParticleShape.Load(this, node);
            ShapeShell = node.ReadAttributeBool("shapeShell");

            Color.Load(node, "color");

            Radial.Load(node, "radial");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            BillboardX.Load(node, "billboardX");
            BillboardY.Load(node, "billboardY");
            BillboardZ.Load(node, "billboardZ");

            RotationX.Load(node, "rotationX");
            RotationY.Load(node, "rotationY");
            RotationZ.Load(node, "rotationZ");

            ScaleX.Load(node, "scaleX");
            ScaleY.Load(node, "scaleY");
            ScaleZ.Load(node, "scaleZ");
            ScaleEach = node.ReadAttributeBool("scaleEach");

            View = node.ReadAttributeEnum<ParticleView>("view");
            StretchRate = node.ReadAttributeFloat("stretchRate");
            LocalSpace = node.ReadAttributeBool("localSpace");
            Prewarm = node.ReadAttributeBool("prewarm");
            Finish = node.ReadAttributeBool("finish", true);
            Clear = node.ReadAttributeBool("clear");
            Life.Load(node, "life");
            EmissionRate = node.ReadAttributeFloat("emissionRate");
            EmissionMax = node.ReadAttributeInt("emissionMax");

            Sources.Clear();
            foreach (XmlNode subnode in node.GetChildNode("sources").ChildNodes) Sources.Add(AnimationSource.Load(this, subnode));
        }

        internal void Build(BinaryWriter writer)
        {
            writer.WriteLength(Sources.Count);
            foreach (var source in Sources) source.Build(writer);

            writer.Write((byte)_Shape.Type);
            _Shape.Build(writer);
            writer.Write(_ShapeShell);
            Life.Build(writer, false);
            writer.Write(_EmissionRate);
            writer.Write(_EmissionMax);
            writer.Write(_Prewarm);
            writer.Write(_Finish);
            writer.Write(_Clear);
           
            Color.Build(writer);

            Radial.Build(writer, false);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            BillboardX.Build(writer, false);
            BillboardY.Build(writer, false);
            BillboardZ.Build(writer, false);

            RotationX.Build(writer, true);
            RotationY.Build(writer, true);
            RotationZ.Build(writer, true);

            ScaleX.Build(writer, false);
            ScaleY.Build(writer, false);
            ScaleZ.Build(writer, false);
            writer.Write(_ScaleEach);

            writer.Write((byte)_View);
            if (_View == ParticleView.StretchBillboard)
            {
                writer.Write(_StretchRate);
            }
            writer.Write(_LocalSpace);

            writer.WriteLength(Sources.Count);
            foreach (var source in Sources) source.Build(writer);
        }

        SceneComponentType IAnimationSubstance.Type => SceneComponentType.Particle;
        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new ParticleObject(this) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new Particle(parent, this);
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) { }
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => Build(writer);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) { }
    }
}
