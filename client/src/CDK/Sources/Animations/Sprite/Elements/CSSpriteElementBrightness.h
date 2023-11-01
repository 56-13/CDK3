#ifndef __CDK__CSSpriteElementBrightness__
#define __CDK__CSSpriteElementBrightness__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementBrightness : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> value;

	CSSpriteElementBrightness() = default;
	CSSpriteElementBrightness(CSBuffer* buffer);
	CSSpriteElementBrightness(const CSSpriteElementBrightness* other);
private:
	~CSSpriteElementBrightness() = default;
public:
	static inline CSSpriteElementBrightness* element() {
		return autorelease(new CSSpriteElementBrightness());
	}

	inline Type type() const override {
		return TypeBrightness;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
private:
	float getValue(float progress, int random) const;
};

#endif