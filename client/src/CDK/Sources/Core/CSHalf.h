#ifndef __CDK__CSHalf__
#define __CDK__CSHalf__

#include "CSTypes.h"

class half {
public:
	ushort raw;

	enum HalfRaw { Raw };
	inline half(HalfRaw, int raw) : raw(raw) {}
	
	inline half() {}
	inline half(float v) {
		setFloat(v);
	}
	inline half& operator=(float v) {
		setFloat(v);
		return *this;
	}
	inline operator float() const {
		return getFloat();
	}
	inline uint hash() const {
		return raw;
	}
	inline bool operator ==(half other) const {
		return raw == other.raw;
	}
	inline bool operator !=(half other) const {
		return raw != other.raw;
	}
	inline operator bool() const {
		return raw != 0;
	}
	inline bool operator !() const {
		return raw == 0;
	}
private:
	void setFloat(float v);
	float getFloat() const;
};

#endif