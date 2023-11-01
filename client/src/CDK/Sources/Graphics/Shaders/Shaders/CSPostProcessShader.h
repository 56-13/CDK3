#ifndef __CDK__CSPostProcessShader__
#define __CDK__CSPostProcessShader__

#include "CSProgramBranch.h"
#include "CSRenderTarget.h"

class CSPostProcessShader {
private:
    CSProgramBranch _programs;
    CSProgram* _bloomProgram;
public:
    CSPostProcessShader();
    ~CSPostProcessShader();

    CSPostProcessShader(const CSPostProcessShader&) = delete;
    CSPostProcessShader& operator =(const CSPostProcessShader&) = delete;

    void draw(CSGraphicsApi* api, CSRenderTarget* src, CSRenderTarget* dest, int bloomPass, float bloomIntensity, float exposure, float gamma);
private:
    void bloom(CSGraphicsApi* api, CSRenderTarget* target, int& pass);
};

#endif
