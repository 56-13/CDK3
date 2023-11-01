#ifndef __CDK__CSSpriteElementScale__
#define __CDK__CSSpriteElementScale__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementScale : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	bool each = false;

	CSSpriteElementScale() = default;
	CSSpriteElementScale(CSBuffer* buffer);
	CSSpriteElementScale(const CSSpriteElementScale* other);
private:
	~CSSpriteElementScale() = default;
public:
	static inline CSSpriteElementScale* element() {
		return autorelease(new CSSpriteElementScale());
	}

	inline Type type() const override {
		return TypeScale;
	}
	int resourceCost() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getScale(float progress, int random) const;
};

#endif