#ifndef __CDK__CSObject__
#define __CDK__CSObject__

#include "CSTypes.h"
#include "CSMemory.h"
#include "CSLock.h"
#include "CSLog.h"

#include <typeinfo>
#include <atomic>

class string;

class CSObject {
private:
    mutable std::atomic<int> _retainCount;
    mutable CSLock* _lock;
public:
    CSObject();
    virtual ~CSObject();

    CSObject(const CSObject&) = delete;
    CSObject& operator=(const CSObject&) = delete;

    void release() const;
    void retain() const;
    void autorelease() const;
    int retainCount() const;
    
    static void* operator new(size_t size);
    
    template <class T>
    static inline T* autorelease(T* t) {
        if (t) t->autorelease();
        return t;
    }
    template <class T>
    static inline T* retain(T* t) {
        if (t) t->retain();
        return t;
    }
    template <class T, class OtherT>
    static inline bool retain(T*& src, OtherT* dest) {
        if (src != dest) {
            release(src);
            src = retain(dest);
            return true;
        }
        return false;
    }
    template <class T>
    static inline void release(T*& t) {
        if (t) {
            t->release();
            t = NULL;
        }
    }
    
    inline virtual uint hash() const {
        return std::hash<const void*>()(this);
    }
    inline virtual bool isEqual(const CSObject* other) const {
        return this == other;
    }
    inline virtual int compareTo(const CSObject* other) const {
        return (int)((uint64)this - (uint64)&other);
    }

    virtual string toString() const;
    
private:
    void useLock() const;
public:
    inline bool assertOnLocked(bool abort = true) const {
#ifdef CS_ASSERT_DEBUG
        CSAssert(!abort || _lock, "should be locked");
        return _lock && _lock->assertOnActive(abort);
#else
        return true;
#endif
    }
    inline bool assertOnUnlocked(bool abort = true) const {
#ifdef CS_ASSERT_DEBUG
        return !_lock || _lock->assertOnDeactive(abort);
#else
        return true;
#endif
    }
    inline bool trylock(int op = 0) const {
        useLock();
        return _lock->trylock(op);
    }
    inline void lock(int op = 0) const {
        useLock();
        _lock->lock(op);
    }
    inline void wait() const {
        CSAssert(_lock, "should be locked");
        _lock->wait();
    }
    inline void wait(float timeout) const {
        CSAssert(_lock, "should be locked");
        _lock->wait(timeout);
    }
    inline void pulse() const {
        CSAssert(_lock, "should be locked");
        _lock->pulse();
    }
    inline void pulseAll() const {
        CSAssert(_lock, "should be locked");
        _lock->pulseAll();
    }
    inline void unlock() const {
        CSAssert(_lock, "should be locked");
        _lock->unlock();
    }

    friend class CSSynchronizedBlock;
};

template <class T>
class CSPtr {
private:
    T* _value;
public:
    inline CSPtr() : _value(NULL) {}
    inline CSPtr(T* value) : _value(CSObject::retain(value)) {}
    inline CSPtr(const CSPtr& other) : _value(CSObject::retain(other._value)) {}
    inline ~CSPtr() {
        CSObject::release(_value);
    }
    inline operator T*() const {
        return _value;
    }
    inline T* value() const {
        return _value;
    }
    inline T* operator ->() const {
        return _value;
    }
    inline CSPtr& operator =(T* value) {
        CSObject::retain(_value, value);
        return *this;
    }
    inline CSPtr& operator =(const CSPtr& other) {
        CSObject::retain(_value, other._value);
        return *this;
    }
    inline uint hash() const {
        return _value ? _value->hash() : 0;
    }
    inline bool operator ==(T* value) const {
        return _value == value;
    }
    inline bool operator !=(T* value) const {
        return _value != value;
    }
    inline operator bool() const {
        return _value != NULL;
    }
    inline bool operator !() const {
        return _value == NULL;
    }
};

class CSSynchronizedBlock {
private:
    CSLock* _obj;
    bool _flag;
public:
    CSSynchronizedBlock(const CSObject* obj, int op = 0, bool commit = true) : _flag(true) {
        if (commit) {
            obj->lock(op);
            _obj = obj->_lock;
        }
        else _obj = NULL;
    }
    CSSynchronizedBlock(CSLock& obj, int op = 0, bool commit = true) : _flag(true) {
        if (commit) {
            obj.lock(op);
            _obj = &obj;
        }
        else _obj = NULL;
    }
    inline ~CSSynchronizedBlock() {
        if (_obj) _obj->unlock();
    }
    inline operator bool() const {
        return _flag;
    }
	inline bool operator !() const {
		return !_flag;
	}
    inline void next() {
        _flag = false;
    }
};
#define synchronized(...) for (CSSynchronizedBlock __sb_ ## __LINE__(__VA_ARGS__); __sb_ ## __LINE__; __sb_ ## __LINE__.next())

#endif
