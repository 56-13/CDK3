#ifndef __CDK__CSSpriteElementSphere__
#define __CDK__CSSpriteElementSphere__

#include "CSSpriteElement.h"

#include "CSBoundingSphere.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementSphere : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> radius;
	bool collision = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementSphere() = default;
	CSSpriteElementSphere(CSBuffer* buffer);
	CSSpriteElementSphere(const CSSpriteElementSphere* other);
private:
	~CSSpriteElementSphere() = default;
public:
	static inline CSSpriteElementSphere* element() {
		return autorelease(new CSSpriteElementSphere());
	}

	inline Type type() const override {
		return TypeSphere;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSBoundingSphere getSphere(float progress, int random) const;
};

#endif