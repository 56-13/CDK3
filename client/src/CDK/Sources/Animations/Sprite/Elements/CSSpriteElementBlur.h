#ifndef __CDK__CSSpriteElementBlur__
#define __CDK__CSSpriteElementBlur__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementBlur : public CSSpriteElement {
public:
	enum Mode : byte {
		ModeNormal,
		ModeDepth,
		ModeDirection,
		ModeCenter
	};
	CSInstanceBlendLayer layer = CSInstanceBlendLayerMiddle;
	Mode mode = ModeNormal;
	CSRect frame = CSRect::Zero;
	CSPtr<CSAnimationFloat> intensity;
	CSPtr<CSAnimationFloat> depthDistance;
	CSPtr<CSAnimationFloat> depthRange;
	CSPtr<CSAnimationFloat> directionX;
	CSPtr<CSAnimationFloat> directionY;
	CSPtr<CSAnimationFloat> centerX;
	CSPtr<CSAnimationFloat> centerY;
	CSPtr<CSAnimationFloat> centerRange;

	CSSpriteElementBlur() = default;
	CSSpriteElementBlur(CSBuffer* buffer);
	CSSpriteElementBlur(const CSSpriteElementBlur* other);
private:
	~CSSpriteElementBlur() = default;
public:
	static inline CSSpriteElementBlur* element() {
		return autorelease(new CSSpriteElementBlur());
	}

	inline Type type() const override {
		return TypeBlur;
	}
	int resourceCost() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
};

#endif