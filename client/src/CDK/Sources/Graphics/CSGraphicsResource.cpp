#define CDK_IMPL

#include "CSGraphicsResource.h"

#include "CSGraphicsContext.h"

bool CSGraphicsResource::batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const {
	bool conf0 = (flags & CSGBatchFlagRead) && writes->containsObject(this);
	bool conf1 = (flags & CSGBatchFlagWrite) && reads->containsObject(this);
	if (flags & CSGBatchFlagRead) reads->addObject(this);
	if (flags & CSGBatchFlagWrite) writes->addObject(this);
	return !conf0 && !conf1;
}

void CSGraphicsResource::flush() const {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [](CSGraphicsApi*) {});
	if (command) command->addFence(this, CSGBatchFlagRead | CSGBatchFlagRetrieve);
}