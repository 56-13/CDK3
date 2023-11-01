#define CDK_IMPL

#include "CSSpriteElementRotate.h"

#include "CSBuffer.h"

CSSpriteElementRotate::CSSpriteElementRotate(CSBuffer* buffer) :
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)) 
{

}

CSSpriteElementRotate::CSSpriteElementRotate(const CSSpriteElementRotate* other) :
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z))
{

}

int CSSpriteElementRotate::resourceCost() const {
	int cost = sizeof(CSSpriteElementRotate);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	return cost;
}

CSMatrix CSSpriteElementRotate::getRotation(float progress, int random) const {
	CSVector3 rotation = CSVector3::Zero;
	if (x) rotation.x = x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) rotation.y = y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) rotation.z = z->value(progress, CSRandom::toFloatSequenced(random, 2));
	return CSMatrix::rotationYawPitchRoll(rotation.y, rotation.x, rotation.z);
}

bool CSSpriteElementRotate::addAABB(TransformParam& param, CSABoundingBox& result) const {
	param.transform = getRotation(param.progress, param.random) * param.transform;
	return false;
}

void CSSpriteElementRotate::addCollider(TransformParam& param, CSCollider*& result) const {
	param.transform = getRotation(param.progress, param.random) * param.transform;
}

void CSSpriteElementRotate::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((x && x->animating()) || (y && y->animating()) || (z && z->animating())) param.inflags |= CSSceneObject::UpdateFlagTransform;
}

void CSSpriteElementRotate::draw(DrawParam& param) const {
	param.graphics->transform(getRotation(param.progress, param.random));
}
