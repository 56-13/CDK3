using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing.Meshing;

using CDK.Assets.Scenes;
using CDK.Assets.Updaters;

namespace CDK.Assets.Meshing
{
    public enum MeshAxis
    {
        ZUp,
        YUp
    }

    public class MeshAsset : Asset
    {
        private struct ImportInfo
        {
            public string Name;
            public string Extension;
            public string Path;
            public bool Geometry;
            public string[] Animations;
        }
        private AssetElementList<ImportInfo> _imports;

        private AssetElementList<MeshGeometry> _Geometries;
        public AssetElementList<MeshGeometry> Geometries
        {
            get
            {
                Load();
                return _Geometries;
            }
        }

        private AssetElementList<MeshAnimation> _Animations;
        public AssetElementList<MeshAnimation> Animations
        {
            get
            {
                Load();
                return _Animations;
            }
        }

        private float _Scale;
        public float Scale
        {
            set
            {
                Load();

                if (_Scale != value)        //TODO:WHY NOT SetProperty?
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new AssetPropertyCommand(this, _Scale, value, "Scale"));
                    }
                    IsDirty = true;

                    _Scale = value;

                    ReImport();

                    OnPropertyChanged("Scale");
                }
            }
            get
            {
                Load();
                return _Scale;
            }
        }

        private MeshAxis _Axis;
        public MeshAxis Axis
        {
            set
            {
                Load();

                if (_Axis != value)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new AssetPropertyCommand(this, _Axis, value, "Axis"));
                    }
                    IsDirty = true;

                    _Axis = value;
                    
                    ReImport();
                    
                    OnPropertyChanged("Axis");
                }
            }
            get
            {
                Load();
                return _Axis;
            }
        }

        private int XRotation
        {
            get
            {
                switch (_Axis)
                {
                    case MeshAxis.ZUp:
                        return -180;
                    case MeshAxis.YUp:
                        return -90;
                }
                return 0;
            }
        }

        private bool _FlipUV;
        public bool FlipUV
        {
            set
            {
                Load();

                if (_FlipUV != value)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new AssetPropertyCommand(this, _FlipUV, value, "FlipUV"));
                    }
                    IsDirty = true;

                    _FlipUV = value;

                    ReImport();

                    OnPropertyChanged("FlipUV");
                }
            }
            get
            {
                Load();
                return _FlipUV;
            }
        }

        private MeshSelection _Selection;
        public MeshSelection Selection
        {
            get
            {
                Load();
                return _Selection;
            }
        }

        public MeshAsset()
        {
            _Scale = 1;

            _imports = new AssetElementList<ImportInfo>(this);
            _Geometries = new AssetElementList<MeshGeometry>(this);
            _Animations = new AssetElementList<MeshAnimation>(this);

            _Selection = new MeshSelection(this, this);

            _Geometries.BeforeListChanged += Geometries_BeforeListChanged;
            _Geometries.ListChanged += Geometries_ListChanged;
            _Animations.BeforeListChanged += Animations_BeforeListChanged;
        }

        public MeshAsset(MeshAsset other, bool content) : base(other, content)
        {
            _imports = new AssetElementList<ImportInfo>(this);
            _Geometries = new AssetElementList<MeshGeometry>(this);
            _Animations = new AssetElementList<MeshAnimation>(this);

            if (content)
            {
                other.Load();

                _Scale = other._Scale;

                _Axis = other._Axis;

                _FlipUV = other._FlipUV;

                using (new AssetCommandHolder())
                {
                    foreach (var geometry in other._Geometries) _Geometries.Add(new MeshGeometry(this, geometry));
                    foreach (var animation in other._Animations) _Animations.Add(new MeshAnimation(this, animation));
                    foreach (var import in other._imports) _imports.Add(import);
                }
            }
            else
            {
                _Scale = 1;
            }

            _Selection = new MeshSelection(this, this);

            _Geometries.BeforeListChanged += Geometries_BeforeListChanged;
            _Geometries.ListChanged += Geometries_ListChanged;
            _Animations.BeforeListChanged += Animations_BeforeListChanged;
        }

        private void Geometries_BeforeListChanged(object sender, BeforeListChangedEventArgs<MeshGeometry> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (_Geometries[e.NewIndex].IsRetained()) e.Cancel = true;
                    break;
                case ListChangedType.Reset:
                    foreach (var geometry in _Geometries)
                    {
                        if (geometry.IsRetained())
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    break;
            }
        }

        private void Geometries_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null) break;
                    goto case ListChangedType.ItemAdded;
                case ListChangedType.ItemAdded:
                    if (_Geometries[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.Reset:
                    foreach (var geometry in _Geometries)
                    {
                        if (geometry.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
        }

        private void Animations_BeforeListChanged(object sender, BeforeListChangedEventArgs<MeshAnimation> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (_Animations[e.NewIndex].IsRetained()) e.Cancel = true;
                    break;
                case ListChangedType.Reset:
                    foreach (var animation in _Animations)
                    {
                        if (animation.IsRetained())
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    break;
            }
        }

        private void RemoveUnusedImports()
        {
            var i = 0;

            while (i < _imports.Count)
            {
                var import = _imports[i];

                if (import.Geometry && !_Geometries.Any(u => u.Origin.Name == import.Name)) 
                {
                    import.Geometry = false;
                }
                if (import.Animations != null && !_Animations.Any(a => import.Animations.Contains(a.Name)))
                {
                    import.Animations = null;
                }
                if (import.Geometry || import.Animations != null)
                { 
                    i++;
                }
                else
                {
                    _imports.RemoveAt(i);
                }
            }
        }

        public override AssetType Type => AssetType.Mesh;
        public override Asset Clone(bool content) => new MeshAsset(this, content);

        internal override void AddRetains(ICollection<string> retains)
        {
            Load();

            _Selection.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            Load();

            return _Selection.IsRetaining(element, out from);
        }

        public override bool Spawnable => true;
        public override SceneComponent NewSceneComponent() => new MeshObject(this);
        public override Scene NewScene()
        {
            Load();

            var obj = new MeshObject(this, true)
            {
                Located = true
            };
            var scene = NewDefaultScene(obj);
            scene.SelectedAnimation = obj;
            return scene;
        }

        public void Import(string path, bool loadGeometry, bool loadAnimation)
        {
            Load();

            Loader.Import(Path.GetFileNameWithoutExtension(path), path, XRotation, _Scale, _FlipUV, out var geometry, out var animations);

            if (loadGeometry)
            {
                if (!_Geometries.Any(u => u.Origin.Name == geometry.Name))
                {
                    _Geometries.Add(new MeshGeometry(this, geometry));
                }
                else loadGeometry = false;
            }
            if (loadAnimation)
            {
                loadAnimation = false;

                foreach (var animation in animations)
                {
                    if (!_Animations.Any(a=>a.Name == animation.Name))
                    {
                        _Animations.Add(new MeshAnimation(this, animation));

                        loadAnimation = true;
                    }
                }
            }
            if (loadGeometry || loadAnimation)
            {
                var import = new ImportInfo()
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    Extension = Path.GetExtension(path).Substring(1),
                    Path = ResourceManager.Instance.AddFile(path)
                };
                import.Geometry = loadGeometry;
                if (loadAnimation) import.Animations = animations.Select(a => a.Name).ToArray();
                _imports.Add(import);
            }
        }

        private void ReImport()
        {
            foreach (var import in _imports)
            {
                Loader.Import(import.Name, import.Path, XRotation, _Scale, _FlipUV, out var geometryOrigin, out var animations);
                
                foreach (var geometry in _Geometries)
                {
                    if (geometry.Origin.Name == geometryOrigin.Name)
                    {
                        geometry.Origin = geometryOrigin;
                        break;
                    }
                }
                foreach (var animation in animations)
                {
                    var target = _Animations.FirstOrDefault(a => a.Name == animation.Name);

                    if (target != null) target.Origin = animation;
                }
            }
        }

        public override void Import(string path)
        {
            Import(path, true, true);
        }

        public override void Export(string dirpath)
        {
            RemoveUnusedImports();

            foreach (var i in _imports)
            {
                var copyPath = Path.Combine(dirpath, $"{i.Name}.{i.Extension}");

                if (copyPath != i.Path)
                {
                    File.Delete(copyPath);
                    File.Copy(i.Path, copyPath);
                }
            }
        }

        private void BuildImpl(BinaryWriter writer)
        {
            //TODO
            throw new NotImplementedException();
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create)) 
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        BuildImpl(writer);
                    }
                }
            }
            else
            {
                BuildImpl(writer);
            }
        }

        protected override bool SaveContent()
        {
            Load();
            
            Directory.CreateDirectory(ContentPath);

            Export(ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("meshAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);

                writer.WriteAttribute("scale", _Scale, 1);
                writer.WriteAttribute("axis", _Axis);
                writer.WriteAttribute("flipuv", _FlipUV);

                writer.WriteStartElement("imports");
                foreach (var import in _imports)
                {
                    writer.WriteStartElement("import");
                    writer.WriteAttribute("name", import.Name);
                    writer.WriteAttribute("extension", import.Extension);
                    writer.WriteAttribute("geometry", import.Geometry);
                    if (import.Animations != null) writer.WriteAttributes("animations", import.Animations);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("geometries");
                foreach (var geometry in _Geometries)
                {
                    geometry.Save(writer);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("animations");
                foreach (var animation in _Animations)
                {
                    writer.WriteStartElement("animation");
                    writer.WriteAttribute("name", animation.Name);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                _Selection.Save(writer);

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

                if (node.LocalName != "meshAsset") throw new XmlException();

                Updater.ValidateMeshAsset(node);

                Scale = node.ReadAttributeFloat("scale", 1);
                Axis = node.ReadAttributeEnum<MeshAxis>("axis");
                FlipUV = node.ReadAttributeBool("flipuv");

                var geometrys = new Dictionary<string, Geometry>();
                var animationOrigins = new Dictionary<string, Animation>();

                var geometries = _Geometries.ToArray();
                var animations = _Animations.ToArray();

                _Geometries.BeforeListChanged -= Geometries_BeforeListChanged;
                _Animations.BeforeListChanged -= Animations_BeforeListChanged;

                _imports.Clear();
                _Geometries.Clear();
                _Animations.Clear();

                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "imports":
                            foreach (XmlNode importNode in subnode.ChildNodes)
                            {
                                var name = importNode.ReadAttributeString("name");
                                var ext = importNode.ReadAttributeString("extension");

                                var import = new ImportInfo()
                                {
                                    Name = name,
                                    Extension = ext,
                                    Path = Path.Combine(ContentPath, $"{name}.{ext}")
                                };
                                import.Geometry = importNode.ReadAttributeBool("geometry");
                                if (importNode.HasAttribute("animations")) import.Animations = importNode.ReadAttributeStrings("animations");
                                _imports.Add(import);

                                Loader.Import(name, import.Path, XRotation, _Scale, _FlipUV, out var newGeometry, out var newAnimations);

                                if (import.Geometry) 
                                {
                                    geometrys.Add(newGeometry.Name, newGeometry);
                                }
                                if (import.Animations != null)
                                {
                                    if (import.Animations.Length != newAnimations.Length) throw new XmlException();

                                    for (var i = 0; i < newAnimations.Length; i++)
                                    {
                                        var newAnimation = newAnimations[i];

                                        if (import.Animations[i] != newAnimation.Name) throw new XmlException();
                                        
                                        animationOrigins.Add(newAnimation.Name, newAnimation);
                                    }
                                }
                            }
                            break;
                        case "geometries":
                            foreach (XmlNode geometryNode in subnode.ChildNodes)
                            {
                                var name = geometryNode.ReadAttributeString("name");
                                var geometryOrigin = geometrys[name];

                                var geometry = geometries.FirstOrDefault(g => g.Name == name) ?? new MeshGeometry(this, geometryOrigin);
                                geometry.Load(geometryNode, geometryOrigin);
                               
                                _Geometries.Add(geometry);
                            }
                            break;
                        case "animations":
                            foreach (XmlNode animationNode in subnode.ChildNodes)
                            {
                                var name = animationNode.ReadAttributeString("name");
                                var animationOrigin = animationOrigins[name];

                                var animation = animations.FirstOrDefault(a => a.Name == name);
                                if (animation == null) animation = new MeshAnimation(this, animationOrigin);
                                else animation.Origin = animationOrigin;

                                _Animations.Add(animation);
                            }
                            break;
                        case "selection":
                            _Selection.Load(subnode);
                            break;
                    }
                }

                _Geometries.BeforeListChanged += Geometries_BeforeListChanged;
                _Animations.BeforeListChanged += Animations_BeforeListChanged;
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
            foreach (var i in _imports)
            {
                paths.Add($"{ContentPath}{Path.DirectorySeparatorChar}{i.Name}.{i.Extension}");
            }
        }
    }
}
