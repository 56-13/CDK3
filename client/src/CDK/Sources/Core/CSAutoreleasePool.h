#ifndef __CDK__CSAutoreleasePool__
#define __CDK__CSAutoreleasePool__

#include "CSArray.h"

class CSAutoreleasePool {
private:
    CSArray<const CSObject*> _autoreleases;
    CSArray<void*> _autofrees;
#ifdef CDK_IOS
    void* _iosAutoreleasePool;
#endif
#ifdef CDK_IMPL
public:
#else
private:
#endif
    CSAutoreleasePool(bool newThread);
    ~CSAutoreleasePool();
public:
#ifdef CDK_IMPL
    void add(const CSObject* obj);
    void add(void* p);
#endif
    void drain();
};

#endif
