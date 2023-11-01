#ifdef CDK_IMPL

#ifndef __CDK__CSSpotShadowMap__
#define __CDK__CSSpotShadowMap__

#include "CSSpotLight.h"

#include "CSMatrix.h"

#include "CSRenderTarget.h"

class CSGraphics;

class CSSpotShadowMap {
private:
	CSSpotLight _light;
	float _range;
	CSRenderTarget* _renderTarget;
	CSMatrix _viewProjection;
public:
	CSSpotShadowMap();
	~CSSpotShadowMap();

	CSSpotShadowMap(const CSSpotShadowMap&) = delete;
	CSSpotShadowMap& operator =(const CSSpotShadowMap&) = delete;

	inline const CSMatrix& viewProjection() const {
		return _viewProjection;
	}
	CSTexture* texture();

	void begin(CSGraphics* graphics, const CSSpotLight& light, float range);
	void end(CSGraphics* graphics);
};

#endif

#endif