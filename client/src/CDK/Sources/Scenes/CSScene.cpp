#define CDK_IMPL

#include "CSScene.h"
#include "CSResourcePool.h"
#include "CSFile.h"
#include "CSBuffer.h"
#include "CSRandom.h"
#include "CSTextures.h"
#include "CSRenderTargets.h"
#include "CSRenderers.h"
#include "CSShaders.h"
#include "CSGround.h"
#include "CSTerrain.h"
#include "CSAnimation.h"

static constexpr int DefaultGrid = 50;

CSScene::CSScene(const CSScenePref& pref, const CSSceneEnv& env) :
	pref(pref),
	env(env),
	_random(randInt())
{
	resetCamera();
}


CSScene::CSScene(CSBuffer* buffer) :
	env(buffer),
	_random(randInt())
{
	resetCamera();
}

CSScene::CSScene(CSBuffer* buffer, const CSScenePref& pref) :
	pref(pref),
	env(buffer),
	_random(randInt())
{
	foreach(CSSceneObjectBuilder*, epb, &env.props) {
		CSSceneObject* envprop = epb->createObject();
		addChild(envprop);
		envprop->release();
	}
	switch (buffer->readByte()) {
		case 1:
			_world = retain(static_assert_cast<CSGround*>(CSResourcePool::sharedPool()->load(CSResourceTypeGround, buffer->readArray<ushort>())));
			break;
		case 2:
			_world = new CSGround(buffer);
			break;
		case 3:
			_world = retain(static_assert_cast<CSTerrain*>(CSResourcePool::sharedPool()->load(CSResourceTypeTerrain, buffer->readArray<ushort>())));
			break;
	}
	if (_world) {
		_props = _world->createProps();
		foreach (CSSceneObject*, prop, _props) addChild(prop);
	}
	int childrenCount = buffer->readLength();
	for (int i = 0; i < childrenCount; i++) {
		CSSceneObjectBuilder* builder = CSSceneObjectBuilder::createWithBuffer(buffer, true);
		CSSceneObject* obj = builder->createObject();
		addChild(obj);
		obj->release();
		builder->release();
	}

	resetCamera();

	//TODO:트리거로 처리
	/*
	int cameraDataId = buffer->readInt();
	if (cameraDataId) _cameraObject = retain(static_assert_cast<CSCameraObject*>(objectForDataId(cameraDataId)));
	*/
}

CSScene::~CSScene() {
	foreach (Sound&, sound, &_sounds) {
		CSAudio::setStopDelegate(sound.handle, NULL);
		CSAudio::stop(sound.handle);
	}

	release(_world);
	if (_lightSpace) delete _lightSpace;
	release(_cameraObject);
	if (_quadTree) delete _quadTree;
	release(_props);
}

void CSScene::setWorld(const CSWorld* world) {
	if (retain(_world, world)) {
		foreach (CSSceneObject*, obj, &_objects) {
			if (obj->fromGround()) obj->addUpdateFlags(CSSceneObject::UpdateFlagTransform);
		}
		if (_props) {
			foreach (CSSceneObject*, prop, _props) removeChild(prop);
			_props->release();
			_props = NULL;
		}
		if (_world) {
			if (_quadTree) _quadTree->resize(_world->space(), _world->grid());
			if (_lightSpace) _lightSpace->space = _world->space();
			_props = _world->createProps();
			foreach (CSSceneObject*, prop, _props) addChild(prop);
		}
	}
}

bool CSScene::setCameraObject(const CSCameraObject* obj) {
	return obj->scene() == this && retain(_cameraObject, obj);
}

void CSScene::resetCamera() {
	if (_cameraObject) return;		//diff with editor

	CSVector3 target;
	if (_world) {
		target = _world->space().center();
		target.z = 0;
	}
	else target = CSVector3::Zero;

	CSQuaternion rot = CSQuaternion::rotationAxis(CSVector3::UnitX, -env.camera.angle);
	CSVector3 pos(0, 0, env.camera.distance);
	pos.transform(rot);
	pos += target;

	CSVector3 up = -CSVector3::UnitY;
	up.transform(rot);

	_camera.setView(pos, target, up);
}

static bool findObjectIndex(CSArray<CSSceneObject>* objects, const CSSceneObject* obj, int dataId, int& index) {
	if (dataId == 0) {
		int i;
		for (i = 0; i < objects->count(); i++) {
			const CSSceneObject* other = objects->objectAtIndex(i);
			if (other == obj) {
				index = i;
				return true;
			}
			else if (other->dataId) break;
		}
		index = i;
		return false;
	}

	int s = 0;
	int e = objects->count();

	for (;;) {
		int i = (s + e) / 2;
		const CSSceneObject* other = objects->objectAtIndex(i);
		if (dataId == other->dataId) {
			index = i;
			return obj == NULL || other == obj;
		}
		else if (dataId < other->dataId) e = i;
		else s = i + 1;
		if (s >= e) {
			index = e;
			return false;
		}
	}
}

void CSScene::locate(CSSceneObject* obj) {
	if (_quadTree) _quadTree->locate(obj);
	int index;
	bool flag = findObjectIndex(&_objects, obj, obj->dataId, index);
	CSAssert(!flag);
	_objects.insertObject(index, obj);
}

void CSScene::unlocate(CSSceneObject* obj) {
	if (_quadTree) _quadTree->unlocate(obj);
	int index;
	bool flag = findObjectIndex(&_objects, obj, obj->dataId, index);
	CSAssert(flag);
	_objects.removeObjectAtIndex(index);
}

void CSScene::relocate(CSSceneObject* obj, const CSABoundingBox* aabb) {
	if (_quadTree) _quadTree->relocate(obj, aabb);
}

float CSScene::getZ(const CSSceneObject* origin, const CSVector3& pos) const {
	float z = 0;
	if (_world) _world->getZ(pos, z);
	if (_quadTree) {
		for (CSSet<CSSceneObject*>::ReadonlyIterator i = _quadTree->select(pos)->iterator(); i.remaining(); i.next()) {
			const CSSceneObject* obj = i.object();
			if (obj != origin && obj->obstacle()) {
				const CSCollider* collider = obj->getCollider();
				if (collider) collider->getZ(pos, z);
			}
		}
	}
	else {
		foreach (const CSSceneObject*, obj, &_objects) {
			if (obj != origin && obj->obstacle()) {
				const CSCollider* collider = obj->getCollider();
				if (collider) collider->getZ(pos, z);
			}
		}
	}
	return z;
}

bool CSScene::intersects(const CSRay& ray, CSCollisionFlags flags, std::function<bool(const CSSceneObject*)> check, float& distance, const CSSceneObject*& target, CSHit* hit) const {
	target = NULL;
	distance = FloatMax;

	bool result = false;

	if (_world) result |= _world->intersects(ray, flags, distance, hit);

	if (_quadTree) {
		for (CSSet<CSSceneObject*>::ReadonlyIterator i = _quadTree->select(ray)->iterator(); i.remaining(); i.next()) {
			const CSSceneObject* obj = i.object();
			if (!check || check(obj)) {
				const CSCollider* collider = obj->getCollider();
				if (collider && collider->intersects(ray, flags, distance, hit)) {
					target = obj;
					result = true;
				}
			}
		}
	}
	else {
		foreach (const CSSceneObject*, obj, &_objects) {
			if (!check || check(obj)) {
				const CSCollider* collider = obj->getCollider();
				if (collider && collider->intersects(ray, flags, distance, hit)) {
					target = obj;
					result = true;
				}
			}
		}
	}
	return result;
}

CSCollisionResult CSScene::intersects(const CSSceneObject* origin, CSCollisionFlags flags, std::function<bool(const CSSceneObject*)> check, const CSSceneObject*& target, CSHit* hit) const {
	target = NULL;
	float distance = FloatMax;

	const CSCollider* collider = origin->getCollider();

	if (!collider) return CSCollisionResultFront;

	CSCollisionResult result = CSCollisionResultFront;

	//TODO:world 는 일단 계산하지 않음

	if (_quadTree) {
		for (CSSet<CSSceneObject*>::ReadonlyIterator i = _quadTree->select(collider)->iterator(); i.remaining(); i.next()) {
			const CSSceneObject* obj = i.object();
			if (!check || check(obj)) {
				const CSCollider* col = obj->getCollider();
				if (col) {
					CSCollisionResult res = collider->intersects(col, flags, distance, hit);
					if (res != CSCollisionResultFront) {
						target = obj;
						result = res;
					}
				}
			}
		}
	}
	else {
		foreach (const CSSceneObject*, obj, &_objects) {
			if (!check || check(obj)) {
				const CSCollider* col = obj->getCollider();
				if (col) {
					CSCollisionResult res = collider->intersects(col, flags, distance, hit);
					if (res != CSCollisionResultFront) {
						target = obj;
						result = res;
					}
				}
			}
		}
	}
	return result;
}

CSSceneObject* CSScene::objectForDataId(int dataId) {
	int index;
	return findObjectIndex(&_objects, NULL, dataId, index) ? _objects.objectAtIndex(index) : NULL;
}

bool CSScene::addChild(CSSceneObject* child) {
	if (!child->animation() && !child->parent() && !child->scene()) {
		child->link(this);
		_children.addObject(child);
		return true;
	}
	return false;
}

bool CSScene::removeChild(CSSceneObject* child) {
	if (_children.removeObjectIdenticalTo(child)) {
		child->unlink();
		return true;
	}
	return false;
}

static void rewind(CSSceneObject* obj) {
	obj->rewind();
	foreach (CSSceneObject*, child, obj->children()) rewind(child);		//TODO:use allobjects
}

void CSScene::rewind() {
	_progress = 0;
	_random = randInt();
	foreach (CSSceneObject*, child, &_children) ::rewind(child);		//TODO:use allobjects
}

void CSScene::updateEnv(float delta, const CSVector2& screenSize) {
	_camera.setProjection(env.camera.fov, screenSize.x, screenSize.y, env.camera.znear, env.camera.zfar);

	if (_cameraObject) _cameraObject->capture(_camera);

	CSABoundingBox space;
	int grid;
	if (_world) {
		space = _world->space();
		grid = _world->grid();
	}
	else {
		CSVector2 hs = screenSize * 0.5f;
		float hd = CSMath::max(hs.x, hs.y);
		space.minimum = CSVector3(-hs.x, -hs.y, -hd);
		space.minimum = CSVector3(hs.x, hs.y, hd);
		grid = DefaultGrid;
	}

	if (env.lighting.enabled) {
		if (!_lightSpace) _lightSpace = new CSLightSpace(space);
		else _lightSpace->space = space;
	}
	else {
		if (_lightSpace) {
			delete _lightSpace;
			_lightSpace = NULL;
		}
	}
	if (env.quadTree) {
		if (_quadTree) _quadTree->resize(space, grid);
		else {
			_quadTree = new CSQuadTree(space, grid);
			foreach (CSSceneObject*, obj, &_objects) _quadTree->locate(obj);
		}
	}
	else {
		if (_quadTree) {
			delete _quadTree;
			_quadTree = NULL;
		}
	}
}

void CSScene::update(float delta, const CSVector2& screenSize) {
	_progress += delta;

	updateEnv(delta, screenSize);

	updateSounds(delta);

	uint inflags = 0;
	if (_camera.viewUpdated()) {
		inflags |= CSSceneObject::UpdateFlagView;
		_camera.view();
	}
	CSArray<CSSceneObject>* subobjs[CSUpdatePass::Max + 1] = {};
	int taskCount = 0;
	foreach (CSSceneObject*, obj, &_objects) {
		int pass = obj->getUpdatePass();
		CSArray<CSSceneObject>*& psubobjs = subobjs[pass];
		if (!psubobjs) psubobjs = new CSArray<CSSceneObject>(_objects.count());
		psubobjs->addObject(obj);
		if (taskCount < psubobjs->count()) taskCount = psubobjs->count();
	}

	if (_lightSpace) _lightSpace->beginUpdate();

	CSArray<CSTaskBase*> tasks(taskCount);

	const CSSceneObject* cameraRoot = NULL;
	if (_cameraObject) {
		cameraRoot = _cameraObject;
		while (cameraRoot && !cameraRoot->located()) {
			if (cameraRoot->parent()) cameraRoot = cameraRoot->parent();
			else if (cameraRoot->animation()) cameraRoot = cameraRoot->animation()->root();
		}
	}

	for (int i = 0; i <= CSUpdatePass::Max; i++) {
		CSArray<CSSceneObject>* psubobjs = subobjs[i];
		if (psubobjs) {
			foreach (CSSceneObject*, obj, psubobjs) {
				if (obj == cameraRoot) {
					uint flags = inflags;
					if (obj->update(delta, true, flags) != CSSceneObject::UpdateStateStopped) {
						_cameraObject->capture(_camera);
						if (_camera.viewUpdated()) {
							inflags |= CSSceneObject::UpdateFlagView;
							_camera.view();
						}
					}
					else obj->unlocate();
				}
				else {
					tasks.addObject(CSThreadPool::run<void>([obj, delta, inflags]() {
						uint flags = inflags;
						if (obj->update(delta, true, flags) == CSSceneObject::UpdateStateStopped) obj->unlocate();
					}));
				}
			}
			CSTaskBase::finishAll(&tasks);
			tasks.removeAllObjects();
			psubobjs->release();
		}
	}

	if (_lightSpace) _lightSpace->endUpdate();
}

bool CSScene::preprocess(CSGraphics* graphics, const CSVector2& screenSize, bool& blit) {
	CSAssert(!graphics->target()->hasViewport() && !graphics->hasScissor());

	CSVector3 scale;
	CSQuaternion rotation;
	CSVector3 trans;
	
	if (!graphics->world().decompose(scale, rotation, trans)) return false;

	const CSMaterial& material = graphics->material();

	blit = rotation.nearEqual(CSQuaternion::Identity) &&
		graphics->color() * material.color == CSColor::White &&
		(material.blendMode == CSBlendNone || material.blendMode == CSBlendAlpha);

	CSRect screenRect(trans.x, trans.y, screenSize.x * scale.x, screenSize.y * scale.y);

	CSInt2 resolution(
		CSMath::ceil(screenSize.x * graphics->target()->width() / graphics->camera().width()),
		CSMath::ceil(screenSize.y * graphics->target()->height() / graphics->camera().height()));

	graphics->push();

	if (env.postprocess.enabled) {
		CSRenderTargetDescription targetDesc;
		targetDesc.width = resolution.x;
		targetDesc.height = resolution.y;

		CSRenderTargetDescription::Attachment colorAttachmentDesc;
		colorAttachmentDesc.attachment = CSFramebufferAttachmentColor0;
		colorAttachmentDesc.format = CSRawFormat::Rgb16f;
		colorAttachmentDesc.samples = pref.samples;
		colorAttachmentDesc.texture = true;
		targetDesc.attachments.addObject(colorAttachmentDesc);

		CSRenderTargetDescription::Attachment depthStencilAttachmentDesc;
		depthStencilAttachmentDesc.attachment = CSFramebufferAttachmentDepthStencil;
		depthStencilAttachmentDesc.format = CSRawFormat::Depth24Stencil8;
		depthStencilAttachmentDesc.samples = pref.samples;
		targetDesc.attachments.addObject(depthStencilAttachmentDesc);

		float bloomIntensity = env.postprocess.bloomIntensity * pref.bloomIntensity;
		float bloomThreshold = env.postprocess.bloomThreshold * pref.bloomThreshold;

		if (bloomIntensity > 0) {
			CSRenderTargetDescription::Attachment bloomAttachmentDesc;
			bloomAttachmentDesc.attachment = CSFramebufferAttachmentColor1;
			bloomAttachmentDesc.format = CSRawFormat::Rgb16f;
			bloomAttachmentDesc.samples = pref.samples;

			if (pref.samples <= 1) {	//멀티샘플의 경우 블룸맵을 다시 캡쳐하므로 랜더버퍼사용, 멀티샘플이 아니면 블룸맵을 바로 사용가능
				bloomAttachmentDesc.texture = true;
				bloomAttachmentDesc.textureMinFilter = CSTextureMinFilterLinear;
				bloomAttachmentDesc.textureMagFilter = CSTextureMagFilterLinear;
			}

			targetDesc.attachments.addObject(bloomAttachmentDesc);
		}

		CSRenderTarget* target = CSRenderTargets::getTemporary(targetDesc);
		
		if (env.lighting.fog) target->clearColor(0, env.lighting.fogColor);
		else target->clearColor(0, CSColor::Black);
		target->clearDepthStencil();
		if (bloomIntensity > 0) {
			target->clearColor(1, CSColor::Black);
			graphics->setBloomThreshold(bloomThreshold);
		}
		if (blit) graphics->target()->setViewport(graphics->convertToTargetSpace(screenRect));

		graphics->setTarget(target);
	}
	else if (blit) {
		graphics->setScissor(screenRect);
		graphics->clear(env.lighting.fog ? env.lighting.fogColor : CSColor::Black);
		graphics->clearScissor();

		graphics->target()->setViewport(graphics->convertToTargetSpace(screenRect));
	}
	else {
		CSRenderTargetDescription targetDesc = graphics->target()->description();
		targetDesc.width = resolution.x;
		targetDesc.height = resolution.y;
				
		CSRenderTarget* target = CSRenderTargets::getTemporary(targetDesc);
		if (env.lighting.fog) target->clearColor(0, env.lighting.fogColor);
		else target->clearColor(0, CSColor::Black);
		target->clearDepthStencil();

		graphics->setTarget(target);
	}

	if (env.lighting.fog) {
		graphics->setFogColor(env.lighting.fogColor);
		graphics->setFogNear(env.lighting.fogNear);
		graphics->setFogFar(env.lighting.fogFar);
	}
	else graphics->clearFog();

	graphics->camera() = _camera;

	graphics->world() = CSMatrix::Identity;
	
	if (_lightSpace) {
		_lightSpace->mode = pref.lightMode;
		_lightSpace->allowShadow = pref.allowShadow;
		_lightSpace->allowShadowPixel32 = pref.allowShadowPixel32;
		_lightSpace->maxShadowResolution = pref.maxShadowResolution;

		_lightSpace->ambientLight = env.lighting.ambientLight;
		_lightSpace->envMap = env.lighting.skyboxTexture();
		_lightSpace->envColor = env.lighting.skyboxColor;
	}
}

void CSScene::postprocess(CSGraphics* graphics, const CSVector2& screenSize, bool blit) {
	if (env.postprocess.enabled) {
		CSRenderTarget* src = retain(graphics->target());
		graphics->pop();

		if (blit) {
			CSRenderTarget* dest = graphics->target();

			CSDelegateRenderCommand* command = graphics->command([this, src, dest](CSGraphicsApi* api) {
				float bloomIntensity = env.postprocess.bloomIntensity * pref.bloomIntensity;
				float exposure = env.postprocess.exposure * pref.exposure;
				CSShaders::postProcess()->draw(api, src, dest, env.postprocess.bloomPass, bloomIntensity, exposure, pref.gamma);
			}, this, src, dest);

			command->addFence(src, CSGBatchFlagRead | CSGBatchFlagRetrieve);
			command->addFence(dest, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);

			graphics->target()->clearViewport();
		}
		else {
			CSDelegateRenderCommand* command = graphics->command([this, src](CSGraphicsApi* api) {
				float bloomIntensity = env.postprocess.bloomIntensity * pref.bloomIntensity;
				float exposure = env.postprocess.exposure * pref.exposure;
				CSShaders::postProcess()->draw(api, src, src, env.postprocess.bloomPass, bloomIntensity, exposure, pref.gamma);
			}, this, src);

			command->addFence(src, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);

			CSImage image(src->captureColor(0, true), 1, CSVector2::Zero);
			graphics->drawImage(&image, CSRect(0, 0, screenSize.x, screenSize.y));
		}

		src->release();
	}
	else {
		if (blit) {
			graphics->pop();

			graphics->target()->clearViewport();
		}
		else {
			CSRenderTarget* src = retain(graphics->target());
			graphics->pop();

			CSImage image(src->captureColor(0, true), 1, CSVector2::Zero);
			graphics->drawImage(&image, CSRect(0, 0, screenSize.x, screenSize.y));

			src->release();
		}
	}
}

void CSScene::draw(CSGraphics* graphics, const CSVector2& screenSize) {
	bool blit;

	if (!preprocess(graphics, screenSize, blit)) return;

	graphics->setDepthMode(CSDepthReadWrite);

	if (_lightSpace) {
		CSInstanceLayer layer;
		while (_lightSpace->beginDraw(graphics, layer)) {
			if (_world) _world->draw(graphics, layer, _progress, _random);
			foreach (CSSceneObject*, obj, &_objects) obj->draw(graphics, layer);
			_lightSpace->endDraw(graphics);
		}
	}

	_quadTree->select(graphics->camera().boundingFrustum(), &_clippedObjects);

	CSTask<uint>** showTasks = (CSTask<uint>**)alloca(_clippedObjects.count() * sizeof(CSTask<uint>*));
	int i = 0;
	for (CSSet<CSSceneObject*>::Iterator ci = _clippedObjects.iterator(); ci.remaining(); ci.next()) {
		showTasks[i++] = CSThreadPool::run<uint>(ci.object(), &CSSceneObject::show);
	}
	uint showFlags = 0;
	for (i = 0; i < _clippedObjects.count(); i++) {
		showTasks[i]->finish();
		showFlags |= showTasks[i]->result();
	}
	for (CSSet<CSSceneObject*>::Iterator ci = _clippedObjects.iterator(); ci.remaining(); ci.next()) {
		CSSceneObject* obj = ci.object();

		CSABoundingBox aabb;
		if (obj->getAABB(aabb)) {
			float d = CSVector3::distance(graphics->camera().position(), aabb.center()) - aabb.radius();
			CSQueue<DrawObject>::Iterator di = _drawObjects.iterator();
			while (di.remaining() && d >= di.object().distance) di.next();
			DrawObject& dobj = di.insert();
			dobj.obj = obj;
			dobj.distance = d;
		}
	}

	if (_world) _world->draw(graphics, CSInstanceLayerBase, _progress, _random);
	for (CSQueue<DrawObject>::Iterator di = _drawObjects.iterator(); di.remaining(); di.next()) di.object().obj->draw(graphics, CSInstanceLayerBase);

	graphics->setDepthMode(CSDepthRead);

	const CSTexture* skyboxTexture = env.lighting.skyboxTexture();
	if (skyboxTexture) graphics->drawSkybox(skyboxTexture);

	for (int layer = CSInstanceLayerBlendBottom; layer <= CSInstanceLayerBlendTop; layer++) {
		if (_world) _world->draw(graphics, (CSInstanceLayer)layer, _progress, _random);
		for (CSQueue<DrawObject>::Iterator di = _drawObjects.iterator(); di.remaining(); di.next()) di.object().obj->draw(graphics, (CSInstanceLayer)layer);
	}

	if ((showFlags & CSSceneObject::ShowFlagDistortion) != 0) {
		CSDistortionRenderer* distortionRenderer = CSRenderers::distortion();
		distortionRenderer->begin(graphics);
		graphics->setRenderer(distortionRenderer);
		_world->draw(graphics, CSInstanceLayerDistortion, _progress, _random);
		for (CSQueue<DrawObject>::Iterator di = _drawObjects.iterator(); di.remaining(); di.next()) di.object().obj->draw(graphics, CSInstanceLayerDistortion);
		distortionRenderer->end(graphics);
	}

	if (_cameraObject) _cameraObject->filter(graphics);

	_clippedObjects.removeAllObjects();
	_drawObjects.removeAllObjects();

	postprocess(graphics, screenSize, blit);
}

int CSScene::playSound(const string& subpath, float volume, CSAudioControl control, int loop, int priority, float duplication, const CSVector3* pos) {
	if (!_soundEnabled) return 0;

	const char* path = CSFile::findPath(subpath);

	if (!path) return 0;

	if (pos) {
		float d = CSVector3::distance(_camera.position(), *pos);
		if (d >= env.sound.maxDistance) return 0;
		float f = env.sound.refDistance / (env.sound.refDistance + env.sound.rollOffFactor * (CSMath::max(d, env.sound.refDistance) - env.sound.refDistance));
		volume *= f;
	}

	synchronized(&_sounds) {
		int playing = 0;

		foreach (Sound&, sound, &_sounds) {
			if (sound.control == control && sound.duplication) {
				if (sound.subpath == subpath) {
					if (sound.volume < volume) {
						sound.volume = volume;
						CSAudio::setVolume(sound.handle, volume);
					}
					if (sound.loop < loop || (sound.loop != 0 && loop == 0)) {
						sound.loop = loop;
						CSAudio::setLoop(sound.handle, loop);
					}
					if (sound.priority < priority) {
						sound.priority = priority;
					}
					return 0;
				}
				playing++;
			}
		}

		int capacity = env.sound.capacity[control];

		if (capacity && playing >= capacity) {
			int index = -1;
			float minpv = priority + volume;
			for (int i = 0; i < _sounds.count(); i++) {
				Sound& sound = _sounds.objectAtIndex(i);
				if (sound.control == control) {
					float pv = sound.priority + sound.volume;
					if (sound.control == control && pv <= minpv) {
						index = i;
						minpv = pv;
					}
				}
			}
			if (index != -1) CSAudio::stop(_sounds.objectAtIndex(index).handle);
			else return 0;
		}
		{
			auto stopcb = [this](int handle) {
				synchronized(&_sounds) {
					for (int i = 0; i < _sounds.count(); i++) {
						if (_sounds.objectAtIndex(i).handle == handle) {
							_sounds.removeObjectAtIndex(i);
							break;
						}
					}
				}
			};

			Sound sound;
			sound.subpath = subpath;
			sound.volume = volume;
			sound.control = control;
			sound.loop = loop;
			sound.priority = priority;
			sound.duplication = CSMath::max(duplication, env.sound.duplication);
			sound.handle = CSAudio::play(path, volume, control, loop, stopcb);
			_sounds.addObject(sound);

			return sound.handle;
		}
	}

	return 0;
}

void CSScene::updateSounds(float delta) {
	synchronized(&_sounds) {
		foreach (Sound&, sound, &_sounds) {
			sound.duplication = CSMath::max(sound.duplication - delta, 0.0f);
		}
	}
}