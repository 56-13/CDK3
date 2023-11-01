#define CDK_IMPL

#include "CSShadow2DRenderer.h"

#include "CSFile.h"

#include "CSGraphics.h"
#include "CSGBuffers.h"
#include "CSShaderCode.h"

CSShadow2DRenderer::Param::Param(const CSMatrix& viewProjections, const CSVector3& lightDir) :
    viewProjection(viewProjection),
    lightDirection(lightDir),
    uniformBuffer(CSGBuffers::fromData(viewProjection, CSGBufferTargetUniform)) 
{

}

//===================================================================================================

CSShadow2DRenderer::CSShadow2DRenderer() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow2D_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow2D_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, CSShaderCode::VSBase, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::Base, fragmentShaderCode);
}

bool CSShadow2DRenderer::visible(const CSRenderState& state) const {
    return state.material.shader != CSMaterial::ShaderDistortion;
}

void CSShadow2DRenderer::validate(CSRenderState& state) const {
    state.depthMode = CSDepthNone;
    state.material.shader = CSMaterial::ShaderLight;
    state.material.blendMode = CSBlendMultiply;
    state.material.cullMode = CSCullNone;
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
    state.material.depthTest = false;
    state.material.depthBias = 0;
    state.material.alphaTest = false;
    state.material.alphaTestBias = 0;
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

bool CSShadow2DRenderer::clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const {
    bounds = CSRect::ScreenNone;

    const Param* param = static_assert_cast<const Param*>(state.rendererParam.value());

    CSABoundingBox box = CSABoundingBox::None;
    for (int i = 0; i < count; i++) box.append(points[i], param->viewProjection);
    if (box.intersects(CSABoundingBox::ViewSpace, CSCollisionFlagNone, NULL) != CSCollisionResultFront) {
        bounds.append(CSVector2(box.minimum.x, box.minimum.y));
        bounds.append(CSVector2(box.maximum.x, box.minimum.y));
        bounds.append(CSVector2(box.minimum.x, box.maximum.y));
        bounds.append(CSVector2(box.maximum.x, box.maximum.y));
        return true;
    }
    return false;
}

void CSShadow2DRenderer::setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) {
    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    _programs.addBranch("UsingMap", state.usingMap(), CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingcolorMap", state.material.colorMap, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingDisplacementMap", state.material.displacementScale != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingBone", boneBuffer != NULL, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingInstance", usingInstance, CSProgramBranch::MaskVertex);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api, boneBuffer);

    const Param* param = static_assert_cast<const Param*>(state.rendererParam.value());

    api->bindBufferBase(CSGBufferTargetUniform, CSRenderState::UniformBlockBindingShadow, param->uniformBuffer->object());
}

void CSShadow2DRenderer::begin(CSGraphics* graphics, const CSMatrix& viewProjection, const CSVector3& lightDir) {
    Param* param = new Param(viewProjection, lightDir);

    graphics->setRendererParam(param);
    param->release();

    param->uniformBuffer->flush();
}

void CSShadow2DRenderer::end(CSGraphics* graphics) {
    graphics->setRendererParam(NULL);
}
