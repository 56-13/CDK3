#ifndef __CDK__CSRenderer__
#define __CDK__CSRenderer__

#include "CSVector3.h"
#include "CSRect.h"
#include "CSGBuffer.h"

struct CSRenderState;

class CSRenderer : public CSObject {
protected:
    CSRenderer() = default;
    virtual ~CSRenderer() = default;
public:
    virtual bool visible(const CSRenderState& state) const = 0;
    virtual void validate(CSRenderState& state) const = 0;
    virtual bool clip(const CSRenderState& state, const CSVector3* points, int count, CSRect& bounds) const = 0;
    virtual void setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance, bool usingVertexColor) = 0;
};

#endif
