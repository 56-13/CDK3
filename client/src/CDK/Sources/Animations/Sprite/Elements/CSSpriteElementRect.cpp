#define CDK_IMPL

#include "CSSpriteElementRect.h"
#include "CSSprite.h"
#include "CSBuffer.h"

CSSpriteElementRect::CSSpriteElementRect(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	width(CSAnimationFloat::factorWithBuffer(buffer)),
	height(CSAnimationFloat::factorWithBuffer(buffer)),
	align((CSAlign)buffer->readByte()),
	fill(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) 
{

}

CSSpriteElementRect::CSSpriteElementRect(const CSSpriteElementRect* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	width(CSAnimationFloat::factorWithFactor(other->width)),
	height(CSAnimationFloat::factorWithFactor(other->height)),
	align(other->align),
	fill(other->fill),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementRect::resourceCost() const {
	int cost = sizeof(CSSpriteElementRect);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (width) cost += width->resourceCost();
	if (height) cost += height->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementRect::preload() const {
	if (material) material->preload();
}

CSZRect CSSpriteElementRect::getRect(float progress, int random) const {
	CSZRect rect;
	rect.origin() = position;
	if (x) rect.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) rect.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) rect.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	rect.width = width ? CSMath::max(width->value(progress, CSRandom::toFloatSequenced(random, 3)), 0.0f) : 0;
	rect.height = height ? CSMath::max(height->value(progress, CSRandom::toFloatSequenced(random, 4)), 0.0f) : 0;
	
	CSGraphics::applyAlign(rect.origin(), rect.size(), align);

	return rect;
}

bool CSSpriteElementRect::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSZRect rect = getRect(param.progress, param.random);
	result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));
	return true;
}

void CSSpriteElementRect::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (width && width->animating()) || (height && height->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementRect::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSZRect rect = getRect(param.progress, param.random);
		param.graphics->drawRect(rect, fill);
	}
}
