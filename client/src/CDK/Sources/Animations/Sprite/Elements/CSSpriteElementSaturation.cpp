#define CDK_IMPL

#include "CSSpriteElementSaturation.h"

#include "CSBuffer.h"

CSSpriteElementSaturation::CSSpriteElementSaturation(CSBuffer* buffer) :
	value(CSAnimationFloat::factorWithBuffer(buffer)) 
{

}

CSSpriteElementSaturation::CSSpriteElementSaturation(const CSSpriteElementSaturation* other) :
	value(CSAnimationFloat::factorWithFactor(other->value)) 
{

}

int CSSpriteElementSaturation::resourceCost() const {
	int cost = sizeof(CSSpriteElementSaturation);
	if (value) cost += value->resourceCost();
	return cost;
}

float CSSpriteElementSaturation::getValue(float progress, int random) const {
	return value ? CSMath::max(value->value(progress, CSRandom::toFloatSequenced(random, 0)), 0.0f) : 1;
}

void CSSpriteElementSaturation::draw(DrawParam& param) const {
	param.graphics->setSaturation(getValue(param.progress, param.random));
}
