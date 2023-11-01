#ifdef CDK_IMPL

#ifndef __CDK__CSDirectionalShadowMap__
#define __CDK__CSDirectionalShadowMap__

#include "CSDirectionalLight.h"
#include "CSABoundingBox.h"
#include "CSCamera.h"
#include "CSRenderTarget.h"

class CSGraphics;

class CSDirectionalShadowMap {
private:
	CSDirectionalLight _light;
	CSABoundingBox _space;
	CSCamera _camera;
	CSRenderTarget* _renderTarget[2];
	CSMatrix _viewProjection[2];
	bool _visible;
public:
	CSDirectionalShadowMap();
	~CSDirectionalShadowMap();

	CSDirectionalShadowMap(const CSDirectionalShadowMap&) = delete;
	CSDirectionalShadowMap& operator =(const CSDirectionalShadowMap&) = delete;

	inline bool visible() const {
		return _visible;
	}
	inline const CSMatrix& viewProjection(bool shadow2D) const {
		return _viewProjection[shadow2D];
	}
	CSTexture* texture(bool shadow2D);

	void clear(bool shadow2D);
	bool begin(CSGraphics* graphics, const CSDirectionalLight& light, const CSABoundingBox& space, bool shadow2D);
	void end(CSGraphics* graphics, bool shadow2D);
private:
	void updateView();

	const CSArray<CSVector3>* lightVolumePoints();
	void updateView(const CSArray<CSVector3>* points);
	void updateView2D(const CSArray<CSVector3>* points);
	const CSArray<CSVector3, 2>* frustumObject();
	const CSArray<CSVector3, 2>* clipObjectByScene(const CSArray<CSVector3, 2>* obj);
	const CSArray<CSVector3>* objectLightVolumePoints(const CSArray<CSVector3, 2>* obj);
};

#endif

#endif