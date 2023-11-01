#ifndef __CDK__CSSpriteElementInvert__
#define __CDK__CSSpriteElementInvert__

#include "CSSpriteElement.h"

class CSSpriteElementInvert : public CSSpriteElement {
public:
	bool x = false;
	bool y = false;
	bool z = false;

	CSSpriteElementInvert() = default;
	CSSpriteElementInvert(CSBuffer* buffer);
	CSSpriteElementInvert(const CSSpriteElementInvert* other);
private:
	~CSSpriteElementInvert() = default;
public:
	static inline CSSpriteElementInvert* element() {
		return autorelease(new CSSpriteElementInvert());
	}

	inline Type type() const override {
		return TypeInvert;
	}
	inline int resourceCost() const override {
		return sizeof(CSSpriteElementInvert);
	}
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getScale() const;
};

#endif