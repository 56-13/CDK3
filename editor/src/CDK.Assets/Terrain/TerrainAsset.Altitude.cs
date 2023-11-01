using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows;

using CDK.Drawing;

using Point = System.Drawing.Point;

namespace CDK.Assets.Terrain
{
    public class AltitudeUpdatedEventArgs : EventArgs
    {
        public Rectangle Area { private set; get; }
        public AltitudeUpdatedEventArgs(in Rectangle area)
        {
            Area = area;
        }
    }

    partial class TerrainAsset
    {
        private class ModifyAltitudeCommand : IAssetCommand
        {
            private struct Pair
            {
                public float prev;
                public float next;
            }

            private TerrainAsset _asset;
            private Dictionary<Point, Pair> _pairs;
            private int _vminx;
            private int _vminy;
            private int _vmaxx;
            private int _vmaxy;

            public Asset Asset => _asset;

            public ModifyAltitudeCommand(TerrainAsset asset)
            {
                _asset = asset;
                _pairs = new Dictionary<Point, Pair>(asset._modifyingAltitudes.Count);

                _vminx = asset._Width * asset._VertexCell;
                _vminy = asset._Height * asset._VertexCell;
                _vmaxx = 0;
                _vmaxy = 0;

                foreach (var p in asset._modifyingAltitudes.Keys)
                {
                    var prev = asset._modifyingAltitudes[p];
                    var next = asset._altitudes[p.X, p.Y];

                    if (prev != next)
                    {
                        if (_vminx > p.X) _vminx = p.X;
                        if (_vminy > p.Y) _vminy = p.Y;
                        if (_vmaxx < p.X) _vmaxx = p.X;
                        if (_vmaxy < p.Y) _vmaxy = p.Y;

                        var pair = new Pair
                        {
                            prev = prev,
                            next = next
                        };
                        _pairs.Add(p, pair);
                    }
                }
            }

            public void Undo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (var p in _pairs.Keys)
                    {
                        var pair = _pairs[p];
                        _asset._altitudes[p.X, p.Y] = pair.prev;
                    }

                    _asset.UpdateAltitude(_vminx, _vminy, _vmaxx, _vmaxy);

                    _asset.UpdateAmbientOcclusion(_vminx, _vminy, _vmaxx, _vmaxy, AmbientOcclusionAction.All);
                }
            }

            public void Redo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (Point p in _pairs.Keys)
                    {
                        Pair pair = _pairs[p];
                        _asset._altitudes[p.X, p.Y] = pair.next;
                    }

                    _asset.UpdateAltitude(_vminx, _vminy, _vmaxx, _vmaxy);

                    _asset.UpdateAmbientOcclusion(_vminx, _vminy, _vmaxx, _vmaxy, AmbientOcclusionAction.All);
                }
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class ResetAltitudeCommand : IAssetCommand
        {
            private TerrainAsset _asset;
            private float[,] _prev;
            private float[,] _next;

            public Asset Asset => _asset;

            public ResetAltitudeCommand(TerrainAsset asset, float[,] prev)
            {
                _asset = asset;
                _prev = prev;
                _next = (float[,])asset._altitudes.Clone();
            }

            public void Undo()
            {
                _asset._altitudes = _prev;

                _asset.UpdateAltitude();

                _asset.UpdateAmbientOcclusion(AmbientOcclusionAction.All);
            }

            public void Redo()
            {
                _asset._altitudes = _next;

                _asset.UpdateAltitude();

                _asset.UpdateAmbientOcclusion(AmbientOcclusionAction.All);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        internal bool IsZQuad(int vx, int vy)
        {
            var a0 = GetAltitude(vx, vy);
            var a1 = GetAltitude(vx + 1, vy);
            var a2 = GetAltitude(vx, vy + 1);
            var a3 = GetAltitude(vx + 1, vy + 1);

            return IsZQuad(a0, a1, a2, a3);
        }

        internal bool IsZQuad(float a0, float a1, float a2, float a3)
        {
            return (Math.Abs(a3 - a0) >= Math.Abs(a2 - a1));
        }

        internal float GetAltitude(int vx, int vy)
        {
            vx = MathUtil.Clamp(vx, 0, _Width * _VertexCell);
            vy = MathUtil.Clamp(vy, 0, _Height * _VertexCell);
            return _altitudes[vx, vy];
        }

        private static float CubicInterpolate(float n0, float n1, float n2, float n3, float x)
        {
            return n1 + 0.5f * x * (n2 - n0 + x * (2.0f * n0 - 5.0f * n1 + 4.0f * n2 - n3 + x * (3.0f * (n1 - n2) + n3 - n0)));
        }

        public float GetAltitude(Vector2 pos)
        {
            Load();

            pos *= _VertexCell;
            var ix = (int)pos.X;
            var iy = (int)pos.Y;
            
            if (pos.X == ix && pos.Y == iy) return GetAltitude(ix, iy);

            var fx = pos.X - ix;
            var fy = pos.Y - iy;

            var n = new float[4, 4];
            for (var ny = -1; ny <= 2; ny++)
            {
                for (var nx = -1; nx <= 2; nx++)
                {
                    n[nx + 1, ny + 1] = GetAltitude(nx + ix, ny + iy);
                }
            }

            var a0 = CubicInterpolate(n[0, 0], n[0, 1], n[0, 2], n[0, 3], fy);
            var a1 = CubicInterpolate(n[1, 0], n[1, 1], n[1, 2], n[1, 3], fy);
            var a2 = CubicInterpolate(n[2, 0], n[2, 1], n[2, 2], n[2, 3], fy);
            var a3 = CubicInterpolate(n[3, 0], n[3, 1], n[3, 2], n[3, 3], fy);
            return CubicInterpolate(a0, a1, a2, a3, fx);
        }

        internal Vector3 GetNormal(int vx, int vy)
        {
            var lh = GetAltitude(vx - 1, vy);
            var rh = GetAltitude(vx + 1, vy);
            var th = GetAltitude(vx, vy - 1);
            var bh = GetAltitude(vx, vy + 1);

            var p0 = new Vector3(2, 0, rh - lh);
            var p1 = new Vector3(0, 2, bh - th);

            var normal = Vector3.Normalize(Vector3.Cross(p0, p1));

            return normal;
        }

        private Vector3 CubicInterpolateNormal(in Vector3 n0, in Vector3 n1, in Vector3 n2, in Vector3 n3, float x)
        {
            return n1 + 0.5f * x * (n2 - n0 + x * (2.0f * n0 - 5.0f * n1 + 4.0f * n2 - n3 + x * (3.0f * (n1 - n2) + n3 - n0)));
        }

        internal Vector3 GetNormal(Vector2 pos)
        {
            pos *= _VertexCell;
            var ix = (int)pos.X;
            var iy = (int)pos.Y;

            if (pos.X == ix && pos.Y == iy) return GetNormal(ix, iy);

            var fx = pos.X - ix;
            var fy = pos.Y - iy;

            var n = new Vector3[4, 4];
            for (var ny = -1; ny <= 2; ny++)
            {
                for (var nx = -1; nx <= 2; nx++)
                {
                    n[nx + 1, ny + 1] = GetNormal(nx + ix, ny + iy);
                }
            }

            var a0 = CubicInterpolateNormal(n[0, 0], n[0, 1], n[0, 2], n[0, 3], fy);
            var a1 = CubicInterpolateNormal(n[1, 0], n[1, 1], n[1, 2], n[1, 3], fy);
            var a2 = CubicInterpolateNormal(n[2, 0], n[2, 1], n[2, 2], n[2, 3], fy);
            var a3 = CubicInterpolateNormal(n[3, 0], n[3, 1], n[3, 2], n[3, 3], fy);
            return CubicInterpolateNormal(a0, a1, a2, a3, fx);
        }

        public void StartModifyingAltitude()
        {
            Load();

            _modifyingAltitudes.Clear();
        }

        private void ModifyAltitude(Point p)
        {
            if (!_modifyingAltitudes.ContainsKey(p))
            {
                _modifyingAltitudes.Add(p, _altitudes[p.X, p.Y]);
            }
        }

        public void EndModifyingAltitude()
        {
            if (_modifyingAltitudes.Count != 0)
            {
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new ModifyAltitudeCommand(this));
                }
                _modifyingAltitudes.Clear();

                CommitAmbientOcclusion();
            }
        }

        public void ModifyAltitude(Vector2 origin, TerrainModifyMode mode, bool up, float size, float atteunation, float degree)
        {
            Load();

            origin *= _VertexCell;
            size *= _VertexCell;

            var vwidth = _Width * _VertexCell;
            var vheight = _Height * _VertexCell;
            var vminx = (int)(origin.X - size);
            var vminy = (int)(origin.Y - size);
            var vmaxx = (int)(origin.X + size);
            var vmaxy = (int)(origin.Y + size);

            if (vminx > vwidth || vmaxx < 0 || vminy > vheight || vmaxy < 0) return;
            if (vminx < 0) vminx = 0;
            if (vminy < 0) vminy = 0;
            if (vmaxx > vwidth) vmaxx = vwidth;
            if (vmaxy > vheight) vmaxy = vheight;

            for (var vy = vminy; vy <= vmaxy; vy++)
            {
                for (var vx = vminx; vx <= vmaxx; vx++)
                {
                    var d = Vector2.Distance(origin, new Vector2(vx, vy));

                    if (d < size)
                    {
                        ModifyAltitude(new Point(vx, vy));

                        var r = AttenuateRate(d, size, atteunation);

                        switch (mode)
                        {
                            case TerrainModifyMode.Add:
                                if (up)
                                {
                                    _altitudes[vx, vy] = Math.Min(_altitudes[vx, vy] + r * degree, _Altitude);
                                }
                                else
                                {
                                    _altitudes[vx, vy] = Math.Max(_altitudes[vx, vy] - r * degree, 0);
                                }
                                break;
                            case TerrainModifyMode.Equal:
                                if (up)
                                {
                                    if (_altitudes[vx, vy] < degree)
                                    {
                                        _altitudes[vx, vy] += r * (degree - _altitudes[vx, vy]);
                                    }
                                }
                                else
                                {
                                    if (_altitudes[vx, vy] > degree)
                                    {
                                        _altitudes[vx, vy] -= r * (_altitudes[vx, vy] - degree);
                                    }
                                }
                                break;
                            case TerrainModifyMode.Rub:
                                {
                                    var avgAltitude = 0f;
                                    var avgCount = 0;

                                    var nvminx = Math.Max(vx - 1, 0);
                                    var nvminy = Math.Max(vy - 1, 0);
                                    var nvmaxx = Math.Min(vx + 1, vwidth);
                                    var nvmaxy = Math.Min(vy + 1, vheight);
                                    for (var nvy = nvminy; nvy <= nvmaxy; nvy++)
                                    {
                                        for (var nvx = nvminx; nvx <= nvmaxx; nvx++)
                                        {
                                            if (nvx != vx || nvy != vy)
                                            {
                                                avgAltitude += _altitudes[nvx, nvy];
                                                avgCount++;
                                            }
                                        }
                                    }
                                    avgAltitude /= avgCount;

                                    if (up)
                                    {
                                        if (_altitudes[vx, vy] < avgAltitude)
                                        {
                                            _altitudes[vx, vy] = Math.Min(_altitudes[vx, vy] + r * degree, avgAltitude);
                                        }
                                    }
                                    else
                                    {
                                        if (_altitudes[vx, vy] > avgAltitude)
                                        {
                                            _altitudes[vx, vy] = Math.Max(_altitudes[vx, vy] - r * degree, avgAltitude);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            IsDirty = true;

            UpdateAltitude(vminx, vminy, vmaxx, vmaxy);

            UpdateAmbientOcclusion(vminx, vminy, vmaxx, vmaxy, AmbientOcclusionAction.None);
        }

        public void ModifyAltitude(bool up, float degree)
        {
            Load();

            if (degree != 0)
            {
                float[,] prev = null;

                if (AssetManager.Instance.CommandEnabled)
                {
                    prev = (float[,])_altitudes.Clone();
                }

                var vwidth = _Width * _VertexCell;
                var vheight = _Height * _VertexCell;
                for (var vy = 0; vy <= vheight; vy++)
                {
                    for (var vx = 0; vx <= vwidth; vx++)
                    {
                        if (up)
                        {
                            _altitudes[vx, vy] = Math.Min(_altitudes[vx, vy] + degree, _Altitude);
                        }
                        else
                        {
                            _altitudes[vx, vy] = Math.Max(_altitudes[vx, vy] - degree, 0);
                        }
                    }
                }

                IsDirty = true;

                if (AssetManager.Instance.CommandEnabled && prev != null)
                {
                    AssetManager.Instance.Command(new ResetAltitudeCommand(this, prev));
                }

                UpdateAltitude();
            }
        }

        private bool GetSlopeRate(in Vector2 p, in Vector2 p0, in Vector2 p1, float thickness, out float rate)
        {
            var v = p1 - p0;

            var len2 = v.LengthSquared();

            var pv = p - p0;

            rate = (pv.X * v.X + pv.Y * v.Y) / len2;

            if (rate > 1 || rate < 0) return false;
            
            var p2 = p0 + v * rate;

            if (Vector2.Distance(p, p2) > thickness) return false;

            return true;
        }

        public void SlopeAltitude(Vector2 p0, Vector2 p1, float thickness)
        {
            if (VectorUtil.NearEqual(p0, p1) || thickness <= 0.0f) return;

            Load();

            var h0 = GetAltitude(p0);
            var h1 = GetAltitude(p1);

            if (h0 == h1)
            {
                return;
            }


            p0 *= _VertexCell;
            p1 *= _VertexCell;
            thickness *= _VertexCell;

            var vwidth = _Width * _VertexCell;
            var vheight = _Height * _VertexCell;
            var vminx = (int)(Math.Min(p0.X, p1.X) - thickness - 1);
            var vminy = (int)(Math.Min(p0.Y, p1.Y) - thickness - 1);
            var vmaxx = (int)(Math.Max(p0.X, p1.X) + thickness + 1);
            var vmaxy = (int)(Math.Max(p0.Y, p1.Y) + thickness + 1);

            if (vminx > vwidth || vmaxx < 0 || vminy > vheight || vmaxy < 0) return;
            
            if (vminx < 0) vminx = 0;
            if (vminy < 0) vminy = 0;
            if (vmaxx > vwidth) vmaxx = vwidth;
            if (vmaxy > vheight) vmaxy = vheight;

            for (var vy = vminy; vy <= vmaxy; vy++)
            {
                for (var vx = vminx; vx <= vmaxx; vx++)
                {
                    if (GetSlopeRate(new Vector2(vx, vy), p0, p1, thickness, out var rate))
                    {
                        ModifyAltitude(new Point(vx, vy));

                        _altitudes[vx, vy] = MathUtil.Lerp(h0, h1, rate);
                    }
                }
            }
            IsDirty = true;

            UpdateAltitude(vminx, vminy, vmaxx, vmaxy);

            UpdateAmbientOcclusion(vminx, vminy, vmaxx, vmaxy, AmbientOcclusionAction.None);
        }

        public void FillAltitude(float degree)
        {
            Load();

            float[,] prev = null;

            if (AssetManager.Instance.CommandEnabled)
            {
                prev = (float[,])_altitudes.Clone();
            }

            var vwidth = _Width * _VertexCell;
            var vheight = _Height * _VertexCell;

            for (var vy = 0; vy <= vheight; vy++)
            {
                for (var vx = 0; vx <= vwidth; vx++) _altitudes[vx, vy] = degree;
            }
            IsDirty = true;

            if (AssetManager.Instance.CommandEnabled && prev != null)
            {
                AssetManager.Instance.Command(new ResetAltitudeCommand(this, prev));
            }

            UpdateAltitude();

            UpdateAmbientOcclusion(AmbientOcclusionAction.All);
        }

        private void UpdateAltitude(int vminx, int vminy, int vmaxx, int vmaxy)
        {
            _Display?.UpdateAltitude(vminx, vminy, vmaxx, vmaxy);

            if (AltitudeUpdated != null)
            {
                var area = new Rectangle(
                    (float)vminx / _VertexCell,
                    (float)vminy / _VertexCell,
                    (float)(vmaxx - vminx) / _VertexCell,
                    (float)(vmaxy - vminy) / _VertexCell);

                AltitudeUpdated(this, new AltitudeUpdatedEventArgs(area));
            }
        }

        private void UpdateAltitude()
        {
            _Display?.UpdateAltitude();

            if (AltitudeUpdated != null)
            {
                var area = new Rectangle(0, 0, _Width, _Height);

                AltitudeUpdated(this, new AltitudeUpdatedEventArgs(area));
            }
        }

        public event EventHandler<AltitudeUpdatedEventArgs> AltitudeUpdated;

        public void AddWeakAltitudeUpdated(EventHandler<AltitudeUpdatedEventArgs> handler)
        {
            WeakEventManager<TerrainAsset, AltitudeUpdatedEventArgs>.AddHandler(this, "AltitudeUpdated", handler);
        }

        public void RemoveWeakAltitudeUpdated(EventHandler<AltitudeUpdatedEventArgs> handler)
        {
            WeakEventManager<TerrainAsset, AltitudeUpdatedEventArgs>.RemoveHandler(this, "AltitudeUpdated", handler);
        }
    }
}
