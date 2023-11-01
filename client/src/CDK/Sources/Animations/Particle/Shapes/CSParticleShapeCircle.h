#ifndef __CDK__CSParticleShapeCircle__
#define __CDK__CSParticleShapeCircle__

#include "CSParticleShape.h"

class CSParticleShapeCircle : public CSParticleShape {
public:
    CSVector2 range = CSVector2::Zero;

    CSParticleShapeCircle() = default;
    CSParticleShapeCircle(const CSVector2& range);
    CSParticleShapeCircle(CSBuffer* buffer);
    CSParticleShapeCircle(const CSParticleShapeCircle* other);
private:
    ~CSParticleShapeCircle() = default;
public:
    static inline CSParticleShapeCircle* shape() {
        return autorelease(new CSParticleShapeCircle());
    }
    static inline CSParticleShapeCircle* shape(const CSVector2& range) {
        return autorelease(new CSParticleShapeCircle(range));
    }

    inline Type type() const override {
        return TypeCircle;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeCircle);
    }
};

#endif
