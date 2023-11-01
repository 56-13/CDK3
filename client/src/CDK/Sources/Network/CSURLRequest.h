#ifndef __CDK__CSURLRequest__
#define __CDK__CSURLRequest__

#include "CSString.h"

#include "CSDictionary.h"

#define CSURLRequestTimeoutDefault	30

class CSURLRequest : public CSObject {
private:
    void* _handle;

    inline CSURLRequest(void* handle) : _handle(handle) {}
    ~CSURLRequest();
public:
    static CSURLRequest* create(const char* url, const char* method = "GET", bool usingCache = true, float timeoutInterval = CSURLRequestTimeoutDefault);

    static inline CSURLRequest* request(const char* url, const char* method = "GET", bool usingCache = true, float timeoutInterval = CSURLRequestTimeoutDefault) {
        return autorelease(create(url, method, usingCache, timeoutInterval));
    }
    
    inline void* handle() const {
        return _handle;
    }
	void setURL(const char* url);
	const char* URL() const;

	void setMethod(const char* method);
	const char* method() const;

	void setUsingCache(bool usingCache);
	bool usingCache() const;

	void setTimeoutInterval(float timeoutInterval);
	float timeoutInterval() const;

	void setHeaderField(const char* name, const char* value);
	const char* headerField(const char* name) const;
	const CSDictionary<string, string>* headerFields() const;

	void setBody(const CSData* data);
	void setBody(const void* data, int length);
	const CSData* body() const;
};

#endif
