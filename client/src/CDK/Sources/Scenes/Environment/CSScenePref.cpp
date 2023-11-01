#define CDK_IMPL

#include "CSScenePref.h"

#include "CSBuffer.h"

CSScenePref::CSScenePref(CSBuffer* buffer) :
	lightMode((CSLightMode)buffer->readByte()),
	samples(buffer->readByte()),
	allowShadow(buffer->readBoolean()),
	allowShadowPixel32(buffer->readBoolean()),
	maxShadowResolution(buffer->readShort()),
	bloomIntensity(buffer->readFloat()),
	bloomThreshold(buffer->readFloat()),
	exposure(buffer->readFloat()),
	gamma(buffer->readFloat()) 
{

}

void CSScenePref::writeTo(CSBuffer* buffer) const {
	buffer->writeByte(lightMode);
	buffer->writeByte(samples);
	buffer->writeBoolean(allowShadow);
	buffer->writeBoolean(allowShadowPixel32);
	buffer->writeShort(maxShadowResolution);
	buffer->writeFloat(bloomIntensity);
	buffer->writeFloat(bloomThreshold);
	buffer->writeFloat(exposure);
	buffer->writeFloat(gamma);
}
