#ifndef __CDK__CSVertexTN__
#define __CDK__CSVertexTN__

#include "CSVertexLayout.h"

#include "CSHalf3.h"

#pragma pack(push, 2)
struct CSVertexTN {
    CSVector3 position;
    CSVector2 texCoord;
    CSHalf3 normal;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexTN(const CSVector3& position, const CSVector2& texCoord, const CSHalf3& normal) :
        position(position),
        texCoord(texCoord),
        normal(normal)
    {
    }
    inline CSVertexTN(const CSVector3& position, const CSVector2& texCoord)
        : CSVertexTN(position, texCoord, CSHalf3::UnitZ) 
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexTN& other) const {
        return memcmp(this, &other, sizeof(CSVertexTN)) == 0;
    }
    inline bool operator !=(const CSVertexTN& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif