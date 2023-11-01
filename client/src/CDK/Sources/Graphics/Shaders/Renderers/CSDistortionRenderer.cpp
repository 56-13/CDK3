#define CDK_IMPL

#include "CSDistortionRenderer.h"

#include "CSFile.h"

#include "CSGraphics.h"
#include "CSGBuffers.h"
#include "CSShaderCode.h"

CSDistortionRenderer::CSDistortionRenderer() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/distortion_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/distortion_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, "#define UsingNormal\n", CSShaderCode::VSBase, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::Base, fragmentShaderCode);
}

bool CSDistortionRenderer::visible(const CSRenderState& state) const {
    return state.material.shader == CSMaterial::ShaderDistortion && state.material.distortionScale > 0;
}

void CSDistortionRenderer::validate(CSRenderState& state) const {
    state.depthMode = CSDepthReadWrite;
    state.material.blendMode = CSBlendNone;
    if (!state.material.alphaTest) state.material.colorMap = NULL;
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
    if (!state.material.alphaTest) state.material.alphaTestBias = 0;
    state.material.emissiveMap = NULL;
    state.material.emission = CSColor3::Black;
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
    state.lightSpaceState = NULL;
}

bool CSDistortionRenderer::clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const {
    bounds = CSRect::ScreenNone;

    CSABoundingBox box = CSABoundingBox::None;
    const CSMatrix& vp = state.camera.viewProjection();
    for (int i = 0; i < count; i++) box.append(points[i], vp);
    if (box.intersects(CSABoundingBox::ViewSpace, CSCollisionFlagNone, NULL) != CSCollisionResultFront) {
        bounds.append(CSVector2(box.minimum.x, box.minimum.y));
        bounds.append(CSVector2(box.maximum.x, box.minimum.y));
        bounds.append(CSVector2(box.minimum.x, box.maximum.y));
        bounds.append(CSVector2(box.maximum.x, box.maximum.y));
        return true;
    }
    return false;
}

void CSDistortionRenderer::setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) {
    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    _programs.addBranch("UsingMap", state.usingMap(), CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingcolorMap", state.material.colorMap, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingnormalMap", state.material.normalMap, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingAlphaTest", state.material.alphaTest, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingDepthBias", state.material.depthBias != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingBone", boneBuffer != NULL, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingInstance", usingInstance, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingPerspective", state.camera.fov() != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsinguvOffset", state.material.uvOffset != CSVector2::Zero, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBloomSupported", state.target->bloomSupported(), CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api, boneBuffer);

    const CSTexture* screenTexture = static_assert_cast<const CSTexture*>(state.rendererParam.value());

    api->bindTextureBase(CSTextureTarget2D, CSRenderState::TextureBindingScreenMap, screenTexture->object());
}

void CSDistortionRenderer::begin(CSGraphics* graphics) {
    const CSTexture* screenTexture = graphics->target()->captureColor(0, true);
    screenTexture->flush();
    graphics->setRendererParam(screenTexture);
}

void CSDistortionRenderer::end(CSGraphics* graphics) {
    const CSTexture* screenTexture = static_assert_cast<const CSTexture*>(graphics->rendererParam());

    CSDelegateRenderCommand* command = graphics->command([screenTexture](CSGraphicsApi* api) {
        CSResourcePool::sharedPool()->remove(screenTexture);
    });
    command->addFence(graphics->target(), CSGBatchFlagReadWrite);           //타겟에 쓰는 명령을 flush

    graphics->setRendererParam(NULL);
}