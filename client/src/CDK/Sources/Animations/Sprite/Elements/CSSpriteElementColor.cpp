#define CDK_IMPL

#include "CSSpriteElementColor.h"

#include "CSBuffer.h"

CSSpriteElementColor::CSSpriteElementColor(CSBuffer* buffer) :
	color(CSAnimationColor::colorWithBuffer(buffer))
{

}

CSSpriteElementColor::CSSpriteElementColor(const CSSpriteElementColor* other) :
	color(CSAnimationColor::colorWithColor(other->color))
{

}

int CSSpriteElementColor::resourceCost() const {
	int cost = sizeof(CSSpriteElementColor);
	if (color) cost += color->resourceCost();
	return cost;
}

CSColor CSSpriteElementColor::getColor(float progress, int random) const {
	if (!color) return CSColor::White;
	
	CSColor cr(
		CSRandom::toFloatSequenced(random, 0),
		CSRandom::toFloatSequenced(random, 1),
		CSRandom::toFloatSequenced(random, 2),
		CSRandom::toFloatSequenced(random, 3));

	return color->value(progress, cr, CSColor::White);
}

void CSSpriteElementColor::draw(DrawParam& param) const {
	param.graphics->setColor(getColor(param.progress, param.random));
}
