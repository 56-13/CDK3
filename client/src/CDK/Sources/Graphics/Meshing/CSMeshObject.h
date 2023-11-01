#ifndef __CDK__CSMeshObject__
#define __CDK__CSMeshObject__

#include "CSMesh.h"
#include "CSMaterialSource.h"
#include "CSSceneObject.h"

class CSMeshObject : public CSSceneObject {
private:
	const CSMeshGeometry* _geometry;
	const CSArray<CSMaterialSource>* _skin;
	const CSMeshAnimation* _animation;
	float _progress;
	float _clippedProgress;
	int _materialRandom;
public:
	CSAnimationLoop loop;
	bool collision;
private:
	byte _frameDivision;
	mutable bool _dirtyTransforms;
	mutable CSDictionary<string, CSMatrix>* _transforms;
	CSDictionary<string, CSMatrix>* _customTransforms;
public:
	CSMeshObject(const CSMeshGeometry* geometry, const CSSceneObjectBuilder* builder = NULL);
	CSMeshObject(const CSMeshObject* other);
	~CSMeshObject();

	static inline CSMeshObject* object(const CSMeshGeometry* geometry) {
		return autorelease(new CSMeshObject(geometry));
	}
	static inline CSMeshObject* object(const CSMeshObject* other) {
		return autorelease(new CSMeshObject(other));
	}

	inline Type type() const override {
		return TypeMesh;
	}
	inline const CSMeshGeometry* geometry() const {
		return _geometry;
	}
	inline void setSkin(const CSArray<CSMaterialSource>* skin) {
		retain(_skin, skin);
	}
	inline const CSArray<CSMaterialSource>* skin() const {
		return _skin;
	}
	void setAnimation(const CSMeshAnimation* animation);
	inline const CSMeshAnimation* animation() const {
		return _animation;
	}
	void setFrameDivision(int frameDivision);
	inline int frameDivision() const {
		return _frameDivision;
	}
	void setProgress(float progress);

	bool addAABB(CSABoundingBox& result) const override;
	void addCollider(CSCollider*& result) const override;
	bool getTransform(float progress, const string& name, CSMatrix& result) const override;

	float clippedProgress(float progress) const;
	inline float clippedProgress() const {
		return _clippedProgress;
	}
	inline float progress() const override {
		return _progress;
	}
	float duration(DurationParam param, float duration = 0) const override;
protected:	
	void onRewind() override;
	UpdateState onUpdate(float delta, bool alive, uint& flags) override;
	void onDraw(CSGraphics* graphics, CSInstanceLayer layer);
public:
	void drawIndirect(CSGraphics* graphics, CSInstanceLayer layer, float materialProgress, int materialRandom, const CSArray<CSVertexArrayInstance>* instances = NULL);

	void setCustomTransform(const string& name, const CSMatrix& transform);
	void removeCustomTransform(const string& name);
	void clearCustomTransforms();

	bool getNodeTransform(const CSMeshNode* node, CSMatrix& result) const;
	bool getNodeTransform(const string& name, CSMatrix& result) const;
	bool getNodeAABB(const CSMeshNode* node, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const;
	bool getNodeAABB(const CSMeshNode* node, bool inclusive, CSABoundingBox& result) const;
	bool getNodeAABB(const string& name, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const;
	bool getNodeAABB(const string& name, bool inclusive, CSABoundingBox& result) const;
	bool getAABB(CSABoundingBox& result) const;
private:
	bool clipProgress();
	void getNodeLocalTransform(const CSMeshNode* node, CSMatrix& result) const;
	inline CSMatrix getNodeLocalTransform(const CSMeshNode* node) const {
		CSMatrix result;
		getNodeLocalTransform(node, result);
		return result;
	}
	void updateTransforms(const CSMeshNode* node, const CSMatrix& parentTransform) const;
	void updateTransforms() const;
};

#endif