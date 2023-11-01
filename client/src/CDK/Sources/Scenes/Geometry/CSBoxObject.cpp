#define CDK_IMPL

#include "CSBoxObject.h"

#include "CSOBoundingBox.h"

#include "CSVertexArrays.h"

#include "CSRandom.h"
#include "CSBuffer.h"

CSBoxBuilder::CSBoxBuilder(CSBuffer* buffer, bool withScene) :
	CSSceneObjectBuilder(buffer, withScene),
	extent(buffer),
	collision(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer))
{

}

CSBoxBuilder::CSBoxBuilder(const CSBoxBuilder* other) : 
	CSSceneObjectBuilder(other), 
	extent(other->extent), 
	collision(other->collision),
	material(other->material) 
{

}

int CSBoxBuilder::resourceCost() const {
	int cost = sizeof(CSBoxBuilder);
	if (material) cost += material->resourceCost();
	return cost;
}

void CSBoxBuilder::preload() const {
	if (material) material->preload();
}

//=============================================================================
CSBoxObject::CSBoxObject(const CSBoxBuilder* origin) : 
	_origin(retain(origin)), 
	_progress(0), 
	_random(randInt()) 
{

}

CSBoxObject::~CSBoxObject() {
	_origin->release();
}

void CSBoxObject::onRewind() {
	_progress = 0;
	_random = randInt();
}

bool CSBoxObject::addAABB(CSABoundingBox& result) const {
	CSMatrix transform;
	if (getTransform(transform)) {
		CSABoundingBox box(-_origin->extent, _origin->extent);
		box.transform(transform);
		result.append(box);
		return true;
	}
	return false;
}

void CSBoxObject::addCollider(CSCollider*& result) const {
	CSMatrix transform;
	if (_origin->collision && getTransform(transform)) {
		if (!result) result = CSCollider::colliderWithCapacity(1);
		CSOBoundingBox box(CSVector3::Zero, _origin->extent, CSVector3::UnitX, CSVector3::UnitY, CSVector3::UnitZ);
		box.transform(transform);
		result->addObject(box);
	}
}

CSSceneObject::UpdateState CSBoxObject::onUpdate(float delta, bool alive, uint& flags) {
	_progress += delta;
	if (flags & UpdateFlagTransform) flags |= UpdateFlagAABB;
	return UpdateStateNone;
}

uint CSBoxObject::onShow() {
	return _origin->material ? _origin->material->showFlags() : 0;
}

void CSBoxObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	if (CSMaterialSource::apply(_origin->material, graphics, layer, _progress, _random, NULL, true)) {
		CSMatrix transform;
		if (getTransform(transform)) {
			CSABoundingBox aabb;
			CSVertexArray* box = CSVertexArrays::getBox(1, -CSVector3::One, CSVector3::One, CSRect::ZeroToOne, &aabb);
			graphics->transform(CSMatrix::scaling(_origin->extent) * transform);
			graphics->drawVertices(box, CSPrimitiveTriangles, &aabb);
		}
		graphics->pop();
	}
}