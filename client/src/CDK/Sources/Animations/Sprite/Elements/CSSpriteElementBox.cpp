#define CDK_IMPL

#include "CSSpriteElementBox.h"

#include "CSSprite.h"

#include "CSVertexArrays.h"

#include "CSBuffer.h"

CSSpriteElementBox::CSSpriteElementBox(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	xext(CSAnimationFloat::factorWithBuffer(buffer)),
	yext(CSAnimationFloat::factorWithBuffer(buffer)),
	zext(CSAnimationFloat::factorWithBuffer(buffer)),
	collision(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer))
{

}

CSSpriteElementBox::CSSpriteElementBox(const CSSpriteElementBox* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	xext(CSAnimationFloat::factorWithFactor(other->xext)),
	yext(CSAnimationFloat::factorWithFactor(other->yext)),
	zext(CSAnimationFloat::factorWithFactor(other->zext)),
	collision(other->collision),
	material(CSMaterialSource::materialWithMaterial(other->material))
{

}

int CSSpriteElementBox::resourceCost() const {
	int cost = sizeof(CSSpriteElementBox);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (xext) cost += xext->resourceCost();
	if (yext) cost += yext->resourceCost();
	if (zext) cost += zext->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementBox::preload() const {
	if (material) material->preload();
}

CSABoundingBox CSSpriteElementBox::getBox(float progress, int random) const {
	CSVector3 pos = position;
	if (x) pos.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) pos.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) pos.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	CSVector3 ext = CSVector3::Zero;
	if (xext) ext.x = CSMath::max(xext->value(progress, CSRandom::toFloatSequenced(random, 3)), 0.0f);
	if (yext) ext.y = CSMath::max(yext->value(progress, CSRandom::toFloatSequenced(random, 4)), 0.0f);
	if (zext) ext.z = CSMath::max(zext->value(progress, CSRandom::toFloatSequenced(random, 5)), 0.0f);
	return CSABoundingBox(pos - ext, pos + ext);
}

bool CSSpriteElementBox::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSABoundingBox box = getBox(param.progress, param.random);
	box.transform(param.transform);
	result.append(box);
	return true;
}

void CSSpriteElementBox::addCollider(TransformParam& param, CSCollider*& result) const {
	if (collision) {
		if (!result) result = CSCollider::colliderWithCapacity(4);
		CSOBoundingBox box = getBox(param.progress, param.random);
		box.transform(param.transform);
		result->addObject(box);
	}
}

void CSSpriteElementBox::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (xext && xext->animating()) || (yext && yext->animating()) || (zext && zext->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementBox::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSABoundingBox box = getBox(param.progress, param.random);

		CSVertexArray* vertices = CSVertexArrays::getBox(1, CSVector3(-0.5f), CSVector3(0.5f), CSRect::ZeroToOne, NULL);

		CSMatrix transform;
		CSMatrix::scaling(box.extent(), transform);
		transform.setTranslationVector(box.center());

		CSMatrix world = param.graphics->world();
		param.graphics->world() = transform * world;
		param.graphics->drawVertices(vertices, CSPrimitiveTriangles, &box);
		param.graphics->world() = world;
	}
}
