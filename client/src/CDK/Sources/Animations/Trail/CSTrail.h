#ifndef __CDK__CSTrail__
#define __CDK__CSTrail__

#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"
#include "CSAnimationSourceImage.h"
#include "CSAnimationSourceMesh.h"

#include "CSSceneObject.h"

class CSTrailBuilder;

class CSTrailObject : public CSSceneObject {
private:
    struct Point {
        CSVector3 point;
        CSMatrix transform;
        float progress;
        int link;

        Point(const CSVector3& point, float progress, int link);
    };
    const CSTrailBuilder* _origin;
    CSArray<Point> _points;
    float _progress;
    float _remaining;
    int _counter;
    int _link;
    CSColor _colorRandom;
    float _rotationRandom;
    float _scaleRandom;
    int _materialRandom;
    bool _emitting;
    CSArray<CSMeshObject> _meshInstances;
public:
    CSTrailObject(const CSTrailBuilder* origin);
private:
    ~CSTrailObject();
public:
    static inline CSTrailObject* object(const CSTrailBuilder* origin) {
        return autorelease(new CSTrailObject(origin));
    }

    inline Type type() const override {
        return TypeTrail;
    }
    inline void stop() {
        _emitting = false;
    }
    bool addAABB(CSABoundingBox& result) const override;
    float duration(DurationParam param, float duration) const override;
    inline float progress() const override {
        return _progress;
    }
    inline bool afterCameraUpdate() const override {
        return true;
    }
protected:
    void onLink() override;
    void onUnlink() override;
    void onRewind() override;
    UpdateState onUpdate(float delta, bool alive, uint& flags) override;
    uint onShow() override;
    void onDraw(CSGraphics* graphics, CSInstanceLayer layer) override;
private:
    void resetRandoms();

    enum BreakEntryType {
        BreakEntryTypeNone,
        BreakEntryTypeRewind,
        BreakEntryTypeReverse
    };
    struct BreakEntry {
        float delta;
        BreakEntryType type;
    };
    void addBreak(float progress, float delta, float duration, const CSAnimationLoop& loop, CSArray<BreakEntry>*& breaks);
    void addPoint(const CSVector3& p, float progress);
    void updatePointTransforms(int s, int e);
    uint showMesh(const CSAnimationSourceMesh* source, int s, int e, int& meshCount);
    void drawImage(CSGraphics* graphics, const CSAnimationSourceImage* source, int s, int e);
    void drawImages(CSGraphics* graphics, CSInstanceLayer layer);
    void drawMeshes(CSGraphics* graphics, CSInstanceLayer layer);
};

class CSTrailBuilder : public CSSceneObjectBuilder {
public:
    CSArray<CSAnimationSource> sources;
    float distance = 0;
    bool billboard = false;
    bool localSpace = false;
    bool emission = false;
    byte emissionSmoothness = 0;
    float emissionLife = 0;
    float repeatScale = 0;
    CSPtr<CSAnimationColor> color;
    float colorDuration = 0;
    CSAnimationLoop colorLoop;
    CSPtr<CSAnimationFloat> rotation;
    float rotationDuration = 0;
    CSAnimationLoop rotationLoop;
    CSPtr<CSAnimationFloat> scale;
    float scaleDuration = 0;
    CSAnimationLoop scaleLoop;

    CSTrailBuilder() = default;
    CSTrailBuilder(CSBuffer* buffer, bool withScene);
    CSTrailBuilder(const CSTrailBuilder* other);
private:
    ~CSTrailBuilder() = default;
public:
    static inline CSTrailBuilder* builder() {
        return autorelease(new CSTrailBuilder());
    }
    static inline CSTrailBuilder* builderWithBuilder(const CSTrailBuilder* other) {
        return autorelease(new CSTrailBuilder(other));
    }

    inline CSSceneObject::Type type() const override {
        return CSSceneObject::TypeTrail;
    }
    int resourceCost() const override;
    inline CSTrailObject* createObject() const override {
        return new CSTrailObject(this);
    }
    void preload() const override;
};

#endif
