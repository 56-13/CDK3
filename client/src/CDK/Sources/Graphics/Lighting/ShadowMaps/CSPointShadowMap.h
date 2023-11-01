#ifdef CDK_IMPL

#ifndef __CDK__CSPointShadowMap__
#define __CDK__CSPointShadowMap__

#include "CSPointLight.h"

#include "CSMatrix.h"

#include "CSRenderTarget.h"

class CSGraphics;

class CSPointShadowMap {
private:
	CSPointLight _light;
	float _range;
	CSRenderTarget* _renderTarget;
	CSMatrix _viewProjections[6];
public:
	CSPointShadowMap();
	~CSPointShadowMap();

	CSPointShadowMap(const CSPointShadowMap&) = delete;
	CSPointShadowMap& operator =(const CSPointShadowMap&) = delete;

	CSTexture* texture();

	void begin(CSGraphics* graphics, const CSPointLight& light, float range);
	void end(CSGraphics* graphics);
};

#endif

#endif