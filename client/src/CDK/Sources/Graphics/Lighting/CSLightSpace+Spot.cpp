#define CDK_IMPL

#include "CSLightSpace.h"

#include "CSRay.h"
#include "CSSegment.h"
#include "CSTriangle.h"
#include "CSABoundingBox.h"

#include "CSSpotShadowMap.h"

#include "CSGraphics.h"

void CSLightSpace::disposeSpotLight() {
    CSObject::release(_spotLightBuffer);
    CSObject::release(_spotLightClusterMap);
    CSObject::release(_spotShadowBuffer);
    for (int i = 0; i < MaxSpotShadowCount; i++) {
        if (_spotShadowMaps[i]) delete _spotShadowMaps[i];
    }
}

void CSLightSpace::setSpotLight(const CSSpotLight& light) {
    CSAssert(_updating);

    synchronized(_lock) {
        _lightUpdateKeys.addObject(light.key);

        for (int i = 0; i < _spotLights.count(); i++) {
            const CSSpotLight& prev = _spotLights.objectAtIndex(i);

            if (prev.key == light.key) {
                if (prev != light) {
                    _spotLights.objectAtIndex(i) = light;
                    _spotLightUpdated = true;
                }
                return;
            }
        }
        if (_spotLights.count() < MaxSpotLightCount) {
            _spotLights.addObject(light);
            _spotLightUpdated = true;
        }
    }
}

bool CSLightSpace::beginSpotShadow(CSGraphics* graphics) {
    const CSCamera& camera = graphics->camera();

    if (_spotShadowCursor == -1) {
        _spotShadowCount = 0;

        if (allowShadow) {
            for (int i = 0; i < _spotLights.count(); i++) {
                const CSSpotLight& light = _spotLights.objectAtIndex(i);

                if (light.castShadow) {
                    if (_spotShadowCount < MaxSpotShadowCount) {
                        _spotShadowLightIndices[_spotShadowCount++] = i;
                    }
                    else {
                        float dist = CSVector3::distance(camera.position(), light.position) - light.range();

                        int index = -1;
                        for (int j = 0; j < MaxSpotShadowCount; j++) {
                            const CSSpotLight& other = _spotLights.objectAtIndex(_spotShadowLightIndices[j]);

                            float d = CSVector3::distance(camera.position(), other.position) - other.range();

                            if (dist < d) {
                                index = j;
                                dist = d;
                            }
                        }
                        if (index != -1) _spotShadowLightIndices[index] = i;
                    }
                }
            }
        }
        for (int i = _spotShadowCount; i < MaxSpotShadowCount; i++) {
            if (_spotShadowMaps[i]) {
                delete _spotShadowMaps[i];
                _spotShadowMaps[i] = NULL;
            }
        }
        if (_spotShadowCount == 0) return false;

        _spotShadowCursor = 0;
    }
    else if (_spotShadowCursor >= _spotShadowCount) {
        _spotShadowCursor = -1;
        return false;
    }

    {
        CSSpotLight light = _spotLights.objectAtIndex(_spotShadowLightIndices[_spotShadowCursor]);
        float range = light.range();
        if (!allowShadowPixel32) light.shadow.pixel32 = false;
        if (light.shadow.resolution > maxShadowResolution) light.shadow.resolution = maxShadowResolution;
        int maxResolution = range * CSMath::tan(light.angle * 0.5f + light.dispersion) * 2;
        while ((light.shadow.resolution >> 1) >= maxResolution) light.shadow.resolution >>= 1;
        if (!_spotShadowMaps[_spotShadowCursor]) _spotShadowMaps[_spotShadowCursor] = new CSSpotShadowMap();
        _spotShadowMaps[_spotShadowCursor]->begin(graphics, light, range);
        return true;
    }
}

void CSLightSpace::endSpotShadow(CSGraphics* graphics) {
    _spotShadowMaps[_spotShadowCursor]->end(graphics);

    _spotShadowCursor++;
}

struct SpotLightData {
    CSVector3 position;
    float cutoff;
    CSVector3 direction;
    float epsilon;
    CSColor3 color;
    float pad0;
    CSVector4 attenuation;
    int shadowMapIndex;
    float shadowBias;
    float shadowBleeding;
    float pad1;
};

void CSLightSpace::updateSpotLights(bool viewUpdated) {
    if (_spotLightUpdated || viewUpdated) {
        if (_spotLights.count()) {
            if (!_spotLightBuffer) _spotLightBuffer = new CSGBuffer(CSGBufferTargetUniform);
            if (!_spotLightClusterMap) {
                CSTextureDescription desc;
                desc.target = CSTextureTarget3D;
                desc.width = Cluster;
                desc.height = Cluster;
                desc.depth = Cluster;
                desc.format = CSRawFormat::Rgba8ui;
                _spotLightClusterMap = new CSTexture(desc, false);
            }
            SpotLightData* lightData = (SpotLightData*)alloca(_spotLights.count() * sizeof(SpotLightData));
            for (int i = 0; i < _spotLights.count(); i++) {
                SpotLightData& data = lightData[i];
                const CSSpotLight& light = _spotLights.objectAtIndex(i);

                float halfAngle = light.angle * 0.5f;
                float cutoff = CSMath::cos(halfAngle + light.dispersion);
                float epsilon = CSMath::cos(halfAngle) - cutoff;

                data.position = light.position;
                data.direction = light.direction;
                data.cutoff = cutoff;
                data.epsilon = epsilon;
                data.color = light.color;
                data.attenuation = CSVector4(light.range(), light.attenuation.constant, light.attenuation.linear, light.attenuation.quadratic);
                data.shadowMapIndex = -1;
                for (int j = 0; j < _spotShadowCount; j++) {
                    if (_spotShadowLightIndices[j] == i) {
                        data.shadowMapIndex = j;
                        break;
                    }
                }
                data.shadowBias = light.shadow.bias;
                data.shadowBleeding = light.shadow.bleeding;
            }
            _spotLightBuffer->upload(lightData, _spotLights.count(), CSGBufferUsageHintDynamicDraw);

            if (_spotShadowCount) {
                if (!_spotShadowBuffer) _spotShadowBuffer = new CSGBuffer(CSGBufferTargetUniform);
                CSMatrix* viewProjections = (CSMatrix*)alloca(_spotShadowCount * sizeof(CSMatrix));
                for (int i = 0; i < _spotShadowCount; i++) viewProjections[i] = _spotShadowMaps[i]->viewProjection();
                _spotShadowBuffer->upload(viewProjections, _spotShadowCount, CSGBufferUsageHintDynamicDraw);
            }
            else {
                CSObject::release(_spotShadowBuffer);
            }
            updateSpotLightCluster();
        }
        else {
            CSObject::release(_spotLightBuffer);
            CSObject::release(_spotLightClusterMap);
            CSObject::release(_spotShadowBuffer);
            _spotLightVisible = false;
        }
        _spotLightUpdated = false;
    }
}

bool CSLightSpace::intersectSpotLightCluster(const CSRay& ray, float range, float tanq, int x, int y, int z) {
    CSVector3 wp0 = clusterToWorld(CSVector3(x, y, z));
    CSVector3 wp1 = clusterToWorld(CSVector3(x + 1, y, z));
    CSVector3 wp2 = clusterToWorld(CSVector3(x, y + 1, z));
    CSVector3 wp3 = clusterToWorld(CSVector3(x + 1, y + 1, z));
    CSVector3 wp4 = clusterToWorld(CSVector3(x, y, z + 1));
    CSVector3 wp5 = clusterToWorld(CSVector3(x + 1, y, z + 1));
    CSVector3 wp6 = clusterToWorld(CSVector3(x, y + 1, z + 1));
    CSVector3 wp7 = clusterToWorld(CSVector3(x + 1, y + 1, z + 1));

    float rd;

    if (ray.intersects(CSTriangle(wp0, wp1, wp2), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp1, wp3, wp2), rd) && rd < range) return true;

    if (ray.intersects(CSTriangle(wp4, wp5, wp6), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp5, wp7, wp6), rd) && rd < range) return true;

    if (ray.intersects(CSTriangle(wp4, wp0, wp6), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp0, wp2, wp6), rd) && rd < range) return true;

    if (ray.intersects(CSTriangle(wp4, wp5, wp0), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp5, wp1, wp0), rd) && rd < range) return true;

    if (ray.intersects(CSTriangle(wp5, wp1, wp7), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp1, wp3, wp7), rd) && rd < range) return true;

    if (ray.intersects(CSTriangle(wp6, wp7, wp2), rd) && rd < range) return true;
    if (ray.intersects(CSTriangle(wp7, wp3, wp2), rd) && rd < range) return true;

    CSVector3 n;
    
    ray.intersects(CSSegment(wp0, wp1), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp0, wp2), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp1, wp3), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp2, wp3), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp4, wp5), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp4, wp6), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp5, wp7), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp6, wp7), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp0, wp4), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp1, wp5), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp2, wp6), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;
    ray.intersects(CSSegment(wp3, wp7), rd, n);
    if (rd >= 0 && rd < range && CSVector3::distance(ray.position + ray.direction * rd, n) < rd * tanq) return true;

    return false;
}



void CSLightSpace::updateSpotLightCluster() {
    byte* clusters = (byte*)fcalloc(Cluster * Cluster * Cluster, 4);
    float* clusterAtts = (float*)fcalloc(Cluster * Cluster * Cluster * 3, sizeof(float));

    _spotLightVisible = false;

    CSPlane planes[3];
    int planeCount = 0;

    static const CSVector3 SpotAngleAxis[] = {
        CSVector3::UnitX,
        -CSVector3::UnitX,
        CSVector3::UnitY,
        -CSVector3::UnitY,
        CSVector3::UnitZ,
        -CSVector3::UnitZ
    };

    for (int li = 0; li < _spotLights.count(); li++) {
        const CSSpotLight& light = _spotLights.objectAtIndex(li);
        float range = light.range();
        float angle = light.angle * 0.5f + light.dispersion;

        CSABoundingBox box(light.position);

        for (int i = 0; i < 6; i++) {
            const CSVector3& axis = SpotAngleAxis[i];

            CSRay rayrot(light.position, CSVector3::transform(light.direction, CSQuaternion::rotationAxis(axis, angle)));

            planeCount = 0;
            if (rayrot.direction.x < 0) planes[planeCount++] = CSPlane(CSVector3::UnitX, -space.minimum.x);
            else if (rayrot.direction.x > 0) planes[planeCount++] = CSPlane(-CSVector3::UnitX, -space.maximum.x);
            if (rayrot.direction.y < 0) planes[planeCount++] = CSPlane(CSVector3::UnitY, -space.minimum.y);
            else if (rayrot.direction.y > 0) planes[planeCount++] = CSPlane(-CSVector3::UnitY, -space.maximum.y);
            if (rayrot.direction.z < 0) planes[planeCount++] = CSPlane(CSVector3::UnitZ, -space.minimum.z);
            else if (rayrot.direction.z > 0) planes[planeCount++] = CSPlane(-CSVector3::UnitZ, -space.maximum.z);

            float distance = range;
            for (int j = 0; j < planeCount; j++) {
                float d;
                if (rayrot.intersects(planes[j], d) && d < distance) distance = d;
            }
            box.append(rayrot.position + rayrot.direction * distance);
        }

        CSVector3 min(FloatMax);
        CSVector3 max(FloatMin);

        CSVector3 corners[8];
        box.getCorners(corners);
        for (int i = 0; i < 8; i++) {
            CSVector3 cp = worldToCluster(corners[i]);
            CSVector3::min(min, cp, min);
            CSVector3::max(max, cp, max);
        }
        int minx = CSMath::max((int)min.x, 0);
        int maxx = CSMath::min((int)max.x, Cluster - 1);
        int miny = CSMath::max((int)min.y, 0);
        int maxy = CSMath::min((int)max.y, Cluster - 1);
        int minz = CSMath::max((int)min.z, 0);
        int maxz = CSMath::min((int)max.z, Cluster - 1);

        CSVector3 lcp = worldToCluster(light.position);

        float tanq = CSMath::tan(angle);

        CSRay ray(light.position, light.direction);

        for (int x = minx; x <= maxx; x++) {
            for (int y = miny; y <= maxy; y++) {
                for (int z = minz; z <= maxz; z++) {
                    if (!intersectSpotLightCluster(ray, range, tanq, x, y, z)) continue;

                    CSVector3 wp = clusterGridToWorld(x, y, z, lcp);

                    float d2 = CSVector3::distanceSquared(wp, light.position);

                    int ci = z * (Cluster * Cluster) + y * Cluster + x;
                    int ci3 = ci * 3;
                    int ci4 = ci * 4;

                    int clusterLightCount = clusters[ci4];

                    float att = light.color.brightness() * FloatTwoPi / light.angle;
                    att /= (light.attenuation.constant + light.attenuation.linear * CSMath::sqrt(d2) + light.attenuation.quadratic * d2);

                    if (clusterLightCount < 3) {
                        clusterAtts[ci3 + clusterLightCount] = att;

                        clusters[ci4] = ++clusterLightCount;
                        clusters[ci4 + clusterLightCount] = (byte)li;
                    }
                    else {
                        for (int i = 0; i < 3; i++) {
                            if (att > clusterAtts[ci3 + i]) {
                                clusterAtts[ci3 + i] = att;
                                clusters[ci4 + 1 + i] = (byte)li;
                                break;
                            }
                        }
                    }

                    _spotLightVisible = true;
                }
            }
        }
    }

    if (_spotLightVisible) _spotLightClusterMap->upload(clusters, Cluster * Cluster * Cluster * 4, 0, Cluster, Cluster, Cluster);

    free(clusters);
    free(clusterAtts);
}
