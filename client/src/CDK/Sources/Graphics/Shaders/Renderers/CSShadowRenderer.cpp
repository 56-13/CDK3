#define CDK_IMPL

#include "CSShadowRenderer.h"

#include "CSFile.h"

#include "CSGraphics.h"
#include "CSGBuffers.h"
#include "CSShaderCode.h"

CSShadowRenderer::Param::Param(Mode mode, const CSMatrix* viewProjections, CSGBuffer* uniformBuffer) :
    mode(mode),
    uniformBuffer(uniformBuffer) 
{
    int viewProjectionCount = mode == ModePoint ? 6 : 1;
    memcpy(this->viewProjections, viewProjections, viewProjectionCount * sizeof(CSMatrix));
}

//===================================================================================================

CSShadowRenderer::CSShadowRenderer() {
    string commonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow.glsl"));
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow_vs.glsl"));
    string geometryShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow_gs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, CSShaderCode::VSBase, commonShaderCode, vertexShaderCode);
    _programs.attach(CSShaderTypeGeometry, commonShaderCode, geometryShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::Base, commonShaderCode, fragmentShaderCode);
}

bool CSShadowRenderer::visible(const CSRenderState& state) const {
    return state.material.receiveLight();
}

void CSShadowRenderer::validate(CSRenderState& state) const {
    const Param* param = static_assert_cast<const Param*>(state.rendererParam.value());

    state.depthMode = CSDepthReadWrite;
    state.material.shader = CSMaterial::ShaderLight;
    state.material.blendMode = CSBlendNone;
    state.material.cullMode = param->mode == Param::ModePoint ? CSCullFront : CSCullBack;       //LH / RH Problem, Consider vulkan or metal later
    if (!state.material.alphaTest) state.material.colorMap = NULL;
    if (!state.material.normalMap) state.material.displacementScale = 0;
    else if (state.material.displacementScale == 0) state.material.normalMap = NULL;
    state.material.distortionScale = 0;
    state.material.materialMap = NULL;
    state.material.materialMapComponents = 0;
    state.material.metallic = 0;
    state.material.roughness = 0;
    state.material.ambientOcclusion = 0;
    state.material.rim = 0;
    state.material.reflection = false;
    state.material.bloom = false;
    state.material.receiveShadow = false;
    state.material.receiveShadow2D = false;
    state.material.depthTest = true;
    state.material.depthBias = 0;
    if (!state.material.alphaTest) state.material.alphaTestBias = 0;
    state.material.emissiveMap = NULL;
    state.material.emission = CSColor3::Black;
    if (!state.usingMap()) state.material.uvOffset = CSVector2::Zero;
    state.fogColor = CSColor3::Black;
    state.fogNear = 0;
    state.fogFar = 0;
    state.bloomThreshold = 1;
    state.brightness = 0;
    state.contrast = 1;
    state.saturation = 1;
    state.strokeWidth = 0;
    state.strokeColor = CSColor::Black;
    state.strokeMode = CSStrokeNormal;
    state.scissor = CSRect::Zero;
    state.lightSpaceState = NULL;
}

bool CSShadowRenderer::clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const {
    bounds = CSRect::ScreenNone;

    const Param* param = static_assert_cast<const Param*>(state.rendererParam.value());

    if (param->mode != Param::ModePoint) {
        CSABoundingBox box = CSABoundingBox::None;
        for (int i = 0; i < count; i++) box.append(points[i], param->viewProjections[0]);
        if (box.intersects(CSABoundingBox::ViewSpace, CSCollisionFlagNone, NULL) != CSCollisionResultFront) {
            bounds.append(CSVector2(box.minimum.x, box.minimum.y));
            bounds.append(CSVector2(box.maximum.x, box.minimum.y));
            bounds.append(CSVector2(box.minimum.x, box.maximum.y));
            bounds.append(CSVector2(box.maximum.x, box.maximum.y));
            return true;
        }
    }
    else {
        bounds = CSRect::ScreenFull;

        for (int i = 0; i < 6; i++) {
            const CSMatrix& m = param->viewProjections[i];
            CSABoundingBox box = CSABoundingBox::None;
            for (int i = 0; i < count; i++) box.append(points[i], m);
            if (box.intersects(CSABoundingBox::ViewSpace, CSCollisionFlagNone, NULL) != CSCollisionResultFront) return true;
        }
    }
    return false;
}

void CSShadowRenderer::setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) {
    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    const Param* param = static_assert_cast<const Param*>(state.rendererParam.value());

    if (param->mode == Param::ModePoint) {
        _programs.addLink(CSProgramBranch::MaskGeometry);
        switch (mode) {
            case CSPrimitivePoints:
                _programs.addBranch("UsingPrimitive", 1, 4, CSProgramBranch::MaskGeometry);
                break;
            case CSPrimitiveLines:
            case CSPrimitiveLineLoop:
            case CSPrimitiveLineStrip:
                _programs.addBranch("UsingPrimitive", 2, 4, CSProgramBranch::MaskGeometry);
                break;
            case CSPrimitiveTriangles:
            case CSPrimitiveTriangleStrip:
            case CSPrimitiveTriangleFan:
                _programs.addBranch("UsingPrimitive", 3, 4, CSProgramBranch::MaskGeometry);
                break;
        }
    }

    _programs.addBranch("UsingShadowMode", param->mode, 4, CSProgramBranch::MaskVertex | CSProgramBranch::MaskGeometry | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingMap", state.usingMap(), CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingColorMap", state.material.colorMap, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingDisplacementMap", state.material.displacementScale != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingAlphaTest", state.material.alphaTest, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBone", boneBuffer != NULL, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingInstance", usingInstance, CSProgramBranch::MaskVertex);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api, boneBuffer);

    api->bindBufferBase(CSGBufferTargetUniform, CSRenderState::UniformBlockBindingShadow, param->uniformBuffer->object());
}

struct PointShadowUniformData {
    CSMatrix viewProjections[6];
    CSVector3 position;
    float range;

    inline PointShadowUniformData() {
        memset(this, 0, sizeof(PointShadowUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(viewProjections[0]);
        hash.combine(viewProjections[1]);
        hash.combine(viewProjections[2]);
        hash.combine(viewProjections[3]);
        hash.combine(viewProjections[4]);
        hash.combine(viewProjections[5]);
        hash.combine(position);
        hash.combine(range);
        return hash;
    }
    inline bool operator ==(const PointShadowUniformData& other) const {
        return position == other.position && range == other.range;
    }
    inline bool operator !=(const PointShadowUniformData& other) const {
        return !(*this == other);
    }
};

struct SpotShadowUniformData {
    CSMatrix viewProjection;
    CSVector3 position;
    float range;

    inline SpotShadowUniformData() {
        memset(this, 0, sizeof(SpotShadowUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(viewProjection);
        hash.combine(position);
        hash.combine(range);
        return hash;
    }
    inline bool operator ==(const SpotShadowUniformData& other) const {
        return viewProjection == other.viewProjection;
    }
    inline bool operator !=(const SpotShadowUniformData& other) const {
        return !(*this == other);
    }
};

void CSShadowRenderer::beginDirectional(CSGraphics* graphics, const CSMatrix& viewProjection) {
    Param* param = new Param(
        Param::ModeDirection,
        &viewProjection,
        CSGBuffers::fromData(viewProjection, CSGBufferTargetUniform));

    graphics->setRendererParam(param);
    param->release();

    param->uniformBuffer->flush();
}

void CSShadowRenderer::beginPoint(CSGraphics* graphics, const CSMatrix* viewProjections, const CSVector3& pos, float range) {
    PointShadowUniformData data;
    memcpy(data.viewProjections, viewProjections, 6 * sizeof(CSMatrix));
    data.position = pos;
    data.range = range;

    Param* param = new Param(
        Param::ModePoint,
        viewProjections,
        CSGBuffers::fromData(data, CSGBufferTargetUniform));

    graphics->setRendererParam(param);
    param->release();

    param->uniformBuffer->flush();
}

void CSShadowRenderer::beginSpot(CSGraphics* graphics, const CSMatrix& viewProjection, const CSVector3& pos, float range) {
    SpotShadowUniformData data;
    data.viewProjection = viewProjection;
    data.position = pos;
    data.range = range;

    Param* param = new Param(
        Param::ModeSpot,
        &viewProjection,
        CSGBuffers::fromData(data, CSGBufferTargetUniform));

    graphics->setRendererParam(param);
    param->release();

    param->uniformBuffer->flush();
}

void CSShadowRenderer::end(CSGraphics* graphics) {
    graphics->setRendererParam(NULL);
}
