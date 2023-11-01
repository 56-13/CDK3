#ifndef __CDK__CSVertexTNT__
#define __CDK__CSVertexTNT__

#include "CSVertexLayout.h"

#include "CSHalf3.h"

#pragma pack(push, 2)
struct CSVertexTNT {
    CSVector3 position;
    CSVector2 texCoord;
    CSHalf3 normal;
    CSHalf3 tangent;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexTNT(const CSVector3& position, const CSVector2& texCoord, const CSHalf3& normal, const CSHalf3& tangent) :
        position(position),
        texCoord(texCoord),
        normal(normal),
        tangent(tangent) 
    {
    }
    inline CSVertexTNT(const CSVector3& position, const CSVector2& texCoord)
        : CSVertexTNT(position, texCoord, CSHalf3::UnitZ, CSHalf3::UnitX) 
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexTNT& other) const {
        return memcmp(this, &other, sizeof(CSVertexTNT)) == 0;
    }
    inline bool operator !=(const CSVertexTNT& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif