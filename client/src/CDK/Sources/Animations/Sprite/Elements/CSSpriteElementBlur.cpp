#define CDK_IMPL

#include "CSSpriteElementBlur.h"

#include "CSBuffer.h"

CSSpriteElementBlur::CSSpriteElementBlur(CSBuffer* buffer) :
	layer((CSInstanceBlendLayer)buffer->readByte()),
	mode((Mode)buffer->readByte())
{
	if (buffer->readBoolean()) frame = CSRect(buffer);
	
	switch (mode) {
        case ModeNormal:
            intensity = CSAnimationFloat::factorWithBuffer(buffer);
            break;
		case ModeDepth:
            intensity = CSAnimationFloat::factorWithBuffer(buffer);
			depthDistance = CSAnimationFloat::factorWithBuffer(buffer);
			depthRange = CSAnimationFloat::factorWithBuffer(buffer);
			break;
		case ModeDirection:
			directionX = CSAnimationFloat::factorWithBuffer(buffer);
			directionY = CSAnimationFloat::factorWithBuffer(buffer);
			break;
		case ModeCenter:
			centerX = CSAnimationFloat::factorWithBuffer(buffer);
			centerY = CSAnimationFloat::factorWithBuffer(buffer);
			centerRange = CSAnimationFloat::factorWithBuffer(buffer);
			break;
	}
}

CSSpriteElementBlur::CSSpriteElementBlur(const CSSpriteElementBlur* other) :
    layer(other->layer),
    mode(other->mode),
    frame(other->frame),
    intensity(CSAnimationFloat::factorWithFactor(other->intensity)),
    depthDistance(CSAnimationFloat::factorWithFactor(other->depthDistance)),
    depthRange(CSAnimationFloat::factorWithFactor(other->depthRange)),
    directionX(CSAnimationFloat::factorWithFactor(other->directionX)),
    directionY(CSAnimationFloat::factorWithFactor(other->directionY)),
    centerX(CSAnimationFloat::factorWithFactor(other->centerX)),
    centerY(CSAnimationFloat::factorWithFactor(other->centerY)),
    centerRange(CSAnimationFloat::factorWithFactor(other->centerRange))
{

}

int CSSpriteElementBlur::resourceCost() const {
    int cost = sizeof(CSSpriteElementBlur);
    if (intensity) cost += intensity->resourceCost();
    if (depthDistance) cost += depthDistance->resourceCost();
    if (depthRange) cost += depthRange->resourceCost();
    if (directionX) cost += directionX->resourceCost();
    if (directionY) cost += directionY->resourceCost();
    if (centerX) cost += centerX->resourceCost();
    if (centerY) cost += centerY->resourceCost();
    if (centerRange) cost += centerRange->resourceCost();
    return cost;
}

bool CSSpriteElementBlur::addAABB(TransformParam& param, CSABoundingBox& result) const {
	if (frame != CSRect::Zero) {
        result.append(CSVector3::transformCoordinate(frame.leftTop(), param.transform));
        result.append(CSVector3::transformCoordinate(frame.rightTop(), param.transform));
        result.append(CSVector3::transformCoordinate(frame.leftBottom(), param.transform));
        result.append(CSVector3::transformCoordinate(frame.rightBottom(), param.transform));
	}
	else {
        result.minimum = CSVector3(-100000);
        result.maximum = CSVector3(100000);
	}
    return true;
}

void CSSpriteElementBlur::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	if ((param.inflags & CSSceneObject::UpdateFlagTransform) && frame != CSRect::Zero) outflags |= CSSceneObject::UpdateFlagAABB;
}

void CSSpriteElementBlur::draw(DrawParam& param) const {
    if (layer == this->layer) {
        switch (mode) {
            case ModeNormal:
                if (intensity) {
                    float i = intensity->value(param.progress, CSRandom::toFloatSequenced(param.random, 0));
                    if (i > 0) {
                        if (frame != CSRect::Zero) param.graphics->blur(frame, i);
                        else param.graphics->blur(i);
                    }
                }
                break;
            case ModeDepth:
                if (intensity && depthRange) {
                    float d = depthDistance ?
                        depthDistance->value(param.progress, CSRandom::toFloatSequenced(param.random, 0)) :
                        CSVector3::distance(param.graphics->camera().position(), param.graphics->camera().target());
                    float r = depthRange->value(param.progress, CSRandom::toFloatSequenced(param.random, 1));
                    float i = intensity->value(param.progress, CSRandom::toFloatSequenced(param.random, 2));
                    if (r > 0 && i > 0) {
                        if (frame != CSRect::Zero) param.graphics->blurDepth(frame, d, r, i);
                        else param.graphics->blurDepth(d, r, i);
                    }
                }
                break;
            case ModeDirection:
                if (directionX || directionY) {
                    CSVector2 dir(
                        directionX ? directionX->value(param.progress, CSRandom::toFloatSequenced(param.random, 0)) : 0,
                        directionY ? directionY->value(param.progress, CSRandom::toFloatSequenced(param.random, 1)) : 0);

                    if (dir != CSVector2::Zero) {
                        if (frame != CSRect::Zero) param.graphics->blurDirection(frame, dir);
                        else param.graphics->blurDirection(dir);
                    }
                }
                break;
            case ModeCenter:
                if (centerRange) {
                    float range = centerRange->value(param.progress, CSRandom::toFloatSequenced(param.random, 2));

                    if (range > 0) {
                        CSVector2 center(
                            centerX ? centerX->value(param.progress, CSRandom::toFloatSequenced(param.random, 0)) : 0,
                            centerY ? centerY->value(param.progress, CSRandom::toFloatSequenced(param.random, 1)) : 0);

                        center.x += param.graphics->camera().width() * 0.5f;
                        center.y += param.graphics->camera().height() * 0.5f;

                        if (frame != CSRect::Zero) param.graphics->blurCenter(frame, center, range);
                        else param.graphics->blurCenter(center, range);
                    }
                }
                break;
        }
    }
}