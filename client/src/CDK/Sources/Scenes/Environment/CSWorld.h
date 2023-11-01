#ifndef __CDK__CSWorld__
#define __CDK__CSWorld__

#include "CSGraphics.h"

#include "CSSceneObject.h"

class CSWorld : public CSResource {
public:
	enum Type : byte {
		TypeNone,
		TypeGround,
		TypeTerrain
	};
protected:
	CSWorld() = default;
	virtual ~CSWorld() = default;
public:
	virtual void preload() const = 0;
	virtual Type type() const = 0;
	virtual CSABoundingBox space() const = 0;
	virtual int grid() const = 0;
	virtual CSArray<CSSceneObject>* createProps() const = 0;
	virtual void getZ(const CSVector3& pos, float& z) const = 0;
	virtual bool intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit = NULL) const = 0;
	virtual void draw(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random) const = 0;
};

#endif