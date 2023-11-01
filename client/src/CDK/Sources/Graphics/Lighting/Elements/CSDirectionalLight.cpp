#define CDK_IMPL

#include "CSDirectionalLight.h"

#include "CSBuffer.h"

CSDirectionalLight::CSDirectionalLight(void* key, const CSVector3& direction, const CSColor3& color, bool castShadow, bool castShadow2D, const CSShadow& shadow) :
	key(key),
	direction(direction),
	color(color),
	castShadow(castShadow),
	castShadow2D(castShadow),
	shadow(shadow)
{
}

CSDirectionalLight::CSDirectionalLight(void* key, CSBuffer* buffer) : 
	key(key),
	direction(buffer),
	color(buffer, false),
	castShadow(buffer->readBoolean()),
	castShadow2D(buffer->readBoolean()),
	shadow(buffer)
{
}

CSDirectionalLight::CSDirectionalLight(void* key, const byte*& bytes) :
	key(key),
	direction(bytes),
	color(bytes, false),
	castShadow(readBoolean(bytes)),
	castShadow2D(readBoolean(bytes)),
	shadow(bytes)
{
}

uint CSDirectionalLight::hash() const {
	CSHash hash;
	hash.combine(key);
	hash.combine(direction);
	hash.combine(color);
	hash.combine(castShadow);
	hash.combine(castShadow2D);
	hash.combine(shadow);
	return hash;
}

bool CSDirectionalLight::operator ==(const CSDirectionalLight& other) const {
	return key == other.key &&
		direction == other.direction &&
		color == other.color &&
		castShadow == other.castShadow &&
		castShadow2D == other.castShadow2D &&
		shadow == other.shadow;
}
