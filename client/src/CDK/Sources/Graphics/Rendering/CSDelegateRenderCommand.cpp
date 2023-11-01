#define CDK_IMPL

#include "CSDelegateRenderCommand.h"

CSDelegateRenderCommand::Fence::Fence(const CSGraphicsResource* resource, CSGBatchFlags flags) : resource(resource), flags(flags) {

}

CSDelegateRenderCommand::CSDelegateRenderCommand(const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0, const CSObject* retain1, const CSObject* retain2) :
    _inv(inv),
    _retains{ retain(retain0), retain(retain1), retain(retain2) }
{

}

CSDelegateRenderCommand::~CSDelegateRenderCommand() {
    release(_retains[0]);
    release(_retains[1]);
    release(_retains[2]);
}

void CSDelegateRenderCommand::addFence(const CSGraphicsResource* resource, CSGBatchFlags flags) {
    new (&_fences.addObject()) Fence(resource, flags);
}

bool CSDelegateRenderCommand::parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const {
    bool result = true;
    foreach (const Fence&, e, &_fences) result &= e.resource->batch(reads, writes, e.flags);
    return result;
}

void CSDelegateRenderCommand::render(CSGraphicsApi* api, bool background, bool foreground) {
    CSAssert(background && foreground);
    _inv(api);
}
