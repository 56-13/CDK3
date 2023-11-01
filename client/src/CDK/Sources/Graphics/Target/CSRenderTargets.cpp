#define CDK_IMPL

#include "CSRenderTargets.h"

#include "CSResourcePool.h"

CSRenderTarget* CSRenderTargets::get(const CSObject* key, int life, bool recycle, const CSRenderTargetDescription& desc) {
    desc.validate();

    auto match = [desc](const CSResource* candidate) -> bool {
        return candidate->resourceType() == CSResourceTypeRenderTarget && static_cast<const CSRenderTarget*>(candidate)->description() == desc;
    };

    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) {
        return static_cast<CSRenderTarget*>(resource);
    }
    CSRenderTarget* newTarget = new CSRenderTarget(desc);
    CSResourcePool::sharedPool()->add(key, newTarget, life, recycle);
    newTarget->release();
    return newTarget;
}