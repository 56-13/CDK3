#ifndef __CDK__CSGBuffer__
#define __CDK__CSGBuffer__

#include "CSGraphicsResource.h"

#include "CSGraphicsContext.h"

#include "CSGBufferData.h"

class CSGBuffer : public CSGraphicsResource {
private:
    CSGBufferTarget _target;
    int _object = 0;
    int _size = 0;
    int _count = 0;
    bool _allocated = false;
public:
    CSGBuffer(CSGBufferTarget target);
private:
    ~CSGBuffer();
public:
    static inline CSGBuffer* buffer(CSGBufferTarget target) {
        return autorelease(new CSGBuffer(target));
    }

    inline CSResourceType resourceType() const override {
        return CSResourceTypeGBuffer;
    }
    inline int resourceCost() const override {
        return sizeof(CSGBuffer) + _size * _count;
    }
    inline CSGBufferTarget target() const {
        return _target;
    }
    inline int object() const {
        return _object;
    }
    inline int size() const {
        return _size;
    }
    inline int count() const {
        return _count;
    }
    inline bool allocated() const {
        return _allocated;
    }

    void deallocate();
    void allocate(int size, int count, CSGBufferUsageHint usage);
private:
    void uploadFail();
    void uploadImpl(CSGraphicsApi* api, const void* p, int size, int offset, CSGBufferUsageHint usage);
    void uploadSubImpl(CSGraphicsApi* api, const void* p, int size, int offset, int count);
    template<typename T>
    void uploadData(const CSGBufferData<T>* data, CSGBufferUsageHint usage);
    template<typename T>
    void uploadSubData(const CSGBufferData<T>* data, int offset);
public:
    void upload(const void* p, int size, int count, CSGBufferUsageHint usage);

    template<typename T>
    inline void upload(const T* values, int count, CSGBufferUsageHint usage) {
        upload(values, sizeof(T), count, usage);
    }
    template<typename T>
    inline void upload(const CSGBufferData<T>* data, CSGBufferUsageHint usage) {
        CSAssert(_target != CSGBufferTargetElementArray);
        uploadData(data, usage);
    }

    inline void upload(const CSVertexIndexData* data, CSGBufferUsageHint usage) {
        CSAssert(_target == CSGBufferTargetElementArray);
        uploadData(data, usage);
    }

    bool uploadSub(const void* p, int size, int offset, int count);

    template<typename T>
    inline bool uploadSub(const T* values, int offset, int count) {
        return uploadSub(values, sizeof(T), offset, count);
    }
    template<typename T>
    inline void uploadSub(const CSGBufferData<T>* data, int offset) {
        CSAssert(_target != CSGBufferTargetElementArray);
        uploadSubData(data, offset);
    }
    inline void uploadSub(const CSVertexIndexData* data, int offset) {
        CSAssert(_target == CSGBufferTargetElementArray);
        uploadSubData(data, offset);
    }
};

template<typename T>
void CSGBuffer::uploadData(const CSGBufferData<T>* data, CSGBufferUsageHint usage) {
    _size = data->size();
    _count = data->count();
    _allocated = true;

    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        uploadImpl(api, data->pointer(), _size, _count, usage);
    }
    else {
        data->fence(false);         //TODO:CHECK
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, data, usage](CSGraphicsApi* api) {
            uploadImpl(api, data->pointer(), data->size(), data->count(), usage);
            data->flict();
        }, this, data);
        if (command) command->addFence(this, CSGBatchFlagWrite);
    }
}
template<typename T>
void CSGBuffer::uploadSubData(const CSGBufferData<T>* data, int offset) {
    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        uploadSubImpl(api, data->pointer(), data->size(), offset, data->count());
    }
    else {
        data->fence(false);         //TODO:CHECK
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, data, offset](CSGraphicsApi* api) {
            uploadSubImpl(api, data->pointer(), data->size(), offset, data->count());
            data->flict();
        }, this, data);
        if (command) command->addFence(this, CSGBatchFlagWrite);
    }
}

#endif
