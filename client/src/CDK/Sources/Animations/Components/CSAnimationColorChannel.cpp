#define CDK_IMPL

#include "CSAnimationColorChannel.h"

#include "CSBuffer.h"

CSAnimationColorChannel::CSAnimationColorChannel(const CSAnimationColorChannel* other) : 
	red(other->red),
	green(other->green),
	blue(other->blue),
	alpha(other->alpha),
	normalized(other->normalized),
	fixedChannel(other->fixedChannel)
{
	
}

CSAnimationColorChannel::CSAnimationColorChannel(CSBuffer* buffer) : normalized(buffer->readBoolean()) {
	bool alphaChannel = buffer->readBoolean();
	red = CSAnimationFloat::factorWithBuffer(buffer);
	green = CSAnimationFloat::factorWithBuffer(buffer);
	blue = CSAnimationFloat::factorWithBuffer(buffer);
	if (alphaChannel) alpha = CSAnimationFloat::factorWithBuffer(buffer);
	fixedChannel = buffer->readBoolean();
}

int CSAnimationColorChannel::resourceCost() const {
	int cost = sizeof(CSAnimationColorChannel);
	if (red) cost += red->resourceCost();
	if (green) cost += green->resourceCost();
	if (blue) cost += blue->resourceCost();
	if (alpha) cost += alpha->resourceCost();
	return cost;
}

CSColor CSAnimationColorChannel::value(float t, const CSColor& r, const CSColor& default) const {
	CSColor rate = r;
	if (fixedChannel) rate.g = rate.b = rate.a = rate.r;

	CSColor color = default;
	if (red) {
		color.r = red->value(t, rate.r);
		if (normalized) color.r = CSMath::clamp(color.r, 0.0f, 1.0f);
	}
	if (green) {
		color.g = green->value(t, rate.g);
		if (normalized) color.g = CSMath::clamp(color.g, 0.0f, 1.0f);
	}
	if (blue) {
		color.b = blue->value(t, rate.b);
		if (normalized) color.b = CSMath::clamp(color.b, 0.0f, 1.0f);
	}
	if (alpha) color.a = CSMath::clamp(alpha->value(t, rate.a), 0.0f, 1.0f);

	return color;
}
