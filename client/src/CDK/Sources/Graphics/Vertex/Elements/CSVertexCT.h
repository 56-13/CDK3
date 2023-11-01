#ifndef __CDK__CSVertexCT__
#define __CDK__CSVertexCT__

#include "CSVertexLayout.h"

#include "CSHalf4.h"

#pragma pack(push, 2)
struct CSVertexCT {
    CSVector3 position;
    CSHalf4 color;
    CSVector2 texCoord;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexCT(const CSVector3& position, const CSHalf4& color, const CSVector2& texCoord) :
        position(position),
        color(color),
        texCoord(texCoord)
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexCT& other) const {
        return memcmp(this, &other, sizeof(CSVertexCT)) == 0;
    }
    inline bool operator !=(const CSVertexCT& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif