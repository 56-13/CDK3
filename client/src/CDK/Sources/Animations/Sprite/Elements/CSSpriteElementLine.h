#ifndef __CDK__CSSpriteElementLine__
#define __CDK__CSSpriteElementLine__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMaterialSource.h"

class CSSpriteElementLine : public CSSpriteElement {
public:
	CSVector3 position0 = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x0;
	CSPtr<CSAnimationFloat> y0;
	CSPtr<CSAnimationFloat> z0;
	CSVector3 position1 = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x1;
	CSPtr<CSAnimationFloat> y1;
	CSPtr<CSAnimationFloat> z1;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementLine() = default;
	CSSpriteElementLine(CSBuffer* buffer);
	CSSpriteElementLine(const CSSpriteElementLine* other);
private:
	~CSSpriteElementLine() = default;
public:
	static inline CSSpriteElementLine* element() {
		return autorelease(new CSSpriteElementLine());
	}

	inline Type type() const override {
		return TypeLine;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	void draw(DrawParam& param) const override;
private:
	void getPosition(float progress, int random, CSVector3& p0, CSVector3& p1) const;
};

#endif