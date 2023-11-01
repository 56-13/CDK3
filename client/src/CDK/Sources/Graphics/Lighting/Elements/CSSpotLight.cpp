#define CDK_IMPL

#include "CSSpotLight.h"

#include "CSBuffer.h"

CSSpotLight::CSSpotLight(void* key, const CSVector3& position, const CSVector3& direction, float angle, float dispersion, const CSColor3& color, const CSLightAttenuation& attenuation, bool castShadow, const CSShadow& shadow) :
	key(key),
	position(position),
	direction(direction),
	angle(angle),
	dispersion(dispersion),
	color(color),
	attenuation(attenuation),
	castShadow(castShadow),
	shadow(shadow) {

}

CSSpotLight::CSSpotLight(void* key, CSBuffer* buffer) :
	key(key),
	position(buffer),
	direction(buffer),
	angle(buffer->readFloat()),
	dispersion(buffer->readFloat()),
	color(buffer, false),
	attenuation(buffer),
	castShadow(buffer->readBoolean()),
	shadow(buffer) {

}

CSSpotLight::CSSpotLight(void* key, const byte*& bytes) :
	key(key),
	position(bytes),
	direction(bytes),
	angle(readFloat(bytes)),
	dispersion(readFloat(bytes)),
	color(bytes, false),
	attenuation(bytes),
	castShadow(readBoolean(bytes)),
	shadow(bytes) {

}

float CSSpotLight::range() const {
	return CSMath::sqrt(color.brightness()) * attenuation.range;
}

uint CSSpotLight::hash() const {
	CSHash hash;
	hash.combine(key);
	hash.combine(position);
	hash.combine(direction);
	hash.combine(angle);
	hash.combine(dispersion);
	hash.combine(color);
	hash.combine(attenuation);
	hash.combine(castShadow);
	hash.combine(shadow);
	return hash;
}

bool CSSpotLight::operator ==(const CSSpotLight& other) const {
	return key == other.key &&
		position == other.position &&
		direction == other.direction &&
		angle == other.angle &&
		dispersion == other.dispersion &&
		color == other.color &&
		attenuation == other.attenuation &&
		castShadow == other.castShadow &&
		shadow == other.shadow;
}
