#define CDK_IMPL

#include "CSMeshFragment.h"

#include "CSBuffer.h"

#include "CSVertex.h"

CSMeshFragment::Bone::Bone(CSBuffer* buffer) : name(buffer->readString()), matrix(buffer), aabb(buffer) {

}

CSMeshFragment::CSMeshFragment(CSBuffer* buffer) : 
	_name(buffer->readString()),
	_materialIndex(buffer->readByte()),
	_hasTexCoords(buffer->readBoolean()),
	_hasNormals(buffer->readBoolean()),
	_hasTangents(buffer->readBoolean()) 
{
	bool hasBones = buffer->readBoolean();

	int stride = 12;
	if (_hasTexCoords) stride += 8;
	if (_hasNormals) stride += 6;
	if (_hasTangents) stride += 6;
	if (hasBones) stride += 12;

	CSArray<CSVertexLayout>* vertexLayouts = new CSArray<CSVertexLayout>(6);
	new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, stride, 0, 0, true);
    int offset = 12;
    if (_hasTexCoords) {
		new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, stride, offset, 0, true);
        offset += 8;
    }
    if (_hasNormals) {
		new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, stride, offset, 0, true);
        offset += 6;
    }
    if (_hasTangents) {
		new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribTangent, 3, CSVertexAttribTypeHalfFloat, false, stride, offset, 0, true);
        offset += 6;
    }
    if (hasBones) {
		new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribBoneIndices, 4, CSVertexAttribTypeUnsignedByte, false, stride, offset, 0, true);
        offset += 4;
		new (&vertexLayouts->addObject()) CSVertexLayout(0, CSRenderState::VertexAttribBoneWeights, 4, CSVertexAttribTypeHalfFloat, false, stride, offset, 0, true);
    }
	_vertices = new CSVertexArray(1, true, vertexLayouts);
	vertexLayouts->release();

	int vertexCount = buffer->readInt();
	_vertices->vertexBuffer(0)->upload(buffer->bytes(), stride, vertexCount, CSGBufferUsageHintStaticDraw);
	buffer->skip(vertexCount * stride);

	int faceCount = buffer->readInt();
	int elementSize;
	if (vertexCount > 65536) elementSize = 4;
	else if (vertexCount > 256) elementSize = 2;
	else elementSize = 1;
	_vertices->indexBuffer()->upload(buffer->bytes(), elementSize, faceCount * 3, CSGBufferUsageHintStaticDraw);
	buffer->skip(faceCount * 3 * elementSize);

	_aabb = CSABoundingBox(buffer);

	if (hasBones) {
		int boneCount = buffer->readInt();
		_bones = new CSArray<Bone>(boneCount);
		_boneIndices = new CSDictionary<string, byte>(boneCount);
		for (int i = 0; i < boneCount; i++) {
			Bone& bone = _bones->addObject();
			new (&bone) Bone(buffer);
			_boneIndices->setObject(bone.name, i);
		}
	}
	else {
		_bones = NULL;
		_boneIndices = NULL;
	}
}

CSMeshFragment::~CSMeshFragment() {
	_vertices->release();
	CSObject::release(_bones);
	CSObject::release(_boneIndices);
}

int CSMeshFragment::resourceCost() const {
	int cost = sizeof(CSMeshFragment) + _vertices->resourceCost();
	if (_name) cost += _name.resourceCost();
	if (_bones) {
		cost += sizeof(CSArray<Bone>) + _bones->capacity() * sizeof(Bone);
		foreach (const Bone&, bone, _bones) cost += bone.name.resourceCost();
	}
	if (_boneIndices) cost += sizeof(CSDictionary<string, byte>) + _boneIndices->capacity() * 16;
	return cost;
}

const CSMeshFragment::Bone* CSMeshFragment::bone(const string& name) const {
	if (!_bones) return NULL;
	const byte* index = _boneIndices->tryGetObjectForKey(name);
	if (!index) return NULL;
	return &_bones->objectAtIndex(*index);
}

const CSMeshFragment::Bone& CSMeshFragment::bone(int index) const {
	return _bones->objectAtIndex(index);
}

void CSMeshFragment::draw(CSGraphics* graphics, const CSGBufferSlice* boneBufferSlice, const CSABoundingBox* aabb, const CSArray<CSVertexArrayInstance>* instances) const {
	CSAssert((boneBufferSlice != NULL) == hasBones());
	if (boneBufferSlice) graphics->drawVertices(_vertices, boneBufferSlice->buffer(), boneBufferSlice->offset(), CSPrimitiveTriangles, aabb, instances);
	else graphics->drawVertices(_vertices, CSPrimitiveTriangles, aabb, instances);
}
