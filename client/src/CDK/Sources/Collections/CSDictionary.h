#ifndef __CDK__CSDictionary__
#define __CDK__CSDictionary__

#include "CSArray.h"

template <typename K, typename V, bool readonly = true>
class CSDictionary : public CSObject {
public:
    typedef typename CSEntryType<K, true>::ConstType KeyType;
    typedef typename CSEntryType<K, true>::ConstPointerParamType KeyPointerParamType;
    typedef typename CSEntryType<K, true>::ConstValueParamType KeyValueParamType;
    typedef typename CSEntryType<K, true>::ConstValueReturnType KeyValueReturnType;
    typedef typename CSEntryType<K, true>::ConstTemplateType KeyConstTemplateType;
    
    typedef typename CSEntryType<V, readonly>::Type ObjectType;
    typedef typename CSEntryType<V, readonly>::PointerParamType ObjectPointerParamType;
    typedef typename CSEntryType<V, readonly>::ValueParamType ObjectValueParamType;
    typedef typename CSEntryType<V, readonly>::ValueReturnType ObjectValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstValueReturnType ObjectConstValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstTemplateType ObjectConstTemplateType;
    typedef typename CSEntryType<V, readonly>::ParamReturnType ObjectParamReturnType;
    typedef typename CSEntryType<V, readonly>::ConstParamReturnType ObjectConstParamReturnType;

    class ReadonlyIterator {
    protected:
        CSDictionary* _dictionary;
        int _index;
        int* _cursor;
        bool _removed;
        
        ReadonlyIterator(CSDictionary* dictionary);
    public:
        virtual ~ReadonlyIterator();
        
        bool remaining() const;
        KeyValueReturnType key() const;
        ObjectConstValueReturnType object() const;
        void next();
        friend class CSDictionary;
    };
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSDictionary* dictionary);
    public:
        void remove();
        ObjectValueReturnType object();
        friend class CSDictionary;
    };
    
    friend class ReadonlyIterator;
    friend class Iterator;
private:
    struct Bucket {
        KeyType key;
        ObjectType object;
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
    CSDictionary();
    explicit CSDictionary(int capacity);
    CSDictionary(KeyPointerParamType keys, ObjectPointerParamType objects, int count);
    template <typename otherK, typename otherV, bool otherReadonly>
    CSDictionary(const CSDictionary<otherK, otherV, otherReadonly>* otherDictionary);
    virtual ~CSDictionary();

    static inline CSDictionary* dictionary() {
        return autorelease(new CSDictionary());
    }
    static inline CSDictionary* dictionaryWithCapacity(int capacity) {
        return autorelease(new CSDictionary(capacity));
    }
    static inline CSDictionary* dictionaryWithObject(KeyValueParamType key, ObjectValueParamType object) {
        return autorelease(new CSDictionary(key, object));
    }
    static CSDictionary* dictionaryWithObjects(KeyPointerParamType keys, ObjectPointerParamType objects, int count) {
        return autorelease(new CSDictionary(keys, objects, count));
    }
    template <typename otherK, typename otherV, bool otherReadonly>
    static inline CSDictionary* dictionaryWithDictionary(const CSDictionary<otherK, otherV, otherReadonly>* otherDictionary, bool deepCopy = false) {
        return autorelease(new CSDictionary(otherDictionary, deepCopy));
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

    const CSArray<K, 1, readonly>* allKeys() const;
    CSArray<KeyConstTemplateType, 1, false>* allKeysMutable() const;
    CSArray<V, 1, readonly>* allObjects();
    inline const CSArray<V, 1, readonly>* allObjects() const {
        return const_cast<CSDictionary*>(this)->allObjects();
    }
    CSArray<ObjectConstTemplateType, 1, false>* allObjectsMutable() const;

    bool containsKey(KeyValueParamType key) const;
    ObjectValueReturnType objectForKey(KeyValueParamType key);
    inline ObjectConstValueReturnType objectForKey(KeyValueParamType key) const {
        return const_cast<CSDictionary*>(this)->objectForKey(key);
    }
    ObjectType* tryGetObjectForKey(KeyValueParamType key);
    inline const ObjectType* tryGetObjectForKey(KeyValueParamType key) const {
        return const_cast<CSDictionary*>(this)->tryGetObjectForKey(key);
    }
    bool tryGetObjectForKey(KeyValueParamType key, ObjectParamReturnType result);
    inline bool tryGetObjectForKey(KeyValueParamType key, ObjectConstParamReturnType result) const {
        return const_cast<CSDictionary*>(this)->tryGetObjectForKey(key, const_cast<ObjectParamReturnType>(result));
    }
    ObjectValueReturnType setObject(KeyValueParamType key);
    void setObject(KeyValueParamType key, ObjectValueParamType object);
    bool removeObject(KeyValueParamType key);
    void removeAllObjects();
    
    inline Iterator iterator() {
        return Iterator(this);
    }
    inline ReadonlyIterator iterator() const {
        return ReadonlyIterator(const_cast<CSDictionary*>(this));
    }

    inline CSDictionary<K, V, false>* asReadWrite() {
        return reinterpret_cast<CSDictionary<K, V, false>*>(this);
    }
    inline CSDictionary<K, V, true>* asReadOnly() {
        return reinterpret_cast<CSDictionary<K, V, true>*>(this);
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

    template <typename friendK, class friendV, bool friendReadonly>
    friend class CSDictionary;
};

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::CSDictionary() {
    setCapacity(13);
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::CSDictionary(int capacity) {
    setCapacity(capacity);
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::CSDictionary(KeyPointerParamType keys, ObjectPointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) setObject(keys[i], objects[i]);
}

template <typename K, typename V, bool readonly> template <typename otherK, class otherV, bool otherReadonly>
CSDictionary<K, V, readonly>::CSDictionary(const CSDictionary<otherK, otherV, otherReadonly>* otherDictionary) {
    setCapacity(otherDictionary->_capacity);
    
    _count = otherDictionary->_count;
    
    for (int i = 0; i < otherDictionary->_expansion; i++) {
        int srcIndex = otherDictionary->_bucketIndices[i];
        int* destIndex = &_bucketIndices[i];
        while (srcIndex != -1) {
            typename CSDictionary<otherK, otherV, otherReadonly>::Bucket& src = otherDictionary->_buckets[srcIndex];
            
            *destIndex = _expansion++;
            
            Bucket& dest = _buckets[*destIndex];
            new (&dest.key) KeyType(CSEntryImpl<otherK>::retain(src.key));
            new (&dest.object) ObjectType(CSEntryImpl<otherV>::retain(src.object));
            dest.next = -1;
            dest.exists = true;
            
            destIndex = &dest.next;
            srcIndex = src.next;
        }
    }
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::~CSDictionary() {
    fence_block(_fence);
    
    if (_capacity) {
        for (int i = 0; i < _expansion; i++) {
            Bucket& bucket = _buckets[i];
            if (bucket.exists) {
                CSEntryImpl<K>::release(bucket.key);
                CSEntryImpl<V>::release(bucket.object);
            }
        }
        free(_buckets);
        free(_bucketIndices);
    }
}

template <typename K, typename V, bool readonly>
int CSDictionary<K, V, readonly>::expand() {
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

template <typename K, typename V, bool readonly>
void CSDictionary<K, V, readonly>::setCapacity(int capacity) {
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
                        int index = CSEntryImpl<K>::hash(_buckets[i].key) % capacity;
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

template <typename K, typename V, bool readonly>
const CSArray<K, 1, readonly>* CSDictionary<K, V, readonly>::allKeys() const {
    if (_count == 0) return NULL;
    CSArray<K>* keys = CSArray<K>::arrayWithCapacity(_count);
    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) keys->addObject(_buckets[i].key);
    }
    return keys;
}

template <typename K, typename V, bool readonly>
CSArray<typename CSDictionary<K, V, readonly>::KeyConstTemplateType, 1, false>* CSDictionary<K, V, readonly>::allKeysMutable() const {
    if (_count == 0) return NULL;
    CSArray<KeyConstTemplateType, 1, false>* keys = CSArray<KeyConstTemplateType, 1, false>::arrayWithCapacity(_count);
    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) keys->addObject(_buckets[i].key);
    }
    return keys;
}

template <typename K, typename V, bool readonly>
CSArray<V, 1, readonly>* CSDictionary<K, V, readonly>::allObjects() {
    if (_count == 0) return NULL;
    CSArray<V, 1, readonly>* objects = CSArray<V, 1, readonly>::arrayWithCapacity(_count);
    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) objects->addObject(_buckets[i].object);
    }
    return objects;
}

template <typename K, typename V, bool readonly>
CSArray<typename CSDictionary<K, V, readonly>::ObjectConstTemplateType, 1, false>* CSDictionary<K, V, readonly>::allObjectsMutable() const {
    if (_count == 0) return NULL;
    CSArray<ObjectConstTemplateType, 1, false>* objects = CSArray<ObjectConstTemplateType, 1, false>::arrayWithCapacity(_count);
    for (int i = 0; i < _expansion; i++) {
        if (_buckets[i].exists) objects->addObject(_buckets[i].object);
    }
    return objects;
}

template <typename K, typename V, bool readonly>
bool CSDictionary<K, V, readonly>::containsKey(KeyValueParamType key) const {
    if (_count) {
        int index = CSEntryImpl<K>::hash(key) % _capacity;
        int current = _bucketIndices[index];
        while (current != -1) {
            Bucket& bucket = _buckets[current];
            if (CSEntryImpl<K>::isEqual(key, bucket.key)) return true;
            current = bucket.next;
        }
    }
    return false;
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::ObjectValueReturnType CSDictionary<K, V, readonly>::objectForKey(KeyValueParamType key) {
    if (_count) {
        int index = CSEntryImpl<K>::hash(key) % _capacity;
        int current = _bucketIndices[index];
        while (current != -1) {
            Bucket& bucket = _buckets[current];
            if (CSEntryImpl<K>::isEqual(key, bucket.key)) return bucket.object;
            current = bucket.next;
        }
    }
    return CSEntryImpl<V>::nullValue();
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::ObjectType* CSDictionary<K, V, readonly>::tryGetObjectForKey(KeyValueParamType key) {
    if (_count) {
        int index = CSEntryImpl<K>::hash(key) % _capacity;
        int current = _bucketIndices[index];
        while (current != -1) {
            Bucket& bucket = _buckets[current];
            if (CSEntryImpl<K>::isEqual(key, bucket.key)) return &bucket.object;
            current = bucket.next;
        }
    }
    return NULL;
}

template <typename K, typename V, bool readonly>
bool CSDictionary<K, V, readonly>::tryGetObjectForKey(KeyValueParamType key, ObjectParamReturnType result) {
    if (_count) {
        int index = CSEntryImpl<K>::hash(key) % _capacity;
        int current = _bucketIndices[index];
        while (current != -1) {
            Bucket& bucket = _buckets[current];
            if (CSEntryImpl<K>::isEqual(key, bucket.key)) {
                result = bucket.object;
                return true;
            }
            current = bucket.next;
        }
    }
    return false;
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::ObjectValueReturnType CSDictionary<K, V, readonly>::setObject(KeyValueParamType key) {
    fence_block(_fence);
    
    uint hash = CSEntryImpl<K>::hash(key);
    int current = _bucketIndices[hash % _capacity];
    while (current != -1) {
        Bucket& bucket = _buckets[current];
        if (CSEntryImpl<K>::isEqual(key, bucket.key)) return CSEntryImpl<V>::notNullValue(bucket.object);
        current = bucket.next;
    }

    current = expand();
    
    _count++;
    
    {
        int index = hash % _capacity;
        Bucket& bucket = _buckets[current];
        new (&bucket.key) KeyType(CSEntryImpl<K>::retain(key));
        bucket.next = _bucketIndices[index];
        bucket.exists = true;
        _bucketIndices[index] = current;
        return CSEntryImpl<V>::notNullValue(bucket.object);
    }
}

template <typename K, typename V, bool readonly>
void CSDictionary<K, V, readonly>::setObject(KeyValueParamType key, ObjectValueParamType object) {
    fence_block(_fence);
    
    uint hash = CSEntryImpl<K>::hash(key);
    int current = _bucketIndices[hash % _capacity];
    while (current != -1) {
        Bucket& bucket = _buckets[current];
        if (CSEntryImpl<K>::isEqual(key, bucket.key)) {
            CSEntryImpl<V>::release(bucket.object);
            new (&bucket.object) ObjectType(CSEntryImpl<V>::retain(object));
            return;
        }
        current = bucket.next;
    }
    
    current = expand();
    
    _count++;
    
    {
        int index = hash % _capacity;
        Bucket& bucket = _buckets[current];
        new (&bucket.key) KeyType(CSEntryImpl<K>::retain(key));
        new (&bucket.object) ObjectType(CSEntryImpl<V>::retain(object));
        bucket.next = _bucketIndices[index];
        bucket.exists = true;
        _bucketIndices[index] = current;
    }
}

template <typename K, typename V, bool readonly>
bool CSDictionary<K, V, readonly>::removeObject(KeyValueParamType key) {
    fence_block(_fence);
    
    int index = CSEntryImpl<K>::hash(key) % _capacity;
    int* current = &_bucketIndices[index];
    while (*current != -1) {
        Bucket& bucket = _buckets[*current];
        if (CSEntryImpl<K>::isEqual(key, bucket.key)) {
            *current = bucket.next;
            CSEntryImpl<K>::release(bucket.key);
            CSEntryImpl<V>::release(bucket.object);
            bucket.exists = false;
            _count--;
            return true;
        }
        current = &bucket.next;
    }
    return false;
}

template <typename K, typename V, bool readonly>
void CSDictionary<K, V, readonly>::removeAllObjects() {
    fence_block(_fence);
    
    for (int i = 0; i < _expansion; i++) {
        Bucket& bucket = _buckets[i];
        if (bucket.exists) {
            CSEntryImpl<K>::release(bucket.key);
            CSEntryImpl<V>::release(bucket.object);
            bucket.exists = false;
        }
    }
    memset(_bucketIndices, -1, _capacity * sizeof(int));
    _expansion = 0;
    _count = 0;
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::ReadonlyIterator::ReadonlyIterator(CSDictionary* dictionary) : _dictionary(dictionary), _index(0), _cursor(NULL), _removed(false) {
    _dictionary->fence();

    if (_dictionary->_count) {
        while (_index < _dictionary->_capacity) {
            if (_dictionary->_bucketIndices[_index] != -1) {
                _cursor = &_dictionary->_bucketIndices[_index];
                break;
            }
            _index++;
        }
    }
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::ReadonlyIterator::~ReadonlyIterator() {
    _dictionary->flict();
}

template <typename K, typename V, bool readonly>
CSDictionary<K, V, readonly>::Iterator::Iterator(CSDictionary* dictionary) : ReadonlyIterator(dictionary) {
}

template <typename K, typename V, bool readonly>
bool CSDictionary<K, V, readonly>::ReadonlyIterator::remaining() const {
    return _cursor != NULL;
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::KeyValueReturnType CSDictionary<K, V, readonly>::ReadonlyIterator::key() const {
    return _cursor ? _dictionary->_buckets[*_cursor].key : CSEntryImpl<K>::nullValue();
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::ObjectConstValueReturnType CSDictionary<K, V, readonly>::ReadonlyIterator::object() const {
    return _cursor ? _dictionary->_buckets[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename K, typename V, bool readonly>
void CSDictionary<K, V, readonly>::ReadonlyIterator::next() {
    if (_cursor) {
        if (_removed) _removed = false;
        else if (_dictionary->_buckets[*_cursor].next != -1) {
            _cursor = &_dictionary->_buckets[*_cursor].next;
        }
        else {
            while (_index < _dictionary->_capacity - 1) {
                _index++;
                _cursor = &_dictionary->_bucketIndices[_index];
                if (*_cursor != -1) return;
            }
            _cursor = NULL;
        }
    }
}

template <typename K, typename V, bool readonly>
void CSDictionary<K, V, readonly>::Iterator::remove() {
    CSAssert(_cursor, "invalid cursor");
    
    CSEntryImpl<K>::release(_dictionary->_buckets[*_cursor].key);
    CSEntryImpl<V>::release(_dictionary->_buckets[*_cursor].object);
    _dictionary->_buckets[*_cursor].exists = false;
    _dictionary->_count--;
    _removed = true;
    *_cursor = _dictionary->_buckets[*_cursor].next;
    
    if (*_cursor == -1) {
        while (_index < _dictionary->_capacity - 1) {
            _index++;
            _cursor = &_dictionary->_bucketIndices[_index];
            if (*_cursor != -1) {
                return;
            }
        }
        _cursor = NULL;
    }
}

template <typename K, typename V, bool readonly>
typename CSDictionary<K, V, readonly>::ObjectValueReturnType CSDictionary<K, V, readonly>::Iterator::object() {
    return _cursor ? _dictionary->_buckets[*_cursor].object : CSEntryImpl<V>::nullValue();
}

#endif
