#define CDK_IMPL

#include "CSAnimationDerivationEmission.h"

#include "CSBuffer.h"

CSAnimationDerivationEmission::CSAnimationDerivationEmission(CSBuffer* buffer) :
    CSAnimationDerivation(buffer),
    delay(buffer->readFloat()),
    capacity(buffer->readShort()),
    prewarm(buffer->readBoolean())
{
}

CSAnimationDerivationEmission::CSAnimationDerivationEmission(const CSAnimationDerivationEmission* other) :
    CSAnimationDerivation(other),
    delay(other->delay),
    capacity(other->capacity),
    prewarm(other->prewarm)
{
        
}

int CSAnimationDerivationEmission::resourceCost() const {
    int cost = sizeof(CSAnimationDerivationEmission) + children.capacity() * 8;
    foreach (const CSAnimationFragment*, child, &children) cost += child->resourceCost();
    return cost;
}

//================================================================================================

CSAnimationObjectDerivationEmission::CSAnimationObjectDerivationEmission(const CSAnimationDerivationEmission* origin) : 
    _origin(retain(origin)), 
    _instances(origin->capacity),
    _index(0),
    _counter(origin->prewarm ? origin->capacity * origin->delay : 0)
{
    
}

CSAnimationObjectDerivationEmission::~CSAnimationObjectDerivationEmission() {
    _origin->release();
}

void CSAnimationObjectDerivationEmission::spreadLink() {
    foreach (CSAnimationObjectFragment*, i, &_instances) i->link(_parent->root(), _parent);
}

void CSAnimationObjectDerivationEmission::spreadUnlink() {
    foreach (CSAnimationObjectFragment*, i, &_instances) i->unlink();
}

bool CSAnimationObjectDerivationEmission::addAABB(CSABoundingBox& result) const {
    bool flag = false;
    foreach (const CSAnimationObjectFragment*, i, &_instances) flag |= i->addAABB(result);
    return flag;
}

void CSAnimationObjectDerivationEmission::addCollider(CSCollider*& result) const {
    foreach (const CSAnimationObjectFragment*, i, &_instances) i->addCollider(result);
}

bool CSAnimationObjectDerivationEmission::getTransform(float progress, const string& name, CSMatrix& result) const {
    return false;
}

float CSAnimationObjectDerivationEmission::duration(CSSceneObject::DurationParam param, float duration) const {
    float rtn = 0;
    foreach (const CSAnimationObjectFragment*, i, &_instances) {
        float d = i->duration(param);
        if (rtn < d) rtn = d;
    }
    rtn += duration;
    return rtn;
}

void CSAnimationObjectDerivationEmission::rewind() {
    _instances.removeAllObjects();
    _index = 0;
    _counter = _origin->prewarm ? _origin->capacity * _origin->delay : 0;
}

CSSceneObject::UpdateState CSAnimationObjectDerivationEmission::update(float delta, bool alive, uint inflags, uint& outflags) {
    int i = 0;
    while (i < _instances.count()) {
        CSAnimationObjectFragment* instance = _instances.objectAtIndex(i);
        
        if (instance->update(delta, alive, inflags, outflags) != CSSceneObject::UpdateStateStopped) i++;
        else {
            instance->unlink();
            _instances.removeObjectAtIndex(i);
        }
    }
    
    if (alive && _origin->delay) {
        _counter += delta;
        
        while (_counter >= _origin->delay) {
            _counter -= _origin->delay;
            
            if (_origin->capacity == 0 || _instances.count() < _origin->capacity) {
                CSMatrix transform;
                if (_parent->getTransform(_parent->progress() - _counter, NULL, transform)) {
                    const CSAnimationFragment* origin = _origin->children.objectAtIndex(_index);

                    CSAnimationObjectFragment* instance = new CSAnimationObjectFragment(origin);
                    instance->postTransform = transform;
                    instance->link(_parent->root(), _parent);
                    _instances.addObject(instance);
                    instance->release();
                    instance->rewind();
                    instance->update(_counter, alive, inflags, outflags);

                    _index = (_index + 1) % _origin->children.count();
                }
            }
        }
    }
    if (alive) return CSSceneObject::UpdateStateAlive;
    if (_instances.count()) return CSSceneObject::UpdateStateFinishing;
    return _origin->finish ? CSSceneObject::UpdateStateStopped : CSSceneObject::UpdateStateNone;
}

uint CSAnimationObjectDerivationEmission::show() {
    uint showFlags = 0;
    foreach (CSAnimationObjectFragment*, instance, &_instances) showFlags |= instance->show();
    return showFlags;
}

void CSAnimationObjectDerivationEmission::draw(CSGraphics* graphics, CSInstanceLayer layer) {
    foreach (CSAnimationObjectFragment*, instance, &_instances) instance->draw(graphics, layer);
}
