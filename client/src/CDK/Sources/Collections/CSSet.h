#ifndef __CDK__CSSet__
#define __CDK__CSSet__

#include "CSArray.h"

template <typename V, bool readonly = true>
class CSSet : public CSObject {
public:
    typedef typename CSEntryType<V, readonly>::Type Type;
    typedef typename CSEntryType<V, readonly>::ValueParamType ValueParamType;
    typedef typename CSEntryType<V, readonly>::PointerParamType PointerParamType;
    typedef typename CSEntryType<V, readonly>::ConstValueParamType ConstValueParamType;
    typedef typename CSEntryType<V, readonly>::ValueReturnType ValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstValueReturnType ConstValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstTemplateType ConstTemplateType;

    class ReadonlyIterator {
    protected:
        CSSet* _set;
        int _index;
        int* _cursor;
        bool _removed;
        
        ReadonlyIterator(CSSet* set);
    public:
        virtual ~ReadonlyIterator();
        
        bool remaining() const;
        ConstValueReturnType object() const;
        void next();
        friend class CSSet;
    };
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSSet* set);
    public:
        ValueReturnType object();
        void remove();
        friend class CSSet;
    };
    
    friend class ReadonlyIterator;
    friend class Iterator;
private:
    struct Bucket {
        Type object;
        int next;
        bool exists;
    };
    
    Bucket* _buckets = NULL;
    int* _bucketIndices = NULL;
    int _count = 0;
    int _expansion = 0;
    int _capacity = 0;
#ifdef CS_ASSERT_DEBUG
    mutable std::atomic<int> _fence;
#endif
public:
    CSSet();
    explicit CSSet(int capacity);
    CSSet(PointerParamType objects, int count);
    CSSet(int count, Type ob, ...);
    template <typename otherV, bool otherReadonly>
    CSSet(const CSSet<otherV, otherReadonly>* otherSet);
    virtual ~CSSet();

    static inline CSSet* set() {
        return autorelease(new CSSet());
    }
    static inline CSSet* setWithCapacity(int capacity) {
        return autorelease(new CSSet(capacity));
    }
    static inline CSSet* setWithObjects(PointerParamType objects, int count) {
        return autorelease(new CSSet(objects, count));
    }
    static CSSet* setWithObjects(int count, Type ob, ...);

    template <typename otherV, bool otherReadonly>
    static inline CSSet* setWithSet(const CSSet<otherV, otherReadonly>* otherSet) {
        return autorelease(new CSSet(otherSet));
    }
private:
    int expand();
public:
    inline int count() const {
        return _count;
    }

    void setCapacity(int capacity);

    inline int capacity() const {
        return _capacity;
    }

    const CSArray<V>* allObjects() const;
    CSArray<ConstTemplateType, 1, false>* allObjectsMutable() const;
    bool containsObject(ConstValueParamType object) const;
    void addObject(ValueParamType object);
    bool removeObject(ConstValueParamType object);
    void removeAllObjects();
    
    inline ReadonlyIterator iterator() const {
        return ReadonlyIterator(const_cast<CSSet*>(this));
    }
    inline Iterator iterator() {
        return Iterator(this);
    }
    
    inline CSSet<V, false>* asReadWrite() {
        return reinterpret_cast<CSSet<V, false>*>(this);
    }
    inline CSSet<V, true>* asReadOnly() {
        return reinterpret_cast<CSSet<V, true>*>(this);
    }

    inline void fence(bool shouldFirst = true) const {
#ifdef CS_ASSERT_DEBUG
        if (shouldFirst) CSAssert(_fence.fetch_add(1, std::memory_order_acq_rel) == 0);
        else _fence.fetch_add(1, std::memory_order_relaxed);
#endif
    }
    inline void flict() const {
        CSAssert(_fence.fetch_sub(1, std::memory_order_acq_rel) >= 1);
    }

    template <typename friendV, bool friendReadonly>
    friend class CSSet;
};

template <typename V, bool readonly>
CSSet<V, readonly>::CSSet() {
    setCapacity(13);
}

template <typename V, bool readonly>
CSSet<V, readonly>::CSSet(int capacity) {
    setCapacity(capacity);
}

template <typename V, bool readonly>
CSSet<V, readonly>::CSSet(PointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) addObject(objects[i]);
}

template <typename V, bool readonly>
CSSet<V, readonly>::CSSet(int count, Type obj, ...) {
    setCapacity(count);

    addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) addObject(va_arg(ap, Type));
    va_end(ap);
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
CSSet<V, readonly>::CSSet(const CSSet<otherV, otherReadonly>* otherSet) {
    setCapacity(otherSet->_capacity);
    
    _count = otherSet->_count;
    
    for (int i = 0; i < _capacity; i++) {
        int srcIndex = otherSet->_bucketIndices[i];
        int* destIndex = &_bucketIndices[i];
        while (srcIndex != -1) {
            typename CSSet<otherV, otherReadonly>::Bucket& src = otherSet->_buckets[srcIndex];
            
            *destIndex = _expansion++;
            
            Bucket& dest = _buckets[*destIndex];
            new (&dest.object) Type(CSEntryImpl<V>::retain(src.object));
            dest.next = -1;
            dest.exists = true;
            
            destIndex = &dest.next;
            srcIndex = src.next;
        }
    }
}

template <typename V, bool readonly>
CSSet<V, readonly>::~CSSet() {
    fence_block(_fence);

    if (_capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (_buckets[i].exists) CSEntryImpl<V>::release(_buckets[i].object);
        }
        free(_buckets);
        free(_bucketIndices);
    }
}

template <typename V, bool readonly>
CSSet<V, readonly>* CSSet<V, readonly>::setWithObjects(int count, Type obj, ...) {
    CSSet<V, readonly>* set = CSSet<V, readonly>::setWithCapacity(count);

    set->addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) set->addObject(va_arg(ap, Type));
    va_end(ap);

    return arr;
}

template <typename V, bool readonly>
int CSSet<V, readonly>::expand() {
    if (_expansion == _capacity) {
        if (_count < _expansion) {
            for (int i = 0; i < _expansion; i++) {
                if (!_buckets[i].exists) return i;
            }
        }
        int nextCapacity = CSMath::max(_capacity << 1, 1);
        setCapacity(nextCapacity);
    }
    return _expansion++;
}

template <typename V, bool readonly>
void CSSet<V, readonly>::setCapacity(int capacity) {
    CSAssert(capacity >= 0);
    if (_expansion <= capacity) {
        if (capacity) {
            capacity = CSHash::primeNumberCapacity(capacity);
            if (capacity != _capacity) {
                _buckets = (Bucket*)frealloc(_buckets, capacity * sizeof(Bucket));
                if (capacity > _capacity) memset(&_buckets[_capacity], 0, (capacity - _capacity) * sizeof(Bucket));
                int* newBucketIndices = (int*)fmalloc(capacity * sizeof(int));
                memset(newBucketIndices, -1, capacity * sizeof(int));

                if (_capacity) {
                    for (int i = 0; i < _expansion; i++) {
                        _buckets[i].next = -1;
                    }
                    for (int i = 0; i < _expansion; i++) {
                        int index = CSEntryImpl<V>::hash(_buckets[i].object) % capacity;
                        int* current = &newBucketIndices[index];
                        while (*current != -1) current = &_buckets[*current].next;
                        *current = i;
                    }
                    free(_bucketIndices);
                }
                _bucketIndices = newBucketIndices;
                _capacity = capacity;
            }
        }
        else if (capacity != _capacity) {
            free(_buckets);
            free(_bucketIndices);
            _buckets = NULL;
            _bucketIndices = NULL;
            _capacity = capacity;
        }
    }
}

template <typename V, bool readonly>
const CSArray<V>* CSSet<V, readonly>::allObjects() const {
    if (_count == 0) return NULL;
    
    CSArray<V>* objects = CSArray<V>::arrayWithCapacity(_count);
    
    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) {
            Bucket& src = _buckets[i];
            
            objects->addObject(src.object);
        }
    }
    return objects;
}

template <typename V, bool readonly>
CSArray<typename CSSet<V, readonly>::ConstTemplateType, 1, false>* CSSet<V, readonly>::allObjectsMutable() const {
    if (_count == 0) return NULL;

    CSArray<ConstTemplateType, 1, false>* objects = CSArray<ConstTemplateType, 1, false>::arrayWithCapacity(_count);

    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) {
            Bucket& src = _buckets[i];
            objects->addObject(src.object);
        }
    }
    return objects;
}


template <typename V, bool readonly>
bool CSSet<V, readonly>::containsObject(ConstValueParamType object) const {
    if (_count) {
        int index = CSEntryImpl<V>::hash(object) % _capacity;
        
        int current = _bucketIndices[index];
        
        while (current != -1) {
            Bucket& bucket = _buckets[current];
            if (CSEntryImpl<V>::isEqual(object, bucket.object)) return true;
            current = bucket.next;
        }
    }
    return false;
}

template <typename V, bool readonly>
void CSSet<V, readonly>::addObject(ValueParamType object) {
    fence_block(_fence);
    
    uint hash = CSEntryImpl<V>::hash(object);
    
    int current = _bucketIndices[hash % _capacity];
    
    while (current != -1) {
        Bucket& bucket = _buckets[current];
        if (CSEntryImpl<V>::isEqual(object, bucket.object)) return;
        current = bucket.next;
    }
    
    current = expand();
    
    _count++;
    
    {
        int index = hash % _capacity;
        Bucket& bucket = _buckets[current];
        new (&bucket.object) Type(CSEntryImpl<V>::retain(object));
        bucket.next = _bucketIndices[index];
        bucket.exists = true;
        _bucketIndices[index] = current;
    }
}

template <typename V, bool readonly>
bool CSSet<V, readonly>::removeObject(ConstValueParamType object) {
    fence_block(_fence);
    
    int index = CSEntryImpl<V>::hash(object) % _capacity;
    
    int* current = &_bucketIndices[index];
    
    while (*current != -1) {
        Bucket& bucket = _buckets[*current];
        if (CSEntryImpl<V>::isEqual(object, bucket.object)) {
            CSEntryImpl<V>::release(bucket.object);
            bucket.exists = false;
            *current = bucket.next;
            _count--;
            return true;
        }
        current = &bucket.next;
    }
    return false;
}

template <typename V, bool readonly>
void CSSet<V, readonly>::removeAllObjects() {
    fence_block(_fence);
    
    for (int i = 0; i < _expansion; i++) {
        Bucket& bucket = _buckets[i];
        if (bucket.exists) {
            CSEntryImpl<V>::release(bucket.object);
            bucket.exists = false;
        }
    }
    memset(_bucketIndices, -1, _capacity * sizeof(int));
    _expansion = 0;
    _count = 0;
}

template <typename V, bool readonly>
CSSet<V, readonly>::ReadonlyIterator::ReadonlyIterator(CSSet<V, readonly>* set) : _set(set), _index(0), _cursor(NULL), _removed(false) {
    _set->fence();

    if (_set->_count) {
        while (_index < _set->_capacity) {
            if (_set->_bucketIndices[_index] != -1) {
                _cursor = &_set->_bucketIndices[_index];
                break;
            }
            _index++;
        }
    }
}

template <typename V, bool readonly>
CSSet<V, readonly>::ReadonlyIterator::~ReadonlyIterator() {
    _set->flict();
}

template <typename V, bool readonly>
CSSet<V, readonly>::Iterator::Iterator(CSSet* set) : ReadonlyIterator(set) {

}

template <typename V, bool readonly>
bool CSSet<V, readonly>::ReadonlyIterator::remaining() const {
    return _cursor != NULL;
}

template <typename V, bool readonly>
typename CSSet<V, readonly>::ConstValueReturnType CSSet<V, readonly>::ReadonlyIterator::object() const {
    return _cursor ? _set->_buckets[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
void CSSet<V, readonly>::ReadonlyIterator::next() {
    if (_cursor) {
        if (_removed) _removed = false;
        if (_set->_buckets[*_cursor].next != -1) _cursor = &_set->_buckets[*_cursor].next;
        else {
            while (_index < _set->_capacity - 1) {
                _index++;
                _cursor = &_set->_bucketIndices[_index];
                if (*_cursor != -1) return;
            }
            _cursor = NULL;
        }
    }
}

template <typename V, bool readonly>
typename CSSet<V, readonly>::ValueReturnType CSSet<V, readonly>::Iterator::object() {
    return _cursor ? _set->_buckets[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
void CSSet<V, readonly>::Iterator::remove() {
    CSAssert(_cursor, "invalid cursor");
    
    CSEntryImpl<V>::release(_set->_buckets[*_cursor].object);
    _set->_buckets[*_cursor].exists = false;
    _set->_count--;
    _removed = true;
    *_cursor = _set->_buckets[*_cursor].next;
    
    if (*_cursor == -1) {
        while (_index < _set->_capacity - 1) {
            _index++;
            _cursor = &_set->_bucketIndices[_index];
            if (*_cursor != -1) {
                return;
            }
        }
        _cursor = NULL;
    }
}

#endif
