#ifndef __CDK__CSSpriteElementImage__
#define __CDK__CSSpriteElementImage__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSAnimationSourceImage.h"

class CSSpriteElementImage : public CSSpriteElement {
public:
	enum ShadowType : byte {
		ShadowTypeNone,
		ShadowTypeFlat,
		ShadowTypeRotate
	};
	CSPtr<CSAnimationSourceImage> source;
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSAlign align = CSAlignCenterMiddle;
	ShadowType shadowType = ShadowTypeNone;
	float shadowDistance = 0;
	CSVector2 shadowFlatOffset = CSVector2::Zero;
	bool shadowFlatXFlip = false;
	bool shadowFlatYFlip = false;
	CSVector2 shadowRotateOffset = CSVector2::Zero;
	float shadowRotateFlatness = 0;

	CSSpriteElementImage() = default;
	CSSpriteElementImage(CSBuffer* buffer);
	CSSpriteElementImage(const CSSpriteElementImage* other);
private:
	~CSSpriteElementImage() = default;
public:
	static inline CSSpriteElementImage* element() {
		return autorelease(new CSSpriteElementImage());
	}

	inline Type type() const override {
		return TypeImage;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getPosition(float progress, int random) const;
};

#endif