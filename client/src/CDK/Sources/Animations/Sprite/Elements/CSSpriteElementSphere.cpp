#define CDK_IMPL

#include "CSSpriteElementSphere.h"
#include "CSSprite.h"
#include "CSVertexArrays.h"
#include "CSBuffer.h"

CSSpriteElementSphere::CSSpriteElementSphere(CSBuffer* buffer) :
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	radius(CSAnimationFloat::factorWithBuffer(buffer)),
	collision(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer))
{

}

CSSpriteElementSphere::CSSpriteElementSphere(const CSSpriteElementSphere* other) :
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	radius(CSAnimationFloat::factorWithFactor(other->radius)),
	collision(other->collision),
	material(CSMaterialSource::materialWithMaterial(other->material))
{

}

int CSSpriteElementSphere::resourceCost() const {
	int cost = sizeof(CSSpriteElementSphere);
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (radius) cost += radius->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementSphere::preload() const {
	if (material) material->preload();
}

CSBoundingSphere CSSpriteElementSphere::getSphere(float progress, int random) const {
	CSBoundingSphere sphere;
	sphere.center = position;
	if (x) sphere.center.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) sphere.center.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) sphere.center.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	sphere.radius = radius ? CSMath::max(radius->value(progress, CSRandom::toFloatSequenced(random, 3)), 0.0f) : 0;
	return sphere;
}

bool CSSpriteElementSphere::addAABB(TransformParam& param, CSABoundingBox& result) const {
	CSBoundingSphere sphere = getSphere(param.progress, param.random);
	sphere.transform(param.transform);
	result.append(CSABoundingBox::fromSphere(sphere));
	return true;
}

void CSSpriteElementSphere::addCollider(TransformParam& param, CSCollider*& result) const {
	if (collision) {
		if (!result) result = CSCollider::colliderWithCapacity(4);
		CSBoundingSphere sphere = getSphere(param.progress, param.random);
		sphere.transform(param.transform);
		result->addObject(sphere);
	}
}

void CSSpriteElementSphere::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating()) || (radius && radius->animating())) {
		outflags |= CSSceneObject::UpdateFlagAABB;
	}
}

void CSSpriteElementSphere::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		CSBoundingSphere sphere = getSphere(param.progress, param.random);
		if (radius->animating()) {
			param.graphics->drawSphere(sphere.center, sphere.radius);
		}
		else {
			CSABoundingBox aabb;
			CSVertexArray* vertices = CSVertexArrays::getSphere(1, sphere.center, sphere.radius, CSRect::ZeroToOne, &aabb);
			param.graphics->drawVertices(vertices, CSPrimitiveTriangles, &aabb);
		}
	}
}
