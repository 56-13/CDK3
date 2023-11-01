#ifndef __CDK__CSGizmo__
#define __CDK__CSGizmo__

#include "CSString.h"

#include "CSCamera.h"

#include "CSUpdatePass.h"

class CSSceneObject;

class CSBuffer;

class CSGizmoData : public CSObject {
public:
	string binding;
	int targetDataId = 0;
	CSVector3 position = CSVector3::Zero;
	CSQuaternion rotation = CSQuaternion::Identity;
	CSVector3 scale = CSVector3::One;
	bool fromGround = false;

	CSGizmoData() = default;
	CSGizmoData(CSBuffer* buffer);
	CSGizmoData(const CSGizmoData* other);
private:
	~CSGizmoData() = default;
public:
	static inline CSGizmoData* data() {
		return autorelease(new CSGizmoData());
	}
	static inline CSGizmoData* dataWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSGizmoData(buffer));
	}
	static inline CSGizmoData* dataWithData(const CSGizmoData* other) {
		return autorelease(new CSGizmoData(other));
	}

	int resourceCost() const;
};

class CSGizmo {
private:
	CSSceneObject* _object;
	string _binding;
	mutable const CSSceneObject* _target;
	mutable int _targetDataId;
	CSVector3 _position;
	CSQuaternion _rotation;
	CSVector3 _scale;
	bool _fromGround;
	mutable bool _updated;
public:
	CSGizmo(CSSceneObject* obj, const CSGizmoData* data = NULL);
	~CSGizmo();

	CSGizmo(const CSGizmo&) = delete;
	CSGizmo& operator=(const CSGizmo&) = delete;

	inline const CSSceneObject* target() const {
		loadTarget();
		return _target;
	}

	inline const string& binding() const {
		return _binding;
	}
	inline const CSVector3& position() const {
		return _position;
	}
	inline const CSQuaternion& rotation() const {
		return _rotation;
	}
	inline const CSVector3& scale() const {
		return _scale;
	}
	inline bool fromGround() const {
		return _fromGround;
	}

	void setBinding(const string& binding);
	void setTarget(const CSSceneObject* target);
	void setPosition(const CSVector3& position);
	void setRotation(const CSQuaternion& rotation);
	void setScale(const CSVector3& scale);
	void setFromGround(bool fromGround);

	bool update();
	bool getUpdatePass(CSUpdatePass& pass) const;
	bool getTransform(float progress, CSMatrix& result) const;
	bool getTransform(CSMatrix& result) const;
private:
	void loadTarget() const;
};

#endif
