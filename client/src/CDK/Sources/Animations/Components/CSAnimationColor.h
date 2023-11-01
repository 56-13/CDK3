#ifndef __CDK__CSAnimationColor__
#define __CDK__CSAnimationColor__

#include "CSObject.h"
#include "CSColor.h"

class CSBuffer;

class CSAnimationColor : public CSObject {
public:
    enum Type : byte {
        TypeConstant = 1,
        TypeLinear,
        TypeChannel
    };
protected:
    CSAnimationColor() = default;
public:
    virtual ~CSAnimationColor() = default;

    static CSAnimationColor* createWithBuffer(CSBuffer* buffer);
    static inline CSAnimationColor* colorWithBuffer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSAnimationColor* createWithColor(const CSAnimationColor* other);
    static inline CSAnimationColor* colorWithColor(const CSAnimationColor* other) {
        return autorelease(createWithColor(other));
    }

    virtual Type type() const = 0;
    inline bool animating() const {
        return type() != TypeConstant;
    }
    virtual int resourceCost() const = 0;
    virtual CSColor value(float t, const CSColor& r, const CSColor& default) const = 0;
};

#endif