#ifndef __CDK__CSSpriteElementLens__
#define __CDK__CSSpriteElementLens__

#include "CSSpriteElement.h"

#include "CSBoundingSphere.h"

#include "CSAnimationFloat.h"

class CSSpriteElementLens : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> radius;
	CSPtr<CSAnimationFloat> convex;

	CSSpriteElementLens() = default;
	CSSpriteElementLens(CSBuffer* buffer);
	CSSpriteElementLens(const CSSpriteElementLens* other);
private:
	~CSSpriteElementLens() = default;
public:
	static inline CSSpriteElementLens* element() {
		return autorelease(new CSSpriteElementLens());
	}

	inline Type type() const override {
		return TypeLens;
	}
	int resourceCost() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	uint showFlags() const override;
	void draw(DrawParam& param) const override;
private:
	CSBoundingSphere getSphere(float progress, int random) const;
	float getConvex(float progress, int random) const;
};

#endif