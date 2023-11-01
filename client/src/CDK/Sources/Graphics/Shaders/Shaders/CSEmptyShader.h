#ifndef __CDK__CSEmptyShader__
#define __CDK__CSEmptyShader__

#include "CSProgram.h"

#include "CSVertexArrayDraw.h"

class CSEmptyShader {
private:
    CSProgram* _program;
public:
    CSEmptyShader();
    ~CSEmptyShader();

    CSEmptyShader(const CSEmptyShader&) = delete;
    CSEmptyShader& operator =(const CSEmptyShader&) = delete;

    void draw(CSGraphicsApi* api, const CSVertexArrayDraw& vertices);
};

#endif
