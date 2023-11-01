#define CDK_IMPL

#include "CSSpriteElementStroke.h"

#include "CSBuffer.h"

CSSpriteElementStroke::CSSpriteElementStroke(CSBuffer* buffer) :
	color(CSAnimationColor::colorWithBuffer(buffer)),
	mode((CSStrokeMode)buffer->readByte()),
	width(buffer->readByte())
{

}

CSSpriteElementStroke::CSSpriteElementStroke(const CSSpriteElementStroke* other) :
	color(CSAnimationColor::colorWithColor(other->color)),
	mode(other->mode),
	width(other->width)
{

}

int CSSpriteElementStroke::resourceCost() const {
	int cost = sizeof(CSSpriteElementStroke);
	if (color) cost += color->resourceCost();
	return cost;
}

CSColor CSSpriteElementStroke::getColor(float progress, int random) const {
	if (!color) return CSColor::White;

	CSColor cr(
		CSRandom::toFloatSequenced(random, 0),
		CSRandom::toFloatSequenced(random, 1),
		CSRandom::toFloatSequenced(random, 2),
		CSRandom::toFloatSequenced(random, 3));

	return color->value(progress, cr, CSColor::White);
}

void CSSpriteElementStroke::draw(DrawParam& param) const {
	param.graphics->setStrokeColor(getColor(param.progress, param.random));
	param.graphics->setStrokeMode(mode);
	param.graphics->setStrokeWidth(width);
}
