#define CDK_IMPL

#include "CSLightSpaceState.h"

#include "CSGBuffers.h"

#include "CSDirectionalShadowMap.h"
#include "CSPointShadowMap.h"
#include "CSSpotShadowMap.h"

#include "CSGraphics.h"

CSLightSpace::CSLightSpace(const CSABoundingBox& space) : 
    space(space),
    _directionalLights(MaxDirectionalLightCount),
    _pointLights(MaxPointLightCount),
    _spotLights(MaxSpotLightCount)
{
}

CSLightSpace::~CSLightSpace() {
    disposeDirectionalLight();
    disposePointLight();
    disposeSpotLight();
}

void CSLightSpace::clear() {
    CSAssert(!_updating);

    _directionalLights.removeAllObjects();
    _directionalLightUpdated = true;
    _pointLights.removeAllObjects();
    _pointLightUpdated = true;
    _spotLights.removeAllObjects();
    _spotLightUpdated = true;
}

void CSLightSpace::beginUpdate() {
    CSAssert(!_updating);

    _updating = true;
}

void CSLightSpace::endUpdate() {
    CSAssert(_updating);

    _updating = false;

    int i = 0;
    while (i < _directionalLights.count()) {
        if (_lightUpdateKeys.containsObject(_directionalLights.objectAtIndex(i).key)) i++;
        else {
            _directionalLights.removeObjectAtIndex(i);
            _directionalLightUpdated = true;
        }
    }
    i = 0;
    while (i < _pointLights.count()) {
        if (_lightUpdateKeys.containsObject(_pointLights.objectAtIndex(i).key)) i++;
        else {
            _pointLights.removeObjectAtIndex(i);
            _pointLightUpdated = true;
        }
    }
    i = 0;
    while (i < _spotLights.count()) {
        if (_lightUpdateKeys.containsObject(_spotLights.objectAtIndex(i).key)) i++;
        else {
            _spotLights.removeObjectAtIndex(i);
            _spotLightUpdated = true;
        }
    }
    _lightUpdateKeys.removeAllObjects();
}

bool CSLightSpace::beginDraw(CSGraphics* graphics, CSInstanceLayer& layer) {
    switch (_cursor) {
        case 0:
            {
                bool shadow2D;
                if (beginDirectionalShadow(graphics, shadow2D)) {
                    layer = shadow2D ? CSInstanceLayerShadow2D : CSInstanceLayerShadow;
                    return true;
                }
            }
            _cursor = 1;
        case 1:
            if (beginPointShadow(graphics)) {
                layer = CSInstanceLayerShadow;
                return true;
            }
            _cursor = 2;
        case 2:
            if (beginSpotShadow(graphics)) {
                layer = CSInstanceLayerShadow;
                return true;
            }
            _cursor = 3;
        case 3:
            uploadState(graphics);
            _cursor = 0;
            break;
    }
    layer = CSInstanceLayerNone;
    return false;
}

void CSLightSpace::endDraw(CSGraphics* graphics) {
    switch (_cursor) {
        case 0:
            endDirectionalShadow(graphics);
            break;
        case 1:
            endPointShadow(graphics);
            break;
        case 2:
            endSpotShadow(graphics);
            break;
    }
}

struct LightStateUniformData {
    CSColor3 ambientLight;
    float clusterMaxDepth;
    CSColor3 envColor;
    int envMapMaxLod;
    int directionalLightCount;

    inline LightStateUniformData() {
        memset(this, 0, sizeof(LightStateUniformData));
    }

    uint hash() const {
        CSHash hash;
        hash.combine(ambientLight);
        hash.combine(clusterMaxDepth);
        hash.combine(envColor);
        hash.combine(envMapMaxLod);
        hash.combine(directionalLightCount);
        return hash;
    }

    inline bool operator ==(const LightStateUniformData& other) const {
        return memcmp(this, &other, sizeof(LightStateUniformData));
    }

    inline bool operator !=(const LightStateUniformData& other) const {
        return !(*this == other);
    }
};

void CSLightSpace::uploadState(CSGraphics* graphics) {
    CSAssert(mode != CSLightNone);

    bool viewUpdated = _first || _camera != graphics->camera();

    if (viewUpdated) {
        _camera = graphics->camera();
        const CSMatrix& viewProjection = _camera.viewProjection();
        CSMatrix::invert(viewProjection, _viewProjectionInv);
        _clusterMaxDepth = 0;

        CSVector3 spaceCorners[8];
        space.getCorners(spaceCorners);
        for (int i = 0; i < 8; i++) {
            CSVector4 vp = CSVector3::transform(spaceCorners[i], viewProjection);
            float d = CSMath::min(vp.w, _camera.zfar());
            if (d > _clusterMaxDepth) _clusterMaxDepth = vp.w;
        }
        _first = false;
    }
    updateDirectionalLights(viewUpdated);
    updatePointLights(viewUpdated);
    updateSpotLights(viewUpdated);

    CSLightSpaceState* state = new CSLightSpaceState();
    state->mode = mode;
    state->usingShadow = allowShadow;
    state->envMap = envMap;
    state->brdfMap = brdfMap;

    LightStateUniformData data;
    data.ambientLight = ambientLight;
    data.clusterMaxDepth = _clusterMaxDepth;
    data.envColor = envColor;
    data.directionalLightCount = _directionalLights.count();
    if (envMap) data.envMapMaxLod = envMap->description().mipmapCount - 1;
    state->lightBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);

    if (_directionalLights.count()) {
        state->usingDirectionalLight = true;
        state->directionalLightBuffer = _directionalLightBuffer;
        for (int i = 0; i < MaxDirectionalLightCount; i++) {
            if (_directionalShadowMaps[i] && _directionalShadowMaps[i]->visible()) {
                state->directionalShadowMaps[i][false] = _directionalShadowMaps[i]->texture(false);
                state->directionalShadowMaps[i][true] = _directionalShadowMaps[i]->texture(true);
            }
        }
    }
    if (_pointLightVisible) {
        state->usingPointLight = true;
        state->pointLightBuffer = _pointLightBuffer;
        state->pointLightClusterMap = _pointLightClusterMap;
        for (int i = 0; i < MaxPointShadowCount; i++) {
            if (_pointShadowMaps[i]) state->pointShadowMaps[i] = _pointShadowMaps[i]->texture();
        }
    }
    if (_spotLightVisible) {
        state->usingSpotLight = true;
        state->spotLightBuffer = _spotLightBuffer;
        state->spotLightClusterMap = _spotLightClusterMap;
        state->spotShadowBuffer = _spotShadowBuffer;
        for (int i = 0; i < MaxSpotShadowCount; i++) {
            if (_spotShadowMaps[i]) state->spotShadowMaps[i] = _spotShadowMaps[i]->texture();
        }
    }

    graphics->setLightSpaceState(state);
    state->release();

    state->flush(graphics);
}
