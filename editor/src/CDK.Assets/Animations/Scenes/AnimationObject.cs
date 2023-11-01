using System;
using System.Linq;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    
    public class AnimationObject : AnimationObjectBase
    {
        public override string Name
        {
            set => base.Name = value;
            get => _Name ?? Asset?.TagName ?? Type.ToString();
        }

        private AnimationAsset _Asset;
        public AnimationAsset Asset 
        { 
            private set
            {
                if (_Asset != value)
                {
                    _Asset?.RemoveWeakPropertyChanged(Asset_PropertyChanged);
                    _Asset = value;
                    _Asset?.AddWeakPropertyChanged(Asset_PropertyChanged);
                    OnPropertyChanged("Asset");

                    Animation = _Asset?.Animation ?? new AnimationFragment() { Parent = this };
                }
            }
            get => _Asset;
        }

        private bool _bindingChanging;

        public AnimationObject(AnimationAsset asset = null)
        {
            Asset = asset;

            if (Object != null) Children.Add(Object);

            PropertyChanged += AnimationObject_PropertyChanged;
            Children.ListChanged += Children_ListChanged;
        }

        public AnimationObject(AnimationObject other, bool binding) : base(other, binding, false)
        {
            AssetManager.Instance.InvokeRedirection(() =>
            {
                if (binding)
                {
                    _Asset = AssetManager.Instance.GetRedirection(other._Asset);
                    
                    if (_Asset != null)
                    {
                        _Asset.AddWeakPropertyChanged(Asset_PropertyChanged);
                        Animation = _Asset.Animation;
                    }
                    else Animation = other.Animation;
                }
                else if (other.Animation != null) Animation = new AnimationFragment(other.Animation) { Parent = this };

                if (Object != null) Children.Add(Object);

                PropertyChanged += AnimationObject_PropertyChanged;
                Children.ListChanged += Children_ListChanged;
            });
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

        private void AnimationObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Animation" && !_bindingChanging)
            {
                _bindingChanging = true;
                Children.Clear();
                if (Object != null) Children.Add(Object);
                _bindingChanging = false;
            }
        }

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging || e.PropertyDescriptor != null) return;

            _bindingChanging = true;
            var animation = ((AnimationObjectFragment)Children.FirstOrDefault())?.Origin;
            if (_Asset != null) _Asset.Animation = animation;
            else Animation = animation;
            _bindingChanging = false;
        }

        public override SceneComponentType Type => SceneComponentType.Animation;
        public override SceneComponent Clone(bool binding) => new AnimationObject(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[] { SceneComponentType.AnimationFragment };
        public override bool AddSubEnabled(SceneComponent obj) => obj.Type == SceneComponentType.AnimationFragment && Children.Count == 0;
        public override void AddSub(SceneComponentType type)
        {
            if (type == SceneComponentType.AnimationFragment && Animation == null)
            {
                if (_Asset != null) _Asset.Animation = new AnimationFragment();
                else Animation = new AnimationFragment() { Parent = this };
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            base.SaveContent(writer);
            writer.WriteAttribute("asset", _Asset);
        }

        protected override void SaveChildren(XmlWriter writer)
        {
            if (_Asset == null) Animation?.Save(writer);
        }

        protected override void LoadContent(XmlNode node)
        {
            base.LoadContent(node);
            Asset = (AnimationAsset)node.ReadAttributeAsset("asset");
        }

        protected override void LoadChildren(XmlNode node)
        {
            if (_Asset == null)
            {
                var subnode = node.GetChildNode("animation");

                if (subnode != null)
                {
                    if (Animation == null) Animation = new AnimationFragment();
                    Animation.Load(subnode);
                }
                else Animation = null;
            }
        }

        protected override void BuildChildren(BinaryWriter writer, SceneBuildParam param)
        {
            if (_Asset != null) throw new InvalidOperationException();

            if (Animation == null) throw new AssetException(Owner, "빈 애니메이션입니다.");

            Animation.Build(writer);
        }
    }
}
