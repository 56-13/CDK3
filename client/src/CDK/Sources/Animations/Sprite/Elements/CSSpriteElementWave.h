#ifndef __CDK__CSSpriteElementWave__
#define __CDK__CSSpriteElementWave__

#include "CSSpriteElement.h"

#include "CSBoundingSphere.h"

#include "CSAnimationFloat.h"

class CSSpriteElementWave : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> radius;
	CSPtr<CSAnimationFloat> thickness;

	CSSpriteElementWave() = default;
	CSSpriteElementWave(CSBuffer* buffer);
	CSSpriteElementWave(const CSSpriteElementWave* other);
private:
	~CSSpriteElementWave() = default;
public:
	static inline CSSpriteElementWave* element() {
		return autorelease(new CSSpriteElementWave());
	}

	inline Type type() const override {
		return TypeWave;
	}
	int resourceCost() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	uint showFlags() const override;
	void draw(DrawParam& param) const override;
private:
	CSBoundingSphere getSphere(float progress, int random) const;
	float getThickness(float progress, int random) const;
};

#endif