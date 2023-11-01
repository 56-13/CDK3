using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Support;
using CDK.Assets.Media;
using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Derivations;

namespace CDK.Assets.Animations
{
    public class AnimationLocaleEventArgs : EventArgs
    {
        public string Locale { private set; get; }
        public AnimationLocaleEventArgs(string locale)
        {
            Locale = locale;
        }
    }

    public class AnimationFragment : AssetElement
    {
        private AssetElement _Parent;
        public AssetElement Parent
        {
            internal set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    OnPropertyChanged("Parent");
                }
            }
            get => _Parent;
        }

        public override AssetElement GetParent() => _Parent;

        private string _Name;
        public string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;
                SetProperty(ref _Name, value);
            }
            get => _Name;
        }

        private string[] _Keys;
        public string[] Keys
        {
            set => SetProperty(ref _Keys, value);
            get => _Keys;
        }

        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }
        public AnimationFloat Radial { private set; get; }
        public AnimationFloat Tangential { private set; get; }
        public AnimationFloat TangentialAngle { private set; get; }
        
        private float _PathSpeed;
        public float PathSpeed
        {
            set => SetProperty(ref _PathSpeed, value);
            get => _PathSpeed;
        }

        private float _PathDuration;
        public float PathDuration
        {
            set => SetProperty(ref _PathDuration, value);
            get => _PathDuration;
        }

        private bool _UsingPathSpeed;
        public bool UsingPathSpeed
        {
            set => SetProperty(ref _UsingPathSpeed, value);
            get => _UsingPathSpeed;
        }

        private AnimationLoop _PathLoop;
        public AnimationLoop PathLoop
        {
            set => SetProperty(ref _PathLoop, value);
            get => _PathLoop;
        }

        private bool _Billboard;
        public bool Billboard
        {
            set => SetProperty(ref _Billboard, value);
            get => _Billboard;
        }

        private bool _Reverse;
        public bool Reverse
        {
            set => SetProperty(ref _Reverse, value);
            get => _Reverse;
        }

        private bool _Facing;
        public bool Facing
        {
            set => SetProperty(ref _Facing, value);
            get => _Facing;
        }
        public AnimationFloat RotationX { private set; get; }
        public AnimationFloat RotationY { private set; get; }
        public AnimationFloat RotationZ { private set; get; }

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

        public AnimationFloat ScaleX { private set; get; }
        public AnimationFloat ScaleY { private set; get; }
        public AnimationFloat ScaleZ { private set; get; }

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

        private bool _ScaleEach;
        public bool ScaleEach
        {
            set => SetProperty(ref _ScaleEach, value);
            get => _ScaleEach;
        }

        private bool _Pivot;
        public bool Pivot
        {
            set => SetProperty(ref _Pivot, value);
            get => _Pivot;
        }

        private bool _Stencil;
        public bool Stencil
        {
            set => SetProperty(ref _Stencil, value);
            get => _Stencil;
        }
        private bool _Closing;
        public bool Closing
        {
            set => SetProperty(ref _Closing, value);
            get => _Closing;
        }

        private int _RandomWeight;
        public int RandomWeight
        {
            set => SetProperty(ref _RandomWeight, value);
            get => _RandomWeight;
        }

        private AnimationTarget _Target;
        public AnimationTarget Target
        {
            set
            {
                if (SetProperty(ref _Target, value)) OnPropertyChanged("Binding");
            }
            get => _Target;
        }

        private string _Binding;
        public string Binding
        {
            set => SetProperty(ref _Binding, value);
            get => _Target == AnimationTarget.Origin ? _Binding : null;
        }

        private float _Duration;
        public float Duration
        {
            set => SetProperty(ref _Duration, value); 
            get => _Duration;
        }

        private float _Latency;
        public float Latency
        {
            set => SetProperty(ref _Latency, value); 
            get => _Latency;
        }

        private MediaAsset _SoundSource;
        public MediaAsset SoundSource
        {
            set => SetProperty(ref _SoundSource, value);
            get => _SoundSource;
        }

        private float _SoundVolume;
        public float SoundVolume
        {
            set => SetProperty(ref _SoundVolume, value);
            get => _SoundVolume;
        }

        private AudioControl _SoundControl;
        public AudioControl SoundControl
        {
            set => SetProperty(ref _SoundControl, value);
            get => _SoundControl;
        }

        private int _SoundLoop;
        public int SoundLoop
        {
            set => SetProperty(ref _SoundLoop, value);
            get => _SoundLoop;
        }

        private int _SoundPriority;
        public int SoundPriority
        {
            set => SetProperty(ref _SoundPriority, value);
            get => _SoundPriority;
        }

        private bool _SoundPerspective;
        public bool SoundPerspective
        {
            set => SetProperty(ref _SoundPerspective, value);
            get => _SoundPerspective;
        }

        private float _SoundLatency;
        public float SoundLatency
        {
            set => SetProperty(ref _SoundLatency, value);
            get => _SoundLatency;
        }

        private float _SoundDuration;
        public float SoundDuration
        {
            set => SetProperty(ref _SoundDuration, value);
            get => _SoundDuration;
        }

        private float _SoundDuplication;
        public float SoundDuplication
        {
            set => SetProperty(ref _SoundDuplication, value);
            get => _SoundDuplication;
        }

        private bool _SoundStop;
        public bool SoundStop
        {
            set => SetProperty(ref _SoundStop, value);
            get => _SoundStop;
        }
        
        private bool _LocaleVisible;
        public bool LocaleVisible
        {
            set => SetProperty(ref _LocaleVisible, value);
            get => _LocaleVisible;
        }

        private HashSet<string> _locales;

        private IAnimationSubstance _Substance;
        public IAnimationSubstance Substance
        {
            private set
            {
                if (SetProperty(ref _Substance, value)) OnPropertyChanged("SubstanceType");
            }
            get => _Substance;
        }

        public SceneComponentType SubstanceType
        {
            set
            {
                if (SubstanceType != value) Substance = AnimationSubstance.Create(this, value);
            }
            get => _Substance == null ? SceneComponentType.None : _Substance.Type;
        }

        private AnimationDerivation _Derivation;
        public AnimationDerivation Derivation
        {
            private set
            {
                if (SetProperty(ref _Derivation, value)) OnPropertyChanged("DerivationType");
            }
            get => _Derivation;
        }

        public AnimationDerivationType DerivationType
        {
            set
            {
                if (_Derivation.Type != value) Derivation = AnimationDerivation.Create(this, value);
            }
            get => _Derivation.Type;
        }

        private bool _DerivationFinish;
        public bool DerivationFinish
        {
            set => SetProperty(ref _DerivationFinish, value);
            get => _DerivationFinish;
        }

        public AssetElementList<AnimationFragment> Children { private set; get; }

        public bool HasPivot
        {
            get
            {
                if (_Pivot) return true;
                foreach (var child in Children)
                {
                    if (child.HasPivot) return true;
                }
                return false;
            }
        }

        public bool HasSubstance
        {
            get
            {
                if (_Substance != null) return true;
                foreach (var child in Children)
                {
                    if (child.HasSubstance) return true;
                }
                return false;
            }
        }

        public AnimationFragment()
        {
            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);
            Radial = new AnimationFloat(this, -10000, 10000, 0);
            Tangential = new AnimationFloat(this, -10000, 10000, 0);
            TangentialAngle = new AnimationFloat(this, -180, 180, 0);

            _UsingPathSpeed = true;

            RotationX = new AnimationFloat(this, -3600, 3600, 0);
            RotationY = new AnimationFloat(this, -3600, 3600, 0);
            RotationZ = new AnimationFloat(this, -3600, 3600, 0);

            ScaleX = new AnimationFloat(this, 0.01f, 100, 1);
            ScaleY = new AnimationFloat(this, 0.01f, 100, 1);
            ScaleZ = new AnimationFloat(this, 0.01f, 100, 1);

            _RandomWeight = 1;
            _DerivationFinish = true;

            _SoundVolume = 1;
            _SoundControl = AudioControl.Effect;
            _SoundPerspective = true;

            _LocaleVisible = true;
            _locales = new HashSet<string>();

            _Derivation = new AnimationDerivationMulti(this);
            Children = new AssetElementList<AnimationFragment>(this);
            Children.BeforeListChanged += Children_BeforeListChanged;
            Children.ListChanged += Animations_ListChanged;
        }

        public AnimationFragment(AnimationFragment other)
        {
            AssetManager.Instance.AddRedirection(other, this);

            _Name = other._Name;
            _Keys = other._Keys;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);
            Radial = new AnimationFloat(this, other.Radial);
            Tangential = new AnimationFloat(this, other.Tangential);
            TangentialAngle = new AnimationFloat(this, other.TangentialAngle);

            _UsingPathSpeed = other._UsingPathSpeed;
            _PathSpeed = other._PathSpeed;
            _PathDuration = other._PathDuration;
            _PathLoop = other._PathLoop;
            _Billboard = other._Billboard;
            _Reverse = other._Reverse;
            _Facing = other._Facing;

            RotationX = new AnimationFloat(this, other.RotationX);
            RotationY = new AnimationFloat(this, other.RotationY);
            RotationZ = new AnimationFloat(this, other.RotationZ);
            _RotationDuration = other._RotationDuration;
            _RotationLoop = other._RotationLoop;

            ScaleX = new AnimationFloat(this, other.ScaleX);
            ScaleY = new AnimationFloat(this, other.ScaleY);
            ScaleZ = new AnimationFloat(this, other.ScaleZ);
            _ScaleDuration = other._ScaleDuration;
            _ScaleLoop = other._ScaleLoop;
            _ScaleEach = other._ScaleEach;

            _Pivot = other._Pivot;
            _Stencil = other._Stencil;
            _Closing = other._Closing;
            _RandomWeight = other._RandomWeight;
            _Target = other._Target;
            _Binding = other._Binding;
            _Duration = other._Duration;
            _Latency = other._Latency;

            _SoundSource = other._SoundSource;
            _SoundVolume = other._SoundVolume;
            _SoundControl = other._SoundControl;
            _SoundLoop = other._SoundLoop;
            _SoundPriority = other._SoundPriority;
            _SoundPerspective = other._SoundPerspective;
            _SoundLatency = other._SoundLatency;
            _SoundDuration = other._SoundDuration;
            _SoundDuplication = other._SoundDuplication;
            _SoundStop = other._SoundStop;

            _LocaleVisible = other._LocaleVisible;
            _locales = new HashSet<string>(other._locales);

            if (other._Substance != null) _Substance = other._Substance.Clone(this);

            _Derivation = other._Derivation.Clone(this);
            _DerivationFinish = other._DerivationFinish;
            Children = new AssetElementList<AnimationFragment>(this);
            Children.BeforeListChanged += Children_BeforeListChanged;
            Children.ListChanged += Animations_ListChanged;

            using (new AssetCommandHolder())
            {
                foreach (var child in other.Children)
                {
                    Children.Add(new AnimationFragment(child));
                }
            }
        }

        public bool GetLocaleChecked(string locale) => _locales.Contains(locale);
        public bool GetLocaleChecked() => _locales.Count != 0;

        private class LocaleCommand : IAssetCommand
        {
            private AnimationFragment _animation;
            private string _locale;
            private bool _prev;
            private bool _next;

            public Asset Asset => _animation.Owner;

            public LocaleCommand(AnimationFragment animation, string locale, bool prev)
            {
                _animation = animation;
                _locale = locale;
                _prev = prev;
                _next = animation.GetLocaleChecked(locale);
            }

            public void Undo()
            {
                _animation.SetLocaleChecked(_locale, _prev);
            }

            public void Redo()
            {
                _animation.SetLocaleChecked(_locale, _next);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        public event EventHandler<AnimationLocaleEventArgs> LocaleChanged;

        public void SetLocaleChecked(string locale, bool next)
        {
            var prev = GetLocaleChecked(locale);

            if (prev != next)
            {
                if (next) _locales.Add(locale);
                else _locales.Remove(locale);

                IsDirty = true;
                
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new LocaleCommand(this, locale, prev));
                }
                LocaleChanged?.Invoke(this, new AnimationLocaleEventArgs(locale));
            }
        }

        internal bool GetLocaleVisible()
        {
            var visible = _LocaleVisible;
            if (GetLocaleChecked(AssetManager.Instance.Locale)) visible = !visible;
            return visible;
        }

        private bool AttachEnabled(AnimationFragment animation)
        {
            if (animation.Parent == this) return false;

            var current = this;

            for (; ; )
            {
                if (current == animation) return false;

                if (current.Parent is AnimationFragment parent) current = parent;
                else break;
            }

            return true;
        }

        private void Children_BeforeListChanged(object sender, BeforeListChangedEventArgs<AnimationFragment> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (!AttachEnabled(e.Object)) e.Cancel = true;
                    break;
                case ListChangedType.ItemChanged:
                    if (!AttachEnabled(e.Object)) e.Cancel = true;
                    else Children[e.NewIndex].Parent = null;
                    break;
                case ListChangedType.ItemDeleted:
                    Children[e.NewIndex].Parent = null;
                    break;
                case ListChangedType.Reset:
                    foreach (var animation in Children)
                    {
                        animation.Parent = null;
                    }
                    break;
            }
        }

        private void ChildrenAdded(int index)
        {
            var animation = Children[index];

            if (animation.Parent != this)
            {
                if (animation.Parent is AnimationFragment parent) parent.Children.Remove(animation);
                else if (animation.Parent is AnimationAsset asset) asset.Animation = null;
                else if (animation.Parent is AnimationObject obj)
                {
                    Debug.Assert(obj.Asset == null);
                    obj.Animation = null;
                }
                animation.Parent = this;
            }
        }

        private void Animations_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ChildrenAdded(e.NewIndex);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null) ChildrenAdded(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    for (var i = 0; i < Children.Count; i++) ChildrenAdded(i);
                    break;
            }
        }

        internal void GetTransformNames(ICollection<string> names)
        {
            if (_Pivot && _Name != null)
            {
                names.Add(_Name);
                Substance?.GetTransformNames(names);
            }
            foreach (var child in Children) child.GetTransformNames(names);
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_SoundSource != null) retains.Add(_SoundSource.Key);

            _Substance?.AddRetains(retains);
            
            foreach (var child in Children) child.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_SoundSource == element)
            {
                from = this;
                return true;
            }

            if (_Substance != null && _Substance.IsRetaining(element, out from)) return true;
            
            foreach (var child in Children)
            {
                if (child.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("animation");
            writer.WriteAttribute("name", _Name);
            if (_Keys != null) writer.WriteAttributes("keys", _Keys);

            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            Radial.Save(writer, "radial");
            Tangential.Save(writer, "tangential");
            TangentialAngle.Save(writer, "tangentialAngle");
            writer.WriteAttribute("usingPathSpeed", _UsingPathSpeed);
            writer.WriteAttribute("pathSpeed", _PathSpeed);
            writer.WriteAttribute("pathDuration", _PathDuration);
            _PathLoop.Save(writer, "pathLoop");
            writer.WriteAttribute("billboard", _Billboard);
            writer.WriteAttribute("reverse", _Reverse);
            writer.WriteAttribute("facing", _Facing);

            RotationX.Save(writer, "rotationX");
            RotationY.Save(writer, "rotationY");
            RotationZ.Save(writer, "rotationZ");
            writer.WriteAttribute("rotationDuration", _RotationDuration);
            _RotationLoop.Save(writer, "rotationLoop");

            ScaleX.Save(writer, "scaleX");
            ScaleY.Save(writer, "scaleY");
            ScaleZ.Save(writer, "scaleZ");
            writer.WriteAttribute("scaleDuration", _ScaleDuration);
            _ScaleLoop.Save(writer, "scaleLoop");
            writer.WriteAttribute("scaleEach", _ScaleEach);

            writer.WriteAttribute("pivot", _Pivot);
            writer.WriteAttribute("stencil", _Stencil);
            writer.WriteAttribute("closing", _Closing);
            writer.WriteAttribute("randomWeight", _RandomWeight, 1);
            writer.WriteAttribute("target", _Target, AnimationTarget.Origin);
            writer.WriteAttribute("binding", _Binding);
            writer.WriteAttribute("duration", _Duration);
            writer.WriteAttribute("latency", _Latency);

            writer.WriteAttribute("localeVisible", _LocaleVisible);
            writer.WriteAttributes("locales", _locales);

            if (_SoundSource != null)
            {
                writer.WriteStartElement("sound");
                writer.WriteAttribute("source", _SoundSource);
                writer.WriteAttribute("volume", _SoundVolume, 1);
                writer.WriteAttribute("control", _SoundControl, AudioControl.Effect);
                writer.WriteAttribute("loop", _SoundLoop);
                writer.WriteAttribute("priority", _SoundPriority);
                writer.WriteAttribute("perspective", _SoundPerspective, true);
                writer.WriteAttribute("latency", _SoundLatency);
                writer.WriteAttribute("duration", _SoundDuration);
                writer.WriteAttribute("duplication", _SoundDuplication);
                writer.WriteAttribute("stop", _SoundStop);
                writer.WriteEndElement();
            }

            if (_Substance != null)
            {
                writer.WriteStartElement("substance");
                writer.WriteAttribute("type", _Substance.Type);
                _Substance.Save(writer);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("derivation");
            writer.WriteAttribute("type", _Derivation.Type);
            writer.WriteAttribute("finish", _DerivationFinish, true);
            _Derivation.Save(writer);
            foreach (var animation in Children) animation.Save(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            Name = node.ReadAttributeString("name");
            Keys = node.HasAttribute("keys") ? node.ReadAttributeStrings("keys") : null;

            if (_Keys != null)
            {
                var keyConstants = Project.GetAnimationKeyConstants();
                foreach (var key in _Keys)
                {
                    if (!keyConstants.Any(c => c.Name == key)) throw new XmlException($"애니메이션 키에 설정되지 않은 값이 있습니다. {key}");
                }
            }

            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Radial.Load(node, "radial");
            Tangential.Load(node, "tangential");
            TangentialAngle.Load(node, "tangentialAngle");
            UsingPathSpeed = node.ReadAttributeBool("usingPathSpeed");
            PathSpeed = node.ReadAttributeFloat("pathSpeed");
            PathDuration = node.ReadAttributeFloat("pathDuration");
            PathLoop = new AnimationLoop(node, "pathLoop");
            Billboard = node.ReadAttributeBool("billboard");
            Reverse = node.ReadAttributeBool("reverse");
            Facing = node.ReadAttributeBool("facing");

            RotationX.Load(node, "rotationX");
            RotationY.Load(node, "rotationY");
            RotationZ.Load(node, "rotationZ");
            RotationDuration = node.ReadAttributeFloat("rotationDuration");
            RotationLoop = new AnimationLoop(node, "rotationLoop");

            ScaleX.Load(node, "scaleX");
            ScaleY.Load(node, "scaleY");
            ScaleZ.Load(node, "scaleZ");
            ScaleDuration = node.ReadAttributeFloat("scaleDuration");
            ScaleLoop = new AnimationLoop(node, "scaleLoop");
            ScaleEach = node.ReadAttributeBool("scaleEach");

            Pivot = node.ReadAttributeBool("pivot");
            Stencil = node.ReadAttributeBool("stencil");
            Closing = node.ReadAttributeBool("closing");
            RandomWeight = node.ReadAttributeInt("randomWeight", 1);
            Target = node.ReadAttributeEnum("target", AnimationTarget.Origin);
            Binding = node.ReadAttributeString("binding");
            Duration = node.ReadAttributeFloat("duration");
            Latency = node.ReadAttributeFloat("latency");

            LocaleVisible = node.ReadAttributeBool("localeVisible");
            foreach (var locale in _locales) SetLocaleChecked(locale, false);
            foreach (var locale in node.ReadAttributeStrings("locales")) SetLocaleChecked(locale, true);

            SoundSource = null;
            Substance = null;
            DerivationFinish = true;

            Children.Clear();
            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName)
                {
                    case "sound":
                        SoundSource = (MediaAsset)node.ReadAttributeAsset("source");
                        SoundVolume = node.ReadAttributeFloat("volume", 1);
                        SoundControl = node.ReadAttributeEnum("control", AudioControl.Effect);
                        SoundLoop = node.ReadAttributeInt("loop");
                        SoundPriority = node.ReadAttributeInt("priority");
                        SoundPerspective = node.ReadAttributeBool("perspective", true);
                        SoundLatency = node.ReadAttributeFloat("latency");
                        SoundDuration = node.ReadAttributeFloat("duration");
                        SoundDuplication = node.ReadAttributeFloat("duplication");
                        SoundStop = node.ReadAttributeBool("stop");
                        break;
                    case "substance":
                        Substance = AnimationSubstance.Create(this, subnode.ReadAttributeEnum<SceneComponentType>("type"));
                        _Substance.Load(subnode.ChildNodes[0]);
                        break;
                    case "derivation":
                        foreach (XmlNode childnode in subnode.ChildNodes)
                        {
                            var child = new AnimationFragment();
                            Children.Add(child);
                            child.Load(childnode);
                        }
                        DerivationFinish = subnode.ReadAttributeBool("finish", true);
                        Derivation = AnimationDerivation.Create(this, subnode.ReadAttributeEnum<AnimationDerivationType>("type"));
                        _Derivation.Load(subnode);
                        break;
                }
            }
        }

        internal void Build(BinaryWriter writer)
        {
            writer.WriteString(_Name);

            var keyMask = 0u;
            if (_Keys != null)
            {
                var keyConstants = Project.GetAnimationKeyConstants();
                foreach (var key in _Keys)
                {
                    var constant = keyConstants.FirstOrDefault(c => c.Name == key);
                    if (constant == null || !uint.TryParse(constant.Value, out var value)) throw new AssetException(Owner, $"애니메이션 키({key})가 없거나 적절한 값이 아닙니다.");
                    keyMask |= value;
                }
            }
            writer.Write(keyMask);

            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            Radial.Build(writer, false);
            Tangential.Build(writer, false);
            if (Tangential.Type != AnimationFloatType.None) TangentialAngle.Build(writer, true);
            writer.Write(_UsingPathSpeed ? _PathSpeed : _PathDuration);
            _PathLoop.Build(writer);
            writer.Write(_UsingPathSpeed);
            writer.Write(_Billboard);
            writer.Write(_Reverse);
            writer.Write(_Facing);

            RotationX.Build(writer, true);
            RotationY.Build(writer, true);
            RotationZ.Build(writer, true);
            writer.Write(_RotationDuration);
            _RotationLoop.Build(writer);

            ScaleX.Build(writer, false);
            ScaleY.Build(writer, false);
            ScaleZ.Build(writer, false);
            writer.Write(_ScaleDuration);
            _ScaleLoop.Build(writer);
            writer.Write(_ScaleEach);

            writer.Write(_Pivot);
            writer.Write(_Stencil);
            writer.Write(_Closing);
            writer.Write((byte)_RandomWeight);
            writer.Write((byte)_Target);
            writer.WriteString(_Target == AnimationTarget.Origin ? _Binding : null);
            writer.Write(_Duration);
            writer.Write(_Latency);

            writer.Write(_SoundSource != null);
            if (_SoundSource != null)
            {
                writer.WriteString(_SoundSource.BuildPath);
                writer.Write(_SoundVolume);
                writer.Write((byte)_SoundControl);
                writer.Write((byte)_SoundLoop);
                writer.Write((byte)_SoundPriority);
                writer.Write(_SoundPerspective);
                writer.Write(_SoundLatency);
                writer.Write(_SoundDuration);
                writer.Write(_SoundDuplication);
                writer.Write(_SoundStop);
            }

            writer.Write(_LocaleVisible);
            writer.WriteLength(_locales.Count);
            foreach (var locale in _locales) writer.WriteString(locale);

            writer.Write((byte)SubstanceType);
            if (_Substance != null) _Substance.Build(writer);

            if (Children.Count != 0)
            {
                writer.Write((byte)_Derivation.Type);
                writer.WriteLength(Children.Count);
                foreach (var child in Children) child.Build(writer);
                writer.Write(_DerivationFinish);
                _Derivation.Build(writer);
            }
            else writer.Write((byte)AnimationDerivationType.None);
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            _Substance?.GetLocaleStrings(strings);

            foreach (var animation in Children) animation.GetLocaleStrings(strings);        
        }
    }
}

