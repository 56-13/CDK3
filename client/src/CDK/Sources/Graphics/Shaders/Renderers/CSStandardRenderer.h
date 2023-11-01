#ifndef __CDK__CSStandardRenderer__
#define __CDK__CSStandardRenderer__

#include "CSRenderer.h"

#include "CSProgramBranch.h"

class CSStandardRenderer : public CSRenderer {
private:
    CSProgramBranch _programs;
public:
    CSStandardRenderer();
private:
    ~CSStandardRenderer() = default;
public:
    bool visible(const CSRenderState& state) const override;
    void validate(CSRenderState& state) const override;
    bool clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const override;
    void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) override;
};

#endif
