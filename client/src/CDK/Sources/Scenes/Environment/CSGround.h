#ifndef __CDK__CSGround__
#define __CDK__CSGround__

#include "CSWorld.h"

#include "CSMaterialSource.h"

class CSGround : public CSWorld {
public:
	ushort width = 0;
	ushort height = 0;
	ushort altitude = 0;
	ushort gridSize = 0;
	CSColor3 gridColor = CSColor3::White;
	bool gridVisible = false;
	CSPtr<const CSMaterialSource> material;

	CSGround() = default;
	CSGround(CSBuffer* buffer);
private:
	~CSGround() = default;
public:
	static inline CSGround* ground() {
		return autorelease(new CSGround());
	}
	static inline CSGround* groundWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSGround(buffer));
	}

	inline CSResourceType resourceType() const override {
		return CSResourceTypeGround;
	}
	inline int resourceCost() const override {
		return sizeof(CSGround) + material->resourceCost();
	}
	inline void preload() const override {
		material->preload();
	}
	inline Type type() const override {
		return TypeGround;
	}
	CSABoundingBox space() const override;
	inline int grid() const override{
		return gridSize;
	}
	inline CSArray<CSSceneObject>* createProps() const override {
		return NULL;
	}
	inline void getZ(const CSVector3& pos, float& z) const {}
	bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const override;
	void draw(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random) const override;
};

#endif