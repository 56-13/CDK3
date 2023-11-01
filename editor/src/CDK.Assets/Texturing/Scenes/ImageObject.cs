using System;
using System.Collections.Generic;
using System.Numerics;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations;

namespace CDK.Assets.Texturing
{
    public class ImageObject : SceneObject, IAnimationSubstance
    {
        private ImageAsset _Asset;
        [Binding]
        public ImageAsset Asset
        {
            set
            {
                var prev = _Asset;
                if (SetProperty(ref _Asset, value))
                {
                    if (prev != null)
                    {
                        prev.RemoveWeakPropertyChanged(Asset_PropertyChanged);
                        prev.RemoveWeakRefresh(Asset_Refresh);
                    }
                    if (_Asset != null)
                    {
                        _Asset.Load();
                        _Asset.AddWeakPropertyChanged(Asset_PropertyChanged);
                        _Asset.AddWeakRefresh(Asset_Refresh);
                    }
                    OnPropertyChanged("Name");

                    AddUpdateFlags(UpdateFlags.AABB);
                }
            }
            get => _Asset;
        }

        public override string Name
        {
            set => base.Name = value;
            get => _Name ?? _Asset?.TagName ?? Type.ToString();
        }

        private InstanceBlendLayer _Layer;
        [Binding]
        public InstanceBlendLayer Layer
        {
            set => SetProperty(ref _Layer, value);
            get => _Layer;
        }

        private BlendMode _BlendMode;
        [Binding]
        public BlendMode BlendMode
        {
            set => SetProperty(ref _BlendMode, value);
            get => _BlendMode;
        }

        private bool _Billboard;
        [Binding]
        public bool Billboard
        {
            set
            {
                if (SetProperty(ref _Billboard, value)) AddUpdateFlags(UpdateFlags.Transform | UpdateFlags.AABB);
            }
            get => _Billboard;
        }

        private float _DepthBias;
        [Binding]
        public float DepthBias
        {
            set => SetProperty(ref _DepthBias, value);
            get => _DepthBias;
        }

        public ImageObject(ImageAsset asset = null)
        {
            if (asset != null) using (new AssetCommandHolder()) Asset = asset;

            _Layer = InstanceBlendLayer.Middle;
            _BlendMode = BlendMode.Alpha;
            _Billboard = true;
        }

        public ImageObject(ImageObject other, bool binding) : base(other, binding, true)
        {
            if (other._Asset != null)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    using (new AssetCommandHolder()) Asset = AssetManager.Instance.GetRedirection(other._Asset);
                });
            }

            _Layer = other._Layer;
            _BlendMode = other._BlendMode;
            _Billboard = other._Billboard;
            _DepthBias = other._DepthBias;
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TagName":
                    OnPropertyChanged("Name");
                    break;
                case "Frame":
                case "ContentScale":
                    AddUpdateFlags(UpdateFlags.AABB);
                    break;
            }
        }

        private void Asset_Refresh(object sender, EventArgs e) => OnRefresh();

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Asset != null) retains.Add(_Asset.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Asset == element)
            {
                from = this;
                return true;
            }
            return Transform.IsRetaining(element, out from);
        }

        public override SceneComponentType Type => SceneComponentType.Image;
        public override SceneComponent Clone(bool binding) => new ImageObject(this, binding);
        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (base.GetTransform(progress, name, out result))
            {
                if (_Billboard && Scene != null) result = Scene.Camera.View.Billboard(result);
                return true;
            }
            return false;
        }

        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (_Asset != null && GetTransform(out var transform))
            {
                var frame = _Asset.GetDisplayFrame(Vector3.Zero, Align.CenterMiddle);

                result.Append(Vector3.Transform(frame.LeftTop, transform));
                result.Append(Vector3.Transform(frame.RightTop, transform));
                result.Append(Vector3.Transform(frame.LeftBottom, transform));
                result.Append(Vector3.Transform(frame.RightBottom, transform));
                return true;
            }
            return false;
        }
        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            if ((flags & (_Billboard ? UpdateFlags.View | UpdateFlags.Transform : UpdateFlags.Transform)) != 0) flags |= UpdateFlags.AABB;
            return UpdateState.None;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == (InstanceLayer)_Layer && _Asset != null && _Asset.GetDisplay(Vector3.Zero, Align.CenterMiddle, out var texture, out var uv, out var frame) && GetTransform(out var transform))
            {
                graphics.Push();
                graphics.Transform(transform);
                graphics.Material.Shader = MaterialShader.NoLight;
                graphics.Material.BlendMode = _BlendMode;
                graphics.Material.DepthBias = _DepthBias;
                graphics.DrawImage(texture, uv, frame);
                graphics.Pop();
            }
        }

        internal override bool AfterCameraUpdate() => _Billboard;

        protected override void SaveContent(XmlWriter writer) 
        {
            writer.WriteAttribute("asset", _Asset);
            writer.WriteAttribute("layer", _Layer, InstanceBlendLayer.Middle);
            writer.WriteAttribute("blendMode", _BlendMode, BlendMode.Alpha);
            writer.WriteAttribute("billboard", _Billboard, true);
            writer.WriteAttribute("depthBias", _DepthBias);
        }

        protected override void LoadContent(XmlNode node)
        {
            Asset = (ImageAsset)node.ReadAttributeAsset("asset");
            Layer = node.ReadAttributeEnum("layer", InstanceBlendLayer.Middle);
            BlendMode = node.ReadAttributeEnum("blendMode", BlendMode.Alpha);
            Billboard = node.ReadAttributeBool("billboard", true);
            DepthBias = node.ReadAttributeFloat("depthBias");
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param) 
        {
            BuildReference(writer, _Asset);
            writer.Write((byte)_Layer);
            writer.Write((byte)_BlendMode);
            writer.Write(_Billboard);
            writer.Write(_DepthBias);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new ImageObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new ImageObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => BuildContent(writer, null);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
