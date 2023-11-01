#define CDK_IMPL

#include "CSAnimationColor.h"

#include "CSAnimationColorConstant.h"
#include "CSAnimationColorLinear.h"
#include "CSAnimationColorChannel.h"

#include "CSBuffer.h"

CSAnimationColor* CSAnimationColor::createWithBuffer(CSBuffer* buffer) {
	switch (buffer->readByte()) {
		case TypeConstant:
			return new CSAnimationColorConstant(buffer);
		case TypeLinear:
			return new CSAnimationColorLinear(buffer);
		case TypeChannel:
			return new CSAnimationColorChannel(buffer);
	}
	return NULL;
}

CSAnimationColor* CSAnimationColor::createWithColor(const CSAnimationColor* other) {
	if (other) {
		switch (other->type()) {
			case TypeConstant:
				return new CSAnimationColorConstant(static_cast<const CSAnimationColorConstant*>(other));
			case TypeLinear:
				return new CSAnimationColorLinear(static_cast<const CSAnimationColorLinear*>(other));
			case TypeChannel:
				return new CSAnimationColorChannel(static_cast<const CSAnimationColorChannel*>(other));
		}
	}
	return NULL;
}