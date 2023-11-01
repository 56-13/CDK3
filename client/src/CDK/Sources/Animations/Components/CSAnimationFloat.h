#ifndef __CDK__CSAnimationFloat__
#define __CDK__CSAnimationFloat__

#include "CSObject.h"

class CSBuffer;

struct CSAnimationFloat : public CSObject {
public:
    enum Type : byte {
        TypeConstant = 1,
        TypeLinear,
        TypeCurve
    };
protected:
    CSAnimationFloat() = default;
public:
    virtual ~CSAnimationFloat() = default;

    static CSAnimationFloat* createWithBuffer(CSBuffer* buffer);
    static inline CSAnimationFloat* factorWithBuffer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSAnimationFloat* createWithFactor(const CSAnimationFloat* other);
    static inline CSAnimationFloat* factorWithFactor(const CSAnimationFloat* other) {
        return autorelease(createWithFactor(other));
    }
    
    virtual Type type() const = 0;
    inline bool animating() const {
        return type() != TypeConstant;
    }
    virtual int resourceCost() const = 0;
    virtual float value(float t) const = 0;
    virtual float value(float t, float r) const = 0;
};

#endif
