#define CDK_IMPL

#include "CSSceneObject.h"

#include "CSBuffer.h"

#include "CSScene.h"

#include "CSAnimation.h"

CSSceneObject::CSSceneObject(const CSSceneObjectBuilder* builder) {
	if (builder) {
		dataId = builder->dataId;
		_located = builder->located;
		if (builder->transform) _transform = new CSGizmo(this, builder->transform);
		foreach (const CSSceneObjectBuilder*, childBuilder, builder->children.value()) {
			CSSceneObject* child = childBuilder->createObject();
			addChild(child);
			child->release();
		}
	}
}

CSSceneObject::~CSSceneObject() {
	release(_children);
	if (_transform) delete _transform;
	release(_tag);
}

bool CSSceneObject::link(CSScene* scene) {
	if (!_scene) {
		_scene = scene;
		onLink();
		if (_located) doLocate();
		foreach (CSSceneObject*, child, _children) child->link(scene);
		return true;
	}
	return false;
}

bool CSSceneObject::link(CSAnimationObjectFragment* animation) {
	if ((!_animation || _animation == animation) && !_located && !_transform && !_parent) {
		CSScene* scene = animation->root() ? animation->root()->scene() : NULL;
		if (_scene != scene) {
			if (_scene) unlink();
			if (scene) link(scene);
		}
		_animation = animation;
		return true;
	}
	return false;
}

void CSSceneObject::unlink() {
	if (_scene) {
		if (_located) {
			_scene->unlocate(this);
			onUnlocate();
		}
		foreach (CSSceneObject*, child, _children) child->unlink();
		onUnlink();
		_scene = NULL;
	}
	_animation = NULL;
}

bool CSSceneObject::addChild(CSSceneObject* child) {
	if (!child->_animation && !child->_parent && !child->_scene) {
		if (!_children) _children = new CSArray<CSSceneObject>();
		child->_parent = this;
		if (_scene) child->link(_scene);
		_children->addObject(child);
		return true;
	}
	return false;
}

bool CSSceneObject::removeChild(CSSceneObject* child) {
	if (_children && _children->removeObjectIdenticalTo(child)) {
		if (_scene) child->unlink();
		child->_parent = NULL;
		return true;
	}
	return false;
}

void CSSceneObject::doLocate() {
	_located = false;		//prevent relocacte
	update(0);
	if (_updateFlags & UpdateFlagAABB) {
		CSABoundingBox aabb = CSABoundingBox::None;
		if ((_aabbFlag = addAABB(aabb))) _aabb = aabb;
		_updateFlags &= ~UpdateFlagAABB;
	}
	_located = true;
	_scene->locate(this);
	onLocate();
}

void CSSceneObject::locate() {
	if (!_located && !_animation) {
		if (_scene) doLocate();
		else _located = true;
	}
}

void CSSceneObject::unlocate() {
	if (_located) {
		if (_scene) {
			_scene->unlocate(this);
			onUnlocate();
		}
		_located = false;
	}
}

void CSSceneObject::useTransform(bool flag) {
	if (flag) {
		if (!_transform && !_animation) {
			_transform = new CSGizmo(this);
		}
	}
	else {
		if (_transform) {
			delete _transform;
			_transform = NULL;
			_updateFlags |= UpdateFlagTransform;
		}
	}
}

bool CSSceneObject::getTransform(float progress, const string& name, CSMatrix& result) const {
	if (!name) {
		if (_animation) return _animation->getTransform(progress + _animation->progress() - this->progress(), NULL, result);
		if (_transform) return _transform->getTransform(progress, result);
		if (_parent) return _parent->getTransform(progress + _parent->progress() - this->progress(), NULL, result);
	}
	return false;
}

bool CSSceneObject::fromGround() const {
	if (_animation) return _animation->root()->fromGround();
	if (_transform && _transform->fromGround()) return true;
	return _parent && _parent->fromGround();
}

void CSSceneObject::rewind() {
	onRewind();
	_updated = 0;
}

CSSceneObject::UpdateState CSSceneObject::update(float delta, bool alive, uint& flags) {
	if (_transform) {
		if (_transform->update()) _updateFlags |= UpdateFlagTransform;
	}
	else if (_parent) {
		if (_parent->transformUpdated()) _updateFlags |= UpdateFlagTransform;
	}
	
	flags |= _updateFlags;
	_updateFlags = 0;

	UpdateState result = onUpdate(delta, alive, flags);

	if ((flags & UpdateFlagAABB)) {
		if (_located && _scene) {
			CSABoundingBox aabb = CSABoundingBox::None;
			bool aabbFlag = addAABB(aabb);
			if (aabb != _aabb || aabbFlag != _aabbFlag) {
				_scene->relocate(this, aabbFlag ? &aabb : NULL);
				_aabb = aabb;
				_aabbFlag = aabbFlag;
			}
		}
		else _updateFlags = UpdateFlagAABB;
	}

	_transformUpdated = (flags & UpdateFlagTransform) != 0;
	_updated = 1;

	return result;
}

bool CSSceneObject::afterCameraUpdate() const {
	if (_parent && _parent->afterCameraUpdate()) return true;
	if (_animation) {
		const CSAnimationObjectFragment* a = _animation;
		for (;;) {
			if (a->origin()->billboard) return true;
			else a = a->parent();
		}
	}
	return false;
}

bool CSSceneObject::getUpdatePass(CSUpdatePass& pass) const {
	if (!pass.remaining()) return false;
	const_cast<CSSceneObject*>(this)->OnUpdatePass(const_cast<CSSceneObject*>(this), pass);
	if (!pass.remaining()) return false;
	if (_parent && !pass.addPrecedence(_parent)) return false;
	if (afterCameraUpdate()) {
		const CSScene* scene = this->scene();
		if (scene) {
			const CSCameraObject* camera = scene->cameraObject();
			if (camera && !pass.addPrecedence(camera)) return false;
		}
	}
	if (_transform && !_transform->getUpdatePass(pass)) return false;
	return true;
}

uint CSSceneObject::show() {
	if (_updated == 0) update(0);
	uint showFlags = onShow();
	_updated = 2;
	return showFlags;
}

void CSSceneObject::draw(CSGraphics* graphics, CSInstanceLayer layer) {
	if (_updated != 2) show();
	onDraw(graphics, layer);
}

void CSSceneObject::onLink() {
	OnLink(this);
}

void CSSceneObject::onUnlink() {
	OnUnlink(this);
}

void CSSceneObject::onLocate() {
	OnLocate(this);
}

void CSSceneObject::onUnlocate() {
	OnUnlocate(this);
}

void CSSceneObject::onRewind() {
	OnRewind(this);
}

CSSceneObject::UpdateState CSSceneObject::onUpdate(float delta, bool alive, uint& flags) {
	UpdateState state = UpdateStateNone;
	OnUpdate(this, delta, alive, flags, state);
	return state;
}

uint CSSceneObject::onShow() {
	uint showFlags = 0;
	OnShow(this, showFlags);
	return showFlags;
}

void CSSceneObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	const CSArray<CSHandler<CSSceneObject*, CSGraphics*, CSInstanceLayer>::Callback>* callbacks = OnDraw.callbacks();
	if (callbacks && callbacks->count()) { 
		graphics->push();
		if (callbacks->count() == 1) {
			callbacks->objectAtIndex(0).func(this, graphics, layer);
		}
		else {
			for (int i = 0; i < callbacks->count(); i++) {
				callbacks->objectAtIndex(i).func(this, graphics, layer);
				if (i + 1 < callbacks->count()) graphics->reset();
			}
		}
		graphics->pop();
	}
}
