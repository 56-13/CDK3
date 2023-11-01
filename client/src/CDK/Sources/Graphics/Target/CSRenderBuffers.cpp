#define CDK_IMPL

#include "CSRenderBuffers.h"

#include "CSResourcePool.h"

CSRenderBuffer* CSRenderBuffers::get(const CSObject* key, int life, bool recycle, const CSRenderBufferDescription& desc) {
    const_cast<CSRenderBufferDescription&>(desc).validate();

    auto match = [desc](const CSResource* candidate) -> bool {
        return candidate->resourceType() == CSResourceTypeRenderBuffer && static_cast<const CSRenderBuffer*>(candidate)->description() == desc;
    };

    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) {
        return static_cast<CSRenderBuffer*>(resource);
    }
    CSRenderBuffer* newBuffer = new CSRenderBuffer(desc);
    CSResourcePool::sharedPool()->add(key, newBuffer, life, recycle);
    newBuffer->release();
    return newBuffer;
}