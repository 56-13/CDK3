#ifndef __CDK__CSSpriteElementSaturation__
#define __CDK__CSSpriteElementSaturation__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementSaturation : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> value;

	CSSpriteElementSaturation() = default;
	CSSpriteElementSaturation(CSBuffer* buffer);
	CSSpriteElementSaturation(const CSSpriteElementSaturation* other);
private:
	~CSSpriteElementSaturation() = default;
public:
	static inline CSSpriteElementSaturation* element() {
		return autorelease(new CSSpriteElementSaturation());
	}

	inline Type type() const override {
		return TypeSaturation;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
private:
	float getValue(float progress, int random) const;
};

#endif