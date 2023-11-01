#ifndef __CDK__CSHandler__
#define __CDK__CSHandler__

#include "CSArray.h"

template <typename ... Params>
class CSHandler {
public:
    typedef std::function<void(Params...)> Func;
    struct Callback {
        Func func;
        const void* obj;
        int ident;

        Callback(const Func& func, const void* obj, int ident) : func(func), obj(obj), ident(ident) {}
    };
private:
    CSArray<Callback>* _callbacks;
    int _ident;
public:
	CSHandler() : _callbacks(NULL), _ident(0) {}

	~CSHandler() {
        if (_callbacks) _callbacks->release();
    }

    CSHandler(const CSHandler&) = delete;
    CSHandler& operator=(const CSHandler&) = delete;
private:
    void add(const Func& func, const void* obj) {
        if (!_callbacks) _callbacks = new CSArray<Callback>(4);
        new (&_callbacks->addObject()) Callback(func, obj, ++_ident);
    }
public:
	inline int add(const Func& func) {
        add(func, NULL);
        return _ident;
    }

    template <class Object>
	inline int add(Object* obj, void(Object::*method)(Params...)) {
        add([obj, method](Params... params) { (obj->*method)(params...); }, obj);
        return _ident;
    }

    void remove(const void* obj) {
        if (_callbacks) {
            int i = 0;
            while (i < _callbacks->count()) {
                if (_callbacks->objectAtIndex(i).obj == obj) _callbacks->removeObjectAtIndex(i);
                else i++;
            }
        }
    }

    void remove(int ident) {
        if (_callbacks) {
            int i = 0;
            while (i < _callbacks->count()) {
                if (_callbacks->objectAtIndex(i).ident == ident) _callbacks->removeObjectAtIndex(i);
                else i++;
            }
        }
    }

    inline void clear() {
        if (_callbacks) _callbacks->removeAllObjects();
    }

    void operator()(Params... params) {
        if (_callbacks) {
			if (_callbacks->count() == 1) {
				_callbacks->objectAtIndex(0).func(params...);
			}
			else if (_callbacks->count() > 1) {
				CSArray<Func>* callbacks = new CSArray<Func>(_callbacks->count());
                foreach (Callback&, callback, _callbacks) callbacks->addObject(callback.func);
				foreach (const Func&, callback, callbacks) callback(params...);
                callbacks->release();
			}
        }
    }

    inline bool exists() const {
        return _callbacks && _callbacks->count() != 0;
    }

    inline const CSArray<Callback>* callbacks() const {
        return _callbacks;
    }
};

#endif
