#define CDK_IMPL

#include "CSAnimationDerivationMulti.h"

int CSAnimationDerivationMulti::resourceCost() const {
    int cost = sizeof(CSAnimationDerivationMulti);
    foreach (const CSAnimationFragment*, child, &children) cost += child->resourceCost();
    return cost;
}

//================================================================================================

CSAnimationObjectDerivationMulti::CSAnimationObjectDerivationMulti(const CSAnimationDerivationMulti* origin) :
    _origin(retain(origin)),
    _children(_origin->children.count())
{
    foreach (const CSAnimationFragment*, child, &_origin->children) {
        CSAnimationObjectFragment* i = new CSAnimationObjectFragment(child);
        _children.addObject(i);
        i->release();
    }
}

CSAnimationObjectDerivationMulti::~CSAnimationObjectDerivationMulti() {
    _origin->release();
}

void CSAnimationObjectDerivationMulti::clearChildren() {
    foreach (CSAnimationObjectFragment*, child, &_children) child->unlink();
    _children.removeAllObjects();
}

bool CSAnimationObjectDerivationMulti::addChild(CSAnimationObjectFragment* child) {
    if (!_children.containsObjectIdenticalTo(child) && (!_parent || child->link(_parent->root(), _parent))) {
        _children.addObject(child);
        return true;
    }
    return false;
}

bool CSAnimationObjectDerivationMulti::removeChild(CSAnimationObjectFragment* child) {
    child->retain();
    if (_children.removeObjectIdenticalTo(child)) {
        if (_parent) child->unlink();
        child->release();
        return true;
    }
    return false;
}

void CSAnimationObjectDerivationMulti::spreadLink() {
    foreach (CSAnimationObjectFragment*, i, &_children) i->link(_parent->root(), _parent);
}

void CSAnimationObjectDerivationMulti::spreadUnlink() {
    foreach (CSAnimationObjectFragment*, i, &_children) i->unlink();
}

bool CSAnimationObjectDerivationMulti::addAABB(CSABoundingBox& result) const {
    bool flag = false;
    foreach (const CSAnimationObjectFragment*, i, &_children) flag |= i->addAABB(result);
    return flag;
}

void CSAnimationObjectDerivationMulti::addCollider(CSCollider*& result) const {
    foreach (const CSAnimationObjectFragment*, child, &_children) child->addCollider(result);
}

bool CSAnimationObjectDerivationMulti::getTransform(float progress, const string& name, CSMatrix& result) const {
    foreach (const CSAnimationObjectFragment*, child, &_children) {
        if (child->getTransform(progress + child->progress() - _parent->progress(), name, result)) return true;
    }
    return false;
}

float CSAnimationObjectDerivationMulti::duration(CSSceneObject::DurationParam param, float duration) const {
    float min = 0;
    float closing = 0;
    
    if (param == CSSceneObject::DurationParamMin) {
        foreach (const CSAnimationObjectFragment*, child, &_children) {
            float d = child->duration(param);
            if (child->origin()->closing) {
                if (closing < d) closing = d;
            }
            else {
                if (min < d) min = d;
            }
        }
        return min + closing;
    }
    else {
        float max = 0;
        
        foreach (const CSAnimationObjectFragment*, child, &_children) {
            float maxd = child->duration(param);
            if (child->origin()->closing) {
                if (closing < maxd) closing = maxd;
            }
            else {
                float mind = child->duration(CSSceneObject::DurationParamMin);
                if (min < mind) min = mind;
                if (max < maxd) max = maxd;
            }
        }
        return CSMath::max(min + closing, max);
    }
}

void CSAnimationObjectDerivationMulti::rewind() {
    foreach (CSAnimationObjectFragment*, child, &_children) child->rewind();
}

CSSceneObject::UpdateState CSAnimationObjectDerivationMulti::update(float delta, bool alive, uint inflags, uint& outflags) {
    CSSceneObject::UpdateState rtn = _origin->finish ? CSSceneObject::UpdateStateStopped : CSSceneObject::UpdateStateNone;
    foreach (CSAnimationObjectFragment*, child, &_children) {
        switch (child->update(delta, alive, inflags, outflags)) {
            case CSSceneObject::UpdateStateAlive:
                rtn = CSSceneObject::UpdateStateAlive;
                break;
            case CSSceneObject::UpdateStateFinishing:
                if (rtn == CSSceneObject::UpdateStateStopped) rtn = CSSceneObject::UpdateStateFinishing;
                break;
        }
    }
    return rtn;
}

uint CSAnimationObjectDerivationMulti::show() {
    uint showFlags = 0;
    foreach (CSAnimationObjectFragment*, instance, &_children) showFlags |= instance->show();
    return showFlags;
}

void CSAnimationObjectDerivationMulti::draw(CSGraphics* graphics, CSInstanceLayer layer) {
    foreach (CSAnimationObjectFragment*, instance, &_children) instance->draw(graphics, layer);
}
