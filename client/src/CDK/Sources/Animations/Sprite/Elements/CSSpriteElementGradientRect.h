#ifndef __CDK__CSSpriteElementGradientRect__
#define __CDK__CSSpriteElementGradientRect__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementGradientRect : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSPtr<CSAnimationColor> color0;
	CSPtr<CSAnimationColor> color1;
	CSAlign align = CSAlignCenterMiddle;
	bool fill = false;
	bool horizontal = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementGradientRect() = default;
	CSSpriteElementGradientRect(CSBuffer* buffer);
	CSSpriteElementGradientRect(const CSSpriteElementGradientRect* other);
private:
	~CSSpriteElementGradientRect() = default;
public:
	static inline CSSpriteElementGradientRect* element() {
		return autorelease(new CSSpriteElementGradientRect());
	}

	inline Type type() const override {
		return TypeGradientRect;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
	void getColor(float progress, int random, CSColor& c0, CSColor& c1) const;
};

#endif