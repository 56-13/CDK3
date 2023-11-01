#define CDK_IMPL

#include "CSEmptyShader.h"

#include "CSFile.h"

#include "CSRenderTarget.h"

CSEmptyShader::CSEmptyShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/empty_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/empty_fs.glsl"));

    CSShader* vertexShader = new CSShader(CSShaderTypeVertex, vertexShaderCode);
    CSShader* fragmentShader = new CSShader(CSShaderTypeFragment, fragmentShaderCode);

    _program = new CSProgram();
    _program->attach(vertexShader);
    _program->attach(fragmentShader);
    _program->link();

    vertexShader->release();
    fragmentShader->release();
}

CSEmptyShader::~CSEmptyShader() {
    _program->release();
}

void CSEmptyShader::draw(CSGraphicsApi* api, const CSVertexArrayDraw& vertices) {
    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _program->use(api);

    vertices.draw(api);
}
