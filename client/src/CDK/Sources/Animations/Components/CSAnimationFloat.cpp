#define CDK_IMPL

#include "CSAnimationFloat.h"

#include "CSAnimationFloatConstant.h"
#include "CSAnimationFloatLinear.h"
#include "CSAnimationFloatCurve.h"

#include "CSBuffer.h"

CSAnimationFloat* CSAnimationFloat::createWithBuffer(CSBuffer* buffer) {
    switch (buffer->readByte()) {
        case TypeConstant:
            return new CSAnimationFloatConstant(buffer);
        case TypeLinear:
            return new CSAnimationFloatLinear(buffer);
        case TypeCurve:
            return new CSAnimationFloatCurve(buffer);
    }
    return NULL;
}

CSAnimationFloat* CSAnimationFloat::createWithFactor(const CSAnimationFloat* other) {
    if (other) {
        switch (other->type()) {
            case TypeConstant:
                return new CSAnimationFloatConstant(static_cast<const CSAnimationFloatConstant*>(other));
            case TypeLinear:
                return new CSAnimationFloatLinear(static_cast<const CSAnimationFloatLinear*>(other));
            case TypeCurve:
                return new CSAnimationFloatCurve(static_cast<const CSAnimationFloatCurve*>(other));
        }
    }
    return NULL;
}
