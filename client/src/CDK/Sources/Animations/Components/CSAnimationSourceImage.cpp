#define CDK_IMPL

#include "CSAnimationSourceImage.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"

CSAnimationSourceImage::CSAnimationSourceImage(CSBuffer* buffer) :
	rootIndices(buffer->readArray<ushort>()),
	subIndices(buffer->readArray<ushort>()),
	material(buffer->readBoolean() ? CSMaterialSource::materialWithBuffer(buffer) : NULL),
	duration(buffer->readFloat()),
	loop(buffer)
{

}

CSAnimationSourceImage::CSAnimationSourceImage(const CSAnimationSourceImage * other) :
	rootIndices(other->rootIndices),
	subIndices(other->subIndices),
	material(CSMaterialSource::materialWithMaterial(other->material)),
	duration(other->duration),
	loop(other->loop)
{

}

int CSAnimationSourceImage::resourceCost() const {
	int cost = sizeof(CSAnimationSource);
	if (rootIndices) cost += sizeof(CSArray<ushort>) + rootIndices->capacity() * sizeof(ushort);
	if (subIndices) cost += sizeof(CSArray<ushort>) + subIndices->capacity() * sizeof(ushort);
	if (material) cost += material->resourceCost();
	return cost;
}

void CSAnimationSourceImage::preload() const {
	CSResourcePool::sharedPool()->load(CSResourceTypeImage, rootIndices);
	if (material) material->preload();
}

const CSImage* CSAnimationSourceImage::image(float progress) const {
	const CSImage* image = static_assert_cast<CSImage*>(CSResourcePool::sharedPool()->load(CSResourceTypeImage, rootIndices));
	if (!image) return NULL;
	if (!subIndices) return image;
	if (duration > 0) progress /= duration;
	progress = loop.getProgress(progress, NULL, NULL);
	return image->sub(CSMath::round(progress * (subIndices->count() - 1)));
}
