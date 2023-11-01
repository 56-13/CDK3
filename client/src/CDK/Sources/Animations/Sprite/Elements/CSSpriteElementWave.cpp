#define CDK_IMPL

#include "CSSpriteElementWave.h"

#include "CSBuffer.h"

CSSpriteElementWave::CSSpriteElementWave(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	radius(CSAnimationFloat::factorWithBuffer(buffer)),
	thickness(CSAnimationFloat::factorWithBuffer(buffer)) 
{

}

CSSpriteElementWave::CSSpriteElementWave(const CSSpriteElementWave* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	radius(CSAnimationFloat::factorWithFactor(other->radius)),
	thickness(CSAnimationFloat::factorWithFactor(other->thickness))
{

}

int CSSpriteElementWave::resourceCost() const {
	int cost = sizeof(CSSpriteElementWave);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (radius) cost += radius->resourceCost();
	if (thickness) cost += thickness->resourceCost();
	return cost;
}

CSBoundingSphere CSSpriteElementWave::getSphere(float progress, int random) const {
	CSBoundingSphere sphere;
	sphere.center = position;
	if (x) sphere.center.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) sphere.center.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) sphere.center.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	sphere.radius = radius ? CSMath::max(radius->value(progress, CSRandom::toFloatSequenced(random, 3)), 0.0f) : 0;
	return sphere;
}

float CSSpriteElementWave::getThickness(float progress, int random) const {
	return thickness->value(progress, CSRandom::toFloatSequenced(random, 4));
}

bool CSSpriteElementWave::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSBoundingSphere sphere = getSphere(param.progress, param.random);

	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x - radius, sphere.center.y - radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x + radius, sphere.center.y - radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x - radius, sphere.center.y + radius, sphere.center.z), param.transform));
	result.append(CSVector3::transformCoordinate(CSVector3(sphere.center.x + radius, sphere.center.y + radius, sphere.center.z), param.transform));
	return true;
}

void CSSpriteElementWave::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (radius && radius->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

uint CSSpriteElementWave::showFlags() const {
	return CSSceneObject::ShowFlagDistortion;
}

void CSSpriteElementWave::draw(DrawParam& param) const {
	if (param.layer == CSInstanceLayerDistortion && radius && thickness) {
		CSBoundingSphere sphere = getSphere(param.progress, param.random);
		if (sphere.radius) {
			float thickness = getThickness(param.progress, param.random);
			if (thickness > 0) param.graphics->wave(sphere.center, sphere.radius * param.progress, thickness * (1 - param.progress));
		}
	}
}
