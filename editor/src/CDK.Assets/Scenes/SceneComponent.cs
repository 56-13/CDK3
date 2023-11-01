using System;
using System.Windows.Forms;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public enum SceneComponentType
    {
        None,
        Object,
        Box,
        Sphere,
        Capsule,
        Mesh,
        Image,
        Particle,
        Trail,
        Sprite,
        DirectionalLight,
        PointLight,
        SpotLight,
        Animation,
        AnimationReference,
        Camera,
        Spawn,
        //==================================
        Environment,
        Ground,
        Terrain,
        TerrainAltitude,
        TerrainSurface,
        TerrainWall,
        TerrainWater,
        TerrainTile,
        TerrainRegion,
        Skybox,
        MeshGeometries,
        MeshGeometry,
        MeshAnimations,
        MeshAnimation,
        AnimationFragment
    }

    public abstract class SceneComponent : SceneContainer, IBindingCloneable<SceneComponent>
    {
        protected string _Name;
        [Binding]
        public virtual string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;
                SetProperty(ref _Name, value);
            }
            get => _Name ?? Type.ToString();
        }

        private AssetElement _Parent;
        public AssetElement Parent
        {
            internal set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    OnPropertyChanged("Parent");

                    var scene = GetAncestor<Scene>(false);

                    if (Scene != scene)
                    {
                        Unlink();
                        if (scene != null) Link(scene);
                    }
                }
            }
            get => _Parent;
        }
        public Scene Scene { private set; get; }

        public override AssetElement GetParent() => _Parent;
        public override string GetLocation() => _Parent is SceneComponent parent ? $"{parent.GetLocation()}.{Name}" : Name;

        public SceneComponent()
        {
            
        }

        public SceneComponent(SceneComponent other, bool binding, bool children) : base(other, binding, children)
        {
            _Name = other._Name;
        }
        
        protected virtual void OnLink() { }
        protected virtual void OnUnlink() { }
        internal void Link(Scene scene) 
        {
            if (Scene != null) throw new InvalidOperationException();

            Scene = scene;
            OnLink();
            foreach (var obj in Children) obj.Link(scene);
        }

        internal void Unlink()
        {
            if (Scene != null)
            {
                foreach (var obj in Children) obj.Unlink();
                OnUnlink();
                Scene = null;
            }
        }
        public abstract SceneComponentType Type { get; }
        public abstract SceneComponent Clone(bool binding);
        public virtual bool AllowPick => !(_Parent is SceneComponent parent) || parent.AllowPick;
        internal virtual void Select(bool focus) { }
        internal virtual ShowFlags Show() => ShowFlags.None;
        internal virtual bool CursorAlwaysVisible => false;
        internal virtual void Draw(Graphics graphics, InstanceLayer layer) { }
        internal virtual bool KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (_Parent is SceneContainer parent) parent.Children.Remove(this);
                    return true;
                case Keys.C:
                    if (e.Control)
                    {
                        AssetManager.Instance.Clip(this, false);
                        return true;
                    }
                    break;
                case Keys.X:
                    if (e.Control)
                    {
                        AssetManager.Instance.Clip(this, true);
                        return true;
                    }
                    break;
                case Keys.V:
                    if (e.Control)
                    {
                        if (Paste()) return true;
                    }
                    break;
            }
            return false;
        }
        internal virtual bool KeyUp(KeyEventArgs e) => false;
        internal virtual bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey) => false;

        internal void Save(XmlWriter writer)
        {
            var nodeName = Type.ToString();
            nodeName = $"{nodeName.Substring(0, 1).ToLower()}{nodeName.Substring(1)}";
            writer.WriteStartElement(nodeName);
            writer.WriteAttribute("name", _Name);
            SaveContent(writer);
            SaveChildren(writer);
            writer.WriteEndElement();
        }
        protected virtual void SaveContent(XmlWriter writer) { }

        internal void Load(XmlNode node)
        {
            var nodeName = Type.ToString();
            nodeName = $"{nodeName.Substring(0, 1).ToLower()}{nodeName.Substring(1)}";
            if (node.LocalName != nodeName) throw new XmlException();
            Name = node.ReadAttributeString("name");
            LoadContent(node);
            LoadChildren(node);
        }
        protected virtual void LoadContent(XmlNode node) { }
        
        internal static SceneComponent Create(SceneConfig config, XmlNode node)
        {
            var typeName = node.LocalName;
            typeName = $"{typeName.Substring(0, 1).ToUpper()}{typeName.Substring(1)}";
            var type = (SceneComponentType)Enum.Parse(typeof(SceneComponentType), typeName);
            var obj = Create(config, type);
            return obj;
        }

        internal static SceneComponent Create(SceneConfig config, SceneComponentType type)
        {
            switch (type)
            {
                case SceneComponentType.Object:
                    return new SceneObject();
                case SceneComponentType.Box:
                    return new BoxObject();
                case SceneComponentType.Sphere:
                    return new SphereObject();
                case SceneComponentType.Capsule:
                    return new CapsuleObject();
                case SceneComponentType.Mesh:
                    return new Meshing.MeshObject();
                case SceneComponentType.Image:
                    return new Texturing.ImageObject();
                case SceneComponentType.DirectionalLight:
                    return new DirectionalLightObject();
                case SceneComponentType.PointLight:
                    return new PointLightObject();
                case SceneComponentType.SpotLight:
                    return new SpotLightObject();
                case SceneComponentType.Animation:
                    return new Animations.AnimationObject();
                case SceneComponentType.AnimationReference:
                    return new Animations.AnimationReferenceObject();
                case SceneComponentType.Spawn:
                    return new Spawn.SpawnObject();
                case SceneComponentType.Camera:
                    return new CameraObject();
                case SceneComponentType.Environment:
                    return new Environment(config.Preferences[0], config.Environments[0]);
                case SceneComponentType.Ground:
                    return new Ground(config.Grounds[0]);
                case SceneComponentType.Terrain:
                    return new Terrain.TerrainComponent();
            }
            return null;
        }
    }
}
