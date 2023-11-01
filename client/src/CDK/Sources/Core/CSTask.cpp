#define CDK_IMPL

#include "CSTask.h"

#include "CSTime.h"

#include "CSThread.h"

double CSTaskBase::execute() {
	if (_state == CSTaskStateNone) return 0;

	double currentTime = CSTime::currentTime();

	if (currentTime >= _elapsed) {
		invoke();

		if (_repeat) _elapsed = currentTime + _delay;
		else {
			synchronized(this) {
				_state = CSTaskStateDone;
				pulseAll();
			}
			return 0;
		}
	}

	return _elapsed;
}

void CSTaskBase::stop() {
	synchronized(this) {
		_state = CSTaskStateNone;
		pulseAll();
	}
}

bool CSTaskBase::start() {
	if (_state == CSTaskStateNone) {
		_state = CSTaskStateActive;
		_elapsed = _delay ? CSTime::currentTime() + _delay : 0.0;
        return true;
    }
    return false;
}

bool CSTaskBase::finish() {
	while (!_repeat && _state == CSTaskStateActive && CSThreadPool::execute());

	if (!_repeat && _state == CSTaskStateActive) {
		synchronized(this) {
			if (!_repeat && _state == CSTaskStateActive) wait();
		}
	}
	return _state == CSTaskStateDone;
}

bool CSTaskBase::finish(float timeout) {
	double prevTime = CSTime::currentTime();
	while (!_repeat && _state == CSTaskStateActive) {
		double currentTime = CSTime::currentTime();
		if (CSThreadPool::execute()) {
			if (currentTime - prevTime >= timeout) goto exit;
		}
		else {
			timeout -= currentTime - prevTime;
			break;
		}
	}
	if (!_repeat && _state == CSTaskStateActive) {
		synchronized(this) {
			if (!_repeat && _state == CSTaskStateActive) wait(timeout);
		}
	}
exit:
	return _state == CSTaskStateDone;
}

bool CSTaskBase::finishAll(CSTaskBase* const* tasks, int count) {
	for (int i = 0; i < count; i++) {
		if (!tasks[i]->finish()) return false;
	}
	return true;
}

bool CSTaskBase::finishAll(CSTaskBase* const* tasks, int count, float timeout) {
	double elapsed = CSTime::currentTime();

	for (int i = 0; i < count; i++) {
		if (!tasks[i]->finish(timeout)) return false;

		double nextElapsed = CSTime::currentTime();
		timeout -= nextElapsed - elapsed;
		elapsed = nextElapsed;
	}
	return true;
}
