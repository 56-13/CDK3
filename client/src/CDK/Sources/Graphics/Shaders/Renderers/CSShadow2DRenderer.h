#ifndef __CDK__CSShadow2DRenderer__
#define __CDK__CSShadow2DRenderer__

#include "CSRenderer.h"

#include "CSMatrix.h"

#include "CSProgramBranch.h"

class CSShadow2DRenderer : public CSRenderer {
public:
    class Param : public CSObject {
    public:
        CSMatrix viewProjection;
        CSVector3 lightDirection;
        CSPtr<CSGBuffer> uniformBuffer;

        Param(const CSMatrix& viewProjections, const CSVector3& lightDir);
    private:
        ~Param() = default;
    };
private:
    CSProgramBranch _programs;
public:
    CSShadow2DRenderer();
private:
    ~CSShadow2DRenderer() = default;
public:
    bool visible(const CSRenderState& state) const override;
    void validate(CSRenderState& state) const override;
    bool clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const override;
    void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) override;

    void begin(CSGraphics* graphics, const CSMatrix& viewProjection, const CSVector3& lightDir);
    void end(CSGraphics* graphics);
};

#endif
