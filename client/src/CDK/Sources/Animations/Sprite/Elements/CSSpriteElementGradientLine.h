#ifndef __CDK__CSSpriteElementGradientLine__
#define __CDK__CSSpriteElementGradientLine__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"

#include "CSMaterialSource.h"

class CSSpriteElementGradientLine : public CSSpriteElement {
public:
	CSVector3 position0 = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x0;
	CSPtr<CSAnimationFloat> y0;
	CSPtr<CSAnimationFloat> z0;
	CSPtr<CSAnimationColor> color0;
	CSVector3 position1 = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x1;
	CSPtr<CSAnimationFloat> y1;
	CSPtr<CSAnimationFloat> z1;
	CSPtr<CSAnimationColor> color1;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementGradientLine() = default;
	CSSpriteElementGradientLine(CSBuffer* buffer);
	CSSpriteElementGradientLine(const CSSpriteElementGradientLine* other);
private:
	~CSSpriteElementGradientLine() = default;
public:
	static inline CSSpriteElementGradientLine* element() {
		return autorelease(new CSSpriteElementGradientLine());
	}

	inline Type type() const override {
		return TypeGradientLine;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	void getPosition(float progress, int random, CSVector3& p0, CSVector3& p1) const;
	void getColor(float progress, int random, CSColor& c0, CSColor& c1) const;
};

#endif