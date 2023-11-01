#define CDK_IMPL

#include "CSCollision.h"

#include "CSSegment.h"

void CSCollision::SegmentToPoint(const CSSegment& seg, const CSVector3& point, CSVector3& segNear) {
    CSVector3 v = seg.position1 - seg.position0;

    float len2 = v.lengthSquared();

    if (CSMath::nearZero(len2)) {
        segNear = seg.position0;
        return;
    }

    CSVector3 pv = point - seg.position0;

    float t = CSVector3::dot(pv, v) / len2;

    if (t > 1) segNear = seg.position1;
    else if (t > 0) segNear = seg.position0 + v * t;
    else segNear = seg.position0;
}

void CSCollision::SegmentToSegment(const CSSegment& seg0, const CSSegment& seg1, CSVector3& segNear0, CSVector3& segNear1) {
    CSVector3 r = seg1.position0 - seg0.position0;
    CSVector3 u = seg0.position1 - seg0.position0;
    CSVector3 v = seg1.position1 - seg1.position0;

    float ru = CSVector3::dot(r, u);
    float rv = CSVector3::dot(r, v);
    float uu = CSVector3::dot(u, u);
    float uv = CSVector3::dot(u, v);
    float vv = CSVector3::dot(v, v);

    float det = uu * vv - uv * uv;

    float s, t;

    if (det < CSMath::ZeroTolerance * uu * vv) {
        s = CSMath::clamp(ru / uu, 0.0f, 1.0f);
        t = 0;
    }
    else {
        s = CSMath::clamp((ru * vv - rv * uv) / det, 0.0f, 1.0f);
        t = CSMath::clamp((ru * uv - rv * uu) / det, 0.0f, 1.0f);
    }

    float a = CSMath::clamp((t * uv + ru) / uu, 0.0f, 1.0f);
    float b = CSMath::clamp((s * uv - rv) / vv, 0.0f, 1.0f);

    segNear0 = seg0.position0 + u * a;
    segNear1 = seg1.position0 + v * b;
}
