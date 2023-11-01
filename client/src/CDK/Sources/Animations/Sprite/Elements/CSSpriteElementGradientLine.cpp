#define CDK_IMPL

#include "CSSpriteElementGradientLine.h"

#include "CSSprite.h"

#include "CSBuffer.h"

CSSpriteElementGradientLine::CSSpriteElementGradientLine(CSBuffer* buffer) :
	position0(buffer),
	x0(CSAnimationFloat::factorWithBuffer(buffer)),
	y0(CSAnimationFloat::factorWithBuffer(buffer)),
	z0(CSAnimationFloat::factorWithBuffer(buffer)),
	color0(CSAnimationColor::colorWithBuffer(buffer)),
	position1(buffer),
	x1(CSAnimationFloat::factorWithBuffer(buffer)),
	y1(CSAnimationFloat::factorWithBuffer(buffer)),
	z1(CSAnimationFloat::factorWithBuffer(buffer)),
	color1(CSAnimationColor::colorWithBuffer(buffer)),
	material(CSMaterialSource::materialWithBuffer(buffer)) 
{

}

CSSpriteElementGradientLine::CSSpriteElementGradientLine(const CSSpriteElementGradientLine* other) :
	position0(other->position0),
	x0(CSAnimationFloat::factorWithFactor(other->x0)),
	y0(CSAnimationFloat::factorWithFactor(other->y0)),
	z0(CSAnimationFloat::factorWithFactor(other->z0)),
	color0(CSAnimationColor::colorWithColor(other->color0)),
	position1(other->position1),
	x1(CSAnimationFloat::factorWithFactor(other->x1)),
	y1(CSAnimationFloat::factorWithFactor(other->y1)),
	z1(CSAnimationFloat::factorWithFactor(other->z1)),
	color1(CSAnimationColor::colorWithColor(other->color1)),
	material(CSMaterialSource::materialWithMaterial(other->material))
{

}

int CSSpriteElementGradientLine::resourceCost() const {
	int cost = sizeof(CSSpriteElementGradientLine);
	if (x0) cost += x0->resourceCost();
	if (y0) cost += y0->resourceCost();
	if (z0) cost += z0->resourceCost();
	if (color0) cost += color0->resourceCost();
	if (x1) cost += x1->resourceCost();
	if (y1) cost += y1->resourceCost();
	if (z1) cost += z1->resourceCost();
	if (color1) cost += color1->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementGradientLine::preload() const {
	if (material) material->preload();
}

void CSSpriteElementGradientLine::getPosition(float progress, int random, CSVector3& p0, CSVector3& p1) const {
	p0 = position0;
	if (x0) p0.x += x0->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y0) p0.y += y0->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z0) p0.z += z0->value(progress, CSRandom::toFloatSequenced(random, 2));

	p1 = position1;
	if (x1) p1.x += x1->value(progress, CSRandom::toFloatSequenced(random, 3));
	if (y1) p1.y += y1->value(progress, CSRandom::toFloatSequenced(random, 4));
	if (z1) p1.z += z1->value(progress, CSRandom::toFloatSequenced(random, 5));
}

void CSSpriteElementGradientLine::getColor(float progress, int random, CSColor& c0, CSColor& c1) const {
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

bool CSSpriteElementGradientLine::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSVector3 p0, p1;
	getPosition(param.progress, param.random, p0, p1);
	CSVector3::transformCoordinate(p0, param.transform, p0);
	CSVector3::transformCoordinate(p1, param.transform, p1);
	result.append(p0);
	result.append(p1);
	return true;
}

void CSSpriteElementGradientLine::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x0 && x0->animating()) || (y0 && y0->animating()) || (z0 && z0->animating()) || (x1 && x1->animating()) || (y1 && y1->animating()) || (z1 && z1->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementGradientLine::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSVector3 p0, p1;
		getPosition(param.progress, param.random, p0, p1);
		CSColor c0, c1;
		getColor(param.progress, param.random, c0, c1);
		param.graphics->drawGradientLine(p0, c0, p1, c1);
	}
}
