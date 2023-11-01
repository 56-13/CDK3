#ifndef __CDK__CSSpriteElementGradientArc__
#define __CDK__CSSpriteElementGradientArc__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"

#include "CSMaterialSource.h"

class CSSpriteElementGradientArc : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSPtr<CSAnimationFloat> angle0;
	CSPtr<CSAnimationFloat> angle1;
	CSPtr<CSAnimationColor> color0;
	CSPtr<CSAnimationColor> color1;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementGradientArc() = default;
	CSSpriteElementGradientArc(CSBuffer* buffer);
	CSSpriteElementGradientArc(const CSSpriteElementGradientArc* other);
private:
	~CSSpriteElementGradientArc() = default;
public:
	static inline CSSpriteElementGradientArc* element() {
		return autorelease(new CSSpriteElementGradientArc());
	}

	inline Type type() const override {
		return TypeGradientArc;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
	void getAngle(float progress, int random, float& a0, float& a1) const;
	void getColor(float progress, int random, CSColor& c0, CSColor& c1) const;
};

#endif
