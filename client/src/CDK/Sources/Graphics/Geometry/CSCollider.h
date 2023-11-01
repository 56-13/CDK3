#ifndef __CDK__CSCollider__
#define __CDK__CSCollider__

#include "CSOBoundingBox.h"
#include "CSBoundingSphere.h"
#include "CSBoundingCapsule.h"
#include "CSBoundingMesh.h"

class CSColliderFragment {
public:
	enum Type : byte {
		TypeNone,
		TypeBox,
		TypeSphere,
		TypeCapsule,
		TypeMesh
	};
private:
	Type _type;
	union shape {
		CSOBoundingBox box;
		CSBoundingSphere sphere;
		CSBoundingCapsule capsule;
		CSBoundingMesh mesh;
		inline shape() {}
		inline ~shape() {}
	} _shape;
public:
	CSColliderFragment();
	CSColliderFragment(const CSOBoundingBox& box);
	CSColliderFragment(const CSBoundingSphere& sphere);
	CSColliderFragment(const CSBoundingCapsule& capsule);
	CSColliderFragment(const CSBoundingMesh& mesh);
	explicit CSColliderFragment(CSBuffer* bufffer);
	~CSColliderFragment();

	CSColliderFragment(const CSColliderFragment& other);
	CSColliderFragment& operator=(const CSColliderFragment& other);
	
	inline Type type() const {
		return _type;
	}
	inline const CSOBoundingBox& asBox() const {
		CSAssert(_type == TypeBox);
		return _shape.box;
	}
	inline const CSBoundingSphere& asSphere() const {
		CSAssert(_type == TypeSphere);
		return _shape.sphere;
	}
	inline const CSBoundingCapsule& asCapsule() const {
		CSAssert(_type == TypeCapsule);
		return _shape.capsule;
	}
	inline const CSBoundingMesh& asMesh() const {
		CSAssert(_type == TypeMesh);
		return _shape.mesh;
	}
	
	void transform(const CSMatrix& trans);
	static inline void transform(const CSColliderFragment& frag, const CSMatrix& trans, CSColliderFragment& result) {
		result = frag;
		result.transform(trans);
	}
	static inline CSColliderFragment transform(const CSColliderFragment& frag, const CSMatrix& trans) {
		CSColliderFragment result = frag;
		result.transform(trans);
		return result;
	}
	float getZ(const CSVector3& pos) const;
	bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const;
	CSCollisionResult intersects(const CSColliderFragment& other, CSCollisionFlags flags, CSHit* hit = NULL) const;
};

class CSCollider : public CSArray<CSColliderFragment> {
public:
	CSCollider() = default;
	inline CSCollider(int capacity) : CSArray<CSColliderFragment>(capacity) {}
private:
	~CSCollider() = default;
public:
	static inline CSCollider* collider() {
		return autorelease(new CSCollider());
	}
	static inline CSCollider* colliderWithCapacity(int capacity) {
		return autorelease(new CSCollider(capacity));
	}

	void transform(const CSMatrix& transform);
	static CSCollider* transform(const CSCollider* collider, const CSMatrix& trans);
	void getZ(const CSVector3& pos, float& z) const;
	bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const;
	CSCollisionResult intersects(const CSCollider* other, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const;
};

#endif
