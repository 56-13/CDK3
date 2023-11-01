#define CDK_IMPL

#include "CSSpriteElementLine.h"

#include "CSSprite.h"

#include "CSBuffer.h"

CSSpriteElementLine::CSSpriteElementLine(CSBuffer* buffer) :
	position0(buffer),
	x0(CSAnimationFloat::factorWithBuffer(buffer)),
	y0(CSAnimationFloat::factorWithBuffer(buffer)),
	z0(CSAnimationFloat::factorWithBuffer(buffer)),
	position1(buffer),
	x1(CSAnimationFloat::factorWithBuffer(buffer)),
	y1(CSAnimationFloat::factorWithBuffer(buffer)),
	z1(CSAnimationFloat::factorWithBuffer(buffer)),
	material(CSMaterialSource::materialWithBuffer(buffer))
{

}

CSSpriteElementLine::CSSpriteElementLine(const CSSpriteElementLine* other) :
	position0(other->position0),
	x0(CSAnimationFloat::factorWithFactor(other->x0)),
	y0(CSAnimationFloat::factorWithFactor(other->y0)),
	z0(CSAnimationFloat::factorWithFactor(other->z0)),
	position1(other->position1),
	x1(CSAnimationFloat::factorWithFactor(other->x1)),
	y1(CSAnimationFloat::factorWithFactor(other->y1)),
	z1(CSAnimationFloat::factorWithFactor(other->z1)),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementLine::resourceCost() const {
	int cost = sizeof(CSSpriteElementLine);
	if (x0) cost += x0->resourceCost();
	if (y0) cost += y0->resourceCost();
	if (z0) cost += z0->resourceCost();
	if (x1) cost += x1->resourceCost();
	if (y1) cost += y1->resourceCost();
	if (z1) cost += z1->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementLine::preload() const {
	if (material) material->preload();
}

void CSSpriteElementLine::getPosition(float progress, int random, CSVector3& p0, CSVector3& p1) const {
	p0 = position0;
	if (x0) p0.x += x0->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y0) p0.y += y0->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z0) p0.z += z0->value(progress, CSRandom::toFloatSequenced(random, 2));
	
	p1 = position1;
	if (x1) p1.x += x1->value(progress, CSRandom::toFloatSequenced(random, 3));
	if (y1) p1.y += y1->value(progress, CSRandom::toFloatSequenced(random, 4));
	if (z1) p1.z += z1->value(progress, CSRandom::toFloatSequenced(random, 5));
}

bool CSSpriteElementLine::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSVector3 p0, p1;
	getPosition(param.progress, param.random, p0, p1);
	CSVector3::transformCoordinate(p0, param.transform, p0);
	CSVector3::transformCoordinate(p1, param.transform, p1);
	result.append(p0);
	result.append(p1);
	return true;
}

void CSSpriteElementLine::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x0 && x0->animating()) || (y0 && y0->animating()) || (z0 && z0->animating()) || (x1 && x1->animating()) || (y1 && y1->animating()) || (z1 && z1->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementLine::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSVector3 p0, p1;
		getPosition(param.progress, param.random, p0, p1);
		param.graphics->drawLine(p0, p1);
	}
}
