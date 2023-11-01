#ifndef __CDK__CSSprite__
#define __CDK__CSSprite__

#include "CSSpriteTimeline.h"
#include "CSAnimationLoop.h"
#include "CSMeshObject.h"

class CSSpriteBuilder;

class CSSpriteObject : public CSSceneObject {
private:
    const CSSpriteBuilder* _origin;
    float _progress;
    float _clippedProgress;
    int64 _randomSeed;
    CSMatrix _transform;
    bool _visible;
    bool _cursor;
    mutable CSDictionary<const void*, CSMeshObject> _meshInstances;
    mutable CSArray<const CSSpriteTimeline> _timelines[2];
public:
    std::function<void(CSGraphics*, const byte*)> externDelegate;

    CSSpriteObject(const CSSpriteBuilder* origin);
private:
    ~CSSpriteObject();
public:
    static inline CSSpriteObject* object(const CSSpriteBuilder* origin) {
        return autorelease(new CSSpriteObject(origin));
    }

    inline Type type() const override {
        return TypeSprite;
    }
    bool addAABB(CSABoundingBox& result) const override;
    void addCollider(CSCollider*& result) const override;
    bool getTransform(float progress, const string& name, CSMatrix& result) const override;
    float duration(DurationParam param, float duration) const override;
    inline float progress() const override {
        return _progress;
    }
    inline float clippedProgress() const {
        return _clippedProgress;
    }
    bool afterCameraUpdate() const override;
protected:
    void onLink() override;
    void onUnlink() override;
    void onRewind() override;
    UpdateState onUpdate(float delta, bool alive, uint& flags) override;
    uint onShow() override;
    void onDraw(CSGraphics* graphics, CSInstanceLayer layer) override;
private:
    bool clipProgress();
    float clipProgress(float progress) const;
    void resetTimelines();
    void getTimelines(float progress, CSArray<const CSSpriteTimeline>* result) const;

    friend class CSSpriteElementMesh;
};

class CSSpriteBuilder : public CSSceneObjectBuilder {
public:
    CSArray<CSSpriteTimeline> timelines;
    CSAnimationLoop loop;
    bool billboard = false;

    CSSpriteBuilder() = default;
    CSSpriteBuilder(CSBuffer* buffer, bool withScene);
    CSSpriteBuilder(const CSSpriteBuilder* other);
private:
    ~CSSpriteBuilder() = default;
public:
    static inline CSSpriteBuilder* builder() {
        return autorelease(new CSSpriteBuilder());
    }
    static CSSpriteBuilder* builderWithBuilder(const CSSpriteBuilder* other) {
        return autorelease(new CSSpriteBuilder(other));
    }

    inline CSSceneObject::Type type() const override {
        return CSSceneObject::TypeSprite;
    }
    int resourceCost() const override;
    inline CSSpriteObject* createObject() const override {
        return new CSSpriteObject(this);
    }
    void preload() const override;

    float singleDuration() const;
    float totalDuration() const;
};

#endif
