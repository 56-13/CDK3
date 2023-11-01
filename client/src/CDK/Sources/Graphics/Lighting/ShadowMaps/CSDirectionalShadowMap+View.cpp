#define CDK_IMPL

#include "CSDirectionalShadowMap.h"

static void getAABB(const CSArray<CSVector3>* points, CSVector3& min, CSVector3& max);
static void getAABB(const CSArray<CSVector3>* points, const CSMatrix& view, CSVector3& min, CSVector3& max);
static const CSArray<CSVector3, 2>* clipObjectByPlane(const CSArray<CSVector3, 2>* obj, const CSPlane& plane);
static void clipPointsByPlane(const CSArray<CSVector3>* objp, const CSPlane& plane, CSArray<CSVector3>* resultp, CSArray<CSVector3>* interp);
static void appendIntersectionPoints(CSArray<CSVector3, 2>* obj, CSArray<CSVector3, 2>* inter);
static float relativeEpsilon(float a, float epsilon);
static bool nearEqual(float a, float b);
static int findSamePointInVecPoint(CSArray<CSVector3>* poly, const CSVector3& p);
static int findSamePointInObjectAndSwapWithLast(CSArray<CSVector3, 2>* inter, const CSVector3& p);
static bool clipTest(float p, float q, float& u1, float& u2);
static bool lineIntersectsBox(const CSVector3& p, const CSVector3& dir, const CSABoundingBox& b, CSVector3& v);

void CSDirectionalShadowMap::updateView() {
    const CSArray<CSVector3>* points = lightVolumePoints();

    _visible = points->count() != 0;

    if (_visible) {
        updateView(points);
        updateView2D(points);
    }
}

const CSArray<CSVector3>* CSDirectionalShadowMap::lightVolumePoints() {
    const CSArray<CSVector3, 2>* obj = frustumObject();

    clipObjectByScene(obj);

    return objectLightVolumePoints(obj);
}

void CSDirectionalShadowMap::updateView2D(const CSArray<CSVector3>* points) {
    CSVector3 min, max;
    getAABB(points, min, max);

    CSMatrix lightViewProj;
    CSMatrix::orthoOffCenterLH(min.x, max.x, min.y, max.y, min.z, max.z, lightViewProj);

    _viewProjection[1] = lightViewProj;
}

void CSDirectionalShadowMap::updateView(const CSArray<CSVector3>* points) {
    CSVector3 left = CSVector3::cross(_light.direction, _camera.forward());
    CSVector3 up = CSVector3::normalize(CSVector3::cross(left, _light.direction));

    CSMatrix lightView;
    CSMatrix::lookAtRH(_camera.position(), _camera.position() + _light.direction, up, lightView);
    
    CSVector3 min, max;
    getAABB(points, lightView, min, max);

    double cosGamma = CSVector3::dot(_camera.forward(), _light.direction);
    double sinGamma = CSMath::sqrt(1.0 - cosGamma * cosGamma);
    double factor = 1.0 / sinGamma;
    double z_n = factor * _camera.znear();
    double d = max.y - min.y; //perspective transform depth //light space y extents
    double z_f = z_n + d * sinGamma;
    double n = (z_n + CSMath::sqrt(z_f * z_n)) / sinGamma;
    double f = n + d;

    //var pos = _camera.Position - up * (n - _camera.Near);
    CSVector3 pos = _camera.position() - up * CSMath::max((float)(n - _camera.znear()), -min.y + 1);

    CSMatrix::lookAtRH(pos, pos + _light.direction, up, lightView);

    double a = (f + n) / (f - n);
    double b = -2 * f * n / (f - n);

    CSMatrix lspmtx = CSMatrix::Identity;
    lspmtx.m22 = a;      // [ 1 0 0 0] 
    lspmtx.m42 = b;      // [ 0 a 0 1]
    lspmtx.m24 = 1;      // [ 0 0 1 0]
    lspmtx.m44 = 0;      // [ 0 b 0 0]

    CSMatrix lightViewProj = lightView * lspmtx;

    getAABB(points, lightViewProj, min, max);

    lightViewProj *= CSMatrix::orthoOffCenterLH(min.x, max.x, max.y, min.y, max.z, min.z);     //LH, 컬링

    _viewProjection[0] = lightViewProj;
}

const CSArray<CSVector3, 2>* CSDirectionalShadowMap::frustumObject() {
    CSMatrix inv;
    CSMatrix::invert(_camera.viewProjection(), inv);

    CSVector3 p[8];
    CSVector3::transformCoordinate(CSVector3(1, -1, -1), inv, p[0]);
    CSVector3::transformCoordinate(CSVector3(-1, -1, -1), inv, p[1]);
    CSVector3::transformCoordinate(CSVector3(-1, 1, -1), inv, p[2]);
    CSVector3::transformCoordinate(CSVector3(1, 1, -1), inv, p[3]);
    CSVector3::transformCoordinate(CSVector3(1, -1, 1), inv, p[4]);
    CSVector3::transformCoordinate(CSVector3(-1, -1, 1), inv, p[5]);
    CSVector3::transformCoordinate(CSVector3(-1, 1, 1), inv, p[6]);
    CSVector3::transformCoordinate(CSVector3(1, 1, 1), inv, p[7]);

    CSArray<CSVector3, 2>* obj = CSArray<CSVector3, 2>::arrayWithCapacity(6);

    for (int i = 0; i < 6; i++) {
        CSArray<CSVector3>* subobj = new CSArray<CSVector3>(4);
        obj->addObject(subobj);
        subobj->release();
    }
    //near poly ccw
    CSArray<CSVector3>* ps = obj->objectAtIndex(0);
    for (int i = 0; i < 4; i++) {
        ps->addObject(p[i]);
    }
    //far poly ccw
    ps = obj->objectAtIndex(1);
    for (int i = 4; i < 8; i++) {
        ps->addObject(p[11 - i]);
    }
    //left poly ccw
    ps = obj->objectAtIndex(2);
    ps->addObject(p[0]);
    ps->addObject(p[3]);
    ps->addObject(p[7]);
    ps->addObject(p[4]);
    //right poly ccw
    ps = obj->objectAtIndex(3);
    ps->addObject(p[1]);
    ps->addObject(p[5]);
    ps->addObject(p[6]);
    ps->addObject(p[2]);
    //bottom poly ccw
    ps = obj->objectAtIndex(4);
    ps->addObject(p[4]);
    ps->addObject(p[5]);
    ps->addObject(p[1]);
    ps->addObject(p[0]);
    //top poly ccw
    ps = obj->objectAtIndex(5);
    ps->addObject(p[6]);
    ps->addObject(p[7]);
    ps->addObject(p[3]);
    ps->addObject(p[2]);

    return obj;
}

const CSArray<CSVector3, 2>* CSDirectionalShadowMap::clipObjectByScene(const CSArray<CSVector3, 2>* obj) {
    CSPlane planes[] = {
        CSPlane(-CSVector3::UnitY, _space.minimum.y),
        CSPlane(CSVector3::UnitY, -_space.maximum.y),
        CSPlane(-CSVector3::UnitX, _space.minimum.x),
        CSPlane(CSVector3::UnitX, -_space.maximum.x),
        CSPlane(-CSVector3::UnitZ, _space.minimum.z),
        CSPlane(CSVector3::UnitZ, -_space.maximum.z),
    };

    for (int i = 0; i < 6; i++) obj = clipObjectByPlane(obj, planes[i]);

    return obj;
}

const CSArray<CSVector3>* CSDirectionalShadowMap::objectLightVolumePoints(const CSArray<CSVector3, 2>* obj) {
    CSArray<CSVector3>* points = CSArray<CSVector3>::arrayWithCapacity(256);

    foreach (const CSArray<CSVector3>*, objp, obj) points->addObjectsFromArray(objp);

    int count = points->count();
    CSVector3 ld = -_light.direction;
    for (int i = 0; i < count; i++) {
        CSVector3 pt;
        if (lineIntersectsBox(points->objectAtIndex(i), ld, _space, pt)) points->addObject(pt);
    }
    return points;
}

static void getAABB(const CSArray<CSVector3>* points, CSVector3& min, CSVector3& max) {
    min = max = points->objectAtIndex(0);

    for (int i = 1; i < points->count(); i++) {
        const CSVector3& p = points->objectAtIndex(i);
        if (p.x < min.x) min.x = p.x;
        if (p.y < min.y) min.y = p.y;
        if (p.z < min.z) min.z = p.z;
        if (p.x > max.x) max.x = p.x;
        if (p.y > max.y) max.y = p.y;
        if (p.z > max.z) max.z = p.z;
    }
}

static void getAABB(const CSArray<CSVector3>* points, const CSMatrix& view, CSVector3& min, CSVector3& max) {
    CSVector3 p = CSVector3::transformCoordinate(points->objectAtIndex(0), view);
    min = max = p;

    for (int i = 1; i < points->count(); i++) {
        CSVector3::transformCoordinate(points->objectAtIndex(i), view, p);
        if (p.x < min.x) min.x = p.x;
        if (p.y < min.y) min.y = p.y;
        if (p.z < min.z) min.z = p.z;
        if (p.x > max.x) max.x = p.x;
        if (p.y > max.y) max.y = p.y;
        if (p.z > max.z) max.z = p.z;
    }
}

static const CSArray<CSVector3, 2>* clipObjectByPlane(const CSArray<CSVector3, 2>* obj, const CSPlane& plane) {
    CSArray<CSVector3, 2>* inter = CSArray<CSVector3, 2>::arrayWithCapacity(obj->count());
    CSArray<CSVector3, 2>* result = CSArray<CSVector3, 2>::arrayWithCapacity(obj->count());

    foreach (const CSArray<CSVector3>*, objp, obj) {
        CSArray<CSVector3>* interp = CSArray<CSVector3>::arrayWithCapacity(objp->count());
        CSArray<CSVector3>* resultp = CSArray<CSVector3>::arrayWithCapacity(objp->count());

        clipPointsByPlane(objp, plane, resultp, interp);

        if (resultp->count()) {
            result->addObject(resultp);
            inter->addObject(interp);
        }
    }
    appendIntersectionPoints(result, inter);

    return result;
}

static void clipPointsByPlane(const CSArray<CSVector3>* objp, const CSPlane& plane, CSArray<CSVector3>* resultp, CSArray<CSVector3>* interp) {
    if (objp->count() < 3) return;

    bool* outside = (bool*)alloca(objp->count());
    for (int i = 0; i < objp->count(); i++) {
        outside[i] = plane.intersects(objp->objectAtIndex(i)) == CSCollisionResultFront;
    }
    for (int i = 0; i < objp->count(); i++) {
        int next = (i + 1) % objp->count();

        if (outside[i] && outside[next]) continue;

        const CSVector3& p0 = objp->objectAtIndex(i);
        const CSVector3& p1 = objp->objectAtIndex(next);

        if (outside[i] || outside[next]) {
            CSVector3 diff = p1 - p0;
            float length = diff.length();
            diff /= length;

            CSRay ray(p0, diff);

            float distance;
            if (ray.intersects(plane, distance) && distance <= length) {
                CSVector3 inter = p0 + diff * distance;

                resultp->addObject(inter);
                interp->addObject(inter);
            }
            if (outside[next]) continue;
        }
        resultp->addObject(p1);
    }
}

static void appendIntersectionPoints(CSArray<CSVector3, 2>* obj, CSArray<CSVector3, 2>* inter) {
    if (inter->count() < 3) return;

    int i;
    for (i = inter->count(); i > 0; i--) {
        if (inter->objectAtIndex(i - 1)->count() == 2) break;
    }
    while (inter->count() > i) inter->removeLastObject();

    if (inter->count() < 3) return;

    CSArray<CSVector3>* objp = new CSArray<CSVector3>(inter->count() + 1);
    obj->addObject(objp);
    objp->release();

    CSArray<CSVector3>* interp = inter->lastObject();
    objp->addObject(interp->objectAtIndex(0));
    objp->addObject(interp->objectAtIndex(1));

    inter->removeLastObject();

    while (inter->count() > 0) {
        const CSVector3& lastp = objp->lastObject();

        int nr = findSamePointInObjectAndSwapWithLast(inter, lastp);

        if (nr >= 0) {
            interp = inter->lastObject();

            objp->addObject(interp->objectAtIndex((nr + 1) % 2));
        }
        inter->removeLastObject();
    }

    objp->removeLastObject();
}

static float relativeEpsilon(float a, float epsilon) {
    return CSMath::max(CSMath::abs(a * epsilon), epsilon);
}

static bool nearEqual(float a, float b) {
    const float Epsilon = 0.001f;

    float releps = relativeEpsilon(a, Epsilon);

    return a - releps <= b && b <= a + releps;
}

static int findSamePointInVecPoint(CSArray<CSVector3>* poly, const CSVector3& p) {
    for (int i = 0; i < poly->count(); i++) {
        const CSVector3& pp = poly->objectAtIndex(i);
        if (nearEqual(pp.x, p.x) && nearEqual(pp.y, p.y) && nearEqual(pp.z, p.z)) return i;
    }
    return -1;
}

static int findSamePointInObjectAndSwapWithLast(CSArray<CSVector3, 2>* inter, const CSVector3& p) {
    for (int i = (int)inter->count() - 1; i >= 0; i--) {
        CSArray<CSVector3>* interp = inter->objectAtIndex(i);

        if (interp->count() == 2) {
            int nr = findSamePointInVecPoint(interp, p);

            if (nr >= 0) {
                interp->retain();
                inter->setObject(i, inter->lastObject());
                inter->setObject(inter->count() - 1, interp);
                interp->release();
                return nr;
            }
        }
    }
    return -1;
}


static bool clipTest(float p, float q, float& u1, float& u2) {
    if (p < 0) {
        float r = q / p;
        if (r > u2) return false;
        else {
            if (r > u1) u1 = r;
            return true;
        }
    }
    else if (p > 0) {
        float r = q / p;
        if (r < u1) return false;
        else {
            if (r < u2) u2 = r;
            return true;
        }
    }
    else {
        return q >= 0;
    }
}

static bool lineIntersectsBox(const CSVector3& p, const CSVector3& dir, const CSABoundingBox& b, CSVector3& v) {
    float t1 = 0.0f;
    float t2 = FloatMax;

    bool intersect =
        clipTest(-dir.z, p.z - b.minimum.z, t1, t2) && clipTest(dir.z, b.maximum.z - p.z, t1, t2) &&
        clipTest(-dir.y, p.y - b.minimum.y, t1, t2) && clipTest(dir.y, b.maximum.y - p.y, t1, t2) &&
        clipTest(-dir.x, p.x - b.minimum.x, t1, t2) && clipTest(dir.x, b.maximum.x - p.x, t1, t2);

    v = p;

    if (!intersect) return false;

    intersect = false;

    if (t1 >= 0) {
        v += dir * t1;
        intersect = true;
    }
    if (t2 >= 0) {
        v += dir * t2;
        intersect = true;
    }

    return intersect;
}
