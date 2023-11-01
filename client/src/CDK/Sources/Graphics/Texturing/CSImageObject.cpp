#define CDK_IMPL

#include "CSImageObject.h"

#include "CSResourcePool.h"

#include "CSRandom.h"
#include "CSBuffer.h"

#include "CSScene.h"

CSImageBuilder::CSImageBuilder(CSBuffer* buffer, bool withScene) :
	CSSceneObjectBuilder(buffer, withScene),
	indices(buffer->readArray<ushort>()),
	layer((CSInstanceBlendLayer)buffer->readByte()),
	blendMode((CSBlendMode)buffer->readByte()),
	billboard(buffer->readBoolean()),
	depthBias(buffer->readFloat())
{

}

CSImageBuilder::CSImageBuilder(const CSImageBuilder* other) :
	CSSceneObjectBuilder(other),
	indices(other->indices),
	billboard(other->billboard)
{

}

int CSImageBuilder::resourceCost() const {
	int cost = sizeof(CSImageBuilder);
	if (indices) cost += sizeof(CSArray<ushort>) + indices->capacity() * sizeof(ushort);
	return cost;
}

void CSImageBuilder::preload() const {
	CSResourcePool::sharedPool()->load(CSResourceTypeImage, indices);
}

//=============================================================================
CSImageObject::CSImageObject(const CSImageBuilder* origin) :
	_origin(retain(origin))
{

}

CSImageObject::~CSImageObject() {
	_origin->release();
}

bool CSImageObject::addAABB(CSABoundingBox& result) const {
	const CSImage* image = static_assert_cast<const CSImage*>(CSResourcePool::sharedPool()->load(CSResourceTypeImage, _origin->indices));

	CSMatrix transform;
	if (image && CSSceneObject::getTransform(transform)) {
		CSZRect rect = image->displayRect(CSVector3::Zero, CSAlignCenterMiddle);

		result.append(CSVector3::transformCoordinate(rect.leftTop(), transform));
		result.append(CSVector3::transformCoordinate(rect.rightTop(), transform));
		result.append(CSVector3::transformCoordinate(rect.leftBottom(), transform));
		result.append(CSVector3::transformCoordinate(rect.rightBottom(), transform));
		return true;
	}
	return false;
}

bool CSImageObject::getTransform(float progress, const string& name, CSMatrix& result) const {
	if (CSSceneObject::getTransform(progress, name, result)) {
		if (_origin->billboard && scene()) result = scene()->camera().view().billboard(&result);
		return true;
	}
	return false;
}

CSSceneObject::UpdateState CSImageObject::onUpdate(float delta, bool alive, uint& flags) {
	if (flags & (_origin->billboard ? UpdateFlagView | UpdateFlagTransform : UpdateFlagTransform)) flags |= UpdateFlagAABB;
	return UpdateStateNone;
}

void CSImageObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	if (layer == _origin->layer) {
		const CSImage* image = static_assert_cast<const CSImage*>(CSResourcePool::sharedPool()->load(CSResourceTypeImage, _origin->indices));

		CSMatrix transform;
		if (image && CSSceneObject::getTransform(transform)) {
			graphics->push();
			graphics->transform(transform);
			graphics->material().shader = CSMaterial::ShaderNoLight;
			graphics->material().blendMode = _origin->blendMode;
			graphics->material().depthBias = _origin->depthBias;
			graphics->drawImage(image, CSVector3::Zero, CSAlignCenterMiddle);
			graphics->pop();
		}
	}
}