#ifdef CDK_WINDOWS

#ifdef CDK_IMPL

#ifndef __CDK__CSURLConnectionImpl__
#define __CDK__CSURLConnectionImpl__

#include "CSURLConnection.h"
#include "CSURLRequestImpl.h"

#include <pthread.h>
#include <curl/curl.h>

enum CSURLConnectionHandleState {
	CSURLConnectionHandleStateNone,
	CSURLConnectionHandleStateQueue,
	CSURLConnectionHandleStateWaiting,
	CSURLConnectionHandleStateReceiving
};

struct CSURLConnectionHandle {
	CSURLRequestHandle* request;
	CSURLConnection* target;
	CSURLConnectionHandleState state;
	CURL* curl;
	int readLength;
	int writeLength;

	CSURLConnectionHandle(CSURLRequestHandle* request, CSURLConnection* target);

	CSURLConnectionHandle(const CSURLConnectionHandle&) = delete;
	CSURLConnectionHandle& operator=(const CSURLConnectionHandle&) = delete;
};

class CSURLConnectionHandleManager {
private:
	struct Thread {
		CSURLConnectionHandleManager* parent;
		pthread_t tid;
		CSLock lock;
		bool active;
		bool alive;
	};
	CURLSH* _share;
	CSLock _connLock;
	CSArray<Thread*> _threads;
	CSArray<CSURLConnectionHandle*> _queue;

	static CSURLConnectionHandleManager* _instance;

	CSURLConnectionHandleManager();
	~CSURLConnectionHandleManager();
public:
	static inline CSURLConnectionHandleManager* sharedManager() {
		return _instance;
	}
	static void initialize();
	static void finalize();

	void start(CSURLConnectionHandle* handle);
	void cancel(CSURLConnectionHandle* handle);
private:
	static void lock_cb(CURL *handle, curl_lock_data data, curl_lock_access access, void *userptr);
	static void unlock_cb(CURL *handle, curl_lock_data data, void *userptr);

	static void* run(void* param);
public:
	static CSData* sendSynchronizedRequest(const CSURLRequestHandle* request, int* statusCode);
};

#endif

#endif

#endif