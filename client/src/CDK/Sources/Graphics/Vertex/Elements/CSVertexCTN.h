#ifndef __CDK__CSVertexCTN__
#define __CDK__CSVertexCTN__

#include "CSVertexLayout.h"

#include "CSHalf4.h"

#pragma pack(push, 2)
struct CSVertexCTN {
    CSVector3 position;
    CSHalf4 color;
    CSVector2 texCoord;
    CSHalf3 normal;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexCTN(const CSVector3& position, const CSHalf4& color, const CSVector2& texCoord, const CSHalf3& normal) :
        position(position),
        color(color),
        texCoord(texCoord),
        normal(normal)
    {
    }
    inline CSVertexCTN(const CSVector3& position, const CSHalf4& color, const CSVector2& texCoord)
        : CSVertexCTN(position, color, texCoord, CSHalf3::UnitZ)
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexCTN& other) const {
        return memcmp(this, &other, sizeof(CSVertexCTN)) == 0;
    }
    inline bool operator !=(const CSVertexCTN& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif