#ifndef __CDK__CSSpriteElementStroke__
#define __CDK__CSSpriteElementStroke__

#include "CSSpriteElement.h"

#include "CSAnimationColor.h"

class CSSpriteElementStroke : public CSSpriteElement {
public:
	CSPtr<CSAnimationColor> color;
	CSStrokeMode mode = CSStrokeNormal;
	byte width = 0;

	CSSpriteElementStroke() = default;
	CSSpriteElementStroke(CSBuffer* buffer);
	CSSpriteElementStroke(const CSSpriteElementStroke* other);
private:
	~CSSpriteElementStroke() = default;
public:
	static inline CSSpriteElementStroke* element() {
		return autorelease(new CSSpriteElementStroke());
	}

	inline Type type() const override {
		return TypeStroke;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
private:
	CSColor getColor(float progress, int random) const;
};

#endif