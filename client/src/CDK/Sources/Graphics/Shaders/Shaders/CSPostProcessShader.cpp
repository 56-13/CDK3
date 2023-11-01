#define CDK_IMPL

#include "CSPostProcessShader.h"

#include "CSFile.h"

#include "CSVertexT.h"
#include "CSVertexArrays.h"
#include "CSRenderState.h"
#include "CSRenderTargets.h"
#include "CSGBuffers.h"

CSPostProcessShader::CSPostProcessShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/post_process_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/post_process_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, fragmentShaderCode);

    vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/bloom_vs.glsl"));
    fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/bloom_fs.glsl"));
    CSShader* vertexShader = new CSShader(CSShaderTypeVertex, vertexShaderCode);
    CSShader* fragmentShader = new CSShader(CSShaderTypeFragment, fragmentShaderCode);

    _bloomProgram->attach(vertexShader);
    _bloomProgram->attach(fragmentShader);
    _bloomProgram->link();

    vertexShader->release();
    fragmentShader->release();
}

CSPostProcessShader::~CSPostProcessShader() {
    _bloomProgram->release();
}

void CSPostProcessShader::bloom(CSGraphicsApi* api, CSRenderTarget* target, int& pass) {
    int width = CSMath::min(target->width(), target->height());
    while ((width >> pass) <= 1) pass--;
    if (pass <= 0) {
        pass = 0;
        return;
    }
    CSRenderTargetDescription targetDesc;
    targetDesc.width = target->width();
    targetDesc.height = target->height();
    CSRenderTargetDescription::Attachment a;
    a.attachment = CSFramebufferAttachmentColor1;
    a.format = target->format(CSFramebufferAttachmentColor1);
    a.textureWrapS = CSTextureWrapClampToEdge;
    a.textureWrapT = CSTextureWrapClampToEdge;
    a.textureWrapR = CSTextureWrapClampToEdge;
    a.textureMinFilter = CSTextureMinFilterLinear;
    a.textureMagFilter = CSTextureMagFilterLinear;
    a.texture = true;
    targetDesc.attachments.addObject(a);

    CSRenderTarget** targets = (CSRenderTarget**)alloca((pass + 1) * sizeof(CSRenderTarget*));
    targets[0] = target;

    for (int i = 1; i <= pass; i++) {
        targetDesc.width /= 2;
        targetDesc.height /= 2;
        targets[i] = CSRenderTargets::getTemporary(targetDesc);
    }

    CSVertexArray* vertices = CSVertexArrays::getScreen2D();

    _bloomProgram->use(api);

    for (int i = 0; i < pass; i++) {
        targets[i + 1]->focus(api);
        targets[i + 1]->setDrawBuffer(api, 1);

        CSTexture* texture = targets[i]->textureAttachment(CSFramebufferAttachmentColor1);

        CSVector2 delta(1.0f / texture->width(), 1.0f / texture->height());
        CSGBuffer* dataBuffer = CSGBuffers::fromData(delta, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(CSTextureTarget2D, 0, texture->object());
        vertices->drawArrays(api, CSPrimitiveTriangleStrip, 0, 4);
    }

    api->applyBlendMode(CSBlendAdd);

    for (int i = pass; i > 0; i--) {
        targets[i - 1]->focus(api);
        targets[i - 1]->setDrawBuffer(api, 1);

        CSTexture* texture = targets[i]->textureAttachment(CSFramebufferAttachmentColor1);

        CSVector2 delta(0.5f / texture->width(), 0.5f / texture->height());
        CSGBuffer* dataBuffer = CSGBuffers::fromData(delta, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(CSTextureTarget2D, 0, texture->object());
        vertices->drawArrays(api, CSPrimitiveTriangleStrip, 0, 4);

        CSResourcePool::sharedPool()->remove(targets[i]);
    }

    api->applyBlendMode(CSBlendNone);
}


struct PostProcessUniformData {
    CSVector2 resolution;
    float bloomIntensity;
    float gammaInv;
    float exposure;

    inline PostProcessUniformData() {
        memset(this, 0, sizeof(PostProcessUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(resolution);
        hash.combine(bloomIntensity);
        hash.combine(gammaInv);
        hash.combine(exposure);
        return hash;
    }
    inline bool operator ==(const PostProcessUniformData& other) const {
        return memcmp(this, &other, sizeof(PostProcessUniformData)) == 0;
    }
    inline bool operator !=(const PostProcessUniformData& other) const {
        return !(*this == other);
    }
};

void CSPostProcessShader::draw(CSGraphicsApi* api, CSRenderTarget* src, CSRenderTarget* dest, int bloomPass, float bloomIntensity, float exposure, float gamma) {
    CSAssert(!src->hasViewport(), "source target must not have viewport");

    CSRenderTarget* captureTarget0 = NULL;
    CSRenderTarget* captureTarget1 = NULL;
    CSTexture* colorMap = NULL;
    CSTexture* bloomMap = NULL;

    if (src != dest) colorMap = src->textureAttachment(CSFramebufferAttachmentColor0);

    bool bloomSupported = bloomIntensity > 0 && src->bloomSupported();

    if (bloomSupported && src->samples() == 1) bloomMap = src->textureAttachment(CSFramebufferAttachmentColor1);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    if (!colorMap || (bloomSupported && !bloomMap)) {
        CSRenderTargetDescription captureDesc;
        captureDesc.width = src->width();
        captureDesc.height = src->height();
        CSRenderTargetDescription::Attachment a;
        a.textureWrapS = CSTextureWrapClampToEdge;
        a.textureWrapT = CSTextureWrapClampToEdge;
        a.textureWrapR = CSTextureWrapClampToEdge;
        a.texture = true;
        captureDesc.attachments.addObject(a);

        if (!colorMap) {
            a.format = src->format(CSFramebufferAttachmentColor0);
            a.attachment = CSFramebufferAttachmentColor0;
            captureTarget0 = CSRenderTargets::getTemporary(captureDesc);
            src->blit(captureTarget0, CSClearBufferColor, CSBlitFramebufferFilterNearest, 0, 0);
            colorMap = captureTarget0->textureAttachment(CSFramebufferAttachmentColor0);
        }
        if (bloomSupported) {
            if (!bloomMap) {
                a.format = src->format(CSFramebufferAttachmentColor1);
                a.attachment = CSFramebufferAttachmentColor1;
                a.textureMinFilter = CSTextureMinFilterLinear;
                a.textureMagFilter = CSTextureMagFilterLinear;
                captureTarget1 = CSRenderTargets::getTemporary(captureDesc);
                src->blit(captureTarget1, CSClearBufferColor, CSBlitFramebufferFilterNearest, 1, 1);
                bloom(api, captureTarget1, bloomPass);
                bloomMap = captureTarget1->textureAttachment(CSFramebufferAttachmentColor1);
            }
            else {
                bloom(api, src, bloomPass);
            }
            bloomIntensity /= bloomPass + 1;
        }
    }

    dest->focus(api);
    dest->setDrawBuffers(api, 0);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    _programs.addBranch("UsingMultisample", colorMap->samples(), 17, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBloom", bloomMap != NULL, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingGamma", gamma != 1, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    PostProcessUniformData data;
    data.resolution = CSVector2(src->width(), src->height());
    data.bloomIntensity = bloomIntensity;
    data.gammaInv = 1.0f / gamma;
    data.exposure = exposure;
    
    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
    api->bindTextureBase(colorMap->target(), 0, colorMap->object());
    if (bloomMap) api->bindTextureBase(bloomMap->target(), 1, bloomMap->object());

    CSVertexArrays::getScreen2D()->drawArrays(api, CSPrimitiveTriangleStrip, 0, 4);

    if (captureTarget0) CSResourcePool::sharedPool()->remove(captureTarget0);
    if (captureTarget1) CSResourcePool::sharedPool()->remove(captureTarget1);
}
