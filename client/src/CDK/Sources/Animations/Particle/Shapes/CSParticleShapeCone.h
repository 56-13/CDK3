#ifndef __CDK__CSParticleShapeCone__
#define __CDK__CSParticleShapeCone__

#include "CSParticleShape.h"

class CSParticleShapeCone : public CSParticleShape {
public:
    float topRange = 0;
    float bottomRange = 0;
    float height = 0;

    CSParticleShapeCone() = default;
    CSParticleShapeCone(float topRange, float bottomRange, float height);
    CSParticleShapeCone(CSBuffer* buffer);
    CSParticleShapeCone(const CSParticleShapeCone* other);
private:
    ~CSParticleShapeCone() = default;
public:
    static inline CSParticleShapeCone* shape() {
        return autorelease(new CSParticleShapeCone());
    }
    static inline CSParticleShapeCone* shape(float topRange, float bottomRange, float height) {
        return autorelease(new CSParticleShapeCone(topRange, bottomRange, height));
    }

    inline Type type() const override {
        return TypeCone;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeCone);
    }
};

#endif
