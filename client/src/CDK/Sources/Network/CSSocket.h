#ifndef __CDK__CSSocket__
#define __CDK__CSSocket__

#include "CSBuffer.h"

#include "CSThread.h"

#ifdef CDK_WINDOWS
#include <WinSock2.h>
typedef SOCKET SOCKET_FD;
#else
typedef int SOCKET_FD;
#define INVALID_SOCKET	-1
#endif

class CSSocket;

enum CSSocketEvent {
    CSSocketEventConnected,
    CSSocketEventReceived,
    CSSocketEventSended,
    CSSocketEventTimeout,
    CSSocketEventError
};

class CSSocket : public CSObject {
public:
    typedef std::function<void(CSSocket*, CSSocketEvent)> Delegate;
private:
    char* _host;
    int _port;
    Delegate _delegate;
    CSTaskBase* _task;
	SOCKET_FD _socket;
    float _readTimeout;
    float _writeTimeout;
    double _readElapsed;
    double _writeElapsed;
    enum : byte {
        ConnectNone,
        ConnectWaiting,
        ConnectDone
    }_connect;
    bool _send;
    bool _receive;
    bool _tcpNoDelay;
public:
    CSSocket(const char* host, int port, const Delegate& delegate);
    template <class Object>
    CSSocket(const char* host, int port, Object* obj, void (Object::* method)(CSSocket*, CSSocketEvent)) : 
        CSSocket(host, port, [obj, method](CSSocket* socket, CSSocketEvent event) { obj->*method(socket, event); })
    {
    }
private:
    ~CSSocket();
public:
    static inline CSSocket* socket(const char* host, int port, const Delegate& delegate) {
        return autorelease(new CSSocket(host, port, delegate));
    }
    template <class Object>
    static inline CSSocket* socket(const char* host, int port, Object* obj, void (Object::* method)(CSSocket*, CSSocketEvent)) {
        return autorelease(new CSSocket(host, port, obj, method));
    }
    
    inline void setReadTimeout(float timeout) {
        _readTimeout = timeout;
    }
    inline float readTimeout() const {
        return _readTimeout;
    }
    inline void setWriteTimeout(float timeout) {
        _writeTimeout = timeout;
    }
    inline float writeTimeout() const {
        return _writeTimeout;
    }
    inline void setTcpNoDelay(bool tcpNoDelay) {
        _tcpNoDelay = tcpNoDelay;
    }
    inline bool tcpNoDelay() const {
        return _tcpNoDelay;
    }

    void open();
    void close();
    
    int read(CSBuffer* buffer);
    int write(CSBuffer* buffer);
private:
    void update();
};

#endif
