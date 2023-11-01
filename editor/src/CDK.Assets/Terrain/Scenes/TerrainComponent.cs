using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Configs;

namespace CDK.Assets.Terrain
{
    public class TerrainComponent : World
    {
        public override string Name
        {
            set => base.Name = value;
            get => _Name ?? _Asset?.TagName ?? Type.ToString();
        }

        private TerrainAsset _Asset;
        [Binding]
        public TerrainAsset Asset {

            set
            {
                if (AssetEdit) throw new InvalidOperationException();

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
                        Asset.AddWeakPropertyChanged(Asset_PropertyChanged);
                        Asset.AddWeakRefresh(Asset_Refresh);
                    }
                }
            }
            get => _Asset;
        }
        
        public bool AssetEdit { private set; get; }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TagName":
                    OnPropertyChanged("Name");
                    break;
            }
        }

        private void Asset_Refresh(object sender, EventArgs e) => OnRefresh();

        private bool _TileVisible;
        public bool TileVisible
        {
            set => SetProperty(ref _TileVisible, value, false);
            get => _TileVisible;
        }

        private SceneObject _propRef;
        private float _progress;
        private int _random;

        public TerrainComponent(TerrainAsset asset = null, bool assetEdit = false)
        {
            Asset = asset;

            AssetEdit = assetEdit && asset != null;

            _propRef = new SceneObject(asset.Props, true, true) { Fixed = true };

            if (AssetEdit)
            {
                Children.Add(new TerrainAltitudeComponent(asset));
                Children.Add(new TerrainSurfaceComponent(asset));
                Children.Add(new TerrainWallComponent(asset));
                Children.Add(new TerrainWaterComponent(asset));
                Children.Add(new TerrainTileComponent(asset));
                Children.Add(_propRef);
            }
            else _propRef.Parent = this;

            _random = RandomUtil.Next();
        }

        public TerrainComponent(TerrainComponent other, bool binding) : base(other, binding)
        {
            if (other._Asset != null)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    Asset = AssetManager.Instance.GetRedirection(other._Asset);
                });
            }

            _random = RandomUtil.Next();
        }

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
            from = null;
            return false;
        }

        public override SceneComponentType Type => SceneComponentType.Terrain;
        public override SceneComponent Clone(bool binding) => new TerrainComponent(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj)
        {
            if (_Asset != null && AssetEdit)
            {
                switch (obj.Type)
                {
                    case SceneComponentType.TerrainAltitude:
                        return ((TerrainAltitudeComponent)obj).Asset == _Asset;
                    case SceneComponentType.TerrainSurface:
                        return ((TerrainSurfaceComponent)obj).Asset == _Asset;
                    case SceneComponentType.TerrainWall:
                        return ((TerrainWallComponent)obj).Asset == _Asset;
                    case SceneComponentType.TerrainWater:
                        return ((TerrainWaterComponent)obj).Asset == _Asset;
                    case SceneComponentType.TerrainTile:
                        return ((TerrainTileComponent)obj).Asset == _Asset;
                    case SceneComponentType.Object:
                        return obj.GetBindingSource<SceneObject>(out var bindingSource) && bindingSource == Asset.Props;
                }
            }
            return false;
        }
        public override void AddSub(SceneComponentType type) { }
        private GroundConfig GroundConfig => Scene?.Config.Grounds[0] ?? AssetManager.Instance.Config.Scene.Grounds[0];
        public override ABoundingBox Space => _Asset?.Space ?? GroundConfig.Space;
        public override int Grid => _Asset?.Grid ?? GroundConfig.Grid;

        public override bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit)
        {
            if (_Asset != null) return _Asset.Intersects(ray, flags, out distance, out hit);

            hit = Hit.Zero;
            distance = 0f;
            return false;
        }

        public override float GetZ(in Vector3 pos) => _Asset?.GetAltitude(pos.ToVector2() / _Asset.Grid) * _Asset.Grid ?? 0f;

        internal override void Rewind()
        {
            _progress = 0;
            _random = RandomUtil.Next();
        }

        internal override void Update(float delta)
        {
            _progress += delta;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (_Asset != null)
            {
                _Asset.Display.Draw(graphics, layer, _progress, _random);

                if (layer == InstanceLayer.Cursor && _TileVisible && !(Scene.SelectedComponent is TerrainTileComponent))
                {
                    Asset.DrawTile(graphics, null);
                }
            }
        }

        protected override void SaveContent(XmlWriter writer) 
        {
            writer.WriteAttribute("asset", _Asset);
        }

        protected override void SaveChildren(XmlWriter writer)
        {
            //nothing to do
        }

        protected override void LoadContent(XmlNode node) 
        {
            Asset = (TerrainAsset)node.ReadAttributeAsset("asset");
        }

        protected override void LoadChildren(XmlNode node)
        {
            //nothing to do
        }

        internal override void Build(BinaryWriter writer) 
        {
            writer.Write((byte)BuildType.TerrainRef);
            BuildReference(writer, _Asset);
        }
    }
}
