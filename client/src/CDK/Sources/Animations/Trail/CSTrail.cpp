#define CDK_IMPL

#include "CSTrail.h"

#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSAnimationFragment.h"
#include "CSAnimationDerivation.h"

#include "CSScene.h"

#include "CSStreamRenderCommand.h"

static constexpr int MaxPoints = 1000;

CSTrailBuilder::CSTrailBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
    sources(buffer, CSAnimationSource::sourceWithBuffer), 
    distance(buffer->readFloat()),
    billboard(buffer->readBoolean()),
    localSpace(buffer->readBoolean()),
    emission(buffer->readBoolean()),
    emissionSmoothness(buffer->readByte()),
    emissionLife(buffer->readFloat()),
    repeatScale(buffer->readFloat()),
    color(CSAnimationColor::colorWithBuffer(buffer)),
    colorDuration(buffer->readFloat()),
    colorLoop(buffer),
    rotation(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationDuration(buffer->readFloat()),
    rotationLoop(buffer),
    scale(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleDuration(buffer->readFloat()),
    scaleLoop(buffer)
{

}

CSTrailBuilder::CSTrailBuilder(const CSTrailBuilder* other) :
    CSSceneObjectBuilder(other),
    sources(other->sources.count()),
    distance(other->distance),
    billboard(other->billboard),
    localSpace(other->localSpace),
    emission(other->emission),
    emissionSmoothness(other->emissionSmoothness),
    emissionLife(other->emissionLife),
    repeatScale(other->repeatScale),
    color(CSAnimationColor::colorWithColor(color)),
    colorDuration(other->colorDuration),
    colorLoop(other->colorLoop),
    rotation(CSAnimationFloat::factorWithFactor(rotation)),
    rotationDuration(other->rotationDuration),
    rotationLoop(other->rotationLoop),
    scale(CSAnimationFloat::factorWithFactor(scale)),
    scaleDuration(other->scaleDuration),
    scaleLoop(other->scaleLoop)
{
    foreach (const CSAnimationSource*, source, &other->sources) sources.addObject(CSAnimationSource::sourceWithSource(source));
}

int CSTrailBuilder::resourceCost() const {
    int cost = sizeof(CSTrailBuilder);
    foreach (const CSAnimationSource*, source, &sources) cost += source->resourceCost();
    if (color) cost += color->resourceCost();
    if (rotation) cost += rotation->resourceCost();
    if (scale) cost += scale->resourceCost();
    return cost;
}

void CSTrailBuilder::preload() const {
    foreach (const CSAnimationSource*, source, &sources) source->preload();
}

//========================================================================================

CSTrailObject::Point::Point(const CSVector3& point, float progress, int link) :
    point(point),
    progress(progress),
    link(link)
{

}

CSTrailObject::CSTrailObject(const CSTrailBuilder* origin) : 
    CSSceneObject(origin),
    _origin(retain(origin)),
    _progress(0),
    _remaining(0),
    _counter(0),
    _link(0),
    _emitting(true)
{
    resetRandoms();
}

CSTrailObject::~CSTrailObject() {
    _origin->release();
}

bool CSTrailObject::addAABB(CSABoundingBox& result) const {
    bool flag = false;
    CSMatrix transform;
    if (getTransform(0, NULL, transform)) {
        result.append(transform.translationVector());
        flag = true;
    }
    if (getTransform(_progress, NULL, transform)) {
        result.append(transform.translationVector());
        flag = true;
    }
    return flag;
}

float CSTrailObject::duration(DurationParam param, float duration) const {
    foreach (const CSAnimationSource*, source, &_origin->sources) {
        switch (source->type()) {
            case CSAnimationSource::TypeImage:
                {
                    const CSAnimationSourceImage* imageSource = static_cast<const CSAnimationSourceImage*>(source);
                    float d = imageSource->duration * imageSource->loop.count;
                    if (duration < d) duration = d;
                }
                break;
            case CSAnimationSource::TypeMesh:
                {
                    const CSAnimationSourceMesh* meshSource = static_cast<const CSAnimationSourceMesh*>(source);

                    if (meshSource->builder) {
                        const CSMeshAnimation* animation = meshSource->builder->animation();

                        if (animation) {
                            float d = animation->duration() * meshSource->builder->loop.count;
                            if (duration < d) duration = d;
                        }
                    }
                }
                break;
        }
    }
    if (_origin->colorDuration > 0) {
        float d = _origin->colorDuration * _origin->colorLoop.count;

        if (duration < d) duration = d;
    }
    if (_origin->rotationDuration > 0) {
        float d = _origin->rotationDuration * _origin->rotationLoop.count;

        if (duration < d) duration = d;
    }
    if (_origin->scaleDuration > 0) {
        float d = _origin->scaleDuration * _origin->scaleLoop.count;

        if (duration < d) duration = d;
    }
    if (duration && _origin->emission && _origin->emissionLife > 0) {
        switch (param) {
            case DurationParamAvg:
                duration += _origin->emissionLife * 0.5f;
                break;
            case DurationParamMax:
                duration += _origin->emissionLife;
                break;
        }
    }
    return duration;
}

void CSTrailObject::resetRandoms() {
    _link = _origin->sources.count() <= 1 ? 0 : randInt(0, _origin->sources.count() - 1);

    _colorRandom = CSColor(
        randFloat(0, 1),
        randFloat(0, 1),
        randFloat(0, 1),
        randFloat(0, 1));
    _rotationRandom = randFloat(0, 1);
    _scaleRandom = randFloat(0, 1);
    _materialRandom = randInt();
}

void CSTrailObject::onLink() {
    CSSceneObject::onLink();

    CSScene* scene = this->scene();
    foreach (CSMeshObject*, i, &_meshInstances) i->link(scene);
}

void CSTrailObject::onUnlink() {
    CSSceneObject::onUnlink();

    foreach (CSMeshObject*, i, &_meshInstances) i->unlink();
}

void CSTrailObject::onRewind() {
    CSSceneObject::onRewind();

    _progress = 0;
    _remaining = 0;
    _counter = 0;
    _points.removeAllObjects();
    _emitting = true;

    resetRandoms();
}


CSSceneObject::UpdateState CSTrailObject::onUpdate(float delta, bool alive, uint& flags) {
    uint inflags = flags;
    
    UpdateState state0 = CSSceneObject::onUpdate(delta, alive, flags);

    if (inflags & UpdateFlagTransform) flags |= UpdateFlagAABB;

    _progress += delta;
    _remaining += delta;
    _counter++;

    _emitting &= alive;

    bool remaining = false;

    if (!_origin->emission) _points.removeAllObjects();
    else if (_origin->emissionLife > 0) {
        while (_points.count() && _points.objectAtIndex(0).progress <= _progress - _origin->emissionLife) _points.removeObjectAtIndex(0);

        if (_points.count()) remaining = true;
    }

    UpdateState state1;
    if (alive) state1 = UpdateStateAlive;
    else if (remaining) state1 = UpdateStateFinishing;
    else state1 = _origin->emissionLife > 0 ? UpdateStateStopped : UpdateStateNone;

    switch (state0) {
        case UpdateStateNone:
            return state1;
        case UpdateStateFinishing:
            return state1 == UpdateStateAlive ? UpdateStateAlive : UpdateStateFinishing;
    }
    return state0;
}

void CSTrailObject::addBreak(float progress, float delta, float duration, const CSAnimationLoop& loop, CSArray<BreakEntry>*& breaks) {
    if (duration > 0) {
        int step0 = CSMath::ceil(progress / duration);
        int step1 = CSMath::ceil((progress - delta) / duration);

        if (step0 != step1 && (loop.count == 0 || step1 <= loop.count)) {
            if (!breaks) breaks = new CSArray<BreakEntry>(1);
            BreakEntry entry;
            entry.delta = progress - (int)(progress / duration) * duration;
            entry.type = loop.count == 0 || step0 <= loop.count ? (loop.roundTrip ? BreakEntryTypeReverse : BreakEntryTypeRewind) : BreakEntryTypeNone;

            int i = 0;
            while (i < breaks->count()) {
                if (entry.delta > breaks->objectAtIndex(i).delta) break;
                i++;
            }
            breaks->insertObject(i, entry);
        }
    }
}

void CSTrailObject::addPoint(const CSVector3& p, float progress) {
    if (_points.count() < MaxPoints) {
        if (_origin->distance > 0 && _points.count()) {
            const Point& lp = _points.lastObject();

            if (lp.point.distanceSquared(p) < _origin->distance * _origin->distance) return;
        }
        bool replace;
        if (_points.count() > 1) {
            const Point& p0 = _points.lastObject();
            const Point& p1 = _points.objectAtIndex(_points.count() - 2);
            replace = p0.link != _link && p0.link != p1.link;
        }
        else if (_points.count() == 1) {
            const Point& p0 = _points.lastObject();
            replace = p0.link != _link;
        }
        else {
            replace = false;
        }
        new (replace ? &_points.lastObject() : &_points.addObject()) Point(p, progress, _link);
    }
}

void CSTrailObject::updatePointTransforms(int s, int e) {
    int pslen = e - s + 1;

    CSVector3* ps = (CSVector3*)alloca(pslen * 2 * sizeof(CSVector3));

    const CSCamera* camera = scene() ? &scene()->camera() : NULL;

    for (int i = s; i <= e; i++) {
        int si = CSMath::max(i - 1, s);
        int ei = CSMath::min(i + 1, e);
        const CSVector3& p0 = _points.objectAtIndex(si).point;
        const CSVector3& p2 = _points.objectAtIndex(ei).point;
        CSVector3 pf = CSVector3::normalize(p2 - p0);

        CSVector3 pr;
        if (_origin->billboard && camera) pr = -camera->forward();
        else if (CSMath::nearOne(CSMath::abs(CSVector3::dot(pf, CSVector3::UnitZ)))) CSVector3::normalize(CSVector3::cross(pf, CSVector3::UnitX), pr);
        else CSVector3::normalize(CSVector3::cross(CSVector3::UnitZ, pf), pr);

        ps[(i - s) * 2] = pf;
        ps[(i - s) * 2 + 1] = pr;
    }

    if (!_origin->billboard && pslen > 2) {
        for (int ss = 0; ss < _origin->emissionSmoothness; ss++) {
            for (int i = 1; i < pslen - 1; i++) {
                CSVector3::normalize(ps[i * 2 + 1] + ps[(i + 1) * 2 + 1] + ps[(i - 1) * 2 + 1], ps[i * 2 + 1]);
            }
        }
    }

    for (int i = s; i <= e; i++) {
        CSVector3& pf = ps[(i - s) * 2];
        CSVector3& pr = ps[(i - s) * 2 + 1];
        CSVector3 pu = CSVector3::normalize(CSVector3::cross(pr, pf));

        Point& p = _points.objectAtIndex(i);

        float delta = _progress - p.progress;
        float scale = _origin->scale ? _origin->scale->value(_origin->scaleDuration > 0 ? _origin->scaleLoop.getProgress(delta / _origin->scaleDuration) : 1, _scaleRandom) : 1;
        float rotation = _origin->rotation ? _origin->rotation->value(_origin->rotationDuration > 0 ? _origin->rotationLoop.getProgress(delta / _origin->rotationDuration) : 1, _rotationRandom) : 0;

        if (rotation) {
            CSQuaternion rm = CSQuaternion::rotationAxis(pf, rotation);
            CSVector3::transform(pr, rm, pr);
            CSVector3::transform(pu, rm, pu);
        }
        pr *= scale;
        pu *= scale;

        CSMatrix m(
            pf.x, pf.y, pf.z, 0,
            pu.x, pu.y, pu.z, 0,
            pr.x, pr.y, pr.z, 0,
            0, 0, 0, 1);

        p.transform = m;
    }
}

uint CSTrailObject::onShow() {
    uint showFlags = CSSceneObject::onShow();

    CSMatrix transform;

    if (!_origin->emission) {
        if (getTransform(0, NULL, transform)) addPoint(transform.translationVector(), 0);
        if (getTransform(_progress, NULL, transform)) addPoint(transform.translationVector(), _progress);
    }
    else {
        if (_origin->localSpace) {
            foreach (Point&, p, &_points) {
                if (getTransform(p.progress, NULL, transform)) p.point = transform.translationVector();
            }
        }
        if (_emitting && _counter != 0) {
            const CSAnimationObjectFragment* current = animation();

            CSArray<BreakEntry>* breaks = NULL;

            for (; ; ) {
                addBreak(current->progress(), _remaining, current->pathDuration(), current->origin()->pathLoop, breaks);
                addBreak(current->progress(), _remaining, current->origin()->rotationDuration, current->origin()->rotationLoop, breaks);
                addBreak(current->progress(), _remaining, current->origin()->scaleDuration, current->origin()->scaleLoop, breaks);
                if (current->parent()) current = current->parent();
                else break;
            }

            for (int i = _counter - 1; i >= 0; i--) {
                float delta = i * _remaining / _counter;

                float progress;

                if (breaks) {
                    while (breaks->count() && delta < breaks->objectAtIndex(0).delta) {
                        const BreakEntry& b = breaks->objectAtIndex(0);

                        if (b.type == BreakEntryTypeRewind) {
                            progress = _progress - b.delta - 0.0001f;

                            if (getTransform(progress, NULL, transform)) {
                                addPoint(transform.translationVector(), progress);
                                _link++;
                            }
                        }
                        progress = _progress - b.delta;

                        if (getTransform(progress, NULL, transform)) {
                            addPoint(transform.translationVector(), progress);

                            if (b.type == BreakEntryTypeReverse) {
                                _link++;
                                addPoint(transform.translationVector(), progress);
                            }
                        }

                        breaks->removeObjectAtIndex(0);
                    }
                }

                progress = _progress - delta;

                if (getTransform(progress, NULL, transform)) addPoint(transform.translationVector(), progress);
            }
        }
    }

    _remaining = 0;
    _counter = 0;

    int meshCount = 0;

    if (_origin->sources.count()) {
        int s = 0;

        while (s < _points.count()) {
            int link = _points.objectAtIndex(s).link;

            int e = s;

            while (e < _points.count() - 1 && _points.objectAtIndex(e + 1).link == link) e++;

            if (s < e) {
                updatePointTransforms(s, e);

                const CSAnimationSource* source = _origin->sources.objectAtIndex(link % _origin->sources.count());

                switch (source->type()) {
                    case CSAnimationSource::TypeMesh:
                        showFlags |= showMesh(static_cast<const CSAnimationSourceMesh*>(source), s, e, meshCount);
                        break;
                }
            }

            s = e + 1;
        }
    }

    while (meshCount > _meshInstances.count()) {
        _meshInstances.lastObject()->unlink();
        _meshInstances.removeLastObject();
    }

    return showFlags;
}

uint CSTrailObject::showMesh(const CSAnimationSourceMesh* source, int s, int e, int& meshCount) {
    if (!source->builder || !source->bones || source->bones->count() < 2) return 0;

    float* boneDistances = (float*)alloca(source->bones->count() * sizeof(float));
    float boneTotalDistance = 0;
    CSVector3 pp = CSVector3::Zero;

    const CSMeshGeometry* geometry = source->builder->geometry();

    for (int i = 0; i < source->bones->count(); i++) {
        CSMatrix boneTransform;
        if (geometry && geometry->getNodeTransform(source->bones->objectAtIndex(i), NULL, 0, boneTransform)) {
            if (i == 0) {
                pp = boneTransform.translationVector();
            }
            else {
                CSVector3 p = boneTransform.translationVector();
                float d = CSVector3::distance(p, pp);
                boneTotalDistance += d;
                boneDistances[i] = boneTotalDistance;
                pp = p;
            }
        }
        else return 0;
    }
    int pointLength = e - s + 1;
    float* pointDistances = (float*)alloca(pointLength * sizeof(float));
    float pointTotalDistance = 0;
    pp = _points.objectAtIndex(s).point;
    for (int i = s + 1; i <= e; i++) {
        const CSVector3& p = _points.objectAtIndex(i).point;
        float d = CSVector3::distance(p, pp);
        pointTotalDistance += d;
        pointDistances[i - s] = pointTotalDistance;
        pp = p;
    }
    int seperation = _origin->repeatScale > 0 ? CSMath::max((int)(pointTotalDistance * _origin->repeatScale / boneTotalDistance), 1) : 1;

    int pc = 0;

    const CSMeshAnimation* animation = source->builder->animation();

    float ap = animation ? source->builder->loop.getProgress(_progress / animation->duration()) * animation->duration() : 0;

    CSScene* scene = this->scene();

    for (int sc = 0; sc < seperation; sc++) {
        CSMeshObject* instance;
        if (meshCount >= _meshInstances.count()) {
            instance = new CSMeshObject(geometry);
            _meshInstances.addObject(instance);
            if (scene) instance->link(scene);
            instance->release();
        }
        else if (_meshInstances.objectAtIndex(meshCount)->geometry() != geometry) {
            instance = new CSMeshObject(geometry);
            _meshInstances.objectAtIndex(meshCount)->unlink();
            _meshInstances.setObject(meshCount, instance);
            if (scene) instance->link(scene);
            instance->release();
        }
        else instance = _meshInstances.objectAtIndex(meshCount);

        instance->setSkin(source->builder->skin);
        instance->setFrameDivision(source->builder->frameDivision);
        instance->setAnimation(animation);
        instance->setProgress(ap);
        meshCount++;

        for (int i = 0; i < source->bones->count(); i++) {
            float pointDistance = pointTotalDistance * (boneDistances[i] + boneTotalDistance * sc) / (boneTotalDistance * seperation);

            while (pc < pointLength - 2 && pointDistance > pointDistances[pc + 1]) pc++;

            float tp = (pointDistance - pointDistances[pc]) / (pointDistances[pc + 1] - pointDistances[pc]);

            instance->setCustomTransform(source->bones->objectAtIndex(i), CSMatrix::lerp(_points.objectAtIndex(pc + s).transform, _points.objectAtIndex(pc + s + 1).transform, tp));
        }
    }

    return source->builder->showFlags();
}

void CSTrailObject::drawImage(CSGraphics* graphics, const CSAnimationSourceImage* source, int s, int e) {
    const CSImage* image = source->image(_progress);
    if (!image) return;
    const CSTexture* texture = image->texture();
    const CSRect& frame = image->frame();
    if (frame.width < 2) return;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(graphics, CSPrimitiveTriangles);
    command->state.material.colorMap = texture;

    int vi = 0;
    for (int i = s; i < e; i++) {
        command->addIndex(vi + 0);
        command->addIndex(vi + 2);
        command->addIndex(vi + 1);
        command->addIndex(vi + 2);
        command->addIndex(vi + 3);
        command->addIndex(vi + 1);
        vi += 2;
    }

    float h = (float)frame.height * image->contentScale() * 0.5f;
    float d = (float)frame.left() / texture->width();
    float tv = (float)frame.top() / texture->height();
    float bv = (float)frame.bottom() / texture->height();

    for (int i = s; i <= e; i++) {
        float u = _origin->repeatScale > 0 ? d : (frame.x + frame.width * (i - s) / (e - s)) / texture->width() + d;

        const Point& p = _points.objectAtIndex(i);

        CSColor color = _origin->color ? _origin->color->value(_origin->colorDuration > 0 ? _origin->colorLoop.getProgress((_progress - p.progress) / _origin->colorDuration) : 1, _colorRandom, CSColor::White) : CSColor::White;

        CSVector3 pu = p.transform.up() * h;

        CSVector3 pn = CSVector3::Zero;
        CSVector3 pt = CSVector3::Zero;
        if (command->state.usingVertexNormal()) {
            CSVector3::normalize(p.transform.right(), pn);
            if (CSVector3::dot(pn, graphics->camera().forward()) < 0) pn = -pn;
            CSVector3::normalize(p.transform.up(), pt);
        }
        command->addVertex(CSFVertex(p.point + pu, color, CSVector2(u, tv), pn, pt));
        command->addVertex(CSFVertex(p.point - pu, color, CSVector2(u, bv), pn, pt));

        if (_origin->repeatScale > 0 && i < e) d += CSVector3::distance(p.point, _points.objectAtIndex(i + 1).point) / (texture->width() * image->contentScale() * _origin->repeatScale);
    }
    graphics->command(command);

    command->release();
}

void CSTrailObject::drawImages(CSGraphics* graphics, CSInstanceLayer layer) {
    int s = 0;

    while (s < _points.count()) {
        int link = _points.objectAtIndex(s).link;

        int e = s;

        while (e < _points.count() - 1 && _points.objectAtIndex(e + 1).link == link) e++;

        if (s < e) {
            const CSAnimationSource* source = _origin->sources.objectAtIndex(link % _origin->sources.count());

            if (source->type() == CSAnimationSource::TypeImage) {
                const CSAnimationSourceImage* imageSource = static_cast<const CSAnimationSourceImage*>(source);
                if (CSMaterialSource::apply(imageSource->material, graphics, layer, _progress, _materialRandom, NULL, true)) {
                    drawImage(graphics, imageSource, s, e);
                    graphics->pop();
                }
            }
        }
        s = e + 1;
    }
}

void CSTrailObject::drawMeshes(CSGraphics* graphics, CSInstanceLayer layer) {
    CSColor prev = graphics->color();

    if (_origin->color) {
        CSColor color = _origin->color->value(_origin->colorDuration > 0 ? _origin->colorLoop.getProgress(_progress / _origin->colorDuration) : 1, _colorRandom, CSColor::White);
        graphics->setColor(color * prev, false);
    }
    foreach (CSMeshObject*, mi, &_meshInstances) {
        mi->drawIndirect(graphics, layer, _progress, _materialRandom);
    }
    if (_origin->color) graphics->setColor(prev, false);
}

void CSTrailObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
    if (_origin->sources.count() && layer != CSInstanceLayerShadow2D) {
        drawImages(graphics, layer);
        drawMeshes(graphics, layer);
    }
}
