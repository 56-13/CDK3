#ifndef __CDK__CSPointLight__
#define __CDK__CSPointLight__

#include "CSColor3.h"

#include "CSLightAttenuation.h"

#include "CSShadow.h"

struct CSPointLight {
	void* key;
	CSVector3 position;
	CSColor3 color;
	CSLightAttenuation attenuation;
	bool castShadow;
	CSShadow shadow;

	CSPointLight() = default;
	CSPointLight(void* key, const CSVector3& position, const CSColor3& color, const CSLightAttenuation& attenuation, bool castShadow, const CSShadow& shadow);
	CSPointLight(void* key, CSBuffer* buffer);
	CSPointLight(void* key, const byte*& bytes);

	float range() const;

	uint hash() const;

	bool operator ==(const CSPointLight& other) const;
	inline bool operator !=(const CSPointLight& other) const {
		return !(*this == other);
	}
};

#endif