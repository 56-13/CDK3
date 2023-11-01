#define CDK_IMPL

#include "CSSphereObject.h"

#include "CSVertexArrays.h"

#include "CSRandom.h"
#include "CSBuffer.h"

CSSphereBuilder::CSSphereBuilder(CSBuffer* buffer, bool withScene) :
	CSSceneObjectBuilder(buffer, withScene),
	radius(buffer->readFloat()),
	collision(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) {

}

CSSphereBuilder::CSSphereBuilder(const CSSphereBuilder* other) :
	CSSceneObjectBuilder(other),
	radius(other->radius),
	collision(other->collision),
	material(other->material) 
{

}

int CSSphereBuilder::resourceCost() const {
	int cost = sizeof(CSSphereBuilder);
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSphereBuilder::preload() const {
	if (material) material->preload();
}

//=============================================================================
CSSphereObject::CSSphereObject(const CSSphereBuilder* origin) :
	_origin(retain(origin)),
	_progress(0),
	_random(randInt()) {

}

CSSphereObject::~CSSphereObject() {
	_origin->release();
}

bool CSSphereObject::addAABB(CSABoundingBox& result) const {
	CSMatrix transform;
	if (getTransform(transform)) {
		CSABoundingBox aabb(CSVector3(-_origin->radius), CSVector3(_origin->radius));
		aabb.transform(transform);
		result.append(aabb);
		return true;
	}
	return false;
}

void CSSphereObject::addCollider(CSCollider*& result) const {
	CSMatrix transform;
	if (_origin->collision && getTransform(transform)) {
		if (!result) result = CSCollider::colliderWithCapacity(1);
		CSBoundingSphere sphere(CSVector3::Zero, _origin->radius);
		sphere.transform(transform);
		result->addObject(sphere);
	}
}

void CSSphereObject::onRewind() {
	_progress = 0;
	_random = randInt();
}

CSSceneObject::UpdateState CSSphereObject::onUpdate(float delta, bool alive, uint& flags) {
	_progress += delta;
	if (flags & UpdateFlagTransform) flags |= UpdateFlagAABB;
	return UpdateStateNone;
}

uint CSSphereObject::onShow() {
	return _origin->material ? _origin->material->showFlags() : 0;
}

void CSSphereObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	if (CSMaterialSource::apply(_origin->material, graphics, layer, _progress, _random, NULL, true)) {
		CSMatrix transform;
		if (getTransform(transform)) {
			CSABoundingBox aabb;
			CSVertexArray* sphere = CSVertexArrays::getSphere(1, CSVector3::Zero, _origin->radius, CSRect::ZeroToOne, &aabb);
			graphics->transform(transform);
			graphics->drawVertices(sphere, CSPrimitiveTriangles, &aabb);
		}
		graphics->pop();
	}
}