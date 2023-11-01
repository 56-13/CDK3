using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Updaters;

using Point = System.Drawing.Point;

namespace CDK.Assets.Terrain
{
    public enum TerrainModifyMode
    {
        Add,
        Equal,
        Rub
    }

    public partial class TerrainAsset : Asset
    {
        private int _Width;
        public int Width
        {
            get
            {
                Load();
                return _Width;
            }
        }

        private int _Height;
        public int Height
        {
            get
            {
                Load();
                return _Height;
            }
        }

        private int _Altitude;
        public int Altitude
        {
            set
            {
                Load();
                if (SetProperty(ref _Altitude, value)) OnPropertyChanged("Space");
            }
            get
            {
                Load();
                return _Altitude;
            }
        }

        private int _Grid;
        public int Grid
        {
            set
            {
                Load();
                if (SetSharedProperty(ref _Grid, value)) OnPropertyChanged("Space");
            }
            get
            {
                Load();
                return _Grid;
            }
        }

        public ABoundingBox Space => new ABoundingBox(Vector3.Zero, new Vector3(_Width, _Height, _Altitude) * _Grid);

        private int _VertexCell;
        public int VertexCell
        {
            get
            {
                Load();
                return _VertexCell;
            }
        }

        private int _SurfaceCell;
        public int SurfaceCell
        {
            get
            {
                Load();
                return _SurfaceCell;
            }
        }
        
        private bool _BuildFixedPoint;
        public bool BuildFixedPoint
        {
            set
            {
                Load();
                SetSharedProperty(ref _BuildFixedPoint, value);
            }
            get
            {
                Load();
                return _BuildFixedPoint;
            }
        }

        private AssetElementList<TerrainSurface> _Surfaces;
        public AssetElementList<TerrainSurface> Surfaces
        {
            get
            {
                Load();
                return _Surfaces;
            }
        }

        private Vector2 _SurfaceOffset;
        internal Vector2 SurfaceOffset
        {
            private set
            {
                Load();
                SetProperty(ref _SurfaceOffset, value);
            }
            get
            {
                Load();
                return _SurfaceOffset;
            }
        }

        private float _AmbientOcclusionIntensity;
        public float AmbientOcclusionIntensity
        {
            set
            {
                Load();
                SetProperty(ref _AmbientOcclusionIntensity, value);
            }
            get
            {
                Load();
                return _AmbientOcclusionIntensity;
            }
        }

        private const float AmbientOcclusionRange = 2;

        private static readonly Vector2[] AmbientOcclusionOffsets;

        private AssetElementList<TerrainWall> _Walls;
        public AssetElementList<TerrainWall> Walls
        {
            get
            {
                Load();
                return _Walls;
            }
        }

        private AssetElementList<TerrainWallInstance> _WallInstances;
        public AssetElementList<TerrainWallInstance> WallInstances
        {
            get
            {
                Load();
                return _WallInstances;
            }
        }

        private AssetElementList<TerrainWater> _Waters;
        public AssetElementList<TerrainWater> Waters
        {
            get
            {
                Load();
                return _Waters;
            }
        }

        private SceneObject _Props;
        public SceneObject Props
        {
            get
            {
                Load();
                return _Props;
            }
        }

        private TerrainDisplay _Display;
        public TerrainDisplay Display
        {
            get
            {
                if (_Display == null)
                {
                    Load();
                    _Display = new TerrainDisplay(this);
                }
                return _Display;
            }
        }

        private float[,] _altitudes;
        private TerrainWaterInstance[,] _waterInstances;
        private Dictionary<Point, float> _modifyingAltitudes;
        private Dictionary<TerrainSurface, TerrainSurfaceInstances> _surfaceInstances;
        private string[,] _tiles;
        private Dictionary<ModifyingSurfaceKey, TerrainSurfaceInstance> _modifyingSurfaceInstances;
        private Dictionary<Point, TerrainWaterInstance> _modifyingWaterInstances;
        private Dictionary<Point, string> _modifyingTiles;

        public const int DefaultVertexPixel = 20;
        public const int DefaultSurfacePixel = 5;

        static TerrainAsset()
        {
            AmbientOcclusionOffsets = new Vector2[32];

            for (var i = 0; i < 32; i++)
            {
                var s = (i + 1) / 32f;
                var a = (float)Math.Sqrt(s * 512);
                var b = (float)Math.Sqrt(s) * AmbientOcclusionRange;
                AmbientOcclusionOffsets[i].X = (float)Math.Sin(a) * b;
                AmbientOcclusionOffsets[i].Y = (float)Math.Cos(a) * b;
            }
        }

        public TerrainAsset()
        {
            var config = AssetManager.Instance.Config.Scene.Grounds[0];

            _Width = config.Width;
            _Height = config.Height;
            _Altitude = config.Altitude;
            _Grid = config.Grid;
            _VertexCell = Math.Max(_Grid / DefaultVertexPixel, 1);
            _SurfaceCell = Math.Max(_Grid / (DefaultSurfacePixel * _VertexCell), 1);

            _altitudes = new float[_Width * _VertexCell + 1, _Height * _VertexCell + 1];

            AmbientOcclusions = new byte[_Width * _VertexCell * _SurfaceCell + 1, _Height * _VertexCell * _SurfaceCell + 1];
            _AmbientOcclusionIntensity = 1;

            _surfaceInstances = new Dictionary<TerrainSurface, TerrainSurfaceInstances>();

            _Surfaces = new AssetElementList<TerrainSurface>(this);
            _Surfaces.BeforeListChanged += Surfaces_BeforeListChanged;
            _Surfaces.ListChanged += Surfaces_ListChanged;

            _Walls = new AssetElementList<TerrainWall>(this);
            _Walls.BeforeListChanged += Walls_BeforeListChanged;
            _Walls.ListChanged += Walls_ListChanged;

            _Waters = new AssetElementList<TerrainWater>(this);
            _Waters.BeforeListChanged += Waters_BeforeListChanged;
            _Waters.ListChanged += Waters_ListChanged;

            _WallInstances = new AssetElementList<TerrainWallInstance>(this);

            _waterInstances = new TerrainWaterInstance[_Width, _Height];

            _Props = new SceneObject
            {
                Parent = this,
                Name = "Props",
                Fixed = true
            };

            _tiles = new string[_Width, _Height];

            _modifyingAltitudes = new Dictionary<Point, float>();
            _modifyingSurfaceInstances = new Dictionary<ModifyingSurfaceKey, TerrainSurfaceInstance>();
            _modifyingWaterInstances = new Dictionary<Point, TerrainWaterInstance>();
            _modifyingTiles = new Dictionary<Point, string>();

            //_Trigger = new CDK.Asset.Trigger.Trigger(this);
            //_Trigger.PropertyChanged += Trigger_PropertyChanged;
        }

        public TerrainAsset(TerrainAsset other, bool content) : base(other, content)
        {
            other.Load();

            _Grid = other._Grid;
            _VertexCell = other._VertexCell;
            _SurfaceCell = other._SurfaceCell;
            _BuildFixedPoint = other._BuildFixedPoint;

            _Surfaces = new AssetElementList<TerrainSurface>(this);
            _Walls = new AssetElementList<TerrainWall>(this);
            _Waters = new AssetElementList<TerrainWater>(this);
            
            _surfaceInstances = new Dictionary<TerrainSurface, TerrainSurfaceInstances>();
            _WallInstances = new AssetElementList<TerrainWallInstance>(this);

            if (content)
            {
                _Width = other._Width;
                _Height = other._Height;
                _Altitude = other._Altitude;

                _altitudes = (float[,])other._altitudes.Clone();

                AmbientOcclusions = (byte[,])other.AmbientOcclusions.Clone();
                _AmbientOcclusionIntensity = other._AmbientOcclusionIntensity;

                _SurfaceOffset = other._SurfaceOffset;

                using (new AssetCommandHolder())
                {
                    foreach (var surface in other._Surfaces)
                    {
                        var copySurface = new TerrainSurface(this, surface);

                        _Surfaces.Add(copySurface);

                        _surfaceInstances.Add(copySurface, new TerrainSurfaceInstances(other._surfaceInstances[surface]));
                    }
                    foreach (var wall in other._Walls)
                    {
                        var copyWall = new TerrainWall(this, wall);
                        
                        _Walls.Add(copyWall);
                    }
                    foreach (var wallInstance in other._WallInstances)
                    {
                        var i = other.Walls.IndexOf(wallInstance.Wall);

                        var copyWallInstance = new TerrainWallInstance(_Walls[i]);
                        foreach (var p in wallInstance.Points) copyWallInstance.Points.Add(new TerrainWallInstancePoint(copyWallInstance, p.X, p.Y, p.Z));
                        _WallInstances.Add(copyWallInstance);
                    }
                    foreach (var water in other._Waters)
                    {
                        var copyWater = new TerrainWater(this, water);

                        _Waters.Add(copyWater);
                    }
                }

                _waterInstances = new TerrainWaterInstance[_Width, _Height];

                for (var y = 0; y < _Height; y++)
                {
                    for (var x = 0; x < _Width; x++)
                    {
                        if (other._waterInstances[x, y] != null)
                        {
                            int index = other._Waters.IndexOf(other._waterInstances[x, y].Water);

                            _waterInstances[x, y] = new TerrainWaterInstance(_Waters[index], other._waterInstances[x, y].Altitude);
                        }
                    }
                }

                _Props = new SceneObject(other._Props, false, true)
                {
                    Parent = this
                };

                _tiles = (string[,])other._tiles.Clone();
            }
            else
            {
                var config = AssetManager.Instance.Config.Scene.Grounds[0];

                _Width = config.Width;
                _Height = config.Height;
                _Altitude = other._Altitude;

                _altitudes = new float[_Width * _VertexCell + 1, _Height * _VertexCell + 1];

                AmbientOcclusions = new byte[_Width * _VertexCell * _SurfaceCell + 1, _Height * _VertexCell * _SurfaceCell + 1];
                _AmbientOcclusionIntensity = 1;

                _waterInstances = new TerrainWaterInstance[_Width, _Height];

                _Props = new SceneObject()
                {
                    Parent = this,
                    Name = "Props"
                };

                _tiles = new string[_Width, _Height];
            }

            _Surfaces.BeforeListChanged += Surfaces_BeforeListChanged;
            _Surfaces.ListChanged += Surfaces_ListChanged;

            _Walls.BeforeListChanged += Walls_BeforeListChanged;
            _Walls.ListChanged += Walls_ListChanged;

            _Waters.BeforeListChanged += Waters_BeforeListChanged;
            _Waters.ListChanged += Waters_ListChanged;

            _modifyingAltitudes = new Dictionary<Point, float>();
            _modifyingSurfaceInstances = new Dictionary<ModifyingSurfaceKey, TerrainSurfaceInstance>();
            _modifyingWaterInstances = new Dictionary<Point, TerrainWaterInstance>();
            _modifyingTiles = new Dictionary<Point, string>();

            //_Trigger = new CDK.Asset.Trigger.Trigger(this);
            //_Trigger.PropertyChanged += Trigger_PropertyChanged;
        }

        private void Surfaces_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainSurface> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (!_surfaceInstances[_Surfaces[e.NewIndex]].IsEmpty) e.Cancel = true;
                    else
                    {
                        _surfaceInstances.Remove(_Surfaces[e.NewIndex]);

                        _Display?.RemoveSurface(_Surfaces[e.NewIndex]);
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    if (!_surfaceInstances[_Surfaces[e.NewIndex]].IsEmpty) e.Cancel = true;
                    else
                    {
                        _surfaceInstances.Remove(_Surfaces[e.NewIndex]);

                        _Display?.RemoveSurface(_Surfaces[e.NewIndex]);
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var surface in _Surfaces)
                    {
                        if (!_surfaceInstances[surface].IsEmpty)
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    if (!e.Cancel)
                    {
                        _surfaceInstances.Clear();

                        _Display?.ClearSurfaces();
                    }
                    break;
            }
        }

        private void Surfaces_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (_Surfaces[e.NewIndex].Parent != this) throw new InvalidOperationException();

                    _surfaceInstances.Add(_Surfaces[e.NewIndex], new TerrainSurfaceInstances(_Width * _VertexCell * _SurfaceCell + 1, _Height * _VertexCell * _SurfaceCell + 1));

                    _Display?.AddSurface(_Surfaces[e.NewIndex]);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (_Surfaces[e.NewIndex].Parent != this) throw new InvalidOperationException();

                        _surfaceInstances.Add(_Surfaces[e.NewIndex], new TerrainSurfaceInstances(_Width * _VertexCell * _SurfaceCell + 1, _Height * _VertexCell * _SurfaceCell + 1));

                        _Display?.AddSurface(_Surfaces[e.NewIndex]);
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var surface in _Surfaces)
                    {
                        if (surface.Parent != this) throw new InvalidOperationException();

                        _Display?.AddSurface(surface);
                    }
                    break;
            }
        }

        private void Walls_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainWall> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (_WallInstances.Any(i => i.Wall == _Walls[e.NewIndex])) e.Cancel = true;
                    break;
                case ListChangedType.Reset:
                    foreach (var wall in _Walls)
                    {
                        if (_WallInstances.Any(i => i.Wall == wall))
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    break;
            }
        }

        private void Walls_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (_Walls[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (_Walls[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var wall in _Walls)
                    {
                        if (wall.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
        }

        private bool IsWaterRetaining(int index)
        {
            var water = _Waters[index];
            for (var x = 0; x < _Width; x++)
            {
                for (var y = 0; y < _Height; y++)
                {
                    if (_waterInstances[x, y] != null && _waterInstances[x, y].Water == water) return true;
                }
            }
            return false;
        }

        private void Waters_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainWater> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (IsWaterRetaining(e.NewIndex)) e.Cancel = true;

                    _Display?.RemoveWater(_Waters[e.NewIndex]);
                    break;
                case ListChangedType.Reset:
                    for (int i = 0; i < _Surfaces.Count; i++)
                    {
                        if (IsWaterRetaining(i))
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    _Display?.ClearWaters();
                    break;
            }
        }

        private void Waters_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (_Waters[e.NewIndex].Parent != this) throw new InvalidOperationException();

                    _Display?.AddWater(_Waters[e.NewIndex]);
                    _Display?.UpdateWater();
                    break;
                case ListChangedType.ItemDeleted:
                    _Display?.UpdateWater();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (_Waters[e.NewIndex].Parent != this) throw new InvalidOperationException();

                        _Display?.AddWater(_Waters[e.NewIndex]);
                    }
                    _Display?.UpdateWater();
                    break;
                case ListChangedType.Reset:
                    foreach (var water in _Waters)
                    {
                        if (water.Parent != this) throw new InvalidOperationException();

                        _Display?.AddWater(water);
                    }
                    _Display?.UpdateWater();
                    break;
            }
        }

        public override AssetType Type => AssetType.Terrain;
        public override Asset Clone(bool content) => new TerrainAsset(this, content);

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var surface in _Surfaces)
            {
                surface.AddRetains(retains);
            }
            foreach (var wall in _Walls)
            {
                wall.AddRetains(retains);
            }
            foreach (var water in _Waters)
            {
                water.AddRetains(retains);
            }

            _Props.AddRetains(retains);

            /*
            _Trigger.AddRetains(retains);
            */
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            Load();

            foreach (var surface in _Surfaces)
            {
                if (surface.IsRetaining(element, out from)) return true;
            }
            foreach (var wall in _Walls)
            {
                if (wall.IsRetaining(element, out from)) return true;
            }
            foreach (var water in _Waters)
            {
                if (water.IsRetaining(element, out from)) return true;
            }

            if (_Props.IsRetaining(element, out from)) return true;

            //if (_Trigger.IsRetaining(element)) return true;

            from = null;
            return false;
        }
        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (TerrainAsset)asset;

            other.Load();

            if (_VertexCell != other._VertexCell || _SurfaceCell != other._SurfaceCell)
            {
                return false;
            }

            //TODO
            /*
            return _Trigger.Format == other._Trigger.Format;
            */
            return true;
        }

        public override SceneComponent NewSceneComponent() => new TerrainComponent(this);
        public override Scene NewScene() => NewDefaultScene(new TerrainComponent(this, true));

        private void LoadTerrain()
        {
            var path = $"{ContentPath}.trn";

            if (File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        for (var vy = 0; vy < _altitudes.GetLength(1); vy++)
                        {
                            for (var vx = 0; vx < _altitudes.GetLength(0); vx++)
                            {
                                _altitudes[vx, vy] = reader.ReadSingle();
                            }
                        }
                        foreach (var surface in _Surfaces)
                        {
                            var surfaceInstances = _surfaceInstances[surface];

                            surfaceInstances.Load(reader);
                        }
                        for (var sy = 0; sy < AmbientOcclusions.GetLength(1); sy++)
                        {
                            for (var sx = 0; sx < AmbientOcclusions.GetLength(0); sx++)
                            {
                                AmbientOcclusions[sx, sy] = reader.ReadByte();
                            }
                        }
                    }
                }
            }
        }

        private void SaveTerrain()
        {
            CommitAmbientOcclusion();

            var path = $"{ContentPath}.trn";

            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(fs))
                {
                    for (var vy = 0; vy < _altitudes.GetLength(1); vy++)
                    {
                        for (var vx = 0; vx < _altitudes.GetLength(0); vx++)
                        {
                            writer.Write(_altitudes[vx, vy]);
                        }
                    }
                    foreach (var surface in _Surfaces)
                    {
                        var surfaceInstances = _surfaceInstances[surface];

                        surfaceInstances.Save(writer);
                    }
                    for (var sy = 0; sy < AmbientOcclusions.GetLength(1); sy++)
                    {
                        for (var sx = 0; sx < AmbientOcclusions.GetLength(0); sx++)
                        {
                            writer.Write(AmbientOcclusions[sx, sy]);
                        }
                    }
                }
            }
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("terrainAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("width", _Width);
                writer.WriteAttribute("height", _Height);
                writer.WriteAttribute("altitude", _Altitude);
                writer.WriteAttribute("grid", _Grid);
                writer.WriteAttribute("vertexCell", _VertexCell);
                writer.WriteAttribute("surfaceCell", _SurfaceCell);

                writer.WriteAttribute("buildFixedPoint", _BuildFixedPoint);

                //Camera.Save(writer);

                writer.WriteAttribute("ambientOcclusionIntensity", _AmbientOcclusionIntensity);
                writer.WriteAttribute("surfaceOffset", _SurfaceOffset);

                writer.WriteStartElement("surfaces");
                foreach (var surface in _Surfaces) surface.Save(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("walls");
                foreach (var wall in _Walls) wall.Save(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("wallInstances");
                foreach (var wallInstance in _WallInstances) wallInstance.Save(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("waters");
                foreach (var water in _Waters) water.Save(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("waterInstances");
                for (var y = 0; y < _Height; y++)
                {
                    for (var x = 0; x < _Width; x++)
                    {
                        if (_waterInstances[x, y] != null)
                        {
                            writer.WriteStartElement("waterInstance");
                            writer.WriteAttribute("x", x);
                            writer.WriteAttribute("y", y);
                            writer.WriteAttribute("water", _waterInstances[x, y].Water.Index);
                            writer.WriteAttribute("altitude", _waterInstances[x, y].Altitude);
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();

                _Props.Save(writer);

                writer.WriteStartElement("tiles");
                for (var y = 0; y < _Height; y++)
                {
                    for (var x = 0; x < _Width; x++)
                    {
                        if (_tiles[x, y] != null)
                        {
                            writer.WriteStartElement("tile");
                            writer.WriteAttribute("x", x);
                            writer.WriteAttribute("y", y);
                            writer.WriteAttribute("element", _tiles[x, y]);
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();

                //TODO
                /*
                _Trigger.Save(writer);
                */
                writer.WriteEndElement();
            }

            SaveTerrain();

            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                AssetManager.Instance.Purge();

                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "terrainAsset") throw new XmlException();

                Updater.ValidateTerrainAsset(node);

                State state = null;

                if (AssetManager.Instance.CommandEnabled) state = new State(this);

                _Grid = node.ReadAttributeInt("grid");
                _VertexCell = node.ReadAttributeInt("vertexCell");
                _SurfaceCell = node.ReadAttributeInt("surfaceCell");

                BuildFixedPoint = node.ReadAttributeBool("buildFixedPoint");

                _Width = node.ReadAttributeInt("width");
                _Height = node.ReadAttributeInt("height");

                _altitudes = new float[_Width * _VertexCell + 1, _Height * _VertexCell + 1];
                AmbientOcclusions = new byte[_Width * _VertexCell * _SurfaceCell + 1, _Height * _VertexCell * _SurfaceCell + 1];
                AmbientOcclusionIntensity = node.ReadAttributeFloat("ambientOcclusionIntensity");
                _aoupdated = false;

                SurfaceOffset = node.ReadAttributeVector2("surfaceOffset");

                _Surfaces.Clear();
                _Walls.Clear();
                _WallInstances.Clear();
                _Waters.Clear();

                _waterInstances = new TerrainWaterInstance[_Width, _Height];

                _Props.Children.Clear();

                _tiles = new string[_Width, _Height];

                var tileConstants = Project.GetTerrainTileConstants();
                
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "surfaces":
                            foreach (XmlNode surfaceNode in subnode.ChildNodes)
                            {
                                _Surfaces.Add(new TerrainSurface(this, surfaceNode));
                            }
                            break;
                        case "walls":
                            foreach (XmlNode wallNode in subnode.ChildNodes)
                            {
                                _Walls.Add(new TerrainWall(this, wallNode));
                            }
                            break;
                        case "wallInstances":
                            foreach (XmlNode wallInstanceNode in subnode.ChildNodes)
                            {
                                _WallInstances.Add(new TerrainWallInstance(this, wallInstanceNode));
                            }
                            break;
                        case "waters":
                            foreach (XmlNode waterNode in subnode.ChildNodes)
                            {
                                _Waters.Add(new TerrainWater(this, waterNode));
                            }
                            break;
                        case "waterInstances":
                            foreach (XmlNode waterInstanceNode in subnode.ChildNodes)
                            {
                                var x = waterInstanceNode.ReadAttributeInt("x");
                                var y = waterInstanceNode.ReadAttributeInt("y");
                                var water = _Waters[waterInstanceNode.ReadAttributeInt("water")];
                                float altitude = waterInstanceNode.ReadAttributeFloat("altitude");
                                _waterInstances[x, y] = new TerrainWaterInstance(water, altitude);
                            }
                            break;
                        case "object":
                            _Props.Load(subnode);
                            break;
                        case "tiles":
                            foreach (XmlNode tileNode in subnode.ChildNodes)
                            {
                                var x = tileNode.ReadAttributeInt("x");
                                var y = tileNode.ReadAttributeInt("y");
                                var e = tileNode.ReadAttributeString("element");
                                _tiles[x, y] = e;
                                if (!tileConstants.Any(c => c.Name == e)) throw new XmlException($"터레인 타일에 설정되지 않은 값이 있습니다. {e}");
                            }
                            break;
                    }
                }
                LoadTerrain();

                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new ResetCommand(this, state));
                }
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
            paths.Add($"{ContentPath}.trn");
        }
        /*
        private Bitmap CreateLightBitmap(byte[,,] scratch, int width, int height)
        {
            Bitmap image = new Bitmap(width, height);

            BitmapData imageData = image.LockBits(new GDIRectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)imageData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int offset = (y * width + x) * 4;
                        ptr[offset + 0] = scratch[x, y, 2];
                        ptr[offset + 1] = scratch[x, y, 1];
                        ptr[offset + 2] = scratch[x, y, 0];
                        ptr[offset + 3] = 255;
                    }
                }
            }
            image.UnlockBits(imageData);

            return image;
        }

        private TextureFormat GetBuildTextureFormat(MapBuildImage format)
        {
            TextureFormat textureFormat = null;
            switch (_BuildLight)
            {
                case MapBuildImage.RawRGB565:
                    textureFormat = new TextureFormat(null);
                    textureFormat.Sources = new TextureSource[]
                    {
                        TextureSource.RawRGB565
                    };
                    break;
                case MapBuildImage.RawRGB8888:
                    textureFormat = new TextureFormat(null);
                    textureFormat.Sources = new TextureSource[]
                    {
                        TextureSource.RawRGBA8888
                    };
                    break;
                case MapBuildImage.ETC2_PVRTC1_DXT1:
                    textureFormat = new TextureFormat(null);
                    textureFormat.Sources = new TextureSource[]
                    {
                        TextureSource.ETC2_RGB,
                        TextureSource.PVRTC1_RGB,
                        TextureSource.DXT1
                    };
                    break;
            }
            textureFormat.Mipmap = TextureMipmap.Nearest;

            return textureFormat;
        }

        internal override int BuildProgress
        {
            get
            {
                return 6;
            }
        }

        private void BuildImpl(BinaryWriter writer, AssetBuildPlatform platform)
        {
            UpdateLight();

            writer.Write((byte)_Width);
            writer.Write((byte)_Height);

            AssetManager.Instance.Progress(Location + " - 미니맵 구성중", 1);       //1
            using (Bitmap image = CreateMinimap())
            {
                GetBuildTextureFormat(_BuildMinimap).Build(writer, image, platform);
            }
            writer.Write((byte)_MinimapLeft);
            writer.Write((byte)_MinimapRight);
            writer.Write((byte)_MinimapTop);
            writer.Write((byte)_MinimapBottom);

            AssetManager.Instance.Progress(Location + " - 지형정보 구성중", 1);       //2
            writer.Write(_Ambient.ToRgba());
            _Light.Build(writer);

            for (int y = 0; y <= _Height; y++)
            {
                for (int x = 0; x <= _Width; x++)
                {
                    if (_BuildFixedPoint)
                    {
                        writer.WriteFixed(altitudes[x, y]);
                    }
                    else
                    {
                        writer.Write(altitudes[x, y]);
                    }
                }
            }

            List<MapSurface> buildSurfaces = new List<MapSurface>();
            foreach (MapSurface surface in _Surfaces)
            {
                if (!surfaceInstances[surface].IsEmpty)
                {
                    buildSurfaces.Add(surface);
                }
            }

            AssetManager.Instance.Progress(Location + " - 라이트맵 구성중", 1);       //3
            {
                List<Bitmap> lightImages = new List<Bitmap>();

                byte[,,] lightScratch = new byte[2048, 2048, 3];
                int lightX = 0;
                int lightY = 0;
                int lightWidth = 0;
                int lightHeight = 0;

                try
                {
                    foreach (MapSurface surface in buildSurfaces)
                    {
                        MapSurfaceInstances surfaceInstances = this.surfaceInstances[surface];

                        for (int y = 0; y < _Height; y++)
                        {
                            for (int x = 0; x < _Width; x++)
                            {
                                bool visible = false;

                                for (int lcy = 0; lcy <= _TileCell && !visible; lcy++)
                                {
                                    for (int lcx = 0; lcx <= _TileCell && !visible; lcx++)
                                    {
                                        int cx = x * _TileCell + lcx;
                                        int cy = y * _TileCell + lcy;

                                        byte a = FloatToUNorm(surfaceInstances[cx, cy].current);

                                        if (a != 0)
                                        {
                                            visible = true;
                                        }
                                    }
                                }
                                if (visible)
                                {
                                    for (int lcy = -1; lcy <= _TileCell + 1; lcy++)
                                    {
                                        for (int lcx = -1; lcx <= _TileCell + 1; lcx++)
                                        {
                                            int sx = x * _TileCell + MathUtil.Clamp(lcx, 0, _TileCell);
                                            int sy = y * _TileCell + MathUtil.Clamp(lcy, 0, _TileCell);
                                            int dx = lightX + lcx + 1;
                                            int dy = lightY + lcy + 1;

                                            lightScratch[dx, dy, 0] = FloatToUNorm((1.0f - light[sx, sy, 0] * _OcclusionIntensity) * 0.5f);
                                            lightScratch[dx, dy, 1] = FloatToUNorm(surfaceInstances[sx, sy].current);
                                            lightScratch[dx, dy, 2] = FloatToUNorm(light[sx, sy, 1]);
                                        }
                                    }
                                    lightX += (_TileCell + 3);
                                    if (lightWidth < lightX) lightWidth = lightX;
                                    if (lightHeight < lightY + (_TileCell + 5)) lightHeight = lightY + (_TileCell + 5);

                                    if (lightX + (_TileCell + 3) > 2048)
                                    {
                                        lightX = 0;
                                        lightY += _TileCell + 3;
                                        if (lightY + (_TileCell + 3) > 2048)
                                        {
                                            lightImages.Add(CreateLightBitmap(lightScratch, lightWidth, lightHeight));
                                            lightY = 0;
                                            lightWidth = 0;
                                            lightHeight = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (lightWidth != 0 && lightHeight != 0)
                    {
                        lightImages.Add(CreateLightBitmap(lightScratch, lightWidth, lightHeight));
                    }

                    TextureFormat textureFormat = GetBuildTextureFormat(_BuildLight);
                    int count = lightImages.Count;
                    writer.WriteLength(count);
                    for (int i = 0; i < count; i++)
                    {
                        textureFormat.Build(writer, lightImages[i], platform);
                    }
                }
                finally
                {
                    foreach (Bitmap lightImage in lightImages) lightImage.Dispose();
                }
            }

            AssetManager.Instance.Progress(Location + " - 표면정보 구성중", 1);       //4

            writer.Write((short)_SurfaceOffset.X);
            writer.Write((short)_SurfaceOffset.Y);
            {
                int totalQuadCount = 0;
                long totalQuadCountPosition = writer.BaseStream.Position;
                writer.Write(0);

                writer.WriteLength(buildSurfaces.Count);

                foreach (MapSurface surface in buildSurfaces)
                {
                    surface.Build(writer);

                    int localQuadCount = 0;
                    long localQuadCountPosition = writer.BaseStream.Position;
                    writer.Write(0);

                    MapSurfaceInstances surfaceInstances = this.surfaceInstances[surface];

                    for (int y = 0; y < _Height; y++)
                    {
                        for (int x = 0; x < _Width; x++)
                        {
                            bool visible = false;

                            for (int lcy = 0; lcy <= _TileCell && !visible; lcy++)
                            {
                                for (int lcx = 0; lcx <= _TileCell && !visible; lcx++)
                                {
                                    int cx = x * _TileCell + lcx;
                                    int cy = y * _TileCell + lcy;

                                    int a = FloatToUNorm(surfaceInstances[cx, cy].current);

                                    if (a != 0)
                                    {
                                        visible = true;
                                    }
                                }
                            }
                            if (visible)
                            {
                                writer.Write((byte)x);
                                writer.Write((byte)y);
                                localQuadCount++;
                            }
                        }
                    }
                    if (localQuadCount != 0)
                    {
                        long position = writer.BaseStream.Position;
                        writer.BaseStream.Position = localQuadCountPosition;
                        writer.Write(localQuadCount);
                        writer.BaseStream.Position = position;

                        totalQuadCount += localQuadCount;
                    }
                }
                {
                    long position = writer.BaseStream.Position;
                    writer.BaseStream.Position = totalQuadCountPosition;
                    writer.Write(totalQuadCount);
                    writer.BaseStream.Position = position;
                }
            }

            AssetManager.Instance.Progress(Location + " - 기타정보 구성중", 1);       //5

            List<MapWater> waters = new List<MapWater>();

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    WaterInstance i = waterInstances[x, y];
                    if (i != null && !waters.Contains(i.Water) && WaterEnabled(i, x, y))
                    {
                        waters.Add(i.Water);
                    }
                }
            }
            writer.WriteLength(waters.Count);
            foreach (MapWater water in waters)
            {
                water.Build(writer);
                int quadCount = 0;
                long quadCountPosition = writer.BaseStream.Position;
                writer.Write(0);
                for (int y = 0; y < _Height; y++)
                {
                    for (int x = 0; x < _Width; x++)
                    {
                        WaterInstance i = waterInstances[x, y];
                        if (i != null && i.Water == water && WaterEnabled(i, x, y))
                        {
                            writer.Write((byte)x);
                            writer.Write((byte)y);
                            writer.Write(i.Altitude);
                            quadCount++;
                        }
                    }
                }
                long position = writer.BaseStream.Position;
                writer.BaseStream.Position = quadCountPosition;
                writer.Write(quadCount);
                writer.BaseStream.Position = position;
            }
            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    foreach (MapTile format in _Tiles)
                    {
                        MapTileElement element = tiles[format][x, y];

                        writer.Write(format.Boundary, element != null ? element.Value : 0);
                    }
                }
            }
            _Element.Build(writer);

            Camera.Build(writer);

            _Trigger.Build(writer);
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform, bool upload)
        {
            Load();

            if (writer == null)
            {
                string filePath = path + GetBuildPlatformDirPath(platform) + BuildPath;

                CreateBuildDirectory(filePath);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        BuildImpl(writer, platform);
                    }
                }
            }
            else
            {
                BuildImpl(writer, platform);
            }
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            _Element.GetLocaleStrings(strings);

            _Trigger.GetLocaleStrings(strings);
        }
        */
    }
}
