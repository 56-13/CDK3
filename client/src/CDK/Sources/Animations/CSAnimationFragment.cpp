#define CDK_IMPL

#include "CSAnimationFragment.h"
#include "CSAnimationDerivation.h"
#include "CSAnimation.h"

#include "CSResourcePool.h"
#include "CSFile.h"
#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSScene.h"

static constexpr float FacingInterval = 0.016f;

CSAnimationFragment::Sound::Sound(const string& subpath) :
    subpath(subpath)
{
}

CSAnimationFragment::Sound::Sound(CSBuffer* buffer) :
    subpath(buffer->readString()),
    volume(buffer->readFloat()),
    control((CSAudioControl)buffer->readByte()),
    loop(buffer->readByte()),
    priority(buffer->readByte()),
    perspective(buffer->readBoolean()),
    latency(buffer->readFloat()),
    duration(buffer->readFloat()),
    duplication(buffer->readFloat()),
    stop(buffer->readBoolean())
{
}

CSAnimationFragment::Sound::Sound(const Sound* other) :
    subpath(other->subpath),
    volume(other->volume),
    duration(other->duration),
    latency(other->latency),
    control(other->control),
    loop(other->loop),
    priority(other->priority),
    stop(other->stop) 
{
}

CSAnimationFragment::CSAnimationFragment(CSBuffer* buffer) : 
    name(buffer->readString()),
    key(buffer->readInt()),
    x(CSAnimationFloat::factorWithBuffer(buffer)),
    y(CSAnimationFloat::factorWithBuffer(buffer)),
    z(CSAnimationFloat::factorWithBuffer(buffer)),
    radial(CSAnimationFloat::factorWithBuffer(buffer)),
    tangential(CSAnimationFloat::factorWithBuffer(buffer)),
    tangentialAngle(tangential ? CSAnimationFloat::factorWithBuffer(buffer) : NULL),
    pathDegree(buffer->readFloat()),
    pathLoop(buffer),
    pathUsingSpeed(buffer->readBoolean()),
    billboard(buffer->readBoolean()),
    reverse(buffer->readBoolean()),
    facing(buffer->readBoolean()),
    rotationX(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationY(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationZ(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationDuration(buffer->readFloat()),
    rotationLoop(buffer),
    scaleX(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleY(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleZ(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleDuration(buffer->readFloat()),
    scaleLoop(buffer),
    scaleEach(buffer->readBoolean()),
    pivot(buffer->readBoolean()),
    stencil(buffer->readBoolean()),
    closing(buffer->readBoolean()),
    randomWeight(buffer->readByte()),
    target(buffer->readByte()),
    binding(buffer->readString()),
    duration(buffer->readFloat()),
    latency(buffer->readFloat()),
    sound(buffer->readBoolean() ? Sound::soundWithBuffer(buffer) : NULL),
    localeVisible(buffer->readBoolean()),
    _locales(retain(buffer->readArray<string>())),
    substance(CSSceneObjectBuilder::builderWithBuffer(buffer, false)),
    derivation(CSAnimationDerivation::derivationWithBuffer(buffer))
{
    
}

CSAnimationFragment::CSAnimationFragment(const CSAnimationFragment* other) :
    name(other->name),
    key(other->key),
    x(CSAnimationFloat::factorWithFactor(other->x)),
    y(CSAnimationFloat::factorWithFactor(other->y)),
    z(CSAnimationFloat::factorWithFactor(other->z)),
    radial(CSAnimationFloat::factorWithFactor(other->radial)),
    tangential(CSAnimationFloat::factorWithFactor(other->tangential)),
    tangentialAngle(CSAnimationFloat::factorWithFactor(other->tangentialAngle)),
    pathDegree(other->pathDegree),
    pathLoop(other->pathLoop),
    pathUsingSpeed(other->pathUsingSpeed),
    billboard(other->billboard),
    reverse(other->reverse),
    facing(other->facing),
    rotationX(CSAnimationFloat::factorWithFactor(other->rotationX)),
    rotationY(CSAnimationFloat::factorWithFactor(other->rotationY)),
    rotationZ(CSAnimationFloat::factorWithFactor(other->rotationZ)),
    rotationDuration(other->rotationDuration),
    rotationLoop(other->rotationLoop),
    scaleX(CSAnimationFloat::factorWithFactor(other->scaleX)),
    scaleY(CSAnimationFloat::factorWithFactor(other->scaleY)),
    scaleZ(CSAnimationFloat::factorWithFactor(other->scaleZ)),
    scaleDuration(other->scaleDuration),
    scaleLoop(other->scaleLoop),
    scaleEach(other->scaleEach),
    pivot(other->pivot),
    stencil(other->stencil),
    closing(other->closing),
    randomWeight(other->randomWeight),
    target(other->target),
    binding(other->binding),
    duration(other->duration),
    latency(other->latency),
    sound(other->sound ? Sound::soundWithSound(other->sound) : NULL),
    localeVisible(other->localeVisible),
    _locales(other->_locales ? new CSArray<string>(other->_locales) : NULL),
    substance(CSSceneObjectBuilder::builderWithBuilder(other->substance)),
    derivation(CSAnimationDerivation::derivationWithDerivation(other->derivation))
{

}

CSAnimationFragment::~CSAnimationFragment() {
    release(_locales);
}

void CSAnimationFragment::addLocale(const string& locale) {
    if (!_locales) _locales = new CSArray<string>(4);
    else if (_locales->containsObject(locale)) return;
    _locales->addObject(locale);
    _localeCurrentMark = 0;
}

void CSAnimationFragment::removeLocale(const string& locale) {
    if (_locales && _locales->removeObject(locale)) {
        if (!_locales->count()) {
            _locales->release();
            _locales = NULL;
        }
        _localeCurrentMark = 0;
    }
}

void CSAnimationFragment::clearLocales() {
    release(_locales);
}

bool CSAnimationFragment::checkLocale() const {
    bool visible = localeVisible;

    if (_locales) {
        uint localeMark = CSLocaleString::localeMark();
        if (_localeCurrentMark != localeMark) {
            _localeCurrentMark = localeMark;

            const string& locale = CSLocaleString::locale();
            _localeCurrentContains = (locale && _locales->containsObject(locale));
        }
        if (_localeCurrentContains) visible = !visible;
    }
    return visible;
}

static bool hasPivot(const CSAnimationFragment* current) {
    if (current->pivot) return true;
    if (current->derivation) {
        foreach (const CSAnimationFragment*, child, &current->derivation->children) {
            if (hasPivot(child)) return true;
        }
    }
    return false;
}

bool CSAnimationFragment::hasPivot() const {
    return ::hasPivot(this);
}

static bool hasSubstance(const CSAnimationFragment* current) {
    if (current->substance) return true;
    if (current->derivation) {
        foreach (const CSAnimationFragment*, child, &current->derivation->children) {
            if (hasSubstance(child)) return true;
        }
    }
    return false;
}

bool CSAnimationFragment::hasSubstance() const {
    return ::hasSubstance(this);
}

int CSAnimationFragment::resourceCost() const {
    int cost = sizeof(CSAnimationFragment);
    cost += name.resourceCost();
    if (x) cost += x->resourceCost();
    if (y) cost += y->resourceCost();
    if (z) cost += z->resourceCost();
    if (radial) cost += radial->resourceCost();
    if (tangential) cost += tangential->resourceCost();
    if (tangentialAngle) cost += tangentialAngle->resourceCost();
    if (rotationX) cost += rotationX->resourceCost();
    if (rotationY) cost += rotationY->resourceCost();
    if (rotationZ) cost += rotationZ->resourceCost();
    if (scaleX) cost += scaleX->resourceCost();
    if (scaleY) cost += scaleY->resourceCost();
    if (scaleZ) cost += scaleZ->resourceCost();
    cost += binding.resourceCost();
    if (sound) {
        cost += sizeof(Sound);
        cost += sound->subpath.resourceCost();
    }
    if (_locales) {
        cost += sizeof(CSArray<string>) + _locales->capacity() * 8;
        foreach (const string&, locale, _locales) cost += locale.resourceCost();
    }
    if (substance) cost += substance->resourceCost();
    if (derivation) cost += derivation->resourceCost();
    return cost;
}

void CSAnimationFragment::preload() const {
    if (substance) substance->preload();
    if (derivation) {
        foreach (const CSAnimationFragment*, child, &derivation->children) child->preload();
    }
}

//===========================================================================================================

CSAnimationObjectFragment::CSAnimationObjectFragment(const CSAnimationFragment* origin) :
    _root(NULL),
    _parent(NULL),
    _origin(retain(origin)),
    _progress(-origin->latency),
    _visible(false),
    _soundHandle(0),
    _soundCounter(0),
    _substance(NULL),
    _derivation(NULL),
    postTransform(CSMatrix::Identity) 
{
    resetRandoms();
    if (_origin->substance) {
        _substance = _origin->substance->createObject();
        _substance->link(this);
    }
    if (_origin->derivation) {
        _derivation = _origin->derivation->createObject();
        _derivation->link(this);
    }
    _visible = !_origin->closing && _origin->latency <= 0 && (_origin->key & _root->keyFlags()) == _origin->key && _origin->checkLocale();
}

CSAnimationObjectFragment::~CSAnimationObjectFragment() {
    _origin->release();
    release(_substance);
    release(_derivation);
    stopSound();
}

bool CSAnimationObjectFragment::setSubstance(CSSceneObject* substance) {
    if (_substance != substance && (!substance || substance->link(this))) {
        if (_substance) {
            _substance->unlink();
            _substance->release();
        }
        _substance = retain(substance);
        return true;
    }
    return false;
}

bool CSAnimationObjectFragment::setDerivation(CSAnimationObjectDerivation* derivation) {
    if (_derivation != derivation && (!derivation || derivation->link(this))) {
        if (_derivation) {
            _derivation->unlink();
            _derivation->release();
        }
        _derivation = retain(derivation);
        return true;
    }
    return false;
}

bool CSAnimationObjectFragment::link(CSAnimationObject* root, CSAnimationObjectFragment* parent) {
    if ((_root && _root != root) || (_parent && _parent != parent)) return false;
    _root = root;
    _parent = parent;
    if (_substance) _substance->link(this);
    if (_derivation) _derivation->link(this);
    return true;
}

void CSAnimationObjectFragment::unlink() {
    if (_root || _parent) {
        if (_substance) _substance->unlink();
        if (_derivation) _derivation->unlink();
        _root = NULL;
        _parent = NULL;
    }
}

bool CSAnimationObjectFragment::addAABB(CSABoundingBox& result) const {
    bool flag = false;
    if (_visible) {
        if (_substance) flag |= _substance->addAABB(result);
        if (_derivation) flag |= _derivation->addAABB(result);
    }
    return flag;
}

void CSAnimationObjectFragment::addCollider(CSCollider*& result) const {
    if (_visible) {
        if (_substance) _substance->addCollider(result);
        if (_derivation) _derivation->addCollider(result);
    }
}

static CSVector3 tangentialDirection(const CSVector3& dir, float angle) {
    CSVector3 rtn(dir.y, -dir.x, 0);
    CSVector3::cross(rtn, dir, rtn);
    rtn.normalize();
    CSQuaternion rotation = CSQuaternion::rotationAxis(dir, angle);
    CSVector3::transform(rtn, rotation, rtn);
    return rtn;
}

bool CSAnimationObjectFragment::getTransform(float progress, const string& name, CSMatrix& result) const {
    if (name) {
        if (_origin->pivot) {
            if (_substance && _substance->getTransform(progress + _substance->progress() - _progress, name, result)) return true;
            if (_origin->name == name) goto local;
        }
        return _derivation && _derivation->getTransform(progress, name, result);
    }

    local:

    if (!_parent) {
        if (!_root->getTransform(progress, NULL, result)) return false;
    }
    else {
        float parentProgress = progress + _parent->_progress - _progress;
        if (_origin->binding && _parent->_substance) {
            if (!_parent->_substance->getTransform(parentProgress, _origin->binding, result)) return false;
        }
        else if (!_parent->getTransform(parentProgress, NULL, result)) return false;
    }

    if (progress < 0) progress = 0;

    result = postTransform * result;

    CSQuaternion rotation;
    CSVector3 scale;

    if (!_origin->rotationX && !_origin->rotationY && _origin->rotationZ) {
        rotation = CSQuaternion::Identity;
    }
    else if (_origin->rotationDuration) {
        int rotation0RandomSeq;
        int rotation1RandomSeq;
        float rotationProgress = _origin->rotationLoop.getProgress(progress / _origin->rotationDuration, &rotation0RandomSeq, &rotation1RandomSeq);

        float r = CSRandom::toFloatSequenced(_rotationRandom, rotation0RandomSeq, rotation1RandomSeq, rotationProgress);

        rotation = CSQuaternion::rotationYawPitchRoll(
            _origin->rotationY ? _origin->rotationY->value(rotationProgress, r) * FloatToRadians : 0,
            _origin->rotationX ? _origin->rotationX->value(rotationProgress, r) * FloatToRadians : 0,
            _origin->rotationZ ? _origin->rotationZ->value(rotationProgress, r) * FloatToRadians : 0);
    }
    else {
        float r = CSRandom::toFloat(_rotationRandom);

        rotation = CSQuaternion::rotationYawPitchRoll(
            _origin->rotationY ? _origin->rotationY->value(1, r) * FloatToRadians : 0,
            _origin->rotationX ? _origin->rotationX->value(1, r) * FloatToRadians : 0,
            _origin->rotationZ ? _origin->rotationZ->value(1, r) * FloatToRadians : 0);
    }

    if (!_origin->scaleX && !_origin->scaleY && _origin->scaleZ) {
        scale = CSVector3::One;
    }
    else if (_origin->scaleDuration) {
        int scale0RandomSeq;
        int scale1RandomSeq;
        float scaleProgress = _origin->scaleLoop.getProgress(progress / _origin->scaleDuration, &scale0RandomSeq, &scale1RandomSeq);

        float r = CSRandom::toFloatSequenced(_scaleRandom, scale0RandomSeq, scale1RandomSeq, scaleProgress);

        scale.x = _origin->scaleX ? _origin->scaleX->value(scaleProgress, r) : 1;
        scale.y = _origin->scaleEach ? (_origin->scaleY ? _origin->scaleY->value(scaleProgress, r) : 1) : scale.x;
        scale.z = _origin->scaleEach ? (_origin->scaleZ ? _origin->scaleZ->value(scaleProgress, r) : 1) : scale.x;
    }
    else {
        float r = CSRandom::toFloat(_scaleRandom);

        scale.x = _origin->scaleX ? _origin->scaleX->value(1, r) : 1;
        scale.y = _origin->scaleEach ? (_origin->scaleY ? _origin->scaleY->value(1, r) : 1) : scale.x;
        scale.z = _origin->scaleEach ? (_origin->scaleZ ? _origin->scaleZ->value(1, r) : 1) : scale.x;
    }

    if (scale.x <= 0 || scale.y <= 0 || scale.z <= 0) return false;

    CSMatrix transforms[2];
    transforms[0] = result;
    if (_origin->target == 0) transforms[1] = result;
    else if (!_root->getTargetTransform(_origin->target, progress + _root->progress() - _progress, transforms[1])) return false;

    CSVector3 points[2];

    const CSCamera* camera = _root->scene() ? &_root->scene()->camera() : NULL;

    for (int i = 0; i < 2; i++) {
        points[i] = CSVector3(
            _origin->x ? _origin->x->value(i) : 0,
            _origin->y ? _origin->y->value(i) : 0,
            _origin->z ? _origin->z->value(i) : 0);

        if (points[i] == CSVector3::Zero) points[i] = transforms[i].translationVector();
        else if (_origin->billboard) {
            if (camera) points[i] = camera->right() * points[i].x - camera->up() * points[i].y + camera->forward() * points[i].z;
            points[i] += transforms[i].translationVector();
        }
        else points[i] = CSVector3::transformCoordinate(points[i], transforms[i]);
    }

    bool normalFlag = !CSVector3::nearEqual(points[0], points[1]);
    CSVector3 normal = normalFlag ? CSVector3::normalize(points[1] - points[0]) : CSVector3::Zero;

    if (normalFlag && _origin->radial) {
        for (int i = 0; i < 2; i++) {
            float radial = _origin->radial->value(i);

            if (radial) points[i] += normal * radial;
        }
    }
    float pathDuration = _origin->pathUsingSpeed ? (_origin->pathDegree != 0 ? CSVector3::distance(points[0], points[1]) / _origin->pathDegree : 0) : _origin->pathDegree;

    int path0RandomSeq = 0;
    int path1RandomSeq = 0;
    float pathProgress = pathDuration != 0 ? _origin->pathLoop.getProgress(progress / pathDuration, &path0RandomSeq, &path1RandomSeq) : 1;

    if (_origin->reverse) pathProgress = 1 - pathProgress;

    CSVector3 offset(
        _origin->x ? _origin->x->value(pathProgress, CSRandom::toFloatSequenced(_xRandom, path0RandomSeq, path1RandomSeq, pathProgress)) : 0,
        _origin->y ? _origin->y->value(pathProgress, CSRandom::toFloatSequenced(_yRandom, path0RandomSeq, path1RandomSeq, pathProgress)) : 0,
        _origin->z ? _origin->z->value(pathProgress, CSRandom::toFloatSequenced(_zRandom, path0RandomSeq, path1RandomSeq, pathProgress)) : 0);

    if (offset == CSVector3::Zero) {
        points[0] = transforms[0].translationVector();
        points[1] = transforms[1].translationVector();
    }
    else if (_origin->billboard) {
        if (camera) offset = camera->right() * offset.x - camera->up() * offset.y + camera->forward() * offset.z;
        points[0] = transforms[0].translationVector() + offset;
        points[1] = transforms[1].translationVector() + offset;
    }
    else {
        points[0] = CSVector3::transformCoordinate(offset, transforms[0]);
        points[1] = CSVector3::transformCoordinate(offset, transforms[1]);
    }

    CSVector3 translation = CSVector3::lerp(points[0], points[1], pathProgress);

    if (_origin->facing) {
        CSMatrix prevTransform;
        if (progress > FacingInterval && getTransform(progress - FacingInterval, NULL, prevTransform)) {
            CSVector3 start = prevTransform.translationVector();
            CSVector3 end = result.translationVector();

            if (!CSVector3::nearEqual(start, end)) {
                CSVector3 forward = result.forward();
                CSVector3 dir = end - start;
                CSVector3 w = CSVector3::cross(forward, dir);
                CSQuaternion facingRotation(w.x, w.y, w.z, CSVector3::dot(forward, dir));
                facingRotation.w += facingRotation.length();
                facingRotation.normalize();

                rotation = facingRotation * rotation;
            }
            else return false;
        }
        else return false;
    }

    if (normalFlag) {
        if (_origin->radial) {
            float radial = _origin->radial->value(pathProgress, CSRandom::toFloatSequenced(_radialRandom, path0RandomSeq, path1RandomSeq, pathProgress));

            if (radial != 0) translation += normal * radial;
        }
        if (_origin->tangential) {
            float tangential = _origin->tangential->value(pathProgress, CSRandom::toFloatSequenced(_tangentialRandom, path0RandomSeq, path1RandomSeq, pathProgress));

            if (tangential != 0) {
                float tangentialAngle = _origin->tangentialAngle->value(pathProgress, CSRandom::toFloatSequenced(_tangentialAngleRandom, path0RandomSeq)) * FloatToRadians;
                translation += tangentialDirection(normal, tangentialAngle) * tangential;
            }
        }
    }

    result = CSMatrix::lerp(transforms[0], transforms[1], pathProgress);
    result.m41 = translation.x;
    result.m42 = translation.y;
    result.m43 = translation.z;

    if (rotation != CSQuaternion::Identity) {
        result = CSMatrix::rotationQuaternion(rotation) * result;
    }
    if (scale.x != 1) {
        result.m11 *= scale.x;
        result.m12 *= scale.x;
        result.m13 *= scale.x;
    }
    if (scale.y != 1) {
        result.m21 *= scale.y;
        result.m22 *= scale.y;
        result.m23 *= scale.y;
    }
    if (scale.z != 1) {
        result.m31 *= scale.z;
        result.m32 *= scale.z;
        result.m33 *= scale.z;
    }

    return true;
}

float CSAnimationObjectFragment::pathDuration() const {
    if (!_origin->pathUsingSpeed) return _origin->pathDegree;

    if (!_origin->pathDegree) return 0;

    CSMatrix transform;
    if (!_parent) {
        if (!_root->getTransform(_progress, NULL, transform)) return 0;
    }
    else if (_origin->binding && _parent->_substance) {
        if (!_parent->_substance->getTransform(_origin->binding, transform)) return 0;
    }
    else if (!_parent->getTransform(_parent->_progress, NULL, transform)) return 0;

    CSMatrix transforms[2];
    transforms[0] = transform;
    if (_origin->target == 0) transforms[1] = transform;
    else if(!_root->getTargetTransform(_origin->target, _root->progress(), transforms[1])) return 0;

    CSVector3 points[2];

    const CSCamera* camera = _root->scene() ? &_root->scene()->camera() : NULL;

    for (int i = 0; i < 2; i++) {
        points[i] = CSVector3(
            _origin->x ? _origin->x->value(i) : 0,
            _origin->y ? _origin->y->value(i) : 0,
            _origin->z ? _origin->z->value(i) : 0);

        if (points[i] == CSVector3::Zero) points[i] = transforms[i].translationVector();
        else if (_origin->billboard) {
            if (camera) points[i] = camera->right() * points[i].x - camera->up() * points[i].y + camera->forward() * points[i].z;
            points[i] += transforms[i].translationVector();
        }
        else points[i] = CSVector3::transformCoordinate(points[i], transforms[i]);
    }

    if (!CSVector3::nearEqual(points[0], points[1]) && _origin->radial) {
        CSVector3 normal = CSVector3::normalize(points[1] - points[0]);

        for (int i = 0; i < 2; i++) {
            float radial = _origin->radial->value(i);

            if (radial) points[i] += normal * radial;
        }
    }

    return CSVector3::distance(points[0], points[1]) / _origin->pathDegree;
}

float CSAnimationObjectFragment::duration(CSSceneObject::DurationParam param) const {
    float duration = _origin->duration;

    float pathDuration = this->pathDuration();

    if (pathDuration) {
        pathDuration *= _origin->pathLoop.count;

        if (duration < pathDuration) duration = pathDuration;
    }

    if (_origin->rotationDuration) {
        float rotationDuration = _origin->rotationDuration * _origin->rotationLoop.count;

        if (duration < rotationDuration) duration = rotationDuration;
    }

    if (_origin->scaleDuration) {
        float scaleDuration = _origin->scaleDuration * _origin->scaleLoop.count;

        if (duration < scaleDuration) duration = scaleDuration;
    }

    if (_substance) {
        float substanceDuration = _substance->duration(param, duration);

        if (duration < substanceDuration) duration = substanceDuration;
    }

    if (_derivation) {
        float derivationDuration = _derivation->duration(param, duration);

        if (duration < derivationDuration) duration = derivationDuration;
    }

    if (duration) duration += _origin->latency;

    return duration;
}

void CSAnimationObjectFragment::resetRandoms() {
    _xRandom = randInt();
    _yRandom = randInt();
    _zRandom = randInt();
    _radialRandom = randInt();
    _tangentialRandom = randInt();
    _tangentialAngleRandom = randInt();
    _rotationRandom = randInt();
    _scaleRandom = randInt();
}

void CSAnimationObjectFragment::stopSound() {
    if (_soundHandle && (!_origin->sound || _origin->sound->loop == 0 || _origin->sound->stop)) {
        CSAudio::stop(_soundHandle);
    }
    _soundCounter = 0;
}

void CSAnimationObjectFragment::rewind() {
    resetRandoms();

    stopSound();

    _progress = -_origin->latency;

    _visible = !_origin->closing && _origin->latency <= 0 && (_origin->key & _root->keyFlags()) == _origin->key && _origin->checkLocale();

    if (_substance) _substance->rewind();
    if (_derivation) _derivation->rewind();
}

CSSceneObject::UpdateState CSAnimationObjectFragment::update(float delta, bool alive, uint inflags, uint& outflags) {
    if (_origin->closing) {
        if (alive) {
            updateVisible(false, outflags);
            return CSSceneObject::UpdateStateAlive;
        }
        else alive = true;
    }

    _progress += delta;

    if (_progress < 0) {
        updateVisible(false, outflags);
        return CSSceneObject::UpdateStateAlive;
    }

    if (_origin->duration && _progress >= _origin->duration) alive = false;

    bool remaining = false;

    if (_origin->billboard && (inflags & CSSceneObject::UpdateFlagView)) inflags |= CSSceneObject::UpdateFlagTransform;

    if (_origin->pathDegree) {
        inflags |= CSSceneObject::UpdateFlagTransform;

        if (_origin->pathLoop.count) {
            float pathDuration = this->pathDuration();

            if (pathDuration != 0) {
                if (_progress < pathDuration * _origin->pathLoop.count) remaining = true;
                else if (_origin->pathLoop.finish) alive = false;
            }
        }
    }

    if (_origin->rotationDuration) {
        inflags |= CSSceneObject::UpdateFlagTransform;

        if (_origin->rotationLoop.count != 0) {
            if (_progress < _origin->rotationDuration * _origin->rotationLoop.count) remaining = true;
            else if (_origin->rotationLoop.finish) alive = false;
        }
    }

    if (_origin->scaleDuration != 0) {
        inflags |= CSSceneObject::UpdateFlagTransform;

        if (_origin->scaleLoop.count != 0) {
            if (_progress < _origin->scaleDuration * _origin->scaleLoop.count) remaining = true;
            else if (_origin->scaleLoop.finish) alive = false;
        }
    }

    if (_progress < delta) delta = _progress;

    if (_origin->pivot && (inflags & CSSceneObject::UpdateFlagTransform)) outflags |= CSSceneObject::UpdateFlagTransform;

    if (_substance) {
        uint flags = inflags;
        switch (_substance->update(delta, alive || remaining, flags)) {
            case CSSceneObject::UpdateStateStopped:
                alive = false;
                break;
            case CSSceneObject::UpdateStateAlive:
                remaining = true;
                break;
            case CSSceneObject::UpdateStateFinishing:
                alive = false;
                remaining = true;
                break;
        }
        if (!_origin->pivot) flags &= ~CSSceneObject::UpdateFlagTransform;
        outflags |= flags;
    }

    if (_derivation) {
        switch (_derivation->update(delta, alive || remaining, inflags, outflags)) {
            case CSSceneObject::UpdateStateStopped:
                alive = false;
                break;
            case CSSceneObject::UpdateStateAlive:
                remaining = true;
                break;
            case CSSceneObject::UpdateStateFinishing:
                alive = false;
                remaining = true;
                break;
        }
    }

    updateVisible((alive || remaining) && (_origin->key & _root->keyFlags()) == _origin->key && _origin->checkLocale(), outflags);

    if (_visible && _origin->sound && _progress >= _origin->sound->latency) {
        int counter = _origin->sound->duration ? (int)((_progress - _origin->sound->latency) / _origin->sound->duration) + 1 : 1;

        if (_soundCounter != counter) {
            _soundCounter = counter;

            int loop = _origin->sound->duration ? 1 : _origin->sound->loop;

            CSScene* scene = _root->scene();
            if (scene) {
                if (_origin->sound->perspective) {
                    CSMatrix transform;
                    if (getTransform(_progress, NULL, transform)) {
                        CSVector3 pos = transform.translationVector();
                        _soundHandle = scene->playSound(_origin->sound->subpath, _origin->sound->volume, _origin->sound->control, loop, _origin->sound->priority, _origin->sound->duplication, &pos);
                    }
                }
                else _soundHandle = scene->playSound(_origin->sound->subpath, _origin->sound->volume, _origin->sound->control, loop, _origin->sound->priority, _origin->sound->duplication, NULL);
            }
            else {
                const char* path = CSFile::findPath(_origin->sound->subpath);
                if (path) _soundHandle = CSAudio::play(path, _origin->sound->volume, _origin->sound->control, loop);
            }
        }
    }

    if (alive) return CSSceneObject::UpdateStateAlive;
    else if (remaining) return CSSceneObject::UpdateStateFinishing;
    else {
        stopSound();
        return CSSceneObject::UpdateStateStopped;
    }
}

void CSAnimationObjectFragment::updateVisible(bool visible, uint& outflags) {
    if (_visible != visible) {
        _visible = visible;
        if (_origin->hasPivot()) outflags |= CSSceneObject::UpdateFlagTransform;
        if (_origin->hasSubstance()) outflags |= CSSceneObject::UpdateFlagAABB;
    }
}

uint CSAnimationObjectFragment::show() {
    uint showFlags = 0;
    if (_visible) {
        if (_substance) showFlags |= _substance->show();
        if (_derivation) showFlags |= _derivation->show();
    }
    return showFlags;
}

void CSAnimationObjectFragment::draw(CSGraphics* graphics, CSInstanceLayer layer) {
    if (_visible) {
        CSStencilMode stencilMode = _origin->stencil ? CSStencilInclusive : CSStencilNone;

        bool flag = graphics->stencilMode() != stencilMode;

        if (flag) {
            graphics->push();
            graphics->setStencilMode(stencilMode);
        }

        if (_substance) _substance->draw(graphics, layer);
        if (_derivation) _derivation->draw(graphics, layer);

        if (flag) graphics->pop();
    }
}