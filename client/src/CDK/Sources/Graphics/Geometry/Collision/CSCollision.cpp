#define CDK_IMPL

#include "CSCollision.h"

void CSCollision::project(const CSVector3& axis, const CSVector3* points, int count, float& min, float& max) {
    min = FloatMax;
    max = FloatMin;
    for (int i = 0; i < count; i++) {
        float val = CSVector3::dot(axis, points[i]);
        if (val < min) min = val;
        if (val > max) max = val;
    }
}

void CSCollision::project(const CSVector3& axis, const CSVector3* points, int count, const CSVector3& offset, float& min, float& max) {
    min = FloatMax;
    max = FloatMin;
    for (int i = 0; i < count; i++) {
        float val = CSVector3::dot(axis, points[i] - offset);
        if (val < min) min = val;
        if (val > max) max = val;
    }
}
