#define CDK_IMPL

#include "CSSpriteElementLens.h"

#include "CSBuffer.h"

CSSpriteElementLens::CSSpriteElementLens(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	radius(CSAnimationFloat::factorWithBuffer(buffer)),
	convex(CSAnimationFloat::factorWithBuffer(buffer))
{

}

CSSpriteElementLens::CSSpriteElementLens(const CSSpriteElementLens* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	radius(CSAnimationFloat::factorWithFactor(other->radius)),
	convex(CSAnimationFloat::factorWithFactor(other->convex))
{

}

int CSSpriteElementLens::resourceCost() const {
	int cost = sizeof(CSSpriteElementLens);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (radius) cost += radius->resourceCost();
	if (convex) cost += convex->resourceCost();
	return cost;
}

CSBoundingSphere CSSpriteElementLens::getSphere(float progress, int random) const {
	CSBoundingSphere sphere;
	sphere.center = position;
	if (x) sphere.center.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) sphere.center.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) sphere.center.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	sphere.radius = radius ? radius->value(progress, CSRandom::toFloatSequenced(random, 3)) : 0;
	return sphere;
}

float CSSpriteElementLens::getConvex(float progress, int random) const {
	return convex->value(progress, CSRandom::toFloatSequenced(random, 4));
}

bool CSSpriteElementLens::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSBoundingSphere sphere = getSphere(param.progress, param.random);

	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x - radius, sphere.center.y - radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x + radius, sphere.center.y - radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x - radius, sphere.center.y + radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x + radius, sphere.center.y + radius, sphere.center.z), param.transform));
	return true;
}

void CSSpriteElementLens::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (radius && radius->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

uint CSSpriteElementLens::showFlags() const {
	return CSSceneObject::ShowFlagDistortion;
}

void CSSpriteElementLens::draw(DrawParam& param) const {
	if (param.layer == CSInstanceLayerDistortion && convex && radius) {
		CSBoundingSphere sphere = getSphere(param.progress, param.random);
		if (sphere.radius) {
			float convex = getConvex(param.progress, param.random);
			if (convex) param.graphics->lens(sphere.center, sphere.radius, convex);
		}
	}
}
