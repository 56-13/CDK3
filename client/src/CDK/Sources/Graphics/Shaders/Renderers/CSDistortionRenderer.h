#ifndef __CDK__CSDistortionRenderer__
#define __CDK__CSDistortionRenderer__

#include "CSRenderer.h"

#include "CSProgramBranch.h"

class CSDistortionRenderer : public CSRenderer {
private:
    CSProgramBranch _programs;
public:
    CSDistortionRenderer();
private:
    ~CSDistortionRenderer() = default;
public:
    bool visible(const CSRenderState& state) const override;
    void validate(CSRenderState& state) const override;
    bool clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const override;
    void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) override;

    void begin(CSGraphics* graphics);
    void end(CSGraphics* graphics);
};

#endif
