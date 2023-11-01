#ifndef __CDK__CSParticleShapeHamisphere__
#define __CDK__CSParticleShapeHamisphere__

#include "CSParticleShape.h"

class CSParticleShapeHamisphere : public CSParticleShape {
public:
    float range = 0;

    CSParticleShapeHamisphere() = default;
    CSParticleShapeHamisphere(float range);
    CSParticleShapeHamisphere(CSBuffer* buffer);
    CSParticleShapeHamisphere(const CSParticleShapeHamisphere* other);
private:
    ~CSParticleShapeHamisphere() = default;
public:
    static inline CSParticleShapeHamisphere* shape() {
        return autorelease(new CSParticleShapeHamisphere());
    }
    static inline CSParticleShapeHamisphere* shape(float range) {
        return autorelease(new CSParticleShapeHamisphere(range));
    }

    inline Type type() const override {
        return TypeHamisphere;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeHamisphere);
    }
};

#endif
