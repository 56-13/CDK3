#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSURLConnectionImpl.h"

#include "CSThread.h"

#include <algorithm>

#define MaxThread		20

#define MaxIdleTime		60

CSURLConnectionHandle::CSURLConnectionHandle(CSURLRequestHandle* request, CSURLConnection* target) : 
	request(request), 
	target(target), 
	state(CSURLConnectionHandleStateNone), 
	curl(NULL), 
	readLength(0),
	writeLength(0) 
{

}

//========================================================================================================

CSURLConnectionHandleManager* CSURLConnectionHandleManager::_instance = NULL;

CSURLConnectionHandleManager::CSURLConnectionHandleManager() {
	curl_global_init(CURL_GLOBAL_ALL);
	_share = curl_share_init();

	curl_share_setopt(_share, CURLSHOPT_LOCKFUNC, CSURLConnectionHandleManager::lock_cb);
	curl_share_setopt(_share, CURLSHOPT_UNLOCKFUNC, CSURLConnectionHandleManager::unlock_cb);
	curl_share_setopt(_share, CURLSHOPT_SHARE, CURL_LOCK_DATA_CONNECT);
	curl_share_setopt(_share, CURLSHOPT_USERDATA, this);
}

CSURLConnectionHandleManager::~CSURLConnectionHandleManager() {
	synchronized(&_threads) {
		for (int i = 0; i < _threads.count(); i++) {
			Thread* thread = _threads.objectAtIndex(i);
			thread->alive = false;
			synchronized(thread->lock) {
				thread->lock.pulse();
			}
			pthread_join(thread->tid, NULL);
			delete thread;
		}
		_threads.removeAllObjects();
	}
	synchronized(&_queue) {
		for (int i = 0; i < _queue.count(); i++) {
			CSURLConnectionHandle* handle = _queue.objectAtIndex(i);

			CSAssert(handle->state == CSURLConnectionHandleStateQueue, "invalid state");

			handle->state = CSURLConnectionHandleStateNone;
			handle->curl = NULL;
			handle->readLength = 0;
			handle->writeLength = 0;
		}
		_queue.removeAllObjects();
	}
	curl_share_cleanup(_share);
	curl_global_cleanup();
}

void CSURLConnectionHandleManager::initialize() {
	if (!_instance) _instance = new CSURLConnectionHandleManager();
}

void CSURLConnectionHandleManager::finalize() {
	if (_instance) {
		delete _instance;
		_instance = NULL;
	}
}

void CSURLConnectionHandleManager::start(CSURLConnectionHandle* handle) {
	synchronized(&_queue) {
		if (handle->state != CSURLConnectionHandleStateNone) return;

		_queue.addObject(handle);

		handle->state = CSURLConnectionHandleStateQueue;
	}

	synchronized(&_threads) {
		bool isNew = true;
		for (int i = 0; i < _threads.count(); i++) {
			Thread* thread = _threads.objectAtIndex(i);

			synchronized(thread->lock) {
				if (!thread->active && thread->alive) {
					thread->active = true;
					thread->lock.pulse();
					isNew = false;
					break;
				}
			}
		}
		if (isNew && _threads.count() < MaxThread) {
			Thread* thread = new Thread();
			thread->parent = this;
			thread->active = true;
			thread->alive = true;
				
			pthread_create(&thread->tid, NULL, CSURLConnectionHandleManager::run, thread);

			_threads.addObject(thread);
		}
	}
}

void CSURLConnectionHandleManager::cancel(CSURLConnectionHandle* handle) {
	synchronized(&_queue) {
		switch (handle->state) {
			case CSURLConnectionHandleStateQueue:
				_queue.removeObjectIdenticalTo(handle);
				handle->state = CSURLConnectionHandleStateNone;
			case CSURLConnectionHandleStateNone:
				return;
		}
	}
	while(handle->state == CSURLConnectionHandleStateNone) Sleep(100);
}

void CSURLConnectionHandleManager::lock_cb(CURL *handle, curl_lock_data data, curl_lock_access access, void *userptr) {
	CSURLConnectionHandleManager* manager = static_cast<CSURLConnectionHandleManager*>(userptr);
	manager->_connLock.lock();
}

void CSURLConnectionHandleManager::unlock_cb(CURL *handle, curl_lock_data data, void *userptr) {
	CSURLConnectionHandleManager* manager = static_cast<CSURLConnectionHandleManager*>(userptr);
	manager->_connLock.unlock();
}

static size_t read_cb(void* dest, size_t size, size_t nmemb, void* data) {
	CSURLConnectionHandle* connection = static_cast<CSURLConnectionHandle*>(data);

	CSAssert(connection->state >= CSURLConnectionHandleStateWaiting, "invalid state");

	size_t bufferSize = size * nmemb;

	if (connection->readLength < connection->request->body->length()) {
		size_t size = connection->request->body->length() - connection->readLength;
		
		if (size > bufferSize) size = bufferSize;

		const byte* src = (const byte*)connection->request->body->bytes() + connection->readLength;

		memcpy(dest, src, size);

		connection->readLength += size;

		return size;
	}

	return 0;
}

static void performResponse(CSURLConnectionHandle* connection) {
	if (connection->state == CSURLConnectionHandleStateWaiting) {
		long statusCode;
		curl_easy_getinfo(connection->curl, CURLINFO_RESPONSE_CODE, &statusCode);

		if (statusCode != 200) {
			CSErrorLog("status code failed: %d", statusCode);
		}

		auto inv = [connection, statusCode]() {
			connection->target->onReceiveResponse(statusCode);
		};
		CSThread::mainThread()->run<void>(inv);

		connection->state = CSURLConnectionHandleStateReceiving;
	}
}

static size_t write_cb(void* src, size_t size, size_t nmemb, void* data) {
	CSURLConnectionHandle* connection = static_cast<CSURLConnectionHandle*>(data);

	CSAssert(connection->state >= CSURLConnectionHandleStateWaiting, "invalid state");

	performResponse(connection);

	size_t total = size * nmemb;

	void* copy = malloc(total);

	if (!copy) return 0;

	connection->writeLength += total;

	memcpy(copy, src, total);
	{
		auto inv = [connection, copy, total]() {
			connection->target->onReceiveData(copy, total);

			free(copy);
		};
		CSThread::mainThread()->run<void>(inv);
	}

	return total;
}

void* CSURLConnectionHandleManager::run(void* param) {
	Thread* thread = static_cast<Thread*>(param);

	for (; ; ) {
		CSURLConnectionHandle* connection = NULL;
		for (; ; ) {
			if (!thread->alive) return NULL;

			if (!thread->active) {
				synchronized(&thread->parent->_threads) {
					thread->parent->_threads.removeObjectIdenticalTo(thread);
				}
				delete thread;

				return NULL;
			}

			synchronized(&thread->parent->_queue) {
				if (thread->parent->_queue.count()) {
					connection = thread->parent->_queue.lastObject();
					thread->parent->_queue.removeLastObject();
					connection->state = CSURLConnectionHandleStateWaiting;
				}
			}
			synchronized(thread->lock) {
				if (connection) {
					thread->active = true;
					goto request;
				}
				else {
					thread->active = false;
					thread->lock.wait(MaxIdleTime);
				}
			}
		}

	request:

		CURL *curl = curl_easy_init();
		curl_easy_setopt(curl, CURLOPT_CUSTOMREQUEST, connection->request->method);
		curl_easy_setopt(curl, CURLOPT_URL, connection->request->url);
		curl_easy_setopt(curl, CURLOPT_TIMEOUT_MS, (long)(connection->request->timeoutInterval * 1000));

		struct curl_slist* header = NULL;
		header = curl_slist_append(header, connection->request->usingCache ? "Cache-Control: max-age=29030400" : "Cache-Control: no-store");

		for (CSDictionary<string, string>::ReadonlyIterator i = connection->request->headerFields.iterator(); i.remaining(); i.next()) {
			const char* key = i.key();
			const char* value = i.object();
			char* buf = (char*)alloca(strlen(key) + 2 + strlen(value) + 1);
			sprintf(buf, "%s: %s", key, value);
			header = curl_slist_append(header, buf);
		}
		curl_easy_setopt(curl, CURLOPT_HTTPHEADER, header);
		//curl_easy_setopt(curl, CURLOPT_VERBOSE, 0L);

		if (connection->request->body) {
			curl_easy_setopt(curl, CURLOPT_POST, 1L);
			curl_easy_setopt(curl, CURLOPT_READFUNCTION, read_cb);
			curl_easy_setopt(curl, CURLOPT_READDATA, connection);
			curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, (long)connection->request->body->length());
		}

		curl_easy_setopt(curl, CURLOPT_SHARE, thread->parent->_share);
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_cb);
		curl_easy_setopt(curl, CURLOPT_WRITEDATA, connection);
		
		if (strnicmp(connection->request->url, "https", 5) == 0) {
			curl_easy_setopt(curl, CURLOPT_USE_SSL, (long)CURLUSESSL_ALL);
			curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
			//curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);
		}

		connection->curl = curl;

		CURLcode res = curl_easy_perform(curl);

		if (header) curl_slist_free_all(header);

		bool success = true;

		if (res == CURLE_OK) {
			double cl;
			res = curl_easy_getinfo(curl, CURLINFO_CONTENT_LENGTH_DOWNLOAD, &cl);
			if (res == CURLE_OK && cl != -1 && cl != connection->writeLength) {
				CSErrorLog("content-length failed: %d / %d", connection->writeLength, (int)cl);
				success = false;
			}
		}
		else {
			CSErrorLog("curl_easy_perform() failed: %s", curl_easy_strerror(res));
			success = false;
		}

		if (success) performResponse(connection);

		connection->state = CSURLConnectionHandleStateNone;
		connection->curl = NULL;
		connection->readLength = 0;
		connection->writeLength = 0;

		if (success) {
			auto inv = [connection]() {
				connection->target->onFinishLoading();
			};
			CSThread::mainThread()->run<void>(inv);
		}
		else {
			auto inv = [connection]() {
				connection->target->onError();
			};
			CSThread::mainThread()->run<void>(inv);
		}
				
		curl_easy_cleanup(curl);
	}
}

static size_t write_synchronized_cb(void *ptr, size_t size, size_t nmemb, void *data) {
	CSData* d = static_cast<CSData*>(data);

	size_t total = size * nmemb;

	d->appendBytes(ptr, total);

	return total;
}

CSData* CSURLConnectionHandleManager::sendSynchronizedRequest(const CSURLRequestHandle* request, int* statusCode) {
	CSData* data = CSData::data();

	CURL *curl = curl_easy_init();
	curl_easy_setopt(curl, CURLOPT_CUSTOMREQUEST, request->method);
	curl_easy_setopt(curl, CURLOPT_URL, request->url);
	curl_easy_setopt(curl, CURLOPT_TIMEOUT_MS, (long)(request->timeoutInterval * 1000));

	struct curl_slist* header = NULL;
	header = curl_slist_append(header, request->usingCache ? "Cache-Control: max-age=29030400" : "Cache-Control: no-store");

	for (CSDictionary<string, string>::ReadonlyIterator i = request->headerFields.iterator(); i.remaining(); i.next()) {
		const char* str = string::cstringWithFormat("%s: %s", i.key().cstring(), i.object().cstring());

		header = curl_slist_append(header, str);
	}
	curl_easy_setopt(curl, CURLOPT_HTTPHEADER, header);
	//curl_easy_setopt(curl, CURLOPT_VERBOSE, 0L);
	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_synchronized_cb);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, data);

	if (strnicmp(request->url, "https", 5) == 0) {
		curl_easy_setopt(curl, CURLOPT_USE_SSL, (long)CURLUSESSL_ALL);
		curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L);
		//curl_easy_setopt(curl, CURLOPT_SSL_VERIFYHOST, 0L);
	}

	CURLcode res = curl_easy_perform(curl);

	if (header) curl_slist_free_all(header);

	if (res == CURLE_OK) {
		long resCode;
		curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &resCode);
		if (statusCode) *statusCode = resCode;
		if (resCode == 200) {
			double cl;
			res = curl_easy_getinfo(curl, CURLINFO_CONTENT_LENGTH_DOWNLOAD, &cl);
			if (res == CURLE_OK && cl != -1 && cl != data->length()) {
				CSErrorLog("content-length failed: %d / %d", data->length(), (int)cl);
				data = NULL;
			}
		}
		else {
			CSErrorLog("status code failed: %d", resCode);
			data = NULL;
		}
	}
	else {
		CSErrorLog("curl_easy_perform() failed: %s", curl_easy_strerror(res));
		if (statusCode) *statusCode = -1;
		data = NULL;
	}

	curl_easy_cleanup(curl);

	return data;
}

#endif