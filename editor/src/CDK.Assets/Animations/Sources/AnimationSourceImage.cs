using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Animations.Components;
using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Animations.Sources
{
    public class AnimationSourceImage : AnimationSource
    {
        private RootImageAsset _RootImage;
        public RootImageAsset RootImage
        {
            set
            {
                if (SetProperty(ref _RootImage, value))
                {
                    if (AssetManager.Instance.CommandEnabled) SubImages = null;
                }
            }
            get => _RootImage;
        }

        private SubImageAsset[] _SubImages;
        public SubImageAsset[] SubImages
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled)
                {
                    if (_RootImage == null || value == null) value = new SubImageAsset[0];
                    else if (value != null)
                    {
                        foreach (var item in value)
                        {
                            if (item.RootImage != _RootImage) throw new InvalidOperationException();
                        }
                    }
                }
                if (!_SubImages.SequenceEqual(value))
                {
                    SetProperty(ref _SubImages, value);
                }
            }
            get => _SubImages;
        }

        private float _Duration;
        public float Duration
        {
            set => SetProperty(ref _Duration, value);
            get => _Duration;
        }

        private AnimationLoop _Loop;
        public AnimationLoop Loop
        {
            set => SetProperty(ref _Loop, value);
            get => _Loop;
        }

        public Material Material { private set; get; }

        public AnimationSourceImage(AssetElement parent) : base(parent)
        {
            _SubImages = new SubImageAsset[0];

            Material = new Material(this, MaterialUsage.Image);
        }

        public AnimationSourceImage(AssetElement parent, AnimationSourceImage other) : base(parent)
        {
            AssetManager.Instance.InvokeRedirection(() =>
            {
                _RootImage = AssetManager.Instance.GetRedirection(other._RootImage);
                _SubImages = AssetManager.Instance.GetRedirection(other._SubImages);
            });
            _Duration = other._Duration;
            _Loop = other._Loop;

            Material = new Material(this, other.Material, MaterialUsage.Image);
        }

        internal SubImageAsset GetImage(float progress)
        {
            if (_SubImages.Length == 0) return null;

            var p = progress;
            if (_Duration != 0) p /= _Duration;
            p = _Loop.GetProgress(p);

            return _SubImages[(int)Math.Round(p * (_SubImages.Length - 1))];
        }

        public override AnimationSourceType Type => AnimationSourceType.Image;
        public override AnimationSource Clone(AssetElement parent) => new AnimationSourceImage(parent, this);

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_RootImage != null) retains.Add(_RootImage.Key);

            if (_SubImages != null)
            {
                foreach (var subImage in _SubImages)
                {
                    retains.Add(subImage.Key);
                }
            }

            Material.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement obj, out AssetElement from)
        {
            if (obj is ImageAsset)
            {
                if (obj == _RootImage)
                {
                    from = this;
                    return true;
                }

                if (_SubImages != null)
                {
                    foreach (var subImage in _SubImages)
                    {
                        if (obj == subImage)
                        {
                            from = this;
                            return true;
                        }
                    }
                }
            }
            return Material.IsRetaining(obj, out from);
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("image");
            writer.WriteAttributeAssets("subImages", _SubImages);
            writer.WriteAttribute("rootImage", _RootImage);
            writer.WriteAttribute("duration", _Duration);
            _Loop.Save(writer, "loop");
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            RootImage = (RootImageAsset)node.ReadAttributeAsset("rootImage");
            SubImages = Array.ConvertAll(node.ReadAttributeAssets("subImages"), item => (SubImageAsset)item);
            Duration = node.ReadAttributeFloat("duration");
            Loop = new AnimationLoop(node, "loop");
            Material.Load(node.GetChildNode("material"));
        }

        internal override void Build(BinaryWriter writer)
        {
            BuildReference(writer, _RootImage);
            writer.WriteLength(_SubImages.Length);
            foreach (var subImage in _SubImages) BuildReference(writer, _RootImage, subImage, 1, false);
            writer.Write(_Duration);
            _Loop.Build(writer);
            Material.Build(writer);
        }
    }
}
