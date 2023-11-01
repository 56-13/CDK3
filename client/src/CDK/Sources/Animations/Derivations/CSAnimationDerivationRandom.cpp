#define CDK_IMPL

#include "CSAnimationDerivationRandom.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSAnimationDerivationRandom::CSAnimationDerivationRandom(CSBuffer* buffer) : CSAnimationDerivation(buffer), loop(buffer->readBoolean()) {
    
}

CSAnimationDerivationRandom::CSAnimationDerivationRandom(const CSAnimationDerivationRandom* other) :
    CSAnimationDerivation(other),
    loop(other->loop) 
{

}

int CSAnimationDerivationRandom::resourceCost() const {
    int cost = sizeof(CSAnimationDerivationRandom);
    foreach (const CSAnimationFragment*, child, &children) cost += child->resourceCost();
    return cost;
}

//================================================================================================

CSAnimationObjectDerivationRandom::CSAnimationObjectDerivationRandom(const CSAnimationDerivationRandom* origin) : _origin(retain(origin)), _selection(NULL) {
    select();
}

CSAnimationObjectDerivationRandom::~CSAnimationObjectDerivationRandom() {
    _origin->release();
    release(_selection);
}

bool CSAnimationObjectDerivationRandom::select() {
    int totalWeight = 0;
    foreach (const CSAnimationFragment*, child, &_origin->children) {
        totalWeight += child->randomWeight;
    }
    if (totalWeight) {
        int randomWeight = randInt(0, totalWeight);
        totalWeight = 0;
        foreach (const CSAnimationFragment*, child, &_origin->children) {
            totalWeight += child->randomWeight;
            if (randomWeight <= totalWeight) {
                if (_parent) _selection->unlink();
                release(_selection);
                _selection = new CSAnimationObjectFragment(child);
                if (_parent) _selection->link(_parent->root(), _parent);
                break;
            }
        }
        return true;
    }
    return false;
}

void CSAnimationObjectDerivationRandom::spreadLink() {
    if (_selection) _selection->link(_parent->root(), _parent);
}

void CSAnimationObjectDerivationRandom::spreadUnlink() {
    if (_selection) _selection->unlink();
}

bool CSAnimationObjectDerivationRandom::addAABB(CSABoundingBox& result) const {
    return _selection && _selection->addAABB(result);
}

void CSAnimationObjectDerivationRandom::addCollider(CSCollider*& result) const {
    if (_selection) _selection->addCollider(result);
}

bool CSAnimationObjectDerivationRandom::getTransform(float progress, const string& name, CSMatrix& result) const {
    return _selection && _selection->getTransform(progress + _selection->progress() - _parent->progress(), name, result);
}

float CSAnimationObjectDerivationRandom::duration(CSSceneObject::DurationParam param, float duration) const {
    if (_origin->loop) return 0;
    float rtn = duration;
    if (_selection) rtn += _selection->duration(param);
    return rtn;
}

void CSAnimationObjectDerivationRandom::rewind() {
    select();
}

CSSceneObject::UpdateState CSAnimationObjectDerivationRandom::update(float delta, bool alive, uint inflags, uint& outflags) {
    if (!_selection) return CSSceneObject::UpdateStateStopped;

    CSSceneObject::UpdateState state = _selection->update(delta, alive, inflags, outflags);
        
    if (state == CSSceneObject::UpdateStateStopped) {
        if (alive && _origin->loop && select()) {
            _selection->rewind();
            return CSSceneObject::UpdateStateNone;
        }
        release(_selection);
        return _origin->finish ? CSSceneObject::UpdateStateStopped : CSSceneObject::UpdateStateNone;
    }
    return _origin->loop ? CSSceneObject::UpdateStateNone : state;
}

uint CSAnimationObjectDerivationRandom::show() {
    return _selection ? _selection->show() : 0;
}

void CSAnimationObjectDerivationRandom::draw(CSGraphics* graphics, CSInstanceLayer layer) {
    if (_selection) _selection->draw(graphics, layer);
}
