#define CDK_IMPL

#include "CSSpriteElementInvert.h"

#include "CSBuffer.h"

CSSpriteElementInvert::CSSpriteElementInvert(CSBuffer* buffer) :
	x(buffer->readBoolean()),
	y(buffer->readBoolean()),
	z(buffer->readBoolean()) 
{

}

CSSpriteElementInvert::CSSpriteElementInvert(const CSSpriteElementInvert* other) :
	x(other->x),
	y(other->y),
	z(other->z)
{

}

CSVector3 CSSpriteElementInvert::getScale() const {
	return CSVector3(x ? -1 : 1, y ? -1 : 1, z ? -1 : 1);
}

bool CSSpriteElementInvert::addAABB(TransformParam& param, CSABoundingBox& result) const {
	param.transform = CSMatrix::scaling(getScale()) * param.transform;
	return false;
}

void CSSpriteElementInvert::draw(DrawParam& param) const {
	param.graphics->scale(getScale());
}
