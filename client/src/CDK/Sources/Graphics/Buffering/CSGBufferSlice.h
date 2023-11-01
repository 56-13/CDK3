#ifndef __CDK__CSGBufferSlice__
#define __CDK__CSGBufferSlice__

#include "CSGBuffer.h"

class CSGBufferSlice;

class CSGBufferSliceSet : public CSObject {
private:
    CSGBuffer* _buffer;
    CSGBufferSlice* _first;
#ifdef CDK_IMPL
public:
#else
private:
#endif
    CSGBufferSliceSet(CSGBufferTarget target, int size, int capacity, CSGBufferUsageHint hint);
private:
    ~CSGBufferSliceSet();
public:
    inline CSGBuffer* buffer() {
        return _buffer;
    }
    CSGBufferSlice* obtainSlice(int count);
    bool releaseSlice(CSGBufferSlice* slice);
};

class CSGBufferSlice : public CSResource {
private:
    CSGBufferSliceSet* _parent;
    int _offset;
    int _count;
    CSGBufferSlice* _prev;
    CSGBufferSlice* _next;
#ifdef CDK_IMPL
public:
#else
private:
#endif
    CSGBufferSlice(CSGBufferSliceSet* parent, int offset, int count);
private:
    ~CSGBufferSlice();
public:
    inline CSResourceType resourceType() const override {
        return CSResourceTypeGBufferSlice;
    }
    inline int resourceCost() const override {
        return sizeof(CSGBufferSlice) + _count * _parent->buffer()->size();
    }
    inline const CSGBuffer* buffer() const {
        return _parent->buffer();
    }
    inline CSGBuffer* buffer() {
        return _parent->buffer();
    }
    inline int offset() const {
        return _offset;
    }
    inline int count() const {
        return _count;
    }

    friend class CSGBufferSliceSet;
};

#endif