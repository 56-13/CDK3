#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSURLRequestImpl.h"

CSURLRequestHandle::CSURLRequestHandle(const char* url, const char* method, bool usingCache, float timeoutInterval) :
	url(_strdup(url)),
	method(_strdup(method)),
	usingCache(usingCache),
	timeoutInterval(timeoutInterval),
	body(NULL)
{

}

CSURLRequestHandle::~CSURLRequestHandle() {
	free(url);
	free(method);
	if (body) body->release();
}

#endif