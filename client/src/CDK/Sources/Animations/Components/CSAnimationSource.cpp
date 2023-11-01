#define CDK_IMPL

#include "CSAnimationSource.h"

#include "CSAnimationSourceImage.h"
#include "CSAnimationSourceMesh.h"

#include "CSBuffer.h"

CSAnimationSource* CSAnimationSource::createWithBuffer(CSBuffer* buffer) {
	switch (buffer->readByte()) {
		case TypeImage:
			return new CSAnimationSourceImage(buffer);
		case TypeMesh:
			return new CSAnimationSourceMesh(buffer);
	}
	return NULL;
}

CSAnimationSource* CSAnimationSource::createWitSource(const CSAnimationSource* other) {
	if (other) {
		switch (other->type()) {
			case TypeImage:
				return new CSAnimationSourceImage(static_cast<const CSAnimationSourceImage*>(other));
			case TypeMesh:
				return new CSAnimationSourceMesh(static_cast<const CSAnimationSourceMesh*>(other));
		}
	}
	return NULL;
}