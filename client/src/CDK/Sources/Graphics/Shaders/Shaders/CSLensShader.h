#ifndef __CDK__CSLensShader__
#define __CDK__CSLensShader__

#include "CSProgramBranch.h"
#include "CSRenderTarget.h"
#include "CSMatrix.h"

class CSLensShader {
private:
    CSProgramBranch _programs;
public:
    CSLensShader();

    CSLensShader(const CSLensShader&) = delete;
    CSLensShader& operator =(const CSLensShader&) = delete;

    void draw(CSGraphicsApi* api, const CSTexture* screenTexture, const CSMatrix& worldViewProjection, const CSVector3& center, float radius, float convex);
};

#endif
