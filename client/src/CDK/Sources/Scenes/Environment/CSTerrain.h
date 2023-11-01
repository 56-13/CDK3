#ifndef __CDK__CSTerrain__
#define __CDK__CSTerrain__

#include "CSWorld.h"

#include "CSMaterialSource.h"

#include "CSFixed2.h"

struct CSTerrainSurface {
	CSPtr<CSMaterialSource> material;
	CSPtr<CSTexture> intensityMap;
	CSPtr<CSVertexArray> vertices;
	float rotation;
	float scale;
	bool triPlaner;
	CSVector2 offset;

	CSTerrainSurface() = default;

	CSTerrainSurface(const CSTerrainSurface&) = delete;
	CSTerrainSurface& operator=(const CSTerrainSurface& other) = delete;
};
struct CSTerrainWater {
	CSPtr<CSMaterialSource> material;
	float textureScale;
	float perturbIntensity;
	CSPtr<CSTexture> foamTexture;
	float foamScale;
	float foamIntensity;
	float foamDepth;
	float angle;
	float forwardSpeed;
	float crossSpeed;
	float waveDistance;
	float waveAltitude;
	float depthMax;
	CSColor shallowColor;
	bool hasTransparency;
	CSPtr<CSVertexArray> vertices;

	CSTerrainWater() = default;

	CSTerrainWater(const CSTerrainWater& other) = delete;
	CSTerrainWater& operator=(const CSTerrainWater& other) = delete;
};

class CSTerrain : public CSWorld {
private:
	CSTexture* _thumbnail;
	ushort _width;
	ushort _height;
	ushort _altitude;
	ushort _grid;
	ushort _vertexCell;
	fixed* _altitudes;
	CSGBuffer* _vertexBuffer;
	CSVertexArray* _vertices;
	CSTexture* _ambientOcclusionMap;
	float _ambientOcclusionIntensity;
	CSArray<CSTerrainSurface>* _surfaces;
	CSArray<CSTerrainWater>* _waters;
	CSArray<CSSceneObjectBuilder>* _props;
public:
	CSTerrain(CSBuffer* buffer);
private:
	~CSTerrain();
public:
	static inline CSTerrain* terrainWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSTerrain(buffer));
	}
	inline const CSTexture* thumbnail() const {
		return _thumbnail;
	}
	inline int width() const {
		return _width;
	}
	inline int height() const {
		return _height;
	}
	inline int altitude() const {
		return _altitude;
	}
	inline int vertexCell() const {
		return _vertexCell;
	}

	fixed altitude(const CSFixed2& p) const;

	inline const CSTexture* ambientOcclusionMap() const {
		return _ambientOcclusionMap;
	}
	inline float ambientOcclusionIntensity() const {
		return _ambientOcclusionIntensity;
	}

	inline CSResourceType resourceType() const override {
		return CSResourceTypeGround;
	}
	int resourceCost() const override;
	void preload() const override;

	inline Type type() const override {
		return TypeTerrain;
	}
	CSABoundingBox space() const override;
	inline int grid() const override {
		return _grid;
	}
	CSArray<CSSceneObject>* createProps() const override;
	void getZ(const CSVector3& pos, float& z) const override;
	bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const override;
	void draw(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random) const override;
private:
	void loadAltitudes(CSBuffer* buffer);
	void loadSurfaces(CSBuffer* buffer);
	void loadWaters(CSBuffer* buffer);
	void addIndex(CSVertexIndexData* indices, int vx, int vy, bool* fills);
	fixed altitude(int vx, int vy) const;
	CSVector3 normal(int vx, int vy) const;
	bool isZQuad(int vx, int vy) const;
	bool isZQuad(fixed a0, fixed a1, fixed a2, fixed a3) const;
	void drawShadow(CSGraphics* graphics) const;
	void drawSurfaces(CSGraphics* graphics, float progress, int random) const;
	void drawWaters(CSGraphics* graphics, float progress, int random) const;
};

#endif