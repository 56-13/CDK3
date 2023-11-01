#define CDK_IMPL

#include "CSGBuffer.h"

#include "CSGraphicsContext.h"

#include "CSData.h"

CSGBuffer::CSGBuffer(CSGBufferTarget target) : _target(target) {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) { _object = api->genBuffer(); }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);
}

CSGBuffer::~CSGBuffer() {
	int object = _object;

	CSGraphicsContext::sharedContext()->invoke(false, [object](CSGraphicsApi* api) { api->deleteBuffer(object); });
}

void CSGBuffer::deallocate() {
    if (_allocated) {
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
            CSAssert(api->vertexArrayBinding() == 0);

            api->bindBuffer(_target, _object);
            api->bufferData(_target, 0, NULL, CSGBufferUsageHintStreamDraw);      //TODO:check zero size safe?
            api->bindBuffer(_target, 0);
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
        _size = 0;
        _count = 0;
        _allocated = false;
    }
}

void CSGBuffer::allocate(int size, int count, CSGBufferUsageHint usage) {
    _size = size;
    _count = count;
    _allocated = true;

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, size, count, usage](CSGraphicsApi* api) {
        CSAssert(api->vertexArrayBinding() == 0);

        api->bindBuffer(_target, _object);
        if (!api->bufferData(_target, size * count, NULL, usage)) uploadFail();
        api->bindBuffer(_target, 0);
    }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);
}

void CSGBuffer::uploadFail() {
    CSErrorLog("upload fail:target:%d size:%d count:%d", _target, _size, _count);
    _allocated = false;
}

void CSGBuffer::uploadImpl(CSGraphicsApi* api, const void* p, int size, int count, CSGBufferUsageHint usage) {
    CSAssert(api->vertexArrayBinding() == 0 && _size == size && _count == count);

    api->bindBuffer(_target, _object);
    if (!api->bufferData(_target, size * count, p, usage)) uploadFail();
    api->bindBuffer(_target, 0);
}

void CSGBuffer::uploadSubImpl(CSGraphicsApi* api, const void* p, int size, int offset, int count) {
    CSAssert(_allocated && _size == size && offset + count <= _count && api->vertexArrayBinding() == 0);

    api->bindBuffer(_target, _object);
    api->bufferSubData(_target, size * offset, size * count, p);
    api->bindBuffer(_target, 0);
}

void CSGBuffer::upload(const void* p, int size, int count, CSGBufferUsageHint usage) {
    _size = size;
    _count = count;
    _allocated = true;

    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        uploadImpl(api, p, size, count, usage);
    }
    else {
        CSData* copy = CSData::dataWithBytes(p, size * count);
        if (!copy) {
            uploadFail();
            return;
        }
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, copy, size, count, usage](CSGraphicsApi* api) {
            uploadImpl(api, copy->bytes(), size, count, usage);
        }, this, copy);
        if (command) command->addFence(this, CSGBatchFlagWrite);
    }
}

bool CSGBuffer::uploadSub(const void* p, int size, int offset, int count) {
    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        uploadSubImpl(api, p, size, offset, count);
    }
    else {
        CSData* copy = CSData::dataWithBytes(p, size * count);
        if (!copy) return false;
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, copy, size, offset, count](CSGraphicsApi* api) {
            uploadSubImpl(api, copy->bytes(), size, offset, count);
        }, this, copy);
        if (command) command->addFence(this, CSGBatchFlagWrite);
    }
    return true;
}
