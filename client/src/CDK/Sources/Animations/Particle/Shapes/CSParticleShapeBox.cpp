#define CDK_IMPL

#include "CSParticleShapeBox.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeBox::CSParticleShapeBox(const CSVector3& range) : range(range) {

}

CSParticleShapeBox::CSParticleShapeBox(CSBuffer* buffer) : range(buffer) {

}

CSParticleShapeBox::CSParticleShapeBox(const CSParticleShapeBox* other) : range(other->range) {

}

void CSParticleShapeBox::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    CSVector3 corners[8];
    CSABoundingBox::getCorners(-range, range, corners);
    for (int i = 0; i < 8; i++) {
        result.append(CSVector3::transformCoordinate(corners[i], transform));
    }
}

void CSParticleShapeBox::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    if (shell) {
        int axis = randInt(0, 2);
        bool neg = randInt(0, 1);
        
        switch (axis) {
            case 0:
                position = CSVector3(neg ? -range.x : range.x,
                                     randFloat(-range.y, range.y),
                                     randFloat(-range.z, range.z));
                direction = CSVector3(neg ? -1 : 1, 0, 0);
                break;
            case 1:
                position = CSVector3(randFloat(-range.x, range.x),
                                     neg ? -range.y : range.y,
                                     randFloat(-range.z, range.z));
                direction = CSVector3(0, neg ? -1 : 1, 0);
                break;
            case 2:
                position = CSVector3(randFloat(-range.x, range.x),
                                     randFloat(-range.y, range.y),
                                     neg ? -range.z : range.z);
                direction = CSVector3(0, 0, neg ? -1 : 1);
                break;
        }
    }
    else {
        position = CSVector3(randFloat(-range.x, range.x),
                             randFloat(-range.y, range.y),
                             randFloat(-range.z, range.z));
		direction = CSVector3::UnitZ;
    }
}
