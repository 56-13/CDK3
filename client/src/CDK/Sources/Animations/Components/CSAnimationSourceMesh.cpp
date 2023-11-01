#define CDK_IMPL

#include "CSAnimationSourceMesh.h"

#include "CSBuffer.h"

CSAnimationSourceMesh::CSAnimationSourceMesh(CSBuffer* buffer) :
	builder(CSMeshBuilder::builderWithBuffer(buffer)),
	bones(buffer->readArray<string>())
{

}

CSAnimationSourceMesh::CSAnimationSourceMesh(const CSAnimationSourceMesh* other) :
	builder(CSMeshBuilder::builderWithBuilder(other->builder)),
	bones(other->bones)
{

}

int CSAnimationSourceMesh::resourceCost() const {
	int cost = sizeof(CSAnimationSourceMesh);
	if (builder) builder->resourceCost();
	if (bones) {
		cost += sizeof(CSArray<string>) + bones->capacity() * 8;
		foreach (const string&, bone, bones) cost += bone.resourceCost();
	}
	return cost;
}

void CSAnimationSourceMesh::preload() const {
	if (builder) builder->preload();
}