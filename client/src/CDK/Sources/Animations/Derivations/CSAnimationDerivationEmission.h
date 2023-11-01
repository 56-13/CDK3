#ifndef __CDK__CSAnimationDerivationEmission__
#define __CDK__CSAnimationDerivationEmission__

#include "CSAnimationDerivation.h"

class CSAnimationDerivationEmission;

class CSAnimationObjectDerivationEmission : public CSAnimationObjectDerivation {
private:
    const CSAnimationDerivationEmission* _origin;
    CSArray<CSAnimationObjectFragment> _instances;
    ushort _index;
    float _counter;
public:
    CSAnimationObjectDerivationEmission(const CSAnimationDerivationEmission* origin);
private:
    ~CSAnimationObjectDerivationEmission();
public:
    static inline CSAnimationObjectDerivationEmission* derivation(const CSAnimationDerivationEmission* origin) {
        return autorelease(new CSAnimationObjectDerivationEmission(origin));
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
};

class CSAnimationDerivationEmission : public CSAnimationDerivation {
public:
    float delay = 0;
    ushort capacity = 0;
    bool prewarm = false;

    CSAnimationDerivationEmission() = default;
    CSAnimationDerivationEmission(CSBuffer* buffer);
    CSAnimationDerivationEmission(const CSAnimationDerivationEmission* other);
private:
    ~CSAnimationDerivationEmission() = default;
public:
    static inline CSAnimationDerivationEmission* derivation() {
        return autorelease(new CSAnimationDerivationEmission());
    }

    inline Type type() const override {
        return TypeEmission;
    }
    inline CSAnimationObjectDerivationEmission* createObject() const override {
        return new CSAnimationObjectDerivationEmission(this);
    }
    int resourceCost() const override;
};
#endif
