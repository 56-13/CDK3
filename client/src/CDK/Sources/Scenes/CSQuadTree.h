#ifndef __CDK__CSQuadTree__
#define __CDK__CSQuadTree__

#include "CSSceneObject.h"

class CSQuadTree {
private:
	class Node {
	private:
		CSABoundingBox _space;
		CSArray<CSSceneObject*>* _objects;
		Node* _nodes[8];
		bool _branch;
	public:
		Node(const CSABoundingBox& space);
		~Node();

		Node(const Node&) = delete;
		Node& operator=(const Node&) = delete;

		inline const CSABoundingBox& space() const {
			return _space;
		}

		void locate(CSSceneObject* obj, const CSABoundingBox& aabb, const CSVector3* aabbCorners, int depth);
		bool unlocate(CSSceneObject* obj, const CSABoundingBox& aabb, const CSVector3* aabbCorners, int depth);
		void selectAll(CSSet<CSSceneObject*>* objs) const;
		void select(const std::function<CSCollisionResult(const CSABoundingBox&)>& func, CSSet<CSSceneObject*>* objs) const;
	};
	Node* _top;
	ushort _grid;
	ushort _depth;
	mutable CSLock _lock;
public:
	CSQuadTree(const CSABoundingBox& space, int grid);
	~CSQuadTree();

	CSQuadTree(const CSQuadTree&) = delete;
	CSQuadTree& operator=(const CSQuadTree&) = delete;

	inline const CSABoundingBox& space() const {
		return _top->space();
	}
	inline int grid() const {
		return _grid;
	}
	inline int depth() const {
		return _depth;
	}

	void resize(const CSABoundingBox& space, int grid);
	void locate(CSSceneObject* obj);
	void unlocate(CSSceneObject* obj);
	void relocate(CSSceneObject* obj, const CSABoundingBox* naabb);

	void selectAll(CSSet<CSSceneObject*>* objs) const;
	void select(const CSVector3& pos, CSSet<CSSceneObject*>* objs) const;
	void select(const CSBoundingFrustum& frustum, CSSet<CSSceneObject*>* objs) const;
	void select(const CSRay& ray, CSSet<CSSceneObject*>* objs) const;
	void select(const CSBoundingSphere& sphere, CSSet<CSSceneObject*>* objs) const;
	void select(const CSBoundingCapsule& capsule, CSSet<CSSceneObject*>* objs) const;
	void select(const CSABoundingBox& box, CSSet<CSSceneObject*>* objs) const;
	void select(const CSOBoundingBox& box, CSSet<CSSceneObject*>* objs) const;
	void select(const CSBoundingMesh& mesh, CSSet<CSSceneObject*>* objs) const;
	void select(const CSCollider* collider, CSSet<CSSceneObject*>* objs) const;

	CSSet<CSSceneObject*>* selectAll() const;
	CSSet<CSSceneObject*>* select(const CSVector3& pos) const;
	CSSet<CSSceneObject*>* select(const CSBoundingFrustum& frustum) const;
	CSSet<CSSceneObject*>* select(const CSRay& ray) const;
	CSSet<CSSceneObject*>* select(const CSBoundingSphere& sphere) const;
	CSSet<CSSceneObject*>* select(const CSBoundingCapsule& capsule) const;
	CSSet<CSSceneObject*>* select(const CSABoundingBox& box) const;
	CSSet<CSSceneObject*>* select(const CSOBoundingBox& box) const;
	CSSet<CSSceneObject*>* select(const CSBoundingMesh& mesh) const;
	CSSet<CSSceneObject*>* select(const CSCollider* collider) const;
};

#endif