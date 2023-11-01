#ifndef __CDK__CSPointLightObject__
#define __CDK__CSPointLightObject__

#include "CSSceneObject.h"

#include "CSPointLight.h"

#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"

class CSPointLightBuilder;

class CSPointLightObject : public CSSceneObject {
private:
	const CSPointLightBuilder* _origin;
	float _progress;
	int _random;
public:
	CSPointLightObject(const CSPointLightBuilder* origin);
private:
	~CSPointLightObject();
public:
	static inline CSPointLightObject* object(const CSPointLightBuilder* origin) {
		return autorelease(new CSPointLightObject(origin));
	}

	inline Type type() const override {
		return TypePointLight;
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

class CSPointLightBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<CSAnimationColor> color;
	float duration = 0;
	CSAnimationLoop loop;
	CSLightAttenuation attenuation = CSLightAttenuation::Default600;
	bool castShadow = true;
	CSShadow shadow;

	CSPointLightBuilder() = default;
	CSPointLightBuilder(CSBuffer* buffer, bool withScene);
	CSPointLightBuilder(const CSPointLightBuilder* other);
private:
	~CSPointLightBuilder() = default;
public:
	static inline CSPointLightBuilder* builder() {
		return autorelease(new CSPointLightBuilder());
	}
	static inline CSPointLightBuilder* builderWithBuilder(const CSPointLightBuilder* other) {
		return autorelease(new CSPointLightBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypePointLight;
	}
	int resourceCost() const override;
	inline CSPointLightObject* createObject() const override {
		return new CSPointLightObject(this);
	}
	inline void preload() const override {}
};

inline float CSPointLightObject::duration(DurationParam param, float duration) const {
	return _origin->duration;
}

#endif