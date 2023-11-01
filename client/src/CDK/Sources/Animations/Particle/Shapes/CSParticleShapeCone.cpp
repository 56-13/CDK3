#define CDK_IMPL

#include "CSParticleShapeCone.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeCone::CSParticleShapeCone(float topRange, float bottomRange, float height) : topRange(topRange), bottomRange(bottomRange), height(height) {

}

CSParticleShapeCone::CSParticleShapeCone(CSBuffer* buffer) : topRange(buffer->readFloat()), bottomRange(buffer->readFloat()), height(buffer->readFloat()) {

}

CSParticleShapeCone::CSParticleShapeCone(const CSParticleShapeCone* other) : topRange(other->topRange), bottomRange(other->bottomRange), height(other->height) {

}

void CSParticleShapeCone::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    float m = CSMath::max(bottomRange, topRange);

    CSVector3 corners[8];
    CSABoundingBox::getCorners(CSVector3(-m, -m, 0), CSVector3(m, m, height), corners);
    for (int i = 0; i < 8; i++) {
        result.append(CSVector3::transformCoordinate(corners[i], transform));
    }
}

void CSParticleShapeCone::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    float a = randFloat(-FloatPi, FloatPi);
    if (shell) {
        position = CSVector3(CSMath::cos(a), CSMath::sin(a), randFloat(0, 1));
    }
    else {
        float r = randFloat(0, 1);
        position = CSVector3(CSMath::cos(a) * r, CSMath::sin(a) * r, randFloat(0, 1));
    }
    
    CSVector3 top(position.x * topRange, position.y * topRange, height);
    CSVector3 bottom(position.x * bottomRange, position.y * bottomRange, 0);
    CSVector3::normalize(top - bottom, direction);
    float range = CSMath::lerp(bottomRange, topRange, position.z);
    position *= CSVector3(range, range, height);
}
