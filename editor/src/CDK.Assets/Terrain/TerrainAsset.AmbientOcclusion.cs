using System;
using System.Numerics;
using System.Threading.Tasks;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        internal byte[,] AmbientOcclusions { private set; get; }

        private int _aovminx;
        private int _aovminy;
        private int _aovmaxx;
        private int _aovmaxy;
        private bool _aoupdated;

        private float GetAmbientOcclusion(in Vector2 origin)
        {
            var normal = GetNormal(origin);
            var h = GetAltitude(origin);

            var occ = 0f;
            foreach (var offset in AmbientOcclusionOffsets)
            {
                var vec = new Vector3(offset.X, offset.Y, GetAltitude(origin + offset) - h);

                var len = vec.Length();

                occ += Vector3.Dot(normal, vec) / (len * (float)Math.Sqrt(len));
            }
            occ /= AmbientOcclusionOffsets.Length;

            return 1 - occ;
        }

        private void UpdateAmbientOcclusionImpl(int sminx, int sminy, int smaxx, int smaxy)
        {
            var s = 1f / (_VertexCell * _SurfaceCell);

            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    var pos = new Vector2(sx * s, sy * s);

                    var ao = GetAmbientOcclusion(pos);

                    AmbientOcclusions[sx, sy] = MathUtil.FloatToUNorm(ao);
                }
            }
        }

        private enum AmbientOcclusionAction
        {
            None,
            Raw,
            All
        }

        private void UpdateAmbientOcclusion(AmbientOcclusionAction action)
        {
            var vwidth = _Width * _VertexCell;
            var vheight = _Height * _VertexCell;

            _aovminx = 0;
            _aovminy = 0;
            _aovmaxx = vwidth;
            _aovmaxy = vheight;
            _aoupdated = true;

            if (action != AmbientOcclusionAction.None) CommitAmbientOcclusion(action == AmbientOcclusionAction.All);
        }

        private void UpdateAmbientOcclusion(int vminx, int vminy, int vmaxx, int vmaxy, AmbientOcclusionAction action)
        {
            var vwidth = _Width * _VertexCell;
            var vheight = _Height * _VertexCell;
            
            vminx = Math.Max((int)(vminx - AmbientOcclusionRange), 0);
            vmaxx = Math.Min((int)(vmaxx + AmbientOcclusionRange), vwidth);
            vminy = Math.Max((int)(vminy - AmbientOcclusionRange), 0);
            vmaxy = Math.Min((int)(vmaxy + AmbientOcclusionRange), vheight);

            if (_aoupdated)
            {
                if (vminx < _aovminx) _aovminx = vminx;
                if (vminy < _aovminy) _aovminy = vminy;
                if (vmaxx > _aovmaxx) _aovmaxx = vmaxx;
                if (vmaxy > _aovmaxy) _aovmaxy = vmaxy;
            }
            else
            {
                _aovminx = vminx;
                _aovminy = vminy;
                _aovmaxx = vmaxx;
                _aovmaxy = vmaxy;
                _aoupdated = true;
            }
            if (action != AmbientOcclusionAction.None) CommitAmbientOcclusion(action == AmbientOcclusionAction.All);
        }

        private void CommitAmbientOcclusion(bool display = true)
        {
            if (!_aoupdated) return;

            var sminx = _aovminx * _SurfaceCell;
            var sminy = _aovminy * _SurfaceCell;
            var smaxx = _aovmaxx * _SurfaceCell;
            var smaxy = _aovmaxy * _SurfaceCell;
            var scx = (sminx + smaxx) / 2;
            var scy = (sminy + smaxy) / 2;

            var t0 = Task.Run(() => UpdateAmbientOcclusionImpl(sminx, sminy, scx, scy));
            var t1 = Task.Run(() => UpdateAmbientOcclusionImpl(scx + 1, sminy, smaxx, scy));
            var t2 = Task.Run(() => UpdateAmbientOcclusionImpl(sminx, scy + 1, scx, smaxy));
            var t3 = Task.Run(() => UpdateAmbientOcclusionImpl(scx + 1, scy + 1, smaxx, smaxy));

            Task.WaitAll(t0, t1, t2, t3);
            
            if (display) _Display?.UpdateAmbientOcclusion(sminx, sminy, smaxx, smaxy);

            _aoupdated = false;
        }
    }
}
