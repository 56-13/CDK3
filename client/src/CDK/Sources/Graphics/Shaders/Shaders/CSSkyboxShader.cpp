#define CDK_IMPL

#include "CSSkyboxShader.h"

#include "CSFile.h"

#include "CSRenderTarget.h"
#include "CSGBuffers.h"

CSSkyboxShader::CSSkyboxShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/skybox_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/skybox_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, fragmentShaderCode);

    CSVertexLayout layout(0, 0, 3, CSVertexAttribTypeFloat, false, 12, 0, 0, true);
    _vertices = new CSVertexArray(1, false, &layout, 1);
    
    static const CSVector3 vertices[] = {
        CSVector3(-1,  1, -1),
        CSVector3(-1, -1, -1),
        CSVector3(1, -1, -1),
        CSVector3(1, -1, -1),
        CSVector3(1,  1, -1),
        CSVector3(-1,  1, -1),

        CSVector3(-1, -1,  1),
        CSVector3(-1, -1, -1),
        CSVector3(-1,  1, -1),
        CSVector3(-1,  1, -1),
        CSVector3(-1,  1,  1),
        CSVector3(-1, -1,  1),

        CSVector3(1, -1, -1),
        CSVector3(1, -1,  1),
        CSVector3(1,  1,  1),
        CSVector3(1,  1,  1),
        CSVector3(1,  1, -1),
        CSVector3(1, -1, -1),

        CSVector3(-1, -1,  1),
        CSVector3(-1,  1,  1),
        CSVector3(1,  1,  1),
        CSVector3(1,  1,  1),
        CSVector3(1, -1,  1),
        CSVector3(-1, -1,  1),

        CSVector3(-1,  1, -1),
        CSVector3(1,  1, -1),
        CSVector3(1,  1,  1),
        CSVector3(1,  1,  1),
        CSVector3(-1,  1,  1),
        CSVector3(-1,  1, -1),

        CSVector3(-1, -1, -1),
        CSVector3(-1, -1,  1),
        CSVector3(1, -1, -1),
        CSVector3(1, -1, -1),
        CSVector3(-1, -1,  1),
        CSVector3(1, -1,  1)
    };
    _vertices->vertexBuffer(0)->upload(vertices, 36, CSGBufferUsageHintStaticDraw);
}

CSSkyboxShader::~CSSkyboxShader() {
    _vertices->release();
}

void CSSkyboxShader::draw(CSGraphicsApi* api, const CSCamera& camera, const CSTexture* texture) {
    api->currentTarget()->setDrawBuffers(api, 0);

    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthRead);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    CSGBuffer* dataBuffer = CSGBuffers::fromData(CSMatrix::translation(camera.position()) * camera.viewProjection(), CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, 0, dataBuffer->object());
    api->bindTextureBase(CSTextureTargetCubeMap, 0, texture->object());

    _vertices->drawArrays(api, CSPrimitiveTriangles, 0, 36);
}
