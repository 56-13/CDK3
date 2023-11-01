#define CDK_IMPL

#include "CSGraphicsContext.h"

#include "CSOpenGLContext.h"

CSGraphicsContext* CSGraphicsContext::_instance = NULL;

void CSGraphicsContext::initialize(CSGraphicsPlatform platform) {
	if (!_instance) {
		if (platform == CSGraphicsPlatformOpenGL) _instance = new CSOpenGLContext();
		else {
			CSErrorLog("not supported platform:%d", platform);
			abort();
		}
	}
}

void CSGraphicsContext::finalize() {
	if (_instance) {
		delete _instance;
		_instance = NULL;
	}
}

CSGraphics* CSGraphicsContext::currentGraphics() {
	pthread_t pid = pthread_self();
	synchronized(&_graphics) {
		foreach(const FocusedGraphics&, e, &_graphics) {
			if (pthread_equal(e.pid, pid)) return e.graphics;
		}
	}
	return NULL;
}

void CSGraphicsContext::attachGraphics(CSGraphics* graphics) {
	pthread_t pid = pthread_self();
	synchronized(&_graphics) {
		foreach(FocusedGraphics&, e, &_graphics) {
			if (pthread_equal(e.pid, pid)) {
				e.graphics = graphics;
				return;
			}
		}
	}
	{
		FocusedGraphics& e = _graphics.addObject();
		e.pid = pid;
		e.graphics = graphics;
	}
}

void CSGraphicsContext::detachGraphics(CSGraphics* graphics) {
	pthread_t pid = pthread_self();
	synchronized(&_graphics) {
		for (int i = 0; i < _graphics.count(); i++) {
			if (pthread_equal(_graphics.objectAtIndex(i).pid, pid)) {
				_graphics.removeObjectAtIndex(i);
				return;
			}
		}
	}
}

void CSGraphicsContext::removeGraphics(CSGraphics* graphics) {
	synchronized(&_graphics) {
		int i = 0;
		while (i < _graphics.count()) {
			if (_graphics.objectAtIndex(i).graphics == graphics) _graphics.removeObjectAtIndex(i);
			else i++;
		}
	}
}
