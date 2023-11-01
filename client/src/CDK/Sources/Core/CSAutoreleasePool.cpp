#define CDK_IMPL

#include "CSAutoreleasePool.h"

#ifdef CDK_IOS
#include "CSIOSAutoreleasePool.h"
#endif

CSAutoreleasePool::CSAutoreleasePool(bool newThread) {
#ifdef CDK_IOS
    _iosAutoreleasePool = newThread ? CSIOSAutoreleasePool::create() : NULL;
#endif
}
CSAutoreleasePool::~CSAutoreleasePool() {
#ifdef CDK_IOS
    if (_iosAutoreleasePool) {
        CSIOSAutoreleasePool::destroy(_iosAutoreleasePool);
    }
#endif
}
void CSAutoreleasePool::add(const CSObject* obj) {
    _autoreleases.addObject(obj);
}
void CSAutoreleasePool::add(void* p) {
    _autofrees.addObject(p);
}
void CSAutoreleasePool::drain() {
    for (int i = 0; i < _autoreleases.count(); i++) _autoreleases.objectAtIndex(i)->release();
    for (int i = 0; i < _autofrees.count(); i++) free(_autofrees.objectAtIndex(i));
    _autoreleases.removeAllObjects();
    _autofrees.removeAllObjects();
#ifdef CDK_IOS
    if (_iosAutoreleasePool) {
        _iosAutoreleasePool = CSIOSAutoreleasePool::drain(_iosAutoreleasePool);
    }
#endif
}
