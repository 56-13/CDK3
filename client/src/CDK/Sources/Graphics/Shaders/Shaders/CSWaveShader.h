#ifndef __CDK__CSWaveShader__
#define __CDK__CSWaveShader__

#include "CSProgramBranch.h"
#include "CSRenderTarget.h"
#include "CSMatrix.h"

class CSWaveShader {
private:
    CSProgramBranch _programs;
public:
    CSWaveShader();

    CSWaveShader(const CSWaveShader&) = delete;
    CSWaveShader& operator =(const CSWaveShader&) = delete;

    void draw(CSGraphicsApi* api, const CSTexture* screenTexture, const CSMatrix& worldViewProjection, const CSVector3& center, float radius, float thickness);
};

#endif
