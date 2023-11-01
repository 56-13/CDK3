#ifndef __CDK__CSLightSpace__
#define __CDK__CSLightSpace__

#include "CSSet.h"

#include "CSGBuffer.h"
#include "CSTexture.h"
#include "CSCamera.h"
#include "CSABoundingBox.h"
#include "CSDirectionalLight.h"
#include "CSPointLight.h"
#include "CSSpotLight.h"

class CSDirectionalShadowMap;
class CSPointShadowMap;
class CSSpotShadowMap;
class CSGraphics;

class CSLightSpace {
public:
	CSLightMode mode = CSLightCookGGX;
	CSColor3 ambientLight = 0x4C4C4CFF;
	CSPtr<const CSTexture> envMap;
	CSColor3 envColor = CSColor3::Black;
	CSPtr<const CSTexture> brdfMap;
	CSABoundingBox space;
	bool allowShadow = true;
	bool allowShadowPixel32 = true;
	ushort maxShadowResolution = 2048;

	static constexpr int MaxDirectionalLightCount = 3;
	static constexpr int MaxPointLightCount = 256;
	static constexpr int MaxSpotLightCount = 256;
	static constexpr int MaxPointShadowCount = 3;
	static constexpr int MaxSpotShadowCount = 3;

	static constexpr int Cluster = 8;
	static constexpr float ClusterZNear = 0.1f;
private:
	CSCamera _camera;
	CSMatrix _viewProjectionInv;
	float _clusterMaxDepth;
	CSSet<void*> _lightUpdateKeys;
	byte _cursor = 0;
	bool _updating = false;
	bool _first = true;

	CSArray<CSDirectionalLight> _directionalLights;
	CSGBuffer* _directionalLightBuffer = NULL;
	CSDirectionalShadowMap* _directionalShadowMaps[MaxDirectionalLightCount] = {};
	bool _directionalLightUpdated = false;
	sbyte _directionalShadowCursor = 0;

	CSArray<CSPointLight> _pointLights;
	CSGBuffer* _pointLightBuffer = NULL;
	CSTexture* _pointLightClusterMap = NULL;
	CSPointShadowMap* _pointShadowMaps[MaxPointShadowCount] = {};
	ushort _pointShadowLightIndices[MaxPointShadowCount] = {};
	byte _pointShadowCount = 0;
	bool _pointLightUpdated = false;
	bool _pointLightVisible = false;
	sbyte _pointShadowCursor = -1;

	CSArray<CSSpotLight> _spotLights;
	CSGBuffer* _spotLightBuffer = NULL;
	CSTexture* _spotLightClusterMap = NULL;
	CSGBuffer* _spotShadowBuffer = NULL;
	CSSpotShadowMap* _spotShadowMaps[MaxSpotShadowCount] = {};
	ushort _spotShadowLightIndices[MaxSpotShadowCount] = {};
	byte _spotShadowCount = 0;
	bool _spotLightUpdated = false;
	bool _spotLightVisible = false;
	sbyte _spotShadowCursor = -1;

	mutable CSLock _lock;
public:
	CSLightSpace(const CSABoundingBox& space);
	~CSLightSpace();

	CSLightSpace(const CSLightSpace&) = delete;
	CSLightSpace& operator=(const CSLightSpace&) = delete;

	void clear();
	void setDirectionalLight(const CSDirectionalLight& light);
	void setPointLight(const CSPointLight& light);
	void setSpotLight(const CSSpotLight& light);
	void beginUpdate();
	void endUpdate();
	bool beginDraw(CSGraphics* graphics, CSInstanceLayer& layer);
	void endDraw(CSGraphics* graphics);
private:
	void disposeDirectionalLight();
	bool beginDirectionalShadow(CSGraphics* graphics, bool& shadow2D);
	void endDirectionalShadow(CSGraphics* graphics);
	void updateDirectionalLights(bool viewUpdated);
	
	void disposePointLight();
	bool beginPointShadow(CSGraphics* graphics);
	void endPointShadow(CSGraphics* graphics);
	void updatePointLights(bool viewUpdated);
	void updatePointLightCluster();

	void disposeSpotLight();
	bool beginSpotShadow(CSGraphics* graphics);
	void endSpotShadow(CSGraphics* graphics);
	void updateSpotLights(bool viewUpdated);
	void updateSpotLightCluster();
	bool intersectSpotLightCluster(const CSRay& ray, float range, float tanq, int x, int y, int z);

	CSVector3 worldToCluster(const CSVector3& wp);
	CSVector3 clusterToWorld(const CSVector3& cp);
	CSVector3 clusterGridToWorld(int x, int y, int z, const CSVector3& lcp);

	void uploadState(CSGraphics* graphics);
};

#endif