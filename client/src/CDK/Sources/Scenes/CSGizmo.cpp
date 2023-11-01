#define CDK_IMPL

#include "CSGizmo.h"

#include "CSScene.h"

#include "CSBuffer.h"

CSGizmoData::CSGizmoData(CSBuffer* buffer) :
	binding(buffer->readString()),
	targetDataId(buffer->readInt()),
	position(buffer),
	rotation(buffer),
	scale(buffer),
	fromGround(buffer->readBoolean()) 
{
}

CSGizmoData::CSGizmoData(const CSGizmoData* other) :
	binding(other->binding),
	targetDataId(other->targetDataId),
	position(other->position),
	rotation(other->rotation),
	scale(other->scale),
	fromGround(other->fromGround)
{

}

int CSGizmoData::resourceCost() const {
	int cost = sizeof(CSGizmoData);
	if (binding) cost += binding.resourceCost();
	return cost;
}

CSGizmo::CSGizmo(CSSceneObject* obj, const CSGizmoData* data) :
	_object(obj),
	_target(NULL),
	_updated(false) 
{
	if (data) {
		_binding = data->binding;
		_targetDataId = data->targetDataId;
		_position = data->position;
		_rotation = data->rotation;
		_scale = data->scale;
		_fromGround = data->fromGround;
	}
	else {
		_binding.clear();
		_targetDataId = 0;
		_position = CSVector3::Zero;
		_rotation = CSQuaternion::Identity;
		_scale = CSVector3::Zero;
		_fromGround = true;
	}
}

CSGizmo::~CSGizmo() {
	CSObject::release(_target);
}

void CSGizmo::setBinding(const string& binding) {
	if (_binding != binding) {
		_binding = binding;
		_updated = true;
	}
}

void CSGizmo::loadTarget() const {
	if (_targetDataId && !_target && _object->scene()) {
		_target = CSObject::retain(_object->scene()->objectForDataId(_targetDataId));
		if (_target) {
			_targetDataId = 0;
			_updated = true;
		}
	}
}

void CSGizmo::setTarget(const CSSceneObject* target) {
	_updated |= CSObject::retain(_target, target);
	_targetDataId = 0;
}

void CSGizmo::setPosition(const CSVector3& position) {
	if (_position != position) {
		_position = position;
		_updated = true;
	}
}

void CSGizmo::setRotation(const CSQuaternion& rotation) {
	if (_rotation != rotation) {
		_rotation = rotation;
		_updated = true;
	}
}

void CSGizmo::setScale(const CSVector3& scale) {
	if (_scale != scale) {
		_scale = scale;
		_updated = true;
	}
}

void CSGizmo::setFromGround(bool fromGround) {
	if (_fromGround != fromGround) {
		_fromGround = fromGround;
		_updated = true;
	}
}

bool CSGizmo::getUpdatePass(CSUpdatePass& pass) const {
	if (!pass.remaining()) return false;
	loadTarget();
	if (_target) return pass.addPrecedence(_target);
	return true;
}

bool CSGizmo::update() {
	loadTarget();

	if (_updated) {
		_updated = false;
		return true;
	}
	if (_target) return _target->transformUpdated();
	if (_object->parent()) return _object->parent()->transformUpdated();
	return false;
}

bool CSGizmo::getTransform(float progress, CSMatrix& result) const {
	loadTarget();

	bool fromGround = false;
	if (_target) {
		if (!_target->getTransform(progress + _target->progress() - _object->progress(), _binding, result)) return false;
	}
	else{
		const CSSceneObject* parent = _object->parent();
		fromGround = _fromGround && _object->scene();
		if (parent) {
			if (!parent->getTransform(progress + parent->progress() - _object->progress(), _binding, result)) return false;
			fromGround &= !parent->fromGround();
		}
		else result = CSMatrix::Identity;
	}

	if (_position != CSVector3::Zero) result = CSMatrix::translation(_position) * result;
	if (_rotation != CSQuaternion::Identity) result = CSMatrix::rotationQuaternion(_rotation) * result;
	if (_scale != CSVector3::One) result = CSMatrix::scaling(_scale) * result;
	if (fromGround) result.m43 += _object->scene()->getZ(_object, result.translationVector());
	return true;
}

bool CSGizmo::getTransform(CSMatrix& result) const {
	return getTransform(_object->progress(), result);
}