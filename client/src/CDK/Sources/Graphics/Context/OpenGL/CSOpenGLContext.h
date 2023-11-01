#ifdef CDK_IMPL

#ifndef __CDK__CSOpenGLContext__
#define __CDK__CSOpenGLContext__

#include "CSGraphicsContext.h"

#include "CSOpenGLApi.h"

class CSOpenGLContext : public CSGraphicsContext {
private:
	CSOpenGLApi _api;
	int _maxUniformBlockSize = 0;
	const char* _extensions = NULL;
public:
	CSOpenGLContext();

	inline CSGraphicsPlatform platform() const override {
		return CSGraphicsPlatformOpenGL;
	}
	inline bool isSupportParallel() const override {
		return false;
	}
	inline int maxUniformBlockSize() const override {
		return _maxUniformBlockSize;
	}
	void clearTargets(CSRenderTarget* target) override;
	bool isSupportRawFormat(CSRawFormat format) const override;
	bool isRenderThread(CSGraphicsApi** api = NULL) override;
	CSGraphicsApi* attachRenderThread() override;
	void detachRenderThread() override;
	CSDelegateRenderCommand* invoke(bool gsync, const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0 = NULL, const CSObject* retain1 = NULL, const CSObject* retain2 = NULL) override;
};

#endif

#endif