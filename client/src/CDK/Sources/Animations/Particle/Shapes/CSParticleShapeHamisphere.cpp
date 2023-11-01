#define CDK_IMPL

#include "CSParticleShapeHamisphere.h"

#include "CSMatrix.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeHamisphere::CSParticleShapeHamisphere(float range) : range(range) {

}

CSParticleShapeHamisphere::CSParticleShapeHamisphere(CSBuffer* buffer) : range(buffer->readFloat()) {

}

CSParticleShapeHamisphere::CSParticleShapeHamisphere(const CSParticleShapeHamisphere* other) : range(other->range) {

}

void CSParticleShapeHamisphere::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    CSVector3 corners[8];
    CSABoundingBox::getCorners(CSVector3(-range, -range, 0), CSVector3(range), corners);
    for (int i = 0; i < 8; i++) {
        result.append(CSVector3::transformCoordinate(corners[i], transform));
    }
}

void CSParticleShapeHamisphere::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    if (shell) {
        position = CSVector3(0, 0, range);
    }
    else {
        position = CSVector3(0, 0, range * randFloat(0, 1));
    }
    CSMatrix matrix = CSMatrix::rotationYawPitchRoll(randFloat(-FloatPiOverTwo, FloatPiOverTwo), randFloat(-FloatPiOverTwo, FloatPiOverTwo), 0);
    CSVector3::transformCoordinate(position, matrix, position);
    CSVector3::normalize(position, direction);
}
