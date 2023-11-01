#define CDK_IMPL

#include "CSLightAttenuation.h"

#include "CSBuffer.h"

const CSLightAttenuation CSLightAttenuation::DefaultNone(10000, 1, 0, 0);
const CSLightAttenuation CSLightAttenuation::Default3250(3250, 1, 0.0014f, 0.000007f);
const CSLightAttenuation CSLightAttenuation::Default600(600, 1, 0.007f, 0.0002f);
const CSLightAttenuation CSLightAttenuation::Default325(325, 1, 0.014f, 0.0007f);
const CSLightAttenuation CSLightAttenuation::Default200(200, 1, 0.022f, 0.0019f);
const CSLightAttenuation CSLightAttenuation::Default160(160, 1, 0.027f, 0.0028f);
const CSLightAttenuation CSLightAttenuation::Default100(100, 1, 0.045f, 0.0075f);
const CSLightAttenuation CSLightAttenuation::Default65(65, 1, 0.07f, 0.017f);
const CSLightAttenuation CSLightAttenuation::Default50(50, 1, 0.09f, 0.032f);
const CSLightAttenuation CSLightAttenuation::Default32(32, 1, 0.14f, 0.07f);
const CSLightAttenuation CSLightAttenuation::Default20(20, 1, 0.22f, 0.20f);
const CSLightAttenuation CSLightAttenuation::Default13(13, 1, 0.35f, 0.44f);
const CSLightAttenuation CSLightAttenuation::Default7(7, 1, 0.7f, 1.8f);

CSLightAttenuation::CSLightAttenuation(float range, float constant, float linear, float quadratic) :
	range(range),
	constant(constant),
	linear(linear),
	quadratic(quadratic)
{

}

CSLightAttenuation::CSLightAttenuation(CSBuffer* buffer) :
	range(buffer->readFloat()),
	constant(buffer->readFloat()),
	linear(buffer->readFloat()),
	quadratic(buffer->readFloat())
{

}

CSLightAttenuation::CSLightAttenuation(const byte*& bytes) :
	range(readFloat(bytes)),
	constant(readFloat(bytes)),
	linear(readFloat(bytes)),
	quadratic(readFloat(bytes))
{

}

uint CSLightAttenuation::hash() const {
	CSHash hash;
	hash.combine(range);
	hash.combine(constant);
	hash.combine(linear);
	hash.combine(quadratic);
	return hash;
}

bool CSLightAttenuation::operator ==(const CSLightAttenuation& other) const {
	return range == other.range &&
		constant == other.constant &&
		linear == other.linear &&
		quadratic == other.quadratic;
}
