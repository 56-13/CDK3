#ifndef __CDK__CSRenderTargets__
#define __CDK__CSRenderTargets__

#include "CSRenderTarget.h"

class CSRenderTargets {
public:
	static CSRenderTarget* get(const CSObject* key, int life, bool recycle, const CSRenderTargetDescription& desc);
	static inline CSRenderTarget* getTemporary(const CSRenderTargetDescription& desc) {
		return get(NULL, 1, true, desc);
	}
};

#endif
