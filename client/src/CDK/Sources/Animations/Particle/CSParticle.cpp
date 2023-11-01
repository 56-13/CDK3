#define CDK_IMPL

#include "CSParticle.h"

#include "CSBuffer.h"
#include "CSRandom.h"

#include "CSAnimationSourceImage.h"
#include "CSAnimationSourceMesh.h"

#include "CSScene.h"

#include "CSStreamRenderCommand.h"

static constexpr int MaxInstances = 1000;

CSParticleBuilder::CSParticleBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene),
    sources(buffer, CSAnimationSource::sourceWithBuffer), 
    shape(CSParticleShape::shapeWithBuffer(buffer)),
    shapeShell(buffer->readBoolean()),
    color(CSAnimationColor::colorWithBuffer(buffer)),
    radial(CSAnimationFloat::factorWithBuffer(buffer)),
    x(CSAnimationFloat::factorWithBuffer(buffer)),
    y(CSAnimationFloat::factorWithBuffer(buffer)),
    z(CSAnimationFloat::factorWithBuffer(buffer)),
    billboardX(CSAnimationFloat::factorWithBuffer(buffer)),
    billboardY(CSAnimationFloat::factorWithBuffer(buffer)),
    billboardZ(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationX(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationY(CSAnimationFloat::factorWithBuffer(buffer)),
    rotationZ(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleX(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleY(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleZ(CSAnimationFloat::factorWithBuffer(buffer)),
    scaleEach(buffer->readBoolean()),
    view((View)buffer->readByte()),
    stretchRate(view == ViewStretchBillboard ? buffer->readFloat() : 0),
    localSpace(buffer->readBoolean()),
    prewarm(buffer->readBoolean()),
    finish(buffer->readBoolean()),
    clear(buffer->readBoolean()),
    life(buffer->readFloat()),
    lifeVar(buffer->readFloat()),
    emissionRate(buffer->readFloat()),
    emissionMax(buffer->readInt())
{

}

CSParticleBuilder::CSParticleBuilder(const CSParticleBuilder* other) :
    CSSceneObjectBuilder(other),
    sources(other->sources.count()),
    shape(CSParticleShape::shapeWithShape(other->shape)),
    shapeShell(other->shapeShell),
    color(CSAnimationColor::colorWithColor(other->color)),
    radial(CSAnimationFloat::factorWithFactor(other->radial)),
    x(CSAnimationFloat::factorWithFactor(other->x)),
    y(CSAnimationFloat::factorWithFactor(other->y)),
    z(CSAnimationFloat::factorWithFactor(other->z)),
    billboardX(CSAnimationFloat::factorWithFactor(other->billboardX)),
    billboardY(CSAnimationFloat::factorWithFactor(other->billboardY)),
    billboardZ(CSAnimationFloat::factorWithFactor(other->billboardZ)),
    rotationX(CSAnimationFloat::factorWithFactor(other->rotationX)),
    rotationY(CSAnimationFloat::factorWithFactor(other->rotationY)),
    rotationZ(CSAnimationFloat::factorWithFactor(other->rotationZ)),
    scaleX(CSAnimationFloat::factorWithFactor(other->scaleX)),
    scaleY(CSAnimationFloat::factorWithFactor(other->scaleY)),
    scaleZ(CSAnimationFloat::factorWithFactor(other->scaleZ)),
    scaleEach(other->scaleEach),
    view(other->view),
    stretchRate(other->stretchRate),
    localSpace(other->localSpace),
    prewarm(other->prewarm),
    finish(other->finish),
    clear(other->clear),
    life(other->life),
    lifeVar(other->lifeVar),
    emissionRate(other->emissionRate),
    emissionMax(other->emissionMax)
{
    foreach (const CSAnimationSource*, source, &other->sources) sources.addObject(CSAnimationSource::sourceWithSource(source));
}

int CSParticleBuilder::resourceCost() const {
    int cost = sizeof(CSParticleBuilder);
    foreach (const CSAnimationSource*, source, &sources) cost += source->resourceCost();
    if (shape) cost += shape->resourceCost();
    if (color) cost += color->resourceCost();
    if (radial) cost += radial->resourceCost();
    if (x) cost += x->resourceCost();
    if (y) cost += y->resourceCost();
    if (z) cost += z->resourceCost();
    if (billboardX) cost += billboardX->resourceCost();
    if (billboardY) cost += billboardY->resourceCost();
    if (billboardZ) cost += billboardZ->resourceCost();
    if (rotationX) cost += rotationX->resourceCost();
    if (rotationY) cost += rotationY->resourceCost();
    if (rotationZ) cost += rotationZ->resourceCost();
    if (scaleX) cost += scaleX->resourceCost();
    if (scaleY) cost += scaleY->resourceCost();
    if (scaleZ) cost += scaleZ->resourceCost();
    return cost;
}

void CSParticleBuilder::preload() const {
    foreach (const CSAnimationSource*, source, &sources) source->preload();
}

//========================================================================================

CSParticleObject::CSParticleObject(const CSParticleBuilder* origin) :
    CSSceneObject(origin),
    _origin(retain(origin)), 
    _instances(origin->emissionMax),
    _progress(0),
    _remaining(0),
    _counter(origin->prewarm && origin->emissionRate ? origin->emissionMax / origin->emissionRate : 0),
    _emitting(true),
    _visible(false),
    _materialRandom(randInt())
{

}

CSParticleObject::~CSParticleObject() {
    _origin->release();
}

bool CSParticleObject::addAABB(CSABoundingBox& result) const {
    if (!_origin->shape) return false;
    CSMatrix transform;
    if (!getTransform(NULL, transform)) return false;
    _origin->shape->addAABB(transform, result);
    return true;
}

float CSParticleObject::duration(DurationParam param, float duration) const {
    if (duration) {
        switch (param) {
            case DurationParamAvg:
                duration += (_origin->life + _origin->lifeVar) * 0.5f;
                break;
            case DurationParamMax:
                duration += _origin->life + _origin->lifeVar;
                break;
        }
    }
    return duration;
}

bool CSParticleObject::afterCameraUpdate() const {
    return _origin->view != CSParticleBuilder::ViewNone || _origin->billboardX || _origin->billboardY || _origin->billboardZ;
}

void CSParticleObject::onLink() {
    CSSceneObject::onLink();

    CSScene* scene = this->scene();
    foreach (MeshFragment&, frag, &_meshFragments) frag.origin->link(scene);
}

void CSParticleObject::onUnlink() {
    CSSceneObject::onUnlink();

    foreach (MeshFragment&, frag, &_meshFragments) frag.origin->unlink();
}

void CSParticleObject::onRewind() {
    CSSceneObject::onRewind();

    _instances.removeAllObjects();
    _progress = 0;
    _counter = _origin->prewarm && _origin->emissionRate ? _origin->emissionMax / _origin->emissionRate : 0;
	_remaining = 0;
    _emitting = true;
    _materialRandom = randInt();
}

CSParticleObject::Instance::Instance() : transform(NULL) {

}

CSParticleObject::Instance::~Instance() {
    if (transform) delete transform;
}

CSParticleObject::MeshFragment::MeshFragment(const CSAnimationSource* source, CSMeshObject* origin) : source(source), origin(retain(origin)) {

}

CSParticleObject::MeshFragment::~MeshFragment() {
    origin->release();
}

void CSParticleObject::addInstance(const CSMatrix* transform, float delta) {
    if (!_origin->shape || !_origin->sources.count() || _instances.count() >= MaxInstances) return;

    float life = _origin->life + _origin->lifeVar * randFloat(-1, 1);
    
    if (life <= delta) return;

    Instance& p = _instances.addObject();
    p.source = _origin->sources.objectAtIndex(randInt(0, _origin->sources.count() - 1));
    p.transform = transform ? new CSMatrix(*transform) : NULL;
    p.life = life;
    p.progress = delta;
    p.delta = delta;

    for (int i = 0; i < InstanceRandomCount; i++) p.randoms[i] = randFloat(0, 1);

    _origin->shape->issue(p.pos, p.firstDir, _origin->shapeShell);
}

void CSParticleObject::updateInstance(Instance& p, const CSMatrix& worldPrev, const CSMatrix* cameraView) {
    float progress = p.progress / p.life;
    
    int randomSeq = 0;

    p.color = _origin->color ? _origin->color->value(progress, CSColor(
        p.randoms[randomSeq++],
        p.randoms[randomSeq++],
        p.randoms[randomSeq++],
        p.randoms[randomSeq++]), CSColor::White) : CSColor::White;

    p.lastDir.x = _origin->x ? _origin->x->value(progress, p.randoms[randomSeq++]) : 0;
    p.lastDir.y = _origin->y ? _origin->y->value(progress, p.randoms[randomSeq++]) : 0;
    p.lastDir.z = _origin->z ? _origin->z->value(progress, p.randoms[randomSeq++]) : 0;

    if (_origin->radial) p.lastDir += p.firstDir * _origin->radial->value(progress, p.randoms[randomSeq++]);

    p.pos += p.lastDir * p.delta;

    p.billboardDir.x = _origin->billboardX ? _origin->billboardX->value(progress, p.randoms[randomSeq++]) : 0;
    p.billboardDir.y = _origin->billboardY ? _origin->billboardY->value(progress, p.randoms[randomSeq++]) : 0;
    p.billboardDir.z = _origin->billboardZ ? _origin->billboardZ->value(progress, p.randoms[randomSeq++]) : 0;
    p.billboard += p.billboardDir * p.delta;

    CSVector3 rotation;
    rotation.x = _origin->rotationX ? _origin->rotationX->value(progress, p.randoms[randomSeq++]) : 0;
    rotation.y = _origin->rotationY ? _origin->rotationY->value(progress, p.randoms[randomSeq++]) : 0;
    rotation.z = _origin->rotationZ ? _origin->rotationZ->value(progress, p.randoms[randomSeq++]) : 0;

    CSVector3 scale;
    scale.x = _origin->scaleX ? _origin->scaleX->value(progress, p.randoms[randomSeq++]) : 1;

    if (_origin->scaleEach) {
        scale.y = _origin->scaleY ? _origin->scaleY->value(progress, p.randoms[randomSeq++]) : 1;
        scale.z = _origin->scaleZ ? _origin->scaleZ->value(progress, p.randoms[randomSeq++]) : 1;
    }
    else scale.y = scale.z = scale.x;

    p.world = p.transform ? *p.transform : worldPrev;

    CSMatrix world = CSMatrix::Identity;

    if (cameraView) {
        switch (_origin->view) {
            case CSParticleBuilder::ViewBillboard:
                cameraView->billboard(&p.world, world);
                break;
            case CSParticleBuilder::ViewHorizontalBillboard:
                cameraView->horizontalBillboard(&p.world, world);
                break;
            case CSParticleBuilder::ViewVerticalBillboard:
                cameraView->verticalBillboard(&p.world, world);
                break;
            case CSParticleBuilder::ViewStretchBillboard:
                {
                    CSVector3 cameraX(cameraView->m11, cameraView->m21, cameraView->m31);       //TODO:CHECK
                    CSVector3 cameraY(-cameraView->m12, -cameraView->m22, -cameraView->m32);
                    CSVector3 cameraZ(cameraView->m13, cameraView->m23, cameraView->m33);

                    CSVector3 dir = p.lastDir;
                    dir += cameraX * p.billboardDir.x;
                    dir += cameraY * p.billboardDir.y;
                    dir += cameraZ * p.billboardDir.z;
                    cameraView->stretchBillboard(&p.world, dir, _origin->stretchRate, world);
                }
                break;
        }
    }
    if (rotation != CSVector3::Zero) {
        world = CSMatrix::rotationYawPitchRoll(rotation.y, rotation.x, rotation.z) * world;
    }
    if (scale.x != 1) {
        world.m11 *= scale.x;
        world.m12 *= scale.x;
        world.m13 *= scale.x;
    }
    if (scale.y != 1) {
        world.m21 *= scale.y;
        world.m22 *= scale.y;
        world.m23 *= scale.y;
    }
    if (scale.z != 1) {
        world.m31 *= scale.z;
        world.m32 *= scale.z;
        world.m33 *= scale.z;
    }
    world.m41 = p.pos.x;
    world.m42 = p.pos.y;
    world.m43 = p.pos.z;
    world *= worldPrev;

    if (cameraView) {
        if (p.billboard.x) {
            CSVector3 cameraX(cameraView->m11, cameraView->m21, cameraView->m31);       //TODO:CHECK

            CSVector3 offset = cameraX * p.billboard.x * p.world.m11;
            world.m41 += offset.x;
            world.m42 += offset.y;
            world.m43 += offset.z;
        }
        if (p.billboard.y) {
            CSVector3 cameraY(-cameraView->m12, -cameraView->m22, -cameraView->m32);

            CSVector3 offset = cameraY * p.billboard.y * p.world.m22;
            world.m41 += offset.x;
            world.m42 += offset.y;
            world.m43 += offset.z;
        }
        if (p.billboard.z) {
            CSVector3 cameraZ(cameraView->m13, cameraView->m23, cameraView->m33);

            CSVector3 offset = cameraZ * p.billboard.z * p.world.m33;
            world.m41 += offset.x;
            world.m42 += offset.y;
            world.m43 += offset.z;
        }
    }
    else {
        world.m41 += p.billboard.x * p.world.m11;
        world.m42 += p.billboard.y * p.world.m22;
        world.m43 += p.billboard.z * p.world.m33;
    }
    
    p.world = world;
    p.delta = 0;
}

CSSceneObject::UpdateState CSParticleObject::update(float delta, bool alive) {
    _progress += delta;
    _remaining += delta;
    _emitting &= alive;

    if (_emitting) return UpdateStateAlive;

    if (!alive && _origin->clear) _instances.removeAllObjects();
    else {
        for (CSQueue<Instance>::Iterator i = _instances.iterator(); i.remaining(); i.next()) {
            Instance& p = i.object();
            if (p.progress + _remaining >= p.life) i.remove();
        }
        if (_instances.count()) return alive ? UpdateStateAlive : UpdateStateFinishing;
    }
    return _origin->finish ? UpdateStateStopped : UpdateStateNone;
}

CSSceneObject::UpdateState CSParticleObject::onUpdate(float delta, bool alive, uint& flags) {
    uint inflags = flags;

    UpdateState state0 = CSSceneObject::onUpdate(delta, alive, flags);
    UpdateState state1 = update(delta, alive);

    if (inflags & UpdateFlagTransform) flags |= UpdateFlagAABB;

    switch (state0) {
        case UpdateStateNone:
            return state1;
        case UpdateStateFinishing:
            return state1 == UpdateStateAlive ? UpdateStateAlive : UpdateStateFinishing;
    }
    return state0;
}

uint CSParticleObject::onShow() {
    uint showFlags = CSSceneObject::onShow();

    _imageFragments.removeAllObjects();
    foreach (MeshFragment&, frag, &_meshFragments) frag.instances.removeAllObjects();

    CSMatrix worldPrev = CSMatrix::Identity;

    if (_origin->localSpace && !getTransform(NULL, worldPrev)) {
        _visible = false;
        _meshFragments.removeAllObjects();
        return 0;
    }

    CSScene* scene = this->scene();
    const CSCamera* camera = scene ? &scene->camera() : NULL;

    if (_remaining) {
        for (CSQueue<Instance>::Iterator i = _instances.iterator(); i.remaining(); i.next()) {
            Instance& p = i.object();
            p.delta = _remaining;
            p.progress += _remaining;
            if (p.progress >= p.life) i.remove();
        }
        if (_emitting && _origin->emissionRate > 0) {
            float rate = 1 / _origin->emissionRate;

            float lifeMax = _origin->life + _origin->lifeVar;

            _counter += _remaining;
            if (_counter > lifeMax) {
                _counter = CSMath::mod(_counter, rate);
                _counter += CSMath::ceil(lifeMax / rate) * rate;
            }

            while (_counter >= rate) {
                _counter -= rate;

                if (_counter < lifeMax && _instances.count() < _origin->emissionMax) {
                    CSMatrix transform;
                    if (getTransform(_progress - _counter, NULL, transform)) {
                        addInstance(_origin->localSpace ? NULL : &transform, _counter);
                    }
                }
            }
        }

        const CSMatrix* cameraView = camera ? &camera->view() : NULL;

        CSArray<CSTaskBase*> tasks(_instances.count());
        for (CSQueue<Instance>::Iterator i = _instances.iterator(); i.remaining(); i.next()) {
            Instance* p = &i.object();
            tasks.addObject(CSThreadPool::run<void>([this, p, worldPrev, cameraView]() {
                updateInstance(*p, worldPrev, cameraView);
            }));
        }
        CSTaskBase::finishAll(&tasks);

        _remaining = 0;
    }

    for (CSQueue<Instance>::ReadonlyIterator i = _instances.iterator(); i.remaining(); i.next()) {
        const Instance& p = i.object();

        switch (p.source->type()) {
            case CSAnimationSource::TypeImage:
                {
                    bool isNew = true;
                    for (int j = 0; j < _imageFragments.count(); j++) {
                        const CSAnimationSource* otherSource = _imageFragments.objectAtIndex(j)->firstObject()->source;

                        if (p.source == otherSource) {
                            _imageFragments.objectAtIndex(j)->addObject(&p);
                            isNew = false;
                            break;
                        }
                    }
                    if (isNew) {
                        CSArray<const Instance*>* frag = new CSArray<const Instance*>(1);
                        frag->addObject(&p);
                        _imageFragments.addObject(frag);
                        frag->release();
                    }
                }
                break;
            case CSAnimationSource::TypeMesh:
                {
                    const CSAnimationSourceMesh* meshSource = static_cast<const CSAnimationSourceMesh*>(p.source.value());

                    if (meshSource->builder) {
                        showFlags |= meshSource->builder->showFlags();

                        bool isNew = true;

                        foreach (MeshFragment&, frag, &_meshFragments) {
                            if (frag.source == p.source && (frag.instances.count() == 0 || frag.origin->clippedProgress(p.progress) == frag.origin->clippedProgress())) {
                                if (scene != frag.origin->scene()) {
                                    frag.origin->unlink();
                                    if (scene) frag.origin->link(scene);
                                }
                                frag.origin->setProgress(p.progress);
                                new (&frag.instances.addObject()) CSVertexArrayInstance(p.world, p.color);
                                isNew = false;
                            }
                        }

                        if (isNew) {
                            CSMeshObject* origin = meshSource->builder->createObject();
                            if (origin) {
                                if (scene) origin->link(scene);
                                origin->setProgress(p.progress);
                                MeshFragment& frag = _meshFragments.addObject();
                                new (&frag) MeshFragment(p.source, origin);
                                new (&frag.instances.addObject()) CSVertexArrayInstance(p.world, p.color);
                                origin->release();
                            }
                        }
                    }
                }
                break;
        }
    }
    int i = 0;
    while (i < _meshFragments.count()) {
        MeshFragment& frag = _meshFragments.objectAtIndex(i);
        if (frag.instances.count()) i++;
        else {
            frag.origin->unlink();
            _meshFragments.removeObjectAtIndex(i);
        }
    }

    if (camera) {
        const CSVector3& cp = camera->position();

        auto sort = [cp](const Instance* a, const Instance* b) -> int {
            float d0 = CSVector3::distanceSquared(cp, a->world.translationVector());
            float d1 = CSVector3::distanceSquared(cp, b->world.translationVector());
            if (d0 < d1) return -1;
            if (d0 > d1) return 1;
            return 0;
        };

        foreach (CSArray<const Instance*>*, frag, &_imageFragments) frag->sort(sort);
    }
    _visible = true;

    return showFlags;
}

void CSParticleObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
    if (!_visible) return;

    if (_imageFragments.count()) {
        foreach (const CSArray<const Instance*>*, frag, &_imageFragments) {
            const CSAnimationSourceImage* source = static_cast<const CSAnimationSourceImage*>(frag->firstObject()->source.value());

            if (CSMaterialSource::apply(source->material, graphics, layer, _progress, _materialRandom, NULL, false)) {
                const CSImage* root = static_assert_cast<CSImage*>(CSResourcePool::sharedPool()->load(CSResourceTypeImage, source->rootIndices));

                if (root) {
                    CSStreamRenderCommand* command = new CSStreamRenderCommand(graphics, CSPrimitiveTriangles, frag->count() * 4, frag->count() * 6);
                    const CSTexture* texture = root->texture();
                    command->state.material.colorMap = texture;

                    int vi = 0;

                    foreach (const Instance*, p, frag) {
                        command->world = p->world;
                        command->state.material.color = p->color;

                        const CSImage* sub = root;
                        if (source->subIndices && source->subIndices->count()) {
                            sub = root->sub(CSMath::round((root->subCount() - 1) * p->progress / p->life));
                        }

                        const CSRect& frame = sub->frame();
                        float rx = frame.width * sub->contentScale() * 0.5f;
                        float lx = -rx;
                        float by = frame.height * sub->contentScale() * 0.5f;
                        float ty = -by;
                        float lu = frame.left() / texture->width();
                        float ru = frame.right() / texture->width();
                        float tv = frame.top() / texture->height();
                        float bv = frame.bottom() / texture->height();

                        command->addIndex(vi + 0);
                        command->addIndex(vi + 1);
                        command->addIndex(vi + 1);
                        command->addIndex(vi + 3);
                        command->addIndex(vi + 3);
                        command->addIndex(vi + 2);
                        command->addIndex(vi + 2);
                        command->addIndex(vi + 0);

                        vi += 4;

                        command->addVertex(CSFVertex(CSVector3(lx, ty, 0), CSVector2(lu, tv)));
                        command->addVertex(CSFVertex(CSVector3(rx, ty, 0), CSVector2(ru, tv)));
                        command->addVertex(CSFVertex(CSVector3(lx, by, 0), CSVector2(lu, bv)));
                        command->addVertex(CSFVertex(CSVector3(rx, by, 0), CSVector2(ru, bv)));
                    }

                    graphics->command(command);

                    command->release();
                }
            }
        }
    }
    foreach (const MeshFragment&, frag, &_meshFragments) {
        frag.origin->drawIndirect(graphics, layer, _progress, _materialRandom, &frag.instances);
    }
}
