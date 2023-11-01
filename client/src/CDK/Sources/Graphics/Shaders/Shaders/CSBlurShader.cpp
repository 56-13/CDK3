#include "CSBlurShader.h"

#include "CSFile.h"

#include "CSRenderTargets.h"
#include "CSGBuffers.h"
#include "CSVertexArrays.h"
#include "CSShaders.h"

static constexpr int KernelRadius = 2;

static constexpr float IntensityToSigma = 3.0f;

static constexpr int BlurModeNormal = 0;
static constexpr int BlurModeCube = 1;
static constexpr int BlurModeDepth = 2;
static constexpr int BlurModeDirection = 3;
static constexpr int BlurModeCenter = 4;
static constexpr int BlurModeCount = 5;

CSBlurShader::CSBlurShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blur_vs.glsl"));
    string geometryShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blur_gs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blur_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, vertexShaderCode);
    _programs.attach(CSShaderTypeGeometry, geometryShaderCode);
    _programs.attach(CSShaderTypeFragment, fragmentShaderCode);
}

static CSBounds2 quadToBounds(const CSRenderTarget* target, const CSVector2* quads) {
    CSVector2 min = CSVector2::One;
    CSVector2 max = -CSVector2::One;

    for (int i = 0; i < 4; i++) {
        CSVector2::min(min, quads[i], min);
        CSVector2::max(max, quads[i], max);
    }
    min = CSVector2::clamp(min * 0.5f + 0.5f, CSVector2::Zero, CSVector2::One);
    max = CSVector2::clamp(max * 0.5f + 0.5f, CSVector2::Zero, CSVector2::One);

    const CSBounds2& viewport = target->viewport();

    return CSBounds2(
        (int)CSMath::floor(min.x * viewport.width) + viewport.x,
        (int)CSMath::floor(min.y * viewport.height) + viewport.y,
        (int)CSMath::ceil((max.x - min.x) * viewport.width),
        (int)CSMath::ceil((max.y - min.y) * viewport.height));
}

static CSRect boundsToQuad(const CSRenderTarget* target, const CSBounds2& bounds) {
    const CSBounds2& viewport = target->viewport();

    return CSRect(
        (float)(bounds.x - viewport.x) / viewport.width,
        (float)(bounds.y - viewport.y) / viewport.height,
        (float)bounds.width / viewport.width,
        (float)bounds.height / viewport.height);
}

void CSBlurShader::draw(CSGraphicsApi* api, const CSVector2* quad, float intensity) {
    draw(api, CSVertexArrayDraw::array(CSVertexArrays::get2D(quad, 4), CSPrimitiveTriangles, 0, 4), quadToBounds(api->currentTarget(), quad), intensity, false);
}

void CSBlurShader::draw(CSGraphicsApi* api, float intensity) {
    draw(api, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), api->currentTarget()->viewport(), intensity, false);
}

void CSBlurShader::draw(CSGraphicsApi* api, const CSBounds2& bounds, float intensity) {
    draw(api, CSVertexArrayDraw::array(CSVertexArrays::get2D(boundsToQuad(api->currentTarget(), bounds)), CSPrimitiveTriangleStrip, 0, 4), bounds, intensity, false);
}

struct BlurUniformData {
    float kernel[4];    //with pad
    CSVector2 direction;

    inline BlurUniformData() {
        memset(this, 0, sizeof(BlurUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(kernel[0]);
        hash.combine(kernel[1]);
        hash.combine(kernel[2]);
        hash.combine(kernel[3]);
        hash.combine(direction);
        return hash;
    }
    inline bool operator ==(const BlurUniformData& other) const {
        return memcmp(this, &other, sizeof(BlurUniformData)) == 0;
    }
    inline bool operator !=(const BlurUniformData& other) const {
        return !(*this == other);
    }
};

void CSBlurShader::draw(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, float intensity, bool cube) {
    CSRenderTarget* target = api->currentTarget();
    
    target->setDrawBuffer(api, 0);

    const CSBounds2& viewport = target->viewport();

    CSRenderTargetDescription sampleTargetDesc;
    sampleTargetDesc.width = viewport.width;
    sampleTargetDesc.height = viewport.height;
    CSRenderTargetDescription::Attachment a;
    a.attachment = CSFramebufferAttachmentColor0;
    a.format = target->format(CSFramebufferAttachmentColor0);
    a.textureTarget = cube ? CSTextureTargetCubeMap : CSTextureTarget2D;
    a.textureWrapS = CSTextureWrapClampToEdge;
    a.textureWrapT = CSTextureWrapClampToEdge;
    a.textureWrapR = CSTextureWrapClampToEdge;
    a.texture = true;
    sampleTargetDesc.attachments.addObject(a);

    CSRenderTarget* sampleTarget0;

    CSTexture* texture0 = target->textureAttachment(CSFramebufferAttachmentColor0);

    if (!texture0 || texture0->samples() > 1) {
        int r = KernelRadius * CSMath::max((int)intensity, 1);

        CSBounds2 srcbounds = bounds;
        srcbounds.inflate(r, r);
        srcbounds.intersect(viewport);

        sampleTarget0 = CSRenderTargets::getTemporary(sampleTargetDesc);

        target->blit(sampleTarget0, srcbounds, srcbounds.offsetBounds(-viewport.origin()), CSClearBufferColor, CSBlitFramebufferFilterNearest);

        texture0 = sampleTarget0->textureAttachment(CSFramebufferAttachmentColor0);
    }
    else {
        sampleTarget0 = target;
    }

    CSRenderTarget* sampleTarget1 = CSRenderTargets::getTemporary(sampleTargetDesc);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    if (cube) _programs.addLink(CSProgramBranch::MaskGeometry);
    _programs.addBranch("UsingBlurModeCube", cube, CSProgramBranch::MaskVertex);
    _programs.addBranch("BlurMode", cube ? BlurModeCube : BlurModeNormal, BlurModeCount, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    CSVector2 textureScale(1.0f / viewport.width, 1.0f / viewport.height);
    CSTexture* texture1 = sampleTarget1->textureAttachment(CSFramebufferAttachmentColor0);

    BlurUniformData data;

    while (intensity > 0) {
        float sigma = CSMath::min(intensity, 1.0f) * IntensityToSigma;
        CSMath::gaussKernel(KernelRadius, sigma, data.kernel);
        sampleTarget1->focus(api);
        data.direction = CSVector2(textureScale.x, 0);
        CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(texture0->target(), 0, texture0->object());
        vertices.draw(api);

        sampleTarget0->focus(api);
        data.direction = CSVector2(0, textureScale.y);
        dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(texture1->target(), 0, texture1->object());
        vertices.draw(api);

        intensity -= 1;
    }

    if (sampleTarget0 != target) {
        target->focus(api);

        CSShaders::blit()->draw(api, texture0, vertices, cube);

        CSResourcePool::sharedPool()->remove(sampleTarget0);
    }

    CSResourcePool::sharedPool()->remove(sampleTarget1);
}

void CSBlurShader::drawCube(CSGraphicsApi* api, float intensity) {
    draw(api, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), api->currentTarget()->viewport(), intensity, true);
}

struct BlurDepthUniformData {
    float kernel[4];        //with pad
    CSVector2 direction;
    float range;
    float distance;
    float znear;
    float zfar;

    inline BlurDepthUniformData() {
        memset(this, 0, sizeof(BlurDepthUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(kernel[0]);
        hash.combine(kernel[1]);
        hash.combine(kernel[2]);
        hash.combine(kernel[3]);
        hash.combine(direction);
        hash.combine(range);
        hash.combine(distance);
        hash.combine(znear);
        hash.combine(zfar);
        return hash;
    }
    inline bool operator ==(const BlurDepthUniformData& other) const {
        return memcmp(this, &other, sizeof(BlurDepthUniformData)) == 0;
    }
    inline bool operator !=(const BlurDepthUniformData& other) const {
        return !(*this == other);
    }
};

void CSBlurShader::drawDepth(CSGraphicsApi* api, const CSVector2* quad, const CSCamera& camera, float distance, float range, float intensity) {
    drawDepth(api, CSVertexArrayDraw::array(CSVertexArrays::get2D(quad, 4), CSPrimitiveTriangles, 0, 4), quadToBounds(api->currentTarget(), quad), camera, distance, range, intensity);
}

void CSBlurShader::drawDepth(CSGraphicsApi* api, const CSCamera& camera, float distance, float range, float intensity) {
    drawDepth(api, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), api->currentTarget()->viewport(), camera, distance, range, intensity);
}

void CSBlurShader::drawDepth(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSCamera& camera, float distance, float range, float intensity) {
    CSRenderTarget* target = api->currentTarget();

    CSRawFormat depthFormat = target->format(CSFramebufferAttachmentDepthStencil);

    if (depthFormat == 0) return;

    target->setDrawBuffer(api, 0);

    const CSBounds2& viewport = target->viewport();

    CSRenderTargetDescription depthTargetDesc;
    depthTargetDesc.width = viewport.width;
    depthTargetDesc.height = viewport.height;
    CSRenderTargetDescription::Attachment a;
    a.attachment = CSFramebufferAttachmentDepthStencil;
    a.format = depthFormat;
    a.texture = true;
    depthTargetDesc.attachments.addObject(a);

    CSRenderTarget* depthTarget = CSRenderTargets::getTemporary(depthTargetDesc);
    target->blit(depthTarget, bounds, bounds.offsetBounds(-viewport.origin()), CSClearBufferDepth | CSClearBufferStencil, CSBlitFramebufferFilterNearest);
    CSTexture* depthMap = depthTarget->textureAttachment(CSFramebufferAttachmentDepthStencil);

    CSRenderTargetDescription sampleTargetDesc;
    sampleTargetDesc.width = viewport.width;
    sampleTargetDesc.height = viewport.height;
    a.attachment = CSFramebufferAttachmentColor0;
    a.format = target->format(CSFramebufferAttachmentColor0);
    a.texture = true;
    depthTargetDesc.attachments.addObject(a);

    CSRenderTarget* sampleTarget0;

    CSTexture* texture0 = target->textureAttachment(CSFramebufferAttachmentColor0);

    if (!texture0 || texture0->samples() > 1) {
        int r = KernelRadius * CSMath::max((int)intensity, 1);
        
        CSBounds2 srcbounds = bounds;
        srcbounds.inflate(r, r);
        srcbounds.intersect(viewport);

        sampleTarget0 = CSRenderTargets::getTemporary(sampleTargetDesc);

        target->blit(sampleTarget0, srcbounds, srcbounds.offsetBounds(-viewport.origin()), CSClearBufferColor, CSBlitFramebufferFilterNearest);

        texture0 = sampleTarget0->textureAttachment(CSFramebufferAttachmentColor0);
    }
    else {
        sampleTarget0 = target;
    }

    CSRenderTarget* sampleTarget1 = CSRenderTargets::getTemporary(sampleTargetDesc);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBlurModeCube", false, CSProgramBranch::MaskVertex);
    _programs.addBranch("BlurMode", BlurModeDepth, BlurModeCount, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    BlurDepthUniformData data;
    data.distance = distance;
    data.range = range;
    data.znear = camera.znear();
    data.zfar = camera.zfar();

    api->bindTextureBase(CSTextureTarget2D, 1, depthMap->object());

    CSVector2 textureScale(1.0f / viewport.width, 1.0f / viewport.height);
    CSTexture* texture1 = sampleTarget1->textureAttachment(CSFramebufferAttachmentColor0);

    while (intensity > 0) {
        float sigma = CSMath::min(intensity, 1.0f) * IntensityToSigma;
        CSMath::gaussKernel(KernelRadius, sigma, data.kernel);
        
        sampleTarget1->focus(api);
        data.direction = CSVector2(textureScale.x, 0);
        CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(CSTextureTarget2D, 0, texture0->object());
        vertices.draw(api);

        sampleTarget0->focus(api);
        data.direction = CSVector2(0, textureScale.y);
        dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
        api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
        api->bindTextureBase(CSTextureTarget2D, 0, texture1->object());
        vertices.draw(api);

        intensity -= 1;
    }

    if (sampleTarget0 != target) {
        target->focus(api);

        CSShaders::blit()->draw(api, texture0, vertices, false);

        CSResourcePool::sharedPool()->remove(sampleTarget0);
    }

    CSResourcePool::sharedPool()->remove(sampleTarget1);
    CSResourcePool::sharedPool()->remove(depthTarget);
}

struct BlurDirectionUniformData {
    CSVector2 resolution;
    CSVector2 direction;

    inline BlurDirectionUniformData() {
        memset(this, 0, sizeof(BlurDirectionUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(resolution);
        hash.combine(direction);
        return hash;
    }
    inline bool operator ==(const BlurDirectionUniformData& other) const {
        return resolution == other.resolution && direction == other.direction;
    }
    inline bool operator !=(const BlurDirectionUniformData& other) const {
        return !(*this == other);
    }
};

void CSBlurShader::drawDirection(CSGraphicsApi* api, const CSVector2* quad, const CSVector2& dir) {
    drawDirection(api, CSVertexArrayDraw::array(CSVertexArrays::get2D(quad, 4), CSPrimitiveTriangles, 0, 4), quadToBounds(api->currentTarget(), quad), dir);
}

void CSBlurShader::drawDirection(CSGraphicsApi* api, const CSVector2& dir) {
    drawDirection(api, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), api->currentTarget()->viewport(), dir);
}

void CSBlurShader::drawDirection(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSVector2& dir) {
    CSRenderTarget* target = api->currentTarget();

    target->setDrawBuffer(api, 0);

    const CSBounds2& viewport = target->viewport();

    CSRenderTargetDescription sampleTargetDesc;
    sampleTargetDesc.width = viewport.width;
    sampleTargetDesc.height = viewport.height;
    CSRenderTargetDescription::Attachment a;
    a.attachment = CSFramebufferAttachmentColor0;
    a.format = target->format(CSFramebufferAttachmentColor0);
    a.texture = true;
    sampleTargetDesc.attachments.addObject(a);

    CSRenderTarget* sampleTarget0 = CSRenderTargets::getTemporary(sampleTargetDesc);

    CSBounds2 srcbounds = bounds;
    srcbounds.x += ((int)CSMath::ceil(CSMath::abs(dir.x)) + 1) * CSMath::sign(dir.x);
    srcbounds.y += ((int)CSMath::ceil(CSMath::abs(dir.y)) + 1) * CSMath::sign(dir.y);
    srcbounds.intersect(viewport);

    target->blit(sampleTarget0, srcbounds, srcbounds.offsetBounds(-viewport.origin()), CSClearBufferColor, CSBlitFramebufferFilterNearest);

    CSTexture* texture = sampleTarget0->textureAttachment(CSFramebufferAttachmentColor0);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBlurModeCube", false, CSProgramBranch::MaskVertex);
    _programs.addBranch("BlurMode", BlurModeDirection, BlurModeCount, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    BlurDirectionUniformData data;
    data.resolution = CSVector2(viewport.width, viewport.height);
    data.direction = dir;

    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
    api->bindTextureBase(CSTextureTarget2D, 0, texture->object());

    vertices.draw(api);

    CSResourcePool::sharedPool()->remove(sampleTarget0);
}


struct BlurCenterUniformData {
    CSVector2 resolution;
    CSVector2 centerCoord;
    float range;

    inline BlurCenterUniformData() {
        memset(this, 0, sizeof(BlurCenterUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(resolution);
        hash.combine(centerCoord);
        hash.combine(range);
        return hash;
    }
    inline bool operator ==(const BlurCenterUniformData& other) const {
        return resolution == other.resolution && centerCoord == other.centerCoord && range == other.range;
    }
    inline bool operator !=(const BlurCenterUniformData& other) const {
        return !(*this == other);
    }
};

void CSBlurShader::drawCenter(CSGraphicsApi* api, const CSVector2* quad, const CSVector2& center, float range) {
    drawCenter(api, CSVertexArrayDraw::array(CSVertexArrays::get2D(quad, 4), CSPrimitiveTriangles, 0, 4), quadToBounds(api->currentTarget(), quad), center, range);
}

void CSBlurShader::drawCenter(CSGraphicsApi* api, const CSVector2& center, float range) {
    drawCenter(api, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), api->currentTarget()->viewport(), center, range);
}

void CSBlurShader::drawCenter(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSVector2& center, float range) {
    CSRenderTarget* target = api->currentTarget();

    target->setDrawBuffer(api, 0);

    const CSBounds2& viewport = target->viewport();

    CSRenderTargetDescription sampleTargetDesc;
    sampleTargetDesc.width = viewport.width;
    sampleTargetDesc.height = viewport.height;
    CSRenderTargetDescription::Attachment a;
    a.attachment = CSFramebufferAttachmentColor0;
    a.format = target->format(CSFramebufferAttachmentColor0);
    a.texture = true;
    sampleTargetDesc.attachments.addObject(a);

    CSRenderTarget* sampleTarget0 = CSRenderTargets::getTemporary(sampleTargetDesc);

    int r = CSMath::ceil(range);
    
    CSBounds2 srcbounds = bounds;
    srcbounds.inflate(r, r);
    srcbounds.intersect(viewport);

    target->blit(sampleTarget0, srcbounds, srcbounds.offsetBounds(-viewport.origin()), CSClearBufferColor, CSBlitFramebufferFilterNearest);

    CSTexture* texture = sampleTarget0->textureAttachment(CSFramebufferAttachmentColor0);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingBlurModeCube", false, CSProgramBranch::MaskVertex);
    _programs.addBranch("BlurMode", BlurModeCenter, BlurModeCount, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    BlurCenterUniformData data;
    data.resolution = CSVector2(viewport.width, viewport.height);
    data.centerCoord = center * 0.5f + 0.5f;
    range = range;

    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
    api->bindTextureBase(CSTextureTarget2D, 0, texture->object());

    vertices.draw(api);

    CSResourcePool::sharedPool()->remove(sampleTarget0);
}