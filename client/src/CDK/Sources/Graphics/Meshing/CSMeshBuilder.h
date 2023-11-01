#ifndef __CDK__CSMeshBuilder__
#define __CDK__CSMeshBuilder__

#include "CSMeshObject.h"

class CSMeshBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<const CSArray<ushort>> geometryIndices;
	string geometryName;
	CSPtr<const CSArray<ushort>> animationIndices;
	string animationName;
	CSPtr<const CSArray<CSMaterialSource>> skin;
	CSAnimationLoop loop;
	ushort frameDivision;
	bool collision;

	CSMeshBuilder() = default;
	explicit CSMeshBuilder(CSBuffer* buffer, bool withScene = false);
	CSMeshBuilder(const CSMeshBuilder* other);

	static inline CSMeshBuilder* builder() {
		return autorelease(new CSMeshBuilder());
	}
	static inline CSMeshBuilder* builderWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSMeshBuilder(buffer));
	}
	static inline CSMeshBuilder* builderWithBuilder(const CSMeshBuilder* other) {
		return autorelease(new CSMeshBuilder(other));
	}

	const CSMeshGeometry* geometry() const;
	const CSMeshAnimation* animation() const;
	uint showFlags() const;

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeMesh;
	}
	int resourceCost() const override;
	CSMeshObject* createObject() const override;
	void updateObject(CSMeshObject*& instance) const;
	float clippedProgress(float progress) const;
	void preload() const override;
};

#endif
