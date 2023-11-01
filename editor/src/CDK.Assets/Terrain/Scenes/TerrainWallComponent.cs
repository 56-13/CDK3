using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Terrain
{
    public class TerrainWallComponent : SceneObject
    {
        public TerrainAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => "Wall";
        }

        private TerrainWall _SelectedSource;
        public TerrainWall SelectedSource
        {
            set => SetProperty(ref _SelectedSource, value, false);
            get => _SelectedSource;
        }

        private TerrainWallInstance _SelectedInstance;
        public TerrainWallInstance SelectedInstance
        {
            set
            {
                var prev = _SelectedInstance;
                if (SetProperty(ref _SelectedInstance, value, false))
                {
                    if (prev != null) prev.Points.RemoveWeakBeforeListChanged(WallInstancePoints_BeforeListChanged);
                    if (_SelectedInstance != null)
                    {
                        _SelectedInstance.Points.AddWeakBeforeListChanged(WallInstancePoints_BeforeListChanged);
                        SelectedSource = _SelectedInstance.Wall;
                    }
                    if (_SelectedInstancePoint != null && _SelectedInstancePoint.Parent != _SelectedInstance) SelectedInstancePoint = null;
                }
            }
            get => _SelectedInstance;
        }

        private TerrainWallInstancePoint _SelectedInstancePoint;
        public TerrainWallInstancePoint SelectedInstancePoint
        {
            set
            {
                if (SetProperty(ref _SelectedInstancePoint, value, false))
                {
                    if (_SelectedInstancePoint != null) SelectedInstance = _SelectedInstancePoint.Parent;
                }
            }
            get => _SelectedInstancePoint;
        }

        private bool _dragging;
        private int _dragX;
        private int _dragY;

        public TerrainWallComponent(TerrainAsset asset)
        {
            Fixed = true;

            Asset = asset;

            Asset.Walls.BeforeListChanged += Walls_BeforeListChanged;

            Asset.Walls.AddWeakBeforeListChanged(Walls_BeforeListChanged);
            Asset.WallInstances.AddWeakBeforeListChanged(WallInstances_BeforeListChanged);
        }

        private void Walls_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainWall> e)
        {
            switch(e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (_SelectedSource == Asset.Walls[e.NewIndex]) SelectedSource = null;
                    break;
                case ListChangedType.Reset:
                    SelectedSource = null;
                    break;
            }
        }

        private void WallInstances_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainWallInstance> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (_SelectedInstance == Asset.WallInstances[e.NewIndex]) SelectedInstance = null;
                    break;
                case ListChangedType.Reset:
                    SelectedInstance = null;
                    break;
            }
        }

        private void WallInstancePoints_BeforeListChanged(object sender, BeforeListChangedEventArgs<TerrainWallInstancePoint> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (_SelectedInstancePoint == _SelectedInstance.Points[e.NewIndex]) SelectedInstancePoint = null;
                    break;
                case ListChangedType.Reset:
                    SelectedInstancePoint = null;
                    break;
            }
        }

        private bool Intersects(Ray ray, TerrainWallInstancePoint p)
        {
            return Intersects(ray, p.Z, out var x, out var y) && x == p.X && y == p.Y;
        }

        private bool Intersects(Ray ray, float z, out int x, out int y)
        {
            if (ray.Intersects(new Plane(0, 0, 1, -z), out var d))
            {
                var p = ray.Position + ray.Direction * d;
                x = (int)(p.X / Asset.Grid);
                y = (int)(p.Y / Asset.Grid);
                if (x >= 0 && x < Asset.Width && y >= 0 && y < Asset.Height)
                {
                    return true;
                }
            }
            x = -1;
            y = -1;
            return false;
        }

        private (int x, int y)[] GetInterPoints()
        {
            if (_SelectedInstancePoint == null) return null;

            var x = _SelectedInstancePoint.X;
            var y = _SelectedInstancePoint.Y;

            if (_SelectedInstance.Points.Count > 1)
            {
                TerrainWallInstancePoint pp;

                if (_SelectedInstancePoint == _SelectedInstance.Points.Last()) pp = _SelectedInstance.Points[_SelectedInstance.Points.Count - 2];
                else if (_SelectedInstancePoint == _SelectedInstance.Points.First()) pp = _SelectedInstance.Points[1];
                else return null;

                if (x > pp.X)
                {
                    if (_dragX < x) return null;
                }
                else if (x < pp.X)
                {
                    if (_dragX > x) return null;
                }
                if (y > pp.Y)
                {
                    if (_dragY < y) return null;
                }
                else if (y < pp.Y)
                {
                    if (_dragY > y) return null;
                }
            }

            var d = Math.Max(Math.Abs(x - _dragX), Math.Abs(y - _dragY));

            if (d == 0) return null;

            var result = new (int x, int y)[d];

            for (var i = 0; i < d; i++)
            {
                if (x < _dragX) x++;
                else if (x > _dragX) x--;

                if (y < _dragY) y++;
                else if (y > _dragY) y--;

                foreach (var wi in Asset.WallInstances)
                {
                    if (wi.Points.Any(p => p.X == x && p.Y == y)) return null;
                }

                result[i] = (x, y);
            }

            return result;
        }

        public void RemoveInstancePoint()
        {
            if (_SelectedInstancePoint != null && _SelectedInstance.Points.Count > 1)
            {
                if (_SelectedInstancePoint == _SelectedInstance.Points.Last())
                {
                    _SelectedInstance.Points.RemoveAt(_SelectedInstance.Points.Count - 1);
                    SelectedInstancePoint = _SelectedInstance.Points.Last();
                    _SelectedInstance.Update();
                }
                else if (_SelectedInstancePoint == _SelectedInstance.Points.First())
                {
                    _SelectedInstance.Points.RemoveAt(0);
                    SelectedInstancePoint = _SelectedInstance.Points.First();
                    _SelectedInstance.Update();
                }
            }
        }

        public override SceneComponentType Type => SceneComponentType.TerrainWall;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == InstanceLayer.Cursor)
            {
                foreach (var invalid in Asset.WallInstances.Where(i => i != _SelectedInstance && (i.Wall.Bones.Count < 2 || i.Wall.Selection.Geometry == null)))
                {
                    invalid.DrawCursor(graphics, null, Color4.LightRed, Color4.Red, Color4.Red, null);
                }

                if (_SelectedInstance != null)
                {
                    var inter = _dragging ? GetInterPoints() : null;

                    _SelectedInstance.DrawCursor(graphics, _SelectedInstancePoint, Color4.White, Color4.LightGreen, Color4.DarkGreen, inter);
                }
            }
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _SelectedInstance?.MoveZ(1);
                    return true;
                case Keys.S:
                    _SelectedInstance?.MoveZ(-1);
                    return true;
                case Keys.Delete:
                    if (_SelectedInstance != null) Asset.WallInstances.Remove(_SelectedInstance);
                    return true;
                case Keys.Back:
                    RemoveInstancePoint();
                    return true;
            }
            return false;
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left)
            {
                var ray = Scene.Camera.PickRay(new Vector2(e.X, e.Y));

                if (e.Clicks > 1)
                {
                    if (_SelectedSource != null && Asset.Intersects(ray, CollisionFlags.None, out var d, out _))          //TODO:프랍이 포함된 높이일 것이다.
                    {
                        var origin = ray.Position + ray.Direction * d;
                        var x = (int)(origin.X / Asset.Grid);
                        var y = (int)(origin.Y / Asset.Grid);

                        if (x >= 0 && x < Asset.Width && y >= 0 && y < Asset.Height)
                        {
                            foreach (var wallInstance in Asset.WallInstances)
                            {
                                if (wallInstance.Points.Any(p => p.X == x && p.Y == y)) return false;
                            }
                            var newWallInstance = new TerrainWallInstance(_SelectedSource);
                            newWallInstance.Points.Add(new TerrainWallInstancePoint(newWallInstance, x, y, origin.Z));
                            Asset.WallInstances.Add(newWallInstance);
                            newWallInstance.Update();
                            SelectedInstance = newWallInstance;
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (var wallInstance in Asset.WallInstances)
                    {
                        foreach (var p in wallInstance.Points)
                        {
                            if (Intersects(ray, p))
                            {
                                SelectedInstancePoint = p;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_dragging)
                {
                    var inter = GetInterPoints();

                    if (inter != null)
                    {
                        if (_SelectedInstancePoint == _SelectedInstance.Points.Last())
                        {
                            foreach (var p in inter)
                            {
                                _SelectedInstance.Points.Add(new TerrainWallInstancePoint(_SelectedInstance, p.x, p.y, _SelectedInstancePoint.Z));
                            }
                            _SelectedInstance.Update();
                            SelectedInstancePoint = _SelectedInstance.Points.Last();
                        }
                        else if (_SelectedInstancePoint == _SelectedInstance.Points.First())
                        {
                            foreach (var p in inter)
                            {
                                _SelectedInstance.Points.Insert(0, new TerrainWallInstancePoint(_SelectedInstance, p.x, p.y, _SelectedInstancePoint.Z));
                            }
                            _SelectedInstance.Update();
                            SelectedInstancePoint = _SelectedInstance.Points.First();
                        }
                    }
                    _dragging = false;
                    return true;
                }
            }
            return false;
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_SelectedInstancePoint != null && (_SelectedInstancePoint == _SelectedInstance.Points.First() || _SelectedInstancePoint == _SelectedInstance.Points.Last()))
                {
                    var ray = Scene.Camera.PickRay(new Vector2(e.X, e.Y));

                    if (Intersects(ray, _SelectedInstancePoint.Z, out var x, out var y))
                    {
                        _dragX = x;
                        _dragY = y;
                        _dragging = true;
                    }
                    else
                    {
                        _dragging = false;
                    }
                    return true;
                }
            }
            return false;
        }

        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}
