#define CDK_IMPL

#include "CSSpriteElementBrightness.h"

#include "CSBuffer.h"

CSSpriteElementBrightness::CSSpriteElementBrightness(CSBuffer* buffer) :
	value(CSAnimationFloat::factorWithBuffer(buffer))
{

}

CSSpriteElementBrightness::CSSpriteElementBrightness(const CSSpriteElementBrightness* other) :
	value(CSAnimationFloat::factorWithFactor(other->value))
{

}

int CSSpriteElementBrightness::resourceCost() const {
	int cost = sizeof(CSSpriteElementBrightness);
	if (value) cost += value->resourceCost();
	return cost;
}

float CSSpriteElementBrightness::getValue(float progress, int random) const {
	return value ? CSMath::max(value->value(progress, CSRandom::toFloatSequenced(random, 0)), 0.0f) : 0;
}

void CSSpriteElementBrightness::draw(DrawParam& param) const {
	param.graphics->setBrightness(getValue(param.progress, param.random));
}
