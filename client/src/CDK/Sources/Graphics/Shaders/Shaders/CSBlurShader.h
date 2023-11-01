#ifndef __CDK__CSBlurShader__
#define __CDK__CSBlurShader__

#include "CSProgramBranch.h"
#include "CSRenderTarget.h"
#include "CSVertexArrayDraw.h"
#include "CSCamera.h"

class CSBlurShader {
private:
    CSProgramBranch _programs;
public:
    CSBlurShader();

    CSBlurShader(const CSBlurShader&) = delete;
    CSBlurShader& operator =(const CSBlurShader&) = delete;

    void draw(CSGraphicsApi* api, const CSVector2* quad, float intensity);
    void draw(CSGraphicsApi* api, float intensity);
    void draw(CSGraphicsApi* api, const CSBounds2& bounds, float intensity);
    void drawCube(CSGraphicsApi* api, float intensity);
    void drawDepth(CSGraphicsApi* api, const CSVector2* quad, const CSCamera& camera, float distance, float range, float intensity);
    void drawDepth(CSGraphicsApi* api, const CSCamera& camera, float distance, float range, float intensity);
    void drawDirection(CSGraphicsApi* api, const CSVector2* quad, const CSVector2& dir);
    void drawDirection(CSGraphicsApi* api, const CSVector2& dir);
    void drawCenter(CSGraphicsApi* api, const CSVector2* quad, const CSVector2& center, float range);
    void drawCenter(CSGraphicsApi* api, const CSVector2& center, float range);
private:    
    void draw(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, float intensity, bool cube);
    void drawDepth(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSCamera& camera, float distance, float range, float intensity);
    void drawDirection(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSVector2& dir);
    void drawCenter(CSGraphicsApi* api, const CSVertexArrayDraw& vertices, const CSBounds2& bounds, const CSVector2& center, float range);
};

#endif
