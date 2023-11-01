#ifndef __CDK__CSRenderCommand__
#define __CDK__CSRenderCommand__

#include "CSRect.h"
#include "CSSet.h"
#include "CSGraphicsResource.h"
#include "CSGraphicsApi.h"

class CSRenderCommand : public CSObject {
protected:
    virtual ~CSRenderCommand() {}
public:
    virtual int layer() const = 0;
    virtual const CSRenderTarget* target() const = 0;
    virtual const CSArray<CSRect>* bounds() const = 0;
    virtual bool submit() = 0;
    virtual bool parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const = 0;
    virtual bool findBatch(CSRenderCommand* command, CSRenderCommand*& candidate) const = 0;
    virtual void batch(CSRenderCommand* command) = 0;
    virtual void render(CSGraphicsApi* api, bool background, bool foreground) = 0;
};

#endif
