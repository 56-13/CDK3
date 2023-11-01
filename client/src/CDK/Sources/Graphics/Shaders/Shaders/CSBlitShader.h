#ifndef __CDK__CSBlitShader__
#define __CDK__CSBlitShader__

#include "CSProgramBranch.h"
#include "CSTexture.h"
#include "CSVertexArrayDraw.h"

class CSBlitShader {
private:
    CSProgramBranch _programs;
public:
    CSBlitShader();

    CSBlitShader(const CSBlitShader&) = delete;
    CSBlitShader& operator =(const CSBlitShader&) = delete;

    void draw(CSGraphicsApi* api, const CSTexture* texture, bool cube);
    void draw(CSGraphicsApi* api, const CSTexture* texture, const CSBounds2& bounds, bool cube);
    void draw(CSGraphicsApi* api, const CSTexture* texture, const CSVertexArrayDraw& vertices, bool cube);
};

#endif
