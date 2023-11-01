#ifndef __CDK__CSDirectionalLightObject__
#define __CDK__CSDirectionalLightObject__

#include "CSSceneObject.h"

#include "CSDirectionalLight.h"

#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"

class CSDirectionalLightBuilder;

class CSDirectionalLightObject : public CSSceneObject {
private:
	const CSDirectionalLightBuilder* _origin;
	float _progress;
	int _random;
public:
	CSDirectionalLightObject(const CSDirectionalLightBuilder* origin);
private:
	~CSDirectionalLightObject();
public:
	static inline CSDirectionalLightObject* object(const CSDirectionalLightBuilder* origin) {
		return autorelease(new CSDirectionalLightObject(origin));
	}

	inline Type type() const override {
		return TypeDirectionalLight;
	}
	float duration(DurationParam param, float duration = 0) const override;
	inline float progress() const override {
		return _progress;
	}
	void onRewind() override;
	UpdateState onUpdate(float delta, bool alive, uint& flags) override;
private:
	void capture(CSColor3& color) const;
};

class CSDirectionalLightBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<CSAnimationColor> color;
	float duration = 0;
	CSAnimationLoop loop;
	bool castShadow = true;
	bool castShadow2D = false;
	CSShadow shadow;

	CSDirectionalLightBuilder() = default;
	CSDirectionalLightBuilder(CSBuffer* buffer, bool withScene);
	CSDirectionalLightBuilder(const CSDirectionalLightBuilder* other);
private:
	~CSDirectionalLightBuilder() = default;
public:
	static inline CSDirectionalLightBuilder* builder() {
		return autorelease(new CSDirectionalLightBuilder());
	}
	static inline CSDirectionalLightBuilder* builderWithBuilder(const CSDirectionalLightBuilder* other) {
		return autorelease(new CSDirectionalLightBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeDirectionalLight;
	}
	int resourceCost() const override;
	inline CSDirectionalLightObject* createObject() const override {
		return new CSDirectionalLightObject(this);
	}
	inline void preload() const override {}
};

inline float CSDirectionalLightObject::duration(DurationParam param, float duration) const {
	return _origin->duration;
}

#endif