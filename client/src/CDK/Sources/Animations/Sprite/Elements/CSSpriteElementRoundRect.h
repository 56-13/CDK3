#ifndef __CDK__CSSpriteElementRoundRect__
#define __CDK__CSSpriteElementRoundRect__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementRoundRect : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSPtr<CSAnimationFloat> radius;
	CSAlign align = CSAlignCenterMiddle;
	CSCorner corner = CSCornerAll;
	bool fill = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementRoundRect() = default;
	CSSpriteElementRoundRect(CSBuffer* buffer);
	CSSpriteElementRoundRect(const CSSpriteElementRoundRect* other);
private:
	~CSSpriteElementRoundRect() = default;
public:
	static inline CSSpriteElementRoundRect* element() {
		return autorelease(new CSSpriteElementRoundRect());
	}

	inline Type type() const override {
		return TypeRoundRect;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
	float getRadius(float progress, int random) const;
};

#endif