#ifndef __CDK__CSSortedDictionary__
#define __CDK__CSSortedDictionary__

#include "CSArray.h"

template <typename K, typename V, bool readonly = true>
class CSSortedDictionary : public CSObject {
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
    private:
        CSSortedDictionary* _dictionary;
        int* _cursor;
        bool _ascending;
        
        ReadonlyIterator(CSSortedDictionary* dictionary, bool ascending);
    public:
        ~ReadonlyIterator();
        
        bool remaining() const;
        KeyValueReturnType key() const;
        ObjectConstValueReturnType object() const;
        void next();
        friend class CSSortedDictionary;
    };
    class Iterator : public ReadonlyIterator {
    private:
        Iterator(CSSortedDictionary* dictionary, bool ascending);
    public:
        ObjectValueReturnType object();
        friend class CSSortedDictionary;
    };

    friend class ReadonlyIterator;
    friend class Iterator;

private:
    struct Node {
        KeyType key;
        ObjectType object;
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
    CSSortedDictionary();
    explicit CSSortedDictionary(int capacity);
    CSSortedDictionary(KeyPointerParamType keys, ObjectPointerParamType objects, int count);
    template <typename otherK, typename otherV, bool otherReadonly>
    CSSortedDictionary(const CSSortedDictionary<otherK, otherV, otherReadonly>* otherDictionary);
    virtual ~CSSortedDictionary();
public:
    static inline CSSortedDictionary* dictionary() {
        return autorelease(new CSSortedDictionary());
    }
    static inline CSSortedDictionary* dictionaryWithCapacity(int capacity) {
        return autorelease(new CSSortedDictionary(capacity));
    }
    static inline CSSortedDictionary* dictionaryWithObjects(KeyPointerParamType keys, ObjectPointerParamType objects, int count) {
        return autorelease(new CSSortedDictionary(keys, objects, count));
    }
    template <typename otherK, typename otherV, bool otherReadonly>
    static inline CSSortedDictionary* dictionaryWithDictionary(const CSSortedDictionary<otherK, otherV, otherReadonly>* otherDictionary) {
        return autorelease(new CSSortedDictionary(otherDictionary));
    }
private:
    int expandReady();
    int expandCommit(int p);
    ObjectType* insert(KeyValueParamType, int prev, int next, int& p);
    void insert(KeyValueParamType, ObjectValueParamType obj, int prev, int next, int& p);
    ObjectType* find(KeyValueParamType key, int p) const;
    bool remove(KeyValueParamType key, int& p);
    void remove(int& p);
    int height(int p);
    int srl(int p1);
    int srr(int p1);
    int drl(int p1);
    int drr(int p1);
    int rrr(int prev, int& p);
    template <typename otherK, bool otherReadonly>
    void allKeys(int p, CSArray<otherK, 1, otherReadonly>* keys, bool ascending) const;
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

    ObjectValueReturnType setObject(KeyValueParamType key);
    void setObject(KeyValueParamType key, ObjectValueParamType obj);
    bool removeObject(KeyValueParamType key);
    void removeAllObjects();
    inline bool containsKey(KeyValueParamType key) const {
        return find(key, _root) != NULL;
    }
    inline ObjectValueReturnType objectForKey(KeyValueParamType key) {
        ObjectType* rtn = find(key, _root);
        return rtn ? *rtn : CSEntryImpl<V>::nullValue();
    }
    inline ObjectConstValueReturnType objectForKey(KeyValueParamType key) const {
        ObjectType* rtn = find(key, _root);
        return rtn ? *rtn : CSEntryImpl<V>::nullValue();
    }
    inline ObjectType* tryGetObjectForKey(KeyValueParamType key) {
        return find(key, _root);
    }
    inline const ObjectType* tryGetObjectForKey(KeyValueParamType key) const {
        return find(key, _root);
    }
    inline bool tryGetObjectForKey(KeyValueParamType key, ObjectParamReturnType result) {
        ObjectType* v = find(key, _root);
        if (!v) return false;
        result = *v;
        return true;
    }
    inline bool tryGetObjectForKey(KeyValueParamType key, ObjectConstParamReturnType result) const {
        return const_cast<CSSortedDictionary*>(this)->tryGetObjectForKey(key, const_cast<ObjectParamReturnType>(result));
    }

    inline ReadonlyIterator iterator(bool ascending = true) const {
        return ReadonlyIterator(const_cast<CSSortedDictionary*>(this), ascending);
    }
    inline Iterator iterator(bool ascending = true) {
        return Iterator(const_cast<CSSortedDictionary*>(this), ascending);
    }
    
    const CSArray<K, 1, readonly>* allKeys(bool ascending = true) const;
    CSArray<KeyConstTemplateType, 1, false>* allKeysMutable(bool ascending = true) const;
    CSArray<V, 1, readonly>* allObjects(bool ascending = true);
    inline const CSArray<V, 1, readonly>* allObjects(bool ascending = true) const {
        return const_cast<CSSortedDictionary*>(this)->allObjects(ascending);
    }
    CSArray<ObjectConstTemplateType, 1, false>* allObjectsMutable(bool ascending = true) const;
    
    inline CSSortedDictionary<K, V, false>* asReadWrite() {
        return reinterpret_cast<CSSortedDictionary<K, V, false>*>(this);
    }
    inline CSSortedDictionary<K, V, true>* asReadOnly() {
        return reinterpret_cast<CSSortedDictionary<K, V, true>*>(this);
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

    template <class friendK, class friendV, bool friendReadonly>
    friend class CSSortedDictionary;
};

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::CSSortedDictionary() {
    setCapacity(12);
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::CSSortedDictionary(int capacity) {
    setCapacity(capacity);
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::CSSortedDictionary(KeyPointerParamType keys, ObjectPointerParamType objects, int count) {
    setCapacity(count);
    
    for (int i = 0; i < count; i++) setObject(keys[i], objects[i]);
}

template <typename K, typename V, bool readonly> template <typename otherK, typename otherV, bool otherReadonly>
CSSortedDictionary<K, V, readonly>::CSSortedDictionary(const CSSortedDictionary<otherK, otherV, otherReadonly>* otherDictionary) {
    setCapacity(otherDictionary->_capacity);
    
    for (int i = 0; i < otherDictionary->_expansion; i++) {
        if (otherDictionary->_nodes[i].exists) {
            setObject(otherDictionary->_nodes[i].key, otherDictionary->_nodes[i].object);
        }
    }
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::~CSSortedDictionary() {
    fence_block(_fence);
    
    if (_capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (_nodes[i].exists) {
                CSEntryImpl<K>::release(_nodes[i].key);
                CSEntryImpl<V>::release(_nodes[i].object);
            }
        }
        free(_nodes);
    }
}

template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::expandReady() {
    if (_expansion == _capacity) {
        for (int i = 0; i < _expansion; i++) {
            if (!_nodes[i].exists) return i;
        }
        int nextCapacity = CSMath::max(_capacity << 1, 1);
        setCapacity(nextCapacity);
    }
    return -1;
}

template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::expandCommit(int p) {
    return p == -1 ? _expansion++ : p;
}

template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::setCapacity(int capacity) {
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

template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::ObjectType* CSSortedDictionary<K, V, readonly>::insert(KeyValueParamType key, int prev, int next, int& p) {
    ObjectType* rtn;
    
    if (p == -1) {
        p = expandCommit(next);
        
        Node& node = _nodes[p];
        
        new (&node.key) KeyType(CSEntryImpl<K>::retain(key));
        node.left = -1;
        node.right = -1;
        node.prev = prev;
        node.height = 0;
        node.exists = true;
        _count++;
        
        rtn = &node.object;
    }
    else {
        Node& node = _nodes[p];
        
        int comp = CSEntryImpl<K>::compareTo(key, node.key);
        
        if (comp == 0) {
            rtn = &node.object;
        }
        else if (comp < 0) {
            rtn = insert(key, p, next, node.left);
            
            if ((height(node.left) - height(node.right)) == 2) {
                if (CSEntryImpl<K>::compareTo(key, _nodes[node.left].key) < 0) {
                    p = srl(p);
                }
                else {
                    p = drl(p);
                }
            }
        }
        else {
            rtn = insert(key, p, next, node.right);
            
            if ((height(node.right) - height(node.left)) == 2) {
                if (CSEntryImpl<K>::compareTo(key, _nodes[node.right].key) > 0) {
                    p = srr(p);
                }
                else {
                    p = drr(p);
                }
            }
        }
        _nodes[p].height = CSMath::max(height(_nodes[p].left), height(_nodes[p].right)) + 1;
    }
    return rtn;
}

template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::insert(KeyValueParamType key, ObjectValueParamType obj, int prev, int next, int& p) {
    if (p == -1) {
        p = expandCommit(next);
        
        Node& node = _nodes[p];
        
        new (&node.key) KeyType(CSEntryImpl<K>::retain(key));
		new (&node.object) ObjectType(CSEntryImpl<V>::retain(obj));
        node.left = -1;
        node.right = -1;
        node.prev = prev;
        node.height = 0;
        node.exists = true;
        _count++;
    }
    else {
        Node& node = _nodes[p];
        
        int comp = CSEntryImpl<K>::compareTo(key, node.key);
        
        if (comp == 0) {
            if (CSEntryImpl<K>::isEqual(key, node.key)) {
                CSEntryImpl<V>::release(node.object);
                new (&node.object) ObjectType(CSEntryImpl<V>::retain(obj));
            }
        }
        else if (comp < 0) {
            insert(key, obj, p, next, node.left);
            
            if ((height(node.left) - height(node.right)) == 2) {
                if (CSEntryImpl<K>::compareTo(key, _nodes[node.left].key) < 0) {
                    p = srl(p);
                }
                else {
                    p = drl(p);
                }
            }
        }
        else {
            insert(key, obj, p, next, node.right);
            
            if ((height(node.right) - height(node.left)) == 2) {
                if (CSEntryImpl<K>::compareTo(key, _nodes[node.right].key) > 0) {
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

template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::ObjectType* CSSortedDictionary<K, V, readonly>::find(KeyValueParamType key, int p) const {
    if (p == -1) {
        return NULL;
    }
    
    Node& node = _nodes[p];
    
    int comp = CSEntryImpl<K>::compareTo(key, node.key);
    
    if (comp < 0) {
        return find(key, node.left);
    }
    else if (comp > 0) {
        return find(key, node.right);
    }
    else {
        return CSEntryImpl<K>::isEqual(key, node.key) ? &node.object : NULL;
    }
}

template <typename K, typename V, bool readonly>
bool CSSortedDictionary<K, V, readonly>::remove(KeyValueParamType key, int& p) {
    if (p != -1) {
        Node& node = _nodes[p];
        
        int comp = CSEntryImpl<K>::compareTo(key, node.key);
        
        if (comp < 0) {
            return remove(key, node.left);
        }
        else if (comp > 0) {
            return remove(key, node.right);
        }
        else if (CSEntryImpl<K>::isEqual(key, node.key)) {
            remove(p);
            return true;
        }
    }
    return false;
}

template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::remove(int& p) {
    Node& node = _nodes[p];
    
    CSEntryImpl<K>::release(node.key);
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
        int n = rrr(p, node.right);
        node.key = _nodes[n].key;
        node.object = _nodes[n].object;
    }
}

template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::height(int p) {
    return p == -1 ? -1 : _nodes[p].height;
}

template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::srl(int p1) {
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

template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::srr(int p1) {
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
template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::drl(int p1) {
    _nodes[p1].left = srr(_nodes[p1].left);
    return srl(p1);
}
template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::drr(int p1) {
    _nodes[p1].right = srl(_nodes[p1].right);
    return srr(p1);
}
template <typename K, typename V, bool readonly>
int CSSortedDictionary<K, V, readonly>::rrr(int prev, int& p) {
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
template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::ObjectValueReturnType  CSSortedDictionary<K, V, readonly>::setObject(KeyValueParamType key) {
    fence_block(_fence);
    
    return CSEntryImpl<V>::notNullValue(*insert(key, -1, expandReady(), _root));
}
template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::setObject(KeyValueParamType key, ObjectValueParamType obj) {
    fence_block(_fence);
    
    insert(key, obj, -1, expandReady(), _root);
}
template <typename K, typename V, bool readonly>
bool CSSortedDictionary<K, V, readonly>::removeObject(KeyValueParamType key) {
    fence_block(_fence);
    
    return remove(key, _root);
}
template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::removeAllObjects() {
    fence_block(_fence);
    
    for (int i = 0; i < _expansion; i++) {
        Node& node = _nodes[i];
        if (node.exists) {
            CSEntryImpl<K>::release(node.key);
            CSEntryImpl<V>::release(node.object);
            node.exists = false;
        }
    }
    _expansion = 0;
    _count = 0;
    _root = -1;
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::ReadonlyIterator::ReadonlyIterator(CSSortedDictionary* dictionary, bool ascending) : _dictionary(dictionary), _ascending(ascending) {
    _dictionary->fence();

    _cursor = &_dictionary->_root;
    
    if (*_cursor != -1) {
        if (ascending) {
            while (_dictionary->_nodes[*_cursor].left != -1) {
                _cursor = &_dictionary->_nodes[*_cursor].left;
            }
        }
        else {
            while (_dictionary->_nodes[*_cursor].right != -1) {
                _cursor = &_dictionary->_nodes[*_cursor].right;
            }
        }
    }
    if (*_cursor == -1) {
        _cursor = NULL;
    }
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::ReadonlyIterator::~ReadonlyIterator() {
    _dictionary->flict();
}

template <typename K, typename V, bool readonly>
bool CSSortedDictionary<K, V, readonly>::ReadonlyIterator::remaining() const {
    return _cursor != NULL;
}

template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::KeyValueReturnType CSSortedDictionary<K, V, readonly>::ReadonlyIterator::key() const {
    return _cursor ? _dictionary->_nodes[*_cursor].key : CSEntryImpl<K>::nullValue();
}

template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::ObjectConstValueReturnType CSSortedDictionary<K, V, readonly>::ReadonlyIterator::object() const {
    return _cursor ? _dictionary->_nodes[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename K, typename V, bool readonly>
void CSSortedDictionary<K, V, readonly>::ReadonlyIterator::next() {
    if (_cursor) {
        if (_ascending) {
            if (_dictionary->_nodes[*_cursor].right != -1) {
                _cursor = &_dictionary->_nodes[*_cursor].right;
                
                while (_dictionary->_nodes[*_cursor].left != -1) {
                    _cursor = &_dictionary->_nodes[*_cursor].left;
                }
            }
            else {
                for (; ; ) {
                    int& prev = _dictionary->_nodes[*_cursor].prev;

                    if (prev == -1) {
                        _cursor = NULL;
                        break;
                    }
                    else if (_dictionary->_nodes[prev].left == *_cursor) {
                        _cursor = &prev;
                        break;
                    }
                    else _cursor = &prev;
                }
            }
        }
        else {
            if (_dictionary->_nodes[*_cursor].left != -1) {
                _cursor = &_dictionary->_nodes[*_cursor].left;
                
                while (_dictionary->_nodes[*_cursor].right != -1) {
                    _cursor = &_dictionary->_nodes[*_cursor].right;
                }
            }
            else {
                for (; ; ) {
                    int& prev = _dictionary->_nodes[*_cursor].prev;
                    
                    if (prev == -1) {
                        _cursor = NULL;
                        break;
                    }
                    else if (_dictionary->_nodes[prev].right == *_cursor) {
                        _cursor = &prev;
                        break;
                    }
                    else _cursor = &prev;
                }
            }
        }
    }
}

template <typename K, typename V, bool readonly>
CSSortedDictionary<K, V, readonly>::Iterator::Iterator(CSSortedDictionary* dictionary, bool ascending) : ReadOnlyIterator(dictionary, ascending) {

}

template <typename K, typename V, bool readonly>
typename CSSortedDictionary<K, V, readonly>::ObjectValueReturnType CSSortedDictionary<K, V, readonly>::Iterator::object() {
    return _cursor ? _dictionary->_nodes[*_cursor].object : CSEntryImpl<V>::nullValue();
}

template <typename K, typename V, bool readonly>
template <typename otherK, bool otherReadonly>
void CSSortedDictionary<K, V, readonly>::allKeys(int p, CSArray<otherK, 1, otherReadonly>* keys, bool ascending) const {
    if (p != -1) {
        Node& node = _nodes[p];
        if (ascending) {
            allKeys(node.left, keys, ascending);
            keys->addObject(node.key);
            allKeys(node.right, keys, ascending);
        }
        else {
            allKeys(node.right, keys, ascending);
            keys->addObject(node.key);
            allKeys(node.left, keys, ascending);
        }
    }
}

template <typename K, typename V, bool readonly> 
template <typename otherV, bool otherReadonly>
void CSSortedDictionary<K, V, readonly>::allObjects(int p, CSArray<otherV, 1, otherReadonly>* objects, bool ascending) const {
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

template <typename K, typename V, bool readonly>
const CSArray<K, 1, readonly>* CSSortedDictionary<K, V, readonly>::allKeys(bool ascending) const {
    if (_count == 0) return NULL;
    CSArray<KeyConstTemplateType>* keys = CSArray<KeyConstTemplateType>::arrayWithCapacity(_count);
    allKeys(_root, keys, ascending);
    return keys;
}

template <typename K, typename V, bool readonly>
CSArray<typename CSSortedDictionary<K, V, readonly>::KeyConstTemplateType, 1, false>* CSSortedDictionary<K, V, readonly>::allKeysMutable(bool ascending) const {
    if (_count == 0) return NULL;
    CSArray<KeyConstTemplateType>* keys = CSArray<KeyConstTemplateType>::arrayWithCapacity(_count);
    allKeys(_root, keys, ascending);
    return keys;
}

template <typename K, typename V, bool readonly>
CSArray<V, 1, readonly>* CSSortedDictionary<K, V, readonly>::allObjects(bool ascending) {
    if (_count == 0) return NULL;
    CSArray<V, 1, readonly>* objects = CSArray<V, 1, readonly>::arrayWithCapacity(_count);
    allObjects(_root, objects, ascending);
    return objects;
}

template <typename K, typename V, bool readonly>
CSArray<typename CSSortedDictionary<K, V, readonly>::ObjectConstTemplateType, 1, false>* CSSortedDictionary<K, V, readonly>::allObjectsMutable(bool ascending) const {
    if (_count == 0) return NULL;
    CSArray<ObjectConstTemplateType>* objects = CSArray<ObjectConstTemplateType>::arrayWithCapacity(_count);
    allObjects(_root, objects, ascending);
    return objects;
}

#endif
