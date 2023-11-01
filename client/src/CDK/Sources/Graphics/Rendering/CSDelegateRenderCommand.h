#ifndef __CDK__CSDelegateRenderCommand__
#define __CDK__CSDelegateRenderCommand__

#include "CSRenderCommand.h"

#include <functional>

class CSDelegateRenderCommand : public CSRenderCommand {
private:
    std::function<void(CSGraphicsApi*)> _inv;
    const CSObject* _retains[3];

    struct Fence {
        CSPtr<const CSGraphicsResource> resource;
        CSGBatchFlags flags;

        Fence(const CSGraphicsResource* resource, CSGBatchFlags flags);
    };
    CSArray<Fence> _fences;
public:
    CSDelegateRenderCommand(const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0 = NULL, const CSObject* retain1 = NULL, const CSObject* retain2 = NULL);
private:
    ~CSDelegateRenderCommand();
public:
    static inline CSDelegateRenderCommand* command(const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0 = NULL, const CSObject* retain1 = NULL, const CSObject* retain2 = NULL) {
        return autorelease(new CSDelegateRenderCommand(inv, retain0, retain1, retain2));
    }

    void addFence(const CSGraphicsResource* resource, CSGBatchFlags flags);

    inline int layer() const override {
        return 0;
    }
    inline const CSRenderTarget* target() const override {
        return NULL;
    }
    inline CSArray<CSRect>* bounds() const override {
        return NULL;
    }
    inline bool submit() override {
        return true;
    }
    
    bool parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const override;

    inline bool findBatch(CSRenderCommand* command, CSRenderCommand*& candidate) const override {
        return false;
    }
    inline void batch(CSRenderCommand* command) override {
        
    }
    void render(CSGraphicsApi* api, bool background, bool foreground) override;
};

#endif
