#ifndef __CDK__CSSpriteElementRotate__
#define __CDK__CSSpriteElementRotate__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementRotate : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;

	CSSpriteElementRotate() = default;
	CSSpriteElementRotate(CSBuffer* buffer);
	CSSpriteElementRotate(const CSSpriteElementRotate* other);
private:
	~CSSpriteElementRotate() = default;
public:
	static inline CSSpriteElementRotate* element() {
		return autorelease(new CSSpriteElementRotate());
	}

	inline Type type() const override {
		return TypeRotate;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSMatrix getRotation(float progress, int random) const;
};

#endif