#define CDK_IMPL

#include "CSPointLight.h"

#include "CSBuffer.h"

CSPointLight::CSPointLight(void* key, const CSVector3& position, const CSColor3& color, const CSLightAttenuation& attenuation, bool castShadow, const CSShadow& shadow) :
	key(key),
	position(position),
	color(color),
	attenuation(attenuation),
	castShadow(castShadow),
	shadow(shadow) {

}

CSPointLight::CSPointLight(void* key, CSBuffer* buffer) :
	key(key),
	position(buffer),
	color(buffer, false),
	attenuation(buffer),
	castShadow(buffer->readBoolean()),
	shadow(buffer) 
{

}

CSPointLight::CSPointLight(void* key, const byte*& bytes) :
	key(key),
	position(bytes),
	color(bytes, false),
	attenuation(bytes),
	castShadow(readBoolean(bytes)),
	shadow(bytes)
{

}

float CSPointLight::range() const {
	return CSMath::sqrt(color.brightness()) * attenuation.range;
}

uint CSPointLight::hash() const {
	CSHash hash;
	hash.combine(key);
	hash.combine(position);
	hash.combine(color);
	hash.combine(attenuation);
	hash.combine(castShadow);
	hash.combine(shadow);
	return hash;
}

bool CSPointLight::operator ==(const CSPointLight& other) const {
	return key == other.key &&
		position == other.position &&
		color == other.color &&
		attenuation == other.attenuation &&
		castShadow == other.castShadow &&
		shadow == other.shadow;
}
