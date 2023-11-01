#define CDK_IMPL

#include "CSCameraObject.h"

#include "CSBuffer.h"

#include "CSScene.h"

CSCameraBuilder::CSCameraBuilder(CSBuffer* buffer, bool withScene) :
    CSSceneObjectBuilder(buffer, withScene)
{
    if (buffer->readBoolean()) target = CSGizmoData::dataWithBuffer(buffer);
    frustum = buffer->readBoolean();
    if (frustum) {
        fov = buffer->readFloat();
        znear = buffer->readFloat();
        zfar = buffer->readFloat();
    }
    blur = buffer->readBoolean();
    if (blur) {
        blurDistance = buffer->readFloat();
        blurRange = buffer->readFloat();
        blurIntensity = buffer->readFloat();
    }
}

CSCameraBuilder::CSCameraBuilder(const CSCameraBuilder* other) :
    CSSceneObjectBuilder(other),
    target(other->target ? CSGizmoData::dataWithData(other->target) : NULL),
    frustum(other->frustum),
    fov(other->fov),
    znear(other->znear),
    zfar(other->zfar),
    blur(other->blur),
    blurDistance(other->blurDistance),
    blurRange(other->blurRange),
    blurIntensity(other->blurIntensity)
{
    
}

int CSCameraBuilder::resourceCost() const {
    return sizeof(CSCameraBuilder) + resourceCostBase();
}

//=======================================================================================

CSCameraObject::CSCameraObject(const CSCameraBuilder* origin) : 
    CSSceneObject(origin),
    _origin(retain(origin)),
    _target(origin->target ? new CSGizmo(this, origin->target) : NULL),
    frustum(origin->frustum),
    fov(origin->fov),
    znear(origin->znear),
    zfar(origin->zfar),
    blur(origin->blur),
    blurDistance(origin->blurDistance),
    blurRange(origin->blurRange),
    blurIntensity(origin->blurIntensity)
{

}

CSCameraObject::~CSCameraObject() {
    _origin->release();
    if (_target) delete _target;
}

void CSCameraObject::useTarget(bool flag) {
    if (flag) {
        if (!_target) {
            _target = new CSGizmo(this);
        }
    }
    else {
        if (_target) {
            delete _target;
            _target = NULL;
            addUpdateFlags(UpdateFlagTransform);
        }
    }
}

void CSCameraObject::reset() {
    if (_origin->target) {
        if (_target) delete _target;
        _target = new CSGizmo(this, _origin->target);
    }
    else if (_target) {
        delete _target;
        _target = NULL;
    }
    addUpdateFlags(UpdateFlagTransform);

    frustum = _origin->frustum;
    fov = _origin->fov;
    znear = _origin->znear;
    zfar = _origin->zfar;
    blur = _origin->blur;
    blurDistance = _origin->blurDistance;
    blurRange = _origin->blurRange;
    blurIntensity = _origin->blurIntensity;
}

bool CSCameraObject::isFocused() const {
    const CSScene* scene = this->scene();

    return scene && scene->cameraObject() == this;
}

bool CSCameraObject::focus() {
    CSScene* scene = this->scene();

    return scene && scene->setCameraObject(this);
}

bool CSCameraObject::capture(CSCamera& camera) const {
    if (frustum) {
        camera.setFov(fov);
        camera.setZNear(znear);
        camera.setZFar(zfar);
    }

    CSMatrix transform;
    if (!getTransform(transform)) return false;

    camera.setPosition(transform.translationVector());
    
    CSMatrix targetTransform;
    if (_target && _target->getTransform(targetTransform)) {
        camera.setTarget(targetTransform.translationVector());

        CSVector3 forward = CSVector3::normalize(camera.target() - camera.position());
        if (CSVector3::nearEqual(forward, CSVector3::UnitZ)) camera.setUp(CSVector3::UnitY);
        else if (CSVector3::nearEqual(forward, -CSVector3::UnitZ)) camera.setUp(-CSVector3::UnitY);
        else {
            CSVector3 right = CSVector3::normalize(CSVector3::cross(forward, -CSVector3::UnitZ));
            CSVector3 up = CSVector3::normalize(CSVector3::cross(forward, right));
            camera.setUp(up);
        }
    }
    else {
        camera.setTarget(transform.translationVector() + transform.forward());
        camera.setUp(-transform.up());
    }
    return true;
}

void CSCameraObject::filter(CSGraphics* graphics) const {
    if (blur) {
        float distance = blurDistance > 0 ? blurDistance : CSVector3::distance(graphics->camera().position(), graphics->camera().target());
        graphics->blurDepth(distance, blurRange, blurIntensity);
    }
}

void CSCameraObject::onUnlink() {
    CSScene* scene = this->scene();

    if (scene && scene->cameraObject() == this) {
        scene->setCameraObject(NULL);
    }
}
