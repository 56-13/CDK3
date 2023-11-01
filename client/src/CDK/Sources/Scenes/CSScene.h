#ifndef __CDK__CSScene__
#define __CDK__CSScene__

#include "CSQueue.h"
#include "CSSceneObject.h"
#include "CSScenePref.h"
#include "CSSceneEnv.h"
#include "CSWorld.h"
#include "CSLightSpace.h"
#include "CSQuadTree.h"
#include "CSAudio.h"

#include "CSCameraObject.h"

class CSScene : public CSObject {
public:
	CSScenePref pref;
	CSSceneEnv env;
private:
	struct DrawObject {
		CSSceneObject* obj;
		float distance;
	};
	struct Sound {
		string subpath;
		float volume;
		CSAudioControl control;
		byte loop;
		byte priority;
		float duplication;
		int handle;
	};
	const CSWorld* _world = NULL;
	CSLightSpace* _lightSpace = NULL;
	CSCamera _camera;
	const CSCameraObject* _cameraObject = NULL;
	CSQuadTree* _quadTree = NULL;
	CSArray<CSSceneObject> _objects;
	CSArray<CSSceneObject> _children;
	CSArray<CSSceneObject>* _props;
	CSQueue<DrawObject> _drawObjects;
	CSSet<CSSceneObject*> _clippedObjects;
	CSArray<Sound> _sounds;
	bool _soundEnabled = true;
	float _progress = 0;
	int _random;
public:
	CSScene() = default;
	CSScene(const CSScenePref& pref, const CSSceneEnv& env);
	CSScene(CSBuffer* buffer);
	CSScene(CSBuffer* buffer, const CSScenePref& pref);
private:
	~CSScene();
public:
	static inline CSScene* scene() {
		return autorelease(new CSScene());
	}
	static inline CSScene* scene(const CSScenePref& pref, const CSSceneEnv& env) {
		return autorelease(new CSScene(pref, env));
	}
	static inline CSScene* scene(CSBuffer* buffer) {
		return autorelease(new CSScene(buffer));
	}
	static inline CSScene* scene(CSBuffer* buffer, const CSScenePref& pref) {
		return autorelease(new CSScene(buffer, pref));
	}

	void setWorld(const CSWorld* world);
	inline const CSWorld* world() const {
		return _world;
	}
	inline CSLightSpace* lightSpace() {
		return _lightSpace;
	}
	inline const CSLightSpace* lightSpace() const {
		return _lightSpace;
	}
	inline const CSCamera& camera() const {
		return _camera;
	}
	bool setCameraObject(const CSCameraObject* obj);
	inline const CSCameraObject* cameraObject() const {
		return _cameraObject;
	}
	void resetCamera();
#ifdef CDK_IMPL
	void locate(CSSceneObject* obj);
	void unlocate(CSSceneObject* obj);
	void relocate(CSSceneObject* obj, const CSABoundingBox* aabb);
#endif
	float getZ(const CSSceneObject* origin, const CSVector3& pos) const;
	bool intersects(const CSRay& ray, CSCollisionFlags flags, std::function<bool(const CSSceneObject*)> check, float& distance, const CSSceneObject*& target, CSHit* hit = NULL) const;
	CSCollisionResult intersects(const CSSceneObject* origin, CSCollisionFlags flags, std::function<bool(const CSSceneObject*)> check, const CSSceneObject*& target, CSHit* hit = NULL) const;

	inline const CSArray<CSSceneObject, 1, false>* objects() {
		return _objects.asReadWrite();
	}
	inline const CSArray<CSSceneObject>* objects() const {
		return &_objects;
	}
	CSSceneObject* objectForDataId(int dataId);
	inline const CSSceneObject* objectFroDataId(int dataId) {
		return const_cast<CSScene*>(this)->objectForDataId(dataId);
	}
	inline const CSArray<CSSceneObject, 1, false>* children() {
		return _children.asReadWrite();
	}
	inline const CSArray<CSSceneObject>* children() const {
		return &_children;
	}
	bool addChild(CSSceneObject* child);
	bool removeChild(CSSceneObject* child);
	void rewind();
	void update(float delta, const CSVector2& screenSize);
	void draw(CSGraphics* graphics, const CSVector2& screenSize);

	int playSound(const string& subpath, float volume, CSAudioControl control, int loop, int priority, float duplication, const CSVector3* pos);
private:
	void updateEnv(float delta, const CSVector2& screenSize);
	void updateSounds(float delta);
	bool preprocess(CSGraphics* graphics, const CSVector2& screenSize, bool& blit);
	void postprocess(CSGraphics* graphics, const CSVector2& screenSize, bool blit);
};

#endif
 