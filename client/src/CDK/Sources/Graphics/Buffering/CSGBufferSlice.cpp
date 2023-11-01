#define CDK_IMPL

#include "CSGBuffers.h"

CSGBufferSliceSet::CSGBufferSliceSet(CSGBufferTarget target, int size, int capacity, CSGBufferUsageHint hint) :
    _buffer(new CSGBuffer(target)),
    _first(NULL)
{
	_buffer->allocate(size, capacity, hint);
}

CSGBufferSliceSet::~CSGBufferSliceSet() {
	_buffer->release();
}

CSGBufferSlice* CSGBufferSliceSet::obtainSlice(int count) {
    if (!_first) {
        _first = new CSGBufferSlice(this, 0, count);
        return _first;
    }

    int offset = 0;
    CSGBufferSlice* current = _first;
    for (; ; )
    {
        int remaining = current->_offset - offset;
        if (remaining >= count) {
            CSGBufferSlice* slice = new CSGBufferSlice(this, offset, count);
            slice->_next = current;
            if (current->_prev)
            {
                slice->_prev = current->_prev;
                current->_prev->_next = slice;
            }
            else _first = slice;
            current->_prev = slice;
            return autorelease(slice);
        }
        offset = current->_offset + current->_count;
        if (!current->_next) break;
        current = current->_next;
    }
    if (offset + count <= _buffer->count()) {
        CSGBufferSlice* slice = new CSGBufferSlice(this, offset, count);
        slice->_prev = current;
        current->_next = slice;
        return autorelease(slice);
    }
    return NULL;
}

bool CSGBufferSliceSet::releaseSlice(CSGBufferSlice* slice) {
    if (slice->_prev) slice->_prev->_next = slice->_next;
    else _first = slice->_next;
    if (slice->_next) slice->_next->_prev = slice->_prev;
    return _first == NULL;
}

CSGBufferSlice::CSGBufferSlice(CSGBufferSliceSet* parent, int offset, int count) : 
    _parent(parent), 
    _offset(offset), 
    _count(count),
    _prev(NULL),
    _next(NULL)
{

}

CSGBufferSlice::~CSGBufferSlice() {
    CSGBuffers::releaseSlice(_parent, this);
}
