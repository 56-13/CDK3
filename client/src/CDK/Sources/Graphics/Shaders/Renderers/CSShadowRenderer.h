#ifndef __CDK__CSShadowRenderer__
#define __CDK__CSShadowRenderer__

#include "CSRenderer.h"

#include "CSMatrix.h"

#include "CSProgramBranch.h"

class CSShadowRenderer : public CSRenderer {
public:
    class Param : public CSObject {
    public:
        enum Mode {
            ModeDirection,
            ModePoint,
            ModeSpot
        };
        Mode mode;
        CSMatrix viewProjections[6];
        CSPtr<CSGBuffer> uniformBuffer;

        Param(Mode mode, const CSMatrix* viewProjections, CSGBuffer* uniformBuffer);
    private:
        ~Param() = default;
    };
private:
    CSProgramBranch _programs;
public:
    CSShadowRenderer();
private:
    ~CSShadowRenderer() = default;
public:
    bool visible(const CSRenderState& state) const override;
    void validate(CSRenderState& state) const override;
    bool clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const override;
    void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) override;

    void beginDirectional(CSGraphics* graphics, const CSMatrix& viewProjection);
    void beginPoint(CSGraphics* graphics, const CSMatrix* viewProjections, const CSVector3& pos, float range);
    void beginSpot(CSGraphics* graphics, const CSMatrix& viewProjection, const CSVector3& pos, float range);
    void end(CSGraphics* graphics);
};

#endif
