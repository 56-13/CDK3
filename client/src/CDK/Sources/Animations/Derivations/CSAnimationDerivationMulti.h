#ifndef __CDK__CSAnimationDerivationMulti__
#define __CDK__CSAnimationDerivationMulti__

#include "CSAnimationDerivation.h"

class CSAnimationDerivationMulti;

class CSAnimationObjectDerivationMulti : public CSAnimationObjectDerivation {
private:
    const CSAnimationDerivationMulti* _origin;
    CSArray<CSAnimationObjectFragment> _children;
public:
    CSAnimationObjectDerivationMulti(const CSAnimationDerivationMulti* origin);
private:
    ~CSAnimationObjectDerivationMulti();
public:
    static inline CSAnimationObjectDerivationMulti* derivation(const CSAnimationDerivationMulti* origin) {
        return autorelease(new CSAnimationObjectDerivationMulti(origin));
    }
    inline const CSArray<CSAnimationObjectFragment>* children() const {
        return &_children;
    }
    inline const CSArray<CSAnimationObjectFragment, 1, false>* children() {
        return _children.asReadWrite();
    }
    void clearChildren();
    bool addChild(CSAnimationObjectFragment* child);
    bool removeChild(CSAnimationObjectFragment* child);
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
};

class CSAnimationDerivationMulti : public CSAnimationDerivation {
public:
    CSAnimationDerivationMulti() = default;
    inline CSAnimationDerivationMulti(CSBuffer* buffer) : CSAnimationDerivation(buffer) {}
    inline CSAnimationDerivationMulti(const CSAnimationDerivationMulti* other) : CSAnimationDerivation(other) {}
private:
    ~CSAnimationDerivationMulti() = default;
public:
    static inline CSAnimationDerivationMulti* derivation() {
        return autorelease(new CSAnimationDerivationMulti());
    }

    inline Type type() const override {
        return TypeMulti;
    }
    inline CSAnimationObjectDerivationMulti* createObject() const override {
        return new CSAnimationObjectDerivationMulti(this);
    }
    int resourceCost() const override;
};

#endif
