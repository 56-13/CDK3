#define CDK_IMPL

#include "CSAnimationColorLinear.h"

#include "CSBuffer.h"

CSAnimationColorLinear::CSAnimationColorLinear(const CSColor& startColor, const CSColor& endColor, bool smooth) : 
	startColor(startColor), 
	endColor(endColor), 
	smooth(smooth) 
{

}

CSAnimationColorLinear::CSAnimationColorLinear(const CSAnimationColorLinear* other) :
	startColor(other->startColor),
	endColor(other->endColor),
	smooth(other->smooth) 
{

}

CSAnimationColorLinear::CSAnimationColorLinear(CSBuffer* buffer) {
	bool normalized = buffer->readBoolean();
	bool alphaChannel = buffer->readBoolean();

	if (alphaChannel) {
		startColor = CSColor(buffer, normalized);
		endColor = CSColor(buffer, normalized);
	}
	else {
		startColor = CSColor3(buffer, normalized);
		endColor = CSColor3(buffer, normalized);
	}
	smooth = buffer->readBoolean();
}

CSColor CSAnimationColorLinear::value(float t, const CSColor& r, const CSColor& default) const {
	if (smooth) t = CSMath::smoothStep(t);
	return CSColor::lerp(startColor, endColor, t);
}
