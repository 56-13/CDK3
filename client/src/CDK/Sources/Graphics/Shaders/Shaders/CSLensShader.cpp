#define CDK_IMPL

#include "CSLensShader.h"

#include "CSFile.h"

#include "CSVertexArrays.h"
#include "CSGBuffers.h"

CSLensShader::CSLensShader() {
    string commonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/lens.glsl"));
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/lens_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/lens_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, commonShaderCode, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, commonShaderCode, fragmentShaderCode);
}

struct LensUniformData {
    CSMatrix worldViewProjection;
    CSVector3 center;
    float radius;
    CSVector2 resolution;
    float convex;

    inline LensUniformData() {
        memset(this, 0, sizeof(LensUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(worldViewProjection);
        hash.combine(center);
        hash.combine(radius);
        hash.combine(resolution);
        hash.combine(convex);
        return hash;
    }
    inline bool operator ==(const LensUniformData& other) const {
        return memcmp(this, &other, sizeof(LensUniformData)) == 0;
    }
    inline bool operator !=(const LensUniformData& other) const {
        return !(*this == other);
    }
};

void CSLensShader::draw(CSGraphicsApi* api, const CSTexture* screenTexture, const CSMatrix& worldViewProjection, const CSVector3& center, float radius, float convex) {
    CSRenderTarget* target = api->currentTarget();

    if (target->bloomSupported()) target->setDrawBuffers(api, 2, 0, 1);
    else target->setDrawBuffer(api, 0);
    
    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    _programs.addBranch("UsingBloomSupported", target->bloomSupported(), CSProgramBranch::MaskFragment);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    LensUniformData data;
    data.worldViewProjection = worldViewProjection;
    data.center = center;
    data.radius = radius;
    data.resolution = CSVector2(target->viewport().width, target->viewport().height);
    data.convex = convex;
    
    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
    api->bindTextureBase(CSTextureTarget2D, 0, screenTexture->object());

    CSVertexArray* vertices = CSVertexArrays::get3D(4,
        CSVector3(center.x - radius, center.y - radius, center.z),
        CSVector3(center.x + radius, center.y - radius, center.z),
        CSVector3(center.x - radius, center.y + radius, center.z),
        CSVector3(center.x + radius, center.y * radius, center.z));

    vertices->drawArrays(api, CSPrimitiveTriangleStrip, 0, 4);
}
