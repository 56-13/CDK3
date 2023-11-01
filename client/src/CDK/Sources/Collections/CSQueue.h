#ifndef __CDK__CSQueue__
#define __CDK__CSQueue__

#include "CSArray.h"

template <typename V, bool readonly = true>
class CSQueue : public CSObject {
public:
    typedef typename CSEntryType<V, readonly>::Type Type;
    typedef typename CSEntryType<V, readonly>::ValueParamType ValueParamType;
    typedef typename CSEntryType<V, readonly>::ConstValueParamType ConstValueParamType;
    typedef typename CSEntryType<V, readonly>::PointerParamType PointerParamType;
    typedef typename CSEntryType<V, readonly>::ValueReturnType ValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstValueReturnType ConstValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstTemplateType ConstTemplateType;
    
    class ReadonlyIterator {
    protected:
        CSQueue* _queue;
        int _cursor;
        bool _forward;
        bool _removed;
        
        ReadonlyIterator(CSQueue* queue, bool forward);
    public:
        virtual ~ReadonlyIterator();
        
        bool remaining() const;
        ConstValueReturnType object() const;
        void next();
        friend class CSQueue;
    };
    
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSQueue* queue, bool forward);
    public:
        void remove();
        ValueReturnType insert();
        void insert(ValueParamType obj);
        void set(ValueParamType obj);
        ValueReturnType object();
        friend class CSQueue;
    };
    
    friend class Iterator;
    friend class ReadonlyIterator;
    
private:
    struct Node {
        Type object;
        int prev;
        int next;
        bool exists;
    };
    Node* _nodes = NULL;
    int _capacity = 0;
    int _count = 0;
    int _expansion = 0;
    int _first = -1;
    int _last = -1;
#ifdef CS_ASSERT_DEBUG
    mutable std::atomic<int> _fence;
#endif
public:
    CSQueue();
    explicit CSQueue(int capacity);
    CSQueue(PointerParamType objects, int count);
    CSQueue(int count, Type ob, ...);
    template <typename otherV, bool otherReadonly>
    CSQueue(const CSQueue<otherV, otherReadonly>* otherQueue);
    virtual ~CSQueue();
public:
    static inline CSQueue* queue() {
        return autorelease(new CSQueue());
    }
    static inline CSQueue* queueWithCapacity(int capacity) {
        return autorelease(new CSQueue(capacity));
    }
    static inline CSQueue* queueWithObjects(PointerParamType objects, int count) {
        return autorelease(new CSQueue(objects, count));
    }
    static CSQueue* queueWithObjects(int count, Type ob, ...);

    template <typename otherV, bool otherReadonly>
    static inline CSQueue* queueWithQueue(const CSQueue<otherV, otherReadonly>* otherQueue) {
        return autorelease(new CSQueue(otherQueue));
    }
private:
    void expand(int& p);
    ValueReturnType insert(int p, bool forward);
    void insert(ValueParamType obj, int p, bool forward);
    void set(ValueParamType obj, int p);
    void remove(int p);
    template <typename otherV, bool otherReadonly>
    void allObjects(CSArray<otherV, 1, otherReadonly>* values, bool forward) const;
public:
    inline int count() const {
        return _count;
    }

    void setCapacity(int capacity);

    inline int capacity() const {
        return _capacity;
    }

    ValueReturnType firstObject();
    inline ConstValueReturnType firstObject() const {
        return const_cast<CSQueue*>(this)->firstObject();
    }
    ValueReturnType lastObject();
    inline ConstValueReturnType lastObject() const {
        return const_cast<CSQueue*>(this)->lastObject();
    }
    void removeFirstObject();
    void removeLastObject();
    ValueReturnType addObject(bool forward = true);
    void addObject(ValueParamType obj, bool forward = true);
    bool removeObject(ConstValueParamType obj, bool forward = true);
    bool removeObjectIdenticalTo(ConstValueParamType obj, bool forward = true);
    void removeAllObjects();
    bool containsObject(ConstValueParamType obj, bool forward = true) const;
    bool containsObjectIdenticalTo(ConstValueParamType obj, bool forward = true) const;
    inline Iterator iterator(bool forward = true) {
        return Iterator(this, forward);
    }
    inline ReadonlyIterator iterator(bool forward = true) const {
        return ReadonlyIterator(const_cast<CSQueue*>(this), forward);
    }
    CSArray<V, 1, readonly>* allObjects(bool forward = true);
    inline const CSArray<V, 1, readonly>* allObjects(bool forward = true) const {
        return const_cast<CSQueue*>(this)->allObjects(forward);
    }
    CSArray<ConstTemplateType, 1, false>* allObjectsMutable(bool forward = true) const;
    
    inline CSQueue<V, false>* asReadWrite() {
        return reinterpret_cast<CSQueue<V, false>*>(this);
    }
    inline CSQueue<V, true>* asReadOnly() {
        return reinterpret_cast<CSQueue<V, true>*>(this);
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
    bool sequentialEqual(const CSQueue* other) const;
};

template <typename V, bool readonly>
CSQueue<V, readonly>::CSQueue() {
    setCapacity(12);
}

template <typename V, bool readonly>
CSQueue<V, readonly>::CSQueue(int capacity) {
    setCapacity(capacity);
}

template <typename V, bool readonly>
CSQueue<V, readonly>::CSQueue(PointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) addObject(objects[i]);
}

template <typename V, bool readonly>
CSQueue<V, readonly>::CSQueue(int count, Type obj, ...) {
    setCapacity(count);

    addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) addObject(va_arg(ap, Type));
    va_end(ap);
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
CSQueue<V, readonly>::CSQueue(const CSQueue<otherV, otherReadonly>* otherQueue) {
    setCapacity(otherQueue->_capacity);
    
    int p = otherQueue->_first;
    
    while (p != -1) {
        addObject(otherQueue->_nodes[p].object);
        p = otherQueue->_nodes[p].next;
    }
}

template <typename V, bool readonly>
CSQueue<V, readonly>::~CSQueue() {
    fence_block(_fence);
    
    if (_capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (_nodes[i].exists) CSEntryImpl<V>::release(_nodes[i].object);
        }
        free(_nodes);
    }
}

template <typename V, bool readonly>
CSQueue<V, readonly>* CSQueue<V, readonly>::queueWithObjects(int count, Type obj, ...) {
    CSQueue<V, readonly>* queue = CSQueue<V, readonly>::queueWithCapacity(count);

    queue->addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) queue->addObject(va_arg(ap, Type));
    va_end(ap);

    return arr;
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::expand(int& p) {
    if (_expansion == _capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (!_nodes[i].exists) {
                p = i;
                return;
            }
        }
		int nextCapacity = CSMath::max(_capacity << 1, 1);
        setCapacity(nextCapacity);
    }
    p = _expansion++;
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::setCapacity(int capacity) {
    CSAssert(capacity >= 0);
    if (_expansion <= capacity && capacity != _capacity) {
        if (capacity) {
            _nodes = (Node*)frealloc(_nodes, capacity * sizeof(Node));
            if (capacity > _capacity) memset(&_nodes[_capacity], 0, (capacity - _capacity) * sizeof(Node));
        }
        else if (_capacity) {
            free(_nodes);
            _nodes = NULL;
        }
        _capacity = capacity;
    }
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ValueReturnType CSQueue<V, readonly>::insert(int p, bool forward) {
    int prev = !forward && p != -1 ? _nodes[p].prev : p;
    int next = forward && p != -1 ? _nodes[p].next : p;
    
    expand(p);
    
    Node& node = _nodes[p];
    node.prev = prev;
    node.next = next;
    
    if (prev != -1) _nodes[prev].next = p;
    else _first = p;
    if (next != -1) _nodes[next].prev = p;
    else _last = p;

    _count++;

    return CSEntryImpl<V>::notNullValue(node.object);
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::insert(ValueParamType obj, int p, bool forward) {
    int prev = !forward && p != -1 ? _nodes[p].prev : p;
    int next = forward && p != -1 ? _nodes[p].next : p;
    
    expand(p);
    
    Node& node = _nodes[p];
    new (&node.object) Type(CSEntryImpl<V>::retain(obj));
    node.prev = prev;
    node.next = next;
    
    if (prev != -1) _nodes[prev].next = p;
    else _first = p;
    if (next != -1) _nodes[next].prev = p;
    else _last = p;

    _count++;
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::set(ValueParamType obj, int p) {
    Node& node = _nodes[p];

    CSEntryImpl<V>::retain(node.object, obj);
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::remove(int p) {
    Node& node = _nodes[p];
    
    CSEntryImpl<V>::release(node.object);
    node.exists = false;
    
    _nodes[node.prev].next = node.next;
    if (_first == p) _first = node.next;
    _nodes[node.next].prev = node.prev;
    if (_last == p) _last = node.prev;

    _count--;
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ValueReturnType CSQueue<V, readonly>::firstObject() {
    return _first != -1 ? _nodes[_first].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ValueReturnType CSQueue<V, readonly>::lastObject() {
    return _last != -1 ? _nodes[_last].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::removeFirstObject() {
    fence_block(_fence);
    if (_first != -1) remove(_first);
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::removeLastObject() {
    fence_block(_fence);
    if (_last != -1) remove(_last);
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ValueReturnType CSQueue<V, readonly>::addObject(bool forward) {
    fence_block(_fence);
    return insert(forward ? _last : _first, forward);
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::addObject(ValueParamType obj, bool forward) {
    fence_block(_fence);
    insert(obj, forward ? _last : _first, forward);
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::removeObject(ConstValueParamType obj, bool forward) {
    fence_block(_fence);
    
    if (forward) {
        int p = _first;
        
        while (p != -1) {
            Node& node = _nodes[p];
            
            if (CSEntryImpl<V>::isEqual(obj, node.object)) {
                remove(p);
                return true;
            }
            p = node.next;
        }
    }
    else {
        int p = _last;
        
        while (p != -1) {
            Node& node = _nodes[p];
            
            if (CSEntryImpl<V>::isEqual(obj, node.object)) {
                remove(p);
                return true;
            }
            p = node.prev;
        }
    }
    return false;
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::removeObjectIdenticalTo(ConstValueParamType obj, bool forward) {
    fence_block(_fence);

    if (forward) {
        int p = _first;

        while (p != -1) {
            Node& node = _nodes[p];

            if (obj == node.object) {
                remove(p);
                return true;
            }
            p = node.next;
        }
    }
    else {
        int p = _last;

        while (p != -1) {
            Node& node = _nodes[p];

            if (obj == node.object) {
                remove(p);
                return true;
            }
            p = node.prev;
        }
    }
    return false;
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::removeAllObjects() {
    fence_block(_fence);

    for (int i = 0; i < _expansion; i++) {
        Node& node = _nodes[i];
        if (node.exists) {
            CSEntryImpl<V>::release(node.object);
            node.exists = false;
        }
    }
    _count = 0;
    _expansion = 0;
    _first = -1;
    _last = -1;
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::containsObject(ConstValueParamType obj, bool forward) const {
    if (forward) {
        int p = _first;
        
        while (p != -1) {
            Node& node = _nodes[p];
            if (CSEntryImpl<V>::isEqual(obj, node.object)) return true;
            p = node.next;
        }
    }
    else {
        int p = _last;
        
        while (p != -1) {
            Node& node = _nodes[p];
            if (CSEntryImpl<V>::isEqual(obj, node.object)) return true;
            p = node.prev;
        }
    }
    return false;
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::containsObjectIdenticalTo(ConstValueParamType obj, bool forward) const {
    if (forward) {
        int p = _first;

        while (p != -1) {
            Node& node = _nodes[p];
            if (obj == node.object) return true;
            p = node.next;
        }
    }
    else {
        int p = _last;

        while (p != -1) {
            Node& node = _nodes[p];
            if (obj == node.object) return true;
            p = node.prev;
        }
    }
    return false;
}

template <typename V, bool readonly>
CSQueue<V, readonly>::ReadonlyIterator::ReadonlyIterator(CSQueue* queue, bool forward) : _queue(queue), _forward(forward), _removed(false) {
    _queue->fence();

    _cursor = forward ? _queue->_first : _queue->_last;
}

template <typename V, bool readonly>
CSQueue<V, readonly>::ReadonlyIterator::~ReadonlyIterator() {
    _queue->flict();
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::ReadonlyIterator::remaining() const {
    return _cursor != -1;
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ConstValueReturnType CSQueue<V, readonly>::ReadonlyIterator::object() const {
    return _cursor != -1 ? _queue->_nodes[_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::ReadonlyIterator::next() {
    if (_removed) _removed = false;
    else if (_cursor != -1) {
        Node& node = _queue->_nodes[_cursor];
        _cursor = _forward ? node.next : node.prev;
    }
}

template <typename V, bool readonly>
CSQueue<V, readonly>::Iterator::Iterator(CSQueue* queue, bool forward) : ReadonlyIterator(queue, forward) {

}

template <typename V, bool readonly>
void CSQueue<V, readonly>::Iterator::remove() {
    CSAssert(_cursor != -1, "invalid cursor");
    Node& node = _queue->_nodes[_cursor];
    int next = _forward ? node.next : node.prev;
    _queue->remove(_cursor);
    _cursor = next;
    _removed = true;
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::Iterator::insert(ValueParamType obj) {
    if (_cursor != -1) _queue->insert(obj, _cursor, !forward);
    else _queue->addObject(obj, forward);
}

template <typename V, bool readonly>
void CSQueue<V, readonly>::Iterator::set(ValueParamType obj) {
    CSAssert(_cursor != -1, "invalid cursor");
    _queue->set(obj, _cursor, false);
}

template <typename V, bool readonly>
typename CSQueue<V, readonly>::ValueReturnType CSQueue<V, readonly>::Iterator::object() {
    return _cursor != -1 ? _queue->_nodes[_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
void CSQueue<V, readonly>::allObjects(CSArray<otherV, 1, otherReadonly>* values, bool forward) const {
    if (forward) {
        int p = _first;
        while (p != -1) {
            Node& node = _nodes[p];
            values->addObject(node.obj);
            p = node.next;
        }
    }
    else {
        int p = _last;
        while (p != -1) {
            Node& node = _nodes[p];
            values->addObject(node.obj);
            p = node.prev;
        }
    }
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>* CSQueue<V, readonly>::allObjects(bool forward) {
    CSArray<V, 1, false>* objects = CSArray<V, 1, false>::arrayWithCapacity(_count);
    allObjects(objects, forward);
    return objects;
}

template <typename V, bool readonly>
CSArray<typename CSQueue<V, readonly>::ConstTemplateType, 1, false>* CSQueue<V, readonly>::allObjectsMutable(bool forward) const {
    CSArray<ConstTemplateType, 1, false>* objects = CSArray<ConstTemplateType, 1, false>::arrayWithCapacity(_count);
    allObjects(objects, forward);
    return objects;
}

template <typename V, bool readonly>
uint CSQueue<V, readonly>::sequentialHash() const {
    CSHash hash;
    int p = _first;
    while (p != -1) {
        Node& node = _nodes[p];
        hash.combine(node.object);
        p = node.next;
    }
    return hash;
}

template <typename V, bool readonly>
bool CSQueue<V, readonly>::sequentialEqual(const CSQueue* other) const {
    if (_count != other->_count) return false;

    int p0 = _first;
    int p1 = other->_first;
    while (p0 != -1) {
        Node& node0 = _nodes[p0];
        Node& node1 = other->_nodes[p1];
        if (!CSEntryImpl<V>::isEqual(node0.object, node1.object)) return false;
        p0 = node0.next;
        p1 = node1.next;
    }
    return true;
}

#endif
