#define CDK_IMPL

#include "CSBoundingFrustum.h"

CSBoundingFrustum::CSBoundingFrustum(const CSMatrix& matrix) : matrix(matrix) {
    // Left plane
    planes[0].normal.x = matrix.m14 + matrix.m11;
    planes[0].normal.y = matrix.m24 + matrix.m21;
    planes[0].normal.z = matrix.m34 + matrix.m31;
    planes[0].d = matrix.m44 + matrix.m41;
    planes[0].normalize();
    
    // Right plane
    planes[1].normal.x = matrix.m14 - matrix.m11;
    planes[1].normal.y = matrix.m24 - matrix.m21;
    planes[1].normal.z = matrix.m34 - matrix.m31;
    planes[1].d = matrix.m44 - matrix.m41;
    planes[1].normalize();
    
    // Top plane
    planes[2].normal.x = matrix.m14 - matrix.m12;
    planes[2].normal.y = matrix.m24 - matrix.m22;
    planes[2].normal.z = matrix.m34 - matrix.m32;
    planes[2].d = matrix.m44 - matrix.m42;
    planes[2].normalize();
    
    // Bottom plane
    planes[3].normal.x = matrix.m14 + matrix.m12;
    planes[3].normal.y = matrix.m24 + matrix.m22;
    planes[3].normal.z = matrix.m34 + matrix.m32;
    planes[3].d = matrix.m44 + matrix.m42;
    planes[3].normalize();
    
    // Near plane
    planes[4].normal.x = matrix.m13;
    planes[4].normal.y = matrix.m23;
    planes[4].normal.z = matrix.m33;
    planes[4].d = matrix.m43;
    planes[4].normalize();
    
    // Far plane
    planes[5].normal.x = matrix.m14 - matrix.m13;
    planes[5].normal.y = matrix.m24 - matrix.m23;
    planes[5].normal.z = matrix.m34 - matrix.m33;
    planes[5].d = matrix.m44 - matrix.m43;
    planes[5].normalize();
}

static void pointInter3Planes(const CSPlane& p1, const CSPlane& p2, const CSPlane& p3, CSVector3& result) {
    //P = -d1 * N2xN3 / N1.N2xN3 - d2 * N3xN1 / N2.N3xN1 - d3 * N1xN2 / N3.N1xN2
    result =
        CSVector3::cross(p2.normal, p3.normal) * -p1.d / CSVector3::dot(p1.normal, CSVector3::cross(p2.normal, p3.normal)) +
        CSVector3::cross(p3.normal, p1.normal) * -p2.d / CSVector3::dot(p2.normal, CSVector3::cross(p3.normal, p1.normal)) +
        CSVector3::cross(p1.normal, p2.normal) * -p3.d / CSVector3::dot(p3.normal, CSVector3::cross(p1.normal, p2.normal));
}

void CSBoundingFrustum::getCorners(CSVector3* corners) const {
    pointInter3Planes(planes[4], planes[3], planes[1], corners[0]);
    pointInter3Planes(planes[4], planes[2], planes[1], corners[1]);
    pointInter3Planes(planes[4], planes[2], planes[0], corners[2]);
    pointInter3Planes(planes[4], planes[3], planes[0], corners[3]);
    pointInter3Planes(planes[5], planes[3], planes[1], corners[4]);
    pointInter3Planes(planes[5], planes[2], planes[1], corners[5]);
    pointInter3Planes(planes[5], planes[2], planes[0], corners[6]);
    pointInter3Planes(planes[5], planes[3], planes[0], corners[7]);
}
