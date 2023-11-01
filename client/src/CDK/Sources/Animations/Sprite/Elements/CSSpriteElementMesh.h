#ifndef __CDK__CSSpriteElementMesh__
#define __CDK__CSSpriteElementMesh__

#include "CSSpriteElement.h"

#include "CSAnimationFloat.h"

#include "CSMeshBuilder.h"

class CSSpriteElementMesh : public CSSpriteElement {
public:
	CSPtr<CSMeshBuilder> builder;
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
public:
	CSSpriteElementMesh() = default;
	CSSpriteElementMesh(CSBuffer* buffer);
	CSSpriteElementMesh(const CSSpriteElementMesh* other);
private:
	~CSSpriteElementMesh() = default;
public:
	static inline CSSpriteElementMesh* element() {
		return autorelease(new CSSpriteElementMesh());
	}

	inline Type type() const override {
		return TypeMesh;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void addCollider(TransformParam& param, CSCollider*& result) const override;
	bool getTransform(TransformParam& param, const string& name, CSMatrix& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const override;
	uint showFlags() const override;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getPosition(float progress, int random) const;
	CSMeshObject* updateInstance(const CSSpriteObject* parent, float progress, float duration) const;
};

#endif