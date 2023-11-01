#ifndef __CDK__CSTask__
#define __CDK__CSTask__

#include "CSArray.h"

#include <functional>

enum CSTaskState : byte {
    CSTaskStateNone,
    CSTaskStateActive,
    CSTaskStateDone
};

class CSTaskBase : public CSObject {
private:
    double _elapsed = 0;
    float _delay;
    CSTaskState _state = CSTaskStateNone;
    bool _repeat;
protected:
    inline CSTaskBase(float delay, bool repeat) : _delay(delay), _repeat(repeat) {}
    virtual ~CSTaskBase() = default;
public:
    inline void setDelay(float delay) {
        _delay = delay;
    }
    inline float delay() const {
        return _delay;
    }
    inline void setRepeat(bool repeat) {
        _repeat = repeat;
    }
    inline bool repeat() const {
        return _repeat;
    }
    inline CSTaskState state() const {
        return _state;
    }
	void stop();
#ifdef CDK_IMPL
    bool start();
	double execute();
#endif
    bool finish();
    bool finish(float timeout);

    static bool finishAll(CSTaskBase* const* tasks, int count);
    static bool finishAll(CSTaskBase* const* tasks, int count, float timeout);

    static inline bool finishAll(CSArray<CSTaskBase>* tasks) {
        return finishAll(tasks->pointer(), tasks->count());
    }
    static inline bool finishAll(CSArray<CSTaskBase*>* tasks) {
        return finishAll(tasks->pointer(), tasks->count());
    }
    static inline bool finishAll(CSArray<CSTaskBase>* tasks, float timeout) {
        return finishAll(tasks->pointer(), tasks->count(), timeout);
    }
    static inline bool finishAll(CSArray<CSTaskBase*>* tasks, float timeout) {
        return finishAll(tasks->pointer(), tasks->count(), timeout);
    }
protected:
    virtual void invoke() = 0;
};

template <typename Result>
class CSTask : public CSTaskBase {
private:
    std::function<Result()> _func;
    Result _result;
public:
    inline CSTask(const std::function<Result()>& func, float delay = 0, bool repeat = false) : CSTaskBase(delay, repeat), _func(func) {}
    template <class Object>
    CSTask(Object* obj, Result(Object::* method)(), float delay = 0, bool repeat = false) : CSTaskBase(delay, repeat), _func([obj, method]() -> Result { return (obj->*method)(); }) {}
private:
    inline ~CSTask() {
        CSEntryImpl<Result>::finalize(_result);
    }
public:
    static inline CSTask* task(const std::function<Result()>& func, float delay = 0, bool repeat = false) {
        return autorelease(new CSTask(func, delay, repeat));
    }
    template <class Object>
    static inline CSTask* task(Object* obj, Result(Object::* method)(), float delay = 0, bool repeat = false) {
        return autorelease(new CSTask(obj, method, delay, repeat));
    }
    inline Result result() {
        CSAssert(state() == CSTaskStateDone);
        return _result;
    }
protected:
    inline void invoke() override {
        CSEntryImpl<Result>::retain(_result, _func());
    }
};

template <>
class CSTask<void> : public CSTaskBase {
private:
    std::function<void()> _func;
public:
    inline CSTask(const std::function<void()>& func, float delay = 0, bool repeat = false) : CSTaskBase(delay, repeat), _func(func) {}
    template <class Object>
    CSTask(Object* obj, void(Object::* method)(), float delay = 0, bool repeat = false) : CSTaskBase(delay, repeat), _func([obj, method]() { (obj->*method)(); }) {}
private:
    ~CSTask() = default;
public:
    static inline CSTask* task(const std::function<void()>& func, float delay = 0, bool repeat = false) {
        return autorelease(new CSTask(func, delay, repeat));
    }
    template <class Object>
    static inline CSTask* task(Object* obj, void(Object::* method)(), float delay = 0, bool repeat = false) {
        return autorelease(new CSTask(obj, method, delay, repeat));
    }
protected:
    inline void invoke() override {
        _func();
    }
};

#endif
