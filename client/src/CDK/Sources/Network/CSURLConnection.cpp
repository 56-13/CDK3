#define CDK_IMPL

#include "CSURLConnection.h"

#include "CSURLConnectionBridge.h"

#include "CSData.h"

class CSAsyncURLConnection : public CSURLConnectionDelegate {
private:
    CSData* _data;
    CSURLConnection* _connection;
    std::function<void(CSData*, int)> _delegate;
    int _statusCode;
public:
    CSAsyncURLConnection(const CSURLRequest* request, const std::function<void(CSData*, int)>& delegate) :
        _data(NULL),
        _connection(new CSURLConnection(request)),
        _delegate(delegate),
        _statusCode(0)
    {
        _connection->setDelegate(this);
        _connection->start();
    }
private:
    ~CSAsyncURLConnection() {
        _connection->setDelegate(NULL);
        _connection->release();
        CSObject::release(_data);
    }
public:
    void urlConnectionError(CSURLConnection* connection) {
        _delegate(NULL, -1);
        delete this;
    }
    
    void urlConnectionReceiveResponse(CSURLConnection* connection, int statusCode) {
        _statusCode = statusCode;
    }
    
    void urlConnectionReceiveData(CSURLConnection* connection, const void* data, int length) {
        if (!_data) _data = new CSData();
        _data->appendBytes(data, length);
    }
    
    void urlConnectionFinishLoading(CSURLConnection* connection) {
        _delegate(_data, _statusCode);
        delete this;
    }
};

CSURLConnection::CSURLConnection(const CSURLRequest* request) : _request(retain(request)), _delegate(NULL) {
    _handle = CSURLConnectionBridge::createHandle(this, request);
}

CSURLConnection::~CSURLConnection() {
    CSURLConnectionBridge::destroyHandle(_handle);

	_request->release();
}

CSData* CSURLConnection::sendSynchronousRequest(const CSURLRequest* request, int* statusCode) {
    return CSURLConnectionBridge::sendSynchronousRequest(request, statusCode);
}

void CSURLConnection::sendAsynchronousRequest(const CSURLRequest* request, const std::function<void(CSData*, int)>& delegate) {
    new CSAsyncURLConnection(request, delegate);
}

void CSURLConnection::start() {
    CSURLConnectionBridge::start(_handle);
}

void CSURLConnection::cancel() {
    CSURLConnectionBridge::cancel(_handle);
}

void CSURLConnection::onError() {
    if (_delegate) _delegate->urlConnectionError(this);
}

void CSURLConnection::onReceiveResponse(int statusCode) {
    if (_delegate) _delegate->urlConnectionReceiveResponse(this, statusCode);
}

void CSURLConnection::onReceiveData(const void* data, int length) {
    if (_delegate) _delegate->urlConnectionReceiveData(this, data, length);
}

void CSURLConnection::onFinishLoading() {
    if (_delegate) _delegate->urlConnectionFinishLoading(this);
}
