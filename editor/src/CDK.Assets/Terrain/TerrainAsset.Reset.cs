using System;
using System.Numerics;
using System.Collections.Generic;

using CDK.Drawing;

using CDK.Assets.Configs;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        private class State
        {
            private int _width;
            private int _height;
            private int _vertexCell;
            private int _surfaceCell;
            private float[,] _altitudes;
            private int _surfaceInstancesRecord;
            private TerrainWaterInstance[,] _waterInstances;
            private string[,] tiles;

            public State(TerrainAsset asset)
            {
                _width = asset._Width;
                _height = asset._Height;
                _vertexCell = asset._VertexCell;
                _surfaceCell = asset._SurfaceCell;

                _altitudes = (float[,])asset._altitudes.Clone();
                _waterInstances = (TerrainWaterInstance[,])asset._waterInstances.Clone();
                _surfaceInstancesRecord = asset.RecordSurfaceInstances();
                tiles = (string[,])asset._tiles.Clone();
            }

            ~State()
            {
                DeleteSurfaceInstancesRecord(_surfaceInstancesRecord);
            }

            public void Restore(TerrainAsset asset)
            {
                asset._Width = _width;
                asset._Height = _height;
                asset._VertexCell = _vertexCell;
                asset._SurfaceCell = _surfaceCell;
                asset._altitudes = _altitudes;
                asset.AmbientOcclusions = new byte[_width * _vertexCell * _surfaceCell + 1, _height * _vertexCell * _surfaceCell + 1];
                asset.UpdateAmbientOcclusion(AmbientOcclusionAction.Raw);
                asset.RestoreSurfaceInstances(_surfaceInstancesRecord);
                asset._waterInstances = _waterInstances;
                asset._tiles = tiles;

                asset.OnPropertyChanged("Width");
                asset.OnPropertyChanged("Height");
                asset.OnPropertyChanged("VertexCell");
                asset.OnPropertyChanged("SurfaceCell");
                asset.OnPropertyChanged("Space");

                asset._Display?.Reset();
            }
        }

        private class ResetCommand : IAssetCommand
        {
            private TerrainAsset _asset;
            private State _prev;
            private State _next;

            public Asset Asset => _asset;

            public ResetCommand(TerrainAsset asset, State prevState)
            {
                _asset = asset;
                _prev = prevState;
                _next = new State(asset);
            }

            public void Undo()
            {
                _prev.Restore(_asset);
            }

            public void Redo()
            {
                _next.Restore(_asset);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        public void Resize(int width, int height, int vertexCell, int surfaceCell, Align align)
        {
            State prev = null;

            if (AssetManager.Instance.CommandEnabled)
            {
                prev = new State(this);
            }

            var altitudes = new float[width * vertexCell + 1, height * vertexCell + 1];

            var surfaceInstances = new Dictionary<TerrainSurface, TerrainSurfaceInstances>();
            foreach (var surface in _Surfaces)
            {
                surfaceInstances.Add(surface, new TerrainSurfaceInstances(width * vertexCell * surfaceCell + 1, height * vertexCell * surfaceCell + 1));
            }

            var waterInstances = new TerrainWaterInstance[width, height];

            var tiles = new string[width, height];

            var bx = 0;
            var by = 0;

            if (((int)align & AlignComponent.Center) != 0) bx = (_Width - width) / 2;
            else if (((int)align & AlignComponent.Right) != 0) bx = _Width - width;

            if (((int)align & AlignComponent.Middle) != 0) by = (_Height - height) / 2;
            else if (((int)align & AlignComponent.Bottom) != 0) by = _Height - height;

            var vwidth = width * vertexCell;
            var vheight = height * vertexCell;

            for (var vy = 0; vy <= vheight; vy++)
            {
                for (var vx = 0; vx <= vwidth; vx++)
                {
                    var origin = new Vector2(bx + (float)vx / vertexCell, by + (float)vy / vertexCell);

                    altitudes[vx, vy] = GetAltitude(origin);
                }
            }

            var swidth = vwidth * surfaceCell;
            var sheight = vheight * surfaceCell;

            foreach (var surface in _Surfaces)
            {
                var src = _surfaceInstances[surface];
                var dest = surfaceInstances[surface];

                for (var sy = 0; sy <= sheight; sy++)
                {
                    for (var sx = 0; sx <= swidth; sx++)
                    {
                        var origin = new Vector2(bx + (float)sx / (vertexCell * surfaceCell), by + (float)sy / (vertexCell * surfaceCell));

                        if (origin.X >= 0 && origin.X <= _Width && origin.Y >= 0 && origin.Y <= _Height)
                        {
                            origin *= _VertexCell * _SurfaceCell;

                            var ix = (int)origin.X;
                            var iy = (int)origin.Y;
                            var fx = origin.X - ix;
                            var fy = origin.Y - iy;

                            var nix = Math.Min(ix + 1, _Width * _VertexCell * _SurfaceCell);
                            var niy = Math.Min(iy + 1, _Height * _VertexCell * _SurfaceCell);

                            var i0 = src[ix, iy];
                            var i1 = src[nix, iy];
                            var i2 = src[ix, niy];
                            var i3 = src[nix, niy];

                            var instance = new TerrainSurfaceInstance
                            {
                                Current = MathUtil.Lerp(MathUtil.Lerp(i0.Current, i1.Current, fx), MathUtil.Lerp(i2.Current, i3.Current, fx), fy),
                                Intermediate = MathUtil.Lerp(MathUtil.Lerp(i0.Intermediate, i1.Intermediate, fx), MathUtil.Lerp(i2.Intermediate, i3.Intermediate, fx), fy)
                            };
                            dest[sx, sy] = instance;
                        }
                    }
                }
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sx = bx + x;
                    int sy = by + y;
                    if (sx >= 0 && sx < _Width && sy >= 0 && sy < _Height)
                    {
                        waterInstances[x, y] = _waterInstances[sx, sy];
                        tiles[x, y] = _tiles[sx, sy];
                    }
                }
            }
            _altitudes = altitudes;
            _surfaceInstances = surfaceInstances;
            _waterInstances = waterInstances;
            _tiles = tiles;

            _Width = width;
            _Height = height;
            _VertexCell = vertexCell;
            _SurfaceCell = surfaceCell;

            AmbientOcclusions = new byte[swidth + 1, sheight + 1];
            UpdateAmbientOcclusion(AmbientOcclusionAction.Raw);

            _Props.Offset(new Vector3(-bx * _Grid, -by * _Grid, 0), true);

            SurfaceOffset = new Vector2(SurfaceOffset.X + bx, SurfaceOffset.Y + by);

            OnPropertyChanged("Width");
            OnPropertyChanged("Height");
            OnPropertyChanged("VertexCell");
            OnPropertyChanged("SurfaceCell");
            OnPropertyChanged("Space");

            _Display?.Reset();

            if (AssetManager.Instance.CommandEnabled && prev != null)
            {
                AssetManager.Instance.Command(new ResetCommand(this, prev));
            }

            AssetManager.Instance.ClearClip();

            if (AssetManager.Instance.RetrieveEnabled)
            {
                using (new AssetRetrieveHolder())
                {
                    foreach (TerrainAsset sibling in GetSiblings())
                    {
                        sibling.Load();

                        if (sibling._VertexCell != vertexCell || sibling._SurfaceCell != surfaceCell)
                        {
                            sibling.Resize(sibling._Width, sibling._Height, vertexCell, surfaceCell, 0);
                        }
                    }
                }
            }
        }
    }
}
