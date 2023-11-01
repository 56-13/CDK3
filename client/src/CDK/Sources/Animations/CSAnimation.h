#ifndef __CDK__CSAnimation__
#define __CDK__CSAnimation__

#include "CSSceneObject.h"

#include "CSAnimationFragment.h"
#include "CSAnimationDerivation.h"

class CSAnimationBuilder;
class CSAnimationObject;

class CSAnimation : public CSResource {
private:
    CSAnimationFragment* _root;
public:
    CSAnimation();
    CSAnimation(CSBuffer* buffer);
    CSAnimation(const CSAnimation* other);
private:
    ~CSAnimation();
public:
    static inline CSAnimation* animation() {
        return autorelease(new CSAnimation());
    }
    static inline CSAnimation* animationWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSAnimation(buffer));
    }
    static inline CSAnimation* animationWithAnimation(const CSAnimation* other) {
        return autorelease(new CSAnimation(other));
    }

    inline CSAnimationFragment* root() {
        return _root;
    }
    inline const CSAnimationFragment* root() const {
        return _root;
    }
    inline CSResourceType resourceType() const override {
        return CSResourceTypeAnimation;
    }
    int resourceCost() const override;

    inline void preload() const {
        _root->preload();
    }
};

class CSAnimationObject : public CSSceneObject {
private:
    CSAnimationObjectFragment* _root;
    CSGizmo* _targets[3];
    uint _keyFlags;
    bool _alive;
public:
    std::function<void(CSGraphics*, const byte*)> spriteExternDelegate;

    CSAnimationObject(const CSAnimation* origin, const CSAnimationBuilder* builder = NULL);
private:
    ~CSAnimationObject();
public:
    static inline CSAnimationObject* object(const CSAnimation* origin, const CSAnimationBuilder* builder = NULL) {
        return autorelease(new CSAnimationObject(origin, builder));
    }

    inline Type type() const override {
        return TypeAnimation;
    }
    inline CSAnimationObjectFragment* root() {
        return _root;
    }
    inline const CSAnimationObjectFragment* root() const {
        return _root;
    }
    inline void setKeyFlags(uint keyFlags) {
        _keyFlags = keyFlags;
    }
    inline uint keyFlags() const {
        return _keyFlags;
    }
    inline void stop() {
        _alive = false;
    }
    void useTarget(int i, bool flag);
    CSGizmo* target(int i);
    const CSGizmo* target(int i) const {
        return const_cast<CSAnimationObject*>(this)->target(i);
    }
    bool getTargetTransform(int i, float progress, CSMatrix& result) const;

    inline bool addAABB(CSABoundingBox& result) const override {
        return _root->addAABB(result);
    }
    inline void addCollider(CSCollider*& result) const override {
        _root->addCollider(result);
    }
    bool getTransform(float progress, const string& name, CSMatrix& result) const override;
    inline float duration(DurationParam param, float duration) const override {
        return _root->duration(param);
    }
    float progress() const override {
        return _root->progress();
    }
    bool getUpdatePass(CSUpdatePass& pass) const override;
protected:
    void onLink() override;
    void onUnlink() override;
    void onRewind() override;
    UpdateState onUpdate(float delta, bool alive, uint& flags) override;
    uint onShow() override;
    void onDraw(CSGraphics* graphics, CSInstanceLayer layer) override;
};

class CSAnimationBuilder : public CSSceneObjectBuilder {
public:
    CSPtr<CSGizmoData> targets[3];
    uint keyFlags = 0xFFFFFFFF;
private:
    const CSArray<ushort>* _indices = NULL;
    const CSAnimation* _animation = NULL;
public:
    CSAnimationBuilder(const CSArray<ushort>* indices);
    CSAnimationBuilder(const CSAnimation* animation);
    CSAnimationBuilder(CSBuffer* buffer, bool withScene, bool withData);
    CSAnimationBuilder(const CSAnimationBuilder* other);
private:
    ~CSAnimationBuilder();
public:
    static inline CSAnimationBuilder* builder(const CSArray<ushort>* indices) {
        return autorelease(new CSAnimationBuilder(indices));
    }
    static inline CSAnimationBuilder* builder(const CSAnimation* animation) {
        return autorelease(new CSAnimationBuilder(animation));
    }
    static inline CSAnimationBuilder* builderWithBuilder(const CSAnimationBuilder* other) {
        return autorelease(new CSAnimationBuilder(other));
    }

    const CSAnimation* origin() const;

    inline CSSceneObject::Type type() const override {
        return CSSceneObject::TypeAnimation;
    }
    int resourceCost() const override;
    CSAnimationObject* createObject() const override;
    void preload() const override;
};

#endif