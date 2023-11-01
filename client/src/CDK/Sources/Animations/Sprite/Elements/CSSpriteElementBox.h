#ifndef __CDK__CSSpriteElementBox__
#define __CDK__CSSpriteElementBox__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementBox : public CSSpriteElement {
public:
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSPtr<CSAnimationFloat> xext;
	CSPtr<CSAnimationFloat> yext;
	CSPtr<CSAnimationFloat> zext;
	bool collision = false;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementBox() = default;
	CSSpriteElementBox(CSBuffer* buffer);
	CSSpriteElementBox(const CSSpriteElementBox* other);
private:
	~CSSpriteElementBox() = default;
public:
	static inline CSSpriteElementBox* element() {
		return autorelease(new CSSpriteElementBox());
	}

	inline Type type() const override {
		return TypeBox;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSABoundingBox getBox(float progress, int random) const;
};

#endif