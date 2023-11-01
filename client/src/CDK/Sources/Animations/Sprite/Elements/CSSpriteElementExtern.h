#ifndef __CDK__CSSpriteElementExtern__
#define __CDK__CSSpriteElementExtern__

#include "CSSpriteElement.h"

#include "CSData.h"

class CSSpriteElementExtern : public CSSpriteElement {
public:
	CSPtr<const CSData> data;

	CSSpriteElementExtern() = default;
	CSSpriteElementExtern(CSBuffer* buffer);
	CSSpriteElementExtern(const CSSpriteElementExtern* other);
private:
	~CSSpriteElementExtern() = default;
public:
	static inline CSSpriteElementExtern* element() {
		return autorelease(new CSSpriteElementExtern());
	}

	inline Type type() const override {
		return TypeExtern;
	}
	int resourceCost() const override;
	void draw(DrawParam& param) const override;
};

#endif