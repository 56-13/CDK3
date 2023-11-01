#ifndef __CDK__CSMesh__
#define __CDK__CSMesh__

#include "CSMeshGeometry.h"

class CSMesh : public CSResource {
private:
	CSDictionary<string, CSMeshGeometry> _geometries;
	CSDictionary<string, CSMeshAnimation> _animations;
public:
	CSMesh() = default;
	explicit CSMesh(CSBuffer* buffer);

	static inline CSMesh* mesh() {
		return autorelease(new CSMesh());
	}
	static inline CSMesh* meshWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSMesh(buffer));
	}

	inline CSResourceType resourceType() const override {
		return CSResourceTypeMesh;
	}
	int resourceCost() const override;

	inline CSMeshGeometry* geometry(const string& name) {
		return _geometries.objectForKey(name);
	}
	inline const CSMeshGeometry* geometry(const string& name) const {
		return _geometries.objectForKey(name);
	}
	inline void addGeometry(CSMeshGeometry* geometry) {
		_geometries.setObject(geometry->name(), geometry);
	}
	inline void removeGeometry(const string& name) {
		_geometries.removeObject(name);
	}
	inline CSMeshAnimation* animation(const string& name) {
		return _animations.objectForKey(name);
	}
	inline const CSMeshAnimation* animation(const string& name) const {
		return _animations.objectForKey(name);
	}
	inline void addAnimation(CSMeshAnimation* animation) {
		_animations.setObject(animation->name(), animation);
	}
	inline void removeAnimation(const string& name) {
		_animations.removeObject(name);
	}
};

#endif