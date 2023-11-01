#define CDK_IMPL

#include "CSSceneObject.h"

#include "CSBuffer.h"

#include "CSBoxObject.h"
#include "CSSphereObject.h"
#include "CSCapsuleObject.h"
#include "CSMeshBuilder.h"
#include "CSImageObject.h"
#include "CSParticle.h"
#include "CSTrail.h"
#include "CSSprite.h"
#include "CSDirectionalLightObject.h"
#include "CSPointLightObject.h"
#include "CSSpotLightObject.h"
#include "CSAnimation.h"
#include "CSCameraObject.h"

CSSceneObjectBuilder::CSSceneObjectBuilder(CSBuffer* buffer, bool withScene) {
	if (withScene) {
		dataId = buffer->readInt();
		located = buffer->readBoolean();
		if (buffer->readBoolean()) transform = CSGizmoData::dataWithBuffer(buffer);
		children = buffer->readArrayFunc<CSSceneObjectBuilder>([](CSBuffer* buffer) -> CSSceneObjectBuilder* {
			return CSSceneObjectBuilder::builderWithBuffer(buffer, true);
		});
	}
}

CSSceneObjectBuilder::CSSceneObjectBuilder(const CSSceneObjectBuilder* other) :
	dataId(other->dataId),
	located(other->located)
{
	if (other->transform) transform = CSGizmoData::dataWithData(other->transform);
	if (other->children) {
		children = CSArray<CSSceneObjectBuilder>::arrayWithCapacity(other->children->count());
		foreach(const CSSceneObjectBuilder*, child, other->children) children->addObject(CSSceneObjectBuilder::builderWithBuilder(child));
	}
}

int CSSceneObjectBuilder::resourceCostBase() const {
	int cost = 0;
	if (transform) cost += transform->resourceCost();
	if (children) {
		cost += sizeof(CSArray<CSSceneObjectBuilder>) + children->capacity() * 8;
		foreach (const CSSceneObjectBuilder*, child, children.value()) cost += child->resourceCost();
	}
	return cost;
}

CSSceneObjectBuilder* CSSceneObjectBuilder::createWithBuffer(CSBuffer* buffer, bool withScene) {
	switch (buffer->readByte()) {
		case CSSceneObject::TypeObject:
			return new CSSceneObjectBuilder(buffer, withScene);
		case CSSceneObject::TypeBox:
			return new CSBoxBuilder(buffer, withScene);
		case CSSceneObject::TypeSphere:
			return new CSSphereBuilder(buffer, withScene);
		case CSSceneObject::TypeCapsule:
			return new CSCapsuleBuilder(buffer, withScene);
		case CSSceneObject::TypeMesh:
			return new CSMeshBuilder(buffer, withScene);
		case CSSceneObject::TypeImage:
			return new CSImageBuilder(buffer, withScene);
		case CSSceneObject::TypeParticle:
			return new CSParticleBuilder(buffer, withScene);
		case CSSceneObject::TypeTrail:
			return new CSTrailBuilder(buffer, withScene);
		case CSSceneObject::TypeSprite:
			return new CSSpriteBuilder(buffer, withScene);
		case CSSceneObject::TypeDirectionalLight:
			return new CSDirectionalLightBuilder(buffer, withScene);
		case CSSceneObject::TypePointLight:
			return new CSPointLightBuilder(buffer, withScene);
		case CSSceneObject::TypeSpotLight:
			return new CSSpotLightBuilder(buffer, withScene);
		case CSSceneObject::TypeAnimation:
			return new CSAnimationBuilder(buffer, withScene, true);
		case CSSceneObject::TypeAnimationReference:
			return new CSAnimationBuilder(buffer, withScene, false);
		case CSSceneObject::TypeCamera:
			return new CSCameraBuilder(buffer, withScene);
	}
	return NULL;
}

CSSceneObjectBuilder* CSSceneObjectBuilder::createWithBuilder(const CSSceneObjectBuilder* other) {
	if (other) {
		switch (other->type()) {
			case CSSceneObject::TypeObject:
				return new CSSceneObjectBuilder(other);
			case CSSceneObject::TypeBox:
				return new CSBoxBuilder(static_cast<const CSBoxBuilder*>(other));
			case CSSceneObject::TypeSphere:
				return new CSSphereBuilder(static_cast<const CSSphereBuilder*>(other));
			case CSSceneObject::TypeCapsule:
				return new CSCapsuleBuilder(static_cast<const CSCapsuleBuilder*>(other));
			case CSSceneObject::TypeMesh:
				return new CSMeshBuilder(static_cast<const CSMeshBuilder*>(other));
			case CSSceneObject::TypeImage:
				return new CSImageBuilder(static_cast<const CSImageBuilder*>(other));
			case CSSceneObject::TypeParticle:
				return new CSParticleBuilder(static_cast<const CSParticleBuilder*>(other));
			case CSSceneObject::TypeTrail:
				return new CSTrailBuilder(static_cast<const CSTrailBuilder*>(other));
			case CSSceneObject::TypeSprite:
				return new CSSpriteBuilder(static_cast<const CSSpriteBuilder*>(other));
			case CSSceneObject::TypeDirectionalLight:
				return new CSDirectionalLightBuilder(static_cast<const CSDirectionalLightBuilder*>(other));
			case CSSceneObject::TypePointLight:
				return new CSPointLightBuilder(static_cast<const CSPointLightBuilder*>(other));
			case CSSceneObject::TypeSpotLight:
				return new CSSpotLightBuilder(static_cast<const CSSpotLightBuilder*>(other));
			case CSSceneObject::TypeAnimation:
				return new CSAnimationBuilder(static_cast<const CSAnimationBuilder*>(other));
			case CSSceneObject::TypeCamera:
				return new CSCameraBuilder(static_cast<const CSCameraBuilder*>(other));
		}
	}
	return NULL;
}
