#ifndef __CDK__CSGBuffers__
#define __CDK__CSGBuffers__

#include "CSGBufferSlice.h"

#include "CSValue.h"

#include "CSResourcePool.h"

class CSGBuffers {
public:
#ifdef CDK_IMPL
    static void initialize();
    static void finalize();
#endif
    static CSGBuffer* get(const CSObject* key, int life, bool recycle, CSGBufferTarget target);
    static CSGBuffer* get(const CSObject* key, int life, bool recycle, CSGBufferTarget target, int preferredSize, int preferredCount);
    static inline CSGBuffer* getTemporary(CSGBufferTarget target) {
        return get(NULL, 1, true, target);
    }
    template <typename T>
    static CSGBuffer* fromData(const T& data, CSGBufferTarget target);
    template <typename T>
    static CSGBuffer* fromData(const CSGBufferData<T>* data, CSGBufferTarget target);
    static CSGBufferSlice* getSlice(CSGBufferTarget target, int size, int count, int capacity, CSGBufferUsageHint hint);
#ifdef CDK_IMPL
    static void releaseSlice(CSGBufferSliceSet* sliceSet, CSGBufferSlice* slice);
#endif
};

template <typename T>
static CSGBuffer* CSGBuffers::fromData(const T& data, CSGBufferTarget target) {
    CSValue<T>* key = new CSValue<T>(data);
    CSGBuffer* buffer = static_assert_cast<CSGBuffer*>(CSResourcePool::sharedPool()->get(key));
    if (!buffer) {
        buffer = get(key, 1, true, target, sizeof(T), 1);
        buffer->upload(&data, 1, CSGBufferUsageHintDynamicDraw);
    }
    key->release();
    return buffer;
}

template <typename T>
static CSGBuffer* CSGBuffers::fromData(const CSGBufferData<T>* data, CSGBufferTarget target) {
    CSGBuffer* buffer = static_assert_cast<CSGBuffer*>(CSResourcePool::sharedPool()->get(data));
    if (!buffer) {
        buffer = get(data, 1, true, target, data->size(), data->count());
        buffer->upload(data, CSGBufferUsageHintDynamicDraw);
    }
    return buffer;
}

#endif