#ifdef CDK_IMPL

#ifndef __CDK__CSURLRequestBridge__
#define __CDK__CSURLRequestBridge__

#include "CSURLRequest.h"

class CSURLRequestBridge {
public:
	static void* createHandle(const char* url, const char* method, bool usingCache, float timeoutInterval);
	static void destroyHandle(void* handle);

	static void setURL(void* handle, const char* url);
	static const char* URL(void* handle);
	static void setMethod(void* handle, const char* method);
	static const char* method(void* handle);
	static void setUsingCache(void* handle, bool usingCache);
	static bool usingCache(void* handle);
	static void setTimeoutInterval(void* handle, float timeoutInterval);
	static float timeoutInterval(void* handle);
	static void setHeaderField(void* handle, const char* name, const char* value);
	static const char* headerField(void* handle, const char* name);
	static const CSDictionary<string, string>* headerFields(void* handle);
	static void setBody(void* handle, const void* data, int length);
	static const CSData* body(void* handle);
};

#endif

#endif
