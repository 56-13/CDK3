using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Texturing;

using Material = CDK.Assets.Texturing.Material;

namespace CDK.Assets.Configs
{
    public class GroundConfig : AssetElement
    {
        public SceneConfig Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public int Index => Parent.Grounds.IndexOf(this);

        public string Key { private set; get; }

        private string _Name;
        public string Name
        {
            set => SetProperty(ref _Name, GetNewName(value));
            get => _Name;
        }

        private int _Width;
        public int Width
        {
            set
            {
                if (SetProperty(ref _Width, value)) OnPropertyChanged("Space");
            }
            get => _Width;
        }

        private int _Height;
        public int Height
        {
            set
            {
                if (SetProperty(ref _Height, value)) OnPropertyChanged("Space");
            }
            get => _Height;
        }

        private int _Altitude;
        public int Altitude
        {
            set
            {
                if (SetProperty(ref _Altitude, value)) OnPropertyChanged("Space");
            }
            get => _Altitude;
        }

        private int _Grid;
        public int Grid
        {
            set
            {
                if (SetProperty(ref _Grid, value)) OnPropertyChanged("Space");
            }
            get => _Grid;
        }

        private Color3 _GridColor;
        public Color3 GridColor
        {
            set => SetProperty(ref _GridColor, value);
            get => _GridColor;
        }

        public Material Material { private set; get; }

        public ABoundingBox Space
        {
            get
            {
                var w = _Width * _Grid * 0.5f;
                var h = _Height * _Grid * 0.5f;
                var a = _Altitude * _Grid;
                return new ABoundingBox(new Vector3(-w, -h, 0), new Vector3(w, h, a));
            }
        }

        public GroundConfig(SceneConfig parent, XmlNode node)
        {
            if (node.LocalName != "ground") throw new XmlException();

            Parent = parent;

            _Name = node.ReadAttributeString("name");

            Key = node.ReadAttributeString("key", _Name);

            _Width = node.ReadAttributeInt("width");
            _Height = node.ReadAttributeInt("height");
            _Altitude = node.ReadAttributeInt("altitude");
            _Grid = node.ReadAttributeInt("grid");
            _GridColor = node.ReadAttributeColor3("gridColor", true);

            Material = new Material(this, MaterialUsage.Ground);
            using (new AssetCommandHolder())
            {
                Material.Load(node.GetChildNode("material"));
            }
        }

        public GroundConfig(SceneConfig parent, GroundConfig other)
        {
            Parent = parent;

            Key = Parent.Grounds.Any(e => e.Key == other.Key) ? AssetManager.NewKey() : other.Key;
            _Name = GetNewName(other._Name);
            _Width = other._Width;
            _Height = other._Height;
            _Altitude = other._Altitude;
            _Grid = other._Grid;
            _GridColor = other._GridColor;

            Material = new Material(this, other.Material, MaterialUsage.Ground);
        }

        private string GetNewName(string name)
        {
            var rname = name;
            var i = rname.LastIndexOf(' ');
            if (i > 0 && int.TryParse(rname.Substring(i + 1), out _)) rname = rname.Substring(0, i);
            i = 1;
            while (Parent.Grounds.Any(c => c != this && c.Name == rname)) rname = $"{name} {i++}";
            name = rname;
            return name;
        }

        internal override void AddRetains(ICollection<string> retains) => Material.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Material.IsRetaining(element, out from);
        
        internal void Draw(Graphics graphics, float progress, int random, bool gridVisible)
        {
            graphics.Push();

            var w = _Width * _Grid * 0.5f;
            var h = _Height * _Grid * 0.5f;
            graphics.Material = Material.GetMaterial(progress, random);
            graphics.DrawRect(new Rectangle(-w, -h, w * 2, h * 2), true);

            if (gridVisible && _Grid > 1)
            {
                graphics.Material.BlendMode = BlendMode.Alpha;

                var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);

                var centerForeColor = new Color4(_GridColor, 1.0f);
                var nearForeColor = new Color4(_GridColor, 0.75f);

                var c = _Width / 2;
                for (var x = -c; x <= c; x++)
                {
                    command.State.Material.Color = x == 0 ? centerForeColor : nearForeColor;
                    command.AddIndex(command.VertexCount);
                    command.AddIndex(command.VertexCount + 1);
                    command.AddVertex(new FVertex(new Vector3(x * _Grid, -w, 0.1f)));
                    command.AddVertex(new FVertex(new Vector3(x * _Grid, w, 0.1f)));
                }
                c = _Height / 2;
                for (var y = -c; y <= c; y++)
                {
                    command.State.Material.Color = y == 0 ? centerForeColor : nearForeColor;
                    command.AddIndex(command.VertexCount);
                    command.AddIndex(command.VertexCount + 1);
                    command.AddVertex(new FVertex(new Vector3(-w, y * _Grid, 0.1f)));
                    command.AddVertex(new FVertex(new Vector3(w, y * _Grid, 0.1f)));
                }

                graphics.Command(command);
            }

            graphics.Pop();
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("ground");
            writer.WriteAttribute("key", Key, _Name);
            writer.WriteAttribute("name", _Name);
            writer.WriteAttribute("width", _Width);
            writer.WriteAttribute("height", _Height);
            writer.WriteAttribute("altitude", _Altitude);
            writer.WriteAttribute("grid", _Grid);
            writer.WriteAttribute("gridColor", _GridColor, true);
            Material.Save(writer);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(_Width);
            writer.Write(_Height);
            writer.Write(_Altitude);
            writer.Write(_Grid);
            writer.Write(_GridColor, true);
            Material.Build(writer);
        }
    }
}
