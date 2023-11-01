#define CDK_IMPL

#include "CSParticleShape.h"

#include "CSParticleShapeSphere.h"
#include "CSParticleShapeHamisphere.h"
#include "CSParticleShapeCone.h"
#include "CSParticleShapeCircle.h"
#include "CSParticleShapeBox.h"
#include "CSParticleShapeRect.h"

#include "CSBuffer.h"

CSParticleShape* CSParticleShape::createWithBuffer(CSBuffer* buffer) {
    switch (buffer->readByte()) {
        case TypeSphere:
            return new CSParticleShapeSphere(buffer);
        case TypeHamisphere:
            return new CSParticleShapeHamisphere(buffer);
        case TypeCone:
            return new CSParticleShapeCone(buffer);
        case TypeCircle:
            return new CSParticleShapeCircle(buffer);
        case TypeBox:
            return new CSParticleShapeBox(buffer);
        case TypeRect:
            return new CSParticleShapeRect(buffer);
    }
    return NULL;
}

CSParticleShape* CSParticleShape::createWithShape(const CSParticleShape* other) {
    if (other) {
        switch (other->type()) {
            case TypeSphere:
                return new CSParticleShapeSphere(static_cast<const CSParticleShapeSphere*>(other));
            case TypeHamisphere:
                return new CSParticleShapeHamisphere(static_cast<const CSParticleShapeHamisphere*>(other));
            case TypeCone:
                return new CSParticleShapeCone(static_cast<const CSParticleShapeCone*>(other));
            case TypeCircle:
                return new CSParticleShapeCircle(static_cast<const CSParticleShapeCircle*>(other));
            case TypeBox:
                return new CSParticleShapeBox(static_cast<const CSParticleShapeBox*>(other));
            case TypeRect:
                return new CSParticleShapeRect(static_cast<const CSParticleShapeRect*>(other));
        }
    }
    return NULL;
}

