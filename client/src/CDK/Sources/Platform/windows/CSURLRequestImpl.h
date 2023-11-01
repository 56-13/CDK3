#ifdef CDK_WINDOWS

#ifdef CDK_IMPL

#ifndef __CDK__CSURLRequestImpl__
#define __CDK__CSURLRequestImpl__

#include "CSDictionary.h"

#include "CSData.h"

struct CSURLRequestHandle {
	char* url;
	char* method;
	bool usingCache;
	CSDictionary<string, string> headerFields;
	float timeoutInterval;
	const CSData* body;

	CSURLRequestHandle(const char* url, const char* method, bool usingCache, float timeoutInterval);
	~CSURLRequestHandle();

	CSURLRequestHandle(const CSURLRequestHandle&) = delete;
	CSURLRequestHandle& operator=(const CSURLRequestHandle&) = delete;
};

#endif

#endif

#endif


