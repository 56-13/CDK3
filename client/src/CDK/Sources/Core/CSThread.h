#ifndef __CDK__CSThread__
#define __CDK__CSThread__

#include "CSString.h"
#include "CSArray.h"
#include "CSTask.h"
#include "CSTime.h"
#include "CSAutoreleasePool.h"

void pthread_empty(pthread_t& t);
bool pthread_exists(const pthread_t& t);

class CSThreadPool;

class CSThread : public CSObject {
private:
    string _name;
    CSArray<CSTaskBase> _tasks;
    CSAutoreleasePool* _autoreleasePool = NULL;
    pthread_t _pthread = {};
    double _timeStarted = 0;
    double _timeStamp = 0;
    double _timeUsed = 0;
    uint64 _timeSeq = 0;
	bool _alive= false;
    bool _paused = false;
    bool _pooling = false;
    bool _idle = false;
    bool _external = false;
public:
    CSThread(const string& name = string(), const pthread_t* pthread = NULL);
private:
    ~CSThread();
public:
    static inline CSThread* thread(const string& name = string(), const pthread_t* pthread = NULL) {
        return autorelease(new CSThread(name, pthread));
    }
    
    bool isMainThread() const;
    bool isRenderThread() const;

    static CSThread* mainThread();
    static CSThread* renderThread();
    static CSThread* currentThread();

    static void yield();
    static void sleep(float timeout);
    
    inline const string& name() const {
        return _name;
    }
    inline bool isAlive() const {
		return _alive;
    }
    inline bool isActive() const {
        return pthread_equal(_pthread, pthread_self()) != 0;
    }
    inline bool isPooling() const {
        return _pooling;
    }
    inline bool isIdle() const {
        return _idle;
    }
    inline double timeStamp() const {
        return _timeStamp;
    }
    inline double timeUsed() const {
        return _timeUsed;
    }
    inline double timeTotal() const {
        return _timeStamp - _timeStarted;
    }
	inline uint64 timeSequence() const {
		return _timeSeq;
	}
    inline double timeAverage() const {
        return _timeSeq ? timeTotal() / _timeSeq : 0.0;
    }
    inline void timeReset() {
        _timeStarted = CSTime::currentTime();
        _timeSeq = 0;
    }

    inline CSAutoreleasePool* autoreleasePool() {
        CSAssert(isActive(), "invalid operation");
        return _autoreleasePool;
    }

    void run(CSTaskBase* task);

    template <typename Result>
    CSTask<Result>* run(const std::function<Result()>& func, float delay = 0, bool repeat = false) {
        CSTask<Result>* task = new CSTask<Result>(func, delay, repeat);
        run(task);
        task->release();
        return task;
    }
    template <typename Result, class Object>
    CSTask<Result>* run(Object* obj, Result(Object::* method)(), float delay = 0, bool repeat = false) {
        CSTask<Result>* task = new CSTask<Result>(obj, method, delay, repeat);
        run(task);
        task->release();
        return task;
    }

    void start();
	void stop();
    void keepAlive();
#ifdef CDK_IMPL
    float execute();
    void notifyPause();
    void notifyResume();
    static void notifyPauseApp();
    static void notifyResumeApp();
    static void initialize(const pthread_t* ptrender);
    static void finalize();
    static void reloadRenderThread();
    static void* run(void* thread);
    static void* runDeadLockDetection(void* param);
#endif
    friend class CSThreadPool;
};

class CSThreadPool {
public:
    static void run(CSTaskBase* task);

    template <typename Result>
    static CSTask<Result>* run(const std::function<Result()>& func, float delay = 0, bool repeat = false) {
        CSTask<Result>* task = new CSTask<Result>(func, delay, repeat);
        run(task);
        task->release();
        return task;
    }
    template <typename Result, class Object>
    static CSTask<Result>* run(Object* obj, Result(Object::* method)(), float delay = 0, bool repeat = false) {
        CSTask<Result>* task = new CSTask<Result>(obj, method, delay, repeat);
        run(task);
        task->release();
        return task;
    }

    static bool execute();
};

#endif
