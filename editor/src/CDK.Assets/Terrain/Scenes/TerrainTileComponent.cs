using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Terrain
{
    public class TerrainTileComponent : SceneObject
    {
        public TerrainAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Tile";
        }

        private string _SelectedElement;
        public string SelectedElement
        {
            set => SetProperty(ref _SelectedElement, value, false);
            get => _SelectedElement;
        }

        private Vector2 _location;

        public TerrainTileComponent(TerrainAsset asset)
        {
            Fixed = true;

            Asset = asset;
        }

        public override SceneComponentType Type => SceneComponentType.TerrainTile;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor) return;

            Asset.DrawTile(graphics, _location);
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Asset.ConvertToMapSpace(Scene.Camera.PickRay(new Vector2(e.X, e.Y)), out var location))
                    {
                        Asset.StartModifyingTile();

                        if (controlKey)
                        {
                            Asset.ModifyTile(null, location, shiftKey);
                        }
                        else if (_SelectedElement != null)
                        {
                            Asset.ModifyTile(_SelectedElement, location, shiftKey);
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
                        if (controlKey)
                        {
                            Asset.ModifyTile(null, location, shiftKey);
                        }
                        else if (_SelectedElement != null)
                        {
                            Asset.ModifyTile(_SelectedElement, location, shiftKey);
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
