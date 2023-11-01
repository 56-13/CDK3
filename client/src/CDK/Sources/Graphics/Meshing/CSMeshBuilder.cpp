#define CDK_IMPL

#include "CSMeshBuilder.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"

CSMeshBuilder::CSMeshBuilder(CSBuffer* buffer, bool withScene) :
	CSSceneObjectBuilder(buffer, withScene),
	geometryIndices(buffer->readArray<ushort>()),
	geometryName(buffer->readString()),
	animationIndices(buffer->readArray<ushort>()),
	animationName(buffer->readString()),
	skin(buffer->readArray<CSMaterialSource>()),
	loop(buffer),
	frameDivision(buffer->readShort()),
	collision(buffer->readBoolean())
{

}

CSMeshBuilder::CSMeshBuilder(const CSMeshBuilder* other) :
	CSSceneObjectBuilder(other),
	geometryIndices(other->geometryIndices),
	geometryName(other->geometryName),
	animationIndices(other->animationIndices),
	animationName(other->animationName),
	loop(other->loop),
	frameDivision(other->frameDivision),
	collision(other->collision)
{
	if (other->skin) {
		CSArray<CSMaterialSource>* skin = CSArray<CSMaterialSource>::arrayWithCapacity(other->skin->count());
		foreach(const CSMaterialSource*, m, other->skin) skin->addObject(CSMaterialSource::materialWithMaterial(m));
		this->skin = skin;
	}
}


const CSMeshGeometry* CSMeshBuilder::geometry() const {
	if (!geometryName) return NULL;
	const CSMesh* container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, geometryIndices));
	if (!container) return NULL;
	const CSMeshGeometry* geometry = container->geometry(geometryName);
	return geometry;
}

const CSMeshAnimation* CSMeshBuilder::animation() const {
	if (!animationName) return NULL;
	const CSArray<ushort>* indices = animationIndices ? animationIndices : geometryIndices;
	const CSMesh* container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, indices));
	if (!container) return NULL;
	const CSMeshAnimation* animation = container->animation(animationName);
	return animation;
}

uint CSMeshBuilder::showFlags() const {
	uint showFlags = 0;
	foreach (const CSMaterialSource*, material, skin) showFlags |= material->showFlags();
	return showFlags;
}

int CSMeshBuilder::resourceCost() const {
	int cost = sizeof(CSMeshBuilder) + resourceCostBase();
	if (geometryIndices) cost += sizeof(CSArray<ushort>) + geometryIndices->capacity() * sizeof(ushort);
	cost += geometryName.resourceCost();
	if (animationIndices) cost += sizeof(CSArray<ushort>) + animationIndices->capacity() * sizeof(ushort);
	cost += animationName.resourceCost();
	if (skin) {
		cost += sizeof(CSArray<CSMaterialSource>) + skin->capacity() * 8;
		foreach (const CSMaterialSource*, m, skin) cost += m->resourceCost();
	}
	return cost;
}

CSMeshObject* CSMeshBuilder::createObject() const {
	if (!geometryName) return NULL;
	const CSMesh* container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, geometryIndices));
	if (!container) return NULL;
	const CSMeshGeometry* geometry = container->geometry(geometryName);
	if (!geometry) return NULL;
	CSMeshObject* instance = new CSMeshObject(geometry, this);
	if (animationName) {
		if (animationIndices) container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, animationIndices));
		if (container) {
			const CSMeshAnimation* animation = container->animation(animationName);
			if (animation) instance->setAnimation(animation);
		}
	}
	instance->setSkin(skin);
	instance->loop = loop;
	instance->setFrameDivision(frameDivision);
	instance->collision = collision;
	return instance;
}

void CSMeshBuilder::updateObject(CSMeshObject*& instance) const {
	if (!geometryName) {
		release(instance);
		return;
	}
	const CSMesh* container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, geometryIndices));
	if (!container) {
		release(instance);
		return;
	}
	const CSMeshGeometry* geometry = container->geometry(geometryName);
	if (instance->geometry() != geometry) {
		release(instance);
		instance = createObject();
	}
	if (animationName) {
		if (animationIndices) container = static_assert_cast<CSMesh*>(CSResourcePool::sharedPool()->load(CSResourceTypeMesh, animationIndices));
		if (container) {
			const CSMeshAnimation* animation = container->animation(animationName);
			if (animation) instance->setAnimation(animation);
		}
	}
	else instance->setAnimation(NULL);

	instance->setSkin(skin);
	instance->loop = loop;
	instance->setFrameDivision(frameDivision);
	instance->collision = collision;
}

void CSMeshBuilder::preload() const {
	if (geometryName) CSResourcePool::sharedPool()->load(CSResourceTypeMesh, geometryIndices);
	if (animationIndices) CSResourcePool::sharedPool()->load(CSResourceTypeMesh, animationIndices);
	foreach (const CSMaterialSource*, material, skin) material->preload();
}
