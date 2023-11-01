#define CDK_IMPL

#include "CSMemory.h"

#include "CSThread.h"

#include "CSLog.h"

void* fmalloc(size_t size) {
    void* p = malloc(size);
    CSAssert(p, "malloc failed:%zu bytes", size);
    return p;
}

void* fcalloc(size_t n, size_t size) {
    void* p = calloc(n, size);
    CSAssert(p, "calloc failed:%zu*%zu bytes", n, size);
    return p;
}

void* frealloc(void* ptr, size_t size) {
    void* p = realloc(ptr, size);
    CSAssert(p, "realloc failed:%zu bytes", size);
    return p;
}

void autofree(void* p) {
    CSThread::currentThread()->autoreleasePool()->add(p);
}

