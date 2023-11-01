#define CDK_IMPL

#include "CSSegment.h"

#include "CSEntry.h"

#include "CSBuffer.h"

CSSegment::CSSegment(CSBuffer* buffer) : position0(buffer), position1(buffer) {

}

CSSegment::CSSegment(const byte*& raw) : position0(raw), position1(raw) {

}

void CSSegment::transform(const CSSegment& seg, const CSQuaternion& rotation, CSSegment& result) {
	CSVector3::transform(seg.position0, rotation, result.position0);
	CSVector3::transform(seg.position1, rotation, result.position1);
}

void CSSegment::transform(const CSSegment& seg, const CSMatrix& trans, CSSegment& result) {
	CSVector3::transformCoordinate(seg.position0, trans, result.position0);
	CSVector3::transformCoordinate(seg.position1, trans, result.position1);
}

uint CSSegment::hash() const {
	CSHash hash;
	hash.combine(position0);
	hash.combine(position1);
	return hash;
}