#ifndef __CDK__CSVertexN__
#define __CDK__CSVertexN__

#include "CSVertexLayout.h"

#include "CSHalf3.h"

#pragma pack(push, 2)
struct CSVertexN {
    CSVector3 position;
    CSHalf3 normal;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexN(const CSVector3& position, const CSHalf3& normal) :
        position(position),
        normal(normal)
    {
    }
    inline CSVertexN(const CSVector3& position)
        : CSVertexN(position, CSHalf3::UnitZ) 
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexN& other) const {
        return memcmp(this, &other, sizeof(CSVertexN)) == 0;
    }
    inline bool operator !=(const CSVertexN& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif