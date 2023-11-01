#ifndef __CDK__CSSpriteElementArc__
#define __CDK__CSSpriteElementArc__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementArc : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> width;
	CSPtr<CSAnimationFloat> height;
	CSPtr<CSAnimationFloat> angle0;
	CSPtr<CSAnimationFloat> angle1;
	bool fill = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementArc() = default;
	CSSpriteElementArc(CSBuffer* buffer);
	CSSpriteElementArc(const CSSpriteElementArc* other);
private:
	~CSSpriteElementArc() = default;
public:
	static inline CSSpriteElementArc* element() {
		return autorelease(new CSSpriteElementArc());
	}

	inline Type type() const override {
		return TypeArc;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSZRect getRect(float progress, int random) const;
	void getAngle(float progress, int random, float& a0, float& a1) const;
};

#endif