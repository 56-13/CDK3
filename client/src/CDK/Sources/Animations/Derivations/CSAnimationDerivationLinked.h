#ifndef __CDK__CSAnimationDerivationLinked__
#define __CDK__CSAnimationDerivationLinked__

#include "CSAnimationDerivation.h"

class CSAnimationDerivationLinked;

class CSAnimationObjectDerivationLinked : public CSAnimationObjectDerivation {
private:
    const CSAnimationDerivationLinked* _origin;
    CSArray<CSAnimationObjectFragment> _children;
    ushort _loop;
    ushort _current;
    ushort _count;
public:
    CSAnimationObjectDerivationLinked(const CSAnimationDerivationLinked* origin);
private:
    ~CSAnimationObjectDerivationLinked();
public:
    static inline CSAnimationObjectDerivationLinked* derivation(const CSAnimationDerivationLinked* origin) {
        return autorelease(new CSAnimationObjectDerivationLinked(origin));
    }
    
    inline const CSArray<CSAnimationObjectFragment>* children() const {
        return &_children;
    }
    inline const CSArray<CSAnimationObjectFragment, 1, false>* children() {
        return _children.asReadWrite();
    }
    void clearChildren();
    bool addChild(CSAnimationObjectFragment* child);
    bool insertChild(int i, CSAnimationObjectFragment* child);
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
private:
    void rewindProgress();
};

class CSAnimationDerivationLinked : public CSAnimationDerivation {
public:
    ushort loopCount = 0;

    CSAnimationDerivationLinked() = default;
    CSAnimationDerivationLinked(CSBuffer* buffer);
    CSAnimationDerivationLinked(const CSAnimationDerivationLinked* other);
private:
    ~CSAnimationDerivationLinked() = default;
public:
    static inline CSAnimationDerivationLinked* derivation() {
        return autorelease(new CSAnimationDerivationLinked());
    }

    inline Type type() const override {
        return TypeLinked;
    }
    inline CSAnimationObjectDerivationLinked* createObject() const override {
        return new CSAnimationObjectDerivationLinked(this);
    }
    int resourceCost() const override;
};

#endif
