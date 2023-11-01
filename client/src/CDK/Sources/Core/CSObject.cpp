#define CDK_IMPL

#include "CSThread.h"
#include "CSDiagnostics.h"

static CSLock __atomicLock;
static CSArray<void*> __newReserved;

static void addNew(void* p) {
    __atomicLock.lock();
    __newReserved.addObject(p);
    __atomicLock.unlock();

    CSDiagnostics::objectNew();
}

static bool getNew(void* p) {
    __atomicLock.lock();
    bool result = __newReserved.removeObjectIdenticalTo(p);
    __atomicLock.unlock();
    return result;
}

//===================================================================

CSObject::CSObject() : _retainCount(getNew(this)), _lock(NULL) {
    
}

CSObject::~CSObject() {
    CSAssert(retainCount() == 0);
    if (_lock) delete _lock;
}

void CSObject::release() const {
    CSAssert(retainCount() != 0, "not retainable object");
    if (_retainCount.fetch_sub(1, std::memory_order_acq_rel) == 1) {
        CSDiagnostics::objectDelete();

        delete this;
    }
}

void CSObject::retain() const {
    CSAssert(retainCount() != 0, "not retainable object");
    _retainCount.fetch_add(1, std::memory_order_relaxed);
}

void CSObject::autorelease() const {
    CSAssert(retainCount() != 0, "not retainable object");
    CSThread::currentThread()->autoreleasePool()->add(this);
}

int CSObject::retainCount() const {
    return _retainCount.load(std::memory_order_acquire);
}

void* CSObject::operator new(size_t size) {
    void* ptr = ::operator new(size);
    addNew(ptr);
    return ptr;
}

string CSObject::toString() const {
    return string::format("%s <%p>", typeid(*this).name(), this);
}

void CSObject::useLock() const {
    if (!_lock) {
        __atomicLock.lock();
        if (!_lock) _lock = new CSLock(true);
        __atomicLock.unlock();
    }
}
