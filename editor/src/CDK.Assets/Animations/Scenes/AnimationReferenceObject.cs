using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    
    public class AnimationReferenceObject : AnimationObjectBase, IAnimationSubstance
    {
        private AnimationAsset _Asset;
        [Binding]
        public AnimationAsset Asset
        {
            set
            {
                var prev = _Asset;
                if (SetProperty(ref _Asset, value))
                {
                    prev?.RemoveWeakPropertyChanged(Asset_PropertyChanged);
                    _Asset?.AddWeakPropertyChanged(Asset_PropertyChanged);
                    OnPropertyChanged("Name");

                    Animation = _Asset?.Animation;
                }
            }
            get => _Asset;
        }

        public override string Name
        {
            set => base.Name = value;
            get => _Name ?? _Asset?.TagName ?? Type.ToString();
        }

        public AnimationReferenceObject(AnimationAsset asset = null)
        {
            if (asset != null) using (new AssetCommandHolder()) Asset = asset;
        }

        public AnimationReferenceObject(AnimationReferenceObject other, bool binding) : base(other, binding, true)
        {
            if (other._Asset != null)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    using (new AssetCommandHolder()) Asset = AssetManager.Instance.GetRedirection(other.Asset);
                });
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TagName":
                    OnPropertyChanged("Name");
                    break;
                case "Animation":
                    Animation = _Asset.Animation;
                    break;
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Asset != null) retains.Add(_Asset.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Asset == element)
            {
                from = this;
                return true;
            }
            return base.IsRetaining(element, out from);
        }

        public override SceneComponentType Type => SceneComponentType.AnimationReference;
        public override SceneComponent Clone(bool binding) => new AnimationReferenceObject(this, binding);

        protected override void SaveContent(XmlWriter writer)
        {
            base.SaveContent(writer);
            writer.WriteAttribute("asset", _Asset);
        }

        protected override void LoadContent(XmlNode node)
        {
            base.LoadContent(node);
            Asset = (AnimationAsset)node.ReadAttributeAsset("asset");
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            base.BuildContent(writer, param);
            BuildReference(writer, _Asset);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new AnimationReferenceObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new AnimationReferenceObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer)
        {
            writer.Write(KeyMask);
            BuildReference(writer, _Asset);
        }

        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
