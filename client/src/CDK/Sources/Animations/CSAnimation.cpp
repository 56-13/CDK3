#define CDK_IMPL

#include "CSAnimation.h"

#include "CSResourcePool.h"
#include "CSBuffer.h"

CSAnimation::CSAnimation() : _root(new CSAnimationFragment()) {

}

CSAnimation::CSAnimation(CSBuffer* buffer) : _root(new CSAnimationFragment(buffer)) {

}

CSAnimation::CSAnimation(const CSAnimation* other) : _root(new CSAnimationFragment(other->_root)) {

}

CSAnimation::~CSAnimation() {
	_root->release();
}

int CSAnimation::resourceCost() const {
	return sizeof(CSAnimation) + _root->resourceCost();
}

//========================================================================================

CSAnimationBuilder::CSAnimationBuilder(const CSArray<ushort>* indices) : _indices(retain(indices)) {

}

CSAnimationBuilder::CSAnimationBuilder(const CSAnimation* animation) : _animation(retain(animation)) {

}

CSAnimationBuilder::CSAnimationBuilder(CSBuffer* buffer, bool withScene, bool withData) : CSSceneObjectBuilder(buffer, withScene) {
	if (withScene) {
		if (buffer->readBoolean()) targets[0] = CSGizmoData::dataWithBuffer(buffer);
		if (buffer->readBoolean()) targets[1] = CSGizmoData::dataWithBuffer(buffer);
		if (buffer->readBoolean()) targets[2] = CSGizmoData::dataWithBuffer(buffer);
	}

	keyFlags = buffer->readInt();

	if (withData) _animation = new CSAnimation(buffer);
	else _indices = retain(buffer->readArray<ushort>());
}

CSAnimationBuilder::CSAnimationBuilder(const CSAnimationBuilder* other) : 
	CSSceneObjectBuilder(other), 
	targets {
		other->targets[0] ? CSGizmoData::dataWithData(other->targets[0]) : NULL,
		other->targets[1] ? CSGizmoData::dataWithData(other->targets[1]) : NULL,
		other->targets[2] ? CSGizmoData::dataWithData(other->targets[2]) : NULL
	},
	keyFlags(other->keyFlags),
	_indices(retain(other->_indices)), 
	_animation(retain(other->_animation)) 
{

}

CSAnimationBuilder::~CSAnimationBuilder() {
	release(_indices);
	release(_animation);
}

const CSAnimation* CSAnimationBuilder::origin() const {
	const CSAnimation* origin = _animation;
	if (!origin) origin = static_assert_cast<CSAnimation*>(CSResourcePool::sharedPool()->load(CSResourceTypeAnimation, _indices));
	return origin;
}

int CSAnimationBuilder::resourceCost() const {
	int cost = sizeof(CSAnimationBuilder) + resourceCostBase();
	if (targets[0]) cost += targets[0]->resourceCost();
	if (targets[1]) cost += targets[1]->resourceCost();
	if (targets[2]) cost += targets[2]->resourceCost();
	if (_animation) cost += _animation->resourceCost();
	else if (_indices) cost += sizeof(CSArray<ushort>) + _indices->capacity() * sizeof(ushort);
	return cost;
}

CSAnimationObject* CSAnimationBuilder::createObject() const {
	const CSAnimation* origin = this->origin();
	return origin ? new CSAnimationObject(origin) : NULL;
}

void CSAnimationBuilder::preload() const {
	const CSAnimation* origin = this->origin();
	if (origin) origin->preload();
}

//========================================================================================

CSAnimationObject::CSAnimationObject(const CSAnimation* origin, const CSAnimationBuilder* builder) :
	CSSceneObject(builder),
	_root(new CSAnimationObjectFragment(origin->root())), 
	_targets {},
	_alive(true)
{
	if (builder) {
		if (builder->targets[0]) _targets[0] = new CSGizmo(this, builder->targets[0]);
		if (builder->targets[1]) _targets[1] = new CSGizmo(this, builder->targets[1]);
		if (builder->targets[2]) _targets[2] = new CSGizmo(this, builder->targets[2]);
		_keyFlags = builder->keyFlags;
	}
	else _keyFlags = 0xFFFFFFFF;

	_root->link(this, NULL);
}

CSAnimationObject::~CSAnimationObject() {
	_root->release();
	if (_targets[0]) delete _targets[0];
	if (_targets[1]) delete _targets[1];
	if (_targets[2]) delete _targets[2];
}

void CSAnimationObject::useTarget(int i, bool flag) {
	switch (i) {
		case 1:
			if (!_targets[0]) _targets[0] = new CSGizmo(this);
			break;
		case 2:
			if (!_targets[1]) _targets[1] = new CSGizmo(this);
			break;
		case 3:
			if (!_targets[2]) _targets[2] = new CSGizmo(this);
			break;
	}
}

CSGizmo* CSAnimationObject::target(int i) {
	switch (i) {
		case 1:
			return _targets[0];
		case 2:
			return _targets[1];
		case 3:
			return _targets[2];
	}
	return transform();
}

bool CSAnimationObject::getTargetTransform(int i, float progress, CSMatrix& result) const {
	switch (i) {
		case 1:
			return _targets[0] && _targets[0]->getTransform(progress, result);
		case 2:
			return _targets[1] && _targets[1]->getTransform(progress, result);
		case 3:
			return _targets[2] && _targets[2]->getTransform(progress, result);
	}
	return getTransform(progress, NULL, result);
}

bool CSAnimationObject::getTransform(float progress, const string& name, CSMatrix& result) const {
	if (name) return _root->getTransform(progress, name, result);

	return CSSceneObject::getTransform(progress, NULL, result);
}

bool CSAnimationObject::getUpdatePass(CSUpdatePass& pass) const {
	if (!CSSceneObject::getUpdatePass(pass)) return false;
	if (_targets[0] && !_targets[0]->getUpdatePass(pass)) return false;
	if (_targets[1] && !_targets[1]->getUpdatePass(pass)) return false;
	if (_targets[2] && !_targets[2]->getUpdatePass(pass)) return false;
	return true;
}

void CSAnimationObject::onLink() {
	CSSceneObject::onLink();

	_root->link(this, NULL);
}

void CSAnimationObject::onUnlink() {
	CSSceneObject::onUnlink();

	_root->link(this, NULL);
}

void CSAnimationObject::onRewind() {
	CSSceneObject::onRewind();

	_root->rewind();
}

CSSceneObject::UpdateState CSAnimationObject::onUpdate(float delta, bool alive, uint& flags) {
	alive &= _alive;

	uint inflags = flags;

	UpdateState state0 = CSSceneObject::onUpdate(delta, alive, flags);

	if (_targets[0] && _targets[0]->update()) inflags |= UpdateFlagTransform;
	if (_targets[1] && _targets[1]->update()) inflags |= UpdateFlagTransform;
	if (_targets[2] && _targets[2]->update()) inflags |= UpdateFlagTransform;

	UpdateState state1 = _root->update(delta, _alive, inflags, flags);

	switch (state0) {
		case UpdateStateNone:
			return state1;
		case UpdateStateFinishing:
			return state1 == UpdateStateAlive ? UpdateStateAlive : UpdateStateFinishing;
	}
	return state0;
}

uint CSAnimationObject::onShow() {
	uint showFlags = CSSceneObject::onShow();
	showFlags |= _root->show();
	return showFlags;
}

void CSAnimationObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	_root->draw(graphics, layer);

	CSSceneObject::onDraw(graphics, layer);
}

