#define CDK_IMPL

#include "CSSpriteElementMesh.h"

#include "CSSprite.h"

#include "CSResourcePool.h"
#include "CSBuffer.h"

CSSpriteElementMesh::CSSpriteElementMesh(CSBuffer* buffer) :
	builder(CSMeshBuilder::builderWithBuffer(buffer)),
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer))
{
	
}

CSSpriteElementMesh::CSSpriteElementMesh(const CSSpriteElementMesh* other) :
	builder(CSMeshBuilder::builderWithBuilder(other->builder)),
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z))
{

}

int CSSpriteElementMesh::resourceCost() const {
	int cost = sizeof(CSSpriteElementMesh);
	if (builder) cost += builder->resourceCost();
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	return cost;
}

void CSSpriteElementMesh::preload() const {
	if (builder) builder->preload();
}

CSVector3 CSSpriteElementMesh::getPosition(float progress, int random) const {
	CSVector3 pos = position;
	if (x) pos.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) pos.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) pos.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	return pos;
}

CSMeshObject* CSSpriteElementMesh::updateInstance(const CSSpriteObject* parent, float progress, float duration) const {
	if (!builder) return NULL;

	CSMeshObject* origin = parent->_meshInstances.objectForKey(this);
	CSMeshObject* current = origin;
	builder->updateObject(current);
	if (origin && origin != current) origin->unlink();
	if (current) {
		if (current != origin) parent->_meshInstances.setObject(this, current);

		CSScene* scene = const_cast<CSScene*>(parent->scene());
		if (current->scene() != scene) {
			current->unlink();
			if (scene) current->link(scene);
		}
	}
	else parent->_meshInstances.removeObject(this);

	if (current && current->animation()) {
		float animationDuration = current->animation()->duration();
		if (builder->loop.count) current->setProgress(builder->loop.getProgress(progress * builder->loop.count) * animationDuration);
		else current->setProgress(builder->loop.getProgress(duration * progress / animationDuration) * animationDuration);
	}
	return current;
}

bool CSSpriteElementMesh::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSMeshObject* instance = updateInstance(param.parent, param.progress, param.duration);

	if (instance) {
		CSABoundingBox laabb;
		if (instance->getAABB(laabb)) {
			CSVector3 corners[8];
			laabb.getCorners(corners);

			CSVector3 pos = getPosition(param.progress, param.random);

			if (pos != CSVector3::Zero) {
				CSMatrix transform_ = CSMatrix::translation(pos) * param.transform;
				for (int i = 0; i < 8; i++) {
					result.append(CSVector3::transformCoordinate(corners[i], transform_));
				}
			}
			else {
				for (int i = 0; i < 8; i++) {
					result.append(CSVector3::transformCoordinate(corners[i], param.transform));
				}
			}
			return true;
		}
	}
	return false;
}

void CSSpriteElementMesh::addCollider(TransformParam& param, CSCollider*& result) const {
	CSMeshObject* instance = updateInstance(param.parent, param.progress, param.duration);

	if (instance) {
		CSCollider* lcollider = instance->getCollider();
		if (lcollider) {
			CSVector3 pos = getPosition(param.progress, param.random);
			if (pos != CSVector3::Zero) lcollider->transform(CSMatrix::translation(pos) * param.transform);
			else lcollider->transform(param.transform);

			if (!result) result = lcollider;
			else result->addObjectsFromArray(lcollider);
		}
	}
}


bool CSSpriteElementMesh::getTransform(TransformParam& param, const string& name, CSMatrix& result) const {
	CSMeshObject* instance = updateInstance(param.parent, param.progress, param.duration);

	if (instance && instance->getNodeTransform(name, result)) {
		CSVector3 pos = getPosition(param.progress, param.random);

		if (pos != CSVector3::Zero) {
			CSMatrix transform = param.transform;
			transform.m41 += pos.x;
			transform.m42 += pos.y;
			transform.m43 += pos.z;
			result *= transform;
		}
		else result *= param.transform;

		return true;
	}
	return false;
}

void CSSpriteElementMesh::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	CSMeshObject* instance = updateInstance(param.parent, param.progress1, param.duration);

	if (instance) {
		if ((param.inflags & CSSceneObject::UpdateFlagTransform) || instance->animation() || (x && x->animating()) || (y && y->animating()) || (z && z->animating())) {
			outflags |= CSSceneObject::UpdateFlagTransform | CSSceneObject::UpdateFlagAABB;
		}
	}
}

uint CSSpriteElementMesh::showFlags() const {
	return builder ? builder->showFlags() : 0;
}

void CSSpriteElementMesh::draw(DrawParam& param) const {
	CSMeshObject* instance = updateInstance(param.parent, param.progress, param.duration);

	if (instance) {
		CSVector3 pos = getPosition(param.progress, param.random);
		if (pos != CSVector3::Zero) {
			CSMatrix prev = param.graphics->world();
			param.graphics->world() = CSMatrix::translation(pos) * prev;
			instance->drawIndirect(param.graphics, param.layer, param.parent->progress(), param.random);
			param.graphics->world() = prev;
		}
		else instance->drawIndirect(param.graphics, param.layer, param.parent->progress(), param.random);
	}
}
