#ifndef __CDK__CSScenePref__
#define __CDK__CSScenePref__

#include "CSGraphics.h"

struct CSScenePref {
	CSLightMode lightMode = CSLightCookGGX;
	byte samples = 1;
	bool allowShadow = true;
	bool allowShadowPixel32 = true;
	ushort maxShadowResolution = 2048;
	float bloomIntensity = 1;
	float bloomThreshold = 1;
	float exposure = 1;
	float gamma = 1;

	CSScenePref() = default;
	explicit CSScenePref(CSBuffer* buffer);

	void writeTo(CSBuffer* buffer) const;
};

#endif
