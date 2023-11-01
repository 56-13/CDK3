#ifndef __CDK__CSSceneObject__
#define __CDK__CSSceneObject__

#include "CSHandler.h"

#include "CSGraphics.h"

#include "CSGizmo.h"
#include "CSCollider.h"

class CSScene;
class CSSceneObjectBuilder;
class CSAnimationObjectFragment;

class CSSceneObject : public CSObject {
public:
	enum Type : byte {
		TypeObject = 1,
		TypeBox,
		TypeSphere,
		TypeCapsule,
		TypeMesh,
		TypeImage,
		TypeParticle,
		TypeTrail,
		TypeSprite,
		TypeDirectionalLight,
		TypePointLight,
		TypeSpotLight,
		TypeAnimation,
		TypeAnimationReference,
		TypeCamera,
		TypeSpawn			//TODO
	};
	enum ShowFlag {
		ShowFlagDistortion = 1
	};
	enum UpdateFlag {
		UpdateFlagTransform = 1,
		UpdateFlagAABB = 2,
		UpdateFlagView = 4
	};
	enum UpdateState {
		UpdateStateNone,
		UpdateStateStopped,
		UpdateStateAlive,
		UpdateStateFinishing
	};
	enum DurationParam {
		DurationParamMin,
		DurationParamMax,
		DurationParamAvg
	};

	CSHandler<CSSceneObject*> OnLink;
	CSHandler<CSSceneObject*> OnUnlink;
	CSHandler<CSSceneObject*> OnLocate;
	CSHandler<CSSceneObject*> OnUnlocate;
	CSHandler<CSSceneObject*> OnRewind;
	CSHandler<CSSceneObject*, CSUpdatePass&> OnUpdatePass;
	CSHandler<CSSceneObject*, float, bool, uint&, UpdateState&> OnUpdate;
	CSHandler<CSSceneObject*, uint&> OnShow;
	CSHandler<CSSceneObject*, CSGraphics*, CSInstanceLayer> OnDraw;
private:
	CSScene* _scene = NULL;
	CSSceneObject* _parent = NULL;
	CSAnimationObjectFragment* _animation = NULL;
	CSArray<CSSceneObject>* _children = NULL;
	bool _located = false;
	byte _updated = 0;
	byte _updateFlags = 0;
	bool _aabbFlag = false;
	bool _transformUpdated = false;
	CSGizmo* _transform = NULL;
	CSABoundingBox _aabb = CSABoundingBox::Zero;
	const CSObject* _tag = NULL;
#ifdef CDK_IMPL
public:
#else
private:
#endif
	int dataId = 0;
public:
	CSSceneObject(const CSSceneObjectBuilder* builder = NULL);
protected:
	virtual ~CSSceneObject();
public:
	static inline CSSceneObject* object(const CSSceneObjectBuilder* builder = NULL) {
		return autorelease(new CSSceneObject(builder));
	}

	inline virtual Type type() const {
		return TypeObject;
	}
	inline CSScene* scene() {
		return _scene;
	}
	inline const CSScene* scene() const {
		return _scene;
	}
#ifdef CDK_IMPL
	bool link(CSScene* scene);
	bool link(CSAnimationObjectFragment* animation);
	void unlink();
#endif
	inline CSSceneObject* parent() {
		return _parent;
	}
	inline const CSSceneObject* parent() const {
		return _parent;
	}
	inline CSAnimationObjectFragment* animation() {
		return _animation;
	}
	inline const CSAnimationObjectFragment* animation() const {
		return _animation;
	}
	inline const CSArray<CSSceneObject, 1, false>* children() {
		return _children->asReadWrite();
	}
	inline const CSArray<CSSceneObject>* children() const {
		return _children;
	}
	bool addChild(CSSceneObject* child);
	bool removeChild(CSSceneObject* child);

	inline bool obstacle() const {
		return _obstacle;
	}
	inline bool located() const {
		return _located && _scene;
	}
	void locate();
	void unlocate();

	void useTransform(bool flag);

	inline CSGizmo* transform() {
		return _transform;
	}
	inline const CSGizmo* transform() const {
		return _transform;
	}
	inline void addUpdateFlags(uint flags) {
		_updateFlags |= flags;
	}
	inline bool getAABB(CSABoundingBox& result) const {
		result = _aabb;
		return _aabbFlag;
	}
	inline virtual bool addAABB(CSABoundingBox& result) const {
		return false;
	}
	virtual bool getTransform(float progress, const string& name, CSMatrix& result) const;

	inline bool getTransform(const string& name, CSMatrix& result) const {
		return getTransform(progress(), name, result);
	}
	inline bool getTransform(CSMatrix& result) const {
		return getTransform(progress(), NULL, result);
	}
	inline bool transformUpdated() const {
		return _transformUpdated;
	}
	bool fromGround() const;

	inline CSCollider* getCollider() const {
		CSCollider* result = NULL;
		addCollider(result);
		return result;
	}
	inline virtual void addCollider(CSCollider*& result) const {}

	inline virtual float duration(DurationParam param, float duration = 0) const {
		return 0;
	}
	inline virtual float progress() const {
		return 0;
	}
	virtual bool afterCameraUpdate() const;
	virtual bool getUpdatePass(CSUpdatePass& pass) const;
	inline int getUpdatePass() const {
		CSUpdatePass pass;
		getUpdatePass(pass);
		return pass;
	}

	void rewind();
	UpdateState update(float delta, bool alive, uint& flags);
	inline bool update(float delta) {
		uint flags = 0;
		return update(delta, true, flags) != UpdateStateStopped;
	}
	uint show();
	void draw(CSGraphics* graphics, CSInstanceLayer layer);

	inline void clearTag() {
		release(_tag);
	}
	template <class T>
	inline void setTag(T* tag) {
		retain(_tag, tag);
	}
	template <class T>
	inline T* tag(bool assertType = true) const {
		return assertType ? static_assert_cast<T*>(const_cast<CSObject*>(_tag)) : dynamic_cast<T*>(const_cast<CSObject*>(_tag));
	}
	inline void setTagAsInt(int tag) {
		setTag(CSInteger::value(tag));
	}
	inline int tagAsInt() const {
		CSInteger* i = tag<CSInteger>();
		return i ? i->value() : 0;
	}
protected:
	virtual void onLink();
	virtual void onUnlink();
	virtual void onLocate();
	virtual void onUnlocate();
	virtual void onRewind();
	virtual UpdateState onUpdate(float delta, bool alive, uint& flags);
	virtual uint onShow();
	virtual void onDraw(CSGraphics* graphics, CSInstanceLayer layer);
private:
	void doLocate();
};

class CSSceneObjectBuilder : public CSObject {
public:
	int dataId = 0;
	bool located = false;
	CSPtr<CSGizmoData> transform;
	CSPtr<CSArray<CSSceneObjectBuilder>> children;

	CSSceneObjectBuilder() = default;
protected:
	CSSceneObjectBuilder(CSBuffer* buffer, bool withScene);
	CSSceneObjectBuilder(const CSSceneObjectBuilder* other);
	virtual ~CSSceneObjectBuilder() = default;
public:
	static inline CSSceneObjectBuilder* builder() {
		return autorelease(new CSSceneObjectBuilder());
	}
	static CSSceneObjectBuilder* createWithBuffer(CSBuffer* buffer, bool withScene);
	static inline CSSceneObjectBuilder* builderWithBuffer(CSBuffer* buffer, bool withScene) {
		return autorelease(createWithBuffer(buffer, withScene));
	}
	static CSSceneObjectBuilder* createWithBuilder(const CSSceneObjectBuilder* other);
	static inline CSSceneObjectBuilder* builderWithBuilder(const CSSceneObjectBuilder* other) {
		return autorelease(createWithBuilder(other));
	}

	virtual inline CSSceneObject::Type type() const {
		return CSSceneObject::TypeObject;
	}
protected:
	int resourceCostBase() const;
public:
	virtual inline int resourceCost() const {
		return sizeof(CSSceneObjectBuilder) + resourceCostBase();
	}
	virtual inline CSSceneObject* createObject() const {
		return new CSSceneObject(this);
	}
	virtual inline void preload() const {}
};

#endif
