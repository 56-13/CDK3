using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Terrain
{
    public class TerrainWaterComponent : SceneObject
    {
        public TerrainAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Water";
        }

        private float _Altitude;
        public float Altitude
        {
            set => SetProperty(ref _Altitude, value, false);
            get => _Altitude;
        }

        private TerrainWater _Selection;
        public TerrainWater Selection
        {
            set => SetProperty(ref _Selection, value, false);
            get => _Selection;
        }

        private Vector2 _location;

        public TerrainWaterComponent(TerrainAsset asset)
        {
            Fixed = true;

            Asset = asset;

            _Altitude = 1;
        }

        public override SceneComponentType Type => SceneComponentType.TerrainWater;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor) return;
            
            var command = new StreamRenderCommand(graphics, PrimitiveMode.Triangles);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;
            command.State.Material.DepthTest = false;

            var x = (int)_location.X;
            var y = (int)_location.Y;

            command.AddVertex(new FVertex(
                        new Vector3(x, y, _Altitude) * Asset.Grid,
                        new Color4(1f, 1f, 1f, 0.5f)));
            command.AddVertex(new FVertex(
                        new Vector3(x + 1, y, _Altitude) * Asset.Grid,
                        new Color4(1f, 1f, 1f, 0.5f)));
            command.AddVertex(new FVertex(
                        new Vector3(x, y + 1, _Altitude) * Asset.Grid,
                        new Color4(1f, 1f, 1f, 0.5f)));
            command.AddVertex(new FVertex(
                        new Vector3(x + 1, y + 1, _Altitude) * Asset.Grid,
                        new Color4(1f, 1f, 1f, 0.5f)));

            command.AddIndex(0);
            command.AddIndex(1);
            command.AddIndex(2);
            command.AddIndex(1);
            command.AddIndex(3);
            command.AddIndex(2);

            graphics.Command(command);
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    Altitude = Math.Min(_Altitude + 0.5f, 8);
                    return true;
                case Keys.S:
                    Altitude = Math.Max(_Altitude - 0.5f, 0.01f);
                    return true;
            }
            return false;
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Asset.ConvertToMapSpace(Scene.Camera.PickRay(new Vector2(e.X, e.Y)), out var origin))
                    {
                        Asset.StartModifyingWater();

                        if (controlKey || _Selection != null) Asset.ModifyWater(controlKey ? null : _Selection, origin, Altitude, shiftKey);

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
                    Asset.EndModifyingWater();
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
                        if (controlKey || _Selection != null) Asset.ModifyWater(controlKey ? null : _Selection, location, Altitude, false);
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
