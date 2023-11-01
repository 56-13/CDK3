using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Terrain
{
    public class TerrainSurfaceComponent : SceneObject
    {
        public TerrainAsset Asset { private set; get; }
        
        public override string Name
        {
            set {}
            get => "Surface";
        }

        private TerrainModifyMode _Mode;
        public TerrainModifyMode Mode
        {
            set
            {
                if (SetProperty(ref _Mode, value, false)) OnPropertyChanged("Degree");
            }
            get => _Mode;
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

        private float _Attenuation;
        public float Attenuation
        {
            set => SetProperty(ref _Attenuation, value, false);
            get => _Attenuation;
        }

        private float _Shadow;
        public float Shadow
        {
            set => SetProperty(ref _Shadow, value, false);
            get => _Shadow;
        }

        private float _ShadowAttenuation;
        public float ShadowAttenuation
        {
            set => SetProperty(ref _ShadowAttenuation, value, false);
            get => _ShadowAttenuation;
        }

        private TerrainSurface _Selection;
        public TerrainSurface Selection
        {
            set => SetProperty(ref _Selection, value, false);
            get => _Selection;
        }

        private TerrainSurface _Origin;
        public TerrainSurface Origin
        {
            set => SetProperty(ref _Origin, value, false);
            get => _Origin;
        }

        private Vector2 _location;

        public TerrainSurfaceComponent(TerrainAsset asset)
        {
            Fixed = true;

            Asset = asset;

            _Size = 2;
            _AddDegree = 1;
            _EqualDegree = 1;
            _RubDegree = 1;
            _Attenuation = 0.5f;
        }

        public override SceneComponentType Type => SceneComponentType.TerrainSurface;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor) return;

            var supdateSize = (_Shadow != 0 && _Size < _ShadowAttenuation ? _ShadowAttenuation : _Size) * Asset.VertexCell * Asset.SurfaceCell;

            var swidth = Asset.Width * Asset.VertexCell * Asset.SurfaceCell;
            var sheight = Asset.Height * Asset.VertexCell * Asset.SurfaceCell;
            var sorigin = _location * Asset.VertexCell * Asset.SurfaceCell;
            var ssize = _Size * Asset.VertexCell * Asset.SurfaceCell;

            var sminx = (int)(sorigin.X - supdateSize);
            var sminy = (int)(sorigin.Y - supdateSize);
            var smaxx = (int)(sorigin.X + supdateSize);
            var smaxy = (int)(sorigin.Y + supdateSize);
            if (sminx > swidth || smaxx < 0 || sminy > sheight || smaxy < 0)
            {
                return;
            }
            if (sminx < 0) sminx = 0;
            if (sminy < 0) sminy = 0;
            if (smaxx > swidth) smaxx = swidth;
            if (smaxy > sheight) smaxy = sheight;

            var sshadowAttenuation = _Shadow != 0 ? _ShadowAttenuation * Asset.VertexCell * Asset.SurfaceCell : 0;
            ssize = Math.Max(ssize, sshadowAttenuation + 1);

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;
            command.State.Material.DepthTest = false;

            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    var d = (sorigin - new Vector2(sx, sy)).Length();

                    var a = TerrainAsset.AttenuateRate(d, ssize, _Attenuation);

                    var l = d >= ssize ? 0 : (d <= ssize - sshadowAttenuation ? 1 : MathUtil.SmoothStep((ssize - d) / sshadowAttenuation));

                    l = 1 - (1 - l) * _Shadow;

                    var x = (float)sx / (Asset.VertexCell * Asset.SurfaceCell);
                    var y = (float)sy / (Asset.VertexCell * Asset.SurfaceCell);

                    command.AddVertex(new FVertex(
                        new Vector3(x * Asset.Grid, y * Asset.Grid, Asset.GetAltitude(new Vector2(x, y)) * Asset.Grid),
                        new Color4(l, l, l, a)));
                }
            }

            var vi = 0;

            for (var sy = sminy; sy < smaxy; sy++)
            {
                for (var sx = sminx; sx < smaxx; sx++)
                {
                    command.AddIndex(vi);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi);
                    command.AddIndex(vi + (smaxx - sminx + 1));
                    vi++;
                }
                vi++;
            }
            graphics.Command(command);
        }

        internal override bool KeyDown(KeyEventArgs e)
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
                    Degree = Math.Min(Degree + 0.05f, 1f);
                    return true;
                case Keys.F:
                    Degree = Math.Max(Degree - 0.05f, 0.01f);
                    return true;
                case Keys.G:
                    Attenuation = Math.Min(_Attenuation + 0.1f, 1.0f);
                    return true;
                case Keys.H:
                    Attenuation = Math.Max(_Attenuation - 0.1f, 0.0f);
                    return true;

            }
            return false;
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Asset.ConvertToMapSpace(Scene.Camera.PickRay(new Vector2(e.X, e.Y)), out var location))
                    {
                        Asset.StartModifyingSurface();

                        if (shiftKey)
                        {
                            Asset.RemoveSurface(location, _Mode, _Size, _Attenuation, Degree);
                        }
                        else if (_Selection != null)
                        {
                            Asset.ModifySurface(_Origin, _Selection, location, _Mode, !controlKey, _Size, _Attenuation, Degree, _Shadow, _ShadowAttenuation);
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
                    Asset.EndModifyingSurface();
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
                        if (shiftKey)
                        {
                            Asset.RemoveSurface(location, _Mode, _Size, _Attenuation, Degree);
                        }
                        else if (_Selection != null)
                        {
                            Asset.ModifySurface(_Origin, _Selection, location, _Mode, !controlKey, _Size, _Attenuation, Degree, _Shadow, _ShadowAttenuation);
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

        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
