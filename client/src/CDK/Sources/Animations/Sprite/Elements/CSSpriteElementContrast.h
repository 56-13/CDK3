#ifndef __CDK__CSSpriteElementContrast__
#define __CDK__CSSpriteElementContrast__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementContrast : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> value;

	CSSpriteElementContrast() = default;
	CSSpriteElementContrast(CSBuffer* buffer);
	CSSpriteElementContrast(const CSSpriteElementContrast* other);
private:
	~CSSpriteElementContrast() = default;
public:
	static inline CSSpriteElementContrast* element() {
		return autorelease(new CSSpriteElementContrast());
	}

	inline Type type() const override {
		return TypeContrast;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
private:
	float getValue(float progress, int random) const;
};

#endif