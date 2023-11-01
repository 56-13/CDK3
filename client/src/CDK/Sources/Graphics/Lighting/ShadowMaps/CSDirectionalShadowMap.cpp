#define CDK_IMPL

#include "CSDirectionalShadowMap.h"

#include "CSGraphics.h"
#include "CSShaders.h"
#include "CSRenderers.h"

CSDirectionalShadowMap::CSDirectionalShadowMap() {
    memset(this, 0, sizeof(CSDirectionalShadowMap));
}

CSDirectionalShadowMap::~CSDirectionalShadowMap() {
	CSObject::release(_renderTarget[0]);
	CSObject::release(_renderTarget[1]);
}

CSTexture* CSDirectionalShadowMap::texture(bool shadow2D) {
	return _renderTarget[shadow2D] ? _renderTarget[shadow2D]->textureAttachment(CSFramebufferAttachmentColor0) : NULL;
}

void CSDirectionalShadowMap::clear(bool shadow2D) {
    CSObject::release(_renderTarget[shadow2D]);
}

static CSRenderTarget* createRenderTarget(int resolution, bool pixel32) {
    CSTextureDescription colorDesc;
    colorDesc.width = resolution;
    colorDesc.height = resolution;
    colorDesc.format = pixel32 ? CSRawFormat::Rg32f : CSRawFormat::Rg16f;
    colorDesc.minFilter = CSTextureMinFilterLinear;
    colorDesc.magFilter = CSTextureMagFilterLinear;
    colorDesc.wrapS = CSTextureWrapClampToBorder;
    colorDesc.wrapT = CSTextureWrapClampToBorder;
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

static CSRenderTarget* createRenderTarget2D(int resolution) {
    CSTextureDescription colorDesc;
    colorDesc.width = resolution;
    colorDesc.height = resolution;
    colorDesc.format = CSRawFormat::R8;
    colorDesc.minFilter = CSTextureMinFilterLinear;
    colorDesc.magFilter = CSTextureMagFilterLinear;
    colorDesc.wrapS = CSTextureWrapClampToBorder;
    colorDesc.wrapT = CSTextureWrapClampToBorder;
    colorDesc.borderColor = CSColor::White;

    CSTexture* colorTexture = new CSTexture(colorDesc);
    CSRenderTarget* target = new CSRenderTarget(resolution, resolution);
    target->attach(CSFramebufferAttachmentColor0, colorTexture, 0, true);
    colorTexture->release();
    return target;
}

bool CSDirectionalShadowMap::begin(CSGraphics* graphics, const CSDirectionalLight& light, const CSABoundingBox& space, bool shadow2D) {
    CSRenderTarget* target;

    if (shadow2D) {
        if (!_renderTarget[1] || _light.shadow.resolution != light.shadow.resolution) {
            CSObject::release(_renderTarget[1]);
            _renderTarget[1] = createRenderTarget2D(light.shadow.resolution);
        }
        target = _renderTarget[1];
    }
    else {
        if (!_renderTarget[0] || _light.shadow.resolution != light.shadow.resolution || _light.shadow.pixel32 != light.shadow.pixel32) {
            CSObject::release(_renderTarget[0]);
            _renderTarget[0] = createRenderTarget(light.shadow.resolution, light.shadow.pixel32);
        }
        target = _renderTarget[0];
    }

    bool resetView = (_light.direction != light.direction || _camera != graphics->camera() || _space != space);

    _light = light;

    if (resetView) {
        _camera = graphics->camera();
        _space = space;

        updateView();
    }

    if (!_visible) return false;

    graphics->push();
    graphics->setTarget(target);
    graphics->setRenderOrder(false);
    graphics->clear(CSColor::White);

    if (shadow2D) {
        CSShadow2DRenderer* renderer = CSRenderers::shadow2D();
        renderer->begin(graphics, _viewProjection[1], _light.direction);
        graphics->setRenderer(renderer);
        graphics->setColor(CSColor::Black);
    }
    else {
        CSShadowRenderer* renderer = CSRenderers::shadow();
        renderer->beginDirectional(graphics, _viewProjection[0]);
        graphics->setRenderer(renderer);
    }
    return true;
}

void CSDirectionalShadowMap::end(CSGraphics* graphics, bool shadow2D) {
    if (shadow2D) {
        CSRenderers::shadow2D()->end(graphics);
    }
    else {
        CSRenderers::shadow()->end(graphics);

        CSRenderTarget* target = _renderTarget[0];
        float blur = _light.shadow.blur;
        CSDelegateRenderCommand* command = graphics->command([target, blur](CSGraphicsApi* api){
            target->focus(api);

            CSShaders::blur()->draw(api, blur);
        });
        command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
    }
    graphics->pop();
}
