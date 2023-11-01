#define CDK_IMPL

#include "CSSpotShadowMap.h"

#include "CSGraphics.h"
#include "CSShaders.h"
#include "CSRenderers.h"

CSSpotShadowMap::CSSpotShadowMap() {
    memset(this, 0, sizeof(CSSpotShadowMap));
}

CSSpotShadowMap::~CSSpotShadowMap() {
    CSObject::release(_renderTarget);
}

CSTexture* CSSpotShadowMap::texture() {
    return _renderTarget ? _renderTarget->textureAttachment(CSFramebufferAttachmentColor0) : NULL;
}

static CSRenderTarget* createRenderTarget(int resolution, bool pixel32) {
    CSTextureDescription colorDesc;
    colorDesc.width = resolution;
    colorDesc.height = resolution;
    colorDesc.format = pixel32 ? CSRawFormat::Rg32f : CSRawFormat::Rg16f;
    colorDesc.wrapS = CSTextureWrapClampToBorder;
    colorDesc.wrapT = CSTextureWrapClampToBorder;
    colorDesc.minFilter = CSTextureMinFilterLinear;
    colorDesc.magFilter = CSTextureMagFilterLinear;
    colorDesc.borderColor = CSColor::White;
    
    CSTexture* colorTexture = new CSTexture(colorDesc);
    CSRenderBuffer* depthBuffer = new CSRenderBuffer(resolution, resolution, CSRawFormat::DepthComponent16);
    CSRenderTarget* target = new CSRenderTarget(resolution, resolution);
    target->attach(CSFramebufferAttachmentColor0, colorTexture, 0, true);
    target->attach(CSFramebufferAttachmentDepth, depthBuffer, true);
    colorTexture->release();
    depthBuffer->release();
    return target;
}

void CSSpotShadowMap::begin(CSGraphics* graphics, const CSSpotLight& light, float range) {
    if (!_renderTarget || _light.shadow.resolution != light.shadow.resolution || _light.shadow.pixel32 != light.shadow.pixel32) {
        CSObject::release(_renderTarget);
        _renderTarget = createRenderTarget(light.shadow.resolution, _light.shadow.pixel32);
    }

    bool resetView = _light.position != light.position || _light.direction != light.direction || _light.angle != light.angle || _light.dispersion != light.dispersion || _range != range;

    _light = light;
    _range = range;

    if (resetView) {
        CSMatrix projection = CSMatrix::perspectiveFovLH(light.angle + light.dispersion * 2, 1, CSMath::max(range / 1000, 1.0f), range);
        CSVector3 target = light.position + light.direction;
        CSVector3 up = !CSVector3::nearEqual(light.direction, CSVector3::UnitX) ? CSVector3::normalize(CSVector3::cross(light.direction, CSVector3::UnitX)) : CSVector3::UnitZ;
        _viewProjection = CSMatrix::lookAtLH(light.position, target, up) * projection;
    }
    graphics->push();
    graphics->setTarget(_renderTarget);
    graphics->clear(CSColor::White);

    CSShadowRenderer* renderer = CSRenderers::shadow();
    renderer->beginSpot(graphics, _viewProjection, light.position, range);
    graphics->setRenderer(renderer);
}

void CSSpotShadowMap::end(CSGraphics* graphics) {
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