#define CDK_IMPL

#include "CSThread.h"
#include "CSStringLiteral.h"
#include "CSTime.h"
#include "CSLog.h"
#include "CSMath.h"
#include "CSQueue.h"
#include "CSAutoreleasePool.h"
#include "CSTask.h"
#include "CSDiagnostics.h"

#include <algorithm>
#include <thread>

#ifdef CDK_WINDOWS
#include <windows.h>
#include <sched.h>
#else
#include <unistd.h>
#ifdef CDK_ANDROID
#include <sched.h>
#include "CSJNI.h"
#endif
#endif

static constexpr float KeepAliveDuration = 30;

struct ThreadContext {
    CSThread* mainThread;
    CSThread* renderThread;
    CSLock threadLock;
    CSArray<CSThread*> threads;
    CSArray<CSThread*> poolingThreads;
    CSQueue<CSTaskBase> poolingTasks;
#ifdef CS_DEADLOCK_DETECTION
    pthread_t deadLockDetection;
#endif
    bool alive;
    bool paused;

    ThreadContext(const pthread_t* ptrender);
    ~ThreadContext();

    ThreadContext(const ThreadContext&) = delete;
    ThreadContext& operator =(const ThreadContext&) = delete;
};

static ThreadContext* __context = NULL;

void pthread_empty(pthread_t& t) {
#ifdef CDK_WINDOWS
	t.p = NULL;
	t.x = 0;
#else
	t = 0;
#endif
}

bool pthread_exists(const pthread_t& t) {
#ifdef CDK_WINDOWS
	return t.p != NULL;
#else
	return t != 0;
#endif
}

CSThread::CSThread(const string& name, const pthread_t* pthread) : _name(name) {
    if (pthread) {
        _pthread = *pthread;
        _external = true;
        _autoreleasePool = new CSAutoreleasePool(false);
    }
}

CSThread::~CSThread() {
    if (_autoreleasePool) delete _autoreleasePool;
}

bool CSThread::isMainThread() const {
    return this == __context->mainThread;
}

bool CSThread::isRenderThread() const {
    return this == __context->renderThread;
}

CSThread* CSThread::mainThread() {
    return __context->mainThread;
}

CSThread* CSThread::renderThread() {
    return __context->renderThread;
}

CSThread* CSThread::currentThread() {
    pthread_t pt = pthread_self();
    if (pthread_equal(pt, __context->mainThread->_pthread)) return __context->mainThread;
    if (pthread_equal(pt, __context->renderThread->_pthread)) return __context->renderThread;
    synchronized(__context->threadLock) {
        foreach (CSThread*, thread, &__context->threads) {
            if (pthread_equal(pt, thread->_pthread)) return thread;
        }
    }
    return NULL;
}

void CSThread::yield() {
    sched_yield();
}

void CSThread::sleep(float timeout) {
#ifdef CDK_WINDOWS
	Sleep(timeout * 1000);
#else
    usleep(timeout * 1000000);
#endif
}

void CSThread::run(CSTaskBase* task) {
    if (task->start()) {
        synchronized(this) {
            _tasks.addObject(task);
            if (!_external) pulse();
        }
    }
}

float CSThread::execute() {
#ifdef CS_DIAGNOSTICS
    double startTime = _name ? CSTime::currentTime() : 0;
#endif
    double minNextElapsed = DoubleMax;
    bool exec = false;

    int i = 0;
    
	for (;;) {
		CSTaskBase* task = NULL;
        bool pooling = false;

        if (i < _tasks.count()) {
            synchronized(this) {
                if (i < _tasks.count()) task = _tasks.objectAtIndex(i);
            }
        }
        if (!task && _pooling && __context->poolingTasks.count()) {
            synchronized(&__context->poolingTasks) {
                if (__context->poolingTasks.count()) {
                    task = __context->poolingTasks.firstObject();
                    task->retain();
                    __context->poolingTasks.removeFirstObject();
                    pooling = true;
                }
            }
        }
        if (!task) goto fin;

		double nextElapsed = task->execute();

        exec = true;

        if (nextElapsed) {
			if (nextElapsed < minNextElapsed) minNextElapsed = nextElapsed;
            if (pooling) {
                synchronized(&__context->poolingTasks) {
                    __context->poolingTasks.addObject(task);
                }
                task->release();
            }
            else i++;
		}
		else {
            if (pooling) task->release();
            else {
                synchronized(this) {
                    _tasks.removeObjectAtIndex(i);
                }
            }
		}
	}

fin:
    _autoreleasePool->drain();

    _timeStamp = CSTime::currentTime();;
    if (exec) _timeUsed = _timeStamp;
    else if (_pooling && _timeStamp - _timeUsed > KeepAliveDuration) _alive = false;
    _timeSeq++;

#ifdef CS_DIAGNOSTICS
    if (_name) CSDiagnostics::timeAdd(_name, _timeStamp - startTime, false);
#endif

    return minNextElapsed != DoubleMax ? CSMath::max((float)(minNextElapsed - _timeStamp), 0.001f) : 0.0f;
}

void* CSThread::run(void* param) {
    CSThread* thread = (CSThread*)param;
    CSAssert(!thread->_external);
    thread->_pthread = pthread_self();
    thread->_autoreleasePool = new CSAutoreleasePool(true);
    
    synchronized(__context->threadLock) {
        __context->threads.addObject(thread);
    }
	for (;;) {
        float remaining = thread->execute();
        
        synchronized(thread) {
            thread->_idle = true;

            if (!thread->_alive) {
                thread->_idle = false;
                goto fin;
            }
            else if (remaining > 0) thread->wait(remaining);
            else if (thread->_pooling) thread->wait(KeepAliveDuration);
			else thread->wait();

            thread->_idle = false;
        }
    }
fin:
    if (thread->_pooling) {
        synchronized(&__context->poolingThreads) {
            __context->poolingThreads.removeObjectIdenticalTo(thread);
        }
    }
    synchronized(__context->threadLock) {
        __context->threads.removeObjectIdenticalTo(thread);
    }
    
#ifdef CDK_ANDROID
    CSJNI::detachThread();
#endif
    
    delete thread->_autoreleasePool;
    thread->_autoreleasePool = NULL;
    thread->release();
    return NULL;
}

void CSThread::start() {
    if (!_alive) {
        _timeStarted = _timeStamp = _timeUsed = CSTime::currentTime();
		_alive = true;

        if (!_external) {
            retain();

            pthread_create(&_pthread, NULL, &CSThread::run, this);
        }
    }
}

void CSThread::stop() {
	if (_alive) {
		_alive = false;
        if (!_external) {
            synchronized(this) {
                pulse();
            }
            pthread_join(_pthread, NULL);
        }
	}
}

void CSThread::keepAlive() {
    _timeStamp = _timeUsed = CSTime::currentTime();
}

void CSThread::notifyPause() {
    _paused = true;
}

void CSThread::notifyResume() {
    _paused = false;
    _timeStamp = _timeUsed = CSTime::currentTime();
}

void CSThread::notifyPauseApp() {
    synchronized(__context->threadLock) {
        __context->paused = true;
        __context->threadLock.pulse();
    }
}

void CSThread::notifyResumeApp() {
    synchronized(__context->threadLock) {
        double currentTime = CSTime::currentTime();
        if (!__context->mainThread->_paused) {
            __context->mainThread->_timeStarted = __context->mainThread->_timeStamp = __context->mainThread->_timeUsed = currentTime;
        }
        if (!__context->renderThread->_paused) {
            __context->renderThread->_timeStarted = __context->renderThread->_timeStamp = __context->renderThread->_timeUsed = currentTime;
        }
        foreach (CSThread*, thread, &__context->threads) {
            if (!thread->_paused) {
                thread->_timeStarted = thread->_timeStamp = thread->_timeUsed = currentTime;
            }
        }
        __context->paused = false;
        __context->threadLock.pulse();
    }
}

ThreadContext::ThreadContext(const pthread_t* ptrender) : poolingThreads(0), alive(true), paused(false) {
    pthread_t ptmain = pthread_self();
    mainThread = new CSThread(S("MainThread"), &ptmain);
    mainThread->start();
    renderThread = new CSThread(S("RenderThread"), ptrender);
    renderThread->start();

    int hardwareConcurrency = std::thread::hardware_concurrency();
    CSAssert(hardwareConcurrency > 0);
    int poolingThreadCapacity = CSMath::max(hardwareConcurrency * 2 - 1, 3);            //concurrency * 2 + 1 - (main thread + render thread)
    poolingThreads.setCapacity(poolingThreadCapacity);
    CSLog("thread pool initialized:%d / %d", hardwareConcurrency, poolingThreadCapacity);

#ifdef CS_DEADLOCK_DETECTION
    pthread_create(&deadLockDetection, NULL, &CSThread::runDeadLockDetection, NULL);
#endif
}

ThreadContext::~ThreadContext() {
    alive = false;

    while (poolingThreads.count()) {
        CSThread* thread = NULL;
        synchronized(&__context->poolingThreads) {
            thread = CSObject::retain(__context->poolingThreads.lastObject());
        }
        if (thread) {
            thread->stop();
            thread->release();
        }
        else break;
    }

    CSAssert(!threads.count());

    renderThread->stop();
    mainThread->stop();

#ifdef CS_DEADLOCK_DETECTION
    synchronized(threadLock) {
        threadLock.pulse();
    }
    pthread_join(deadLockDetection, NULL);
#endif

    renderThread->release();
    mainThread->release();
}

void CSThread::initialize(const pthread_t* ptrender) {
    if (!__context) __context = new ThreadContext(ptrender);
}

void CSThread::finalize() {
    if (__context) {
        delete __context;
        __context = NULL;
    }
}

void CSThread::reloadRenderThread() {           //shoud be render thread
    __context->renderThread->_pthread = pthread_self();
}

#ifdef CS_DEADLOCK_DETECTION
void* CSThread::runDeadLockDetection(void* param) {
    for (; ; ) {
        double currentTime = CSTime::currentTime();
        
        synchronized(__context->threadLock) {
            if (!__context->alive) goto exit;
            if (!__context->mainThread->_paused && currentTime - __context->mainThread->_timeStamp > KeepAliveDuration) {
                abort();
                goto exit;
            }
            if (!__context->renderThread->_paused && currentTime - __context->renderThread->_timeStamp > KeepAliveDuration) {
                abort();
                goto exit;
            }
            foreach (CSThread*, thread, &__context->threads) {
                if (!thread->_paused && currentTime - thread->_timeStamp > KeepAliveDuration) {
                    abort();
                    goto exit;
                }
            }
            if (!__context->paused) __context->threadLock.wait(5);
			if (!__context->alive) goto exit;
            if (__context->paused) __context->threadLock.wait();
        }
    }
exit:
    return NULL;
}
#endif

void CSThreadPool::run(CSTaskBase* task) {
    synchronized(&__context->poolingTasks) {
        __context->poolingTasks.addObject(task);
    }
    synchronized(&__context->poolingThreads) {
        foreach (CSThread*, thread, &__context->poolingThreads) {
            if (thread->_idle) {
                synchronized(thread) {
                    if (thread->_idle) {
                        thread->pulse();
                        return;
                    }
                }
            }
        }
        if (__context->poolingThreads.count() < __context->poolingThreads.capacity()) {
            CSThread* thread = new CSThread(S("PoolingThread"));
            thread->_pooling = true;
            __context->poolingThreads.addObject(thread);
            thread->start();
            thread->release();
        }
    }
}

bool CSThreadPool::execute() {
    if (__context->poolingTasks.count()) {
        CSTaskBase* task = NULL;
        synchronized(&__context->poolingTasks) {
            if (__context->poolingTasks.count()) {
                task = __context->poolingTasks.firstObject();
                task->retain();
                __context->poolingTasks.removeFirstObject();
            }
        }
        if (task) {
            if (task->execute()) {
                synchronized(&__context->poolingTasks) {
                    __context->poolingTasks.addObject(task);
                }
            }
            task->release();
            return true;
        }
    }
    return false;
}
