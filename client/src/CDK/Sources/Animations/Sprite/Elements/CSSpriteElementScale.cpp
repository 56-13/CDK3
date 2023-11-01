#define CDK_IMPL

#include "CSSpriteElementScale.h"

#include "CSBuffer.h"

CSSpriteElementScale::CSSpriteElementScale(CSBuffer* buffer) :
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	each(buffer->readBoolean())
{

}

CSSpriteElementScale::CSSpriteElementScale(const CSSpriteElementScale* other) :
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	each(other->each)
{

}

int CSSpriteElementScale::resourceCost() const {
	int cost = sizeof(CSSpriteElementScale);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	return cost;
}

CSVector3 CSSpriteElementScale::getScale(float progress, int random) const {
	CSVector3 scale = CSVector3::One;
	
	if (x) scale.x = x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (each) {
		if (y) scale.y = y->value(progress, CSRandom::toFloatSequenced(random, 1));
		if (z) scale.z = z->value(progress, CSRandom::toFloatSequenced(random, 2));
	}
	else scale.y = scale.z = scale.x;

	return scale;
}

bool CSSpriteElementScale::addAABB(TransformParam& param, CSABoundingBox& result) const {
	param.transform = CSMatrix::scaling(getScale(param.progress, param.random)) * param.transform;
	return false;
}

void CSSpriteElementScale::addCollider(TransformParam& param, CSCollider*& result) const {
	param.transform = CSMatrix::scaling(getScale(param.progress, param.random)) * param.transform;
}

void CSSpriteElementScale::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((x && x->animating()) || (y && y->animating()) || (z && z->animating())) param.inflags |= CSSceneObject::UpdateFlagTransform;
}

void CSSpriteElementScale::draw(DrawParam& param) const {
	param.graphics->scale(getScale(param.progress, param.random));
}
