#define CDK_IMPL

#include "CSCharSequence.h"

//TODO:WORD MERGE 제거됨. 테스트필요. 2023-04-10

#ifndef CDK_IOS     //iOS는 ubrk 관련 라이브러리 사용불가

CSCharSequence::CSCharSequence(UBreakIteratorType type, const char* str) : _seq(NULL), _count(0) {
	UErrorCode err = U_ZERO_ERROR;
	UBreakIterator* iterator = NULL;
	UText* text = NULL;

	iterator = ubrk_open(type, NULL, NULL, 0, &err);
	if (err > U_ZERO_ERROR) goto exit;
	text = utext_openUTF8(NULL, str, -1, &err);
	if (err > U_ZERO_ERROR) goto exit;
	ubrk_setUText(iterator, text, &err);
	if (err > U_ZERO_ERROR) goto exit;

	init(iterator);
exit:
	if (text) utext_close(text);
	if (iterator) ubrk_close(iterator);
}

CSCharSequence::CSCharSequence(UBreakIteratorType type, const uchar* str) : _seq(NULL), _count(0) {
	UErrorCode err = U_ZERO_ERROR;
	UBreakIterator* iterator = NULL;

	iterator = ubrk_open(type, NULL, str, -1, &err);
	if (err > U_ZERO_ERROR) return;

	init(iterator);

	ubrk_close(iterator);
}

void CSCharSequence::init(UBreakIterator* iterator) {
	int cursor;

	ubrk_first(iterator);
	while ((cursor = ubrk_next(iterator)) != UBRK_DONE) _count++;
	if (!_count) return;

	_seq = (int*)fmalloc(_count * sizeof(int));

	ubrk_first(iterator);

	int i = 0;
	while ((cursor = ubrk_next(iterator)) != UBRK_DONE) _seq[i++] = cursor;
}

#endif

CSCharSequence::~CSCharSequence() {
	if (_seq) free(_seq);
}

int CSCharSequence::resourceCost() const {
	return sizeof(CSCharSequence) + _count * sizeof(int);
}
