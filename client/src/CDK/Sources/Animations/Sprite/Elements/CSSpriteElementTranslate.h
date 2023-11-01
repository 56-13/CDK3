#ifndef __CDK__CSSpriteElementTranslate__
#define __CDK__CSSpriteElementTranslate__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

class CSSpriteElementTranslate : public CSSpriteElement {
public:
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;

	CSSpriteElementTranslate() = default;
	CSSpriteElementTranslate(CSBuffer* buffer);
	CSSpriteElementTranslate(const CSSpriteElementTranslate* other);
private:
	~CSSpriteElementTranslate() = default;
public:
	static inline CSSpriteElementTranslate* element() {
		return autorelease(new CSSpriteElementTranslate());
	}

	inline Type type() const override {
		return TypeTranslate;
	}
	int resourceCost() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getTranslation(float progress, int random) const;
};

#endif