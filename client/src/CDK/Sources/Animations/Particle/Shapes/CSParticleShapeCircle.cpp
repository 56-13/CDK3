#define CDK_IMPL

#include "CSParticleShapeCircle.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeCircle::CSParticleShapeCircle(const CSVector2& range) : range(range) {

}

CSParticleShapeCircle::CSParticleShapeCircle(CSBuffer* buffer) : range(buffer) {

}

CSParticleShapeCircle::CSParticleShapeCircle(const CSParticleShapeCircle* other) : range(other->range) {

}

void CSParticleShapeCircle::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    result.append(CSVector3::transformCoordinate(CSVector3(-range.x, -range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(range.x, -range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(-range.x, range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(range.x, range.y, 0), transform));
}

void CSParticleShapeCircle::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    float a = randFloat(-FloatPi, FloatPi);
    if (shell) {
        position = CSVector3(CSMath::cos(a) * range.x, CSMath::sin(a) * range.y, 0.0f);
    }
    else {
        float r = randFloat(0, 1);
        position = CSVector3(CSMath::cos(a) * r * range.x, CSMath::sin(a) * r * range.y, 0.0f);
    }
    CSVector3::normalize(position, direction);
}
