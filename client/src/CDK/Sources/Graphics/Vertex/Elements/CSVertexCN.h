#ifndef __CDK__CSVertexCN__
#define __CDK__CSVertexCN__

#include "CSVertexLayout.h"

#include "CSHalf4.h"

#pragma pack(push, 2)
struct CSVertexCN {
    CSVector3 position;
    CSHalf4 color;
    CSHalf3 normal;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexCN(const CSVector3& position, const CSHalf4& color, const CSHalf3& normal) :
        position(position),
        color(color),
        normal(normal)
    {
    }
    inline CSVertexCN(const CSVector3& position, const CSHalf4& color)
        : CSVertexCN(position, color, CSHalf3::UnitZ)
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexCN& other) const {
        return memcmp(this, &other, sizeof(CSVertexCN)) == 0;
    }
    inline bool operator !=(const CSVertexCN& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif