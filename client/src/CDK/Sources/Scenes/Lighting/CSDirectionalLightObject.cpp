#define CDK_IMPL

#include "CSDirectionalLightObject.h"

#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSScene.h"

static const CSColor3 DefaultColor(5.0f, 5.0f, 5.0f);

CSDirectionalLightBuilder::CSDirectionalLightBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
    color(CSAnimationColor::colorWithBuffer(buffer)),
    duration(buffer->readFloat()),
    loop(buffer),
    castShadow(buffer->readBoolean()),
    castShadow2D(buffer->readBoolean()),
    shadow(buffer) 
{

}

CSDirectionalLightBuilder::CSDirectionalLightBuilder(const CSDirectionalLightBuilder* other) :
    CSSceneObjectBuilder(other),
    color(CSAnimationColor::colorWithColor(other->color)),
    duration(other->duration),
    loop(other->loop),
    castShadow(other->castShadow),
    castShadow2D(other->castShadow2D),
    shadow(other->shadow)
{

}

int CSDirectionalLightBuilder::resourceCost() const {
    int cost = sizeof(CSDirectionalLightBuilder) + resourceCostBase();
    if (color) cost += color->resourceCost();
    return cost;
}

//================================================================================================

CSDirectionalLightObject::CSDirectionalLightObject(const CSDirectionalLightBuilder* origin) : 
    CSSceneObject(origin), 
    _origin(retain(origin)), 
    _progress(0),
    _random(randInt()) 
{

}

CSDirectionalLightObject::~CSDirectionalLightObject() {
    _origin->release();
}

void CSDirectionalLightObject::onRewind() {
    CSSceneObject::onRewind();

    _progress = 0;
    _random = randInt();
}

void CSDirectionalLightObject::capture(CSColor3& color) const {
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

CSSceneObject::UpdateState CSDirectionalLightObject::onUpdate(float delta, bool alive, uint& flags) {
    _progress += delta;

    CSScene* scene = this->scene();

    if (scene && scene->lightSpace()) {
        CSMatrix trans;
        if (getTransform(_progress, NULL, trans)) {
            CSColor3 color;
            capture(color);

            CSDirectionalLight light(
                this,
                trans.translationVector(),
                color,
                _origin->castShadow,
                _origin->castShadow2D,
                _origin->shadow);

            scene->lightSpace()->setDirectionalLight(light);
        }
    }

    if (_progress < _origin->duration * _origin->loop.count) {
        return alive ? UpdateStateAlive : UpdateStateFinishing;
    }
    else {
        return _origin->loop.finish ? UpdateStateStopped : UpdateStateNone;
    }
}
