#ifndef __CDK__CSParticleShapeRect__
#define __CDK__CSParticleShapeRect__

#include "CSParticleShape.h"

class CSParticleShapeRect : public CSParticleShape {
public:
    CSVector2 range = CSVector2::Zero;

    CSParticleShapeRect() = default;
    CSParticleShapeRect(const CSVector2& range);
    CSParticleShapeRect(CSBuffer* buffer);
    CSParticleShapeRect(const CSParticleShapeRect* other);
private:
    ~CSParticleShapeRect() = default;
public:
    static inline CSParticleShapeRect* shape() {
        return autorelease(new CSParticleShapeRect());
    }
    static inline CSParticleShapeRect* shape(const CSVector2& range) {
        return autorelease(new CSParticleShapeRect(range));
    }

    inline Type type() const override {
        return TypeRect;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeRect);
    }
};

#endif
