#define CDK_IMPL

#include "CSStandardRenderer.h"

#include "CSFile.h"

#include "CSRenderState.h"
#include "CSShaderCode.h"

CSStandardRenderer::CSStandardRenderer() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/standard_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/standard_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, CSShaderCode::VSBase, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::FSBase, fragmentShaderCode);
}

bool CSStandardRenderer::visible(const CSRenderState& state) const {
    return state.material.shader != CSMaterial::ShaderDistortion;
}

void CSStandardRenderer::validate(CSRenderState& state) const {
    bool usingLight = state.material.receiveLight() && state.lightSpaceState;
    bool usingStroke = state.usingStroke();
    bool usingFog = state.usingFog();

    if (state.stencilMode != CSStencilNone) {
        state.material.bloom = false;
        if (state.material.blendMode > CSBlendAlpha) state.material.blendMode = CSBlendAlpha;
        state.material.emission = CSColor3::Black;
        usingLight = false;
        usingStroke = false;
        usingFog = false;
        state.brightness = 0;
        state.contrast = 1;
        state.saturation = 1;
    }

    if (!usingLight) {
        if (state.material.shader == CSMaterial::ShaderLight) state.material.shader = CSMaterial::ShaderNoLight;
        if (state.material.displacementScale == 0) state.material.normalMap = NULL;
        state.material.materialMap = NULL;
        state.material.materialMapComponents = 0;
        state.material.reflection = false;
        state.material.metallic = 0;
        state.material.roughness = 0;
        state.material.ambientOcclusion = 0;
        state.material.rim = 0;
        state.material.receiveShadow = false;
        state.material.receiveShadow2D = false;
        state.lightSpaceState = NULL;
    }
    else {
        if (!state.material.materialMap) state.material.materialMapComponents = 0;
        else if (state.material.materialMapComponents == 0) state.material.materialMap = NULL;

        if (!state.lightSpaceState->envMap) state.material.reflection = false;

        switch (state.lightSpaceState->mode) {
            case CSLightBlinn:
            case CSLightPhong:
                state.material.rim = 0;
                break;
        }
    }

    state.material.distortionScale = 0;

    if (!state.material.normalMap) state.material.displacementScale = 0;

    if (!state.usingMap()) state.material.uvOffset = CSVector2::Zero;

    if (!state.target->bloomSupported()) state.material.bloom = false;
    if (!state.material.bloom) state.bloomThreshold = 1;

    if (state.depthMode == CSDepthNone || !state.material.depthTest) {
        state.material.depthTest = false;
        state.material.depthBias = 0;
    }

    if (!state.material.alphaTest) state.material.alphaTestBias = 0;

    if (!usingFog) {
        state.fogColor = CSColor3::Black;
        state.fogNear = 0;
        state.fogFar = 0;
    }

    if (!usingStroke) {
        state.strokeWidth = 0;
        state.strokeColor = CSColor::Black;
        state.strokeMode = CSStrokeNormal;
    }
}

bool CSStandardRenderer::clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const {
    bounds = CSRect::ScreenNone;

    CSABoundingBox box = CSABoundingBox::None;
    const CSMatrix& vp = state.camera.viewProjection();
    for (int i = 0; i < count; i++) box.append(points[i], vp);
    if (box.intersects(CSABoundingBox::ViewSpace, CSCollisionFlagNone) != CSCollisionResultFront) {
        bounds.append(CSVector2(box.minimum.x, box.minimum.y));
        bounds.append(CSVector2(box.maximum.x, box.minimum.y));
        bounds.append(CSVector2(box.minimum.x, box.maximum.y));
        bounds.append(CSVector2(box.maximum.x, box.maximum.y));
        return true;
    }
    return false;
}

void CSStandardRenderer::setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) {
    state.applyBranch(&_programs, boneBuffer != NULL, usingInstance, usingVertexColor);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api, boneBuffer);
}