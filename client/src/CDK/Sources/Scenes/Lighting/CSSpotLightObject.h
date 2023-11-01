#ifndef __CDK__CSSpotLightObject__
#define __CDK__CSSpotLightObject__

#include "CSSceneObject.h"

#include "CSSpotLight.h"

#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"

class CSSpotLightBuilder;

class CSSpotLightObject : public CSSceneObject {
private:
	const CSSpotLightBuilder* _origin;
	float _progress;
	int _random;
public:
	CSSpotLightObject(const CSSpotLightBuilder* origin);
private:
	~CSSpotLightObject();
public:
	static inline CSSpotLightObject* object(const CSSpotLightBuilder* origin) {
		return autorelease(new CSSpotLightObject(origin));
	}

	inline Type type() const override {
		return TypeSpotLight;
	}
	float duration(DurationParam param, float duration = 0) const override;
	inline float progress() const override {
		return _progress;
	}
	void onRewind() override;
	UpdateState onUpdate(float delta, bool alive, uint& flags) override;
private:
	void capture(float& angle, float& dispersion, CSColor3& color) const;
};

class CSSpotLightBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<CSAnimationFloat> angle;
	CSPtr<CSAnimationFloat> dispersion;
	CSPtr<CSAnimationColor> color;
	float duration = 0;
	CSAnimationLoop loop;
	CSLightAttenuation attenuation = CSLightAttenuation::Default100;
	bool castShadow = true;
	CSShadow shadow;

	CSSpotLightBuilder() = default;
	CSSpotLightBuilder(CSBuffer* buffer, bool withScene);
	CSSpotLightBuilder(const CSSpotLightBuilder* other);
private:
	~CSSpotLightBuilder() = default;
public:
	static inline CSSpotLightBuilder* builder() {
		return autorelease(new CSSpotLightBuilder());
	}
	static inline CSSpotLightBuilder* builderWithBuilder(const CSSpotLightBuilder* other) {
		return autorelease(new CSSpotLightBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeSpotLight;
	}
	int resourceCost() const override;
	inline CSSpotLightObject* createObject() const override {
		return new CSSpotLightObject(this);
	}
	inline void preload() const override {}
};

inline float CSSpotLightObject::duration(DurationParam param, float duration) const {
	return _origin->duration;
}

#endif