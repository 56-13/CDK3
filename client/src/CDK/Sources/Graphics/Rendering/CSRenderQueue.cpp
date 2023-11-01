#define CDK_IMPL

#include "CSRenderQueue.h"

#include "CSGraphicsContext.h"

CSRenderQueue::CSRenderQueue() : _frame(new CSRenderFrame()) {

}

CSRenderQueue::~CSRenderQueue() {
	_frame->release();
}

int CSRenderQueue::remaining() const {
    return _remaining.load(std::memory_order_acquire);
}

void CSRenderQueue::render() {
    _remaining.fetch_add(1, std::memory_order_relaxed);

    CSRenderFrame* frame = _frame;

    CSGraphicsContext::sharedContext()->invoke(false, [this, frame](CSGraphicsApi* api) {
        frame->render(api);

        _commandCapacity = frame->commandCapacity() + 8;
        _readCapacity = frame->readCapacity() + 8;
        _writeCapacity = frame->writeCapacity() + 8;
        _parallelCommandCapacity = frame->parallelCommandCapacity() + 8;
        _parallelLocalCommandCapacity = frame->parallelLocalCommandCapacity() + 2;
        _firstDone = true;

        _remaining.fetch_sub(1, std::memory_order_relaxed);
    }, this, frame);

    _frame->release();
    _frame = _firstDone ? new CSRenderFrame(_commandCapacity, _readCapacity, _writeCapacity, _parallelCommandCapacity, _parallelLocalCommandCapacity) : new CSRenderFrame();
}
