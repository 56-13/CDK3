#define CDK_IMPL

#include "CSParticleShapeSphere.h"

#include "CSMatrix.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeSphere::CSParticleShapeSphere(float range) : range(range) {

}

CSParticleShapeSphere::CSParticleShapeSphere(CSBuffer* buffer) : range(buffer->readFloat()) {

}

CSParticleShapeSphere::CSParticleShapeSphere(const CSParticleShapeSphere* other) : range(other->range) {

}

void CSParticleShapeSphere::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    CSVector3 corners[8];
    CSABoundingBox::getCorners(CSVector3(-range), CSVector3(range), corners);
    for (int i = 0; i < 8; i++) {
        result.append(CSVector3::transformCoordinate(corners[i], transform));
    }
}

void CSParticleShapeSphere::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    if (shell) {
        position = CSVector3(0, 0, range);
    }
    else {
        position = CSVector3(0, 0, range * randFloat(0, 1));
    }
    CSMatrix matrix = CSMatrix::rotationYawPitchRoll(randFloat(-FloatPi, FloatPi), randFloat(-FloatPi, FloatPi), 0);
    CSVector3::transformCoordinate(position, matrix, position);
    CSVector3::normalize(position, direction);
}
