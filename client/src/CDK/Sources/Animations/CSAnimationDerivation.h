#ifndef __CDK__CSAnimationDerivation__
#define __CDK__CSAnimationDerivation__

#include "CSGraphics.h"

#include "CSAnimationFragment.h"

class CSBuffer;

class CSAnimationDerivation : public CSObject {
public:
    enum Type {
        TypeMulti = 1,
        TypeLinked,
        TypeEmission,
        TypeRandom
    };
    CSArray<CSAnimationFragment> children;
    bool finish = true;
protected:
    CSAnimationDerivation();
    CSAnimationDerivation(CSBuffer* buffer);
    CSAnimationDerivation(const CSAnimationDerivation* other);
    virtual ~CSAnimationDerivation() = default;
public:
    static CSAnimationDerivation* createWithBuffer(CSBuffer* buffer);
    static inline CSAnimationDerivation* derivationWithBuffer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSAnimationDerivation* createWithDerivation(const CSAnimationDerivation* other);
    static inline CSAnimationDerivation* derivationWithDerivation(const CSAnimationDerivation* other) {
        return autorelease(createWithDerivation(other));
    }

    virtual Type type() const = 0;
    virtual CSAnimationObjectDerivation* createObject() const = 0;
    virtual int resourceCost() const = 0;
};

class CSAnimationObjectDerivation : public CSObject {
protected:
    CSAnimationObjectFragment* _parent;

    CSAnimationObjectDerivation() = default;
    virtual ~CSAnimationObjectDerivation() = default;
public:
#ifdef CDK_IMPL
    bool link(CSAnimationObjectFragment* parent);
    void unlink();
#endif
protected:
    virtual void spreadLink() = 0;
    virtual void spreadUnlink() = 0;
public:
    virtual bool addAABB(CSABoundingBox& result) const = 0;
    virtual void addCollider(CSCollider*& result) const = 0;
    virtual bool getTransform(float progress, const string& name, CSMatrix& result) const = 0;
    virtual float duration(CSSceneObject::DurationParam param, float duration) const = 0;
    virtual void rewind() = 0;
    virtual CSSceneObject::UpdateState update(float delta, bool alive, uint inflags, uint& outflags) = 0;
    virtual uint show() = 0;
    virtual void draw(CSGraphics* graphics, CSInstanceLayer layer) = 0;
};
#endif
