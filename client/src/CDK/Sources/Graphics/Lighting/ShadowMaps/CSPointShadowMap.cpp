#define CDK_IMPL

#include "CSPointShadowMap.h"

#include "CSGraphics.h"
#include "CSShaders.h"
#include "CSRenderers.h"

CSPointShadowMap::CSPointShadowMap() {
    memset(this, 0, sizeof(CSPointShadowMap));
}

CSPointShadowMap::~CSPointShadowMap() {
	CSObject::release(_renderTarget);
}

CSTexture* CSPointShadowMap::texture() {
	return _renderTarget ? _renderTarget->textureAttachment(CSFramebufferAttachmentColor0) : NULL;
}

static CSRenderTarget* createRenderTarget(int resolution, bool pixel32) {
    CSTextureDescription colorDesc;
    colorDesc.width = resolution;
    colorDesc.height = resolution;
    colorDesc.target = CSTextureTargetCubeMap;
    colorDesc.format = pixel32 ? CSRawFormat::Rg32f : CSRawFormat::Rg16f;
    colorDesc.wrapS = CSTextureWrapClampToEdge;
    colorDesc.wrapT = CSTextureWrapClampToEdge;
    colorDesc.wrapR = CSTextureWrapClampToEdge;
    colorDesc.minFilter = CSTextureMinFilterLinear;
    colorDesc.magFilter = CSTextureMagFilterLinear;
    CSTexture* colorTexture = new CSTexture(colorDesc);

    CSTextureDescription depthDesc;
    depthDesc.width = resolution;
    depthDesc.height = resolution;
    depthDesc.target = CSTextureTargetCubeMap;
    depthDesc.format = CSRawFormat::DepthComponent16;
    CSTexture* depthTexture = new CSTexture(depthDesc);

    CSRenderTarget* target = new CSRenderTarget(resolution, resolution);
    target->attach(CSFramebufferAttachmentColor0, colorTexture, 0, true);
    target->attach(CSFramebufferAttachmentDepth, depthTexture, 0, true);
    colorTexture->release();
    depthTexture->release();
    return target;
}

void CSPointShadowMap::begin(CSGraphics* graphics, const CSPointLight& light, float range) {
    if (!_renderTarget || _light.shadow.resolution != light.shadow.resolution || _light.shadow.pixel32 != light.shadow.pixel32) {
        CSObject::release(_renderTarget);
        _renderTarget = createRenderTarget(light.shadow.resolution, _light.shadow.pixel32);
    }

    bool resetView = _light.position != light.position || _range != range;

    _light = light;
    _range = range;

    if (resetView) {
        //far ~ near 비율이 너무 크면 정밀도가 매우 떨어짐
        CSMatrix projection = CSMatrix::perspectiveFovRH(FloatPiOverTwo, 1, CSMath::max(range / 1000, 1.0f), range);
        _viewProjections[0] = CSMatrix::lookAtRH(light.position, light.position + CSVector3::UnitX, -CSVector3::UnitY) * projection;
        _viewProjections[1] = CSMatrix::lookAtRH(light.position, light.position - CSVector3::UnitX, -CSVector3::UnitY) * projection;
        _viewProjections[2] = CSMatrix::lookAtRH(light.position, light.position + CSVector3::UnitY, CSVector3::UnitZ) * projection;
        _viewProjections[3] = CSMatrix::lookAtRH(light.position, light.position - CSVector3::UnitY, -CSVector3::UnitZ) * projection;
        _viewProjections[4] = CSMatrix::lookAtRH(light.position, light.position + CSVector3::UnitZ, -CSVector3::UnitY) * projection;
        _viewProjections[5] = CSMatrix::lookAtRH(light.position, light.position - CSVector3::UnitZ, -CSVector3::UnitY) * projection;
    }
    graphics->push();
    graphics->setTarget(_renderTarget);
    graphics->clear(CSColor::White);

    CSShadowRenderer* renderer = CSRenderers::shadow();
    renderer->beginPoint(graphics, _viewProjections, light.position, range);
    graphics->setRenderer(renderer);
}

void CSPointShadowMap::end(CSGraphics* graphics) {
    CSRenderers::shadow()->end(graphics);

    CSRenderTarget* target = _renderTarget;
    float blur = _light.shadow.blur;
    CSDelegateRenderCommand* command = graphics->command([target, blur](CSGraphicsApi* api) {
        target->focus(api);

        CSShaders::blur()->drawCube(api, blur);
    });
    command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
    graphics->pop();
}