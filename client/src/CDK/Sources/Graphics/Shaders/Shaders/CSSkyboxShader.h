#ifndef __CDK__CSSkyboxShader__
#define __CDK__CSSkyboxShader__

#include "CSProgramBranch.h"
#include "CSTexture.h"
#include "CSVertexArray.h"
#include "CSCamera.h"

class CSSkyboxShader {
private:
    CSProgramBranch _programs;
    CSVertexArray* _vertices;
public:
    CSSkyboxShader();
    ~CSSkyboxShader();

    CSSkyboxShader(const CSSkyboxShader&) = delete;
    CSSkyboxShader& operator =(const CSSkyboxShader&) = delete;

    void draw(CSGraphicsApi* api, const CSCamera& camera, const CSTexture* texture);
};

#endif
