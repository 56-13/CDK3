using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Sources;

namespace CDK.Assets.Animations.Trail
{
    public class Trail : AssetElement, IAnimationSubstance
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public AssetElementList<AnimationSource> Sources { private set; get; }

        private float _Distance;
        public float Distance
        {
            set => SetProperty(ref _Distance, value);
            get => _Distance;
        }

        private bool _Billboard;
        public bool Billboard
        {
            set => SetProperty(ref _Billboard, value);
            get => _Billboard;
        }

        private bool _LocalSpace;
        public bool LocalSpace
        {
            set => SetProperty(ref _LocalSpace, value);
            get => _LocalSpace;
        }

        private bool _Emission;
        public bool Emission
        {
            set => SetProperty(ref _Emission, value);
            get => _Emission;
        }

        private float _EmissionLife;
        public float EmissionLife
        {
            set => SetProperty(ref _EmissionLife, value);
            get => _EmissionLife;
        }

        private int _EmissionSmoothness;
        public int EmissionSmoothness
        {
            set => SetProperty(ref _EmissionSmoothness, value);
            get => _EmissionSmoothness;
        }

        private float _RepeatScale;
        public float RepeatScale
        {
            set => SetProperty(ref _RepeatScale, value);
            get => _RepeatScale;
        }

        public AnimationColor Color { private set; get; }

        private float _ColorDuration;
        public float ColorDuration
        {
            set => SetProperty(ref _ColorDuration, value); 
            get => _ColorDuration;
        }

        private AnimationLoop _ColorLoop;
        public AnimationLoop ColorLoop
        {
            set => SetProperty(ref _ColorLoop, value); 
            get => _ColorLoop;
        }

        public AnimationFloat Rotation { private set; get; }

        private float _RotationDuration;
        public float RotationDuration
        {
            set => SetProperty(ref _RotationDuration, value);
            get => _RotationDuration;
        }

        private AnimationLoop _RotationLoop;
        public AnimationLoop RotationLoop
        {
            set => SetProperty(ref _RotationLoop, value);
            get => _RotationLoop;
        }

        public AnimationFloat Scale { private set; get; }

        private float _ScaleDuration;
        public float ScaleDuration
        {
            set => SetProperty(ref _ScaleDuration, value);
            get => _ScaleDuration;
        }

        private AnimationLoop _ScaleLoop;
        public AnimationLoop ScaleLoop
        {
            set => SetProperty(ref _ScaleLoop, value);
            get => _ScaleLoop;
        }

        private const float DefaultDistance = 10;
        private const int DefaultEmissionSmoothness = 2;

        public Trail(AssetElement parent)
        {
            Parent = parent;

            Sources = new AssetElementList<AnimationSource>(this);

            _Distance = DefaultDistance;

            _EmissionSmoothness = DefaultEmissionSmoothness;

            _RepeatScale = 1;

            Color = new AnimationColor(this, true, true, Color4.White);

            Rotation = new AnimationFloat(this, -3600, 3600, 0);
            Scale = new AnimationFloat(this, 0.01f, 100, 1);
        }

        public Trail(AssetElement parent, Trail other)
        {
            Parent = parent;

            Sources = new AssetElementList<AnimationSource>(this);
            using (new AssetCommandHolder())
            {
                foreach (var source in other.Sources) Sources.Add(source.Clone(this));
            }

            _Distance = other._Distance;
            _Billboard = other._Billboard;
            _LocalSpace = other._LocalSpace;
            _Emission = other._Emission;
            _EmissionLife = other._EmissionLife;
            _EmissionSmoothness = other._EmissionSmoothness;
            _RepeatScale = other._RepeatScale;

            Color = new AnimationColor(this, other.Color);
            _ColorDuration = other._ColorDuration;
            _ColorLoop = other._ColorLoop;

            Rotation = new AnimationFloat(this, other.Rotation);
            _RotationDuration = other._RotationDuration;
            _RotationLoop = other._RotationLoop;

            Scale = new AnimationFloat(this, other.Scale);
            _ScaleDuration = other._ScaleDuration;
            _ScaleLoop = other._ScaleLoop;
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
            from = null;
            return false;
        }

        internal void Build(BinaryWriter writer)
        {
            writer.WriteLength(Sources.Count);
            foreach (var source in Sources) source.Build(writer);

            writer.Write(_Distance);
            writer.Write(_Billboard);
            writer.Write(_LocalSpace);
            writer.Write(_Emission);
            if (_Emission)
            {
                writer.Write(_EmissionLife);
                writer.Write(_EmissionSmoothness);
            }
            writer.Write(_RepeatScale);

            Color.Build(writer);
            writer.Write(_ColorDuration);
            _ColorLoop.Build(writer);

            Rotation.Build(writer, true);
            writer.Write(_RotationDuration);
            _RotationLoop.Build(writer);

            Scale.Build(writer, false);
            writer.Write(_ScaleDuration);
            _ScaleLoop.Build(writer);
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("trail");

            writer.WriteAttribute("distance", _Distance, DefaultDistance);
            writer.WriteAttribute("billboard", _Billboard);
            writer.WriteAttribute("localSpace", _LocalSpace);
            writer.WriteAttribute("emission", _Emission);
            writer.WriteAttribute("emissionLife", _EmissionLife);
            writer.WriteAttribute("emissionSmoothness", _EmissionSmoothness, DefaultEmissionSmoothness);
            writer.WriteAttribute("repeatScale", _RepeatScale);
            Color.Save(writer, "color");
            writer.WriteAttribute("colorDuration", _ColorDuration);
            _ColorLoop.Save(writer, "colorLoop");
            Rotation.Save(writer, "rotation");
            writer.WriteAttribute("rotationDuration", _RotationDuration);
            _RotationLoop.Save(writer, "rotationLoop");
            Scale.Save(writer, "scale");
            writer.WriteAttribute("scaleDuration", _ScaleDuration);
            _ScaleLoop.Save(writer, "scaleLoop");

            writer.WriteStartElement("sources");
            foreach (var source in Sources) source.Save(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "trail") throw new XmlException();

            Sources.Clear();
            foreach (XmlNode subnode in node.GetChildNode("sources").ChildNodes)
            {
                Sources.Add(AnimationSource.Load(this, subnode));
            }

            Distance = node.ReadAttributeFloat("distance", DefaultDistance);
            Billboard = node.ReadAttributeBool("billboard");
            LocalSpace = node.ReadAttributeBool("localSpace");
            Emission = node.ReadAttributeBool("emission");
            EmissionLife = node.ReadAttributeFloat("emissionLife");
            EmissionSmoothness = node.ReadAttributeInt("emissionSmoothness", DefaultEmissionSmoothness);
            RepeatScale = node.ReadAttributeFloat("repeatScale");

            Color.Load(node, "color");
            ColorDuration = node.ReadAttributeFloat("colorDuration");
            ColorLoop = new AnimationLoop(node, "colorLoop");

            Rotation.Load(node, "rotation");
            RotationDuration = node.ReadAttributeFloat("rotationDuration");
            RotationLoop = new AnimationLoop(node, "rotationLoop");

            Scale.Load(node, "scale");
            ScaleDuration = node.ReadAttributeFloat("scaleDuration");
            ScaleLoop = new AnimationLoop(node, "scaleLoop");
        }

        SceneComponentType IAnimationSubstance.Type => SceneComponentType.Trail;
        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new TrailObject(this) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new Trail(parent, this);
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) { }
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => Build(writer);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) { }
    }
}
