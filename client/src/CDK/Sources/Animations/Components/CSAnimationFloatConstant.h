#ifndef __CDK__CSAnimationFloatConstant__
#define __CDK__CSAnimationFloatConstant__

#include "CSAnimationFloat.h"

#include "CSMath.h"

struct CSAnimationFloatConstant : public CSAnimationFloat {
public:
    float valueOrigin = 0;
    float valueVar = 0;

    CSAnimationFloatConstant() = default;
    CSAnimationFloatConstant(float valueOrigin, float valueVar);
    explicit CSAnimationFloatConstant(CSBuffer* buffer);
    CSAnimationFloatConstant(const CSAnimationFloatConstant* other);

    static inline CSAnimationFloatConstant* factor() {
        return autorelease(new CSAnimationFloatConstant());
    }
    static inline CSAnimationFloatConstant* factor(float value, float valueVar) {
        return autorelease(new CSAnimationFloatConstant(value, valueVar));
    }
    
    inline Type type() const override {
        return TypeConstant;
    }
    inline int resourceCost() const override {
        return sizeof(CSAnimationFloatConstant);
    }
    inline float value(float t) const override {
        return valueOrigin;
    }
    inline float value(float t, float r) const override {
        return valueVar ? CSMath::lerp(valueOrigin - valueVar, valueOrigin + valueVar, r) : valueOrigin;
    }
};

#endif
