#define CDK_IMPL

#include "CSSpriteElementContrast.h"

#include "CSBuffer.h"

CSSpriteElementContrast::CSSpriteElementContrast(CSBuffer* buffer) :
	value(CSAnimationFloat::factorWithBuffer(buffer)) {

}

CSSpriteElementContrast::CSSpriteElementContrast(const CSSpriteElementContrast* other) :
	value(CSAnimationFloat::factorWithFactor(other->value)) 
{

}

int CSSpriteElementContrast::resourceCost() const {
	int cost = sizeof(CSSpriteElementContrast);
	if (value) cost += value->resourceCost();
	return cost;
}

float CSSpriteElementContrast::getValue(float progress, int random) const {
	return value ? CSMath::max(value->value(progress, CSRandom::toFloatSequenced(random, 0)), 0.0f) : 1;
}

void CSSpriteElementContrast::draw(DrawParam& param) const {
	param.graphics->setContrast(getValue(param.progress, param.random));
}
