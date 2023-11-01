using System;
using System.Numerics;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

using CDK.Assets.Animations.Components;

namespace CDK.Assets.Texturing
{
    public enum MaterialUsage
    {
        Origin,
        Ground,
        Mesh,
        Image,
        TerrainSurface,
        TerrainWater
    }

    public class Material : AssetElement, ISkin
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public MaterialUsage Usage { private set; get; }

        private Drawing.Material _instance;

        private MaterialAsset _Origin;
        public MaterialAsset Origin
        {
            set
            {
                if (Usage == MaterialUsage.Origin || Usage == MaterialUsage.Image) throw new InvalidOperationException();

                var prev = _Origin;
                if (SetProperty(ref _Origin, value))
                {
                    prev?.RemoveWeakTextureRefresh(Origin_TextureRefresh);
                    _Origin?.AddWeakTextureRefresh(Origin_TextureRefresh);

                    if (AssetManager.Instance.CommandEnabled) Local = _Origin == null;

                    ApplyInstanceTexture();

                    OnPropertyChanged("Name");
                }
            }
            get => _Origin;
        }

        private bool _Local;
        public bool Local
        {
            set
            {
                if (Usage == MaterialUsage.Origin || (AssetManager.Instance.CommandEnabled && _Origin == null)) value = true;

                SetProperty(ref _Local, value);
            }
            get => _Local;
        }

        public bool HasTexture
        {
            get
            {
                if (Usage == MaterialUsage.Image) return true;

                if (_Origin != null)
                {
                    if (_Origin.HasColor || _Origin.HasNormal || _Origin.HasEmissive) return true;

                    if (Shader == MaterialShader.Light && _Origin.HasMaterial) return true;
                }

                return false;
            }
        }

        public void CopyFrom(Material other, bool texture)
        {
            Shader = other.Shader;
            if (texture && Usage != MaterialUsage.Origin && Usage != MaterialUsage.Image) Origin = other._Origin;
            BlendMode = other.BlendMode;
            BlendLayer = other.BlendLayer;
            CullMode = other.CullMode;
            DepthTest = other.DepthTest;
            AlphaTest = other.AlphaTest;
            AlphaTestBias = other.AlphaTestBias;
            DisplacementScale = other.DisplacementScale;
            DistortionScale = other.DistortionScale;
            Color.CopyFrom(other.Color);
            ColorDuration = other._ColorDuration;
            ColorLoop = other._ColorLoop;
            Bloom = other.Bloom;
            Reflection = other.Reflection;
            Metallic = other.Metallic;
            Roughness = other.Roughness;
            AmbientOcclusion = other.AmbientOcclusion;
            Rim = other.Rim;
            Emission.CopyFrom(other.Emission);
            EmissionDuration = other._EmissionDuration;
            EmissionLoop = other._EmissionLoop;
            UVScroll = other._UVScroll;
            UVScrollAngle = other._UVScrollAngle;
        }

        private void ApplyInstanceTexture()
        {
            if (_Origin != null)
            {
                _instance.ColorMap = _Origin.ColorReference?.ColorMap.Texture ?? _Origin.ColorMap.Texture;
                _instance.NormalMap = _Origin.NormalReference?.NormalMap.Texture ?? _Origin.NormalMap.Texture;
                _instance.MaterialMap = _Origin.MaterialReference?.MaterialMap.Texture ?? _Origin.MaterialMap.Texture;
                _instance.MaterialMapComponents = _Origin.MaterialComponent;
                _instance.EmissiveMap = _Origin.EmissiveReference?.EmissiveMap.Texture ?? _Origin.EmissiveMap.Texture;
            }
            else
            {
                _instance.ColorMap = null;
                _instance.NormalMap = null;
                _instance.MaterialMap = null;
                _instance.MaterialMapComponents = 0;
                _instance.EmissiveMap = null;
            }
        }

        public void LoadTexture()
        {
            if (Usage != MaterialUsage.Origin && Usage != MaterialUsage.Image && _Origin != null) CopyFrom(_Origin.Material, false);
        }

        public void SaveTexture()
        {
            if (Usage != MaterialUsage.Origin && Usage != MaterialUsage.Image && _Origin != null) _Origin.Material.CopyFrom(this, false);
        }

        public string Name { internal set; get; }

        public MaterialShader Shader
        {
            set => SetProperty(ref _instance.Shader, value);
            get => _instance.Shader;
        }

        public InstanceBlendLayer _BlendLayer;
        public InstanceBlendLayer BlendLayer
        {
            set => SetProperty(ref _BlendLayer, value);
            get => _BlendLayer;
        }

        public BlendMode BlendMode
        {
            set => SetProperty(ref _instance.BlendMode, value);
            get => _instance.BlendMode;
        }

        public CullMode CullMode
        {
            set => SetProperty(ref _instance.CullMode, value);
            get => _instance.CullMode;
        }

        public bool DepthTest
        {
            set => SetProperty(ref _instance.DepthTest, value);
            get => _instance.DepthTest;
        }

        public float DepthBias
        {
            set => SetProperty(ref _instance.DepthBias, value);
            get => _instance.DepthBias;
        }

        public bool AlphaTest
        {
            set => SetProperty(ref _instance.AlphaTest, value);
            get => _instance.AlphaTest;
        }

        public float AlphaTestBias
        {
            set => SetProperty(ref _instance.AlphaTestBias, value);
            get => _instance.AlphaTestBias;
        }

        public float DisplacementScale
        {
            set => SetProperty(ref _instance.DisplacementScale, value);
            get => _instance.DisplacementScale;
        }

        public float DistortionScale
        {
            set => SetProperty(ref _instance.DistortionScale, value);
            get => _instance.DistortionScale;
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

        public bool Bloom
        {
            set => SetProperty(ref _instance.Bloom, value);
            get => _instance.Bloom;
        }

        public bool Reflection
        {
            set => SetProperty(ref _instance.Reflection, value);
            get => _instance.Reflection;
        }

        public bool ReceiveShadow
        {
            set => SetProperty(ref _instance.ReceiveShadow, value);
            get => _instance.ReceiveShadow;
        }

        public bool ReceiveShadow2D
        {
            set => SetProperty(ref _instance.ReceiveShadow2D, value);
            get => _instance.ReceiveShadow2D;
        }

        public float Metallic
        {
            set => SetProperty(ref _instance.Metallic, value);
            get => _instance.Metallic;
        }

        public float Roughness
        {
            set => SetProperty(ref _instance.Roughness, value);
            get => _instance.Roughness;
        }

        public float AmbientOcclusion
        {
            set => SetProperty(ref _instance.AmbientOcclusion, value);
            get => _instance.AmbientOcclusion;
        }

        public float Rim
        {
            set => SetProperty(ref _instance.Rim, value);
            get => _instance.Rim;
        }

        public AnimationColor Emission { private set; get; }

        private float _EmissionDuration;
        public float EmissionDuration
        {
            set => SetProperty(ref _EmissionDuration, value);
            get => _EmissionDuration;
        }

        private AnimationLoop _EmissionLoop;
        public AnimationLoop EmissionLoop
        {
            set => SetProperty(ref _EmissionLoop, value);
            get => _EmissionLoop;
        }

        public float _UVScroll;
        public float UVScroll
        {
            set
            {
                if (SetProperty(ref _UVScroll, value)) ApplyInstanceUVOffset();
            }
            get => _UVScroll;
        }

        public float _UVScrollAngle;
        public float UVScrollAngle
        {
            set
            {
                if (SetProperty(ref _UVScrollAngle, value)) ApplyInstanceUVOffset();
            }
            get => _UVScrollAngle;
        }

        private Vector2 _uvOffset;

        private void ApplyInstanceUVOffset()
        {
            var a = _UVScrollAngle * MathUtil.ToRadians;

            _uvOffset = new Vector2(
                _UVScroll * (float)Math.Cos(a),
                _UVScroll * (float)Math.Sin(a));
        }

        public bool HasOpacity => Color.HasOpacity || (_Origin != null && _Origin.HasOpacity);

        private void Origin_TextureRefresh(object sender, EventArgs e)
        {
            ApplyInstanceTexture();

            OnPropertyChanged("Origin");
        }

        internal Scenes.ShowFlags ShowFlags => Shader == MaterialShader.Distortion ? Scenes.ShowFlags.Distortion : Scenes.ShowFlags.None;

        public Material(AssetElement parent, MaterialUsage usage)
        {
            Parent = parent;

            Name = "Material";

            Usage = usage;

            _instance = Drawing.Material.Default;

            if (usage == MaterialUsage.Origin)
            {
                _Origin = (MaterialAsset)parent;
                ApplyInstanceTexture();
                _Origin.AddWeakTextureRefresh(Origin_TextureRefresh);
            }
            else if (usage == MaterialUsage.Image)
            {
                _instance.Shader = MaterialShader.NoLight;
                _instance.BlendMode = BlendMode.Alpha;
            }

            _Local = true;

            _BlendLayer = InstanceBlendLayer.Middle;
            Color = new AnimationColor(this, true, true, Color4.White);
            Emission = new AnimationColor(this, false, false, Color4.Black);
        }

        public Material(AssetElement parent, Material other, MaterialUsage usage)
        {
            Parent = parent;

            Name = "Material";

            Usage = usage;

            _instance = other._instance;

            if (usage == MaterialUsage.Origin)
            {
                _Origin = (MaterialAsset)parent;
                _Origin.AddWeakTextureRefresh(Origin_TextureRefresh);
                ApplyInstanceTexture();
            }
            else if (usage != MaterialUsage.Image)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    _Origin = AssetManager.Instance.GetRedirection(other._Origin);
                    _Origin?.AddWeakTextureRefresh(Origin_TextureRefresh);
                    ApplyInstanceTexture();
                });
            }

            _Local = other._Local;
            _BlendLayer = other._BlendLayer;
            Color = new AnimationColor(this, other.Color);
            _ColorDuration = other._ColorDuration;
            _ColorLoop = other._ColorLoop;
            Emission = new AnimationColor(this, other.Emission);
            _EmissionDuration = other._EmissionDuration;
            _EmissionLoop = other._EmissionLoop;
            _UVScroll = other._UVScroll;
            _UVScrollAngle = other._UVScrollAngle;

            ApplyInstanceUVOffset();
        }

        public bool ReceiveLight => _Local ? _instance.ReceiveLight : _Origin.Material.ReceiveLight;
        public bool UsingColor => _Local ? _instance.UsingColor : _Origin.Material.UsingColor;

        public Drawing.Material GetMaterial(float progress, int random)
        {
            if (!_Local) return _Origin?.Material.GetMaterial(progress, random) ?? Drawing.Material.Default;

            if (_ColorDuration != 0)
            {
                var cp = _ColorLoop.GetProgress(progress / _ColorDuration, out var randomSeq0, out var randomSeq1);
                randomSeq0 *= 7;
                randomSeq1 *= 7;
                var cr = new Color4(
                    RandomUtil.ToFloatSequenced(random, randomSeq0, randomSeq1, cp),
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 1, randomSeq1 + 1, cp),
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 2, randomSeq1 + 2, cp),
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 3, randomSeq1 + 3, cp));

                _instance.Color = Color.GetColor(cp, cr);
            }
            else
            {
                var cr = new Color4(
                    RandomUtil.ToFloatSequenced(random, 0),
                    RandomUtil.ToFloatSequenced(random, 1),
                    RandomUtil.ToFloatSequenced(random, 2),
                    RandomUtil.ToFloatSequenced(random, 3));

                _instance.Color = Color.GetColor(1, cr);
            }
            if (_EmissionDuration != 0)
            {
                var ep = _ColorLoop.GetProgress(progress / _EmissionDuration, out var randomSeq0, out var randomSeq1);
                randomSeq0 *= 7;
                randomSeq1 *= 7;
                var er = new Color4(
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 4, randomSeq1 + 4, ep),
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 5, randomSeq1 + 5, ep),
                    RandomUtil.ToFloatSequenced(random, randomSeq0 + 6, randomSeq1 + 6, ep),
                    1);
                _instance.Emission = (Color3)Emission.GetColor(ep, er);
            }
            else
            {
                var er = new Color4(
                    RandomUtil.ToFloatSequenced(random, 4),
                    RandomUtil.ToFloatSequenced(random, 5),
                    RandomUtil.ToFloatSequenced(random, 6),
                    1);
                _instance.Emission = (Color3)Emission.GetColor(1, er);
            }

            _instance.UVOffset = _uvOffset * progress;

            return _instance;
        }

        internal bool Apply(Graphics graphics, float progress, int random, InstanceLayer layer, bool push)
        {
            var instance = _Local ? _instance : _Origin.Material._instance;

            switch (layer)
            {
                case InstanceLayer.None:
                    if (instance.Shader == MaterialShader.Distortion) break;
                    if (push) graphics.Push();
                    graphics.Material = GetMaterial(progress, random);
                    graphics.Material.ReceiveShadow = false;
                    graphics.Material.ReceiveShadow2D = false;
                    return true;
                case InstanceLayer.Shadow:
                    if (!instance.ReceiveLight) break;
                    if (push) graphics.Push();
                    return true;
                case InstanceLayer.Base:
                    {
                        if (instance.Shader == MaterialShader.Distortion) break;
                        var blending = instance.BlendMode != BlendMode.None || graphics.Color.A < 1;
                        if (blending) break;
                        if (push) graphics.Push();
                        graphics.Material = GetMaterial(progress, random);
                    }
                    return true;
                case InstanceLayer.BlendBottom:
                case InstanceLayer.BlendMiddle:
                case InstanceLayer.BlendTop:
                    {
                        if ((InstanceLayer)_BlendLayer != layer || instance.Shader == MaterialShader.Distortion) break;
                        var blending = instance.BlendMode != BlendMode.None || graphics.Color.A < 1;
                        if (!blending) break;
                        if (push) graphics.Push();
                        graphics.Material = GetMaterial(progress, random);
                        if (graphics.Material.BlendMode == BlendMode.None) graphics.Material.BlendMode = BlendMode.Alpha;
                    }
                    return true;
                case InstanceLayer.Distortion:
                    if (instance.Shader != MaterialShader.Distortion) break;
                    if (push) graphics.Push();
                    graphics.Material = GetMaterial(progress, random);
                    return true;
                case InstanceLayer.Cursor:
                    if (push) graphics.Push();
                    graphics.PolygonMode = PolygonMode.Line;
                    graphics.Material.Shader = MaterialShader.NoLight;
                    graphics.Material.BlendMode = BlendMode.Alpha;
                    graphics.Material.Color = Color4.FaintWhite;
                    return true;
            }

            return false;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (Usage != MaterialUsage.Origin && _Origin != null) retains.Add(_Origin.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Usage != MaterialUsage.Origin && _Origin == element)
            {
                from = this;
                return true;
            }
            else
            {
                from = null;
                return false;
            }
        }
        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("material");
            if (Usage != MaterialUsage.Origin && Usage != MaterialUsage.Image) writer.WriteAttribute("origin", _Origin);
            writer.WriteAttribute("local", _Local);
            writer.WriteAttribute("shader", _instance.Shader, MaterialShader.Light);
            writer.WriteAttribute("blendLayer", _BlendLayer, InstanceBlendLayer.Middle);
            writer.WriteAttribute("blendMode", _instance.BlendMode, BlendMode.None);
            writer.WriteAttribute("cullMode", _instance.CullMode, CullMode.Back);
            writer.WriteAttribute("depthTest", _instance.DepthTest, true);
            writer.WriteAttribute("depthBias", _instance.DepthBias);
            writer.WriteAttribute("alphTest", _instance.AlphaTest);
            writer.WriteAttribute("alphTestBias", _instance.AlphaTestBias);
            writer.WriteAttribute("displacementScale", _instance.DisplacementScale);
            writer.WriteAttribute("distortionScale", _instance.DistortionScale);
            Color.Save(writer, "color");
            writer.WriteAttribute("colorDuration", _ColorDuration);
            _ColorLoop.Save(writer, "colorLoop");
            writer.WriteAttribute("bloom", _instance.Bloom, true);
            writer.WriteAttribute("reflection", _instance.Reflection, true);
            writer.WriteAttribute("receiveShadow", _instance.ReceiveShadow, true);
            writer.WriteAttribute("receiveShadow2D", _instance.ReceiveShadow2D);
            writer.WriteAttribute("metallic", _instance.Metallic, 0.5f);
            writer.WriteAttribute("roughness", _instance.Roughness, 0.5f);
            writer.WriteAttribute("ambientOcclusion", _instance.AmbientOcclusion, 1);
            writer.WriteAttribute("rim", _instance.Rim);
            Emission.Save(writer, "emission");
            writer.WriteAttribute("emissionDuration", _EmissionDuration);
            _EmissionLoop.Save(writer, "emissionLoop");
            writer.WriteAttribute("uvScroll", _UVScroll);
            writer.WriteAttribute("uvScrollAngle", _UVScrollAngle);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (Usage != MaterialUsage.Origin && Usage != MaterialUsage.Image) Origin = (MaterialAsset)node.ReadAttributeAsset("origin");
            Local = node.ReadAttributeBool("local");
            Shader = node.ReadAttributeEnum("shader", MaterialShader.Light);
            BlendLayer = node.ReadAttributeEnum("blendLayer", InstanceBlendLayer.Middle);
            BlendMode = node.ReadAttributeEnum("blendMode", BlendMode.None);
            CullMode = node.ReadAttributeEnum("cullMode", CullMode.Back);
            DepthTest = node.ReadAttributeBool("depthTest", true);
            DepthBias = node.ReadAttributeFloat("depthBias");
            AlphaTest = node.ReadAttributeBool("alphaTest");
            AlphaTestBias = node.ReadAttributeFloat("alphaTestBias");
            DisplacementScale = node.ReadAttributeFloat("displacementScale");
            DistortionScale = node.ReadAttributeFloat("distortionScale");
            Color.Load(node, "color");
            ColorDuration = node.ReadAttributeFloat("colorDuration");
            ColorLoop = new AnimationLoop(node, "colorLoop");
            Bloom = node.ReadAttributeBool("bloom", true);
            Reflection = node.ReadAttributeBool("reflection", true);
            ReceiveShadow = node.ReadAttributeBool("receiveShadow", true);
            ReceiveShadow2D = node.ReadAttributeBool("receiveShadow2D");
            Metallic = node.ReadAttributeFloat("metallic", 0.5f);
            Roughness = node.ReadAttributeFloat("roughness", 0.5f);
            AmbientOcclusion = node.ReadAttributeFloat("ambientOcclusion", 1);
            Rim = node.ReadAttributeFloat("rim");
            Emission.Load(node, "emission");
            EmissionDuration = node.ReadAttributeFloat("emissionDuration");
            EmissionLoop = new AnimationLoop(node, "emissionLoop");
            UVScroll = node.ReadAttributeFloat("uvScroll");
            UVScrollAngle = node.ReadAttributeFloat("uvScrollAngle");
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(_Local);

            if (_Local)
            {
                writer.Write((byte)_instance.Shader);
                writer.Write((byte)_BlendLayer);
                writer.Write((byte)_instance.BlendMode);
                writer.Write((byte)_instance.CullMode);
                writer.Write(_instance.DepthTest);
                writer.Write(_instance.DepthBias);
                writer.Write(_instance.AlphaTest);
                writer.Write(_instance.AlphaTestBias);
                writer.Write(_instance.DisplacementScale);
                writer.Write(_instance.DistortionScale);
                Color.Build(writer);
                if (Color.IsAnimating)
                {
                    writer.Write(_ColorDuration);
                    _ColorLoop.Build(writer);
                }
                writer.Write(_instance.Bloom);
                writer.Write(_instance.Reflection);
                writer.Write(_instance.ReceiveShadow);
                writer.Write(_instance.ReceiveShadow2D);
                writer.Write(_instance.Metallic);
                writer.Write(_instance.Roughness);
                writer.Write(_instance.AmbientOcclusion);
                writer.Write(_instance.Rim);

                Emission.Build(writer);
                if (Emission.IsAnimating)
                {
                    writer.Write(_EmissionDuration);
                    _EmissionLoop.Build(writer);
                }
                writer.Write(_UVScroll);
                writer.Write(_UVScrollAngle * MathUtil.ToRadians);
            }
            else BuildReference(writer, _Origin);
        }
    }
}
