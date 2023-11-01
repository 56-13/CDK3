using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Texturing;
using CDK.Assets.Support;

namespace CDK.Assets.Configs
{
    public class EnvironmentConfig : AssetElement
    {
        public SceneConfig Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public int Index => Parent.Environments.IndexOf(this);

        public string Key { private set; get; }

        private string _Name;
        public string Name
        {
            set => SetProperty(ref _Name, GetNewName(value));
            get => _Name;
        }

        private bool _UsingLight;
        public bool UsingLight
        {
            set => SetProperty(ref _UsingLight, value);
            get => _UsingLight;
        }

        private Color3 _AmbientLight;
        public Color3 AmbientLight
        {
            set => SetProperty(ref _AmbientLight, value);
            get => _AmbientLight;
        }

        public AssetElementList<SceneComponent> Props { private set; get; }

        private bool _UsingShadow;
        public bool UsingShadow
        {
            set => SetProperty(ref _UsingShadow, value);
            get => _UsingShadow;
        }

        private bool _UsingShadowPixel32;
        public bool UsingShadowPixel32
        {
            set => SetProperty(ref _UsingShadowPixel32, value);
            get => _UsingShadowPixel32;
        }

        private int _MaxShadowResolution;
        public int MaxShadowResolution
        {
            set => SetProperty(ref _MaxShadowResolution, value);
            get => _MaxShadowResolution;
        }

        private bool _UsingFog;
        public bool UsingFog
        {
            set => SetProperty(ref _UsingFog, value);
            get => _UsingFog;
        }

        private Color3 _FogColor;
        public Color3 FogColor
        {
            set => SetProperty(ref _FogColor, value);
            get => _FogColor;
        }

        private float _FogNear;
        public float FogNear
        {
            set
            {
                if (value > _FogFar - 1) value = _FogFar - 1;
                SetProperty(ref _FogNear, value);
            }
            get => _FogNear;
        }

        private float _FogFar;
        public float FogFar
        {
            set
            {
                if (value < _FogNear + 1) value = _FogNear + 1;
                SetProperty(ref _FogFar, value);
            }
            get => _FogFar;
        }

        private SkyboxAsset _Skybox;
        public SkyboxAsset Skybox
        {
            set
            {
                var prev = _Skybox;
                if (SetProperty(ref _Skybox, value))
                {
                    prev?.RemoveWeakPropertyChanged(Skybox_PropertyChanged);
                    _Skybox?.AddWeakPropertyChanged(Skybox_PropertyChanged);
                    OnPropertyChanged("SkyboxMap");
                    OnPropertyChanged("SkyboxColor");
                }
            }
            get => _Skybox;
        }

        private void Skybox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Texture")
            {
                OnPropertyChanged("SkyboxMap");
                OnPropertyChanged("SkyboxColor");
            }
        }

        public Texture SkyboxMap => _Skybox != null ? _Skybox.Content.Texture : AssetManager.Instance.Config.Resource.SkyboxMap;
        public Color3 SkyboxColor => _Skybox != null ? (Color3)_Skybox.Content.AverageColor : AssetManager.Instance.Config.Resource.SkyboxColor;

        private bool _UsingPostProcess;
        public bool UsingPostProcess
        {
            set => SetProperty(ref _UsingPostProcess, value);
            get => _UsingPostProcess;
        }

        private int _BloomPass;
        public int BloomPass
        {
            set => SetProperty(ref _BloomPass, value);
            get => _BloomPass;
        }

        private float _BloomIntensity;
        public float BloomIntensity
        {
            set => SetProperty(ref _BloomIntensity, value);
            get => _BloomIntensity;
        }

        private float _BloomThreshold;
        public float BloomThreshold
        {
            set => SetProperty(ref _BloomThreshold, value);
            get => _BloomThreshold;
        }

        private float _Exposure;
        public float Exposure
        {
            set => SetProperty(ref _Exposure, value);
            get => _Exposure;
        }

        private float _Gamma;
        public float Gamma
        {
            set => SetProperty(ref _Gamma, value);
            get => _Gamma;
        }

        private float _SoundMaxDistance;
        public float SoundMaxDistance
        {
            set => SetProperty(ref _SoundMaxDistance, value);
            get => _SoundMaxDistance;
        }

        private float _SoundRefDistance;
        public float SoundRefDistance
        {
            set => SetProperty(ref _SoundRefDistance, value);
            get => _SoundRefDistance;
        }

        private float _SoundRollOffFactor;
        public float SoundRollOffFactor
        {
            set => SetProperty(ref _SoundRollOffFactor, value);
            get => _SoundRollOffFactor;
        }

        private float _SoundDuplication;
        public float SoundDuplication
        {
            set => SetProperty(ref _SoundDuplication, value);
            get => _SoundDuplication;
        }

        private int _SoundBgmCapacity;
        public int SoundBgmCapacity
        {
            set => SetProperty(ref _SoundBgmCapacity, value);
            get => _SoundBgmCapacity;
        }

        private int _SoundEffectCapacity;
        public int SoundEffectCapacity
        {
            set => SetProperty(ref _SoundEffectCapacity, value);
            get => _SoundEffectCapacity;
        }

        private int _SoundVoiceCapacity;
        public int SoundVoiceCapacity
        {
            set => SetProperty(ref _SoundVoiceCapacity, value);
            get => _SoundVoiceCapacity;
        }

        public int SoundCapacity(AudioControl control)
        {
            switch(control)
            {
                case AudioControl.Bgm:
                    return _SoundBgmCapacity;
                case AudioControl.Effect:
                    return _SoundEffectCapacity;
                case AudioControl.Voice:
                    return _SoundVoiceCapacity;
            }
            return 0;
        }

        private float _CameraFov;
        public float CameraFov
        {
            set => SetProperty(ref _CameraFov, value);
            get => _CameraFov;
        }

        private float _CameraNear;
        public float CameraNear
        {
            set
            {
                if (value > _CameraFar - 1) value = _CameraFar - 1;
                SetProperty(ref _CameraNear, value);
            }
            get => _CameraNear;
        }

        private float _CameraFar;
        public float CameraFar
        {
            set
            {
                if (value < _CameraNear + 1) value = _CameraNear + 1;
                SetProperty(ref _CameraFar, value);
            }
            get => _CameraFar;
        }

        private float _CameraAngle;
        public float CameraAngle
        {
            set => SetProperty(ref _CameraAngle, value);
            get => _CameraAngle;
        }

        private float _CameraDistance;
        public float CameraDistance
        {
            set => SetProperty(ref _CameraDistance, value);
            get => _CameraDistance;
        }

        private bool _UsingQuadTree;
        public bool UsingQuadTree
        {
            set => SetProperty(ref _UsingQuadTree, value);
            get => _UsingQuadTree;
        }

        public static readonly Color3 DefaultAmbientLight = new Color3(0x4C4C4CFF);
        public const float DefaultFogNear = 100;
        public const float DefaultFogFar = 10000;
        public const int DefaultBloomPass = 4;
        public const float DefaultBloomIntensity = 1;
        public const float DefaultBloomThreshold = 1;
        public const float DefaultExposure = 2;
        public const float DefaultGamma = 1.6f;
        public const float DefaultSoundMaxDistance = 16;
        public const float DefaultSoundRefDistance = 2;
        public const float DefaultSoundRollOffFactor = 0.75f;
        public const float DefaultSoundDuplication = 0.2f;
        public const int DefaultSoundBgmCapacity = 0;
        public const int DefaultSoundEffectCapacity = 8;
        public const int DefaultSoundVoiceCapacity = 1;
        public const float DefaultCameraFov = 60;
        public const float DefaultCameraNear = 10;
        public const float DefaultCameraFar = 10000;
        public const float DefaultCameraAngle = 45;
        public const float DefaultCameraDistance = 1000;

        public EnvironmentConfig(SceneConfig parent, XmlNode node)
        {
            Parent = parent;

            if (node.LocalName != "environment") throw new XmlException();

            _Name = node.ReadAttributeString("name");

            Key = node.ReadAttributeString("key", _Name);

            Props = new AssetElementList<SceneComponent>(this);
            Props.BeforeListChanged += Props_BeforeListChanged;
            Props.ListChanged += Props_ListChanged;

            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName)
                {
                    case "light":
                        _UsingLight = node.ReadAttributeBool("enabled", true);
                        _AmbientLight = node.ReadAttributeColor3("ambient", true, DefaultAmbientLight);
                        _UsingShadow = node.ReadAttributeBool("Shadow", true);
                        _UsingShadowPixel32 = node.ReadAttributeBool("ShadowPixel32", true);
                        _MaxShadowResolution = node.ReadAttributeInt("maxShadowResolution", 2048);
                        _UsingFog = subnode.ReadAttributeBool("fog");
                        _FogColor = subnode.ReadAttributeColor3("fogColor", true);
                        _FogNear = subnode.ReadAttributeFloat("fogNear", DefaultFogNear);
                        _FogFar = subnode.ReadAttributeFloat("fogFar", DefaultFogFar);
                        _Skybox = (SkyboxAsset)subnode.ReadAttributeAsset("skybox");
                        _Skybox?.AddWeakPropertyChanged(Skybox_PropertyChanged);
                        using (new AssetCommandHolder())
                        {
                            foreach (XmlNode objnode in subnode.ChildNodes)
                            {
                                var obj = SceneComponent.Create(Parent, objnode);
                                Props.Add(obj);
                                obj.Load(objnode);
                            }
                        }
                        break;
                    case "postprocess":
                        _UsingPostProcess = subnode.ReadAttributeBool("enabled", true);
                        _BloomPass = subnode.ReadAttributeInt("bloomPass", DefaultBloomPass);
                        _BloomIntensity = subnode.ReadAttributeFloat("bloomIntensity", DefaultBloomIntensity);
                        _BloomThreshold = subnode.ReadAttributeFloat("bloomThreshold", DefaultBloomThreshold);
                        _Exposure = subnode.ReadAttributeFloat("exposure", DefaultExposure);
                        _Gamma = subnode.ReadAttributeFloat("gamma", DefaultGamma);
                        break;
                    case "sound":
                        _SoundMaxDistance = subnode.ReadAttributeFloat("maxDistance", DefaultSoundMaxDistance);
                        _SoundRefDistance = subnode.ReadAttributeFloat("refDistance", DefaultSoundRefDistance);
                        _SoundRollOffFactor = subnode.ReadAttributeFloat("rollOffFactor", DefaultSoundRollOffFactor);
                        _SoundDuplication = subnode.ReadAttributeFloat("duplication", DefaultSoundDuplication);
                        _SoundBgmCapacity = subnode.ReadAttributeInt("bgmCapacity", DefaultSoundBgmCapacity);
                        _SoundEffectCapacity = subnode.ReadAttributeInt("effectCapacity", DefaultSoundEffectCapacity);
                        _SoundVoiceCapacity = subnode.ReadAttributeInt("voiceCapacity", DefaultSoundVoiceCapacity);
                        break;
                    case "camera":
                        _CameraFov = subnode.ReadAttributeFloat("fov", DefaultCameraFov);
                        _CameraNear = subnode.ReadAttributeFloat("near", DefaultCameraNear);
                        _CameraFar = subnode.ReadAttributeFloat("far", DefaultCameraFar);
                        _CameraAngle = subnode.ReadAttributeFloat("angle", DefaultCameraAngle);
                        _CameraDistance = subnode.ReadAttributeFloat("distance", DefaultCameraDistance);
                        break;
                }
            }
        }

        public EnvironmentConfig(SceneConfig parent, EnvironmentConfig other)
        {
            Parent = parent;

            Key = Parent.Environments.Any(e => e.Key == other.Key) ? AssetManager.NewKey() : other.Key;
            _Name = GetNewName(other._Name);
            _AmbientLight = other._AmbientLight;
            _UsingFog = other._UsingFog;
            _FogColor = other._FogColor;
            _FogNear = other._FogNear;
            _FogFar = other._FogFar;

            if (other._Skybox != null)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    _Skybox = other._Skybox;
                    _Skybox?.AddWeakPropertyChanged(Skybox_PropertyChanged);
                });
            }

            Props = new AssetElementList<SceneComponent>(this);
            Props.BeforeListChanged += Props_BeforeListChanged;
            Props.ListChanged += Props_ListChanged;

            using (new AssetCommandHolder())
            {
                foreach (var prop in other.Props) Props.Add((SceneObject)prop.Clone(false));
            }

            _UsingPostProcess = other._UsingPostProcess;
            _BloomPass = other._BloomPass;
            _BloomIntensity = other._BloomIntensity;
            _BloomThreshold = other._BloomThreshold;
            _Exposure = other._Exposure;
            _Gamma = other._Gamma;

            _SoundMaxDistance = other._SoundMaxDistance;
            _SoundRefDistance = other._SoundRefDistance;
            _SoundRollOffFactor = other._SoundRollOffFactor;
            _SoundDuplication = other._SoundDuplication;
            _SoundBgmCapacity = other._SoundBgmCapacity;
            _SoundEffectCapacity = other._SoundEffectCapacity;
            _SoundVoiceCapacity = other._SoundVoiceCapacity;

            _CameraFov = other._CameraFov;
            _CameraNear = other._CameraNear;
            _CameraFar = other._CameraFar;
        }

        private string GetNewName(string name)
        {
            var rname = name;
            var i = rname.LastIndexOf(' ');
            if (i > 0 && int.TryParse(rname.Substring(i + 1), out _)) rname = rname.Substring(0, i);
            i = 1;
            while (Parent.Environments.Any(c => c != this && c.Name == rname)) rname = $"{name} {i++}";
            name = rname;
            return name;
        }

        private void Props_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        var item = Props[e.NewIndex];
                        if (item.Parent != null) throw new InvalidOperationException();
                        item.Parent = this;
                    }
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null) goto case ListChangedType.ItemAdded;
                    break;
                case ListChangedType.Reset:
                    foreach (var item in Props)
                    {
                        if (item.Parent != null) throw new InvalidOperationException();
                        item.Parent = this;
                    }
                    break;
            }
        }

        private void Props_BeforeListChanged(object sender, BeforeListChangedEventArgs<SceneComponent> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (Props[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    Props[e.NewIndex].Parent = null;
                    break;
                case ListChangedType.Reset:
                    foreach (var item in Props)
                    {
                        if (item.Parent != this) throw new InvalidOperationException();
                        item.Parent = null;
                    }
                    break;
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Skybox != null) retains.Add(_Skybox.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var light in Props)
            {
                if (light.IsRetaining(element, out from)) return true;
            }

            if (_Skybox == element)
            {
                from = this;
                return true;
            }
            from = null;
            return false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("environment");

            writer.WriteAttribute("key", Key, _Name);
            writer.WriteAttribute("name", _Name);

            writer.WriteStartElement("light");
            writer.WriteAttribute("enabled", _UsingLight, true);
            writer.WriteAttribute("ambient", _AmbientLight, true, DefaultAmbientLight);
            writer.WriteAttribute("shadow", _UsingShadow, true);
            writer.WriteAttribute("shadowPixel32", _UsingShadowPixel32, true);
            writer.WriteAttribute("maxShadowResolution", _MaxShadowResolution, 2048);
            writer.WriteAttribute("fog", _UsingFog);
            writer.WriteAttribute("fogColor", _FogColor, true);
            writer.WriteAttribute("fogNear", _FogNear, DefaultFogNear);
            writer.WriteAttribute("fogFar", _FogFar, DefaultFogFar);
            writer.WriteAttribute("skybox", _Skybox);
            foreach (var light in Props) light.Save(writer);
            writer.WriteEndElement();
            
            writer.WriteStartElement("postprocess");
            writer.WriteAttribute("enabled", _UsingPostProcess, true);
            writer.WriteAttribute("bloomPass", _BloomPass, DefaultBloomPass);
            writer.WriteAttribute("bloomIntensity", _BloomIntensity, DefaultBloomIntensity);
            writer.WriteAttribute("bloomThreshold", _BloomThreshold, DefaultBloomThreshold);
            writer.WriteAttribute("exposure", _Exposure, DefaultExposure);
            writer.WriteAttribute("gamma", _Gamma, DefaultGamma);
            writer.WriteEndElement();

            writer.WriteStartElement("sound");
            writer.WriteAttribute("maxDistance", _SoundMaxDistance, DefaultSoundMaxDistance);
            writer.WriteAttribute("refDistance", _SoundRefDistance, DefaultSoundRefDistance);
            writer.WriteAttribute("rollOffFactor", _SoundRollOffFactor, DefaultSoundRollOffFactor);
            writer.WriteAttribute("duplication", _SoundDuplication, DefaultSoundDuplication);
            writer.WriteAttribute("bgmCapacity", _SoundBgmCapacity, DefaultSoundBgmCapacity);
            writer.WriteAttribute("effectCapacity", _SoundEffectCapacity, DefaultSoundEffectCapacity);
            writer.WriteAttribute("voiceCapacity", _SoundVoiceCapacity, DefaultSoundVoiceCapacity);
            writer.WriteEndElement();

            writer.WriteStartElement("camera");
            writer.WriteAttribute("fov", _CameraFov, DefaultCameraFov);
            writer.WriteAttribute("near", _CameraNear, DefaultCameraNear);
            writer.WriteAttribute("far", _CameraFar, DefaultCameraFar);
            writer.WriteAttribute("angle", _CameraAngle, DefaultCameraAngle);
            writer.WriteAttribute("distance", _CameraDistance, DefaultCameraDistance);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer, SceneBuildParam param)
        {
            writer.Write(_UsingLight);
            writer.Write(_AmbientLight, true);
            writer.Write(_UsingShadow);
            if (_UsingShadow)
            {
                writer.Write(_UsingShadowPixel32);
                writer.Write((ushort)_MaxShadowResolution);
            }
            writer.Write(_UsingFog);
            if (_UsingFog)
            {
                writer.Write(_FogColor, true);
                writer.Write(_FogNear);
                writer.Write(_FogFar);
            }
            writer.Write(_Skybox != null);
            if (_Skybox != null) BuildReference(writer, _Skybox);

            writer.WriteLength(Props.Count);
            foreach (SceneObject prop in Props) prop.Build(writer, param);

            writer.Write(_UsingPostProcess);
            if (_UsingPostProcess)
            {
                writer.Write((byte)_BloomPass);
                writer.Write(_BloomIntensity);
                writer.Write(_BloomThreshold);
                writer.Write(_Exposure);
                writer.Write(_Gamma);
            }

            writer.Write(_SoundMaxDistance);
            writer.Write(_SoundRefDistance);
            writer.Write(_SoundRollOffFactor);
            writer.Write(_SoundDuplication);
            writer.Write((byte)_SoundBgmCapacity);
            writer.Write((byte)_SoundEffectCapacity);
            writer.Write((byte)_SoundVoiceCapacity);

            writer.Write(_CameraFov * MathUtil.ToRadians);
            writer.Write(_CameraNear);
            writer.Write(_CameraFar);
            writer.Write(_CameraAngle * MathUtil.ToRadians);
            writer.Write(_CameraDistance);

            writer.Write(_UsingQuadTree);
        }
    }
}
