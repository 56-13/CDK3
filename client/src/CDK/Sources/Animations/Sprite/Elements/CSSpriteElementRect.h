#ifndef __CDK__CSSpriteElementRect__
#define __CDK__CSSpriteElementRect__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementRect : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSAlign align = CSAlignCenterMiddle;
	bool fill = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementRect() = default;
	CSSpriteElementRect(CSBuffer* buffer);
	CSSpriteElementRect(const CSSpriteElementRect* other);
private:
	~CSSpriteElementRect() = default;
public:
	static inline CSSpriteElementRect* element() {
		return autorelease(new CSSpriteElementRect());
	}

	inline Type type() const override {
		return TypeRect;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
};

#endif