#define CDK_IMPL

#include "CSLightSpace.h"

#include "CSDirectionalShadowMap.h"

#include "CSGraphics.h"

void CSLightSpace::disposeDirectionalLight() {
    CSObject::release(_directionalLightBuffer);
    for (int i = 0; i < MaxDirectionalLightCount; i++) {
        if (_directionalShadowMaps[i]) delete _directionalShadowMaps[i];
    }
}

void CSLightSpace::setDirectionalLight(const CSDirectionalLight& light) {
    CSAssert(_updating);

    synchronized(_lock) {
        _lightUpdateKeys.addObject(light.key);

        for (int i = 0; i < _directionalLights.count(); i++) {
            const CSDirectionalLight& prev = _directionalLights.objectAtIndex(i);

            if (prev.key == light.key) {
                if (prev != light) {
                    _directionalLights.objectAtIndex(i) = light;
                    _directionalLightUpdated = true;
                }
                return;
            }
        }
        if (_directionalLights.count() < MaxDirectionalLightCount) {
            _directionalLights.addObject(light);
            _directionalLightUpdated = true;
        }
    }
}

bool CSLightSpace::beginDirectionalShadow(CSGraphics* graphics, bool& shadow2D) {
    if (!allowShadow) {
        for (int i = 0; i < MaxDirectionalLightCount; i++) {
            if (_directionalShadowMaps[i]) {
                delete _directionalShadowMaps[i];
                _directionalShadowMaps[i] = NULL;
            }
        }
        _directionalShadowCursor = 0;
        shadow2D = false;
        return false;
    }
    while (_directionalShadowCursor < _directionalLights.count() * 2) {
        int lightIndex = _directionalShadowCursor >> 1;
        shadow2D = (_directionalShadowCursor & 1) == 1;

        CSDirectionalLight light = _directionalLights.objectAtIndex(lightIndex);

        if (shadow2D ? light.castShadow2D : light.castShadow) {
            if (!allowShadowPixel32) light.shadow.pixel32 = false;
            if (light.shadow.resolution > maxShadowResolution) light.shadow.resolution = maxShadowResolution;
            if (!_directionalShadowMaps[lightIndex]) _directionalShadowMaps[lightIndex] = new CSDirectionalShadowMap();
            if (_directionalShadowMaps[lightIndex]->begin(graphics, light, space, shadow2D)) return true;
        }
        else if (_directionalShadowMaps[lightIndex]) {
            if (!shadow2D && !light.castShadow2D) {
                delete _directionalShadowMaps[lightIndex];      //both false
                _directionalShadowMaps[lightIndex] = NULL;
            }
            else _directionalShadowMaps[lightIndex]->clear(shadow2D);
        }

        _directionalShadowCursor++;
    }
    for (int i = _directionalLights.count(); i < MaxDirectionalLightCount; i++) {
        if (_directionalShadowMaps[i]) {
            delete _directionalShadowMaps[i];
            _directionalShadowMaps[i] = NULL;
        }
    }
    _directionalShadowCursor = 0;
    shadow2D = false;
    return false;
}

void CSLightSpace::endDirectionalShadow(CSGraphics* graphics) {
    int lightIndex = _directionalShadowCursor >> 1;
    bool shadow2D = (_directionalShadowCursor & 1) == 1;
    _directionalShadowMaps[lightIndex]->end(graphics, shadow2D);

    _directionalShadowCursor++;
}

struct DirectionalLightData {
    CSVector3 direction;
    float pad0;
    CSColor3 color;
    float pad1;
    CSMatrix shadowViewProjection;
    CSMatrix shadow2DViewProjection;
    int shadowMapIndex;
    int shadow2DMapIndex;
    float shadowBias;
    float shadowBleeding;
};

void CSLightSpace::updateDirectionalLights(bool viewUpdated) {
    if (_directionalLightUpdated || viewUpdated) {
        if (_directionalLights.count()) {
            if (!_directionalLightBuffer) _directionalLightBuffer = new CSGBuffer(CSGBufferTargetUniform);

            DirectionalLightData* lightData = (DirectionalLightData*)alloca(_directionalLights.count() * sizeof(DirectionalLightData));

            for (int i = 0; i < _directionalLights.count(); i++) {
                DirectionalLightData& data = lightData[i];

                const CSDirectionalLight& light = _directionalLights.objectAtIndex(i);

                data.direction = light.direction;
                data.color = light.color;
                data.shadowMapIndex = -1;
                data.shadow2DMapIndex = -1;
                data.shadowBias = light.shadow.bias;
                data.shadowBleeding = light.shadow.bleeding;

                if (_directionalShadowMaps[i] && _directionalShadowMaps[i]->visible()) {
                    if (light.castShadow) {
                        data.shadowMapIndex = i;
                        data.shadowViewProjection = _directionalShadowMaps[i]->viewProjection(false);
                    }
                    if (light.castShadow2D) {
                        data.shadowMapIndex = i;
                        data.shadow2DViewProjection = _directionalShadowMaps[i]->viewProjection(true);
                    }
                }
            }
            _directionalLightBuffer->upload(lightData, _directionalLights.count(), CSGBufferUsageHintDynamicDraw);
        }
        else {
            CSObject::release(_directionalLightBuffer);
        }
        _directionalLightUpdated = false;
    }
}