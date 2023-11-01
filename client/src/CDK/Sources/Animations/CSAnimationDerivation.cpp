#define CDK_IMPL

#include "CSAnimationDerivation.h"
#include "CSAnimationDerivationMulti.h"
#include "CSAnimationDerivationLinked.h"
#include "CSAnimationDerivationEmission.h"
#include "CSAnimationDerivationRandom.h"
#include "CSAnimation.h"

#include "CSBuffer.h"

CSAnimationDerivation::CSAnimationDerivation() :
    children(4)
{
}

CSAnimationDerivation::CSAnimationDerivation(CSBuffer* buffer) : 
    children(buffer),
    finish(buffer->readBoolean())
{
    
}

CSAnimationDerivation::CSAnimationDerivation(const CSAnimationDerivation* other) :
    children(other->children.count()),
    finish(other->finish) 
{
    foreach (const CSAnimationFragment*, child, &other->children) children.addObject(CSAnimationFragment::fragmentWithFragment(child));
}

CSAnimationDerivation* CSAnimationDerivation::createWithBuffer(CSBuffer* buffer) {
    switch (buffer->readByte()) {
        case TypeMulti:
            return new CSAnimationDerivationMulti(buffer);
        case TypeLinked:
            return new CSAnimationDerivationLinked(buffer);
        case TypeEmission:
            return new CSAnimationDerivationEmission(buffer);
        case TypeRandom:
            return new CSAnimationDerivationRandom(buffer);
    }
    return NULL;
}

CSAnimationDerivation* CSAnimationDerivation::createWithDerivation(const CSAnimationDerivation* other) {
    if (other) {
        switch (other->type()) {
            case TypeMulti:
                return new CSAnimationDerivationMulti(static_cast<const CSAnimationDerivationMulti*>(other));
            case TypeLinked:
                return new CSAnimationDerivationLinked(static_cast<const CSAnimationDerivationLinked*>(other));
            case TypeEmission:
                return new CSAnimationDerivationEmission(static_cast<const CSAnimationDerivationEmission*>(other));
            case TypeRandom:
                return new CSAnimationDerivationRandom(static_cast<const CSAnimationDerivationRandom*>(other));
        }
    }
    return NULL;
}

bool CSAnimationObjectDerivation::link(CSAnimationObjectFragment* parent) {
    if (_parent && _parent != parent) return false;
    _parent = parent;
    spreadLink();
    return true;
}

void CSAnimationObjectDerivation::unlink() {
    if (_parent) {
        spreadUnlink();
        _parent = NULL;
    }
}
