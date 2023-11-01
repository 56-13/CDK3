#define CDK_IMPL

#include "CSParticleShapeRect.h"

#include "CSBuffer.h"
#include "CSRandom.h"

CSParticleShapeRect::CSParticleShapeRect(const CSVector2& range) : range(range) {

}

CSParticleShapeRect::CSParticleShapeRect(CSBuffer* buffer) : range(buffer) {

}

CSParticleShapeRect::CSParticleShapeRect(const CSParticleShapeRect* other) : range(other->range) {

}

void CSParticleShapeRect::addAABB(const CSMatrix& transform, CSABoundingBox& result) const {
    result.append(CSVector3::transformCoordinate(CSVector3(-range.x, -range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(range.x, -range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(-range.x, range.y, 0), transform));
    result.append(CSVector3::transformCoordinate(CSVector3(range.x, range.y, 0), transform));
}

void CSParticleShapeRect::issue(CSVector3& position, CSVector3& direction, bool shell) const {
    if (shell) {
        bool x = randInt(0, 1);
        bool neg = randInt(0, 1);
        
        if (x) {
            position = CSVector3(neg ? -range.x : range.x, randFloat(-range.y, range.y), 0);
            direction = CSVector3(neg ? -1 : 1, 0, 0);
        }
        else {
            position = CSVector3(randFloat(-range.x, range.x), neg ? -range.y : range.y, 0);
            direction = CSVector3(0, neg ? -1 : 1, 0);
        }
    }
    else {
        position = CSVector3(randFloat(-range.x, range.x), randFloat(-range.y, range.y), 0);
        direction = CSVector3::UnitZ;
    }
}
