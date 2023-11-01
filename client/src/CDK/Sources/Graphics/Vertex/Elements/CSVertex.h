#ifndef __CDK__CSVertex__
#define __CDK__CSVertex__

#include "CSVertexC.h"
#include "CSVertexCN.h"
#include "CSVertexCT.h"
#include "CSVertexCTN.h"
#include "CSVertexN.h"
#include "CSVertexT.h"
#include "CSVertexTN.h"
#include "CSVertexTNT.h"
#include "CSFVertex.h"

#pragma pack(push, 2)
struct CSVertex {
    CSVector3 position;
    CSHalf4 color;
    CSVector2 texCoord;
    CSHalf3 normal;
    CSHalf3 tangent;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertex(const CSVector3& position, const CSHalf4& color, const CSVector2& texCoord, const CSHalf3& normal, const CSHalf3& tangent) :
        position(position),
        color(color),
        texCoord(texCoord),
        normal(normal),
        tangent(tangent)
    {
    }
    inline CSVertex(const CSVector3& position)
        : CSVertex(position, CSHalf4::One, CSVector2::Zero, CSHalf3::UnitZ, CSHalf3::UnitX)
    {
    }
    inline CSVertex(const CSVector3& position, const CSHalf4& color)
        : CSVertex(position, color, CSVector2::Zero, CSHalf3::UnitZ, CSHalf3::UnitX)
    {
    }
    inline CSVertex(const CSVector3& position, const CSVector2& texCoord)
        : CSVertex(position, CSHalf4::One, texCoord, CSHalf3::UnitZ, CSHalf3::UnitX)
    {
    }
    inline CSVertex(const CSVector3& position, const CSHalf4& color, const CSVector2& texCoord)
        : CSVertex(position, color, texCoord, CSHalf3::UnitZ, CSHalf3::UnitX)
    {
    }
    inline CSVertex(const CSVector3& position, const CSHalf3& normal, const CSHalf3& tangent)
        : CSVertex(position, CSHalf4::One, CSVector2::Zero, normal, tangent)
    {
    }
    inline CSVertex(const CSVector3& position, const CSHalf4& color, const CSHalf3& normal, const CSHalf3& tangent)
        : CSVertex(position, color, CSVector2::Zero, normal, tangent)
    {
    }
    inline CSVertex(const CSVector3& position, const CSVector2& texCoord, const CSHalf3& normal, const CSHalf3& tangent)
        : CSVertex(position, CSHalf4::One, texCoord, normal, tangent)
    {
    }
    inline CSVertex(const CSFVertex& vertex) :
        position(vertex.position),
        color(vertex.color),
        texCoord(vertex.texCoord),
        normal(vertex.normal),
        tangent(vertex.tangent)
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertex& other) const {
        return memcmp(this, &other, sizeof(CSVertex)) == 0;
    }
    inline bool operator !=(const CSVertex& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif