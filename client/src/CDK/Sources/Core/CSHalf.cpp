#define CDK_IMPL

#include "CSHalf.h"

void half::setFloat(float v) {
	union {
		float f;
		uint i;
	} value = { v };

	uint i = value.i;
	
	register int s = (i >> 16) & 0x00008000;
	register int e = ((i >> 23) & 0x000000ff) - (127 - 15);
	register int f = i & 0x007fffff;

	if (e <= 0) {
		if (e < -10) {
			if (s) raw = 0x8000;
			else raw = 0;
		}
		f = (f | 0x00800000) >> (1 - e);
		raw = s | (f >> 13);
	}
	else if (e == 0xff - (127 - 15)) {
		if (f == 0) raw = s | 0x7c00;
		else {
			f >>= 13;
			raw = s | 0x7c00 | f | (f == 0);
		}
	}
	else {
		if (e > 30) raw = s | 0x7c00;
		raw = s | (e << 10) | (f >> 13);
	}
}

float half::getFloat() const {
	union {
		float f;
		uint i;
	} value;

	int s = (raw >> 15) & 0x00000001;
	int e = (raw >> 10) & 0x0000001f;
	int f = raw & 0x000003ff;

	if (e == 0) {
		if (f == 0) value.i = s << 31;
		else {
			while (!(f & 0x00000400)) {
				f <<= 1;
				e--;
			}
			e++;
			f &= ~0x00000400;
		}
	}
	else if (e == 31) {
		if (f == 0) value.i = (s << 31) | 0x7f800000;
		else value.i = (s << 31) | 0x7f800000 | (f << 13);
	}
	e = e + (127 - 15);
	f = f << 13;

	value.i = ((s << 31) | (e << 23) | f);

	return value.f;
}
