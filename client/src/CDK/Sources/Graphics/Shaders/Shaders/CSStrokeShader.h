#ifndef __CDK__CSStrokeShader__
#define __CDK__CSStrokeShader__

#include "CSProgramBranch.h"
#include "CSRenderState.h"

class CSStrokeShader {
private:
    CSProgramBranch _programs;
public:
    CSStrokeShader();

    CSStrokeShader(const CSStrokeShader&) = delete;
    CSStrokeShader& operator =(const CSStrokeShader&) = delete;

    void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance);
};

#endif
