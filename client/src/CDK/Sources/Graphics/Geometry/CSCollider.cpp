#define CDK_IMPL

#include "CSCollider.h"

#include "CSBuffer.h"

CSColliderFragment::CSColliderFragment() : _type(TypeNone) {
	
}

CSColliderFragment::CSColliderFragment(const CSOBoundingBox& box) : _type(TypeBox) {
	new (&_shape.box) CSOBoundingBox(box);
}

CSColliderFragment::CSColliderFragment(const CSBoundingSphere& sphere) : _type(TypeSphere) {
	new (&_shape.sphere) CSBoundingSphere(sphere);
}

CSColliderFragment::CSColliderFragment(const CSBoundingCapsule& capsule) : _type(TypeCapsule) {
	new (&_shape.capsule) CSBoundingCapsule(capsule);
}

CSColliderFragment::CSColliderFragment(const CSBoundingMesh& mesh) : _type(TypeMesh) {
	new (&_shape.mesh) CSBoundingMesh(mesh);
}

CSColliderFragment::CSColliderFragment(CSBuffer* buffer) : _type((Type)buffer->readByte()) {
	switch (_type) {
		case TypeBox:
			new (&_shape.box) CSOBoundingBox(buffer);
			break;
		case TypeSphere:
			new (&_shape.sphere) CSBoundingSphere(buffer);
			break;
		case TypeCapsule:
			new (&_shape.capsule) CSBoundingCapsule(buffer);
			break;
		case TypeMesh:
			new (&_shape.mesh) CSBoundingMesh(buffer);
			break;
	}
}

CSColliderFragment::~CSColliderFragment() {
	switch (_type) {
		case TypeBox:
			_shape.box.~CSOBoundingBox();
			break;
		case TypeSphere:
			_shape.sphere.~CSBoundingSphere();
			break;
		case TypeCapsule:
			_shape.capsule.~CSBoundingCapsule();
			break;
		case TypeMesh:
			_shape.mesh.~CSBoundingMesh();
			break;
	}
}

CSColliderFragment::CSColliderFragment(const CSColliderFragment& other) : _type(other._type) {
	switch (_type) {
		case TypeBox:
			new (&_shape.box) CSOBoundingBox(other._shape.box);
			break;
		case TypeSphere:
			new (&_shape.sphere) CSBoundingSphere(other._shape.sphere);
			break;
		case TypeCapsule:
			new (&_shape.capsule) CSBoundingCapsule(other._shape.capsule);
			break;
		case TypeMesh:
			new (&_shape.mesh) CSBoundingMesh(other._shape.mesh);
			break;
	}
}

CSColliderFragment& CSColliderFragment::operator=(const CSColliderFragment& other) {
	this->~CSColliderFragment();

	_type = other._type;

	switch (_type) {
		case TypeBox:
			new (&_shape.box) CSOBoundingBox(other._shape.box);
			break;
		case TypeSphere:
			new (&_shape.sphere) CSBoundingSphere(other._shape.sphere);
			break;
		case TypeCapsule:
			new (&_shape.capsule) CSBoundingCapsule(other._shape.capsule);
			break;
		case TypeMesh:
			new (&_shape.mesh) CSBoundingMesh(other._shape.mesh);
			break;
	}

	return *this;
}

void CSColliderFragment::transform(const CSMatrix& transform) {
	switch (_type) {
		case TypeBox:
			CSOBoundingBox::transform(_shape.box, transform, _shape.box);
			break;
		case TypeSphere:
			CSBoundingSphere::transform(_shape.sphere, transform, _shape.sphere);
			break;
		case TypeCapsule:
			CSBoundingCapsule::transform(_shape.capsule, transform, _shape.capsule);
			break;
		case TypeMesh:
			_shape.mesh.transform(transform);
			break;
	}
}

float CSColliderFragment::getZ(const CSVector3& pos) const {
	switch (_type) {
		case TypeBox:
			return _shape.box.getZ(pos);
		case TypeSphere:
			return _shape.sphere.getZ(pos);
		case TypeCapsule:
			return _shape.capsule.getZ(pos);
		case TypeMesh:
			return _shape.mesh.getZ(pos);
	}
	return 0;
}

bool CSColliderFragment::intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit) const {
	switch (_type) {
		case TypeBox:
			return _shape.box.intersects(ray, flags, distance, hit);
		case TypeSphere:
			return _shape.sphere.intersects(ray, flags, distance, hit);
		case TypeCapsule:
			return _shape.capsule.intersects(ray, flags, distance, hit);
		case TypeMesh:
			return _shape.mesh.intersects(ray, flags, distance, hit);
	}
	return false;
}

CSCollisionResult CSColliderFragment::intersects(const CSColliderFragment& other, CSCollisionFlags flags, CSHit* hit) const {
	switch (_type) {
		case TypeBox:
			switch (other._type) {
				case TypeBox:
					return _shape.box.intersects(other._shape.box, flags, hit);
				case TypeSphere:
					return _shape.box.intersects(other._shape.sphere, flags, hit);
				case TypeCapsule:
					return _shape.box.intersects(other._shape.capsule, flags, hit);
				case TypeMesh:
					return _shape.box.intersects(other._shape.mesh, flags, hit);
			}
			break;
		case TypeSphere:
			switch (other._type) {
				case TypeBox:
					return _shape.sphere.intersects(other._shape.box, flags, hit);
				case TypeSphere:
					return _shape.sphere.intersects(other._shape.sphere, flags, hit);
				case TypeCapsule:
					return _shape.sphere.intersects(other._shape.capsule, flags, hit);
				case TypeMesh:
					return _shape.sphere.intersects(other._shape.mesh, flags, hit);
			}
			break;
		case TypeCapsule:
			switch (other._type) {
				case TypeBox:
					return _shape.capsule.intersects(other._shape.box, flags, hit);
				case TypeSphere:
					return _shape.capsule.intersects(other._shape.sphere, flags, hit);
				case TypeCapsule:
					return _shape.capsule.intersects(other._shape.capsule, flags, hit);
				case TypeMesh:
					return _shape.capsule.intersects(other._shape.mesh, flags, hit);
			}
			break;
		case TypeMesh:
			switch (other._type) {
				case TypeBox:
					return _shape.mesh.intersects(other._shape.box, flags, hit);
				case TypeSphere:
					return _shape.mesh.intersects(other._shape.sphere, flags, hit);
				case TypeCapsule:
					return _shape.mesh.intersects(other._shape.capsule, flags, hit);
				case TypeMesh:
					return _shape.mesh.intersects(other._shape.mesh, flags, hit);
			}
			break;
	}
	return CSCollisionResultFront;
}

//=====================================================================================================
void CSCollider::transform(const CSMatrix& trans) {
	foreach (CSColliderFragment&, frag, this) frag.transform(trans);
}

CSCollider* CSCollider::transform(const CSCollider* collider, const CSMatrix& trans) {
	CSCollider* result = colliderWithCapacity(collider->count());
	foreach (const CSColliderFragment&, frag, collider) result->addObject(CSColliderFragment::transform(frag, trans));
	return result;
}

void CSCollider::getZ(const CSVector3& pos, float& z) const {
	foreach (const CSColliderFragment&, frag, this) {
		float fz = frag.getZ(pos);
		if (z < fz) z = fz;
	}
}

bool CSCollider::intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit) const {
	bool flag = false;
	float d;
	CSHit h;
	foreach (const CSColliderFragment&, frag, this) {
		if (frag.intersects(ray, flags, d, &h) && d < distance) {
			distance = d;
			if (hit) *hit = h;
			flag = true;
		}
	}
	return flag;
}

CSCollisionResult CSCollider::intersects(const CSCollider* other, CSCollisionFlags flags, float& distance, CSHit* hit) const {
	CSCollisionResult result = CSCollisionResultFront;
	flags |= CSCollisionFlagHit;
	CSHit h;
	foreach (const CSColliderFragment&, frag, this) {
		foreach (const CSColliderFragment&, otherfrag, other) {
			CSCollisionResult r = frag.intersects(otherfrag, flags, &h);
			if (r != CSCollisionResultFront && h.distance < distance) {
				result = r;
				distance = h.distance;
				if (hit) *hit = h;
			}
		}
	}
	return result;
}