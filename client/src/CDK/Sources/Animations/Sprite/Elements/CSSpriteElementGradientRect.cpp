#define CDK_IMPL

#include "CSSpriteElementGradientRect.h"

#include "CSSprite.h"

#include "CSBuffer.h"

CSSpriteElementGradientRect::CSSpriteElementGradientRect(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	width(CSAnimationFloat::factorWithBuffer(buffer)),
	height(CSAnimationFloat::factorWithBuffer(buffer)),
	color0(CSAnimationColor::colorWithBuffer(buffer)),
	color1(CSAnimationColor::colorWithBuffer(buffer)),
	align((CSAlign)buffer->readByte()),
	fill(buffer->readBoolean()),
	horizontal(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) 
{

}

CSSpriteElementGradientRect::CSSpriteElementGradientRect(const CSSpriteElementGradientRect* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	width(CSAnimationFloat::factorWithFactor(other->width)),
	height(CSAnimationFloat::factorWithFactor(other->height)),
	color0(CSAnimationColor::colorWithColor(other->color0)),
	color1(CSAnimationColor::colorWithColor(other->color1)),
	align(other->align),
	fill(other->fill),
	horizontal(other->horizontal),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementGradientRect::resourceCost() const {
	int cost = sizeof(CSSpriteElementGradientRect);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (width) cost += width->resourceCost();
	if (height) cost += height->resourceCost();
	if (color0) cost += color0->resourceCost();
	if (color1) cost += color1->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementGradientRect::preload() const {
	if (material) material->preload();
}

CSZRect CSSpriteElementGradientRect::getRect(float progress, int random) const {
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

void CSSpriteElementGradientRect::getColor(float progress, int random, CSColor& c0, CSColor& c1) const {
	if (color0) {
		CSColor cr0(
			CSRandom::toFloatSequenced(random, 5),
			CSRandom::toFloatSequenced(random, 6),
			CSRandom::toFloatSequenced(random, 7),
			CSRandom::toFloatSequenced(random, 8));

		c0 = color0->value(progress, cr0, CSColor::White);
	}
	else c0 = CSColor::White;

	if (color1) {
		CSColor cr1(
			CSRandom::toFloatSequenced(random, 9),
			CSRandom::toFloatSequenced(random, 10),
			CSRandom::toFloatSequenced(random, 11),
			CSRandom::toFloatSequenced(random, 12));

		c1 = color1->value(progress, cr1, CSColor::White);
	}
	else c1 = CSColor::White;
}

bool CSSpriteElementGradientRect::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSZRect rect = getRect(param.progress, param.random);
	result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));
	return true;
}

void CSSpriteElementGradientRect::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (width && width->animating()) || (height && height->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementGradientRect::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSZRect rect = getRect(param.progress, param.random);
		CSColor c0, c1;
		getColor(param.progress, param.random, c0, c1);
		if (horizontal) param.graphics->drawGradientRectH(rect, c0, c1, fill);
		else  param.graphics->drawGradientRectV(rect, c0, c1, fill);
	}
}
