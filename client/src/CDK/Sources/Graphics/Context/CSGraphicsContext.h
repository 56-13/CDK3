#ifndef __CDK__CSGraphicsContext__
#define __CDK__CSGraphicsContext__

#include "CSGraphicsApi.h"
#include "CSArray.h"
#include "CSThread.h"
#include "CSDelegateRenderCommand.h"

class CSGraphics;

enum CSGraphicsPlatform {
    CSGraphicsPlatformOpenGL,
    CSGraphicsPlatformVulcan,
    CSGraphicsPlatformMetal
};

class CSGraphicsContext {
private:
    struct FocusedGraphics {
        pthread_t pid;
        CSGraphics* graphics;
    };
    CSArray<FocusedGraphics> _graphics;

    static CSGraphicsContext* _instance;
protected:
    CSGraphicsContext() = default;
    virtual ~CSGraphicsContext() = default;
public:
#ifdef CDK_IMPL
    static void initialize(CSGraphicsPlatform platform);
    static void finalize();
#endif
    static inline CSGraphicsContext* sharedContext() {
        return _instance;
    }
    CSGraphics* currentGraphics();
#ifdef CDK_IMPL
    void attachGraphics(CSGraphics* graphics);
    void detachGraphics(CSGraphics* graphics);
    void removeGraphics(CSGraphics* graphics);

    virtual void clearTargets(CSRenderTarget* target) = 0;
#endif
    virtual CSGraphicsPlatform platform() const = 0;
    virtual bool isSupportParallel() const = 0;
    virtual int maxUniformBlockSize() const = 0;
    virtual bool isSupportRawFormat(CSRawFormat format) const = 0;
    virtual bool isRenderThread(CSGraphicsApi** api = NULL) = 0;
    virtual CSGraphicsApi* attachRenderThread() = 0;
    virtual void detachRenderThread() = 0;
    virtual CSDelegateRenderCommand* invoke(bool gsync, const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0 = NULL, const CSObject* retain1 = NULL, const CSObject* retain2 = NULL) = 0;
};

#endif
