#ifndef __CDK__CSParticle__
#define __CDK__CSParticle__

#include "CSQueue.h"

#include "CSParticleShape.h"

#include "CSAnimationSource.h"
#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"

#include "CSMeshObject.h"

class CSParticleBuilder;

class CSParticleObject : public CSSceneObject {
private:
    static constexpr int InstanceRandomCount = 18;

    struct Instance {
        CSPtr<const CSAnimationSource> source;
        CSMatrix* transform;
        CSMatrix world;
        CSVector3 firstDir;
        CSVector3 lastDir;
        CSVector3 pos;
        CSVector3 billboardDir;
        CSVector3 billboard;
        CSColor color;
        float randoms[InstanceRandomCount];
        float life;
        float progress;
        float delta;

        Instance();
        ~Instance();

        Instance(const Instance& other) = delete;
        Instance& operator =(const Instance& other) = delete;
    };
    struct MeshFragment {
        const CSAnimationSource* source;
        CSMeshObject* origin;
        CSArray<CSVertexArrayInstance> instances;

        MeshFragment(const CSAnimationSource* source, CSMeshObject* origin);
        ~MeshFragment();

        MeshFragment(const MeshFragment& other) = delete;
        MeshFragment& operator =(const MeshFragment& other) = delete;
    };
    const CSParticleBuilder* _origin;
    CSQueue<Instance> _instances;
    float _progress;
    float _remaining;
    float _counter;
    bool _emitting;
    bool _visible;
    int _materialRandom;
    CSArray<const Instance*, 2> _imageFragments;
    CSArray<MeshFragment> _meshFragments;
public:
    CSParticleObject(const CSParticleBuilder* origin);
private:
    ~CSParticleObject();
public:
    static inline CSParticleObject* object(const CSParticleBuilder* origin) {
        return autorelease(new CSParticleObject(origin));
    }

    inline Type type() const override {
        return TypeParticle;
    }
    inline void stop() {
        _emitting = false;
    }
    bool addAABB(CSABoundingBox& result) const override;
    float duration(DurationParam param, float duration) const override;
    inline float progress() const override {
        return _progress;
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
    UpdateState update(float delta, bool alive);
    void addInstance(const CSMatrix* transform, float delta);
    void updateInstance(Instance& p, const CSMatrix& worldPrev, const CSMatrix* cameraView);
};

class CSParticleBuilder : public CSSceneObjectBuilder {
public:
    enum View : byte {
        ViewNone,
        ViewBillboard,
        ViewHorizontalBillboard,
        ViewVerticalBillboard,
        ViewStretchBillboard
    };
    CSArray<CSAnimationSource> sources;
    CSPtr<CSParticleShape> shape;
    bool shapeShell = false;
    CSPtr<CSAnimationColor> color;
    CSPtr<CSAnimationFloat> radial;
    CSPtr<CSAnimationFloat> x;
    CSPtr<CSAnimationFloat> y;
    CSPtr<CSAnimationFloat> z;
    CSPtr<CSAnimationFloat> billboardX;
    CSPtr<CSAnimationFloat> billboardY;
    CSPtr<CSAnimationFloat> billboardZ;
    CSPtr<CSAnimationFloat> rotationX;
    CSPtr<CSAnimationFloat> rotationY;
    CSPtr<CSAnimationFloat> rotationZ;
    CSPtr<CSAnimationFloat> scaleX;
    CSPtr<CSAnimationFloat> scaleY;
    CSPtr<CSAnimationFloat> scaleZ;
    bool scaleEach = false;
    View view = ViewNone;
    float stretchRate = 0;
    bool localSpace = true;
    bool prewarm = false;
    bool finish = false;
    bool clear = false;
    float life = 0;
    float lifeVar = 0;
    float emissionRate = 0;
    int emissionMax = 0;

    CSParticleBuilder() = default;
    CSParticleBuilder(CSBuffer* buffer, bool withScene);
    CSParticleBuilder(const CSParticleBuilder* other);
private:
    ~CSParticleBuilder() = default;
public:
    static inline CSParticleBuilder* builder() {
        return autorelease(new CSParticleBuilder());
    }
    static inline CSParticleBuilder* builderWithBuilder(const CSParticleBuilder* other) {
        return autorelease(new CSParticleBuilder(other));
    }

    inline CSSceneObject::Type type() const override {
        return CSSceneObject::TypeParticle;
    }
    int resourceCost() const override;
    inline CSParticleObject* createObject() const override {
        return new CSParticleObject(this);
    }
    void preload() const override;
};

#endif
