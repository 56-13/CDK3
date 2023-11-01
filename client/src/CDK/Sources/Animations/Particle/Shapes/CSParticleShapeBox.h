#ifndef __CDK__CSParticleShapeBox__
#define __CDK__CSParticleShapeBox__

#include "CSParticleShape.h"

class CSParticleShapeBox : public CSParticleShape {
public:
    CSVector3 range = CSVector3::Zero;

    CSParticleShapeBox() = default;
    CSParticleShapeBox(const CSVector3& range);
    CSParticleShapeBox(CSBuffer* buffer);
    CSParticleShapeBox(const CSParticleShapeBox* other);
private:
    ~CSParticleShapeBox() = default;
public:
    static inline CSParticleShapeBox* shape() {
        return autorelease(new CSParticleShapeBox());
    }
    static inline CSParticleShapeBox* shape(const CSVector3& range) {
        return autorelease(new CSParticleShapeBox(range));
    }

    inline Type type() const override {
        return TypeBox;
    }
    void addAABB(const CSMatrix& transform, CSABoundingBox& result) const override;
    void issue(CSVector3& position, CSVector3& direction, bool shell) const override;
    inline int resourceCost() const override {
        return sizeof(CSParticleShapeBox);
    }
};

#endif
