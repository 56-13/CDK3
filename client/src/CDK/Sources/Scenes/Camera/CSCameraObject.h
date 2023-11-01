#ifndef __CDK__CSCameraObject__
#define __CDK__CSCameraObject__

#include "CSSceneObject.h"

#include "CSCamera.h"

#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"

class CSCameraBuilder;

class CSCameraObject : public CSSceneObject {
private:
	const CSCameraBuilder* _origin;
	CSGizmo* _target;
public:
	bool frustum;
	float fov;
	float znear;
	float zfar;
	bool blur;
	float blurDistance;
	float blurRange;
	float blurIntensity;

	CSCameraObject(const CSCameraBuilder* origin);
private:
	~CSCameraObject();
public:
	static inline CSCameraObject* object(const CSCameraBuilder* origin) {
		return autorelease(new CSCameraObject(origin));
	}

	inline Type type() const override {
		return TypeCamera;
	}
	inline const CSCameraBuilder* origin() const {
		return _origin;
	}

	void useTarget(bool flag);
	inline CSGizmo* target() {
		return _target;
	}
	inline const CSGizmo* target() const {
		return _target;
	}
	void reset();
	bool isFocused() const;
	bool focus();
	bool capture(CSCamera& camera) const;
	void filter(CSGraphics* graphics) const;
protected:
	void onUnlink();
};

class CSCameraBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<CSGizmoData> target;
	bool frustum = false;
	float fov = 60 * FloatToRadians;
	float znear = 10;
	float zfar = 10000;
	bool blur = false;
	float blurDistance = 1000;
	float blurRange = 1000;
	float blurIntensity = 4;

	CSCameraBuilder() = default;
	CSCameraBuilder(CSBuffer* buffer, bool withScene);
	CSCameraBuilder(const CSCameraBuilder* other);
private:
	~CSCameraBuilder() = default;
public:
	static inline CSCameraBuilder* builder() {
		return autorelease(new CSCameraBuilder());
	}
	static inline CSCameraBuilder* builderWithBuilder(const CSCameraBuilder* other) {
		return autorelease(new CSCameraBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeCamera;
	}
	int resourceCost() const override;
	inline CSCameraObject* createObject() const override {
		return new CSCameraObject(this);
	}
	inline void preload() const override {}
};

#endif