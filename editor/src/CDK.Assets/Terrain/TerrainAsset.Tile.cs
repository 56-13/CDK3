using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Drawing;

using CDK.Drawing;

using Graphics = CDK.Drawing.Graphics;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        private class ModifyTileCommand : IAssetCommand
        {
            private struct Pair
            {
                public string prev;
                public string next;
            }
            private TerrainAsset _asset;
            private Dictionary<Point, Pair> _pairs;

            public Asset Asset => _asset;

            public ModifyTileCommand(TerrainAsset asset)
            {
                _asset = asset;
                _pairs = new Dictionary<Point, Pair>();

                foreach (var point in asset._modifyingTiles.Keys)
                {
                    var prev = asset._modifyingTiles[point];
                    var next = asset._tiles[point.X, point.Y];

                    if (prev != next)
                    {
                        var pair = new Pair
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
                foreach (var point in _pairs.Keys)
                {
                    _asset._tiles[point.X, point.Y] = _pairs[point].prev;
                }
            }

            public void Redo()
            {
                foreach (var point in _pairs.Keys)
                {
                    _asset._tiles[point.X, point.Y] = _pairs[point].next;
                }
            }

            public bool Merge(IAssetCommand command) => false;
        }

        public void StartModifyingTile()
        {
            Load();

            _modifyingTiles.Clear();
        }

        private void ModifyTile(in Point p)
        {
            Load();

            if (!_modifyingTiles.ContainsKey(p)) _modifyingTiles.Add(p, _tiles[p.X, p.Y]);
        }

        public void EndModifyingTile()
        {
            Load();

            if (AssetManager.Instance.CommandEnabled && _modifyingTiles.Count != 0)
            {
                AssetManager.Instance.Command(new ModifyTileCommand(this));
            }
            _modifyingTiles.Clear();
        }

        public void ModifyTile(string dest, in Vector2 origin, bool fill)
        {
            Load();

            var x = (int)origin.X;
            var y = (int)origin.Y;

            if (x >= 0 && x < _Width && y >= 0 && y < _Height)
            {
                var p = new Point(x, y);

                if (fill)
                {
                    var nearPoints = new Point[]{
                        new Point(-1, 0),
                        new Point(0, -1),
                        new Point(1, 0),
                        new Point(0, 1)
                    };

                    var src = _tiles[x, y];
                    if (src != dest)
                    {
                        var points = new List<Point>
                        {
                            p
                        };
                        ModifyTile(p);

                        _tiles[x, y] = dest;

                        do
                        {
                            p = points[0];
                            points.RemoveAt(0);

                            for (var i = 0; i < 4; i++)
                            {
                                var np = new Point(p.X + nearPoints[i].X, p.Y + nearPoints[i].Y);

                                if (np.X >= 0 && np.X < _Width && np.Y >= 0 && np.Y < _Height && _tiles[np.X, np.Y] == src)
                                {
                                    points.Add(np);
                                    ModifyTile(np);
                                _tiles[np.X, np.Y] = dest;
                                }
                            }
                        }
                        while (points.Count != 0);
                    }
                }
                else
                {
                    ModifyTile(p);
                    _tiles[x, y] = dest;
                }
                IsDirty = true;
            }
        }

        public string GetTile(int x, int y)
        {
            return x >= 0 && x < _Width && y >= 0 && y < _Height ? _tiles[x, y] : null;
        }

        private static readonly Point[] quadPointOffsets = new Point[4] {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };

        public void DrawTile(Graphics graphics, Vector2? origin)
        {
            Load();

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Triangles);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;
            command.State.Material.DepthTest = false;
            command.Color.A *= 0.5f;

            var vi = 0;

            var tileConstants = Project.GetTerrainTileConstants();

            for (var y = 0; y < _Height; y++)
            {
                for (var x = 0; x < _Width; x++)
                {
                    if (origin != null && x == (int)origin.Value.X && y == (int)origin.Value.Y) command.State.Material.Color = Color4.White;
                    else if (_tiles[x, y] != null) command.State.Material.Color = tileConstants.First(c => c.Name == _tiles[x, y]).Color;
                    else continue;

                    for (var i = 0; i < 4; i++)
                    {
                        var p = new Point(x + quadPointOffsets[i].X, y + quadPointOffsets[i].Y);

                        command.AddVertex(new FVertex(new Vector3(p.X, p.Y, _altitudes[p.X * _VertexCell, p.Y * _VertexCell]) * _Grid));
                    }

                    if (IsZQuad(x, y))
                    {
                        command.AddIndex(vi + 0);
                        command.AddIndex(vi + 1);
                        command.AddIndex(vi + 2);
                        command.AddIndex(vi + 1);
                        command.AddIndex(vi + 3);
                        command.AddIndex(vi + 2);
                    }
                    else
                    {
                        command.AddIndex(vi + 0);
                        command.AddIndex(vi + 1);
                        command.AddIndex(vi + 3);
                        command.AddIndex(vi + 0);
                        command.AddIndex(vi + 3);
                        command.AddIndex(vi + 2);
                    }

                    vi += 4;
                }
            }

            graphics.Command(command);
        }
    }
}
