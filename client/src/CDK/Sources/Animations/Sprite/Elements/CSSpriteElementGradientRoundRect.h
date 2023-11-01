#ifndef __CDK__CSSpriteElementGradientRoundRect__
#define __CDK__CSSpriteElementGradientRoundRect__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"

#include "CSMaterialSource.h"

class CSSpriteElementGradientRoundRect : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSPtr<CSAnimationFloat> radius;
	CSPtr<CSAnimationColor> color0;
	CSPtr<CSAnimationColor> color1;
	CSAlign align = CSAlignCenterMiddle;
	CSCorner corner = CSCornerAll;
	bool fill = false;
	bool horizontal = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementGradientRoundRect() = default;
	CSSpriteElementGradientRoundRect(CSBuffer* buffer);
	CSSpriteElementGradientRoundRect(const CSSpriteElementGradientRoundRect* other);
private:
	~CSSpriteElementGradientRoundRect() = default;
public:
	static inline CSSpriteElementGradientRoundRect* element() {
		return autorelease(new CSSpriteElementGradientRoundRect());
	}

	inline Type type() const override {
		return TypeGradientRoundRect;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
	float getRadius(float progress, int random) const;
	void getColor(float progress, int random, CSColor& c0, CSColor& c1) const;
};

#endif