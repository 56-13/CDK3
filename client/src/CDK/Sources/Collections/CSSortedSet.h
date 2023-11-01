#ifndef __CDK__CSSortedSet__
#define __CDK__CSSortedSet__

#include "CSArray.h"

template <typename V, bool readonly = true>
class CSSortedSet : public CSObject {
public:
    typedef typename CSEntryType<V, readonly>::Type Type;
    typedef typename CSEntryType<V, readonly>::ValueParamType ValueParamType;
    typedef typename CSEntryType<V, readonly>::PointerParamType PointerParamType;
    typedef typename CSEntryType<V, readonly>::ConstValueParamType ConstValueParamType;
    typedef typename CSEntryType<V, readonly>::ValueReturnType ValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstValueReturnType ConstValueReturnType;
    typedef typename CSEntryType<V, readonly>::ConstTemplateType ConstTemplateType;
    
    class ReadonlyIterator {
    private:
        CSSortedSet* _set;
        int* _cursor;
        bool _ascending;
        
        ReadonlyIterator(CSSortedSet* set, bool ascending);
    public:
        virtual ~ReadonlyIterator();
        
        bool remaining() const;
        ConstValueReturnType object() const;
        void next();
        
        friend class CSSortedSet;
    };
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSSortedSet* set, bool ascending);
    public:
        ValueReturnType object();
        
        friend class CSSortedSet;
    };

    friend class ReadonlyIterator;
    friend class Iterator;
private:
    struct Node {
        Type object;
        int prev;
        int left;
        int right;
        int height;
        bool exists;
    };
    Node* _nodes = NULL;
    int _capacity = 0;
    int _count = 0;
    int _expansion = 0;
    int _root = -1;
#ifdef CS_ASSERT_DEBUG
    mutable std::atomic<int> _fence;
#endif
public:
    CSSortedSet();
    explicit CSSortedSet(int capacity);
    CSSortedSet(PointerParamType objects, int count);
    CSSortedSet(int count, Type obj, ...);
    template <typename otherV, bool otherReadonly>
    CSSortedSet(const CSSortedSet<otherV, otherReadonly>* otherSet);
    virtual ~CSSortedSet();
public:
    static inline CSSortedSet* set() {
        return autorelease(new CSSortedSet());
    }
    static inline CSSortedSet* setWithCapacity(int capacity) {
        return autorelease(new CSSortedSet(capacity));
    }
    static inline CSSortedSet* setWithObjects(PointerParamType objects, int count) {
        return autorelease(new CSSortedSet(objects, count));
    }
    static CSSortedSet* setWithObjects(int count, Type obj, ...);

    template <typename otherV, bool otherReadonly>
    static inline CSSortedSet* setWithSet(const CSSortedSet<otherV, otherReadonly>* otherSet) {
        return autorelease(new CSSortedSet(otherSet));
    }
private:
    int expandReady();
    int expandCommit(int p);
    void insert(ValueParamType obj, int prev, int next, int& p);
    bool contains(ConstValueParamType, int p) const;
    bool remove(ConstValueParamType, int& p);
    void remove(int& p);
    int height(int p);
    int srl(int p1);
    int srr(int p1);
    int drl(int p1);
    int drr(int p1);
    int rrr(int prev, int& p);
    template <typename otherV, bool otherReadonly>
    void allObjects(int p, CSArray<otherV, 1, otherReadonly>* values, bool ascending) const;
public:
    inline int count() const {
        return _count;
    }

    void setCapacity(int capacity);

    inline int capacity() const {
        return _capacity;
    }

    void addObject(ValueParamType obj);
    bool removeObject(ConstValueParamType obj);
    void removeAllObjects();
    bool containsObject(ConstValueParamType obj) const;
    inline ReadonlyIterator iterator(bool ascending = true) const {
        return ReadonlyIterator(const_cast<CSSortedSet*>(this), ascending);
    }
    inline Iterator iterator(bool ascending = true) {
        return Iterator(this, ascending);
    }

    const CSArray<V>* allObjects(bool ascending = true) const;
    CSArray<ConstTemplateType, 1, false>* allObjectsMutable(bool ascending = true) const;
    
    inline CSSortedSet<V, false>* asReadWrite() {
        return reinterpret_cast<CSSortedSet<V, false>*>(this);
    }
    inline CSSortedSet<V, true>* asReadOnly() {
        return reinterpret_cast<CSSortedSet<V, true>*>(this);
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
    friend class CSSortedSet;
};

template <typename V, bool readonly>
CSSortedSet<V, readonly>::CSSortedSet() {
    setCapacity(12);
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::CSSortedSet(int capacity) {
    setCapacity(capacity);
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::CSSortedSet(int count, Type obj, ...) {
    setCapacity(count);

    addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) addObject(va_arg(ap, Type));
    va_end(ap);
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::CSSortedSet(PointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) addObject(objects[i]);
}

template <typename V, bool readonly> template <typename otherV, bool otherReadonly>
CSSortedSet<V, readonly>::CSSortedSet(const CSSortedSet<otherV, otherReadonly>* otherSet) {
    setCapacity(otherSet->_capacity);
    
    for (int i = 0; i < otherSet->_expansion; i++) {
        if (otherSet->_nodes[i].exists) addObject(otherSet->_nodes[i].object);
    }
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::~CSSortedSet() {
    fence_block(_fence);
    
    if (_capacity) {
        for (int i = 0; i < _expansion; i++) {
            Node& node = _nodes[i];
            if (node.exists) {
                CSEntryImpl<V>::release(node.object);
            }
        }
        free(_nodes);
    }
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>* CSSortedSet<V, readonly>::setWithObjects(int count, Type obj, ...) {
    CSSortedSet<V>* set = CSSortedSet<V, readonly>::setWithCapacity(count);

    set->addObject(obj);

    va_list ap;
    va_start(ap, obj);
    for (int i = 1; i < count; i++) set->addObject(va_arg(ap, Type));
    va_end(ap);

    return arr;
}

template <typename V, bool readonly>
int CSSortedSet<V, readonly>::expandReady() {
    if (_expansion == _capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (!_nodes[i].exists) return i;
        }
        int nextCapacity = CSMath::max(_capacity << 1, 1);
        setCapacity(nextCapacity);
    }
    return -1;
}

template <typename V, bool readonly>
int CSSortedSet<V, readonly>::expandCommit(int p) {
    return p == -1 ? _expansion++ : p;
}

template <typename V, bool readonly>
void CSSortedSet<V, readonly>::setCapacity(int capacity) {
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
void CSSortedSet<V, readonly>::insert(ValueParamType obj, int prev, int next, int& p) {
    if (p == -1) {
        p = expandCommit(next);
        
        Node& node = _nodes[p];
        
        new (&node.object) Type(CSEntryImpl<V>::retain(obj));
        node.left = -1;
        node.right = -1;
        node.prev = prev;
        node.height = 0;
        node.exists = true;
        _count++;
    }
    else {
        Node& node = _nodes[p];
        
        int comp = CSEntryImpl<V>::compareTo(obj, node.object);
        
        if (comp == 0) {
            if (node.object != obj) {
                CSEntryImpl<V>::release(node.object);
                new (&node.object) Type(CSEntryImpl<V>::retain(obj));
            }
        }
        else if (comp < 0) {
            insert(obj, p, node.left);
            
            if ((height(node.left) - height(node.right)) == 2) {
                if (CSEntryImpl<V>::compareTo(obj, _nodes[node.left].object) < 0) {
                    p = srl(p);
                }
                else {
                    p = drl(p);
                }
            }
        }
        else {
            insert(obj, p, node.right);
            
            if ((height(node.right) - height(node.left)) == 2) {
                if (CSEntryImpl<V>::compareTo(obj, _nodes[node.right].object) > 0) {
                    p = srr(p);
                }
                else {
                    p = drr(p);
                }
            }
        }
        _nodes[p].height = CSMath::max(height(_nodes[p].left), height(_nodes[p].right)) + 1;
    }
}

template <typename V, bool readonly>
bool CSSortedSet<V, readonly>::contains(ConstValueParamType obj, int p) const {
    if (p == -1) return false;
    
    const Node& node = _nodes[p];
    
    int comp = CSEntryImpl<V>::compareTo(obj, node.object);
    
    if (comp < 0) {
        return contains(obj, node.left);
    }
    else if (comp > 0) {
        return contains(obj, node.right);
    }
    else {
        return CSEntryImpl<V>::isEqual(obj, node.object);
    }
}

template <typename V, bool readonly>
bool CSSortedSet<V, readonly>::remove(ConstValueParamType obj, int& p) {
    if (p == -1) {
        Node& node = _nodes[p];
        
        int comp = CSEntryImpl<V>::compareTo(obj, node.object);
        
        if (comp < 0) {
            return remove(obj, node.left);
        }
        else if (comp > 0) {
            return remove(obj, node.right);
        }
        else if (CSEntryImpl<V>::isEqual(obj, node.object)) {
            remove(p);
            return true;
        }
    }
    return false;
}

template <typename V, bool readonly>
void CSSortedSet<V, readonly>::remove(int& p) {
    Node& node = _nodes[p];
    
    CSEntryImpl<V>::release(node.object);
    node.exists = false;
    _count--;
    
    if ((node.left == -1) && (node.right == -1)) {
        p = -1;
    }
    else if (node.left == -1) {
        p = node.right;
        _nodes[node.right].prev = node.prev;
    }
    else if (node.right == -1) {
        p = node.left;
        _nodes[node.left].prev = node.prev;
    }
    else {
        node.object = _nodes[rrr(p, node.right)].object;
    }
}

template <typename V, bool readonly>
int CSSortedSet<V, readonly>::height(int p) {
    return p == -1 ? -1 : _nodes[p].height;
}

template <typename V, bool readonly>
int CSSortedSet<V, readonly>::srl(int p1) {
    Node& n1 = _nodes[p1];
    int p2 = _nodes[p1].left;
    Node& n2 = _nodes[p2];
    n1.left = n2.right;
    if (n1.left != -1) {
        _nodes[n1.left].prev = p1;
    }
    n2.right = p1;
    n2.prev = n1.prev;
    n1.prev = p2;
    n1.height = CSMath::max(height(n1.left), height(n1.right)) + 1;
    n2.height = CSMath::max(height(n2.left), n1.height) + 1;
    return p2;
}

template <typename V, bool readonly>
int CSSortedSet<V, readonly>::srr(int p1) {
    Node& n1 = _nodes[p1];
    int p2 = _nodes[p1].right;
    Node& n2 = _nodes[p2];
    n1.right = n2.left;
    if (n1.right != -1) {
        _nodes[n1.right].prev = p1;
    }
    n2.left = p1;
    n2.prev = n1.prev;
    n1.prev = p2;
    n1.height = CSMath::max(height(n1.left), height(n1.right)) + 1;
    n2.height = CSMath::max(n1.height, height(n2.right)) + 1;
    return p2;
}
template <typename V, bool readonly>
int CSSortedSet<V, readonly>::drl(int p1) {
    _nodes[p1].left = srr(_nodes[p1].left);
    return srl(p1);
}
template <typename V, bool readonly>
int CSSortedSet<V, readonly>::drr(int p1) {
    _nodes[p1].right = srl(_nodes[p1].right);
    return srr(p1);
}
template <typename V, bool readonly>
int CSSortedSet<V, readonly>::rrr(int prev, int& p) {
    int rtn;
    
    Node& node = _nodes[p];
    if (node.left == -1) {
        rtn = p;
        p = node.right;
        if (p != -1) {
            _nodes[p].prev = prev;
        }
    }
    else
    {
        rtn = rrr(p, node.left);
    }
    return rtn;
}
template <typename V, bool readonly>
void CSSortedSet<V, readonly>::addObject(ValueParamType obj) {
    fence_block(_fence);
    
    insert(obj, -1, expandReady(), _root);
}
template <typename V, bool readonly>
bool CSSortedSet<V, readonly>::removeObject(ConstValueParamType obj) {
    fence_block(_fence);
    
    return remove(obj, _root);
}
template <typename V, bool readonly>
void CSSortedSet<V, readonly>::removeAllObjects() {
    fence_block(_fence);
    
    for (int i = 0; i < _expansion; i++) {
        Node& node = _nodes[i];
        if (node.exists) {
            CSEntryImpl<V>::release(node.object);
            node.exists = false;
        }
    }
    _expansion = 0;
    _count = 0;
    _root = -1;
}
template <typename V, bool readonly>
bool CSSortedSet<V, readonly>::containsObject(ConstValueParamType obj) const {
    return contains(obj, _root);
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::ReadonlyIterator::ReadonlyIterator(CSSortedSet* set, bool ascending) : _set(set), _ascending(ascending) {
    _set->fence();

    _cursor = &_set->_root;
    
    if (*_cursor != -1) {
        if (ascending) {
            while (_set->_nodes[*_cursor].left != -1) {
                _cursor = &_set->_nodes[*_cursor].left;
            }
        }
        else {
            while (_set->_nodes[*_cursor].right != -1) {
                _cursor = &_set->_nodes[*_cursor].right;
            }
        }
    }
    if (*_cursor == -1) {
        _cursor = NULL;
    }
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::ReadonlyIterator::~ReadonlyIterator() {
    _set->flict();
}

template <typename V, bool readonly>
bool CSSortedSet<V, readonly>::ReadonlyIterator::remaining() const {
    return _cursor != NULL;
}

template <typename V, bool readonly>
typename CSSortedSet<V, readonly>::ConstValueReturnType CSSortedSet<V, readonly>::ReadonlyIterator::object() const {
    return _cursor ? _set->_nodes[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
void CSSortedSet<V, readonly>::ReadonlyIterator::next() {
    if (_cursor) {
        if (_ascending) {
            if (_set->_nodes[*_cursor].right != -1) {
                _cursor = &_set->_nodes[*_cursor].right;
                
                while (_set->_nodes[*_cursor].left != -1) {
                    _cursor = &_set->_nodes[*_cursor].left;
                }
            }
            else {
                for (; ; ) {
                    int& prev = _set->_nodes[*_cursor].prev;
                    
                    if (prev == -1) {
                        _cursor = NULL;
                        break;
                    }
                    else if (_set->_nodes[prev].left == *_cursor) {
                        _cursor = &prev;
                        break;
                    }
                    else _cursor = &prev;
                }
            }
        }
        else {
            if (_set->_nodes[*_cursor].left != -1) {
                _cursor = &_set->_nodes[*_cursor].left;
                
                while (_set->_nodes[*_cursor].right != -1) {
                    _cursor = &_set->_nodes[*_cursor].right;
                }
            }
            else {
                for (; ; ) {
                    int& prev = _set->_nodes[*_cursor].prev;
                    
                    if (prev == -1) {
                        _cursor = NULL;
                        break;
                    }
                    else if (_set->_nodes[prev].right == *_cursor) {
                        _cursor = &prev;
                        break;
                    }
                    else _cursor = &prev;
                }
            }
        }
    }
}

template <typename V, bool readonly>
CSSortedSet<V, readonly>::Iterator::Iterator(CSSortedSet* set, bool ascending) : ReadOnlyIterator(set, ascending) {
    
}

template <typename V, bool readonly>
typename CSSortedSet<V, readonly>::ValueReturnType CSSortedSet<V, readonly>::Iterator::object() {
    return _cursor ? _set->_nodes[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename V, bool readonly>
template <typename otherV, bool otherReadonly>
void CSSortedSet<V, readonly>::allObjects(int p, CSArray<otherV, 1, otherReadonly>* objects, bool ascending) const {
    if (p != -1) {
        Node& node = _nodes[p];
        if (ascending) {
            allObjects(node.left, objects, ascending);
            objects->addObject(node.object);
            allObjects(node.right, objects, ascending);
        }
        else {
            allObjects(node.right, objects, ascending);
            objects->addObject(node.object);
            allObjects(node.left, objects, ascending);
        }
    }
}

template <typename V, bool readonly>
const CSArray<V>* CSSortedSet<V, readonly>::allObjects(bool ascending) const {
    if (_count == 0) return NULL;
    CSArray<V>* objects = CSArray<V>::arrayWithCapacity(_count);
    allObjects(_root, objects, ascending);
    return objects;
}

template <typename V, bool readonly>
CSArray<typename CSSortedSet<V, readonly>::ConstTemplateType, 1, false>* CSSortedSet<V, readonly>::allObjectsMutable(bool ascending) const {
    if (_count == 0) return NULL;
    CSArray<ConstTemplateType, 1, false>* objects = CSArray<ConstTemplateType, 1, false>::arrayWithCapacity(_count);
    allObjects(_root, objects, ascending);
    return objects;
}

#endif
