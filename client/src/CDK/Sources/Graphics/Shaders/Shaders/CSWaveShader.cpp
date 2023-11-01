#define CDK_IMPL

#include "CSWaveShader.h"

#include "CSFile.h"

#include "CSGBuffers.h"
#include "CSVertexArrays.h"

CSWaveShader::CSWaveShader() {
    string commonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/wave.glsl"));
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/wave_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/wave_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, commonShaderCode, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, commonShaderCode, fragmentShaderCode);
}

struct WaveUniformData {
    CSMatrix worldViewProjection;
    CSVector3 center;
    float radius;
    CSVector2 resolution;
    float thickness;

    inline WaveUniformData() {
        memset(this, 0, sizeof(WaveUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(worldViewProjection);
        hash.combine(center);
        hash.combine(radius);
        hash.combine(resolution);
        hash.combine(thickness);
        return hash;
    }
    inline bool operator ==(const WaveUniformData& other) const {
        return memcmp(this, &other, sizeof(WaveUniformData)) == 0;
    }
    inline bool operator !=(const WaveUniformData& other) const {
        return !(*this == other);
    }
};

void CSWaveShader::draw(CSGraphicsApi* api, const CSTexture* screenTexture, const CSMatrix& worldViewProjection, const CSVector3& center, float radius, float thickness) {
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

    WaveUniformData data;
    data.worldViewProjection = worldViewProjection;
    data.center = center;
    data.radius = radius;
    data.resolution = CSVector2(target->viewport().width, target->viewport().height);
    data.thickness = thickness;

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
