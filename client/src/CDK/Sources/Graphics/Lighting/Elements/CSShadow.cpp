#define CDK_IMPL

#include "CSShadow.h"

#include "CSBuffer.h"

CSShadow::CSShadow() : pixel32(true), resolution(1024), blur(4.0f), bias(0.001f), bleeding(0.9f) {

}

CSShadow::CSShadow(bool pixel32, int resolution, float blur, float bias, float bleeding) : 
	pixel32(pixel32),
	resolution(resolution),
	blur(blur),
	bias(bias),
	bleeding(bleeding)
{

}

CSShadow::CSShadow(CSBuffer* buffer) :
	pixel32(buffer->readBoolean()),
	resolution(buffer->readShort()),
	blur(buffer->readFloat()),
	bias(buffer->readFloat()),
	bleeding(buffer->readFloat()) 
{

}

CSShadow::CSShadow(const byte*& bytes) :
	pixel32(readBoolean(bytes)),
	resolution(readShort(bytes)),
	blur(readFloat(bytes)),
	bias(readFloat(bytes)),
	bleeding(readFloat(bytes)) 
{

}

uint CSShadow::hash() const {
	CSHash hash;
	hash.combine(pixel32);
	hash.combine(resolution);
	hash.combine(blur);
	hash.combine(bias);
	hash.combine(bleeding);
	return hash;
}

bool CSShadow::operator ==(const CSShadow& other) const {
	return pixel32 == other.pixel32 &&
		resolution == other.resolution &&
		blur == other.blur &&
		bias == other.bias &&
		bleeding == other.bleeding;
}
