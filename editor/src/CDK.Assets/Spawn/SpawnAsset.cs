using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Attributes;
using CDK.Assets.Updaters;
using CDK.Assets.Scenes;

namespace CDK.Assets.Spawn
{
    public enum SpawnLocationType
    {
        Full,
        Half,
        One
    };

    public class SpawnAsset : Asset
    {
        private Attributes.Attribute _Attribute;
        public Attributes.Attribute Attribute
        {
            get
            {
                Load();
                return _Attribute;
            }
        }

        private Asset _View;
        public Asset View
        {
            set
            {
                if (value != null && (!value.Spawnable || value.Type == AssetType.Spawn)) throw new InvalidOperationException();

                Load();

                var prev = _View;
                if (SetProperty(ref _View, value))
                {
                    prev?.RemoveWeakRefresh(View_Refresh);
                    _View?.AddWeakRefresh(View_Refresh);
                }
            }
            get
            {
                Load();
                return _View;
            }
        }

        private void View_Refresh(object sender, EventArgs e)
        {
            if (this != sender) OnRefresh();
        }

        private string _CollisionSource;
        public string CollisionSource
        {
            set
            {
                Load();
                SetProperty(ref _CollisionSource, value);
            }
            get
            {
                Load();
                return _CollisionSource;
            }
        }

        private float _Collider;
        public float Collider
        {
            set
            {
                Load();
                if (_ColliderReference != null) _ColliderReference.Value = value.ToString();
                else SetProperty(ref _Collider, value);
            }
            get
            {
                Load();
                return _ColliderReference != null ? (float)_ColliderReference.GetConvertedNumericValue() : _Collider;
            }
        }

        private AttributeElement _ColliderReference;
        public AttributeElement ColliderReference
        {
            set
            {
                Load();

                var prev = _ColliderReference;
                if (SetProperty(ref _ColliderReference, value))
                {
                    prev?.RemoveWeakPropertyChanged(ColliderReference_PropertyChanged);
                    _ColliderReference?.AddWeakPropertyChanged(ColliderReference_PropertyChanged);

                    OnPropertyChanged("Collider");
                }
            }
            get
            {
                Load();
                return _ColliderReference;
            }
        }

        private void ColliderReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Collider");
        }

        private SpawnLocationType _LocationType;
        public SpawnLocationType LocationType
        {
            set
            {
                Load();
                SetProperty(ref _LocationType, value);
            }
            get
            {
                Load();
                return _LocationType;
            }
        }

        private AssetElementList<SpawnCollisionTarget> _CollisionTargets;
        public AssetElementList<SpawnCollisionTarget> CollisionTargets
        {
            get
            {
                Load();
                return _CollisionTargets;
            }
        }

        private AssetElementList<string> _CollisionTiles;
        public AssetElementList<string> CollisionTiles
        {
            get
            {
                Load();
                return _CollisionTiles;
            }
        }

        public SpawnAsset()
        {
            _Attribute = new Attributes.Attribute(this, "Attribute");

            _CollisionTargets = new AssetElementList<SpawnCollisionTarget>(this);
            _CollisionTargets.AddingNew += CollisionTargets_AddingNew;
            _CollisionTargets.ListChanged += CollisionTargets_ListChanged;

            _CollisionTiles = new AssetElementList<string>(this);
        }

        public SpawnAsset(SpawnAsset other, bool content)
        {
            _Attribute = new Attributes.Attribute(this, "Attribute", other.Attribute, content);

            _CollisionTargets = new AssetElementList<SpawnCollisionTarget>(this);
            _CollisionTiles = new AssetElementList<string>(this);

            if (content)
            {
                other.Load();

                AssetManager.Instance.InvokeRedirection(() =>
                {
                    if (other._View != null)
                    {
                        _View = AssetManager.Instance.GetRedirection(other._View);
                        _View.AddWeakRefresh(View_Refresh);
                    }
                    _ColliderReference = other._ColliderReference;
                });

                _CollisionSource = other._CollisionSource;
                _Collider = other._Collider;
                _LocationType = other._LocationType;

                using (new AssetCommandHolder())
                {
                    foreach (var e in other._CollisionTargets) _CollisionTargets.Add(new SpawnCollisionTarget(this, e));
                    foreach (var e in other._CollisionTiles) _CollisionTiles.Add(e);
                }
            }

            _CollisionTargets.AddingNew += CollisionTargets_AddingNew;
            _CollisionTargets.ListChanged += CollisionTargets_ListChanged;
        }

        private void CollisionTargets_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject = new SpawnCollisionTarget(this);
        }

        private void CollisionTargets_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (_CollisionTargets[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (_CollisionTargets[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var target in _CollisionTargets)
                    {
                        if (target.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
        }


        public void ReplaceCollisionSource(string prev, string next)
        {
            Load();

            if (_CollisionSource == prev) CollisionSource = next;

            foreach (var target in _CollisionTargets)
            {
                if (target.Source == prev) target.Source = next;
            }

            using (new AssetRetrieveHolder())
            {
                foreach (SpawnAsset sibling in GetSiblings())
                {
                    sibling.Load();

                    if (sibling._CollisionSource == prev) sibling.CollisionSource = _CollisionSource;

                    foreach (var siblingTarget in sibling._CollisionTargets)
                    {
                        if (siblingTarget.Source == prev) siblingTarget.Source = next;
                    }
                }
            }
        }

        public override AssetType Type => AssetType.Spawn;
        public override Asset Clone(bool content) => new SpawnAsset(this, content);
        protected override bool AddChildTypeEnabled(AssetType type)
        {
            switch (type)
            {
                case AssetType.Project:
                case AssetType.SubImage:
                case AssetType.Spawn:
                case AssetType.Terrain:
                case AssetType.Scene:
                    return false;
            }
            return true;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_View != null) retains.Add(_View.Key);
            if (_ColliderReference != null) retains.Add(_ColliderReference.Owner.Key);
        }
        
        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_View == element || (_ColliderReference != null && element.Contains(_ColliderReference)))
            {
                from = this;
                return true;
            }
            from = null;
            return false;
        }

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (SpawnAsset)asset;

            other.Load();

            return _Attribute.Compare(other._Attribute);
        }
        
        public override bool Spawnable => true;
        public override SceneComponent NewSceneComponent() => new SpawnObject(this);
        public override Scene NewScene() => NewDefaultScene(new SpawnObject(this));

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("spawnAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("view", _View);

                _Attribute.Save(writer);

                writer.WriteStartElement("collision");
                writer.WriteAttribute("source", _CollisionSource);
                writer.WriteAttribute("collider", _Collider);
                if (_ColliderReference != null) writer.WriteAttribute("colliderReference", $"{_ColliderReference.Owner.Key}/{_ColliderReference.Key}");
                writer.WriteAttribute("locationType", _LocationType, SpawnLocationType.Full);

                writer.WriteStartElement("tiles");
                foreach (var tile in _CollisionTiles)
                {
                    writer.WriteStartElement("tile");
                    writer.WriteAttribute("name", tile);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("targets");
                foreach (var target in _CollisionTargets) target.Save(writer);
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "spawnAsset") throw new XmlException();

                Updater.ValidateSpawnAsset(node);

                View = node.ReadAttributeAsset("view");

                _Attribute.Load(node.GetChildNode("attribute"));

                var subnode = node.GetChildNode("collision");

                CollisionSource = subnode.ReadAttributeString("source");
                Collider = subnode.ReadAttributeFloat("collider");

                var crefstr = subnode.ReadAttributeString("colliderReference");
                if (crefstr != null)
                {
                    var i = crefstr.LastIndexOf('/');
                    var key = crefstr.Substring(0, i);
                    var subkey = crefstr.Substring(i + 1);
                    ColliderReference = ((AttributeAsset)AssetManager.Instance.GetAsset(key))?.Attribute.Elements.FirstOrDefault(e => e.Key == subkey);
                }
                else ColliderReference = null;

                LocationType = subnode.ReadAttributeEnum("locationType", SpawnLocationType.Full);

                _CollisionTiles.Clear();
                foreach (XmlNode cnode in subnode.GetChildNode("tiles").ChildNodes) _CollisionTiles.Add(cnode.ReadAttributeString("name"));

                _CollisionTargets.Clear();
                foreach (XmlNode cnode in subnode.GetChildNode("targets").ChildNodes) _CollisionTargets.Add(new SpawnCollisionTarget(this, cnode));
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            _Attribute.GetLocaleStrings(strings);
        }
    }
}
