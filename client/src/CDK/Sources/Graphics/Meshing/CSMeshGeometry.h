#ifndef __CDK__CSMeshGeometry__
#define __CDK__CSMeshGeometry__

#include "CSMeshNode.h"
#include "CSMeshFragment.h"
#include "CSMeshAnimation.h"
#include "CSCollider.h"

class CSMeshGeometry : public CSObject {
public:
	struct ColliderFragment {
		string name;
		CSColliderFragment origin;

		ColliderFragment() = default;
		ColliderFragment(CSBuffer* buffer);
	};
private:
	string _name;
	CSMeshNode* _rootNode;
	CSDictionary<string, CSMeshNode> _allNodes;
	CSArray<CSMeshNode> _renderNodes;
	CSArray<CSMeshFragment> _fragments;
	CSArray<string> _materialConfigs;
	CSArray<ColliderFragment> _colliderFragments;
	CSABoundingBox _aabb;
public:
	CSMeshGeometry(CSBuffer* buffer);
private:
	~CSMeshGeometry();
public:
	static inline CSMeshGeometry* geometryWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSMeshGeometry(buffer));
	}

	int resourceCost() const;

	inline const string& name() const {
		return _name;
	}
	inline const CSMeshNode* rootNode() const {
		return _rootNode;
	}
	inline const CSMeshNode* findNode(const string& name) const {
		return _allNodes.objectForKey(name);
	}
	inline const CSArray<CSMeshNode>* renderNodes() const {
		return &_renderNodes;
	}
	inline const CSArray<CSMeshFragment>* fragments() const {
		return &_fragments;
	}
	inline const CSArray<string>* materialConfigs() const {
		return &_materialConfigs;
	}
	inline const CSArray<ColliderFragment>* colliderFragments() const {
		return &_colliderFragments;
	}
	inline const CSABoundingBox& aabb() const {
		return _aabb;
	}
	int vertexCount() const;
	int faceCount() const;
	int boneCount() const;

	void getNodeTransform(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, CSMatrix& result) const;
	inline CSMatrix getNodeTransform(const CSMeshNode* node, const CSMeshAnimation* animation, float progress) {
		CSMatrix result;
		getNodeTransform(node, animation, progress, result);
		return result;
	}
	bool getNodeTransform(const string& name, const CSMeshAnimation* animation, float progress, CSMatrix& result) const;
	bool getNodeAABB(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const;
	bool getNodeAABB(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, CSABoundingBox& result) const;
	bool getNodeAABB(const string& name, const CSMeshAnimation* animation, float progress, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const;
	bool getNodeAABB(const string& name, const CSMeshAnimation* animation, float progress, bool inclusive, CSABoundingBox& result) const;
	bool getAABB(const CSMeshAnimation* animation, float progress, CSABoundingBox& result) const;
#ifdef CDK_IMPL
	bool getNodeAABBInternal(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, const CSMatrix& parentTransform, bool leaf, CSABoundingBox& result) const;
#endif
private:
	void retrieve(CSMeshNode* node);
};

#endif
