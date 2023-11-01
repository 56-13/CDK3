#define CDK_IMPL

#include "CSLightSpace.h"

#include "CSVector4.h"

CSVector3 CSLightSpace::worldToCluster(const CSVector3& wp) {
    CSVector4 vp = CSVector3::transform(wp, _camera.viewProjection());

    if (vp.w != 0) {
        CSVector3 cp(
            (vp.x / CSMath::abs(vp.w) * 0.5f + 0.5f) * Cluster,
            (vp.y / CSMath::abs(vp.w) * 0.5f + 0.5f) * Cluster,
            vp.w / _clusterMaxDepth * Cluster);
        return cp;
    }
    else {
        CSVector3 cp(Cluster / 2);
        if (vp.x < 0) cp.x = -1;
        else if (vp.x > 0) cp.x = Cluster;
        if (vp.y < 0) cp.y = -1;
        else if (vp.y > 0) cp.y = Cluster;
        if (vp.z < 0) cp.z = -1;
        else if (vp.z > 0) cp.z = Cluster;
        return cp;
    }
}

CSVector3 CSLightSpace::clusterToWorld(const CSVector3& cp) {
    CSAssert(cp.x >= 0 && cp.x <= Cluster && cp.y >= 0 && cp.y <= Cluster && cp.z >= 0 && cp.z <= Cluster);

    CSVector4 vp;
    vp.w = CSMath::max(cp.z, ClusterZNear) * _clusterMaxDepth / Cluster;
    vp.x = (cp.x / Cluster * 2 - 1) * vp.w;
    vp.y = (cp.y / Cluster * 2 - 1) * vp.w;
    vp.z = (1 - (vp.x * _viewProjectionInv.m14 + vp.y * _viewProjectionInv.m24 + vp.w * _viewProjectionInv.m44)) / _viewProjectionInv.m34;

    CSVector4 wp = CSVector4::transform(vp, _viewProjectionInv);
    return (CSVector3)wp;
}

CSVector3 CSLightSpace::clusterGridToWorld(int x, int y, int z, const CSVector3& lcp) {
    CSVector3 cp(
        x <= lcp.x - 1 ? x + 1 : (x > lcp.x ? x : lcp.x),
        y <= lcp.y - 1 ? y + 1 : (y > lcp.y ? y : lcp.y),
        z <= lcp.z - 1 ? z + 1 : (z > lcp.z ? z : lcp.z));

    return clusterToWorld(cp);
}
