using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Terrain
{
    public class TerrainAltitudeComponent : SceneObject
    {
        public TerrainAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Altitude";
        }

        private TerrainModifyMode _Mode;
        public TerrainModifyMode Mode
        {
            set
            {
                if (SetProperty(ref _Mode, value, false))
                {
                    OnPropertyChanged("DegreeMax");
                    OnPropertyChanged("DegreeMin");
                    OnPropertyChanged("DegreeIncrement");
                    OnPropertyChanged("Degree");
                }
            }
            get => _Mode;
        }

        private bool _Slope;
        public bool Slope
        {
            set
            {
                if (SetProperty(ref _Slope, value, false)) _slopeStartLocation = null;
            }
            get => _Slope;
        }

        private float _Size;
        public float Size
        {
            set => SetProperty(ref _Size, value, false);
            get => _Size;
        }

        private float _AddDegree;
        public float AddDegree
        {
            set
            {
                if (SetProperty(ref _AddDegree, value, false)) OnPropertyChanged("Degree");
            }
            get => _AddDegree;
        }

        private float _EqualDegree;
        public float EqualDegree
        {
            set
            {
                if (SetProperty(ref _EqualDegree, value, false)) OnPropertyChanged("Degree");
            }
            get => _EqualDegree;
        }

        private float _RubDegree;
        public float RubDegree
        {
            set
            {
                if (SetProperty(ref _RubDegree, value, false)) OnPropertyChanged("Degree");
            }
            get => _RubDegree;
        }

        public float Degree
        {
            set
            {
                switch (_Mode)
                {
                    case TerrainModifyMode.Add:
                        AddDegree = value;
                        break;
                    case TerrainModifyMode.Equal:
                        EqualDegree = value;
                        break;
                    case TerrainModifyMode.Rub:
                        RubDegree = value;
                        break;
                }
            }
            get
            {
                switch (_Mode)
                {
                    case TerrainModifyMode.Add:
                        return _AddDegree;
                    case TerrainModifyMode.Equal:
                        return _EqualDegree;
                    case TerrainModifyMode.Rub:
                        return _RubDegree;
                }
                return 0;
            }
        }

        public float DegreeMax
        {
            get
            {
                switch (_Mode)
                {
                    case TerrainModifyMode.Equal:
                        return 10;
                }
                return 1;
            }
        }

        public float DegreeMin
        {
            get
            {
                switch (_Mode)
                {
                    case TerrainModifyMode.Equal:
                        return 0;
                }
                return 0.01f;
            }
        }

        public float DegreeIncrement
        {
            get
            {
                switch (_Mode)
                {
                    case TerrainModifyMode.Equal:
                        return 0.1f;
                }
                return 0.01f;
            }
        }

        private float _Attenuation;
        public float Attenuation
        {
            set => SetProperty(ref _Attenuation, value, false);
            get => _Attenuation;
        }

        private float _AllDegree;
        public float AllDegree
        {
            set => SetProperty(ref _AllDegree, value, false);
            get => _AllDegree;
        }

        private Vector2 _location;
        private Vector2? _slopeStartLocation;
        private float _slopeThickness;

        public TerrainAltitudeComponent(TerrainAsset asset)
        {
            Fixed = true;

            Asset = asset;

            _Size = 4;
            _AddDegree = 0.05f;
            _RubDegree = 0.05f;
            _Attenuation = 0.5f;
        }

        public override SceneComponentType Type => SceneComponentType.TerrainAltitude;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }

        private void DrawModifyCursor(Graphics graphics)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;
            var vorigin = _location * Asset.VertexCell;
            var vsize = _Size * Asset.VertexCell;
            var vminx = (int)(vorigin.X - vsize);
            var vminy = (int)(vorigin.Y - vsize);
            var vmaxx = (int)(vorigin.X + vsize);
            var vmaxy = (int)(vorigin.Y + vsize);
            if (vminx > vwidth || vmaxx < 0 || vminy > vheight || vmaxy < 0)
            {
                return;
            }
            if (vminx < 0) vminx = 0;
            if (vminy < 0) vminy = 0;
            if (vmaxx >= vwidth) vmaxx = vwidth;
            if (vmaxy >= vheight) vmaxy = vheight;

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;
            command.State.Material.DepthTest = false;

            for (var vy = vminy; vy <= vmaxy; vy++)
            {
                for (var vx = vminx; vx <= vmaxx; vx++)
                {
                    var d = (vorigin - new Vector2(vx, vy)).Length();

                    var r = TerrainAsset.AttenuateRate(d, vsize, _Attenuation);

                    command.AddVertex(new FVertex(
                        new Vector3((float)vx * Asset.Grid / Asset.VertexCell, (float)vy * Asset.Grid / Asset.VertexCell, Asset.GetAltitude(vx, vy) * Asset.Grid),
                        new Color4(1f, 1f, 1f, r)));
                }
            }

            var vi = 0;

            for (var vy = vminy; vy < vmaxy; vy++)
            {
                for (var vx = vminx; vx < vmaxx; vx++)
                {
                    command.AddIndex(vi);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi);
                    command.AddIndex(vi + (vmaxx - vminx + 1));
                    vi++;
                }
                vi++;
            }

            graphics.Command(command);
        }

        private void DrawSlopeCursor(Graphics graphics)
        {
            if (_slopeStartLocation == null) return;

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Triangles);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;
            command.State.Material.DepthTest = false;
            command.State.Material.Color.A *= 0.25f;

            var p0 = _slopeStartLocation.Value;
            var p1 = _location;

            var h0 = Asset.GetAltitude(p0);
            var h1 = Asset.GetAltitude(p1);

            var np = Vector2.Normalize(p1 - p0) * _slopeThickness;

            command.AddVertex(new FVertex(new Vector3(p0.X - np.Y, p0.Y + np.X, h0) * Asset.Grid));
            command.AddVertex(new FVertex(new Vector3(p0.X + np.Y, p0.Y - np.X, h0) * Asset.Grid));
            command.AddVertex(new FVertex(new Vector3(p1.X - np.Y, p1.Y + np.X, h1) * Asset.Grid));
            command.AddVertex(new FVertex(new Vector3(p1.X + np.Y, p1.Y - np.X, h1) * Asset.Grid));
            command.AddIndex(0);
            command.AddIndex(1);
            command.AddIndex(2);
            command.AddIndex(1);
            command.AddIndex(3);
            command.AddIndex(2);

            graphics.Command(command);

            var wp0 = new Vector3(p0.X, p0.Y, h0) * Asset.Grid;
            var wp1 = new Vector3(p1.X, p1.Y, h1) * Asset.Grid;
            graphics.DrawLine(wp0, wp1);
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor) return;

            if (_Slope) DrawSlopeCursor(graphics);
            else DrawModifyCursor(graphics);
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            if (_Slope)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        _slopeStartLocation = null;
                        return true;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Q:
                        Mode = TerrainModifyMode.Add;
                        return true;
                    case Keys.W:
                        Mode = TerrainModifyMode.Equal;
                        return true;
                    case Keys.E:
                        Mode = TerrainModifyMode.Rub;
                        return true;
                    case Keys.A:
                        Size = Math.Min(_Size + 0.5f, 8);
                        return true;
                    case Keys.S:
                        Size = Math.Max(_Size - 0.5f, 0.5f);
                        return true;
                    case Keys.D:
                        Degree = Math.Min(Degree + DegreeIncrement * 5, DegreeMax);
                        return true;
                    case Keys.F:
                        Degree = Math.Max(Degree - DegreeIncrement * 5, DegreeMin);
                        return true;
                    case Keys.G:
                        Attenuation = Math.Min(_Attenuation + 0.1f, 1.0f);
                        return true;
                    case Keys.H:
                        Attenuation = Math.Max(_Attenuation - 0.1f, 0.0f);
                        return true;
                }
            }
            return false;
        }
        
        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    if (Asset.ConvertToMapSpace(Scene.Camera.PickRay(new Vector2(e.X, e.Y)), out var location))
                    {
                        Asset.StartModifyingAltitude();

                        if (_Slope)
                        {
                            _slopeStartLocation = location;
                            _location = location;
                        }
                        else if (shiftKey)
                        {
                            if (_Mode == TerrainModifyMode.Equal)
                            {
                                EqualDegree = Asset.GetAltitude(location);
                            }
                        }
                        else
                        {
                            Asset.ModifyAltitude(location, _Mode, !controlKey, _Size, _Attenuation, Degree);
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_Slope && _slopeStartLocation != null)
                    {
                        Asset.SlopeAltitude(_slopeStartLocation.Value, _location, _slopeThickness);

                        _slopeStartLocation = null;
                    }
                    Asset.EndModifyingAltitude();
                    return true;
            }
            return false;
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (Asset.ConvertToMapSpace(Scene.Camera.PickRay(new Vector2(e.X, e.Y)), out var location))
            {
                _location = location;

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (!_Slope)
                        {
                            if (shiftKey)
                            {
                                if (_Mode == TerrainModifyMode.Equal)
                                {
                                    EqualDegree = Asset.GetAltitude(location);
                                }
                            }
                            else
                            {
                                Asset.ModifyAltitude(location, _Mode, !controlKey, _Size, _Attenuation, Degree);
                            }
                        }
                        return true;
                    case MouseButtons.Right:
                        break;
                    default:
                        return true;
                }
            }
            return false;
        }

        internal override bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (_Slope && _slopeStartLocation != null) 
            {
                _slopeThickness = MathUtil.Clamp(_slopeThickness + e.Delta * 0.001f, 0, 10);
                return true;
            }
            return false;
        }

        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
