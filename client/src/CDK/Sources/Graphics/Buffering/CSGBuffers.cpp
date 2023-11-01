#define CDK_IMPL

#include "CSGBuffers.h"

static CSArray<CSGBufferSliceSet>* __sliceSets = NULL;

void CSGBuffers::initialize() {
	if (!__sliceSets) __sliceSets = new CSArray<CSGBufferSliceSet>();
}

void CSGBuffers::finalize() {
    CSObject::release(__sliceSets);
}

CSGBuffer* CSGBuffers::get(const CSObject* key, int life, bool recycle, CSGBufferTarget target) {
    auto match = [target](const CSResource* candidate) -> bool {
        if (candidate->resourceType() != CSResourceTypeGBuffer) return false;
        const CSGBuffer* buf = static_cast<const CSGBuffer*>(candidate);
        return buf->target() == target;
    };

    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) return static_cast<CSGBuffer*>(resource);
    CSGBuffer* newBuf = CSGBuffer::buffer(target);
    CSResourcePool::sharedPool()->add(key, newBuf, life, recycle);
    return newBuf;
}

CSGBuffer* CSGBuffers::get(const CSObject* key, int life, bool recycle, CSGBufferTarget target, int preferredSize, int preferredCount) {
    auto match = [target, preferredSize, preferredCount](const CSResource* candidate) -> bool {
        if (candidate->resourceType() != CSResourceTypeGBuffer) return false;
        const CSGBuffer* buf = static_cast<const CSGBuffer*>(candidate);
        return buf->target() == target && buf->size() == preferredSize && buf->count() == preferredCount;
    };
    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) return static_cast<CSGBuffer*>(resource);
    return get(key, life, recycle, target);
}

CSGBufferSlice* CSGBuffers::getSlice(CSGBufferTarget target, int size, int count, int capacity, CSGBufferUsageHint hint) {
    synchronized(__sliceSets) {
        foreach (CSGBufferSliceSet*, sliceSet, __sliceSets) {
            if (sliceSet->buffer()->target() == target && sliceSet->buffer()->size() == size) {
                CSGBufferSlice* slice = sliceSet->obtainSlice(count);
                if (slice) return slice;
            }
        }
        CSAssert(count <= capacity);

        CSLog("buffer slice add:%d:%d:%d", size, capacity, __sliceSets->count() + 1);

        {
            CSGBufferSliceSet* sliceSet = new CSGBufferSliceSet(target, size, capacity, hint);
            __sliceSets->addObject(sliceSet);
            sliceSet->release();
            CSGBufferSlice* slice = sliceSet->obtainSlice(count);
            return slice;
        }
    }
    return NULL;
}

void CSGBuffers::releaseSlice(CSGBufferSliceSet* sliceSet, CSGBufferSlice* slice) {
    synchronized(__sliceSets) {
        if (sliceSet->releaseSlice(slice)) {
            __sliceSets->removeObjectIdenticalTo(sliceSet);
        }
    }
}
