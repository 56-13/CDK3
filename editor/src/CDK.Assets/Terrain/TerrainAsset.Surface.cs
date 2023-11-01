using System;
using System.Numerics;
using System.Collections.Generic;
using System.Drawing;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    public partial class TerrainAsset
    {
        internal TerrainSurfaceInstances GetSurfaceInstances(TerrainSurface surface)
        {
            return _surfaceInstances[surface];
        }

        private class ModifySurfaceCommand : IAssetCommand
        {
            private struct Pair
            {
                public TerrainSurfaceInstance prev;
                public TerrainSurfaceInstance next;
            }

            private TerrainAsset _asset;
            private Dictionary<ModifyingSurfaceKey, Pair> _pairs;
            private int _sminx;
            private int _sminy;
            private int _smaxx;
            private int _smaxy;

            public Asset Asset => _asset;

            public ModifySurfaceCommand(TerrainAsset asset)
            {
                _asset = asset;
                _pairs = new Dictionary<ModifyingSurfaceKey, Pair>(asset._modifyingSurfaceInstances.Count);

                _sminx = asset._Width * asset._VertexCell * asset._SurfaceCell;
                _sminy = asset._Height * asset._VertexCell * asset._SurfaceCell;
                _smaxx = 0;
                _smaxy = 0;

                foreach (ModifyingSurfaceKey key in asset._modifyingSurfaceInstances.Keys)
                {
                    var prev = asset._modifyingSurfaceInstances[key];
                    var next = asset._surfaceInstances[key.Surface][key.SurfacePoint];

                    if (prev != next)
                    {
                        if (_sminx > key.SurfacePoint.X) _sminx = key.SurfacePoint.X;
                        if (_sminy > key.SurfacePoint.Y) _sminy = key.SurfacePoint.Y;
                        if (_smaxx < key.SurfacePoint.X) _smaxx = key.SurfacePoint.X;
                        if (_smaxy < key.SurfacePoint.Y) _smaxy = key.SurfacePoint.Y;

                        Pair pair = new Pair();
                        pair.prev = prev;
                        pair.next = next;
                        _pairs.Add(key, pair);
                    }
                }
            }

            public void Undo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (ModifyingSurfaceKey key in _pairs.Keys)
                    {
                        _asset._surfaceInstances[key.Surface][key.SurfacePoint] = _pairs[key].prev;
                    }
                    _asset._Display?.UpdateSurfaces(_sminx, _sminy, _smaxx, _smaxy);
                }
            }

            public void Redo()
            {
                if (_pairs.Count != 0)
                {
                    foreach (ModifyingSurfaceKey key in _pairs.Keys)
                    {
                        _asset._surfaceInstances[key.Surface][key.SurfacePoint] = _pairs[key].next;
                    }
                    _asset._Display?.UpdateSurfaces(_sminx, _sminy, _smaxx, _smaxy);
                }
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class ResetSurfaceCommand : IAssetCommand
        {
            private TerrainAsset _asset;
            private int _prev;
            private int _next;

            public Asset Asset => _asset;

            public ResetSurfaceCommand(TerrainAsset asset, int prev)
            {
                _asset = asset;
                _prev = prev;
                _next = asset.RecordSurfaceInstances();
            }

            ~ResetSurfaceCommand()
            {
                DeleteSurfaceInstancesRecord(_prev);
                DeleteSurfaceInstancesRecord(_next);
            }

            public void Undo()
            {
                _asset.RestoreSurfaceInstances(_prev);

                _asset._Display?.UpdateSurfaces();
            }

            public void Redo()
            {
                _asset.RestoreSurfaceInstances(_next);

                _asset._Display?.UpdateSurfaces();
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private struct ModifyingSurfaceKey
        {
            public TerrainSurface Surface { private set; get; }
            public Point SurfacePoint { private set; get; }

            public ModifyingSurfaceKey(TerrainSurface surface, Point cellPoint) : this()
            {
                Surface = surface;
                SurfacePoint = cellPoint;
            }

            public override bool Equals(object obj)
            {
                var other = (ModifyingSurfaceKey)obj;

                return Surface == other.Surface && SurfacePoint == other.SurfacePoint;
            }

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Surface.GetHashCode());
                hash.Combine(SurfacePoint.GetHashCode());
                return hash;
            }
        }

        public void StartModifyingSurface()
        {
            Load();

            _modifyingSurfaceInstances.Clear();
        }

        private void ModifySurface(TerrainSurface surface, Point cp)
        {
            var key = new ModifyingSurfaceKey(surface, cp);

            if (!_modifyingSurfaceInstances.ContainsKey(key))
            {
                _modifyingSurfaceInstances.Add(key, _surfaceInstances[surface][cp.X, cp.Y]);
            }
        }

        public void EndModifyingSurface()
        {
            if (_modifyingSurfaceInstances.Count != 0)
            {
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new ModifySurfaceCommand(this));
                }
                _modifyingSurfaceInstances.Clear();
            }
        }

        private static readonly double MinSurfaceDensity = 1.0 / 512.0;
        private static readonly double MaxSurfaceDensity = 1.0 - MinSurfaceDensity;

        public void RemoveSurface(Vector2 origin, TerrainModifyMode mode, float size, float attenuation, float degree)
        {
            Load();

            origin *= _VertexCell * _SurfaceCell;
            size *= _VertexCell * _SurfaceCell;
            var swidth = _Width * _VertexCell * _SurfaceCell;
            var sheight = _Height * _VertexCell * _SurfaceCell;
            var sminx = (int)(origin.X - size);
            var sminy = (int)(origin.Y - size);
            var smaxx = (int)(origin.X + size);
            var smaxy = (int)(origin.Y + size);
            
            if (sminx > swidth || smaxx < 0 || sminy > sheight || smaxy < 0) return;
            if (sminx < 0) sminx = 0;
            if (sminy < 0) sminy = 0;
            if (smaxx > swidth) smaxx = swidth;
            if (smaxy > sheight) smaxy = sheight;

            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    var d = Vector2.Distance(new Vector2(sx, sy), origin);

                    if (d < size)
                    {
                        var r = AttenuateRate(d, size, attenuation);

                        foreach (var surface in _Surfaces)
                        {
                            ModifySurface(surface, new Point(sx, sy));

                            var surfaceInstances = _surfaceInstances[surface];

                            var nextInstance = surfaceInstances[sx, sy];

                            switch (mode)
                            {
                                case TerrainModifyMode.Add:
                                    nextInstance.Intermediate = Math.Max(nextInstance.Intermediate - r * degree, 0);
                                    break;
                                case TerrainModifyMode.Equal:
                                    if (nextInstance.Intermediate > degree)
                                    {
                                        nextInstance.Intermediate -= r * (nextInstance.Intermediate - degree);
                                    }
                                    break;
                                case TerrainModifyMode.Rub:
                                    {
                                        double density = 0;
                                        var count = 0;

                                        var nsminx = Math.Max(sx - 1, 0);
                                        var nsmaxx = Math.Min(sx + 1, swidth);
                                        var nsminy = Math.Max(sy - 1, 0);
                                        var nsmaxy = Math.Min(sy + 1, sheight);
                                        for (var nsy = nsminy; nsy <= nsmaxy; nsy++)
                                        {
                                            for (var nsx = nsminx; nsx <= nsmaxx; nsx++)
                                            {
                                                if (nsx != sx || nsy != sy)
                                                {
                                                    density += surfaceInstances[nsx, nsy].Current;

                                                    count++;
                                                }
                                            }
                                        }
                                        density /= count;
                                        if (nextInstance.Current > density)
                                        {
                                            nextInstance.Intermediate = Math.Max(nextInstance.Current - r * degree, density);
                                        }
                                    }
                                    break;
                            }
                            nextInstance.Current = nextInstance.Intermediate;

                            surfaceInstances[sx, sy] = nextInstance;
                        }
                    }
                }
            }

            IsDirty = true;

            _Display?.UpdateSurfaces(sminx, sminy, smaxx, smaxy);
        }

        public void ModifySurface(TerrainSurface originSurface, TerrainSurface surface, Vector2 origin, TerrainModifyMode mode, bool up, float size, float attenuation, float degree, float shadow, float shadowAttenuation)
        {
            Load();

            if (!_Surfaces.Contains(surface) || (originSurface != null && !_Surfaces.Contains(originSurface))) throw new InvalidOperationException();

            origin *= _VertexCell * _SurfaceCell;

            shadowAttenuation = shadow != 0 ? shadowAttenuation * _VertexCell * _SurfaceCell : 0;
            size = Math.Max(size * _VertexCell * _SurfaceCell, shadowAttenuation);

            var swidth = _Width * _VertexCell * _SurfaceCell;
            var sheight = _Height * _VertexCell * _SurfaceCell;
            var sminx = (int)(origin.X - size);
            var sminy = (int)(origin.Y - size);
            var smaxx = (int)(origin.X + size);
            var smaxy = (int)(origin.Y + size);

            if (sminx > swidth || smaxx < 0 || sminy > sheight || smaxy < 0) return;

            if (sminx < 0) sminx = 0;
            if (sminy < 0) sminy = 0;
            if (smaxx > swidth) smaxx = swidth;
            if (smaxy > sheight) smaxy = sheight;

            var surfaceInstances = _surfaceInstances[surface];
            var originSurfaceInstances = originSurface != null ? _surfaceInstances[originSurface] : null;

            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    var d = Vector2.Distance(new Vector2(sx, sy), origin);

                    var cp = new Point(sx, sy);

                    if (d < size)
                    {
                        var r = AttenuateRate(d, size, attenuation);

                        var prevInstance = surfaceInstances[sx, sy];

                        var originMax = originSurfaceInstances != null ? originSurfaceInstances[sx, sy].Intermediate / (1 - prevInstance.Intermediate) : MaxSurfaceDensity;

                        ModifySurface(surface, cp);

                        var nextInstance = prevInstance;

                        switch (mode)
                        {
                            case TerrainModifyMode.Add:
                                if (up)
                                {
                                    if (nextInstance.Intermediate < originMax)
                                    {
                                        nextInstance.Intermediate = Math.Min(nextInstance.Intermediate + r * degree, originMax);
                                    }
                                }
                                else
                                {
                                    nextInstance.Intermediate = Math.Max(nextInstance.Intermediate - r * degree, 0);
                                }
                                break;
                            case TerrainModifyMode.Equal:
                                if (up)
                                {
                                    var max = Math.Min(originMax, degree);

                                    if (nextInstance.Intermediate < max)
                                    {
                                        nextInstance.Intermediate += r * (max - nextInstance.Intermediate);
                                    }
                                }
                                else
                                {
                                    if (nextInstance.Intermediate > degree)
                                    {
                                        nextInstance.Intermediate -= r * (nextInstance.Intermediate - degree);
                                    }
                                }
                                break;
                            case TerrainModifyMode.Rub:
                                {
                                    double density = 0;
                                    var count = 0;

                                    var nsminx = Math.Max(sx - 1, 0);
                                    var nsmaxx = Math.Min(sx + 1, swidth);
                                    var nsminy = Math.Max(sy - 1, 0);
                                    var nsmaxy = Math.Min(sy + 1, sheight);
                                    for (var nsy = nsminy; nsy <= nsmaxy; nsy++)
                                    {
                                        for (var nsx = nsminx; nsx <= nsmaxx; nsx++)
                                        {
                                            if (nsx != sx || nsy != sy)
                                            {
                                                density += surfaceInstances[nsx, nsy].Current;

                                                count++;
                                            }
                                        }
                                    }
                                    density /= count;

                                    if (up)
                                    {
                                        var max = Math.Min(originMax, density);

                                        if (nextInstance.Current < max)
                                        {
                                            nextInstance.Intermediate = Math.Min(nextInstance.Current + r * degree, max);
                                        }
                                    }
                                    else
                                    {
                                        if (nextInstance.Current > density)
                                        {
                                            nextInstance.Intermediate = Math.Max(nextInstance.Current - r * degree, density);
                                        }
                                    }
                                }
                                break;
                        }
                        if (prevInstance.Intermediate != nextInstance.Intermediate)
                        {
                            var a = (1 - nextInstance.Intermediate) / (1 - prevInstance.Intermediate);

                            foreach (var otherSurface in _Surfaces)
                            {
                                if (otherSurface != surface)
                                {
                                    var otherSurfaceInstances = _surfaceInstances[otherSurface];

                                    var otherSurfaceInstance = otherSurfaceInstances[sx, sy];

                                    if (otherSurfaceInstance.Current != 0)
                                    {
                                        ModifySurface(otherSurface, cp);

                                        otherSurfaceInstances[sx, sy] = new TerrainSurfaceInstance(otherSurfaceInstance.Intermediate * a, otherSurfaceInstance.Current * a);
                                    }
                                }
                            }
                        }
                        if (up && mode != TerrainModifyMode.Rub && shadow != 0 && d > size - shadowAttenuation)
                        {
                            r = MathUtil.SmoothStep((size - d) / shadowAttenuation);

                            var a = 1.0 - (1.0 - r) * shadow;

                            nextInstance.Current = Math.Max(nextInstance.Intermediate * a, prevInstance.Current);
                        }
                        else
                        {
                            nextInstance.Current = nextInstance.Intermediate;
                        }
                        surfaceInstances[sx, sy] = nextInstance;
                    }
                }
            }

            IsDirty = true;

            _Display?.UpdateSurfaces(sminx, sminy, smaxx, smaxy);
        }

        public void FillSurface(TerrainSurface surface)
        {
            Load();

            if (!_Surfaces.Contains(surface)) throw new InvalidOperationException();

            var prev = 0;

            if (AssetManager.Instance.CommandEnabled) prev = RecordSurfaceInstances();

            var swidth = _Width * _VertexCell * _SurfaceCell;
            var sheight = _Height * _VertexCell * _SurfaceCell;

            foreach (var otherSurface in _Surfaces)
            {
                var density = otherSurface == surface ? MaxSurfaceDensity : 0;
                var otherSurfaceInstances = _surfaceInstances[otherSurface];
                for (var sy = 0; sy <= sheight; sy++)
                {
                    for (var sx = 0; sx <= swidth; sx++)
                    {
                        otherSurfaceInstances[sx, sy] = new TerrainSurfaceInstance(density);
                    }
                }
            }

            IsDirty = true;

            if (AssetManager.Instance.CommandEnabled && prev != 0)
            {
                AssetManager.Instance.Command(new ResetSurfaceCommand(this, prev));
            }

            _Display?.UpdateSurfaces();
        }
        public void ClearSurface(TerrainSurface surface)
        {
            Load();

            if (!_Surfaces.Contains(surface)) throw new InvalidOperationException();

            var prev = 0;

            if (AssetManager.Instance.CommandEnabled) prev = RecordSurfaceInstances();

            var swidth = _Width * _VertexCell * _SurfaceCell;
            var sheight = _Height * _VertexCell * _SurfaceCell;

            if (surface != null)
            {
                var surfaceInstances = _surfaceInstances[surface];

                for (var sy = 0; sy <= sheight; sy++)
                {
                    for (var sx = 0; sx <= swidth; sx++)
                    {
                        var prevIntermediate = surfaceInstances[sx, sy].Intermediate;

                        surfaceInstances[sx, sy] = TerrainSurfaceInstance.Empty;

                        if (prevIntermediate != 0)
                        {
                            var a = 1 / (1 - prevIntermediate);

                            foreach (var otherSurface in _Surfaces)
                            {
                                if (otherSurface != surface)
                                {
                                    var otherSurfaceInstances = _surfaceInstances[otherSurface];

                                    var otherSurfaceInstance = otherSurfaceInstances[sx, sy];

                                    if (otherSurfaceInstance.Current != 0)
                                    {
                                        otherSurfaceInstances[sx, sy] = new TerrainSurfaceInstance(otherSurfaceInstance.Intermediate * a, otherSurfaceInstance.Current * a);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var otherSurface in _Surfaces)
                {
                    var otherSurfaceInstances = _surfaceInstances[otherSurface];

                    for (var sy = 0; sy <= sheight; sy++)
                    {
                        for (var sx = 0; sx <= swidth; sx++)
                        {
                            otherSurfaceInstances[sx, sy] = TerrainSurfaceInstance.Empty;
                        }
                    }
                }
            }

            IsDirty = true;

            if (AssetManager.Instance.CommandEnabled && prev != 0)
            {
                AssetManager.Instance.Command(new ResetSurfaceCommand(this, prev));
            }

            _Display?.UpdateSurfaces();
        }
    }
}
