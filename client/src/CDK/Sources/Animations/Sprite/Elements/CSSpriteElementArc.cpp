#define CDK_IMPL

#include "CSSpriteElementArc.h"

#include "CSBuffer.h"

#include "CSSprite.h"

CSSpriteElementArc::CSSpriteElementArc(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	width(CSAnimationFloat::factorWithBuffer(buffer)),
	height(CSAnimationFloat::factorWithBuffer(buffer)),
	angle0(CSAnimationFloat::factorWithBuffer(buffer)),
	angle1(CSAnimationFloat::factorWithBuffer(buffer)),
	fill(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) 
{

}

CSSpriteElementArc::CSSpriteElementArc(const CSSpriteElementArc* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	width(CSAnimationFloat::factorWithFactor(other->width)),
	height(CSAnimationFloat::factorWithFactor(other->height)),
	angle0(CSAnimationFloat::factorWithFactor(other->angle0)),
	angle1(CSAnimationFloat::factorWithFactor(other->angle1)),
	fill(other->fill),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementArc::resourceCost() const {
	int cost = sizeof(CSSpriteElementArc);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (width) cost += width->resourceCost();
	if (height) cost += height->resourceCost();
	if (angle0) cost += angle0->resourceCost();
	if (angle1) cost += angle1->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementArc::preload() const {
	if (material) material->preload();
}

CSZRect CSSpriteElementArc::getRect(float progress, int random) const {
	CSZRect rect;
	rect.origin() = position;
	if (x) rect.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) rect.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) rect.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	rect.width = width ? CSMath::max(width->value(progress, CSRandom::toFloatSequenced(random, 3)), 0.0f) : 0;
	rect.height = height ? CSMath::max(height->value(progress, CSRandom::toFloatSequenced(random, 4)), 0.0f) : 0;
	rect.x -= rect.width * 0.5f;
	rect.y -= rect.height * 0.5f;
	return rect;
}

void CSSpriteElementArc::getAngle(float progress, int random, float& a0, float& a1) const {
	a0 = angle0 ? angle0->value(progress, CSRandom::toFloatSequenced(random, 5)) : 0;
	a1 = angle1 ? angle1->value(progress, CSRandom::toFloatSequenced(random, 6)) : FloatTwoPi;
}

bool CSSpriteElementArc::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSZRect rect = getRect(param.progress, param.random);
	result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));
	return true;
}

void CSSpriteElementArc::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (width && width->animating()) || (height && height->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementArc::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSZRect rect = getRect(param.progress, param.random);
		float a0, a1;
		getAngle(param.progress, param.random, a0, a1);
		param.graphics->drawArc(rect, a0, a1, fill);
	}
}
