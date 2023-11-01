#ifndef __CDK__CSAnimationColorConstant__
#define __CDK__CSAnimationColorConstant__

#include "CSAnimationColor.h"

class CSAnimationColorConstant : public CSAnimationColor {
public:
	CSColor origin = CSColor::White;

	CSAnimationColorConstant() = default;
	CSAnimationColorConstant(const CSColor& color);
	explicit CSAnimationColorConstant(CSBuffer* buffer);
	CSAnimationColorConstant(const CSAnimationColorConstant* other);

	static inline CSAnimationColorConstant* color() {
		return autorelease(new CSAnimationColorConstant());
	}
	static inline CSAnimationColorConstant* color(const CSColor& color) {
		return autorelease(new CSAnimationColorConstant(color));
	}

	inline Type type() const override {
		return TypeConstant;
	}
	inline int resourceCost() const override {
		return sizeof(CSAnimationColorConstant);
	}
	inline CSColor value(float t, const CSColor& r, const CSColor& default) const override {
		return origin;
	}
};

#endif