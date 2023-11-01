#ifndef __CDK__CSCharSequence__
#define __CDK__CSCharSequence__

#include "CSObject.h"

#include <unicode/ubrk.h>

class CSCharSequence {
private:
	int* _seq;
	int _count;
public:
	CSCharSequence(UBreakIteratorType type, const char* str);
	CSCharSequence(UBreakIteratorType type, const uchar* str);
	~CSCharSequence();

	CSCharSequence(const CSCharSequence&) = delete;
	CSCharSequence& operator=(const CSCharSequence&) = delete;

	int resourceCost() const;

	inline int count() const {
		return _count;
	}
	inline int from(int i) const {
		CSAssert(i >= 0 && i <= _count, "out of range");
		return i ? _seq[i - 1] : 0;
	}
	inline int to(int i) const {
		CSAssert(i >= 0 && i < _count, "out of range");
		return _seq[i];
	}
	inline int length(int i) const {
		return to(i) - from(i);
	}
private:
	void init(UBreakIterator* iterator);
};

#endif
