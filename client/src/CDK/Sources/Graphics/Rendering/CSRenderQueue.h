#ifndef __CDK__CSRenderQueue__
#define __CDK__CSRenderQueue__

#include "CSRenderFrame.h"

class CSRenderQueue : public CSObject {
private:
	CSRenderFrame* _frame;
	std::atomic<int> _remaining;
	int _commandCapacity = 0;
	int _readCapacity = 0;
	int _writeCapacity = 0;
	int _parallelCommandCapacity = 0;
	int _parallelLocalCommandCapacity = 0;
	bool _firstDone = false;
public:
	CSRenderQueue();
	~CSRenderQueue();

	static inline CSRenderQueue* queue() {
		return autorelease(new CSRenderQueue());
	}

	inline CSRenderFrame* frame() {
		return _frame;
	}
	inline const CSRenderFrame* frame() const {
		return _frame;
	}
	int remaining() const;

	void render();
};

#endif
