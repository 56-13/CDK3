#define CDK_IMPL

#include "CSTriangle.h"

#include "CSEntry.h"

#include "CSBuffer.h"

CSTriangle::CSTriangle(CSBuffer* buffer) : position0(buffer), position1(buffer), position2(buffer) {

}

CSTriangle::CSTriangle(const byte*& raw) : position0(raw), position1(raw), position2(raw) {

}

void CSTriangle::transform(const CSTriangle& tri, const CSQuaternion& rotation, CSTriangle& result) {
	CSVector3::transform(tri.position0, rotation, result.position0);
	CSVector3::transform(tri.position1, rotation, result.position1);
	CSVector3::transform(tri.position2, rotation, result.position2);
}

void CSTriangle::transform(const CSTriangle& tri, const CSMatrix& trans, CSTriangle& result) {
	CSVector3::transformCoordinate(tri.position0, trans, result.position0);
	CSVector3::transformCoordinate(tri.position1, trans, result.position1);
	CSVector3::transformCoordinate(tri.position2, trans, result.position2);
}

uint CSTriangle::hash() const {
	CSHash hash;
	hash.combine(position0);
	hash.combine(position1);
	hash.combine(position2);
	return hash;
}
