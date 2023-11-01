#ifndef __CDK__CSRenderBuffers__
#define __CDK__CSRenderBuffers__

#include "CSRenderBuffer.h"

class CSRenderBuffers {
public:
	static CSRenderBuffer* get(const CSObject* key, int life, bool recycle, const CSRenderBufferDescription& desc);
	static inline CSRenderBuffer* getTemporary(const CSRenderBufferDescription& desc) {
		return get(NULL, 1, true, desc);
	}
};

#endif
