#define CDK_IMPL

#include "CSSpriteElementGradientRoundRect.h"

#include "CSSprite.h"

#include "CSBuffer.h"

CSSpriteElementGradientRoundRect::CSSpriteElementGradientRoundRect(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	width(CSAnimationFloat::factorWithBuffer(buffer)),
	height(CSAnimationFloat::factorWithBuffer(buffer)),
	radius(CSAnimationFloat::factorWithBuffer(buffer)),
	color0(CSAnimationColor::colorWithBuffer(buffer)),
	color1(CSAnimationColor::colorWithBuffer(buffer)),
	align((CSAlign)buffer->readByte()),
	corner((CSCorner)buffer->readByte()),
	fill(buffer->readBoolean()),
	horizontal(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer)) 
{

}

CSSpriteElementGradientRoundRect::CSSpriteElementGradientRoundRect(const CSSpriteElementGradientRoundRect* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	width(CSAnimationFloat::factorWithFactor(other->width)),
	height(CSAnimationFloat::factorWithFactor(other->height)),
	radius(CSAnimationFloat::factorWithFactor(other->radius)),
	color0(CSAnimationColor::colorWithColor(other->color0)),
	color1(CSAnimationColor::colorWithColor(other->color1)),
	align(other->align),
	corner(other->corner),
	fill(other->fill),
	horizontal(other->horizontal),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementGradientRoundRect::resourceCost() const {
	int cost = sizeof(CSSpriteElementGradientRoundRect);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (width) cost += width->resourceCost();
	if (height) cost += height->resourceCost();
	if (radius) cost += radius->resourceCost();
	if (color0) cost += color0->resourceCost();
	if (color1) cost += color1->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementGradientRoundRect::preload() const {
	if (material) material->preload();
}

CSZRect CSSpriteElementGradientRoundRect::getRect(float progress, int random) const {
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

float CSSpriteElementGradientRoundRect::getRadius(float progress, int random) const {
	return radius ? radius->value(progress, CSRandom::toFloatSequenced(random, 5)) : 0;
}

void CSSpriteElementGradientRoundRect::getColor(float progress, int random, CSColor& c0, CSColor& c1) const {
	if (color0) {
		CSColor cr0(
			CSRandom::toFloatSequenced(random, 6),
			CSRandom::toFloatSequenced(random, 7),
			CSRandom::toFloatSequenced(random, 8),
			CSRandom::toFloatSequenced(random, 9));

		c0 = color0->value(progress, cr0, CSColor::White);
	}
	else c0 = CSColor::White;

	if (color1) {
		CSColor cr1(
			CSRandom::toFloatSequenced(random, 10),
			CSRandom::toFloatSequenced(random, 11),
			CSRandom::toFloatSequenced(random, 12),
			CSRandom::toFloatSequenced(random, 13));

		c1 = color1->value(progress, cr1, CSColor::White);
	}
	else c1 = CSColor::White;
}

bool CSSpriteElementGradientRoundRect::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSZRect rect = getRect(param.progress, param.random);
	result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
	result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));
	return true;
}

void CSSpriteElementGradientRoundRect::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (width && width->animating()) || (height && height->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementGradientRoundRect::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSZRect rect = getRect(param.progress, param.random);
		float r = getRadius(param.progress, param.random);
		CSColor c0, c1;
		getColor(param.progress, param.random, c0, c1);
		if (horizontal) param.graphics->drawGradientRoundRectH(rect, r, c0, c1, fill, corner);
		else param.graphics->drawGradientRoundRectV(rect, r, c0, c1, fill, corner);
	}
}
