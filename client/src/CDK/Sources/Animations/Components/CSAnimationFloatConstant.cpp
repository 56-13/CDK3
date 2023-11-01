#define CDK_IMPL

#include "CSAnimationFloatConstant.h"

#include "CSBuffer.h"

CSAnimationFloatConstant::CSAnimationFloatConstant(float valueOrigin, float valueVar) : valueOrigin(valueOrigin), valueVar(valueVar) {

}

CSAnimationFloatConstant::CSAnimationFloatConstant(const CSAnimationFloatConstant* other) : valueOrigin(other->valueOrigin), valueVar(other->valueVar) {

}

CSAnimationFloatConstant::CSAnimationFloatConstant(CSBuffer* buffer) : valueOrigin(buffer->readFloat()), valueVar(buffer->readFloat()) {

}
