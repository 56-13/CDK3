#define CDK_IMPL

#include "CSCapsuleObject.h"

#include "CSBoundingCapsule.h"

#include "CSVertexArrays.h"

#include "CSRandom.h"
#include "CSBuffer.h"

CSCapsuleBuilder::CSCapsuleBuilder(CSBuffer* buffer, bool withScene) :
	CSSceneObjectBuilder(buffer, withScene),
	height(buffer->readFloat()),
	radius(buffer->readFloat()),
	collision(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) {

}

CSCapsuleBuilder::CSCapsuleBuilder(const CSCapsuleBuilder* other) :
	CSSceneObjectBuilder(other),
	height(other->height),
	radius(other->radius),
	collision(other->collision),
	material(other->material) {

}

int CSCapsuleBuilder::resourceCost() const {
	int cost = sizeof(CSCapsuleBuilder);
	if (material) cost += material->resourceCost();
	return cost;
}

void CSCapsuleBuilder::preload() const {
	if (material) material->preload();
}

//=============================================================================
CSCapsuleObject::CSCapsuleObject(const CSCapsuleBuilder* origin) :
	_origin(retain(origin)),
	_progress(0),
	_random(randInt()) {

}

CSCapsuleObject::~CSCapsuleObject() {
	_origin->release();
}

bool CSCapsuleObject::addAABB(CSABoundingBox& result) const {
	CSMatrix transform;
	if (getTransform(transform)) {
		CSVector3 abbr(_origin->radius, _origin->radius, _origin->radius + _origin->height);
		CSABoundingBox aabb(-abbr, abbr);
		aabb.transform(transform);
		result.append(aabb);
		return true;
	}
	return false;
}

void CSCapsuleObject::addCollider(CSCollider*& result) const {
	CSMatrix transform;
	if (_origin->collision && getTransform(transform)) {
		if (!result) result = CSCollider::colliderWithCapacity(1);
		CSBoundingCapsule capsule(CSVector3(0, 0, _origin->height), CSVector3(0, 0, -_origin->height), _origin->radius);
		capsule.transform(transform);
		result->addObject(capsule);
	}
}

void CSCapsuleObject::onRewind() {
	_progress = 0;
	_random = randInt();
}

CSSceneObject::UpdateState CSCapsuleObject::onUpdate(float delta, bool alive, uint& flags) {
	_progress += delta;
	if (flags & UpdateFlagTransform) flags |= UpdateFlagAABB;
	return UpdateStateNone;
}

uint CSCapsuleObject::onShow() {
	return _origin->material ? _origin->material->showFlags() : 0;
}

void CSCapsuleObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	if (CSMaterialSource::apply(_origin->material, graphics, layer, _progress, _random, NULL, true)) {
		CSMatrix transform;
		if (getTransform(transform)) {
			CSABoundingBox aabb;
			CSVertexArray* sphere = CSVertexArrays::getCapsule(1, CSVector3::Zero, _origin->height, _origin->radius, CSRect::ZeroToOne, &aabb);
			graphics->transform(transform);
			graphics->drawVertices(sphere, CSPrimitiveTriangles, &aabb);
		}
		graphics->pop();
	}
}