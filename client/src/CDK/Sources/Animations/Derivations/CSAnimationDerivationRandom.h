#ifndef __CDK__CSAnimationDerivationRandom__
#define __CDK__CSAnimationDerivationRandom__

#include "CSAnimationDerivation.h"

class CSAnimationDerivationRandom;

class CSAnimationObjectDerivationRandom : public CSAnimationObjectDerivation {
private:
    const CSAnimationDerivationRandom* _origin;
    CSAnimationObjectFragment* _selection;
public:
    CSAnimationObjectDerivationRandom(const CSAnimationDerivationRandom* origin);
private:
    ~CSAnimationObjectDerivationRandom();
public:
    static inline CSAnimationObjectDerivationRandom* derivation(const CSAnimationDerivationRandom* origin) {
        return autorelease(new CSAnimationObjectDerivationRandom(origin));
    }
protected:
    void spreadLink() override;
    void spreadUnlink() override;
public:
    bool addAABB(CSABoundingBox& result) const override;
    void addCollider(CSCollider*& result) const override;
    bool getTransform(float progress, const string& name, CSMatrix& result) const override;
    float duration(CSSceneObject::DurationParam param, float duration) const override;
    void rewind() override;
    CSSceneObject::UpdateState update(float delta, bool alive, uint inflags, uint& outflags) override;
    uint show() override;
    void draw(CSGraphics* graphics, CSInstanceLayer layer) override;
private:
    bool select();
};

class CSAnimationDerivationRandom : public CSAnimationDerivation {
public:
    bool loop = true;

    CSAnimationDerivationRandom() = default;
    CSAnimationDerivationRandom(CSBuffer* buffer);
    CSAnimationDerivationRandom(const CSAnimationDerivationRandom* other);
private:
    ~CSAnimationDerivationRandom() = default;
public:
    static inline CSAnimationDerivationRandom* derivation() {
        return autorelease(new CSAnimationDerivationRandom());
    }

    inline Type type() const override {
        return TypeRandom;
    }
    inline CSAnimationObjectDerivationRandom* createObject() const override {
        return new CSAnimationObjectDerivationRandom(this);
    }
    int resourceCost() const override;
};

#endif
