#define CDK_IMPL

#include "CSSprite.h"

#include "CSSpriteElementMesh.h"

#include "CSResourcePool.h"
#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSScene.h"

CSSpriteBuilder::CSSpriteBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
    timelines(buffer), 
    loop(buffer), 
    billboard(buffer->readBoolean()) 
{
    
}

CSSpriteBuilder::CSSpriteBuilder(const CSSpriteBuilder* other) :
    CSSceneObjectBuilder(other),
    timelines(other->timelines.count()),
    loop(other->loop),
    billboard(other->billboard) 
{
    foreach (const CSSpriteTimeline*, timeline, &other->timelines) timelines.addObject(CSSpriteTimeline::timelineWithTimeline(timeline));
}

int CSSpriteBuilder::resourceCost() const {
    int cost = sizeof(CSSpriteBuilder) + timelines.capacity() * 8;
    foreach (const CSSpriteTimeline*, timeline, &timelines) cost += timeline->resourceCost();
    return cost;
}

void CSSpriteBuilder::preload() const {
    foreach (const CSSpriteTimeline*, timeline, &timelines) timeline->preload();
}

float CSSpriteBuilder::singleDuration() const {
    float duration = 0;
    foreach (const CSSpriteTimeline*, timeline, &timelines) {
        if (duration < timeline->endTime) duration = timeline->endTime;
    }
    return duration;
}

float CSSpriteBuilder::totalDuration() const {
    return loop.count ? singleDuration() * loop.count : 0;
}

//========================================================================================

CSSpriteObject::CSSpriteObject(const CSSpriteBuilder* origin) :
    CSSceneObject(origin),
    _origin(retain(origin)), 
    _progress(0),
    _clippedProgress(0),
    _randomSeed(randLong()),
    _visible(false),
    _cursor(false)
{
    resetTimelines();
}

CSSpriteObject::~CSSpriteObject() {
    _origin->release();
}

bool CSSpriteObject::addAABB(CSABoundingBox& result) const {
    CSMatrix transform;
    if (!getTransform(_progress, NULL, transform)) return false;

    CSSpriteElement::TransformParam param;
    param.parent = this;
    param.transform = transform;
    bool flag = false;
    foreach (const CSSpriteTimeline*, timeline, &_timelines[_cursor]) {
        if (timeline->reset) param.transform = transform;
        param.duration = timeline->duration();
        param.progress = (_clippedProgress - timeline->startTime) / param.duration;

        CSRandom random(_randomSeed);
        foreach (const CSSpriteElement*, element, &timeline->elements) {
            param.random = random.nextInt();
            flag |= element->addAABB(param, result);
        }
    }
    return flag;
}

void CSSpriteObject::addCollider(CSCollider*& result) const {
    CSMatrix transform;
    if (!getTransform(_progress, NULL, transform)) return;

    CSSpriteElement::TransformParam param;
    param.parent = this;
    param.transform = transform;
    foreach (const CSSpriteTimeline*, timeline, &_timelines[_cursor]) {
        if (timeline->reset) param.transform = transform;
        param.duration = timeline->duration();
        param.progress = (_clippedProgress - timeline->startTime) / param.duration;

        CSRandom random(_randomSeed);
        foreach (const CSSpriteElement*, element, &timeline->elements) {
            param.random = random.nextInt();
            element->addCollider(param, result);
        }
    }
}

bool CSSpriteObject::getTransform(float progress, const string& name, CSMatrix& result) const {
    if (!CSSceneObject::getTransform(progress, NULL, result)) return false;
    if (_origin->billboard) {
        const CSScene* scene = this->scene();
        if (scene) scene->camera().view().billboard(&result, result);
    }
    if (!name) return true;

    CSArray<const CSSpriteTimeline>* timelines;
    float clippedProgress;

    if (CSMath::nearEqual(_progress, progress)) {
        timelines = &_timelines[_cursor];
        clippedProgress = _clippedProgress;
    }
    else {
        timelines = CSArray<const CSSpriteTimeline>::array();
        clippedProgress = clipProgress(progress);
        getTimelines(clippedProgress, timelines);
    }
    CSSpriteElement::TransformParam param;
    param.parent = this;
    param.transform = result;
    foreach (const CSSpriteTimeline*, timeline, timelines) {
        if (timeline->reset) param.transform = result;
        param.duration = timeline->duration();
        param.progress = (clippedProgress - timeline->startTime) / param.duration;
        
        CSRandom random(_randomSeed);
        foreach (const CSSpriteElement*, element, &timeline->elements) {
            param.random = random.nextInt();
            if (element->getTransform(param, name, result)) return true;
        }
    }
    return false;
}

float CSSpriteObject::duration(DurationParam param, float duration) const {
    return _origin->totalDuration();
}

bool CSSpriteObject::afterCameraUpdate() const {
    return _origin->billboard || CSSceneObject::afterCameraUpdate();
}

void CSSpriteObject::onLink() {
    CSSceneObject::onLink();

    CSScene* scene = this->scene();
    for (CSDictionary<const void*, CSMeshObject>::Iterator i = _meshInstances.iterator(); i.remaining(); i.next()) i.object()->link(scene);
}

void CSSpriteObject::onUnlink() {
    CSSceneObject::onUnlink();

    for (CSDictionary<const void*, CSMeshObject>::Iterator i = _meshInstances.iterator(); i.remaining(); i.next()) i.object()->unlink();
}

void CSSpriteObject::onRewind() {
    _progress = 0;
    _clippedProgress = 0;
    _randomSeed = randLong();
    resetTimelines();
}

bool CSSpriteObject::clipProgress() {
    float clippedProgress = clipProgress(_progress);
    if (_clippedProgress != clippedProgress) {
        _clippedProgress = clippedProgress;
        return true;
    }
    return false;
}

float CSSpriteObject::clipProgress(float progress) const {
    float result = 0;
    float duration = _origin->singleDuration();
    if (duration) result = _origin->loop.getProgress(progress / duration, NULL, NULL) * duration;
    return result;
}

void CSSpriteObject::resetTimelines() {
    _cursor = !_cursor;
    getTimelines(_clippedProgress, &_timelines[_cursor]);
}

void CSSpriteObject::getTimelines(float progress, CSArray<const CSSpriteTimeline>* result) const {
    result->removeAllObjects();

    if (_origin->timelines.count()) {
        foreach (const CSSpriteTimeline*, timeline, &_origin->timelines) {
            if (timeline->startTime <= progress && progress <= timeline->endTime) {
                int i = 0;
                while (i < result->count()) {
                    if (timeline->layer >= result->objectAtIndex(i)->layer) i++;
                    else break;
                }
                result->insertObject(i, timeline);
            }
        }
    }
}

CSSceneObject::UpdateState CSSpriteObject::onUpdate(float delta, bool alive, uint& flags) {
    uint inflags = flags;

    UpdateState state0 = CSSceneObject::onUpdate(delta, alive, flags);

    float prevClippedProgress = _clippedProgress;
    _progress += delta;

    bool flag = true;
    if (clipProgress()) {
        resetTimelines();
        flag = _timelines[_cursor].sequentialEqual(&_timelines[!_cursor]);
    }
    if (flag) {
        if (_origin->billboard && (inflags & UpdateFlagView)) inflags |= UpdateFlagTransform;

        CSSpriteElement::TransformUpdatedParam param;
        param.parent = this;
        param.inflags = inflags;
        foreach (const CSSpriteTimeline*, timeline, &_timelines[_cursor]) {
            if (timeline->reset) param.inflags = inflags;
            param.duration = timeline->duration();
            param.progress0 = (prevClippedProgress - timeline->startTime) / param.duration;
            param.progress1 = (_clippedProgress - timeline->startTime) / param.duration;
            foreach (const CSSpriteElement*, element, &timeline->elements) element->getTransformUpdated(param, flags);
        }
    }
    else flags |= UpdateFlagTransform | UpdateFlagAABB;

    UpdateState state1;
    if (_origin->loop.count == 0) state1 = UpdateStateNone;
    else if (_progress < _origin->singleDuration() * _origin->loop.count) state1 = alive ? UpdateStateAlive : UpdateStateFinishing;
    else state1 = _origin->loop.finish ? UpdateStateStopped : UpdateStateNone;

    switch (state0) {
        case UpdateStateNone:
            return state1;
        case UpdateStateFinishing:
            return state1 == UpdateStateAlive ? UpdateStateAlive : UpdateStateFinishing;
    }
    return state0;
}

uint CSSpriteObject::onShow() {
    uint showFlags = CSSceneObject::onShow();
    foreach (const CSSpriteTimeline*, timeline, &_timelines[_cursor]) {
        foreach (const CSSpriteElement*, element, &timeline->elements) showFlags |= element->showFlags();
    }
    _visible = _timelines[_cursor].count() && getTransform(_progress, NULL, _transform);
    return showFlags;
}

void CSSpriteObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
    if (_visible) {
        CSMatrix prev = graphics->world();
        graphics->world() = _transform * prev;
        graphics->push();

        CSSpriteElement::DrawParam param;
        param.graphics = graphics;
        param.layer = layer;
        param.parent = this;

        foreach (const CSSpriteTimeline*, timeline, &_timelines[_cursor]) {
            if (timeline->reset) graphics->reset();
            param.duration = timeline->duration();
            param.progress = (_clippedProgress - timeline->startTime) / param.duration;

            CSRandom random(_randomSeed);
            foreach (const CSSpriteElement*, element, &timeline->elements) {
                param.random = random.nextInt();
                element->draw(param);
            }
        }
        graphics->pop();
        graphics->world() = prev;
    }
}
