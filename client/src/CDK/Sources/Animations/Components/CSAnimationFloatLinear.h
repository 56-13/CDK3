#ifndef __CDK__CSAnimationFloatLinear__
#define __CDK__CSAnimationFloatLinear__

#include "CSAnimationFloat.h"

#include "CSMath.h"

struct CSAnimationFloatLinear : public CSAnimationFloat {
public:
    float startValueOrigin = 0;
    float startValueVar = 0;
    float endValueOrigin = 0;
    float endValueVar = 0;
    bool smooth = false;

    CSAnimationFloatLinear() = default;
    CSAnimationFloatLinear(float startValueOrigin, float startValueVar, float endValueOrigin, float endValueVar, bool smooth);
    explicit CSAnimationFloatLinear(CSBuffer* buffer);
    CSAnimationFloatLinear(const CSAnimationFloatLinear* other);

    static inline CSAnimationFloatLinear* factor() {
        return autorelease(new CSAnimationFloatLinear());
    }
    static inline CSAnimationFloatLinear* factor(float startValueOrigin, float startValueVar, float endValueOrigin, float endValueVar, bool smooth) {
        return autorelease(new CSAnimationFloatLinear(startValueOrigin, startValueVar, endValueOrigin, endValueVar, smooth));
    }
    
    inline Type type() const override {
        return TypeLinear;
    }
    inline int resourceCost() const override {
        return sizeof(CSAnimationFloatLinear);
    }
    float value(float t) const override;
    float value(float t, float r) const override;
};

#endif
