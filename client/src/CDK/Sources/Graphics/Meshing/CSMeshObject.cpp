#define CDK_IMPL

#include "CSMeshObject.h"

#include "CSRandom.h"

static constexpr int BoneBufferSliceLife = 300;

CSMeshObject::CSMeshObject(const CSMeshGeometry* geometry, const CSSceneObjectBuilder* builder) : 
	CSSceneObject(builder), 
	_geometry(retain(geometry)), 
	_progress(0),
	_clippedProgress(0),
	_materialRandom(randInt()),
	collision(false),
	_frameDivision(0),
	_dirtyTransforms(false),
	_customTransforms(NULL),
	_transforms(NULL)
{

}

CSMeshObject::CSMeshObject(const CSMeshObject* other) :
	_geometry(retain(other->_geometry)),
	_skin(retain(other->_skin)),
	_animation(retain(other->_animation)),
	_progress(other->_progress),
	_clippedProgress(other->_clippedProgress),
	_materialRandom(other->_materialRandom),
	loop(other->loop),
	collision(other->collision),
	_frameDivision(other->_frameDivision),
	_dirtyTransforms(false),
	_customTransforms(NULL),
	_transforms(NULL)
{
	other->updateTransforms();

	if (other->_customTransforms) _customTransforms = new CSDictionary<string, CSMatrix>(other->_customTransforms);
	if (other->_transforms) _transforms = new CSDictionary<string, CSMatrix>(other->_transforms);
}

CSMeshObject::~CSMeshObject() {
	_geometry->release();
	release(_skin);
	release(_animation);
	release(_customTransforms);
	release(_transforms);
}

void CSMeshObject::setAnimation(const CSMeshAnimation* animation) {
	if (retain(_animation, animation)) {
		_progress = _clippedProgress = 0;
		_dirtyTransforms = true;
		addUpdateFlags(UpdateFlagTransform);
	}
}

void CSMeshObject::setFrameDivision(int frameDivision) {
	if (_frameDivision != frameDivision) {
		_frameDivision = frameDivision;
		if (clipProgress()) addUpdateFlags(UpdateFlagTransform);
	}
}

float CSMeshObject::clippedProgress(float progress) const {
	if (_animation) {
		float duration = _animation->duration();
		progress = loop.getProgress(progress / duration) * duration;
		if (progress < duration && _frameDivision) {
			progress = CSMath::round(progress * _frameDivision) / _frameDivision;
		}
	}
	else progress = 0;
	
	return progress;
}

bool CSMeshObject::clipProgress() {
	float p = clippedProgress(_progress);
	if (_clippedProgress != p) {
		_clippedProgress = p;
		_dirtyTransforms = true;
		return true;
	}
	return false;
}

void CSMeshObject::setProgress(float progress) {
	_progress = progress;
	if (clipProgress()) addUpdateFlags(UpdateFlagTransform);
}

bool CSMeshObject::addAABB(CSABoundingBox& result) const {
	CSABoundingBox aabb;
	if (!getAABB(aabb)) return false;
	result.append(aabb);
	return true;
}

void CSMeshObject::addCollider(CSCollider*& result) const {
	if (collision) {
		const CSArray<CSMeshGeometry::ColliderFragment>* origin = _geometry->colliderFragments();

		if (origin) {
			if (!result) result = CSCollider::colliderWithCapacity(origin->count());

			CSMatrix trans;
			foreach (const CSMeshGeometry::ColliderFragment&, originfrag, origin) {
				if (getTransform(_progress, originfrag.name, trans)) {
					result->addObject(CSColliderFragment::transform(originfrag.origin, trans));
				}
			}
		}
	}
}

bool CSMeshObject::getTransform(float progress, const string& name, CSMatrix& result) const {
	if (name) {
		progress = clippedProgress(progress);
		if (!_animation || CSMath::abs(progress - _clippedProgress) < _frameDivision ? 1.0f / _frameDivision : CSMath::ZeroTolerance) {
			return getNodeTransform(name, result);
		}
		else {
			if (!CSSceneObject::getTransform(progress, NULL, result)) return false;
			CSMatrix nt;
			if (!_geometry->getNodeTransform(name, _animation, progress, nt)) return false;
			result = nt * result;
			return true;
		}
	}
	return CSSceneObject::getTransform(progress, NULL, result);
}

float CSMeshObject::duration(DurationParam param, float duration) const {
	return _animation ? _animation->duration() * loop.count : duration;
}

void CSMeshObject::onRewind() {
	CSSceneObject::onRewind();

	setProgress(0);
	_materialRandom = randInt();
}

CSSceneObject::UpdateState CSMeshObject::onUpdate(float delta, bool alive, uint& flags) {
	uint inflags = flags;

	UpdateState state0 = CSSceneObject::onUpdate(delta, alive, flags);

	_progress += delta;
	if (clipProgress() || (inflags & UpdateFlagTransform)) flags |= UpdateFlagTransform | UpdateFlagAABB;

	UpdateState state1;
	if (!_animation || loop.count == 0) state1 = UpdateStateNone;
	else if (_progress < _animation->duration() * loop.count) state1 = UpdateStateAlive;
	else state1 = loop.finish ? UpdateStateStopped : UpdateStateNone;

	switch (state0) {
		case UpdateStateNone:
			return state1;
		case UpdateStateFinishing:
			return state1 == UpdateStateAlive ? UpdateStateAlive : UpdateStateFinishing;
	}
	return state0;
}

void CSMeshObject::onDraw(CSGraphics* graphics, CSInstanceLayer layer) {
	CSMatrix transform;
	if (CSSceneObject::getTransform(_progress, NULL, transform)) {
		CSMatrix world = graphics->world();
		graphics->world() = transform * world;
		drawIndirect(graphics, layer, _progress, _materialRandom);
		graphics->world() = world;
	}
}

void CSMeshObject::setCustomTransform(const string& name, const CSMatrix& transform) {
	if (!_customTransforms) _customTransforms = new CSDictionary<string, CSMatrix>();
	else {
		CSMatrix prev;
		if (_customTransforms->tryGetObjectForKey(name, prev) && prev == transform) return;
	}
	_customTransforms->setObject(name, transform);
	_dirtyTransforms = true;
	addUpdateFlags(UpdateFlagTransform);
}

void CSMeshObject::removeCustomTransform(const string& name) {
	if (_customTransforms && _customTransforms->removeObject(name)) {
		if (_customTransforms->count() == 0) {
			_customTransforms->release();
			_customTransforms = NULL;
		}
		_dirtyTransforms = true;
		addUpdateFlags(UpdateFlagTransform);
	}
}

void CSMeshObject::clearCustomTransforms() {
	if (_customTransforms) {
		_customTransforms->release();
		_customTransforms = NULL;
		_dirtyTransforms = true;
		addUpdateFlags(UpdateFlagTransform);
	}
}

void CSMeshObject::getNodeLocalTransform(const CSMeshNode* node, CSMatrix& result) const {
	if (!_transforms || !_transforms->tryGetObjectForKey(node->name(), result)) _geometry->getNodeTransform(node, NULL, 0, result);
}

bool CSMeshObject::getNodeTransform(const CSMeshNode* node, CSMatrix& result) const {
	if (!CSSceneObject::getTransform(_progress, NULL, result)) return false;
	result = getNodeLocalTransform(node) * result;
	return true;
}

bool CSMeshObject::getNodeTransform(const string& name, CSMatrix& result) const {
	const CSMeshNode* node = _geometry->findNode(name);
	return node && getNodeTransform(node, result);
}

bool CSMeshObject::getNodeAABB(const CSMeshNode* node, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const {
	updateTransforms();
	result = CSABoundingBox::None;
	return getNodeTransform(node, transform) && _geometry->getNodeAABBInternal(node, _animation, _clippedProgress, inclusive, CSMatrix::Identity, false, result);
}

bool CSMeshObject::getNodeAABB(const CSMeshNode* node, bool inclusive, CSABoundingBox& result) const {
	updateTransforms();
	result = CSABoundingBox::None;
	CSMatrix transform;
	return getNodeTransform(node, transform) && _geometry->getNodeAABBInternal(node, _animation, _clippedProgress, inclusive, transform, false, result);
}

bool CSMeshObject::getNodeAABB(const string& name, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const {
	const CSMeshNode* node = _geometry->findNode(name);
	if (!node) return false;
	return getNodeAABB(node, inclusive, transform, result);
}

bool CSMeshObject::getNodeAABB(const string& name, bool inclusive, CSABoundingBox& result) const {
	const CSMeshNode* node = _geometry->findNode(name);
	if (!node) return false;
	return getNodeAABB(node, inclusive, result);
}

bool CSMeshObject::getAABB(CSABoundingBox& result) const {
	if (!_animation && !_customTransforms) {
		CSMatrix transform;
		if (!CSSceneObject::getTransform(NULL, transform)) return false;
		CSVector3 corners[8];
		_geometry->aabb().getCorners(corners);
		result = CSABoundingBox::None;
		for (int i = 0; i < 8; i++) result.append(CSVector3::transformCoordinate(corners[i], transform));
		return true;
	}
	return getNodeAABB(_geometry->rootNode(), true, result);
}

void CSMeshObject::updateTransforms(const CSMeshNode* node, const CSMatrix& parentTransform) const {
	CSMatrix transform;
	if (_customTransforms && _customTransforms->tryGetObjectForKey(node->name(), transform)) {
		CSMatrix animationTransform;
		if (_animation && _animation->getNodeTransform(node->name(), _clippedProgress, animationTransform)) {
			transform = CSMatrix::invert(node->localTransform()) * animationTransform * transform;
		}
	}
	else {
		if (!_animation || !_animation->getNodeTransform(node->name(), _clippedProgress, transform)) transform = node->localTransform();
		transform *= parentTransform;
	}

	_transforms->setObject(node->name(), transform);

	foreach (const CSMeshNode*, child, node->children()) updateTransforms(child, transform);
}

void CSMeshObject::updateTransforms() const {
	if (_dirtyTransforms) {
		if (_animation || _customTransforms) {
			if (_transforms) _transforms = new CSDictionary<string, CSMatrix>();
			updateTransforms(_geometry->rootNode(), CSMatrix::Identity);
		}
		else release(_transforms);

		_dirtyTransforms = false;
	}
}

class BoneKey : public CSObject {
private:
	void* _instance;
	const void* _fragment;
	const void* _animation;
	float _progress;
public:
	BoneKey(CSMeshObject* instance, const CSMeshFragment& frag, const CSMeshAnimation* animation, float progress) : 
		_instance(instance), 
		_fragment(&frag), 
		_animation(animation), 
		_progress(progress) 
	{

	}

	uint hash() const override {
		CSHash hash;
		hash.combine(_instance);
		hash.combine(_fragment);
		hash.combine(_animation);
		hash.combine(_progress);
		return hash;
	}

	bool isEqual(const BoneKey* other) const {
		return _instance == other->_instance && _fragment == other->_fragment && _animation == other->_animation && _progress == other->_progress;
	}

	bool isEqual(const CSObject* obj) const override {
		const BoneKey* other = dynamic_cast<const BoneKey*>(obj);
		
		return other && isEqual(other);
	}
};

void CSMeshObject::drawIndirect(CSGraphics* graphics, CSInstanceLayer layer, float materialProgress, int materialRandom, const CSArray<CSVertexArrayInstance>* instances) {
	bool flag = false;

	foreach (const CSMeshNode*, node, _geometry->renderNodes()) {
		foreach (int, fi, node->fragmentIndices()) {
			const CSMeshFragment& frag = _geometry->fragments()->objectAtIndex(fi);

			const CSArray<CSVertexArrayInstance>* currInstances = instances;
			
			const CSMaterialSource* material = _skin && frag.materialIndex() < _skin->count() ? _skin->objectAtIndex(frag.materialIndex()) : NULL;
			
			if (CSMaterialSource::apply(material, graphics, layer, materialProgress, materialRandom, &currInstances, !flag)) {
				flag = true;

				if (frag.hasBones()) {
					CSObject* boneKey = NULL;
					CSGBufferSlice* boneBufferSlice = NULL;

					bool upload = false;

					if (_customTransforms) {
						boneKey = new BoneKey(this, frag, _animation, _clippedProgress);
						if (_dirtyTransforms) upload = true;
					}
					else boneKey = new BoneKey(NULL, frag, _animation, _clippedProgress);

					boneBufferSlice = static_assert_cast<CSGBufferSlice*>(CSResourcePool::sharedPool()->get(boneKey));

					int boneCount = frag.boneCount();

					if (boneBufferSlice && boneBufferSlice->count() != boneCount) {
						CSResourcePool::sharedPool()->remove(boneKey);
						boneBufferSlice = NULL;
					}
					if (!boneBufferSlice) {
						boneBufferSlice = CSGBuffers::getSlice(CSGBufferTargetUniform, 64, boneCount, CSGraphicsContext::sharedContext()->maxUniformBlockSize() / 64, CSGBufferUsageHintDynamicDraw);
						CSResourcePool::sharedPool()->add(boneKey, boneBufferSlice, BoneBufferSliceLife, false);
						upload = true;
					}
					release(boneKey);

					if (upload) {
						updateTransforms();

						CSMatrix* boneTransforms = (CSMatrix*)fcalloc(boneCount, sizeof(CSMatrix));
						for (int i = 0; i < boneCount; i++) {
							CSMatrix& bt = boneTransforms[i];
							const CSMeshFragment::Bone& bone = frag.bone(i);
							const CSMeshNode* node = _geometry->findNode(bone.name);
							if (node) {
								getNodeLocalTransform(node, bt);
								bt = bone.matrix * bt;
							}
						}

						boneBufferSlice->buffer()->uploadSub(boneTransforms, boneBufferSlice->offset(), boneCount);
						free(boneTransforms);
					}

					const CSABoundingBox* aabb = NULL;
					if (!_customTransforms) aabb = &_geometry->aabb();
					frag.draw(graphics, boneBufferSlice, aabb, currInstances);
					//계산 편의를 위해 랜더영역을 결정하는 AABB는 포즈를 사용, 애니메이션이 포즈에서 크게 벗어나고 블랜딩 드로우가 섞일 경우 랜더순서가 달라보이는 경우가 드물게 있을 수 있음.
					//SceneObject의 aabb는 lazy 업데이트 방식이고 글로벌 aabb 이므로 맞지 않음
				}
				else {
					updateTransforms();
					graphics->transform(getNodeLocalTransform(node));
					frag.draw(graphics, NULL, &frag.aabb(), currInstances);
				}
				graphics->reset();
			}
		}
	}
	if (flag) graphics->pop();
}


