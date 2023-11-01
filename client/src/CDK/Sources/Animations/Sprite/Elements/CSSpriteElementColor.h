#ifndef __CDK__CSSpriteElementColor__
#define __CDK__CSSpriteElementColor__

#include "CSSpriteElement.h"

#include "CSAnimationColor.h"

class CSSpriteElementColor : public CSSpriteElement {
public:
	CSPtr<CSAnimationColor> color;

	CSSpriteElementColor() = default;
	CSSpriteElementColor(CSBuffer* buffer);
	CSSpriteElementColor(const CSSpriteElementColor* other);
private:
	~CSSpriteElementColor() = default;
public:
	static inline CSSpriteElementColor* element() {
		return autorelease(new CSSpriteElementColor());
	}

	inline Type type() const override {
		return TypeColor;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
private:
	CSColor getColor(float progress, int random) const;
};

#endif