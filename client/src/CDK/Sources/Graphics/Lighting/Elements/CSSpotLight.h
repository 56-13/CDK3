#ifndef __CDK__CSSpotLight__
#define __CDK__CSSpotLight__

#include "CSColor3.h"

#include "CSLightAttenuation.h"

#include "CSShadow.h"

struct CSSpotLight {
	void* key;
	CSVector3 position;
	CSVector3 direction;
	float angle;
	float dispersion;
	CSColor3 color;
	CSLightAttenuation attenuation;
	bool castShadow;
	CSShadow shadow;

	CSSpotLight() = default;
	CSSpotLight(void* key, const CSVector3& position, const CSVector3& direction, float angle, float dispersion, const CSColor3& color, const CSLightAttenuation& attenuation, bool castShadow, const CSShadow& shadow);
	CSSpotLight(void* key, CSBuffer* buffer);
	CSSpotLight(void* key, const byte*& bytes);

	float range() const;

	uint hash() const;

	bool operator ==(const CSSpotLight& other) const;
	inline bool operator !=(const CSSpotLight& other) const {
		return !(*this == other);
	}
};

#endif