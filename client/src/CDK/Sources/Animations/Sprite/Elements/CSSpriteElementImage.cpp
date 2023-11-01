#define CDK_IMPL

#include "CSSpriteElementImage.h"

#include "CSSprite.h"

#include "CSResourcePool.h"
#include "CSBuffer.h"

CSSpriteElementImage::CSSpriteElementImage(CSBuffer* buffer) :
	source(CSAnimationSourceImage::sourceWithBuffer(buffer)),
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	align((CSAlign)buffer->readByte()),
	shadowType((ShadowType)buffer->readByte())
{
	switch (shadowType) {
		case ShadowTypeFlat:
			shadowDistance = buffer->readFloat();
			shadowFlatOffset = CSVector2(buffer);
			shadowFlatXFlip = buffer->readBoolean();
			shadowFlatYFlip = buffer->readBoolean();
			break;
		case ShadowTypeRotate:
			shadowDistance = buffer->readFloat();
			shadowRotateOffset = CSVector2(buffer);
			shadowRotateFlatness = buffer->readFloat();
			break;
	}
}

CSSpriteElementImage::CSSpriteElementImage(const CSSpriteElementImage* other) :
	source(CSAnimationSourceImage::sourceWithSource(other->source)),
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	align(other->align),
	shadowType(other->shadowType),
	shadowDistance(other->shadowDistance),
	shadowFlatOffset(other->shadowFlatOffset),
	shadowFlatXFlip(other->shadowFlatXFlip),
	shadowFlatYFlip(other->shadowFlatYFlip),
	shadowRotateOffset(other->shadowRotateOffset),
	shadowRotateFlatness(other->shadowRotateFlatness)
{

}

int CSSpriteElementImage::resourceCost() const {
	int cost = sizeof(CSSpriteElementImage);
	if (source) cost += source->resourceCost();
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	return cost;
}

void CSSpriteElementImage::preload() const {
	if (source) source->preload();
}

CSVector3 CSSpriteElementImage::getPosition(float progress, int random) const {
	CSVector3 pos = position;
	if (x) pos.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) pos.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) pos.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	return pos;
}

bool CSSpriteElementImage::addAABB(TransformParam& param, CSABoundingBox& result) const {
	if (source) {
		const CSImage* image = source->image(param.progress);

		if (image) {
			CSZRect rect = image->displayRect(getPosition(param.progress, param.random), align);

			result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
			result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
			result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
			result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));
			return true;
		}
	}
	return false;
}

void CSSpriteElementImage::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if (source) {
		const CSImage* image1 = source->image(param.progress1);
		if (image1 && ((param.inflags & CSSceneObject::UpdateFlagTransform) ||
			(x && x->animating()) ||
			(y && y->animating()) ||
			(z && z->animating()) ||
			image1 != source->image(param.progress0)))
		{
			outflags |= CSSceneObject::UpdateFlagAABB;
		}
	}
}

void CSSpriteElementImage::draw(DrawParam& param) const {
	if (source) {
		switch (param.layer) {
			case CSInstanceLayerNone:
			case CSInstanceLayerShadow:
			case CSInstanceLayerBase:
			case CSInstanceLayerBlendBottom:
			case CSInstanceLayerBlendMiddle:
			case CSInstanceLayerBlendTop:
				if (CSMaterialSource::apply(source->material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
					const CSImage* image = source->image(param.progress);
					if (image) param.graphics->drawImage(image, getPosition(param.progress, param.random), align);
				}
				break;
			case CSInstanceLayerShadow2D:
				if (shadowType) {
					const CSImage* image = source->image(param.progress);

					if (image) {
						CSVector3 scale;
						CSQuaternion rotation;
						CSVector3 translation;
						CSMatrix world = param.graphics->world();

						if (world.decompose(scale, rotation, translation)) {
							CSVector3 pos = getPosition(param.progress, param.random);
							pos.z += shadowDistance + translation.z;
							rotation.x = 0;
							rotation.y = 0;
							rotation.z = -rotation.z;
							rotation.normalize();

							translation.z = 0;

							param.graphics->world() = CSMatrix::scaling(scale) * CSMatrix::rotationQuaternion(rotation) * CSMatrix::translation(translation);

							switch (shadowType) {
								case ShadowTypeFlat:
									param.graphics->drawShadowFlatImage(image, pos, align, shadowFlatOffset, shadowFlatXFlip, shadowFlatYFlip);
									break;
								case ShadowTypeRotate:
									param.graphics->drawShadowRotateImage(image, pos, align, shadowRotateOffset, shadowRotateFlatness);
									break;
							}

							param.graphics->world() = world;
						}
					}
				}
				break;
		}
	}
}