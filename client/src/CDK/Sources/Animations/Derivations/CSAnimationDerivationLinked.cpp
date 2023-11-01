#define CDK_IMPL

#include "CSAnimationDerivationLinked.h"

#include "CSBuffer.h"

CSAnimationDerivationLinked::CSAnimationDerivationLinked(CSBuffer* buffer) : 
    CSAnimationDerivation(buffer), 
    loopCount(buffer->readShort()) 
{

}

CSAnimationDerivationLinked::CSAnimationDerivationLinked(const CSAnimationDerivationLinked* other) :
    CSAnimationDerivation(other),
    loopCount(other->loopCount)
{

}

int CSAnimationDerivationLinked::resourceCost() const {
    int cost = sizeof(CSAnimationDerivationLinked) + children.capacity() * 8;
    foreach (const CSAnimationFragment*, child, &children) cost += child->resourceCost();
    return cost;
}

//================================================================================================

CSAnimationObjectDerivationLinked::CSAnimationObjectDerivationLinked(const CSAnimationDerivationLinked* origin) :
    _origin(retain(origin)),
    _children(_origin->children.count()),
    _loop(0),
    _current(0),
    _count(0)
{
    foreach (const CSAnimationFragment*, child, &_origin->children) {
        CSAnimationObjectFragment* i = new CSAnimationObjectFragment(child);
        _children.addObject(i);
        i->release();
    }
    _count = _children.count() ? 1 : 0;
}

CSAnimationObjectDerivationLinked::~CSAnimationObjectDerivationLinked() {
    _origin->release();
}

void CSAnimationObjectDerivationLinked::clearChildren() {
    foreach (CSAnimationObjectFragment*, child, &_children) child->unlink();
    _children.removeAllObjects();
}

bool CSAnimationObjectDerivationLinked::addChild(CSAnimationObjectFragment* child) {
    if (!_children.containsObjectIdenticalTo(child) && (!_parent || child->link(_parent->root(), _parent))) {
        _children.addObject(child);
        return true;
    }
    return false;
}

bool CSAnimationObjectDerivationLinked::insertChild(int i, CSAnimationObjectFragment* child) {
    if (!_children.containsObjectIdenticalTo(child) && (!_parent || child->link(_parent->root(), _parent))) {
        _children.insertObject(i, child);
        return true;
    }
    return false;
}

bool CSAnimationObjectDerivationLinked::removeChild(CSAnimationObjectFragment* child) {
    child->retain();
    if (_children.removeObjectIdenticalTo(child)) {
        child->unlink();
        child->release();
        return true;
    }
    return false;
}

void CSAnimationObjectDerivationLinked::spreadLink() {
    foreach (CSAnimationObjectFragment*, i, &_children) i->link(_parent->root(), _parent);
}

void CSAnimationObjectDerivationLinked::spreadUnlink() {
    foreach (CSAnimationObjectFragment*, i, &_children) i->unlink();
}

bool CSAnimationObjectDerivationLinked::addAABB(CSABoundingBox& result) const {
    bool flag = false;
    int max = CSMath::min(_current + _count, _children.count());
    for (int i = _current; i < max; i++) {
        flag |= _children.objectAtIndex(i)->addAABB(result);
    }
    return flag;
}

void CSAnimationObjectDerivationLinked::addCollider(CSCollider*& result) const {
    int max = CSMath::min(_current + _count, _children.count());
    for (int i = _current; i < max; i++) {
        _children.objectAtIndex(i)->addCollider(result);
    }
}

bool CSAnimationObjectDerivationLinked::getTransform(float progress, const string& name, CSMatrix& result) const {
    int max = CSMath::min(_current + _count, _children.count());
    for (int i = _current; i < max; i++) {
        const CSAnimationObjectFragment* child = _children.objectAtIndex(i);
        if (child->getTransform(progress + child->progress() - _parent->progress(), name, result)) return true;
    }
    return false;
}

float CSAnimationObjectDerivationLinked::duration(CSSceneObject::DurationParam param, float duration) const {
    float rtn = 0;
    
    if (param == CSSceneObject::DurationParamMin) {
        foreach (const CSAnimationObjectFragment*, child, &_children) {
            rtn += child->duration(CSSceneObject::DurationParamMin);
        }
    }
    else {
        float min = 0;
        foreach (const CSAnimationObjectFragment*, child, &_children) {
            float max = min + child->duration(param);
            if (max > rtn) rtn = max;
            min += child->duration(CSSceneObject::DurationParamMin);
        }
    }
    if (_origin->loopCount) rtn *= _origin->loopCount;
    else rtn += duration;

    return rtn;
}

void CSAnimationObjectDerivationLinked::rewindProgress() {
    foreach (CSAnimationObjectFragment*, child, &_children) child->rewind();
    _current = 0;
    _count = _children.count() ? 1 : 0;
}

void CSAnimationObjectDerivationLinked::rewind() {
    rewindProgress();
    _loop = 0;
}

CSSceneObject::UpdateState CSAnimationObjectDerivationLinked::update(float delta, bool alive, uint inflags, uint& outflags) {
    _count = 0;
    
    for (int i = _current; i < _children.count(); i++) {
        switch (_children.objectAtIndex(i)->update(delta, alive, inflags, outflags)) {
            case CSSceneObject::UpdateStateStopped:
                if (i == _current) _current++;
                break;
            case CSSceneObject::UpdateStateFinishing:
                _count++;
                break;
            case CSSceneObject::UpdateStateAlive:
                _count++;
                return CSSceneObject::UpdateStateAlive;
        }
    }
    if (_current >= _children.count()) {
        _loop++;
        
        if (_origin->loopCount == 0) {
            if (alive) {
                rewindProgress();
                return CSSceneObject::UpdateStateNone;
            }
        }
        else if (_loop < _origin->loopCount) {
            rewindProgress();
            return CSSceneObject::UpdateStateAlive;
        }
        return _origin->finish ? CSSceneObject::UpdateStateStopped : CSSceneObject::UpdateStateNone;
    }
    return CSSceneObject::UpdateStateFinishing;
}


uint CSAnimationObjectDerivationLinked::show() {
    uint showFlags = 0;
    int max = CSMath::min(_current + _count, _children.count());
    for (int i = _current; i < max; i++) showFlags |= _children.objectAtIndex(i)->show();
    return showFlags;
}

void CSAnimationObjectDerivationLinked::draw(CSGraphics* graphics, CSInstanceLayer layer) {
    int max = CSMath::min(_current + _count, _children.count());
    for (int i = _current; i < max; i++) _children.objectAtIndex(i)->draw(graphics, layer);
}
