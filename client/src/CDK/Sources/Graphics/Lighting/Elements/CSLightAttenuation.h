#ifndef __CDK__CSLightAttenuation__
#define __CDK__CSLightAttenuation__

#include "CSTypes.h"

class CSBuffer;

struct CSLightAttenuation {
	float range;
	float constant;
	float linear;
	float quadratic;

	static const CSLightAttenuation DefaultNone;
	static const CSLightAttenuation Default3250;
	static const CSLightAttenuation Default600;
	static const CSLightAttenuation Default325;
	static const CSLightAttenuation Default200;
	static const CSLightAttenuation Default160;
	static const CSLightAttenuation Default100;
	static const CSLightAttenuation Default65;
	static const CSLightAttenuation Default50;
	static const CSLightAttenuation Default32;
	static const CSLightAttenuation Default20;
	static const CSLightAttenuation Default13;
	static const CSLightAttenuation Default7;

	CSLightAttenuation() = default;
	CSLightAttenuation(float range, float constant, float linear, float quadratic);
	explicit CSLightAttenuation(CSBuffer* buffer);
	explicit CSLightAttenuation(const byte*& bytes);

	uint hash() const;

	bool operator ==(const CSLightAttenuation& other) const;
	inline bool operator !=(const CSLightAttenuation& other) const {
		return !(*this == other);
	}
};

#endif