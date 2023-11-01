#define CDK_IMPL

#include "CSBoundingMesh.h"

#include "CSBuffer.h"

CSBoundingMesh::Face::Face(const CSVector3& normal, float min, float max, const int* indices, int indexCount) :
	normal(normal),
	min(min),
	max(max),
	indexCount(indexCount)
{
	CSAssert(indexCount < 8);
	memcpy(this->indices, indices, indexCount * sizeof(int));
}

uint CSBoundingMesh::Face::hash() const {
	CSHash hash;
	for (int i = 0; i < indexCount; i++) hash.combine(indices[i]);
	return hash;
}

bool CSBoundingMesh::Face::operator ==(const Face& other) const {
	return indexCount == other.indexCount && memcmp(indices, other.indices, indexCount * sizeof(int)) == 0;
}

//=========================================================================================================

CSBoundingMesh::CSBoundingMesh() : CSBoundingMesh(8, 6) {

}

CSBoundingMesh::CSBoundingMesh(int vertexCapacity, int faceCapacity) : _vertices(vertexCapacity), _faces(faceCapacity) {

}

CSBoundingMesh::CSBoundingMesh(const CSBoundingMesh& other) : _vertices(&other._vertices), _faces(&other._faces) {

}

CSBoundingMesh::CSBoundingMesh(CSBuffer* buffer) : _vertices(0), _faces(0) {
	int vertexCount = buffer->readLength();
	_vertices.setCapacity(vertexCount);
	for (int i = 0; i < vertexCount; i++) new (&_vertices.addObject()) CSVector3(buffer);

	int faceCount = buffer->readLength();
	_faces.setCapacity(faceCount);
	for (int i = 0; i < faceCount; i++) {
		int indexCount = buffer->readLength();
		int* indices = (int*)alloca(indexCount * sizeof(int));
		buffer->read(indices, indexCount * sizeof(int));
		addFace(indices, indexCount);
	}
}

CSBoundingMesh::CSBoundingMesh(const byte*& raw) : _vertices(0), _faces(0) {
	int vertexCount = readLength(raw);
	_vertices.setCapacity(vertexCount);
	for (int i = 0; i < vertexCount; i++) new (&_vertices.addObject()) CSVector3(raw);

	int faceCount = readLength(raw);
	_faces.setCapacity(faceCount);
	for (int i = 0; i < faceCount; i++) {
		int indexCount = readLength(raw);
		int indexLength = indexCount * sizeof(int);
		int* indices = (int*)alloca(indexLength);
		memcpy(indices, raw, indexLength);
		raw += indexLength;
		addFace(indices, indexCount);
	}
}

CSVector3 CSBoundingMesh::center() const {
	CSVector3 center(CSVector3::Zero);
	if (_vertices.count()) {
		foreach (const CSVector3&, p, &_vertices) center += p;
		center /= _vertices.count();
	}
	return center;
}

float CSBoundingMesh::radius() const {
	CSVector3 center = this->center();
	float radius = 0;
	foreach (const CSVector3&, p, &_vertices) {
		float r = CSVector3::distanceSquared(center, p);
		if (r > radius) radius = r;
	}
	radius = CSMath::sqrt(radius);
	return radius;
}

void CSBoundingMesh::addVertex(const CSVector3& vertex) {
	foreach (Face&, face, &_faces) {
		float t = CSVector3::dot(vertex, face.normal);
		if (t < face.min) face.min = t;
		if (t > face.max) face.max = t;
	}
	_vertices.addObject(vertex);
}

bool CSBoundingMesh::addFace(int indexCount, int index, ...) {
	int* indices = (int*)alloca(indexCount * sizeof(int));
	va_list ap;
	va_start(ap, indexCount);
	for (int i = 0; i < indexCount; i++) indices[i] = va_arg(ap, int);
	va_end(ap);

	return addFace(indices, indexCount);
}

bool CSBoundingMesh::addFace(const int* indices, int indexCount) {
	CSAssert(indexCount >= 3);

	CSVector3 normal = CSVector3::cross(_vertices.objectAtIndex(indices[1]) - _vertices.objectAtIndex(indices[0]), _vertices.objectAtIndex(indices[2]) - _vertices.objectAtIndex(indices[0]));

	for (int i = 1; i < indexCount - 2; i++) {
		CSVector3 n = CSVector3::cross(_vertices.objectAtIndex(indices[i + 1]) - _vertices.objectAtIndex(indices[i]), _vertices.objectAtIndex(indices[i + 2]) - _vertices.objectAtIndex(indices[i]));

		if (!normal.nearEqual(n)) return false;
	}

	float min = FloatMax;
	float max = FloatMin;
	foreach (const CSVector3&, vertex, &_vertices) {
		float t = CSVector3::dot(vertex, normal);
		if (t < min) min = t;
		if (t > max) max = t;
	}
	new (&_faces.addObject()) Face(normal, min, max, indices, indexCount);

	return true;
}

void CSBoundingMesh::transform(const CSQuaternion& rotation) {
	foreach (CSVector3&, p, &_vertices) CSVector3::transform(p, rotation, p);

	foreach (Face&, face, &_faces) {
		CSVector3::transform(face.normal, rotation, face.normal);
		face.min = FloatMax;
		face.max = FloatMin;
		foreach (const CSVector3&, p, &_vertices) {
			float t = CSVector3::dot(p, face.normal);
			if (t < face.min) face.min = t;
			if (t > face.max) face.max = t;
		}
	}
}

void CSBoundingMesh::transform(const CSMatrix& trans) {
	foreach (CSVector3&, p, &_vertices) CSVector3::transform(p, trans, p);

	foreach (Face&, face, &_faces) {
		CSVector3::transformNormal(face.normal, trans, face.normal);
		face.normal.normalize();
		face.min = FloatMax;
		face.max = FloatMin;
		foreach (const CSVector3&, p, &_vertices) {
			float t = CSVector3::dot(p, face.normal);
			if (t < face.min) face.min = t;
			if (t > face.max) face.max = t;
		}
	}
}

CSBoundingMesh& CSBoundingMesh::operator =(const CSBoundingMesh& other) {
	_vertices.removeAllObjects();
	_faces.removeAllObjects();
	_vertices.addObjectsFromArray(&other._vertices);
	_faces.addObjectsFromArray(&other._faces);
	return *this;
}

uint CSBoundingMesh::hash() const {
	CSHash hash;
	foreach (const CSVector3&, p, &_vertices) hash.combine(p);
	foreach (const Face&, f, &_faces) hash.combine(f);
	return hash;
}

bool CSBoundingMesh::operator ==(const CSBoundingMesh& other) const {
	return _vertices.sequentialEqual(&other._vertices) && _faces.sequentialEqual(&other._faces);
}
