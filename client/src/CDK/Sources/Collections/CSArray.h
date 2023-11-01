#ifndef __CDK__CSArray__
#define __CDK__CSArray__

#include "CSEntry.h"

#include "CSMath.h"

template <typename V, int dimension, bool readonly>
class CSArray;

template <typename V, int dimension = 1, bool readonly = true>
class CSArray : public CSArray<CSArray<V, dimension - 1, readonly>, 1, readonly> {
public:
    typedef CSArray<V, dimension - 1, readonly> IV;
    
    CSArray() = default;
    inline CSArray(int capacity) : CSArray<IV, 1, readonly>(capacity) {}
    inline CSArray(IV* const * objects, int count) : CSArray<IV, 1, readonly>(objects, count) {}
    
    template<typename otherV, bool otherReadonly>
    inline CSArray(const CSArray<otherV, dimension, otherReadonly>* otherArray) :
        CSArray<IV, 1, readonly>(static_cast<const CSArray<CSArray<otherV, dimension - 1, otherReadonly>, 1, otherReadonly>&>(otherArray))
    {
    }
    
    CSArray(CSBuffer* buffer, bool nullIfEmpty = true);
    CSArray(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty = true);

    static inline CSArray* array() {
        return autorelease(new CSArray());
    }
    static inline CSArray* arrayWithCapacity(int capacity) {
        return autorelease(new CSArray(capacity));
    }
    static inline CSArray* arrayWithObjects(IV* const* objects, int count) {
        return autorelease(new CSArray(objects, count));
    }
    template<typename otherV, bool otherReadonly>
    static inline CSArray* arrayWithArray(const CSArray<otherV, dimension, otherReadonly>* otherArray) {
        return autorelease(new CSArray(otherArray));
    }
};

template <typename V, bool readonly>
class CSArray<V, 1, readonly> : public CSObject {
public:
    typedef typename CSEntryType<V, readonly>::Type Type;
    typedef typename CSEntryType<V, readonly>::ValueParamType ValueParamType;
    typedef typename CSEntryType<V, readonly>::ConstValueParamType ConstValueParamType;
    typedef typename CSEntryType<V, readonly>::PointerParamType PointerParamType;
    typedef typename CSEntryType<V, readonly>::ValueReturnType ValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstValueReturnType ConstValueReturnType;
    typedef typename CSEntryType<V, readonly>::PointerReturnType PointerReturnType;
    typedef typename CSEntryType<V, readonly>::ConstPointerReturnType ConstPointerReturnType;
    typedef typename CSEntryType<V, readonly>::ConstTemplateType ConstTemplateType;
    
    class ReadonlyIterator {
    protected:
        CSArray* _array;
        int _index;
        
        ReadonlyIterator(CSArray* array);
    public:
        virtual ~ReadonlyIterator();
        
        bool remaining() const;
        ConstValueReturnType object() const;
        void next();
        friend class CSArray;
    };
    
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSArray* array);
    public:
        void remove();
        void insert(ValueParamType obj);
        ValueReturnType insert();
        void set(ValueParamType obj);
        ValueReturnType object();
        friend class CSArray;
    };
    
    friend class ReadonlyIterator;
    friend class Iterator;
private:
    Type* _objects = NULL;
    int _count = 0;
    int _capacity = 0;
#ifdef CS_ASSERT_DEBUG
    mutable std::atomic<int> _fence;
#endif
public:
    CSArray();
    explicit CSArray(int capacity);
    CSArray(PointerParamType objects, int count);
    CSArray(int count, Type obj, ...);
    template<typename otherV, bool otherReadonly>
    CSArray(const CSArray<otherV, 1, otherReadonly>* otherArray);
    virtual ~CSArray();

    explicit CSArray(CSBuffer* buffer);
    CSArray(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func);

    static inline CSArray* array() {
        return autorelease(new CSArray());
    }
    static inline CSArray* arrayWithCapacity(int capacity) {
        return autorelease(new CSArray(capacity));
    }
    static inline CSArray* arrayWithObjects(PointerParamType objects, int count) {
        return autorelease(new CSArray(objects, count));
    }
    static CSArray<V, 1, readonly>* arrayWithObjects(int count, Type obj, ...);
    
    template<typename otherV, bool otherReadonly>
    static inline CSArray* arrayWithArray(const CSArray<otherV, 1, otherReadonly>* otherArray) {
        return autorelease(new CSArray(otherArray));
    }
private:
    void expand(int count);
public:
    inline ConstPointerReturnType pointer() const {
        return _objects;
    }
    inline PointerReturnType pointer() {
        return _objects;
    }
    inline int count() const {
        return _count;
    }

    void setCapacity(int capacity);

    inline int capacity() const {
        return _capacity;
    }
    
    int indexOfObject(ConstValueParamType object) const;
    int indexOfObjectIdenticalTo(ConstValueParamType object) const;
    ValueReturnType objectAtIndex(int index);
    inline ConstValueReturnType objectAtIndex(int index) const {
        return const_cast<CSArray*>(this)->objectAtIndex(index);
    }
    ValueReturnType firstObject();
    inline ConstValueReturnType firstObject() const {
        return const_cast<CSArray*>(this)->firstObject();
    }
    ValueReturnType lastObject();
    inline ConstValueReturnType lastObject() const {
        return const_cast<CSArray*>(this)->lastObject();
    }
    bool containsObject(ConstValueParamType object) const;
    bool containsObjectIdenticalTo(ConstValueParamType object) const;
    CSArray* subarrayWithRange(int offset, int count);
    inline const CSArray* subarrayWithRange(int offset, int count) const {
        return const_cast<CSArray*>(this)->subarrayWithRange(offset, count);
    }
    ValueReturnType addObject();
    void addObject(ValueParamType object);
    template <typename otherV>
    void addObjects(const otherV* objects, int count);
    template <typename otherV, bool otherReadonly>
    void addObjectsFromArray(const CSArray<otherV, 1, otherReadonly>* otherArray);
    void addObjectsFromPointer(const void* ptr, int count);
    ValueReturnType insertObject(int index);
    void insertObject(int index, ValueParamType object);
    template <typename otherV>
    void insertObjects(int index, const otherV* objects, int count);
    template <typename otherV, bool otherReadonly>
    void insertObjectsFromArray(int index, const CSArray<otherV, 1, otherReadonly>* otherArray);
    void insertObjectsFromPointer(int index, const void* ptr, int count);
    void setObject(int index, ValueParamType object);
    void removeLastObject();
    bool removeObject(ConstValueParamType object);
    bool removeObjectIdenticalTo(ConstValueParamType object);
    void removeObjectAtIndex(int index);
    void removeObjectsWithRange(int index, int count);
    void removeAllObjects();

    typedef std::function<int(typename CSEntryType<V, readonly>::ConstValueParamType, typename CSEntryType<V, readonly>::ConstValueParamType)> CompareTo;

    void sort();
    void sort(const CompareTo& compareTo);
    CSArray* sortedArray();
    inline const CSArray* sortedArray() const {
        return const_cast<CSArray*>(this)->sortedArray();
    }
    CSArray* sortedArray(const CompareTo& compareTo);
    inline const CSArray* sortedArray(const CompareTo& compareTo) const {
        return const_cast<CSArray*>(this)->sortedArray();
    }
    
    //=====================================================================
    //linq
    template <typename otherV>
    CSArray<otherV>* select(const std::function<otherV(ConstValueParamType)>& func) const;
    CSArray* where(const std::function<bool(ConstValueParamType)>& func);
    inline const CSArray* where(const std::function<bool(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->where(func);
    }
    ValueReturnType firstObject(const std::function<bool(ConstValueParamType)>& func);
    inline ConstValueReturnType firstObject(const std::function<bool(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->firstObject(func);
    }
    ValueReturnType lastObject(const std::function<bool(ConstValueParamType)>& func);
    inline ConstValueReturnType lastObject(const std::function<bool(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->lastObject(func);
    }
    bool any(const std::function<bool(ConstValueParamType)>& func) const;
    bool all(const std::function<bool(ConstValueParamType)>& func) const;
    int count(const std::function<bool(ConstValueParamType)>& func) const;
    template <typename C>
    CSArray* orderBy(const std::function<C(ConstValueParamType)>& func);
    template <typename C>
    inline const CSArray* orderBy(const std::function<C(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->orderBy(func);
    }
    template <typename C>
    CSArray* orderByDescending(const std::function<C(ConstValueParamType)>& func);
    template <typename C>
    inline const CSArray* orderByDescending(const std::function<C(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->orderByDescending(func);
    }
    template <typename C>
    ValueReturnType max(const std::function<C(ConstValueParamType)>& func);
    template <typename C>
    inline ConstValueReturnType max(const std::function<C(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->max(func);
    }
    ValueReturnType max();
    inline ConstValueReturnType max() const {
        return const_cast<CSArray*>(this)->max();
    }
    template <typename C>
    ValueReturnType min(const std::function<C(ConstValueParamType)>& func);
    template <typename C>
    inline ConstValueReturnType min(const std::function<C(ConstValueParamType)>& func) const {
        return const_cast<CSArray*>(this)->min(func);
    }
    ValueReturnType min();
    inline ConstValueReturnType min() const {
        return const_cast<CSArray*>(this)->min();
    }
    //=====================================================================
    inline Iterator iterator() {
        return Iterator(this);
    }
    inline ReadonlyIterator iterator() const {
        return Iterator(const_cast<CSArray*>(this));
    }

    inline CSArray<V, 1, false>* asReadWrite() {
        return reinterpret_cast<CSArray<V, 1, false>*>(this);
    }
    inline CSArray<V, 1, true>* asReadOnly() {
        return reinterpret_cast<CSArray<V, 1, true>*>(this);
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

    uint sequentialHash() const;
    bool sequentialEqual(const CSArray* other) const;

    template <typename friendV, int friendDimension, bool friendReadonly>
    friend class CSArray;
};

//============================================================================================================================================
template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray() {
    setCapacity(12);
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray(int capacity) {
    setCapacity(capacity);
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray(PointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) {
		new (&_objects[_count++]) Type(CSEntryImpl<V>::retain(objects[i]));
    }
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray(int count, Type obj, ...) {
    setCapacity(count);

    new (&_objects[_count++]) Type(CSEntryImpl<V>::retain(obj));

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) {
        new (&_objects[_count++]) Type(CSEntryImpl<V>::retain(va_arg(ap, Type)));
    }
    va_end(ap);
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
CSArray<V, 1, readonly>::CSArray(const CSArray<otherV, 1, otherReadonly>* otherArray) {
    setCapacity(otherArray->_capacity);
    
    for (int i = 0; i < otherArray->_count; i++) {
		new (&_objects[_count++]) Type(CSEntryImpl<otherV>::retain(otherArray->_objects[i]));
    }
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::~CSArray() {
    fence_block(_fence);
    
    if (_capacity) {
        CSEntryImpl<V>::release(_objects, _count);
        free(_objects);
    }
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::expand(int count) {
    CSAssert(count > 0);
    int nextCapacity = CSMath::max(_capacity, 1);
    while (nextCapacity < _count + count) {
        nextCapacity <<= 1;
    }
    setCapacity(nextCapacity);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::setCapacity(int capacity) {
    CSAssert(capacity >= 0);
    if (_count <= capacity && capacity != _capacity) {
        if (capacity) _objects = (Type*)frealloc(_objects, capacity * sizeof(Type));
        else if (_capacity) {
            free(_objects);
            _objects = NULL;
        }
        _capacity = capacity;
    }
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::arrayWithObjects(int count, Type obj, ...) {
    CSArray<V, 1, readonly>* arr = CSArray<V, 1, readonly>::arrayWithCapacity(count);

    new (&arr->_objects[arr->_count++]) Type(CSEntryImpl<V>::retain(obj));

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) {
        new (&arr->_objects[arr->_count++]) Type(CSEntryImpl<V>::retain(va_arg(ap, Type)));
    }
    va_end(ap);

    return arr;
}

template <typename V, bool readonly>
int CSArray<V, 1, readonly>::indexOfObject(ConstValueParamType object) const {
    for (int i = 0; i < _count; i++) {
        if (CSEntryImpl<V>::isEqual(_objects[i], object)) return i;
    }
    return -1;
}

template <typename V, bool readonly>
int CSArray<V, 1, readonly>::indexOfObjectIdenticalTo(ConstValueParamType object) const {
    for (int i = 0; i < _count; i++) {
        if (_objects[i] == object) return i;
    }
    return -1;
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::objectAtIndex(int index) {
    CSAssert(index >= 0 && index < _count, "index out of range");
    return _objects[index];
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::firstObject() {
    return _count ? _objects[0] : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::lastObject() {
    return _count ? _objects[_count - 1] : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::containsObject(ConstValueParamType object) const {
    return indexOfObject(object) != -1;
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::containsObjectIdenticalTo(ConstValueParamType object) const {
    return indexOfObjectIdenticalTo(object) != -1;
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::subarrayWithRange(int offset, int count) {
    CSAssert(count > 0 && _offset >= 0 && _count >= offset + count, "index out of range");
    
    CSArray<V, 1, readonly>* arr = CSArray<V, 1, readonly>::arrayWithCapacity(count);
    for (int i = offset; i < offset + count; i++) {
		new (&arr->_objects[arr->_count++]) Type(CSEntryImpl<V>::retain(_objects[i]));
    }
    return arr;
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::addObject() {
    fence_block(_fence);
    
    expand(1);
    
    return CSEntryImpl<V>::notNullValue(_objects[_count++]);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::addObject(ValueParamType object) {
    fence_block(_fence);
    
    expand(1);

    new (&_objects[_count++]) Type(CSEntryImpl<V>::retain(object));
}

template <typename V, bool readonly> template <typename otherV>
void CSArray<V, 1, readonly>::addObjects(const otherV* objects, int count) {
    fence_block(_fence);

    expand(count);
    for (int i = 0; i < count; i++) {
        new (&_objects[_count++]) Type(CSEntryImpl<otherV>::retain(_objects[i]));
    }
    _count += count;
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
void CSArray<V, 1, readonly>::addObjectsFromArray(const CSArray<otherV, 1, otherReadonly>* otherArray) {
    addObjects(otherArray->_objects, otherArray->_count);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::addObjectsFromPointer(const void* ptr, int count) {
    fence_block(_fence);
    
    expand(count);
    memcpy(_objects + _count, ptr, count * sizeof(Type));
    _count += count;
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::insertObject(int index) {
    fence_block(_fence);
    
    CSAssert(index >= 0 && index <= _count, "index out of range");
    
    expand(1);
    
    int remaining = _count - index;
    if (remaining) {
        memmove(&_objects[index + 1], &_objects[index], remaining * sizeof(Type));
    }
    _count++;
    return CSEntryImpl<V>::notNullValue(_objects[index]);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::insertObject(int index, ValueParamType object) {
    fence_block(_fence);
    
    CSAssert(index >= 0 && index <= _count, "index out of range");
    
    expand(1);
    
    int remaining = _count - index;
    if (remaining) {
        memmove(&_objects[index + 1], &_objects[index], remaining * sizeof(Type));
    }
    _count++;
    new (&_objects[index]) Type(CSEntryImpl<V>::retain(object));
}

template <typename V, bool readonly> template <typename otherV>
void CSArray<V, 1, readonly>::insertObjects(int index, const otherV* objects, int count) {
    fence_block(_fence);

    CSAssert(index >= 0 && index <= _count, "index out of range");

    expand(count);
    int remaining = _count - index;
    if (remaining) {
        memmove(_objects + index + count, _objects + index, remaining * sizeof(Type));
    }
    for (int i = 0; i < count; i++) {
        new (&_objects[index + i]) Type(CSEntryImpl<otherV>::retain(_objects[i]));
    }
    _count += count;
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
void CSArray<V, 1, readonly>::insertObjectsFromArray(int index, const CSArray<otherV, 1, otherReadonly>* otherArray) {
    insertObjects(otherArray->_objects, otherArray->_count, index);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::insertObjectsFromPointer(int index, const void* ptr, int count) {
    fence_block(_fence);
    
    CSAssert(index >= 0 && index <= _count, "index out of range");
    
    expand(count);
    int remaining = _count - index;
    if (remaining) {
        memmove(&_objects[index + count], &_objects[index], remaining * sizeof(Type));
    }
    memcpy(_objects + _count, ptr, count * sizeof(Type));
    _count += count;
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::setObject(int index, ValueParamType object) {
    fence_block(_fence);

    CSAssert(index >= 0 && index < _count, "index out of range");

    CSEntryImpl<V>::retain(_objects[_count], object);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::removeLastObject() {
    fence_block(_fence);
    
    if (_count) {
        _count--;
        CSEntryImpl<V>::release(_objects[_count]);
    }
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::removeObject(ConstValueParamType object) {
    int index = indexOfObject(object);
    
    if (index == -1) return false;

    removeObjectAtIndex(index);
    return true;
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::removeObjectIdenticalTo(ConstValueParamType object) {
    int index = indexOfObjectIdenticalTo(object);
    
    if (index == -1) return false;

    removeObjectAtIndex(index);
    return true;
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::removeObjectAtIndex(int index) {
    fence_block(_fence);
    
    CSAssert(index >= 0 && index < _count, "index out of range");
    
    CSEntryImpl<V>::release(_objects[index]);

    _count--;
    
    int remaining = _count - index;
    if (remaining) {
        memmove(&_objects[index], &_objects[index + 1], remaining * sizeof(Type));
    }
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::removeObjectsWithRange(int index, int count) {
    fence_block(_fence);

    CSAssert(index >= 0 && index + count <= _count, "index out of range");

    CSEntryImpl<V>::release(&_objects[index], count);
    _count -= count;

    int remaining = _count - index;
    if (remaining) {
        memmove(&_objects[index], &_objects[index + count], remaining * sizeof(Type));
    }
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::removeAllObjects() {
    fence_block(_fence);
    
    CSEntryImpl<V>::release(_objects, _count);

    _count = 0;
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::sort() {
    fence_block(_fence);
    
    if (_count) {
        bool flag;
        
        do {
            flag = false;
            
            for (int i = 0; i < _count - 1; i++) {
                int rtn = CSEntryImpl<V>::compareTo(_objects[i], _objects[i + 1]);
                
                if (rtn > 0) {
                    CSMath::swap(_objects[i], _objects[i + 1]);
                    flag = true;
                }
            }
        } while(flag);
    }
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::sort(const CompareTo& compareTo) {
    fence_block(_fence);
    
    if (_count) {
        bool flag;
        
        do {
            flag = false;
            
            for (int i = 0; i < _count - 1; i++) {
                int rtn = compareTo(_objects[i], _objects[i + 1]);
                
                if (rtn > 0) {
                    CSMath::swap(_objects[i], _objects[i + 1]);
                    flag = true;
                }
            }
        } while(flag);
    }
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::sortedArray() {
    CSArray<V, 1, readonly>* array = CSArray<V, 1, readonly>::arrayWithArray(this);
    array->sort();
    return array;
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::sortedArray(const CompareTo& compareTo) {
    CSArray<V, 1, readonly>* array = CSArray<V, 1, readonly>::arrayWithArray(this);
    array->sort(compareTo);
    return array;
}

//======================================================================================================
//linq

template <typename V, bool readonly>
template <typename otherV>
CSArray<otherV>* CSArray<V, 1, readonly>::select(const std::function<otherV(ConstValueParamType)>& func) const {
    CSArray<otherV>* result = CSArray<otherV>::arrayWithCapacity(_count);
    for (int i = 0; i < _count; i++) result->addObject(func(_objects[i]));
    return result;
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::where(const std::function<bool(ConstValueParamType)>& func) {
    CSArray<V, 1, readonly>* result = CSArray<V, 1, readonly>::arrayWithCapacity(_count);
    for (int i = 0; i < _count; i++) {
        if (func(_objects[i])) result->addObject(_objects[i]);
    }
    return result;
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::firstObject(const std::function<bool(ConstValueParamType)>& func) {
    for (int i = 0; i < _count; i++) {
        if (func(_objects[i])) return _objects[i];
    }
    return CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::lastObject(const std::function<bool(ConstValueParamType)>& func) {
    for (int i = _count - 1; i >= 0; i--) {
        if (func(_objects[i])) return _objects[i];
    }
    return CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::any(const std::function<bool(ConstValueParamType)>& func) const {
    for (int i = 0; i < _count; i++) {
        if (func(_objects[i])) return true;
    }
    return false;
}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::all(const std::function<bool(ConstValueParamType)>& func) const {
    for (int i = 0; i < _count; i++) {
        if (func(_objects[i])) return false;
    }
    return true;
}

template <typename V, bool readonly>
int CSArray<V, 1, readonly>::count(const std::function<bool(ConstValueParamType)>& func) const {
    int count = 0;
    for (int i = 0; i < _count; i++) {
        if (func(_objects[i])) count++;
    }
    return count;
}

template <typename V, bool readonly>
template <typename C>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::orderBy(const std::function<C(ConstValueParamType)>& func) {
    auto compareTo = [func](ConstValueParamType a, ConstValueParamType b) {
        C ac = func(a);
        C bc = func(b);
        if (ac < bc) return -1;
        else if (ac > bc) return 1;
        else return 0;
    };
    return sortedArray(compareTo);
}

template <typename V, bool readonly>
template <typename C>
CSArray<V, 1, readonly>* CSArray<V, 1, readonly>::orderByDescending(const std::function<C(ConstValueParamType)>& func) {
    auto compareTo = [func](ConstValueParamType a, ConstValueParamType b) {
        C ac = func(a);
        C bc = func(b);
        if (ac < bc) return 1;
        else if (ac > bc) return -1;
        else return 0;
    };
    return sortedArray(compareTo);
}

template <typename V, bool readonly>
template <typename C>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::max(const std::function<C(ConstValueParamType)>& func) {
    if (!_count) return CSEntryImpl<V>::nullValue();
    int index = 0;
    C maxv = func(_objects[0]);
    for (int i = 1; i < _count; i++) {
        C v = func(_objects[i]);
        if (maxv < v) {
            index = i;
            maxv = v;
        }
    }
    return _objects[index];
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType max() {
    if (!_count) return CSEntryImpl<V>::nullValue();
    int index = 0;
    for (int i = 1; i < _count; i++) {
        C v = func(_objects[i]);
        if (_objects[index] < _objects[i]) index = i;
    }
    return _objects[index];
}

template <typename V, bool readonly>
template <typename C>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::min(const std::function<C(ConstValueParamType)>& func) {
    if (!_count) return CSEntryImpl<V>::nullValue();
    int index = 0;
    C minv = func(_objects[0]);
    for (int i = 1; i < _count; i++) {
        C v = func(_objects[i]);
        if (minv > v) {
            index = i;
            minv = v;
        }
    }
    return _objects[index];
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType min() {
    if (!_count) return CSEntryImpl<V>::nullValue();
    int index = 0;
    for (int i = 1; i < _count; i++) {
        C v = func(_objects[i]);
        if (_objects[index] > _objects[i]) index = i;
    }
    return _objects[index];
}

//======================================================================================================

template<typename V, bool readonly>
uint CSArray<V, 1, readonly>::sequentialHash() const {
    CSHash hash;
    for (int i = 0; i < _count; i++) hash.combine(_objects[i]);
    return hash;
}

template<typename V, bool readonly>
bool CSArray<V, 1, readonly>::sequentialEqual(const CSArray* other) const {
    if (_count != other->_count) return false;
    for (int i = 0; i < _count; i++) {
        if (!CSEntryImpl<V>::isEqual(_objects[i], other->_objects[i])) return false;
    }
    return true;
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::ReadonlyIterator::ReadonlyIterator(CSArray<V, 1, readonly>* array) : _array(array), _index(0) {

}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::ReadonlyIterator::~ReadonlyIterator() {

}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::Iterator::Iterator(CSArray<V, 1, readonly>* array) : ReadonlyIterator(array) {

}

template <typename V, bool readonly>
bool CSArray<V, 1, readonly>::ReadonlyIterator::remaining() const {
    return _index < _array->_count;
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ConstValueReturnType CSArray<V, 1, readonly>::ReadonlyIterator::object() const {
    return _array->objectAtIndex(_index);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::ReadonlyIterator::next() {
    ++_index;
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::Iterator::remove() {
    _array->removeObjectAtIndex(_index);
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::Iterator::insert() {
    return _array->insertObject(_index);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::Iterator::insert(ValueParamType obj) {
    _array->insertObject(_index, obj);
}

template <typename V, bool readonly>
void CSArray<V, 1, readonly>::Iterator::set(ValueParamType obj) {
    _array->setObject(_index, obj);
}

template <typename V, bool readonly>
typename CSArray<V, 1, readonly>::ValueReturnType CSArray<V, 1, readonly>::Iterator::object() {
    return _array->objectAtIndex(_index);
}

#endif
