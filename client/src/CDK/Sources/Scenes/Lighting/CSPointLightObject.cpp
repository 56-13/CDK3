#define CDK_IMPL

#include "CSPointLightObject.h"

#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSScene.h"

static const CSColor3 DefaultColor(5.0f, 5.0f, 5.0f);

CSPointLightBuilder::CSPointLightBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
	color(CSAnimationColor::colorWithBuffer(buffer)), 
	duration(buffer->readFloat()),
	loop(buffer),
	attenuation(buffer),
	castShadow(buffer->readBoolean()),
	shadow(buffer)
{

}

CSPointLightBuilder::CSPointLightBuilder(const CSPointLightBuilder* other) :
    CSSceneObjectBuilder(other),
    color(CSAnimationColor::colorWithColor(other->color)),
    duration(other->duration),
    loop(other->loop),
    attenuation(other->attenuation),
    castShadow(other->castShadow),
    shadow(other->shadow)
{

}

int CSPointLightBuilder::resourceCost() const {
	int cost = sizeof(CSPointLightBuilder) + resourceCostBase();
	if (color) cost += color->resourceCost();
	return cost;
}

CSPointLightObject::CSPointLightObject(const CSPointLightBuilder* origin) : 
    CSSceneObject(origin), 
    _origin(retain(origin)), 
    _progress(0),
    _random(randInt()) 
{

}

CSPointLightObject::~CSPointLightObject() {
    _origin->release();
}

void CSPointLightObject::onRewind() {
    CSSceneObject::onRewind();

	_progress = 0;
	_random = randInt();
}

void CSPointLightObject::capture(CSColor3& color) const {
    if (_origin->duration > 0) {
        int randomSeq0, randomSeq1;
        float cp = _origin->loop.getProgress(_progress / _origin->duration, &randomSeq0, &randomSeq1);
        randomSeq0 *= 3;
        randomSeq1 *= 3;
        CSColor cr(
            CSRandom::toFloatSequenced(_random, randomSeq0, randomSeq1, cp),
            CSRandom::toFloatSequenced(_random, randomSeq0 + 1, randomSeq1 + 1, cp),
            CSRandom::toFloatSequenced(_random, randomSeq0 + 2, randomSeq1 + 2, cp),
            1.0f);

        color = _origin->color ? (CSColor3)_origin->color->value(cp, cr, DefaultColor) : DefaultColor;
    }
    else {
        CSColor cr(
            CSRandom::toFloatSequenced(_random, 0),
            CSRandom::toFloatSequenced(_random, 1),
            CSRandom::toFloatSequenced(_random, 2),
            1.0f);

        color = _origin->color ? (CSColor3)_origin->color->value(1, cr, DefaultColor) : DefaultColor;
    }
}

CSSceneObject::UpdateState CSPointLightObject::onUpdate(float delta, bool alive, uint& flags) {
	_progress += delta;

    CSScene* scene = this->scene();

    if (scene && scene->lightSpace()) {
        CSMatrix trans;
        if (getTransform(_progress, NULL, trans)) {
            CSColor3 color;
            capture(color);

            CSPointLight light(
                this,
                trans.translationVector(),
                color,
                _origin->attenuation,
                _origin->castShadow,
                _origin->shadow);

            scene->lightSpace()->setPointLight(light);
        }
    }

    if (_progress < _origin->duration * _origin->loop.count) {
        return alive ? UpdateStateAlive : UpdateStateFinishing;
    }
    else {
        return _origin->loop.finish ? UpdateStateStopped : UpdateStateNone;
    }
}
