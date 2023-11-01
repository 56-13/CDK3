#ifndef __CDK__CSEntry__
#define __CDK__CSEntry__

#include "CSObject.h"
#include "CSMacro.h"

#include <type_traits>
#include <functional>

class CSBuffer;

template <typename V, bool readonly, bool retaining = derived<CSObject, V>::value>
struct CSEntryType {

};

template <typename V>
class CSEntryType<V, true, false> {
public:
    typedef V Type;
    typedef V ConstType;
    typedef const V& ValueParamType;
    typedef const V& ConstValueParamType;
    typedef const V* PointerParamType;
    typedef const V* ConstPointerParamType;
    typedef V& ValueReturnType;
    typedef const V& ConstValueReturnType;
    typedef V* PointerReturnType;
    typedef const V* ConstPointerReturnType;
    typedef V& ParamReturnType;
    typedef V& ConstParamReturnType;
    typedef V ConstTemplateType;
};

template <typename V>
class CSEntryType<V, false, false> {
public:
    typedef V Type;
    typedef V ConstType;
    typedef const V& ValueParamType;
    typedef const V& ConstValueParamType;
    typedef const V* PointerParamType;
    typedef const V* ConstPointerParamType;
    typedef V& ValueReturnType;
    typedef V& ConstValueReturnType;
    typedef V* PointerReturnType;
    typedef V* ConstPointerReturnType;
    typedef V& ParamReturnType;
    typedef V& ConstParamReturnType;
    typedef V ConstTemplateType;
};

template <typename V>
class CSEntryType<V, true, true> {
public:
    typedef V* Type;
    typedef const V* ConstType;
    typedef V* ValueParamType;
    typedef const V* ConstValueParamType;
    typedef V* const* PointerParamType;
    typedef const V* const* ConstPointerParamType;
    typedef V* ValueReturnType;
    typedef const V* ConstValueReturnType;
    typedef V* const* PointerReturnType;
    typedef const V* const* ConstPointerReturnType;
    typedef V*& ParamReturnType;
    typedef const V*& ConstParamReturnType;
    typedef const V ConstTemplateType;
};

template <typename V>
class CSEntryType<V, false, true> {
public:
    typedef V* Type;
    typedef const V* ConstType;
    typedef V* ValueParamType;
    typedef const V* ConstValueParamType;
    typedef V* const* PointerParamType;
    typedef const V* const* ConstPointerParamType;
    typedef V* ValueReturnType;
    typedef V* ConstValueReturnType;
    typedef V* const* PointerReturnType;
    typedef V* const* ConstPointerReturnType;
    typedef V*& ParamReturnType;
    typedef V*& ConstParamReturnType;
    typedef V ConstTemplateType;
};

template <typename V>
struct has_hash {
private:
    template <typename T>
    static constexpr auto check(const T*) -> typename
        std::is_same<decltype(std::declval<T>().hash()), uint>::type;

    template<typename>
    static constexpr std::false_type check(...);
public:
    static constexpr bool value = decltype(check<V>(0))::value;
};

template <typename V, typename R, typename = R>
struct equality_impl : std::false_type {};
template <typename V, typename R>
struct equality_impl<V, R, decltype(std::declval<V>() == std::declval<V>())> : std::true_type {};
template <typename V, typename R = bool>
struct equality : equality_impl<V, R> {};

template <typename V, typename R, typename = R>
struct smaller_impl : std::false_type {};
template <typename V, typename R>
struct smaller_impl<V, R, decltype(std::declval<V>() < std::declval<V>())> : std::true_type {};
template <typename V, typename R = bool>
struct smaller : smaller_impl<V, R> {};

template <typename V, typename R, typename = R>
struct greator_impl : std::false_type {};
template <typename V, typename R>
struct greator_impl<V, R, decltype(std::declval<V>() > std::declval<V>())> : std::true_type {};
template <typename V, typename R = bool>
struct greator : greator_impl<V, R> {};

template <typename T, typename = std::void_t<>>
struct is_std_hashable : std::false_type {};

template <typename T>
struct is_std_hashable<T, std::void_t<decltype(std::declval<std::hash<T>>()(std::declval<T>()))>> : std::true_type {};

template <typename V>
static inline typename enable_if<has_hash<V>::value, uint>::type CSEntryImpl_hash(const V& a) {
    return a.hash();
}

template <typename V>
static inline typename enable_if<!has_hash<V>::value && is_std_hashable<V>::value, uint>::type CSEntryImpl_hash(const V& a) {
    return std::hash<V>()(a);
}

template <typename V>
static inline typename enable_if<!has_hash<V>::value && !is_std_hashable<V>::value, uint>::type CSEntryImpl_hash(const V& a) {
    static_assert(false, "hash() member function or std::hash<> not exists");
}

template <typename V>
static inline typename enable_if<equality<V>::value, bool>::type CSEntryImpl_isEqual(const V& a, const V& b) {
    return a == b;
}

template <typename V>
static inline typename enable_if<!equality<V>::value, bool>::type CSEntryImpl_isEqual(const V& a, const V& b) {
    static_assert(false, "equality operator not exists");
}

template <typename V>
static inline typename enable_if<smaller<V>::value && greator<V>::value, int>::type CSEntryImpl_compareTo(const V& a, const V& b) {
    if (a < b) return -1;
    else if (a > b) return 1;
    else return 0;
}

template <typename V>
static inline typename enable_if<!smaller<V>::value || !greator<V>::value, int>::type CSEntryImpl_compareTo(const V& a, const V& b) {
    static_assert(false, "comparison operator not exists");
}

template <typename V, bool retaining = derived<CSObject, V>::value>
class CSEntryImpl {
public:
    static inline uint hash(const V& a) {
        return CSEntryImpl_hash(a);
    }
    static inline bool isEqual(const V& a, const V& b) {
        return CSEntryImpl_isEqual(a, b);
    }
    static inline int compareTo(const V& a, const V& b) {
        return CSEntryImpl_compareTo(a, b);
    }
    static inline void retain(V& dest, const V& src) {
        dest = src;
    }
    static inline const V& retain(const V& v) {
        return v;
    }
    static inline void retain(V* a, int count) {
    
    }
    static inline void release(V& a) {
        a.~V();
    }
    static inline void release(V* a, int count) {
        if (!std::is_trivially_destructible<V>::value) {
            for (int i = 0; i < count; i++) a[i].~V();
        }
    }
    static inline void finalize(V& a) {
        
    }
    static inline V& nullValue() {
        CSErrorLog("invalid call");
        abort();
    }
    static inline V& notNullValue(V& a) {
        return a;
    }
};

template <typename V>
class CSEntryImpl<V, true> {
public:
    static inline uint hash(const V* a) {
        return a ? a->hash() : 0;
    }
    static inline bool isEqual(const V* a, const V* b) {
        return a ? b && a->isEqual(b) : b == NULL;
    }
    static inline int compareTo(const V* a, const V* b) {
        return a && b ? a->compareTo(b) : 0;
    }
    template <typename PV>
    static inline void retain(PV*& dest, PV* src) {
        CSObject::retain(dest, src);
    }
    template <typename PV>
    static inline PV* retain(PV* a) {
        return CSObject::retain(a);
    }
    template <typename PV>
    static inline void retain(PV** a, int count) {
        for (int i = 0; i < count; i++) CSObject::retain(a[i]);
    }
    template <typename PV>
    static inline void release(PV*& a) {
        CSObject::release(a);
    }
    template <typename PV>
    static inline void release(PV** a, int count) {
        for (int i = 0; i < count; i++) CSObject::release(a[i]);
    }
    template <typename PV>
    static inline void finalize(PV*& a) {
        CSObject::release(a);
    }
    static inline V* nullValue() {
        return NULL;
    }
    static inline V*& notNullValue(V*& a) {
        CSErrorLog("invalid call");
        abort();
    }
};


struct CSHash {
private:
    uint _hash;
public:
    inline CSHash() : _hash(5381) {}

    inline operator uint() const {
        return _hash;
    }
    template <typename V>
    static inline typename enable_if<!derived<CSObject, V>::value, uint>::type value(const V& a) {
        return CSEntryImpl<V>::hash(a);
    }
    template <typename V>
    static inline typename enable_if<derived<CSObject, V>::value, uint>::type value(const V* a) {
        return CSEntryImpl<V>::hash(a);
    }
    template <typename V>
    inline typename enable_if<!derived<CSObject, V>::value, void>::type combine(const V& a) {
        _hash = ((_hash << 5) + _hash) ^ value(a);
    }
    template <typename V>
    inline typename enable_if<derived<CSObject, V>::value, void>::type combine(const V* a) {
        if (a) _hash = ((_hash << 5) + _hash) ^ value(a);
    }
    static int primeNumberCapacity(int value);
};

template <typename V, bool retaining = derived<CSObject, V>::value>
class CSEntryBufferType {
public:
    typedef std::function<void(CSBuffer*, V&)> ReadFunc;
    typedef std::function<void(CSBuffer*, const V&)> WriteFunc;
};

template <typename V>
class CSEntryBufferType<V, true> {
public:
    typedef std::function<V* (CSBuffer*)> ReadFunc;
    typedef std::function<void(CSBuffer*, const V*)> WriteFunc;
};

template <typename Type, typename Array>
class CSForeachBlock {
private:
    Array _arr;
    int _i;
    bool _flag;
public:
    inline CSForeachBlock(Array arr) : _arr(arr), _i(0), _flag(true) {

    }
    
    inline void next1() {
        ++_i;
    }
    
    inline bool check1() {
        return _arr && _i < _arr->count() && _flag;
    }
    
    inline void next2() {
        _flag = true;
    }

    inline bool check2() {
        return !_flag;
    }
    
    inline Type object() {
        _flag = false;
        
        return static_cast<Type>(_arr->objectAtIndex(_i));
    }
};

#define foreach(type, value, array)	\
for (CSForeachBlock<type, decltype(array)> __fb_ ## value(array); __fb_ ## value.check1(); __fb_ ## value.next1()) for (type value = __fb_ ## value.object(); __fb_ ## value.check2(); __fb_ ## value.next2())

#ifdef CS_ASSERT_DEBUG
class CSFenceBlock {
private:
    std::atomic<int>& _fence;
public:
    inline CSFenceBlock(std::atomic<int>& fence) : _fence(fence) {
        CSAssert(_fence.fetch_add(1, std::memory_order_acq_rel) == 0, "fence error");
    }
    inline ~CSFenceBlock() {
        _fence.fetch_sub(1, std::memory_order_relaxed);
    }
};

#define fence_block(value)  CSFenceBlock __fb_ ## __LINE__(value)

#else

#define fence_block(value);

#endif

#endif
