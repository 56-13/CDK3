#define CDK_IMPL

#include "CSMesh.h"

#include "CSBuffer.h"

CSMesh::CSMesh(CSBuffer* buffer) : _geometries(0), _animations(0) {
	int geometryCount = buffer->readLength();
	_geometries.setCapacity(geometryCount);
	for (int i = 0; i < geometryCount; i++) {
		CSMeshGeometry* geometry = new CSMeshGeometry(buffer);
		_geometries.setObject(geometry->name(), geometry);
		geometry->release();
	}

	int animationCount = buffer->readLength();
	_animations.setCapacity(animationCount);
	for (int i = 0; i < animationCount; i++) {
		CSMeshAnimation* animation = new CSMeshAnimation(buffer);
		_animations.setObject(animation->name(), animation);
		animation->release();
	}
}

int CSMesh::resourceCost() const {
	int cost = sizeof(CSMesh);
	cost += _geometries.capacity() * 16;
	cost += _animations.capacity() * 16;
	for (CSDictionary<string, CSMeshGeometry>::ReadonlyIterator i = _geometries.iterator(); i.remaining(); i.next()) cost += i.object()->resourceCost();
	for (CSDictionary<string, CSMeshAnimation>::ReadonlyIterator i = _animations.iterator(); i.remaining(); i.next()) cost += i.object()->resourceCost();
	return cost;
}