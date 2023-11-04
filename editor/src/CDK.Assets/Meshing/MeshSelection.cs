using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing.Meshing;

using CDK.Assets.Texturing;
using CDK.Assets.Scenes;
using CDK.Assets.Animations;
using CDK.Assets.Animations.Components;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Meshing
{
    public class MeshSelection : AssetElement, ISkinContainer, IAnimationSubstance
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        
        private MeshAsset _Asset;
        public MeshAsset Asset
        {
            set
            {
                if (SetProperty(ref _Asset, value))
                {
                    if (AssetManager.Instance.CommandEnabled) Geometry = _Asset?.Geometries.FirstOrDefault();

                    if (_AnimationAsset == null) OnPropertyChanged("AnimationAsset");
                }
            }
            get => _Asset;
        }

        private MeshGeometry _Geometry;
        public MeshGeometry Geometry
        {
            set
            {
                if (_Geometry != value)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        ResetMaterials(value);

                        Animation = null;
                    }
                    SetProperty(ref _Geometry, value);
                }
            }
            get => _Geometry;
        }

        private MeshAsset _AnimationAsset;
        public MeshAsset AnimationAsset
        {
            set
            {
                if (SetProperty(ref _AnimationAsset, value))
                {
                    if (AssetManager.Instance.CommandEnabled) Animation = null;
                }
            }
            get => _AnimationAsset ?? _Asset;
        }

        private MeshAnimation _Animation;
        public MeshAnimation Animation
        {
            set => SetProperty(ref _Animation, value);
            get => _Animation;
        }

        private AnimationLoop _Loop;
        public AnimationLoop Loop
        {
            set => SetProperty(ref _Loop, value);
            get => _Loop;
        }

        private int _FrameDivision;
        public int FrameDivision
        {
            set => SetProperty(ref _FrameDivision, value);
            get => _FrameDivision;
        }

        private bool _Collision;
        public bool Collision
        {
            set => SetProperty(ref _Collision, value);
            get => _Collision;
        }

        public float GetInstanceProgress(float progress)
        {
            if (_Animation == null || progress <= 0) return 0;
            var duration = _Animation.Duration;
            progress = _Loop.GetProgress(progress / duration) * duration;
            return Instance.GetFrameDividedProgress(progress, _FrameDivision);
        }

        public AssetElementList<Material> Materials { private set; get; }
        public ISkin this[int index] => Materials[index];

        public MeshSelection(AssetElement parent)
        {
            Parent = parent;

            Materials = new AssetElementList<Material>(this);
            Materials.BeforeListChanged += Materials_BeforeListChanged;
        }

        public MeshSelection(AssetElement parent, MeshAsset asset)
        {
            Parent = parent;

            _Asset = asset;

            _Geometry = _Asset?.Geometries.FirstOrDefault();

            Materials = new AssetElementList<Material>(this);
            Materials.BeforeListChanged += Materials_BeforeListChanged;

            if (_Geometry != null)
            {
                using (new AssetCommandHolder()) ResetMaterials(_Geometry);
            }
        }

        public MeshSelection(AssetElement parent, MeshSelection other)
        {
            Parent = parent;

            AssetManager.Instance.InvokeRedirection(() =>
            {
                _Asset = AssetManager.Instance.GetRedirection(other._Asset);
                _Geometry = AssetManager.Instance.GetRedirection(other._Geometry);
                _AnimationAsset = AssetManager.Instance.GetRedirection(other._AnimationAsset);
                _Animation = AssetManager.Instance.GetRedirection(other._Animation);
            });

            Materials = new AssetElementList<Material>(this);

            using (new AssetCommandHolder())
            {
                foreach (var material in other.Materials) Materials.Add(new Material(this, material, MaterialUsage.Mesh));
            }
            Materials.BeforeListChanged += Materials_BeforeListChanged;

            _Loop = other._Loop;
            _FrameDivision = other._FrameDivision;
            _Collision = other._Collision;
        }

        private void Materials_BeforeListChanged(object sender, BeforeListChangedEventArgs<Material> e)
        {
            e.Cancel = true;
        }

        private void ResetMaterials(MeshGeometry geometry)
        {
            Materials.BeforeListChanged -= Materials_BeforeListChanged;

            Materials.Clear();

            if (geometry != null)
            {
                foreach (var config in geometry.MaterialConfigs)
                {
                    var material = new Material(this, MaterialUsage.Mesh)
                    {
                        Name = config.Name,
                        Origin = config.Origin,
                        Local = config.Origin == null
                    };

                    Materials.Add(material);
                }
            }

            Materials.BeforeListChanged += Materials_BeforeListChanged;
        }

        internal ShowFlags ShowFlags
        {
            get
            {
                var showFlags = ShowFlags.None;
                foreach (var material in Materials) showFlags |= material.ShowFlags;
                return showFlags;
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Asset != null) retains.Add(_Asset.Key);
            if (_AnimationAsset != null) retains.Add(_AnimationAsset.Key);
            foreach (var material in Materials) material.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (element == _Asset || element == _Geometry || element == _AnimationAsset || element == _Animation)
            {
                from = this;
                return true;
            }
            foreach (var material in Materials)
            {
                if (material.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }
        
        internal bool UpdateInstance(ref Instance instance)
        {
            var updated = false;
            if (_Geometry == null) 
            {
                updated = instance != null;
                instance = null;
                return updated;
            }
            if (instance == null || instance.Geometry != _Geometry.Origin)
            {
                updated = true;
                instance = new Instance(_Geometry.Origin);
            }
            var animation = _Animation?.Origin;
            instance.Skin = this;
            if (animation != instance.Animation)
            {
                updated = true;
                instance.Animation = animation;
            }
            instance.FrameDivision = _FrameDivision;
            return updated;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("selection");
            writer.WriteAttribute("asset", _Asset);
            if (_Geometry != null) writer.WriteAttribute("geometry", _Geometry.Name);
            writer.WriteAttribute("animationAsset", _AnimationAsset);
            if (_Animation != null) writer.WriteAttribute("animation", _Animation.Name);
            _Loop.Save(writer, "loop");
            writer.WriteAttribute("frameDivision", _FrameDivision);
            writer.WriteAttribute("collision", _Collision);

            foreach (var material in Materials) material.Save(writer);
            
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "selection") throw new XmlException();

            if (node.HasAttribute("asset"))
            {
                Asset = (MeshAsset)node.ReadAttributeAsset("asset");
            }

            if (node.HasAttribute("geometry"))
            {
                var geometry = node.ReadAttributeString("geometry");
                Geometry = _Asset.Geometries.FirstOrDefault(g => g.Name == geometry);
            }
            else Geometry = null;

            AnimationAsset = (MeshAsset)node.ReadAttributeAsset("animationAsset");

            if (node.HasAttribute("animation"))
            {
                var animation = node.ReadAttributeString("animation");
                Animation = _Asset.Animations.FirstOrDefault(a => a.Name == animation);
            }
            else Animation = null;

            Loop = new AnimationLoop(node, "loop");
            FrameDivision = node.ReadAttributeInt("frameDivision");
            Collision = node.ReadAttributeBool("collision");

            ResetMaterials(_Geometry);
            for (var i = 0; i < node.ChildNodes.Count; i++) Materials[i].Load(node.ChildNodes[i]);
        }

        internal void Build(BinaryWriter writer)
        {
            BuildReference(writer, _Asset);
            writer.Write((short)(_Geometry != null ? _Asset.Geometries.IndexOf(_Geometry) : -1));
            writer.Write(_AnimationAsset != null);
            if (_AnimationAsset != null) BuildReference(writer, _AnimationAsset);
            writer.Write((short)(_Animation != null ? _Asset.Animations.IndexOf(_Animation) : -1));
            writer.WriteLength(Materials.Count);
            foreach (var material in Materials) material.Build(writer);
            _Loop.Build(writer);
            writer.Write((short)_FrameDivision);
            writer.Write(_Collision);
        }

        SceneComponentType IAnimationSubstance.Type => SceneComponentType.Mesh;
        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new MeshObject(this) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new MeshSelection(parent, this);
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => Geometry?.GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => Build(writer);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) { }
    }
}
