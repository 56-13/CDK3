#ifndef __CDK__CSShadow__
#define __CDK__CSShadow__

#include "CSTypes.h"

class CSBuffer;

struct CSShadow {
	bool pixel32;
	ushort resolution;
	float blur;
	float bias;
	float bleeding;

	CSShadow();
	CSShadow(bool pixel32, int resolution, float blur, float bias, float bleeding);
	explicit CSShadow(CSBuffer* buffer);
	explicit CSShadow(const byte*& bytes);

	uint hash() const;

	bool operator ==(const CSShadow& other) const;
	inline bool operator !=(const CSShadow& other) const {
		return !(*this == other);
	}
};

#endif