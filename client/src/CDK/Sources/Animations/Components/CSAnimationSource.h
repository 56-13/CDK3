#ifndef CSAnimationSource_h
#define CSAnimationSource_h

#include "CSObject.h"

class CSBuffer;

class CSAnimationSource : public CSObject {
public:
    enum Type {
        TypeImage = 1,
        TypeMesh
    };
protected:
    CSAnimationSource() = default;
public:
    virtual ~CSAnimationSource() = default;

    static CSAnimationSource* createWithBuffer(CSBuffer* buffer);
    static inline CSAnimationSource* sourceWithBuffer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSAnimationSource* createWitSource(const CSAnimationSource* other);
    static inline CSAnimationSource* sourceWithSource(const CSAnimationSource* other) {
        return autorelease(createWitSource(other));
    }

    virtual Type type() const = 0;
    virtual int resourceCost() const = 0;
    virtual void preload() const = 0;
};

#endif
