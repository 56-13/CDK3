#define CDK_IMPL

#include "CSBlitShader.h"

#include "CSFile.h"

#include "CSRenderTarget.h"
#include "CSVertexArrays.h"

CSBlitShader::CSBlitShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blit_vs.glsl"));
    string geometryShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blit_gs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/blit_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, vertexShaderCode);
    _programs.attach(CSShaderTypeGeometry, geometryShaderCode);
    _programs.attach(CSShaderTypeFragment, fragmentShaderCode);
}

static CSRect boundsToQuad(const CSRenderTarget* target, const CSBounds2& bounds) {
    const CSBounds2& viewport = target->viewport();

    return CSRect(
        (float)(bounds.x - viewport.x) / viewport.width,
        (float)(bounds.y - viewport.y) / viewport.height,
        (float)bounds.width / viewport.width,
        (float)bounds.height / viewport.height);
}

void CSBlitShader::draw(CSGraphicsApi* api, const CSTexture* texture, bool cube) {
    draw(api, texture, CSVertexArrayDraw::array(CSVertexArrays::getScreen2D(), CSPrimitiveTriangleStrip, 0, 4), cube);
}

void CSBlitShader::draw(CSGraphicsApi* api, const CSTexture* texture, const CSBounds2& bounds, bool cube) {
    draw(api, texture, CSVertexArrayDraw::array(CSVertexArrays::get2D(boundsToQuad(api->currentTarget(), bounds)), CSPrimitiveTriangleStrip, 0, 4), cube);
}

void CSBlitShader::draw(CSGraphicsApi* api, const CSTexture* texture, const CSVertexArrayDraw& vertices, bool cube) {
    api->applyPolygonMode(CSPolygonFill);
    api->applyBlendMode(CSBlendNone);
    api->applyCullMode(CSCullNone);
    api->applyDepthMode(CSDepthNone);
    api->applyScissor(CSBounds2::Zero);

    _programs.addLink(CSProgramBranch::MaskVertex);
    _programs.addLink(CSProgramBranch::MaskFragment);
    if (cube) _programs.addLink(CSProgramBranch::MaskGeometry);
    _programs.addBranch("UsingCube", cube, CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    api->bindTextureBase(CSTextureTarget2D, 0, texture->object());

    vertices.draw(api);
}
