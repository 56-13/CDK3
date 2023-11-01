#ifndef __CDK__CSAnimationColorLinear__
#define __CDK__CSAnimationColorLinear__

#include "CSAnimationColor.h"

class CSAnimationColorLinear : public CSAnimationColor {
public:
	CSColor startColor = CSColor::White;
	CSColor endColor = CSColor::White;
	bool smooth = false;

	CSAnimationColorLinear() = default;
	CSAnimationColorLinear(const CSColor& startColor, const CSColor& endColor, bool smooth);
	explicit CSAnimationColorLinear(CSBuffer* buffer);
	CSAnimationColorLinear(const CSAnimationColorLinear* other);

	static inline CSAnimationColorLinear* color() {
		return autorelease(new CSAnimationColorLinear());
	}
	static inline CSAnimationColorLinear* color(const CSColor& startColor, const CSColor& endColor, bool smooth) {
		return autorelease(new CSAnimationColorLinear(startColor, endColor, smooth));
	}

	inline Type type() const override {
		return TypeLinear;
	}
	inline int resourceCost() const override {
		return sizeof(CSAnimationColorLinear);
	}
	CSColor value(float t, const CSColor& r, const CSColor& default) const override;
};

#endif