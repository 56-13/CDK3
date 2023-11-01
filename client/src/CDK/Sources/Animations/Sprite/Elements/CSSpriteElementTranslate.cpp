#define CDK_IMPL

#include "CSSpriteElementTranslate.h"

#include "CSBuffer.h"

CSSpriteElementTranslate::CSSpriteElementTranslate(CSBuffer* buffer) :
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)) 
{

}

CSSpriteElementTranslate::CSSpriteElementTranslate(const CSSpriteElementTranslate* other) :
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)) 
{

}

int CSSpriteElementTranslate::resourceCost() const {
	int cost = sizeof(CSSpriteElementTranslate);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	return cost;
}

CSVector3 CSSpriteElementTranslate::getTranslation(float progress, int random) const {
	CSVector3 translation = CSVector3::Zero;
	if (x) translation.x = x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) translation.y = y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) translation.z = z->value(progress, CSRandom::toFloatSequenced(random, 2));
	return translation;
}

bool CSSpriteElementTranslate::addAABB(TransformParam& param, CSABoundingBox& result) const {
	param.transform = CSMatrix::translation(getTranslation(param.progress, param.random)) * param.transform;
	return false;
}

void CSSpriteElementTranslate::addCollider(TransformParam& param, CSCollider*& result) const {
	param.transform = CSMatrix::translation(getTranslation(param.progress, param.random)) * param.transform;
}

void CSSpriteElementTranslate::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((x && x->animating()) || (y && y->animating()) || (z && z->animating())) param.inflags |= CSSceneObject::UpdateFlagTransform;
}

void CSSpriteElementTranslate::draw(DrawParam& param) const {
	param.graphics->translate(getTranslation(param.progress, param.random));
}
