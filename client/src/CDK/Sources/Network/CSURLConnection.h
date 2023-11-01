#ifndef __CDK__CSURLConnection__
#define __CDK__CSURLConnection__

#include "CSURLRequest.h"

class CSURLConnection;

class CSURLConnectionDelegate {
public:
    virtual void urlConnectionError(CSURLConnection* connection) = 0;
    virtual void urlConnectionReceiveResponse(CSURLConnection* connection, int statusCode) = 0;
    virtual void urlConnectionReceiveData(CSURLConnection* connection, const void* data, int length) = 0;
    virtual void urlConnectionFinishLoading(CSURLConnection* connection) = 0;
};

class CSURLConnection : public CSObject {
private:
	const CSURLRequest* _request;
    void* _handle;
    CSURLConnectionDelegate* _delegate;
public:
    CSURLConnection(const CSURLRequest* request);
private:
    ~CSURLConnection();
public:
    static inline CSURLConnection* connection(const CSURLRequest* request) {
        return autorelease(new CSURLConnection(request));
    }
    
    static CSData* sendSynchronousRequest(const CSURLRequest* request, int* statusCode = NULL);
    static void sendAsynchronousRequest(const CSURLRequest* request, const std::function<void(CSData*, int)>& delegate);
    template<class Object>
    static void sendAsynchronousRequest(const CSURLRequest* request, Object* obj, void (Object::*method)(CSData*, int)) {
        sendAsynchronousRequest(request, [obj, method](CSData* data, int statusCode) { obj->*method(data, statusCode); });
    }
    inline void setDelegate(CSURLConnectionDelegate* delegate) {
        _delegate = delegate;
    }
    void start();
    void cancel();
    
    void onError();
    void onReceiveResponse(int statusCode);
    void onReceiveData(const void* data, int length);
    void onFinishLoading();
};

#endif
