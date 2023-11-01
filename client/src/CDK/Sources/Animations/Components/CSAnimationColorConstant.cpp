#define CDK_IMPL

#include "CSAnimationColorConstant.h"

#include "CSBuffer.h"

CSAnimationColorConstant::CSAnimationColorConstant(const CSColor& color) : origin(color) {

}

CSAnimationColorConstant::CSAnimationColorConstant(const CSAnimationColorConstant* other) : origin(other->origin) {

}

CSAnimationColorConstant::CSAnimationColorConstant(CSBuffer* buffer) {
	bool normalized = buffer->readBoolean();
	bool alphaChannel = buffer->readBoolean();

	if (alphaChannel) origin = CSColor(buffer, normalized);
	else origin = CSColor3(buffer, normalized);
}
