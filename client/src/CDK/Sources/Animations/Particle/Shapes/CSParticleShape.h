#ifndef __CDK__CSParticleShape__
#define __CDK__CSParticleShape__

#include "CSObject.h"

#include "CSABoundingBox.h"

class CSBuffer;

class CSParticleShape : public CSObject {
public:
    enum Type : byte {
        TypeSphere = 1,
        TypeHamisphere,
        TypeCone,
        TypeCircle,
        TypeBox,
        TypeRect
    };
protected:
    CSParticleShape() = default;
    virtual ~CSParticleShape() = default;
public:
    static CSParticleShape* createWithBuffer(CSBuffer* buffer);
    static inline CSParticleShape* shapeWithBuffer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSParticleShape* createWithShape(const CSParticleShape* other);
    static inline CSParticleShape* shapeWithShape(const CSParticleShape* other) {
        return autorelease(createWithShape(other));
    }

    virtual Type type() const = 0;
    virtual void addAABB(const CSMatrix& transform, CSABoundingBox& result) const = 0;
    virtual void issue(CSVector3& position, CSVector3& direction, bool shell) const = 0;
    virtual int resourceCost() const = 0;
};

#endif
