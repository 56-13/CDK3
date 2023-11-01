#ifndef __CDK__CSParticleShapeSphere__
#define __CDK__CSParticleShapeSphere__

#include "CSParticleShape.h"

class CSParticleShapeSphere : public CSParticleShape {
public:
    float range = 0;

    CSParticleShapeSphere() = default;
    CSParticleShapeSphere(float range);
    CSParticleShapeSphere(CSBuffer* buffer);
    CSParticleShapeSphere(const CSParticleShapeSphere* other);
private:
    ~CSParticleShapeSphere() = default;
public:
    static inline CSParticleShapeSphere* shape() {
        return autorelease(new CSParticleShapeSphere());
    }
    static inline CSParticleShapeSphere* shape(float range) {
        return autorelease(new CSParticleShapeSphere(range));
    }

    inline Type type() const override {
        return TypeSphere;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeSphere);
    }
};

#endif
