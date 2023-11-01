#define CDK_IMPL

#include "CSSpotLightObject.h"

#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSScene.h"

static const CSColor3 DefaultColor(5.0f, 5.0f, 5.0f);
static constexpr float DefaultAngle = 30 * FloatToRadians;
static constexpr float DefaultDispersion = 15 * FloatToRadians;

CSSpotLightBuilder::CSSpotLightBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
    angle(CSAnimationFloat::factorWithBuffer(buffer)),
    dispersion(CSAnimationFloat::factorWithBuffer(buffer)),
    color(CSAnimationColor::colorWithBuffer(buffer)),
    duration(buffer->readFloat()),
    loop(buffer),
    attenuation(buffer),
    castShadow(buffer->readBoolean()),
    shadow(buffer) 
{

}

CSSpotLightBuilder::CSSpotLightBuilder(const CSSpotLightBuilder* other) :
    CSSceneObjectBuilder(other),
    angle(CSAnimationFloat::factorWithFactor(other->angle)),
    dispersion(CSAnimationFloat::factorWithFactor(other->dispersion)),
    color(CSAnimationColor::colorWithColor(other->color)),
    duration(other->duration),
    loop(other->loop),
    attenuation(other->attenuation),
    castShadow(other->castShadow),
    shadow(other->shadow)
{

}

int CSSpotLightBuilder::resourceCost() const {
    int cost = sizeof(CSSpotLightBuilder) + resourceCostBase();
    if (color) cost += color->resourceCost();
    return cost;
}

//=======================================================================================

CSSpotLightObject::CSSpotLightObject(const CSSpotLightBuilder* origin) : 
    CSSceneObject(origin), 
    _origin(retain(origin)), 
    _progress(0),
    _random(randInt()) 
{

}

CSSpotLightObject::~CSSpotLightObject() {
    _origin->release();
}

void CSSpotLightObject::onRewind() {
    CSSceneObject::onRewind();

    _origin->release();
    _progress = 0;
    _random = randInt();
}

void CSSpotLightObject::capture(float& angle, float& dispersion, CSColor3& color) const {
    if (_origin->duration > 0) {
        int randomSeq0, randomSeq1;
        float cp = _origin->loop.getProgress(_progress / _origin->duration, &randomSeq0, &randomSeq1);
        randomSeq0 *= 5;
        randomSeq1 *= 5;
        CSColor cr(
            CSRandom::toFloatSequenced(_random, randomSeq0, randomSeq1, cp),
            CSRandom::toFloatSequenced(_random, randomSeq0 + 1, randomSeq1 + 1, cp),
            CSRandom::toFloatSequenced(_random, randomSeq0 + 2, randomSeq1 + 2, cp),
            1.0f);

        color = _origin->color ? (CSColor3)_origin->color->value(cp, cr, DefaultColor) : DefaultColor;
        angle = _origin->angle ? _origin->angle->value(cp, CSRandom::toFloatSequenced(_random, randomSeq0 + 3, randomSeq1 + 3, cp)) : DefaultAngle;
        dispersion = _origin->dispersion ? _origin->dispersion->value(cp, CSRandom::toFloatSequenced(_random, randomSeq0 + 4, randomSeq1 + 4, cp)) : DefaultDispersion;
    }
    else {
        CSColor cr(
            CSRandom::toFloatSequenced(_random, 0),
            CSRandom::toFloatSequenced(_random, 1),
            CSRandom::toFloatSequenced(_random, 2),
            1.0f);

        color = _origin->color ? (CSColor3)_origin->color->value(1, cr, DefaultColor) : DefaultColor;
        angle = _origin->angle ? _origin->angle->value(1, CSRandom::toFloatSequenced(_random, 3)) : DefaultAngle;
        dispersion = _origin->dispersion ? _origin->dispersion->value(1, CSRandom::toFloatSequenced(_random, 4)) : DefaultDispersion;
    }
}

CSSceneObject::UpdateState CSSpotLightObject::onUpdate(float delta, bool alive, uint& flags) {
    _progress += delta;

    CSScene* scene = this->scene();

    if (scene && scene->lightSpace()) {
        CSMatrix trans;
        if (getTransform(_progress, NULL, trans)) {
            float angle, dispersion;
            CSColor3 color;
            capture(angle, dispersion, color);

            CSSpotLight light(
                this,
                trans.translationVector(),
                trans.forward(),
                angle,
                dispersion,
                color,
                _origin->attenuation,
                _origin->castShadow,
                _origin->shadow);

            scene->lightSpace()->setSpotLight(light);
        }
    }

    if (_progress < _origin->duration * _origin->loop.count) {
        return alive ? UpdateStateAlive : UpdateStateFinishing;
    }
    else {
        return _origin->loop.finish ? UpdateStateStopped : UpdateStateNone;
    }
}
