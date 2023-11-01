#define CDK_IMPL

#include "CSLightSpace.h"

#include "CSPointShadowMap.h"

#include "CSGraphics.h"

void CSLightSpace::disposePointLight() {
    CSObject::release(_pointLightBuffer);
    CSObject::release(_pointLightClusterMap);
    for (int i = 0; i < MaxPointShadowCount; i++) {
        if (_pointShadowMaps[i]) delete _pointShadowMaps[i];
    }
}

void CSLightSpace::setPointLight(const CSPointLight& light) {
    CSAssert(_updating);

    synchronized(_lock) {
        _lightUpdateKeys.addObject(light.key);

        for (int i = 0; i < _pointLights.count(); i++) {
            const CSPointLight& prev = _pointLights.objectAtIndex(i);

            if (prev.key == light.key) {
                if (prev != light) {
                    _pointLights.objectAtIndex(i) = light;
                    _pointLightUpdated = true;
                }
                return;
            }
        }
        if (_pointLights.count() < MaxPointLightCount) {
            _pointLights.addObject(light);
            _pointLightUpdated = true;
        }
    }
}

bool CSLightSpace::beginPointShadow(CSGraphics* graphics) {
    const CSCamera& camera = graphics->camera();

    if (_pointShadowCursor == -1) {
        _pointShadowCount = 0;

        if (allowShadow) {
            for (int i = 0; i < _pointLights.count(); i++) {
                const CSPointLight& light = _pointLights.objectAtIndex(i);

                if (light.castShadow) {
                    if (_pointShadowCount < MaxPointShadowCount) {
                        _pointShadowLightIndices[_pointShadowCount++] = i;
                    }
                    else {
                        float dist = CSVector3::distance(camera.position(), light.position) - light.range();

                        int index = -1;
                        for (int j = 0; j < MaxPointShadowCount; j++) {
                            const CSPointLight& other = _pointLights.objectAtIndex(_pointShadowLightIndices[j]);

                            float d = CSVector3::distance(camera.position(), other.position) - other.range();

                            if (dist < d) {
                                index = j;
                                dist = d;
                            }
                        }
                        if (index != -1) _pointShadowLightIndices[index] = i;
                    }
                }
            }
        }
        for (int i = _pointShadowCount; i < MaxPointShadowCount; i++) {
            if (_pointShadowMaps[i]) {
                delete _pointShadowMaps[i];
                _pointShadowMaps[i] = NULL;
            }
        }

        if (_pointShadowCount == 0) return false;

        _pointShadowCursor = 0;
    }
    else if (_pointShadowCursor >= _pointShadowCount) {
        _pointShadowCursor = -1;
        return false;
    }

    {
        CSPointLight light = _pointLights.objectAtIndex(_pointShadowLightIndices[_pointShadowCursor]);
        float range = light.range();
        if (!allowShadowPixel32) light.shadow.pixel32 = false;
        if (light.shadow.resolution > maxShadowResolution) light.shadow.resolution = maxShadowResolution;
        while ((light.shadow.resolution >> 1) >= range) light.shadow.resolution >>= 1;
        if (!_pointShadowMaps[_pointShadowCursor]) _pointShadowMaps[_pointShadowCursor] = new CSPointShadowMap();
        _pointShadowMaps[_pointShadowCursor]->begin(graphics, light, range);
        return true;
    }
}

void CSLightSpace::endPointShadow(CSGraphics* graphics) {
    _pointShadowMaps[_pointShadowCursor]->end(graphics);
    _pointShadowCursor++;
}

struct PointLightData {
    CSVector3 position;
    float pad0;
    CSColor3 color;
    float pad1;
    CSVector4 attenuation;
    int shadowMapIndex;
    float shadowBias;
    float shadowBleeding;
    float pad2;
};

void CSLightSpace::updatePointLights(bool viewUpdated) {
    if (_pointLightUpdated || viewUpdated) {
        if (_pointLights.count()) {
            if (!_pointLightBuffer) _pointLightBuffer = new CSGBuffer(CSGBufferTargetUniform);
            if (!_pointLightClusterMap) {
                CSTextureDescription desc;
                desc.target = CSTextureTarget3D;
                desc.width = Cluster;
                desc.height = Cluster;
                desc.depth = Cluster;
                desc.format = CSRawFormat::Rgba8ui;
                _pointLightClusterMap = new CSTexture(desc, false);
            }
            PointLightData* lightData = (PointLightData*)alloca(_pointLights.count() * sizeof(PointLightData));
            for (int i = 0; i < _pointLights.count(); i++) {
                PointLightData& data = lightData[i];
                const CSPointLight& light = _pointLights.objectAtIndex(i);

                data.position = light.position;
                data.color = light.color;
                data.attenuation = CSVector4(light.range(), light.attenuation.constant, light.attenuation.linear, light.attenuation.quadratic);
                data.shadowMapIndex = -1;
                for (int j = 0; j < _pointShadowCount; j++) {
                    if (_pointShadowLightIndices[j] == i) {
                        data.shadowMapIndex = j;
                        break;
                    }
                }
                data.shadowBias = light.shadow.bias;
                data.shadowBleeding = light.shadow.bleeding;
            }
            _pointLightBuffer->upload(lightData, _pointLights.count(), CSGBufferUsageHintDynamicDraw);

            updatePointLightCluster();
        }
        else {
            CSObject::release(_pointLightBuffer);
            CSObject::release(_pointLightClusterMap);
            _pointLightVisible = false;
        }
        _pointLightUpdated = false;
    }
}

void CSLightSpace::updatePointLightCluster() {
    byte* clusters = (byte*)fcalloc(Cluster * Cluster * Cluster, 4);
    float* clusterAtts = (float*)fcalloc(Cluster * Cluster * Cluster * 3, sizeof(float));

    _pointLightVisible = false;

    for (int li = 0; li < _pointLights.count(); li++) {
        const CSPointLight& light = _pointLights.objectAtIndex(li);
        float range = light.range();

        CSVector3 min(FloatMax);
        CSVector3 max(FloatMin);

        CSVector3 corners[8];
        CSABoundingBox::getCorners(light.position - range, light.position + range, corners);
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
        float range2 = range * range;

        for (int x = minx; x <= maxx; x++) {
            for (int y = miny; y <= maxy; y++) {
                for (int z = minz; z <= maxz; z++) {
                    CSVector3 wp = clusterGridToWorld(x, y, z, lcp);
                    float d2 = CSVector3::distanceSquared(wp, light.position);

                    if (d2 < range2) {
                        int ci = z * (Cluster * Cluster) + y * Cluster + x;
                        int ci3 = ci * 3;
                        int ci4 = ci * 4;

                        int clusterLightCount = clusters[ci4];

                        float att = light.color.brightness();
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

                        _pointLightVisible = true;
                    }
                }
            }
        }
    }

    if (_pointLightVisible) _pointLightClusterMap->upload(clusters, Cluster * Cluster * Cluster * 4, 0, Cluster, Cluster, Cluster);

    free(clusters);
    free(clusterAtts);
}