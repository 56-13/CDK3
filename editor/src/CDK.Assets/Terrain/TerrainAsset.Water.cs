using System.Numerics;
using System.Collections.Generic;
using System.Drawing;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        internal TerrainWaterInstance GetWaterInstance(int x, int y)
        {
            return _waterInstances[x, y];
        }

        private class ModifyWaterCommand : IAssetCommand
        {
            private struct Pair
            {
                public TerrainWaterInstance prev;
                public TerrainWaterInstance next;
            }
            private TerrainAsset _asset;
            private Dictionary<Point, Pair> _pairs;
            private int _minx;
            private int _miny;
            private int _maxx;
            private int _maxy;

            public Asset Asset => _asset;

            public ModifyWaterCommand(TerrainAsset asset)
            {
                _asset = asset;

                _pairs = new Dictionary<Point, Pair>(asset._modifyingWaterInstances.Count);
                _minx = asset._Width;
                _miny = asset._Height;
                _maxx = 0;
                _maxy = 0;

                foreach (Point point in asset._modifyingWaterInstances.Keys)
                {
                    var prev = asset._modifyingWaterInstances[point];
                    var next = asset._waterInstances[point.X, point.Y];

                    if (prev != next)
                    {
                        if (_minx > point.X) _minx = point.X;
                        if (_maxx <= point.X) _maxx = point.X + 1;
                        if (_miny > point.Y) _miny = point.Y;
                        if (_maxy <= point.Y) _maxy = point.Y + 1;

                        Pair pair = new Pair
                        {
                            prev = prev,
                            next = next
                        };
                        _pairs.Add(point, pair);
                    }
                }
            }

            public void Undo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (var point in _pairs.Keys)
                    {
                        _asset._waterInstances[point.X, point.Y] = _pairs[point].prev;
                    }
                    _asset._Display?.UpdateWater(_minx, _miny, _maxx, _maxy);
                }
            }

            public void Redo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (var point in _pairs.Keys)
                    {
                        _asset._waterInstances[point.X, point.Y] = _pairs[point].next;
                    }
                    _asset._Display?.UpdateWater(_minx, _miny, _maxx, _maxy);
                }
            }

            public bool Merge(IAssetCommand other) => false;
        }

        
        internal bool WaterEnabled(TerrainWaterInstance i, int x, int y)
        {
            var z = i.Altitude + i.Water.WaveAltitude / _Grid;

            for (var vy = 0; vy <= _VertexCell; vy++)
            {
                for (var vx = 0; vx <= _VertexCell; vx++)
                {
                    if (_altitudes[x * _VertexCell + vx, y * _VertexCell + vy] < z) return true;
                }
            }
            return false;
        }

        public void StartModifyingWater()
        {
            Load();

            _modifyingWaterInstances.Clear();
        }

        private void ModifyWater(Point p)
        {
            if (!_modifyingWaterInstances.ContainsKey(p))
            {
                _modifyingWaterInstances.Add(p, _waterInstances[p.X, p.Y]);
            }
        }

        public void EndModifyingWater()
        {
            if (_modifyingWaterInstances.Count != 0)
            {
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new ModifyWaterCommand(this));
                }
                _modifyingWaterInstances.Clear();
            }
        }

        public void ModifyWater(TerrainWater water, in Vector2 origin, float altitude, bool fill)
        {
            Load();

            if (water == null || water.Parent == this)
            {
                var p = new Point((int)origin.X, (int)origin.Y);

                if (p.X >= 0 && p.X < _Width && p.Y >= 0 && p.Y < _Height)
                {
                    var instance = water != null ? new TerrainWaterInstance(water, altitude) : null;

                    var src = _waterInstances[p.X, p.Y];

                    if (!Equals(src, instance))
                    {
                        var minx = p.X;
                        var maxx = p.X + 1;
                        var miny = p.Y;
                        var maxy = p.Y + 1;

                        ModifyWater(p);

                        _waterInstances[p.X, p.Y] = instance;

                        if (fill)
                        {
                            Point[] nearPoints = new Point[] {
                                new Point(-1, 0),
                                new Point(0, -1),
                                new Point(1, 0),
                                new Point(0, 1)
                            };

                            List<Point> points = new List<Point>();
                            points.Add(p);

                            do
                            {
                                p = points[0];
                                points.RemoveAt(0);

                                for (var i = 0; i < 4; i++)
                                {
                                    var np = new Point(p.X + nearPoints[i].X, p.Y + nearPoints[i].Y);

                                    if (np.X >= 0 && np.X < _Width && np.Y >= 0 && np.Y < _Height && Equals(_waterInstances[np.X, np.Y], src) && (instance == null || WaterEnabled(instance, np.X, np.Y)))
                                    {
                                        points.Add(np);

                                        ModifyWater(np);

                                        _waterInstances[np.X, np.Y] = instance;

                                        if (minx > np.X) minx = np.X;
                                        if (maxx <= np.X) maxx = np.X + 1;
                                        if (miny > np.Y) miny = np.Y;
                                        if (maxy <= np.Y) maxy = np.Y + 1;
                                    }
                                }
                            }
                            while (points.Count != 0);
                        }

                        IsDirty = true;

                        _Display?.UpdateWater(minx, miny, maxx, maxy);
                    }
                }
            }
        }
        
        public void ClearWater(TerrainWater water)
        {
            Load();

            var minx = _Width;
            var maxx = _Height;
            var miny = 0;
            var maxy = 0;
            var updated = false;

            for (var y = 0; y < _Height; y++)
            {
                for (var x = 0; x < _Width; x++)
                {
                    if (_waterInstances[x, y] != null && _waterInstances[x, y].Water == water)
                    {
                        if (!updated)
                        {
                            StartModifyingWater();

                            updated = true;
                        }

                        ModifyWater(new Point(x, y));

                        _waterInstances[x, y] = null;

                        if (minx > x) minx = x;
                        if (maxx <= x) maxx = x + 1;
                        if (miny > y) miny = y;
                        if (maxy <= y) maxy = y + 1;
                    }
                }
            }
            if (updated)
            {
                EndModifyingWater();

                IsDirty = true;

                _Display?.UpdateWater(minx, miny, maxx, maxy);
            }
        }
    }
}
