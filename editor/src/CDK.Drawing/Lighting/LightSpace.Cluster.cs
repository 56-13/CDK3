using System;
using System.Numerics;

namespace CDK.Drawing
{
    public partial class LightSpace
    {
        private const int Cluster = 8;

        private const float ClusterZNear = 0.1f;

        private Vector3 WorldToCluster(in Vector3 wp)
        {
            var vp = Vector4.Transform(wp, _camera.ViewProjection);

            if (vp.W != 0)
            {
                var cp = new Vector3(
                    (vp.X / Math.Abs(vp.W) * 0.5f + 0.5f) * Cluster,
                    (vp.Y / Math.Abs(vp.W) * 0.5f + 0.5f) * Cluster,
                    vp.W / _clusterMaxDepth * Cluster);
                return cp;
            }
            else
            {
                var cp = new Vector3(Cluster / 2);
                if (vp.X < 0) cp.X = -1;
                else if (vp.X > 0) cp.X = Cluster;
                if (vp.Y < 0) cp.Y = -1;
                else if (vp.Y > 0) cp.Y = Cluster;
                if (vp.Z < 0) cp.Z = -1;
                else if (vp.Z > 0) cp.Z = Cluster;
                return cp;
            }
        }

        private Vector3 ClusterToWorld(in Vector3 cp)
        {
            Debug.Assert(cp.X >= 0 && cp.X <= Cluster && cp.Y >= 0 && cp.Y <= Cluster && cp.Z >= 0 && cp.Z <= Cluster);

            var w = Math.Max(cp.Z, ClusterZNear) * _clusterMaxDepth / Cluster;
            var x = (cp.X / Cluster * 2 - 1) * w;
            var y = (cp.Y / Cluster * 2 - 1) * w;
            var z = (1 - (x * _viewProjectionInv.M14 + y * _viewProjectionInv.M24 + w * _viewProjectionInv.M44)) / _viewProjectionInv.M34;

            var vp = new Vector4(x, y, z, w);
            var wp = Vector4.Transform(vp, _viewProjectionInv);
            return wp.ToVector3();
        }

        private Vector3 ClusterGridToWorld(int x, int y, int z, in Vector3 lcp)
        {
            var cx = x <= lcp.X - 1 ? x + 1 : (x > lcp.X ? x : lcp.X);
            var cy = y <= lcp.Y - 1 ? y + 1 : (y > lcp.Y ? y : lcp.Y);
            var cz = z <= lcp.Z - 1 ? z + 1 : (z > lcp.Z ? z : lcp.Z);

            return ClusterToWorld(new Vector3(cx, cy, cz));
        }
    }
}
